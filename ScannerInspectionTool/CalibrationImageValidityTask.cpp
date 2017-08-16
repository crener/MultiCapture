#include "CalibrationImageValidityTask.h"
#include <QFileDialog>


CalibrationImageValidityTask::CalibrationImageValidityTask(QString path)
{
	this->path = path;
}

CalibrationImageValidityTask::~CalibrationImageValidityTask()
{
}

void CalibrationImageValidityTask::run()
{
	if(!QFile::exists(path))
	{
		emit failed();
		return;
	}



	emit complete();
}
