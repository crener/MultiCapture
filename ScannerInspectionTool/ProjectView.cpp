#include "ProjectView.h"
#include <qtimer.h>
#include "Lib/json.hpp"
#include "Project.h"

ProjectView::ProjectView(QPushButton* refresh, QPushButton* transfer, QTableView* table, ScannerInteraction* connector)
{
	//assign external members
	this->refresh = refresh;
	this->transfer = transfer;
	this->table = table;

	this->connector = connector;

	dataModel = new ProjectTableView();
	table->setModel(dataModel);
	table->setEditTriggers(QAbstractItemView::NoEditTriggers);
	table->resizeColumnsToContents();

	connect(refresh, &QPushButton::clicked, this, &ProjectView::refreshProjects);
}

ProjectView::~ProjectView()
{
	delete refresh;
	delete transfer;
	delete table;

	//delete timer;
}

void ProjectView::respondToScanner(ScannerCommands command, QByteArray data)
{
	switch (command)
	{
	case ScannerCommands::getLoadedProjects:
		processProjects(data);
		break;
	default:
		return;
	}
}

void ProjectView::refreshProjects()
{
	connector->requestScanner(ScannerCommands::getLoadedProjects, "", this);
}

void ProjectView::processProjects(QByteArray data)
{
	nlohmann::json result = nlohmann::json::parse(data.toStdString().c_str());

	dataModel->clearData();

	//collect all the data from the response
	for (int i = 0; i < result.size(); ++i)
	{
		std::string normal = result[i].dump();
		nlohmann::json jsonProject = nlohmann::json::parse(normal.c_str());

		project instance = project();

		instance.id = jsonProject.at("Id").get<int>();
		instance.name = jsonProject.at("Name").get<std::string>();
		instance.imageCount = jsonProject.at("ImageCount").get<int>();
		instance.imagesAvaliable = jsonProject.at("SavedCount").get<int>();

		dataModel->addItem(instance);
	}

	dataModel->updateTable();

	table->resizeColumnsToContents();
	table->resizeRowsToContents();
}
