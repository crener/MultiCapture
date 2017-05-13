#include "ScannerInteraction.h"
#include <QNetworkAccessManager>
#include "ScannerDeviceInformation.h"


ScannerInteraction::ScannerInteraction()
{
	connection = new QTcpSocket();
	connect(connection, &QTcpSocket::connected, this, &ScannerInteraction::scannerConnected);
	connect(connection, static_cast<void(QAbstractSocket::*)(QAbstractSocket::SocketError)>(&QAbstractSocket::error), this, &ScannerInteraction::connectionError);
	connect(connection, &QTcpSocket::disconnected, this, &ScannerInteraction::scannerConnectionLost);
}


ScannerInteraction::~ScannerInteraction()
{
	delete connection;
}

void ScannerInteraction::connectToScanner(ScannerDeviceInformation* device)
{
	connection->connectToHost(device->address, communicationPort);
}

void ScannerInteraction::disconnect()
{
	if (connection->ConnectedState == QAbstractSocket::ConnectedState) {
		connection->disconnectFromHost();
	}
}

void ScannerInteraction::connectionError(QAbstractSocket::SocketError)
{
	//todo log error
	emit scannerConnectionLost();
}
