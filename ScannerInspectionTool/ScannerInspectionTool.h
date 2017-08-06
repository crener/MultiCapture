#pragma once
#include "ui_ScannerInspectionTool.h"
#include <QtWidgets/QMainWindow>
#include <QtNetwork>
#include "ScannerDeviceInformation.h"
#include "ScannerResponseListener.h"
#include "ScannerInteraction.h"
#include "DirectInteractionWindow.h"
#include "ProjectView.h"
#include "projectTransfer.h"

QT_BEGIN_NAMESPACE
class QUdpSocket;
class QPushButton;
class QListView;
class QAction;
class QLabel;
class QLineEdit;
class QTimer;
class QThread;
QT_END_NAMESPACE

class ScannerInspectionTool : public QMainWindow, public IDeviceResponder
{
	Q_OBJECT

public:
	ScannerInspectionTool(QWidget *parent = Q_NULLPTR);
	~ScannerInspectionTool();

	public slots:
	void addNewScanner(ScannerDeviceInformation*);

	private slots:
	void refreshDevices();
	void selectionChanged() const;
	void setImagePreview(QString) const;

	//buttons
	void handleConnectionBtn();
	void changeScannerName();
	void refreshLogs();

	//scanner interaction
	void connectToScanner();
	void disconnectFromScanner();
	void scannerConnected();
	void scannerDisconnected();
	void respondToScanner(ScannerCommands, QByteArray) override;

	//windows
	void openDirectInteraction();
	void splitterChanged(int pos, int index);

protected:
	void resizeEvent(QResizeEvent *event) override;

private:
	void setupBroadcastListener();
	void setupProjectView();
	void setupProjectTransfer();
	void clearScanners();
	void updateLogView(QByteArray);
	void refreshLogs(bool);
	void refreshImagePreview() const;

	const int brdPort = 8470; //broadcast port

	ScannerResponseListener* listener;
	ScannerInteraction* connector;
	ProjectView* projects;
	projectTransfer* transfer;
	QThread* listenerThread;
	QStringList* scannerItems;
	std::list<ScannerDeviceInformation*> scanners;

	DirectInteractionWindow* directWn;
	QAction* DirectInteractionBtn;

	Ui::ScannerInspectionToolClass ui;
	QByteArray datagram = "InspectionApp";
	QTimer* timer;
	QUdpSocket* broadcastSocket;
	QUdpSocket* listenSocket;
	QGraphicsScene* scene;
	bool connected = false;

	//ui elements
	QListView* deviceList;
	QPushButton* deviceScanBtn;
	QPushButton* deviceConnectBtn;
	QLineEdit* nameText;
	QPushButton* nameBtn;
	QLabel* currentLbl;
	QGraphicsView* imgPreview;

	QPushButton* logRefresh;
	QListView* logView;
	QStringList* logData;
};
