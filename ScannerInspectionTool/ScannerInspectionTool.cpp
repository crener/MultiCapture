#include "ScannerInspectionTool.h"
#include "ScannerResponseListener.h"


ScannerInspectionTool::ScannerInspectionTool(QWidget *parent)
	: QMainWindow(parent)
{
	ui.setupUi(this);
	deviceList = findChild<QListView*>("deviceList");
	deviceScanBtn = findChild<QPushButton*>("deviceScanBtn");

	broadcastSocket = new QUdpSocket(this);

	timer = new QTimer(this);
	timer->start(30000);

	connect(timer, SIGNAL(timeout()), this, SLOT(refresh()));
	connect(deviceScanBtn, SIGNAL(released()), this, SLOT(refresh()));

	setupBroadcastListener();
}

void ScannerInspectionTool::refresh()
{
	broadcastSocket->writeDatagram(datagram.data(), datagram.size(), QHostAddress::Broadcast, brdPort);
}

void ScannerInspectionTool::addNewScanner(ScannerDeviceInformation&)
{
	
}

void ScannerInspectionTool::setupBroadcastListener()
{
	listenerThread = new QThread(this);
	listener = new ScannerResponseListener();
	listenerThread->setObjectName("Listener Thread");
	listener->moveToThread(listenerThread);

	connect(listenerThread, &QThread::started, listener, &ScannerResponseListener::startProcessing);
	connect(listenerThread, &QThread::finished, listener, &QObject::deleteLater);
	connect(listener, &ScannerResponseListener::newScannerFound, this, &ScannerInspectionTool::addNewScanner);

	listenerThread->start();
}
