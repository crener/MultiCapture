#pragma once
#include <QPushButton>
#include "ScannerInteraction.h"
#include <qdir.h>
#include <QStandardItemModel.h>
#include "Lib/json.hpp"
#include <QErrorMessage>

QT_BEGIN_NAMESPACE
class QTreeView;
class QPushButton;
class QLineEdit;
QT_END_NAMESPACE

class projectTransfer : public QObject, public IDeviceResponder
{
	Q_OBJECT

	struct Set;
	struct Image
	{
		int cameraId;
		QString fileName;

		QStandardItem* item;
	};

	struct Set
	{
		int setId;
		QString name;
		QList<Image*>* images = new QList<Image*>();

		QStandardItem* item;
	};

public:
	projectTransfer(QLineEdit*, QPushButton*, QTreeView*, ScannerInteraction*);
	~projectTransfer();

	void respondToScanner(ScannerCommands, QByteArray) override;

	signals:
	void projectChanged();

	public slots:
	void changeTargetProject(int);

	private slots:
	void changeTransferAction();

private:
	void processProjectDetails(QByteArray);
	void setupModelHeadings() const;

	//project detail management
	void modifyExistingProject(nlohmann::json) const;
	void overrideProject(nlohmann::json) const;
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
	int transferSet = 0;
	int transferImage = 0;

	const QIcon ImageTransfered = QIcon(":/ScannerInspectionTool/transferComplete");
	const QIcon ImageNotTransfered = QIcon(":/ScannerInspectionTool/transferNeeded");
	const QIcon Play = QIcon(":/ScannerInspectionTool/play");
	const QIcon Pause = QIcon(":/ScannerInspectionTool/pause");
	
	QLineEdit* path;
	QPushButton* statusControl;
	QTreeView* projectView;
	ScannerInteraction* connector;
};

