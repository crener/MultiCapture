#pragma once
#include <QPushButton>
#include "ScannerInteraction.h"
#include <qdir.h>
#include "ProjectTransferViewModel.h"

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
	void projectChanged();

	public slots:
	void changeTargetProject(int);

private:
	void processProjectDetails(QByteArray);

	int projectid = -1;
	QDir* transferRoot;
	ProjectTransferViewModel* model;
	
	QLineEdit* path;
	QPushButton* statusControl;
	QTreeView* projectView;
	ScannerInteraction* connector;
};

