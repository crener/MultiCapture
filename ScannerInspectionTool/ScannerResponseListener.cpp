#include "ScannerResponseListener.h"
#include <qnetworkdatagram.h>
#include "ScannerDeviceInformation.h"
#include "ScannerInspectionTool.h"


ScannerResponseListener::ScannerResponseListener()
{
	listenSocket = new QUdpSocket(this);
	listenSocket->bind(listenPort, QUdpSocket::ShareAddress);

	connect(listenSocket, &QUdpSocket::readyRead, this, &ScannerResponseListener::startProcessing);
}


ScannerResponseListener::~ScannerResponseListener()
{
	delete listenSocket;
}

void ScannerResponseListener::startProcessing()
{
	while (listenSocket->hasPendingDatagrams()) {
		processData(listenSocket->receiveDatagram());
	}
}

void ScannerResponseListener::processData(QNetworkDatagram data)
{
	ScannerDeviceInformation* device = new ScannerDeviceInformation();
	device->name = data.data();
	device->address = data.senderAddress();

	emit newScannerFound(device);
}
