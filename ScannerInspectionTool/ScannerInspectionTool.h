#pragma once

#include "ui_ScannerInspectionTool.h"
#include <QtWidgets/QMainWindow>
#include <QtNetwork>
#include "ScannerDeviceInformation.h"
#include "ScannerResponseListener.h"

QT_BEGIN_NAMESPACE
class QUdpSocket;
class QPushButton;
class QListView;
class QTimer;
QT_END_NAMESPACE

class ScannerInspectionTool : public QMainWindow
{
	Q_OBJECT

public:
	ScannerInspectionTool(QWidget *parent = Q_NULLPTR);

private slots:
	void refresh();
	void addNewScanner(ScannerDeviceInformation &);

private:
	void setupBroadcastListener();

	const int brdPort = 8470; //broadcast port

	ScannerResponseListener* listener;
	QThread* listenerThread;

	Ui::ScannerInspectionToolClass ui;
	QByteArray datagram = "InspectionApp";
	QTimer* timer;
	QUdpSocket* broadcastSocket;
	QUdpSocket* listenSocket;

	//ui elements
	QListView* deviceList;
	QPushButton* deviceScanBtn;
};
