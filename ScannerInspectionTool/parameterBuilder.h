#pragma once
#include <QString>
class QString;

class parameterBuilder
{
public:
	parameterBuilder();

	parameterBuilder* addParam(QString key, QString value);
	QString toString();

private:
	static const char seperator = '&';

	QString parameters = "";
};

