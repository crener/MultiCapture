#pragma once
#include <QString>

class ScannerDeviceInformation
{
public:
	ScannerDeviceInformation();
	ScannerDeviceInformation(QString name);

private:
	QString name;
};
