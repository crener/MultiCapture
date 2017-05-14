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

void ScannerInteraction::requestScanner(ScannerCommands command, QString params)
{
	if (!connection->isWritable()) return;

	QString data = QString(std::to_string(static_cast<int>(command)).c_str());
	if (params != QString::null && !params.isEmpty()) data += '?' + params;

	connection->write(data.toLatin1());
	
	//QByteArray result = connection->read(returnLengthLimit);
	//emit scannerResult(command, result);
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
