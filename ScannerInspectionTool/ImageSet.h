#pragma once
#include "GenericTreeItem.h"

class Image;

class ImageSet : public GenericTreeItem
{
public:
	explicit ImageSet(const std::string, QString, int, GenericTreeItem* root = 0);
	~ImageSet();

	QVariant data(int column) override;
	int columnCount() override;

private:
	QString setName;
	int setId;
	bool transfered = false;
};
