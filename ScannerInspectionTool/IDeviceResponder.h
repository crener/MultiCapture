#pragma once

enum class ScannerCommands;
class QByteArray;

class IDeviceResponder
{
public:
	virtual void respondToScanner(ScannerCommands, QByteArray) = 0;
};
