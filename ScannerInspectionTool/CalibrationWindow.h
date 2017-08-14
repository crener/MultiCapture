#pragma once
#include "IDeviceResponder.h"
#include "ui_CalibrationWindow.h"
#include "ScannerInteraction.h"
#include "CalibrationListModel.h"

QT_BEGIN_NAMESPACE
class QGraphicsView;
class QListView;
class QLayout;
QT_END_NAMESPACE

class CalibrationWindow : public QWidget, public IDeviceResponder
{
	Q_OBJECT

public:
	CalibrationWindow(QWidget *parent = Q_NULLPTR);
	~CalibrationWindow();

	void setConnection(ScannerInteraction* scanner) { connection = scanner; }

	public slots:
	void projectSelected(QString project);
	void updateProject();

private:
	void respondToScanner(ScannerCommands, QByteArray) override;
	QString getProjectJsonString();

	QString projectPath = "";
	CalibrationListModel* model;

	Ui::CalibrationWindow ui;
	ScannerInteraction* connection;
	QGraphicsView *leftCam, *rightCam;
	QListView* imageSets;
	QLayout* pairSelectionLayout;
};

