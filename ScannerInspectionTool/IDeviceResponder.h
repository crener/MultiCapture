#pragma once

enum class ScannerCommands;
class QByteArray;

class IDeviceResponder
{
signals:
	virtual void respondToScanner(ScannerCommands, QByteArray) = 0;
};
