#include "projectTransfer.h"
#include "ScannerInteraction.h"
#include "parameterBuilder.h"
#include "Lib/json.hpp"
#include <QTreeView>
#include <QInputDialog>
#include <qdir.h>
#include <QMessageBox>
#include <QTimer>
#include <QFileDialog>
#include <QModelIndex>


projectTransfer::projectTransfer(QLineEdit* path, QPushButton* statusBtn, QTreeView* project, ScannerInteraction* connector)
{
	this->path = path;
	this->connector = connector;
	statusControl = statusBtn;
	projectView = project;

	transferRoot = new QDir();
	projectError = new QErrorMessage();
	projectError->setWindowTitle("Project Transfer Error");

	timer = new QTimer(this);
	timer->setInterval(60000);

	model = new QStandardItemModel(project);
	setupModelHeadings();
	project->setModel(model);

	connect(statusControl, &QPushButton::clicked, this, &projectTransfer::changeTransferAction);
	connect(connector, &ScannerInteraction::scannerConnected, this, &projectTransfer::newScannerConnection);
	connect(timer, &QTimer::timeout, this, &projectTransfer::timerReset);
	connect(project, &QTreeView::clicked, this, &projectTransfer::changeImagePreview);
}


projectTransfer::~projectTransfer()
{
	delete transferRoot;
	setData->clear();
	delete setData;
	delete projectError;
	delete timer;
}

void projectTransfer::respondToScanner(ScannerCommands command, QByteArray data)
{
	switch (command)
	{
	case ScannerCommands::ProjectDetails:
		processProjectDetails(data);
		break;
	case ScannerCommands::ImageSetImageData:
		continueTransfer(data);
		break;
	case ScannerCommands::CurrentProject:
		currentScanner(data);
		break;
	default:
		return;
	}
}

void projectTransfer::changeTargetProject(int project)
{
	if (project == projectId || transfering) return;
	if (project == currentProject) timer->start();
	else timer->stop();

	bool done = transferRoot->exists(path->text());
	QString newPath = path->text();

	//check the path the user entered is expected and correct
	while (!done) {
		//ensure correct path
		do {
			newPath = QFileDialog::getExistingDirectory(path, tr("Transfer root directory"),
				"/home", QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks);

			if (newPath.isEmpty() && newPath.isNull()) return;
		} while (!transferRoot->exists(newPath));

		transferRoot->setPath(newPath);
		if (transferRoot->entryList(QDir::AllEntries | QDir::NoDotAndDotDot).size() != 0)
		{
			//there are some files here, warn user
			QMessageBox::StandardButton reply = QMessageBox::question(path, "Are You Sure?",
				"There are some files in this directory, they might be modified. Are you sure you would like to use this directory to transfer projects?",
				QMessageBox::Yes | QMessageBox::No);

			if (reply == QMessageBox::Yes) done = true;
		}
		else done = true;
	}

	path->setText(newPath);
	projectId = project;
	initialLoad = true;
	emit projectChanged();

	emit connector->requestScanner(ScannerCommands::ProjectDetails,
		parameterBuilder().addParam("id", QString::number(project))->toString(), this);
}

void projectTransfer::changeTransferAction()
{
	resumeRequired = false;

	if (transfering)
	{
		transfering = false;
		statusControl->setIcon(Play);
		path->setEnabled(true);
	}
	else
	{
		if (transferSet >= setData->size() ||
			transferImage >= setData->at(transferSet)->images->size()) return;

		transfering = true;
		statusControl->setIcon(Pause);
		path->setEnabled(false);

		QString param = parameterBuilder().addParam("id", QString::number(projectId))
			->addParam("set", QString::number(setData->at(transferSet)->setId))
			->addParam("image", QString::number(setData->at(transferSet)->images->at(transferImage)->cameraId))
			->toString();

		emit connector->requestScanner(ScannerCommands::ImageSetImageData, param, this);
	}
}

void projectTransfer::newScannerConnection()
{
	emit connector->requestScanner(ScannerCommands::CurrentProject, "", this);
}

void projectTransfer::timerReset()
{
	emit connector->requestScanner(ScannerCommands::ProjectDetails,
		parameterBuilder().addParam("id", QString::number(projectId))->toString(), this);

	if (projectId == currentProject) timer->start();
}

void projectTransfer::changeImagePreview(const QModelIndex& index)
{
	QModelIndex parent = model->parent(index);
	if (!parent.isValid()) return;

	QVariant selectText = model->data(index);
	QVariant parentText = model->data(parent);

	Set* selectedSet = nullptr;
	for (int i = 0; i < setData->size(); ++i)
		if (setData->at(i)->name == parentText.toString())
		{
			selectedSet = setData->at(i);
			break;
		}
	if (selectedSet == nullptr) return;

	Image* selectedImage = nullptr;
	for (int i = 0; i < selectedSet->images->size(); ++i)
		if (selectedSet->images->at(i)->fileName == selectText.toString())
		{
			selectedImage = selectedSet->images->at(i);
			break;
		}
	if (selectedImage == nullptr) return;

	emit triggerImagePreview(this->path->text() + "/" + QString::number(projectId) + "/" + selectedSet->name + "/" + selectedImage->fileName);
}

void projectTransfer::processProjectDetails(QByteArray data)
{
	QString response = QString(data);
	if (response.startsWith("Fail")) return;

	QString directory = transferRoot->path() + "/" + QString::number(projectId);
	if (!QDir(directory).exists()) QDir().mkdir(directory);

	QString projectFilePath = directory + "/project.scan";
	QFile projectFile(projectFilePath);

	try {
		projectFile.open(QIODevice::WriteOnly);
		projectFile.write(data);
		projectFile.close();
	}
	catch (std::exception) {}
	if (projectFile.isOpen()) projectFile.close();

	//extract image details
	nlohmann::json result = nlohmann::json::parse(response.toStdString().c_str());

	if (result["ProjectId"] == projectId && !initialLoad)
	{
		//must be a potential update to the project
		updateProjectTree(result);

		if (transfering && resumeRequired && transferSet < setData->size())
		{
			Set* set = setData->at(transferSet);
			resumeRequired = false;

			QString param = parameterBuilder().addParam("id", QString::number(projectId))
				->addParam("set", QString::number(set->setId))
				->addParam("image", QString::number(set->images->at(transferImage)->cameraId))
				->toString();

			emit connector->requestScanner(ScannerCommands::ImageSetImageData, param, this);
		}
	}
	else
	{
		//clear the old project information and regnerate the new stuff
		model->clear();
		setupModelHeadings();

		resetProjectTree(result);

		initalTransferSetup();
		initialLoad = false;
	}
}

void projectTransfer::setupModelHeadings() const
{
	model->setColumnCount(2);

	QStringList labels = QStringList();
	labels.append("Name");
	labels.append("ID");
	model->setHorizontalHeaderLabels(labels);
}

void projectTransfer::currentScanner(QByteArray data)
{
	QString result = QString(data);
	if (result.startsWith("Fail")) projectError->showMessage(result.mid(result.indexOf("?") + 1));

	currentProject = result.toInt();
}

void projectTransfer::continueTransfer(QByteArray data)
{
	QString response = QString(data);

	if (response.startsWith("Fail"))
		projectError->showMessage(response.mid(response.indexOf("?") + 1));
	else
	{
		QString dirPath = path->text() + "/" + QString::number(projectId) + "/" + setData->at(transferSet)->name;
		if (!QDir().exists(dirPath)) QDir().mkdir(dirPath);

		QString savePath = dirPath + "/" + setData->at(transferSet)->images->at(transferImage)->fileName;
		QFile projectFile(savePath);

		try {
			projectFile.open(QIODevice::WriteOnly);
			projectFile.write(data);
			projectFile.close();
		}
		catch (std::exception) {}
		if (projectFile.isOpen()) projectFile.close();

		if (fileExists(savePath))
		{
			setData->at(transferSet)->images->at(transferImage)->item->setIcon(ImageTransfered);

			//check if all images have been transfered and update icons
			bool allTransfered = true;
			for (int j = 0; j < setData->at(transferSet)->images->size(); ++j)
			{
				QString path = this->path->text() + "/" + QString::number(projectId) + "/" + setData->at(transferSet)->name + "/" + setData->at(transferSet)->images->at(j)->fileName;
				if (!fileExists(path)) allTransfered = false;
			}

			if (allTransfered) setData->at(transferSet)->item->setIcon(ImageTransfered);
		}
		else setData->at(transferSet)->images->at(transferImage)->item->setIcon(ImageNotTransfered);
	}

	//setup the next request
	if (transfering) resumeTransferRequest();
}

void projectTransfer::initalTransferSetup()
{
	QString basePath = path->text() + "/" + QString::number(projectId) + "/";

	for (int i = 0; i < setData->size(); ++i)
	{
		QString setPath = basePath + setData->at(i)->name + "/";

		for (int j = 0; j < setData->at(i)->images->size(); ++j)
		{
			QString imgPath = setPath + setData->at(i)->images->at(j)->fileName;
			if (!fileExists(imgPath))
			{
				transferSet = i;
				transferImage = j;
				return;
			}
		}
	}

	transferSet = setData->size();
}

void projectTransfer::resumeTransferRequest()
{
	if (transferSet < setData->size())
	{
		QString projectDir = path->text() + "/" + QString::number(projectId) + "/";

		do {
			if (transferImage < setData->at(transferSet)->images->size() - 1)
				transferImage++;
			else
			{
				transferSet++;
				transferImage = 0;
			}
		} while (transferSet < setData->size() &&
			fileExists(projectDir + setData->at(transferSet)->name + "/" + setData->at(transferSet)->images->at(transferImage)->fileName));

		//dont stop transfering if the current  
		if (transferSet >= setData->size() && currentProject != projectId) changeTransferAction();
		else if (transferSet >= setData->size() && currentProject == projectId) resumeRequired = true;
		else {
			Set* set = setData->at(transferSet);
			QString param = parameterBuilder().addParam("id", QString::number(projectId))
				->addParam("set", QString::number(set->setId))
				->addParam("image", QString::number(set->images->at(transferImage)->cameraId))
				->toString();

			emit connector->requestScanner(ScannerCommands::ImageSetImageData, param, this);
		}
	}
	else if (currentProject == projectId) resumeRequired = true;
	else changeTransferAction();
}

void projectTransfer::populateIcons() const
{
	for (int i = 0; i < setData->size(); ++i)
	{
		Set* set = setData->at(i);
		QModelIndex setIndex = model->index(i, 0);
		bool complete = true;

		for (int j = 0; j < set->images->size(); ++j)
		{
			QString path = this->path->text() + "/" + QString::number(projectId) + "/" + set->name + "/" + set->images->at(j)->fileName;

			if (fileExists(path))
			{
				set->images->at(j)->item->setIcon(ImageTransfered);
			}
			else
			{
				complete = false;
				set->images->at(j)->item->setIcon(ImageNotTransfered);
			}
		}

		if (complete) set->item->setIcon(ImageTransfered);
		else set->item->setIcon(ImageNotTransfered);
	}
}

void projectTransfer::generateImageSetModel(int row) const
{
	Set* set = setData->at(row);

	QStandardItem* name = new QStandardItem(set->name);
	QStandardItem* id = new QStandardItem(QString::number(set->setId));
	model->appendRow(name);
	set->item = name;
	model->setItem(row, 1, id);

	for (int i = 0; i < set->images->size(); ++i)
	{
		Image* img = set->images->at(i);

		QStandardItem* imgName = new QStandardItem(img->fileName);
		name->setChild(i, 0, imgName);
		img->item = imgName;

		QStandardItem* imgId = new QStandardItem(QString::number(img->cameraId));
		name->setChild(i, 1, imgId);
	}
}

bool projectTransfer::imageSetExists(int setId) const
{
	for (int i = 0; i < setData->size(); ++i)
		if (setData->at(i)->setId == setId) return true;
	return false;
}

bool projectTransfer::fileExists(QString path)
{
	return QFile().exists(path);
}

void projectTransfer::updateProjectTree(nlohmann::json json) const
{
	nlohmann::json imageSets = json["ImageSets"];
	for (int i = 0; i < imageSets.size(); ++i)
	{
		nlohmann::json imageSetJson = imageSets[i];

		if (imageSetExists(imageSetJson["id"])) continue;

		Set* newSet = new Set();
		newSet->setId = imageSetJson["id"];
		newSet->name = QString::fromStdString(imageSetJson["path"]);

		nlohmann::json data = imageSetJson["images"];
		for (int j = 0; j < data.size(); ++j)
		{
			Image* img = new Image();
			img->fileName = QString::fromStdString(data[j]["path"]);
			img->cameraId = data[j]["id"];

			newSet->images->append(img);
		}

		setData->append(newSet);
		generateImageSetModel(setData->size() - 1);
	}

	populateIcons();
}

void projectTransfer::resetProjectTree(nlohmann::json json) const
{
	setData->clear();

	nlohmann::json imageSets = json["ImageSets"];
	for (int i = 0; i < imageSets.size(); ++i)
	{
		nlohmann::json imageSetJson = imageSets[i];

		Set* newSet = new Set();
		newSet->setId = imageSetJson["id"];
		newSet->name = QString::fromStdString(imageSetJson["path"]);

		nlohmann::json data = imageSetJson["images"];
		for (int j = 0; j < data.size(); ++j)
		{
			Image* img = new Image();
			img->fileName = QString::fromStdString(data[j]["path"]);
			img->cameraId = data[j]["id"];

			newSet->images->append(img);
		}

		setData->append(newSet);
		generateImageSetModel(setData->size() - 1);
	}

	populateIcons();
}
