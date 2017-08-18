#pragma once
#include <qrunnable.h>
#include <qobjectdefs.h>
#include <QObject>
#include "opencv2/calib3d.hpp"
#include "Lib/json.hpp"

using namespace cv;
using namespace nlohmann;

class CalibrationImageValidityTask : public QObject, public QRunnable
{
	Q_OBJECT

public:
	CalibrationImageValidityTask(QString,QString, QString, int, int);
	~CalibrationImageValidityTask();

	void run() override;

	signals:
	void complete(int, int);
	void failed(int, int);

private:
	const Size board = Size(9, 6);

	QString path, saveRoot, fileName;
	int set, img;
};