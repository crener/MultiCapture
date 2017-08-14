#include "CalibrationWindow.h"
#include <QFileDialog>
#include "Lib/json.hpp"
#include "JsonTypes.h"


CalibrationWindow::CalibrationWindow(QWidget *parent) : QWidget(parent)
{
	ui.setupUi(this);

	leftCam = findChild<QGraphicsView*>("LeftCamera");
	rightCam = findChild<QGraphicsView*>("RightCamera");
	pairSelectionLayout = findChild<QLayout*>("CameraPairSelection");
	imageSets = findChild<QListView*>("imageSets");

	model = new CalibrationListModel();
	imageSets->setModel(model);
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

		CalibrationSet* newSet = new CalibrationSet();
		newSet->setId = imageSetJson["id"];
		newSet->name = QString::fromStdString(imageSetJson["path"]);

		if(model->containsSet(newSet->setId))
		{
			delete newSet;
			return;
		}

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

void CalibrationWindow::respondToScanner(ScannerCommands command, QByteArray data)
{
	switch (command)
	{
	case ScannerCommands::ProjectDetails:
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
