#pragma once
#include <QString>
#include <qhostaddress.h>

class ScannerDeviceInformation
{
public:
	ScannerDeviceInformation();
	~ScannerDeviceInformation();

	QString name;
	QHostAddress address;
};
