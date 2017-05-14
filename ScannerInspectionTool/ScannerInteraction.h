#pragma once
#include <QObject>
#include <qtcpsocket.h>

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
	void requestScanner(ScannerCommands, QString);
	void disconnect();

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
	//Global Commands
	setName = 10,
	setProjectNiceName = 11,
	getRecentLogFile = 12,
	getLoadedProjects = 13,
	getCameraConfiguration = 14,
	getCapacity = 15,

	//Camera Commands
	captureImageSet = 20,

	//Project Management Commands
	removeProject = 30,
	getAllImageSets = 31,
	getImageSet = 32,
	getProjectStats = 33,

};