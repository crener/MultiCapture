#pragma once
#include <QObject>
#include <qudpsocket.h>

class ScannerDeviceInformation;
QT_BEGIN_NAMESPACE
class QUdpSocket;
QT_END_NAMESPACE

class ScannerResponseListener : public QObject
{
	Q_OBJECT
public:
	ScannerResponseListener();
	~ScannerResponseListener();

public slots:
	void startProcessing();

signals:
	void newScannerFound(ScannerDeviceInformation &);

private:
	void processData(QByteArray);

	bool keepRunning = true;
	QUdpSocket* listenSocket;

	const int maxDataSize = 500;
	const qint16 listenPort = 8471;
};

