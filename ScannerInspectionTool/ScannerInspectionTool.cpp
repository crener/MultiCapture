#include "ScannerInspectionTool.h"


ScannerInspectionTool::ScannerInspectionTool(QWidget *parent)
	: QMainWindow(parent)
{
	ui.setupUi(this);
	deviceList = findChild<QListView*>("deviceList");
	deviceScanBtn = findChild<QPushButton*>("deviceScanBtn");

	broadcastSocket = new QUdpSocket(this);
	listenSocket = new QUdpSocket(this);

	timer = new QTimer(this);
	timer->start(30000);

	connect(timer, SIGNAL(timeout()), this, SLOT(refresh()));
	connect(deviceScanBtn, SIGNAL(released()), this, SLOT(refresh()));
}

void ScannerInspectionTool::refresh()
{
	broadcastSocket->writeDatagram(datagram.data(), datagram.size(), QHostAddress::Broadcast, brdPort);
}
