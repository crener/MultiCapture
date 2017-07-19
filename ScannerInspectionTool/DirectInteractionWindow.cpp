#include "DirectInteractionWindow.h"
#include "ScannerInteraction.h"
#include "ScannerInspectionTool.h"
#include "parameterBuilder.h"

DirectInteractionWindow::DirectInteractionWindow(QWidget *parent)
	: QWidget(parent)
{
	ui.setupUi(this);

	apiSelection = findChild<QSpinBox*>("apiSelection");
	apiCode = findChild<QLabel*>("apiCode");
	submit = findChild<QPushButton*>("submitBtn");
	apiResponse = findChild<QPlainTextEdit*>("apiResponse");
	parameters = findChild<QLineEdit*>("parameterInput");

	connect(submit, &QPushButton::released, this, &DirectInteractionWindow::makeRequest);
}

DirectInteractionWindow::~DirectInteractionWindow()
{
}

void DirectInteractionWindow::makeRequest()
{
	ScannerCommands command = ScannerCommands(apiSelection->value());
	apiResponse->setPlainText("No Data Recieved");
	emit connection->requestScanner(command, parameters->text(), this);
}

void DirectInteractionWindow::respondToScanner(ScannerCommands command, QByteArray data)
{
	apiCode->setText(QString(std::to_string(static_cast<int>(command)).c_str()));
	apiResponse->setPlainText(data);
}
