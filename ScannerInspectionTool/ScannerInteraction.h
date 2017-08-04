#pragma once
#include <QObject>
#include <qtcpsocket.h>
#include "IDeviceResponder.h"

enum class ScannerCommands;
class ScannerDeviceInformation;

class ScannerInteraction : public QObject
{
	Q_OBJECT

public:
	ScannerInteraction();
	~ScannerInteraction();


	public slots:
	void connectToScanner(ScannerDeviceInformation* device);
	//void requestScanner(ScannerCommands, QString);
	void requestScanner(ScannerCommands command, QString params, IDeviceResponder* responder);
	void disconnect();

	bool isConnected() { return connection->isWritable(); }

signals:
	void scannerConnected();
	void scannerConnectionLost();
	void scannerResult(ScannerCommands, QByteArray);

	private slots:
	void connectionError(QAbstractSocket::SocketError);

private:
	QTcpSocket* connection;

	const qint16 communicationPort = 8472;
	const qint64 returnLengthLimit = 128000; //128Kb
};

enum class ScannerCommands
{
	Unknown = 0,
	ApiCompatability = 1,

	//Global Commands
	setName = 100,
	getRecentLogFile = 120,
	getRecentLogDiff = 121,
	getLoadedProjects = 130,
	getCameraConfiguration = 140,
	getCapacity = 150,
	getApiVersion = 180,

	//Camera Commands
	CaptureImageSet = 200,

	//Project Management Commands
	RemoveProject = 300,
	getAllImageSets = 310,
	ImageSetMetaData = 320,
	ImageSetImageData = 321,
	ProjectDetails = 330,
	CurrentProject = 331,
	setProjectNiceName = 350,
};