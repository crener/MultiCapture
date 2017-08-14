#pragma once
#include <QStandardItemModel.h>

struct Set;
struct Image
{
	int cameraId;
	QString fileName;

	QStandardItem* item;
};

struct Set
{
	int setId;
	QString name;
	QList<Image*>* images = new QList<Image*>();

	QStandardItem* item;
};

enum CalibrationValidity
{
	Pending,
	Valid,
	Invalid
};

struct CalibrationImage
{
	int cameraId;
	QString fileName;
	CalibrationValidity valid = Pending;
};
struct CalibrationSet
{
	int setId;
	QString name;
	QList<CalibrationImage*>* images = new QList<CalibrationImage*>();
	CalibrationValidity valid = Pending;
};