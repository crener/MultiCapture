#pragma once
#include "GenericTreeItem.h"

class GenericTreeRootItem : public GenericTreeItem
{
public:
	GenericTreeRootItem();
	~GenericTreeRootItem();

	QVariant data(int column) override;
	int columnCount() override;
};

