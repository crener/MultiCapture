#pragma once
#include <qrunnable.h>
#include <qobjectdefs.h>
#include <QString>

class CalibrationImageValidityTask : public QRunnable
{
public:
	CalibrationImageValidityTask(QString path);
	~CalibrationImageValidityTask();

	void run() override;

	signals:
	void complete();
	void failed();

private:
	QString path;
};

