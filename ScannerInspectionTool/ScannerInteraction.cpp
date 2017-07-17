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

void ScannerInteraction::requestScanner(ScannerCommands command, QString params, IDeviceResponder* responder)
{
	if (!connection->isWritable()) return;

	QString data = QString(std::to_string(static_cast<int>(command)).c_str());
	data += params;

	connection->write(data.toLatin1());

	connection->waitForReadyRead(-1);
	QByteArray result = connection->read(returnLengthLimit);

	//extract message details
	bool ok = true;
	QString resultPrefix = result.mid(0, result.indexOf(">"));
	int length = resultPrefix.mid(resultPrefix.indexOf(":") + 1).toInt(&ok);

	if (ok)
	{
		//initialise new array
		int progress = 0;
		QByteArray correct = QByteArray(length, 'NUL');
		{
			int preLen = resultPrefix.length() + 1;
			for (int i = preLen; i < result.length(); ++i, ++progress)
				correct[i - preLen] = result[i];
		}
		while (progress < correct.length())
		{
			connection->waitForReadyRead(20000);
			if (!connection->isReadable()) break;
			QByteArray temp = connection->read(returnLengthLimit);

			if (progress + temp.length() > correct.length())
			{
				//either there is another command in there or something is wrong
				break;
			}

			for (int i = 0; i < temp.length(); ++i, ++progress)
				correct[progress] = temp[i];
		}

		result = correct;
	}
	else result = result.mid(result.indexOf(">") + 1);

	//emit scannerResult(command, result);
	responder->respondToScanner(command, result);
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
