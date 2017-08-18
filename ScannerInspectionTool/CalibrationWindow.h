#pragma once
#include "IDeviceResponder.h"
#include "ui_CalibrationWindow.h"
#include "ScannerInteraction.h"
#include "CalibrationListModel.h"
#include "TagPushButton.h"
#include <qthreadpool.h>
#include "Lib/json.hpp"

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
	void newImageTransfered(int setId, int imageId);

	private slots:
	void selctionChanged(QModelIndex index);
	void pairChange(const int &id);
	void splitterChanged(int pos, int index);

private:
	void respondToScanner(ScannerCommands, QByteArray) override;
	QString getProjectJsonString();
	void processCameraPairs(QByteArray data);
	void updateCameraImages();
	void calculateButtonStates();
	void resizePreviews() const;
	void resizeEvent(QResizeEvent *event) override;
	QString getImageName(CalibrationSet* search, int camId) const;

	CalibrationSet* generateCalibrationSet(nlohmann::json json) const;
	void checkImagePairs(CalibrationSet* set) const;
	void generateCalibrationTasks(CalibrationSet* set) const;

	void imageTaskComplete(int, int);
	void imageTaskFailed(int, int);

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
	QThreadPool* workQueue = new QThreadPool(this);

	Ui::CalibrationWindow ui;
	ScannerInteraction* connection;
	QGraphicsView *leftCamView, *rightCamView;
	QListView* imageSets;
	QLayout* pairLayout;
};

