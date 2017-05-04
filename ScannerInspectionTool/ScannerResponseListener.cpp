#include "ScannerResponseListener.h"
#include <qnetworkdatagram.h>


ScannerResponseListener::ScannerResponseListener()
{
	listenSocket = new QUdpSocket(this);
	listenSocket->bind(listenPort, QUdpSocket::ShareAddress);

	connect(listenSocket, &QUdpSocket::readyRead, this, &ScannerResponseListener::startProcessing);
}


ScannerResponseListener::~ScannerResponseListener()
{
}

void ScannerResponseListener::startProcessing()
{
	while (listenSocket->hasPendingDatagrams()) {
		QNetworkDatagram datagram = listenSocket->receiveDatagram();
		processData(datagram.data());
	}
}

void ScannerResponseListener::processData(QByteArray data)
{
	//todo figure out what data is actually needed
}
