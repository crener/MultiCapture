#pragma once
#include <qrunnable.h>
#include <qobjectdefs.h>
#include <QString>
#include <QObject>

class CalibrationImageValidityTask : public QObject, public QRunnable
{
	Q_OBJECT

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

