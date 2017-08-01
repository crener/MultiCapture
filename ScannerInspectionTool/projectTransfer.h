#pragma once
#include <QPushButton>
#include "ScannerInteraction.h"
#include <qdir.h>
#include "ImageSetViewModel.h"

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
	ImageSetViewModel* model;
	
	QLineEdit* path;
	QPushButton* statusControl;
	QTreeView* projectView;
	ScannerInteraction* connector;
};

