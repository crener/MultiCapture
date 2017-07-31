#include "ImageSet.h"
#include "Lib/json.hpp"
#include <QStringList>
#include "Image.h"

ImageSet::ImageSet(const std::string json, QString name, int setId, GenericTreeItem* root)
	: GenericTreeItem(root)
{
	this->setName = name;
	this->setId = setId;

	//extract required image data from json
	{
		nlohmann::json data = nlohmann::json::parse(json.c_str());
		for (int i = 0; i < data.size(); ++i)
		{
			int id = data[i]["id"];
			QString file = QString::fromStdString(data[i]["path"]);

			Image* img = new Image(id, file, this);
			appendChild(img);
		}
	}
}

ImageSet::~ImageSet()
{
}

QVariant ImageSet::data(int column)
{
	if (column == 0) return setName;
	else return setId;
}

int ImageSet::columnCount()
{
	return 2;
}
