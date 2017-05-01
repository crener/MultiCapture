#pragma once

#include "ui_ScannerInspectionTool.h"
#include <QtWidgets/QMainWindow>
#include <QtNetwork>

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

private:

	const int brdPort = 8470; //broadcast port

	Ui::ScannerInspectionToolClass ui;
	QByteArray datagram = "InspectionApp";
	QTimer* timer;
	QUdpSocket* broadcastSocket;
	QUdpSocket* listenSocket;

	QListView* deviceList;
	QPushButton* deviceScanBtn;
};
