#include "ScannerInspectionTool.h"
#include "ScannerResponseListener.h"
#include <iostream>


ScannerInspectionTool::ScannerInspectionTool(QWidget *parent)
	: QMainWindow(parent)
{
	ui.setupUi(this);

	deviceList = findChild<QListView*>("deviceList");
	deviceScanBtn = findChild<QPushButton*>("deviceScanBtn");
	broadcastSocket = new QUdpSocket(this);

	scannerItems = new QStringList();
	deviceList->setModel(new QStringListModel(*scannerItems));

	timer = new QTimer(this);
	timer->start(30000);

	connect(timer, SIGNAL(timeout()), this, SLOT(refresh()));
	connect(deviceScanBtn, SIGNAL(released()), this, SLOT(refresh()));
	emit refresh();

	setupBroadcastListener();
}

ScannerInspectionTool::~ScannerInspectionTool()
{
	delete broadcastSocket;
	delete scannerItems;
	delete timer;

	for (int i = scanners.size() - 1; i >= 0; --i) {
		ScannerDeviceInformation* remove = scanners.back();
		scanners.remove(remove);
		delete remove;
	}
}

void ScannerInspectionTool::refresh()
{
	broadcastSocket->writeDatagram(datagram.data(), datagram.size(), QHostAddress::Broadcast, brdPort);
}

void ScannerInspectionTool::addNewScanner(ScannerDeviceInformation* scanner)
{
	//check if the scanner is already tracked in the list
	bool exists = false;

	std::list<ScannerDeviceInformation*>::iterator it;
	for (it = scanners.begin(); it != scanners.end(); ++it) {
		if ((*it)->address.isEqual(scanner->address)) {
			exists = true;
			break;
		}
	}

	if (exists)
	{
		//scanner already exists
		delete scanner;
		return;
	}

	scanners.push_back(scanner);
	scannerItems->append(scanner->name);
	static_cast<QStringListModel*>(deviceList->model())->setStringList(*scannerItems);
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
