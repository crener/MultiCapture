#include "GenericTreeRootItem.h"



GenericTreeRootItem::GenericTreeRootItem()
{
}


GenericTreeRootItem::~GenericTreeRootItem()
{
}

QVariant GenericTreeRootItem::data(int column)
{
	if (column == 0) return "Name";
	else return "Set";
}

int GenericTreeRootItem::columnCount()
{
	return 0;
}
