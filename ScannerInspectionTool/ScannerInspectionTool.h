#pragma once

#include "ui_ScannerInspectionTool.h"
#include <QtWidgets/QMainWindow>
#include <QtNetwork>
#include "ScannerDeviceInformation.h"
#include "ScannerResponseListener.h"
#include "ScannerInteraction.h"

QT_BEGIN_NAMESPACE
class QUdpSocket;
class QPushButton;
class QListView;
class QTimer;
class QThread;
QT_END_NAMESPACE

class ScannerInspectionTool : public QMainWindow
{
	Q_OBJECT

public:
	ScannerInspectionTool(QWidget *parent = Q_NULLPTR);
	~ScannerInspectionTool();

	public slots:
	void addNewScanner(ScannerDeviceInformation*);

	private slots:
	void refresh();
	void selectionChanged();
	void handleConnectionBtn();
	void connectToScanner();
	void disconnectFromScanner();
	void scannerConnected();
	void scannerDisconnected();

private:
	void setupBroadcastListener();

	const int brdPort = 8470; //broadcast port

	ScannerResponseListener* listener;
	ScannerInteraction* connector;
	QThread* listenerThread;
	QStringList* scannerItems;
	std::list<ScannerDeviceInformation*> scanners;

	Ui::ScannerInspectionToolClass ui;
	QByteArray datagram = "InspectionApp";
	QTimer* timer;
	QUdpSocket* broadcastSocket;
	QUdpSocket* listenSocket;
	bool connected = false;

	//ui elements
	QListView* deviceList;
	QPushButton* deviceScanBtn;
	QPushButton* deviceConnectBtn;
};
