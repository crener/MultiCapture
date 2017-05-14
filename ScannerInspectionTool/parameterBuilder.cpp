#include "parameterBuilder.h"
#include <QString>

parameterBuilder::parameterBuilder()
{
}

parameterBuilder* parameterBuilder::addParam(QString key, QString value)
{
	if (key.contains(seperator, Qt::CaseSensitive)) return this;
	if (value.contains(seperator, Qt::CaseSensitive)) return this;

	parameters.append(seperator).append(key).append("=").append(value);
	return this;
}

QString parameterBuilder::toString()
{
	return parameters;
}
