#pragma once
#include <QPushButton>
#include "ScannerInteraction.h"
#include <qdir.h>
#include <QStandardItemModel.h>
#include "Lib/json.hpp"
#include <QErrorMessage>
#include "JsonTypes.h"

QT_BEGIN_NAMESPACE
class QTreeView;
class QPushButton;
class QLineEdit;
QT_END_NAMESPACE

class projectTransfer : public QObject, public IDeviceResponder
{
	Q_OBJECT

public:
	projectTransfer(QLineEdit*, QPushButton*, QTreeView*, ScannerInteraction*);
	~projectTransfer();

	void respondToScanner(ScannerCommands, QByteArray) override;

	signals:
	void projectChanged(QString path);
	void triggerImagePreview(QString);
	void newProjectImageDetected();
	void imageTransfered(int setId, int imageId);

	public slots:
	void changeTargetProject(int);

	private slots:
	void changeTransferAction();
	void newScannerConnection();
	void timerReset();
	void changeImagePreview(const QModelIndex&);

private:
	void processProjectDetails(QByteArray);
	void setupModelHeadings() const;
	void currentScanner(QByteArray);

	//project detail management
	void updateProjectTree(nlohmann::json) const;
	void resetProjectTree(nlohmann::json) const;
	void generateImageSetModel(int row) const;
	bool imageSetExists(int setId) const;

	//transfer methods
	static bool fileExists(QString path);
	void populateIcons() const;
	void continueTransfer(QByteArray data);
	void initalTransferSetup();
	void resumeTransferRequest();

	int projectId = -1;
	QDir* transferRoot;
	QStandardItemModel* model;
	QList<Set*>* setData = new QList<Set*>();
	QErrorMessage* projectError;

	bool transfering = false;
	bool resumeRequired = false;
	bool initialLoad = true;
	int transferSet = 0;
	int transferImage = 0;
	int currentProject = -1;
	QTimer* timer;

	const QIcon ImageTransfered = QIcon(":/ScannerInspectionTool/transferComplete");
	const QIcon ImageNotTransfered = QIcon(":/ScannerInspectionTool/transferNeeded");
	const QIcon Play = QIcon(":/ScannerInspectionTool/play");
	const QIcon Pause = QIcon(":/ScannerInspectionTool/pause");
	
	QLineEdit* path;
	QPushButton* statusControl;
	QTreeView* projectView;
	ScannerInteraction* connector;
};

