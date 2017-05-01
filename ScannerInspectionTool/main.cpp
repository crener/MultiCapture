#include "ScannerInspectionTool.h"
#include <QtWidgets/QApplication>

int main(int argc, char *argv[])
{
	QApplication a(argc, argv);
	ScannerInspectionTool w;
	w.show();
	return a.exec();
}
