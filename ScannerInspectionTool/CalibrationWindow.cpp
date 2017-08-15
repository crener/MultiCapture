#include "CalibrationWindow.h"
#include <QFileDialog>
#include "Lib/json.hpp"
#include "JsonTypes.h"
#include <set>
#include <QGraphicsPixmapItem>


CalibrationWindow::CalibrationWindow(QWidget *parent) : QWidget(parent)
{
	ui.setupUi(this);

	leftCamView = findChild<QGraphicsView*>("LeftCamera");
	rightCamView = findChild<QGraphicsView*>("RightCamera");
	pairLayout = findChild<QLayout*>("CameraPairSelection");
	imageSets = findChild<QListView*>("imageSets");

	model = new CalibrationListModel();
	imageSets->setModel(model);
	connect(imageSets, &QListView::activated, this, &CalibrationWindow::selctionChanged);

	spacer = new QSpacerItem(40, 20, QSizePolicy::Expanding);
	pairLayout->addItem(spacer);

	leftCam = new QGraphicsScene();
	leftCamView->setScene(leftCam);

	rightCam = new QGraphicsScene();
	rightCamView->setScene(rightCam);
}


CalibrationWindow::~CalibrationWindow()
{
	delete model;
}

void CalibrationWindow::projectSelected(QString project)
{
	projectPath = project;

	nlohmann::json json = nlohmann::json::parse(getProjectJsonString().toStdString().c_str());
	for (int i = 0; i < json["ImageSets"].size(); ++i) {
		nlohmann::json imageSetJson = json["ImageSets"][i];

		CalibrationSet* newSet = new CalibrationSet();
		newSet->setId = imageSetJson["id"];
		newSet->name = QString::fromStdString(imageSetJson["path"]);
		for (int i = 0; i < cameras->size(); ++i) newSet->pairs->push_back(Pending);

		nlohmann::json data = imageSetJson["images"];
		for (int j = 0; j < data.size(); ++j)
		{
			CalibrationImage* img = new CalibrationImage();
			img->fileName = QString::fromStdString(data[j]["path"]);
			img->cameraId = data[j]["id"];

			newSet->images->append(img);
		}

		model->addItem(newSet);
	}
}

void CalibrationWindow::updateProject()
{
	nlohmann::json json = nlohmann::json::parse(getProjectJsonString().toStdString().c_str());

	if (json["ImageSets"].size() <= model->rowCount()) return;

	for (int i = 0; i < json["ImageSets"].size(); ++i) {
		nlohmann::json imageSetJson = json["ImageSets"][i];

		int setId = imageSetJson["id"];
		if (model->containsSet(setId)) continue;

		CalibrationSet* newSet = new CalibrationSet();
		newSet->setId = setId;
		newSet->name = QString::fromStdString(imageSetJson["path"]);
		for (int j = 0; j < cameras->size(); ++j) newSet->pairs->push_back(Pending);

		nlohmann::json data = imageSetJson["images"];
		for (int j = 0; j < data.size(); ++j)
		{
			CalibrationImage* img = new CalibrationImage();
			img->fileName = QString::fromStdString(data[j]["path"]);
			img->cameraId = data[j]["id"];

			newSet->images->append(img);
		}

		model->addItem(newSet);
	}
}

void CalibrationWindow::scannerConnected()
{
	emit connection->requestScanner(ScannerCommands::CameraPairs, "", this);
}

void CalibrationWindow::scannerDisconnected()
{
	cameras->clear();
	model->clearData();

	activeSet = nullptr;
	activePair = -1;

	for (int i = 0; i < buttons->size(); ++i)
	{
		pairLayout->removeWidget(buttons->at(i));
		delete buttons->at(i);
	}
	buttons->clear();
}

void CalibrationWindow::selctionChanged(QModelIndex index)
{
	activeSet = model->getSet(index.row());

	//check images exist for each pair
	QString imagePath = projectPath + "/" + activeSet->name + "/";
	for (int i = 0; i < cameras->size(); ++i)
	{
		QString leftName = nullptr, rightName = nullptr;

		for (int j = 0; j < activeSet->images->size(); ++j)
		{
			if (activeSet->images->at(j)->cameraId == cameras->at(i).leftId)
				leftName = activeSet->images->at(j)->fileName;
			else if (activeSet->images->at(j)->cameraId == cameras->at(i).rightId)
				rightName = activeSet->images->at(j)->fileName;

			if (leftName != nullptr && rightName != nullptr) break;
		}

		if (leftName != nullptr && rightName != nullptr)buttons->at(i)->setEnabled(false);
		else activeSet->pairs->at(i) = CalibrationValidity::Missing;
	}

	//try to find an image pair to display
	activePair = -1;
	for (int i = 0; i < buttons->size(); ++i)
	{
		if (buttons->at(i)->isEnabled())
		{
			activePair = i;
			break;
		}
	}

	updateCameraImages();
}

void CalibrationWindow::cameraChange(const int& id)
{
}

void CalibrationWindow::respondToScanner(ScannerCommands command, QByteArray data)
{
	switch (command)
	{
	case ScannerCommands::CameraPairs:
		processCameraPairs(data);
		break;
	}
}

QString CalibrationWindow::getProjectJsonString()
{
	QFile text(projectPath + "/" + "project.scan");
	QString jsonText = "";

	try {
		if (text.open(QIODevice::ReadOnly))
		{
			jsonText = text.readAll();
			text.close();
		}
	}
	catch (std::exception) {}
	if (text.isOpen()) text.close();

	return jsonText;
}

void CalibrationWindow::processCameraPairs(QByteArray data)
{
	QString jsonText = data;
	if (jsonText.startsWith("Fail")) return;
	if (cameras->size() > 0) cameras->clear();

	nlohmann::json pairs = nlohmann::json::parse(jsonText.toStdString().c_str());
	for (int i = 0; i < pairs.size(); ++i)
	{
		CameraPair pair = CameraPair();
		pair.leftId = pairs.at(i)["LeftCamera"];
		pair.rightId = pairs.at(i)["RightCamera"];

		cameras->push_back(pair);
	}

	//button setup
	pairLayout->removeItem(spacer);

	for (int i = 0; i < cameras->size(); ++i)
	{
		CameraPair pair = cameras->at(i);
		TagPushButton* newButton;
		if (buttons->size() < i) newButton = buttons->at(i);
		else {
			newButton = new TagPushButton(this);
			newButton->setMinimumWidth(30);
			newButton->setPalette(calibrationPending);
			if (activeSet == nullptr || activePair == -1) newButton->setEnabled(false);

			pairLayout->addWidget(newButton);
			buttons->push_back(newButton);
		}

		QString text = QString::number(pair.leftId) + " - " + QString::number(pair.rightId);
		newButton->setText(text);
		newButton->setTag(i);

		connect(newButton, &TagPushButton::triggered, this, &CalibrationWindow::cameraChange);
	}

	//remove all unused buttons
	while (buttons->size() > cameras->size())
	{
		TagPushButton* remove = buttons->back();
		pairLayout->removeWidget(remove);
		delete remove;
		buttons->erase(buttons->end());
	}

	pairLayout->addItem(spacer);
}

void CalibrationWindow::updateCameraImages()
{
	if (activePair == -1 || activePair > cameras->size())
	{
		leftCam->clear();
		leftCam->addText("No image pair in set");
		leftCamView->show();

		rightCam->clear();
		rightCam->addText("No image pair in set");
		rightCamView->show();
		return;
	}

	QString leftName = nullptr, rightName = nullptr;

	for (int j = 0; j < activeSet->images->size(); ++j)
	{
		if (activeSet->images->at(j)->cameraId == cameras->at(activePair).leftId)
			leftName = activeSet->images->at(j)->fileName;
		else if (activeSet->images->at(j)->cameraId == cameras->at(activePair).rightId)
			rightName = activeSet->images->at(j)->fileName;

		if (leftName != nullptr && rightName != nullptr) break;
	}

	if (leftCam->items().size() > 0) leftCam->clear();
	leftCam->addItem(new QGraphicsPixmapItem(QPixmap::fromImage(QImage(projectPath + "/" + activeSet->name + "/" + leftName))));
	leftCamView->fitInView(leftCam->itemsBoundingRect(), Qt::KeepAspectRatio);
	leftCamView->show();


	if (rightCam->items().size() > 0)rightCam->clear();
	rightCam->addItem(new QGraphicsPixmapItem(QPixmap::fromImage(QImage(projectPath + "/" + activeSet->name + "/" + rightName))));
	rightCamView->fitInView(rightCam->itemsBoundingRect(), Qt::KeepAspectRatio);
	rightCamView->show();

}
