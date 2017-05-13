#pragma once
#include <QObject>
#include <qtcpsocket.h>

class ScannerDeviceInformation;

class ScannerInteraction : public QObject
{
	Q_OBJECT

public:
	ScannerInteraction();
	~ScannerInteraction();

	public slots:
	void connectToScanner(ScannerDeviceInformation* device);
	void disconnect();

signals:
	void scannerConnected();
	void scannerConnectionLost();

	private slots:
	void connectionError(QAbstractSocket::SocketError);

private:
	QTcpSocket* connection;

	const qint16 communicationPort = 8472;
};

