#pragma once
#include <QList>

class ImageSet
{
public:
	struct Image
	{
		QString fileName;
		int cameraId;
	};

	ImageSet(std::string, QString, int);
	~ImageSet();

	QList<Image> images = QList<Image>();
	int id;
	QString name;

private:
	
};

