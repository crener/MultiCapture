#pragma once
#include "ImageSet.h"

class Image : public GenericTreeItem
{
public:
	Image(int, QString, GenericTreeItem*);
	~Image();

	QVariant data(int column) override;
	int columnCount() override;

private:
	int camId;
	QString fileName;
	bool transfered = false;
};