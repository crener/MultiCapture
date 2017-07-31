#include "projectTransfer.h"
#include "ScannerInteraction.h"
#include "parameterBuilder.h"
#include "Lib/json.hpp"
#include <QTreeView>
#include <QInputDialog>
#include <qdir.h>
#include <QMessageBox>
#include <QFileDialog>
#include "ImageSet.h"
#include "GenericTreeViewModel.h"
#include "GenericTreeRootItem.h"


projectTransfer::projectTransfer(QLineEdit* path, QPushButton* statusBtn, QTreeView* project, ScannerInteraction* connector)
{
	this->path = path;
	this->connector = connector;
	statusControl = statusBtn;
	projectView = project;

	transferRoot = new QDir();
	
	GenericTreeRootItem* root = new GenericTreeRootItem();
	model = new GenericTreeViewModel(root);
	project->setModel(model);
}


projectTransfer::~projectTransfer()
{
}

void projectTransfer::respondToScanner(ScannerCommands command, QByteArray data)
{
	switch (command)
	{
	case ScannerCommands::ProjectDetails:
		processProjectDetails(data);
	default:
		return;
	}
}

void projectTransfer::changeTargetProject(int project)
{
	if (project == projectid) return;

	bool done = transferRoot->exists(path->text());
	QString newPath = path->text();

	//check the path the user entered is expected and correct
	while (!done) {
		//ensure correct path
		do {
			newPath = QFileDialog::getExistingDirectory(path, tr("Transfer root directory"),
				"/home", QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks);
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
	projectid = project;
	emit projectChanged();

	connector->requestScanner(ScannerCommands::ProjectDetails,
		parameterBuilder().addParam("id", QString::number(project))->toString(), this);
}

void projectTransfer::processProjectDetails(QByteArray data)
{
	QString directory = transferRoot->path() + "/" + QString::number(projectid);

	if (!QDir(directory).exists()) QDir().mkdir(directory);

	QString projectFilePath = directory + "/project.scan";
	QFile projectFile(projectFilePath);

	try {
		projectFile.open(QIODevice::WriteOnly);
		projectFile.write(data);
		projectFile.close();
	}
	catch (std::exception) {}

	model->clearData();

	//extract image details
	{
		nlohmann::json result = nlohmann::json::parse(data.toStdString().c_str());
		nlohmann::json imageSets = result["ImageSets"];
		for (int i = 0; i < imageSets.size(); ++i)
		{
			nlohmann::json imageSetJson = imageSets[i];

			int id = imageSetJson["id"];
			std::string name = imageSetJson["path"];

			std::string json = imageSetJson["images"].dump();
			ImageSet* set = new ImageSet(json, QString::fromStdString(name), id);

			model->addItem(set);
			//bool added = model->addItem(set);
			//if (!added) delete set;
		}
	}
}
