#pragma once
#include <QVariant>

class GenericTreeItem
{
public:
	GenericTreeItem(GenericTreeItem *parentItem = 0);
	virtual ~GenericTreeItem();

	void appendChild(GenericTreeItem *child);

	virtual QVariant data(int column) = 0;
	virtual int columnCount() = 0;

	virtual int row() const;

	int childCount() const;
	GenericTreeItem *child(int row);
	GenericTreeItem *parentItem();

	void clearItems();

protected:
	QList<GenericTreeItem*> childItems = QList<GenericTreeItem*>();
	GenericTreeItem *parent = nullptr;
};

