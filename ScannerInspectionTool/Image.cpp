#include "Image.h"


Image::Image(int cam, QString name, GenericTreeItem* parent) : GenericTreeItem(parent)
{
	fileName = name;
	camId = cam;
}

Image::~Image()
{
}

QVariant Image::data(int column)
{
	if (column == 1) return fileName;
	else QVariant(camId);
}

int Image::columnCount()
{
	return 2;
}
