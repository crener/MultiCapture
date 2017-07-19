#pragma once

#include "ui_DirectInteractionWindow.h"
#include "IDeviceResponder.h"

QT_BEGIN_NAMESPACE
class QSpinBox;
class QPlainTextEdit;
class ScannerInteraction;
QT_END_NAMESPACE

class DirectInteractionWindow : public QWidget, public IDeviceResponder
{
	Q_OBJECT

public:
	DirectInteractionWindow(QWidget *parent = Q_NULLPTR);
	~DirectInteractionWindow();

	void setConnection(ScannerInteraction* scanner) { connection = scanner; }

	public slots:
	void makeRequest();

private:
	Ui::DirectInteractionWindow ui;

	QSpinBox* apiSelection;
	QLabel* apiCode;
	QPushButton* submit;
	QPlainTextEdit* apiResponse;
	QLineEdit* parameters;

	ScannerInteraction* connection;


	void respondToScanner(ScannerCommands, QByteArray) override;
};
