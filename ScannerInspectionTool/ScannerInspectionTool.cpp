#include "ScannerInspectionTool.h"
#include "ScannerResponseListener.h"
#include <iostream>
#include <QMessageBox>
#include <QLabel>
#include "ScannerInteraction.h"
#include "parameterBuilder.h"


ScannerInspectionTool::ScannerInspectionTool(QWidget *parent)
	: QMainWindow(parent)
{
	ui.setupUi(this);

	deviceList = findChild<QListView*>("deviceList");
	deviceList->setSelectionMode(QAbstractItemView::SingleSelection);
	deviceScanBtn = findChild<QPushButton*>("deviceScanBtn");
	deviceConnectBtn = findChild<QPushButton*>("deviceConnectBtn");
	currentLbl = findChild<QLabel*>("currentLbl");
	broadcastSocket = new QUdpSocket(this);

	nameText = findChild<QLineEdit*>("nameText");
	nameBtn = findChild<QPushButton*>("nameUpdateBtn");

	scannerItems = new QStringList();
	deviceList->setModel(new QStringListModel(*scannerItems));

	timer = new QTimer(this);
	timer->start(30000);

	connect(timer, SIGNAL(timeout()), this, SLOT(refresh()));
	connect(deviceList, &QListView::clicked, this, &ScannerInspectionTool::selectionChanged);
	connect(deviceList, &QListView::doubleClicked, this, &ScannerInspectionTool::handleConnectionBtn);
	connect(deviceConnectBtn, &QPushButton::released, this, &ScannerInspectionTool::handleConnectionBtn);
	connect(deviceScanBtn, SIGNAL(released()), this, SLOT(refresh()));
	connect(nameBtn, &QPushButton::released, this, &ScannerInspectionTool::changeScannerName);

	setupBroadcastListener();
	emit refresh();
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

	//remove threaded items
	listenerThread->quit();
	listenerThread->wait();
	delete listenerThread;
	//delete listenSocket;
	//delete connector;
}

void ScannerInspectionTool::refresh()
{
	clearScanners();

	broadcastSocket->writeDatagram(datagram.data(), datagram.size(), QHostAddress::Broadcast, brdPort);
}

void ScannerInspectionTool::selectionChanged()
{
	if (!connected)
	{
		deviceConnectBtn->setDisabled(false);
	}
}

void ScannerInspectionTool::handleConnectionBtn()
{
	if (connected)
	{
		//disconnect, show confirmation message
		QMessageBox msgBox;
		msgBox.setWindowTitle("Disconnect?");
		msgBox.setIcon(QMessageBox::Question);
		msgBox.setInformativeText("Are you sure you want to disconnect?");
		msgBox.setStandardButtons(QMessageBox::No | QMessageBox::Yes);
		msgBox.setDefaultButton(QMessageBox::Yes);
		int ret = msgBox.exec();

		switch (ret) {
		case QMessageBox::No:
			return;
		case QMessageBox::Yes:
			disconnectFromScanner();
			break;
		default:
			//do nothing 
			break;
		}
	}
	else
	{
		connectToScanner();
	}
}

void ScannerInspectionTool::connectToScanner()
{
	QModelIndex current_index = deviceList->selectionModel()->currentIndex();
	QString scannerName = current_index.data(Qt::DisplayRole).toString();

	//find the network address for this device
	ScannerDeviceInformation* deviceInformation = nullptr;
	std::list<ScannerDeviceInformation*>::iterator it;
	for (it = scanners.begin(); it != scanners.end(); ++it) {
		if ((*it)->name == scannerName) {
			deviceInformation = *it;
			break;
		}
	}

	if (deviceInformation == nullptr) return;

	emit connector->connectToScanner(deviceInformation);
	nameText->setText(deviceInformation->name);
}

void ScannerInspectionTool::disconnectFromScanner()
{
	emit connector->disconnect();
}

void ScannerInspectionTool::scannerConnected()
{
	deviceConnectBtn->setText("Disconnect");
	currentLbl->setText("Connected");
	currentLbl->setStyleSheet("color: rgb(0, 128, 0);");

	connected = true;
}

void ScannerInspectionTool::scannerDisconnected()
{
	deviceConnectBtn->setText("Connected");
	currentLbl->setText("Disconnected");
	currentLbl->setStyleSheet("color: rgb(237, 20, 61);");

	connected = false;
}

void ScannerInspectionTool::changeScannerName()
{
	if (!connected) return;

	emit connector->requestScanner(ScannerCommands::setName,
		parameterBuilder().addParam("name", nameText->text())->toString());

	clearScanners();
	emit refresh();
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
	connector = new ScannerInteraction();
	connector->moveToThread(listenerThread);

	connect(listenerThread, &QThread::started, listener, &ScannerResponseListener::startProcessing);
	connect(listenerThread, &QThread::finished, listener, &QObject::deleteLater);
	connect(listener, &ScannerResponseListener::newScannerFound, this, &ScannerInspectionTool::addNewScanner);
	connect(connector, &ScannerInteraction::scannerConnected, this, &ScannerInspectionTool::scannerConnected);
	connect(connector, &ScannerInteraction::scannerConnectionLost, this, &ScannerInspectionTool::scannerDisconnected);

	listenerThread->start();
}

void ScannerInspectionTool::clearScanners()
{
	for (int i = scanners.size() - 1; i >= 0; --i) {
		ScannerDeviceInformation* remove = scanners.back();
		scanners.remove(remove);
		delete remove;
	}

	scannerItems->clear();
}