#pragma once
#include "IDeviceResponder.h"
#include "ui_CalibrationWindow.h"
#include "ScannerInteraction.h"
#include "CalibrationListModel.h"
#include "TagPushButton.h"

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
	void scannerConnected();
	void scannerDisconnected();

	private slots:
	void selctionChanged(QModelIndex index);
	void cameraChange(const int &id);

private:
	void respondToScanner(ScannerCommands, QByteArray) override;
	QString getProjectJsonString();
	void processCameraPairs(QByteArray data);
	void updateCameraImages();

	CalibrationListModel* model;
	QSpacerItem* spacer;
	const QPalette calibrationValid = QPalette(QColor(24, 185, 119));
	const QPalette calibrationInvalid = QPalette(QColor(253, 91, 93));
	const QPalette calibrationPending = QPalette();
	QGraphicsScene *leftCam, *rightCam;

	QString projectPath = "";
	std::vector<CameraPair>* cameras = new std::vector<CameraPair>;
	std::vector<TagPushButton*>* buttons = new std::vector<TagPushButton*>;

	CalibrationSet* activeSet = nullptr;
	int activePair = -1;

	Ui::CalibrationWindow ui;
	ScannerInteraction* connection;
	QGraphicsView *leftCamView, *rightCamView;
	QListView* imageSets;
	QLayout* pairLayout;
};

