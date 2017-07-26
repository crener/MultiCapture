#pragma once
#include <QPushButton>
#include <QTableWidget>
#include "ScannerInteraction.h"
#include "Lib/json.hpp"
#include "ProjectTableView.h"

QT_BEGIN_NAMESPACE
class QPushButton;
class QTableWidget;
class QTimer;
QT_END_NAMESPACE

class ProjectView : public QObject, public IDeviceResponder
{
public:
	ProjectView(QPushButton*, QPushButton*, QTableView*, ScannerInteraction*);
	~ProjectView();

	void respondToScanner(ScannerCommands, QByteArray) override;

	public slots:
	void refreshProjects();

private:
	const int timerDuration = 30000; //30sec

	ScannerInteraction* connector;
	ProjectTableView* dataModel;

	void processProjects(QByteArray);

	//ui elements
	QPushButton* refresh, *transfer;
	QTableView* table;
};