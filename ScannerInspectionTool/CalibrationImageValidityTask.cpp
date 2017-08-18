#include "CalibrationImageValidityTask.h"
#include <opencv2/opencv.hpp>
#include "opencv2/imgcodecs.hpp"
#include <QFileDialog>
#include "Lib/json.hpp"


using namespace std;

CalibrationImageValidityTask::CalibrationImageValidityTask(QString loadPath, QString savePath, QString imageName, int setId, int imgId)
{
	path = loadPath;
	this->saveRoot = savePath;
	fileName = imageName;

	set = setId;
	img = imgId;
}

CalibrationImageValidityTask::~CalibrationImageValidityTask()
{
}

void CalibrationImageValidityTask::run()
{
	if (!QFile::exists(path))
	{
		emit failed(set, img);
		return;
	}

	Mat image = imread(path.toStdString(), 1); // 0 = greayscale, 1 = colour
	if (image.empty())
	{
		emit failed(set, img);
		return;
	}

	vector<Point2f>& corners = vector<Point2f>();

	bool found = findChessboardCorners(image, board, corners,
		CALIB_CB_ADAPTIVE_THRESH | CALIB_CB_NORMALIZE_IMAGE);
	if (!found)
	{
		emit failed(set, img);
		return;
	}

	//save image
	vector<int> compression_params;
	compression_params.push_back(CV_IMWRITE_JPEG_QUALITY);
	compression_params.push_back(100);
	drawChessboardCorners(image, board, corners, found);
	imwrite((saveRoot + fileName).toStdString(), image);

	//save points
	json save;
	for (int i = 0; i < corners.size(); ++i)
	{
		json point = { { "x", corners.at(i).x },{ "y", corners.at(i).y } };
		save["points"].push_back(point);
	}
	string jsonString = save.dump();

	QString resultSavePath = saveRoot + fileName.section('.', 0, 0) + ".conf";
	QFile calibrationFile(resultSavePath);
	try {
		calibrationFile.open(QIODevice::WriteOnly);
		calibrationFile.write(QString::fromStdString(jsonString).toUtf8());
		calibrationFile.close();
	}
	catch (std::exception) {}
	if (calibrationFile.isOpen()) calibrationFile.close();

	emit complete(set, img);
}
