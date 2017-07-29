#include "ImageSet.h"
#include "Lib/json.hpp"


ImageSet::ImageSet(std::string json, QString name, int setId)
{
	this->name = name;
	id = setId;

	//extract required image data from json
	{
		nlohmann::json data = nlohmann::json::parse(json.c_str());
		for (int i = 0; i < data.size(); ++i)
		{
			Image img;
			img.cameraId = data[i]["id"];
			img.fileName = QString::fromStdString(data[i]["path"]);

			images.append(img);
		}
	}
}

ImageSet::~ImageSet()
{
}
