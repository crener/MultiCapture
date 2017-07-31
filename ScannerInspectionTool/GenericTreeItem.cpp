#include "GenericTreeItem.h"
#include <QStringList>

GenericTreeItem::GenericTreeItem(GenericTreeItem *parent)
{
	parent = parent;
}

GenericTreeItem::~GenericTreeItem()
{
	qDeleteAll(childItems);
}

void GenericTreeItem::appendChild(GenericTreeItem *item)
{
	childItems.append(item);
}

GenericTreeItem *GenericTreeItem::child(int row)
{
	return childItems.value(row);
}

int GenericTreeItem::childCount() const
{
	return childItems.count();
}

GenericTreeItem *GenericTreeItem::parentItem()
{
	return parent;
}

void GenericTreeItem::clearItems()
{
	childItems.clear();
}

int GenericTreeItem::row() const
{
	if (parent)
		return parent->childItems.indexOf(const_cast<GenericTreeItem*>(this));

	return 0;
}
