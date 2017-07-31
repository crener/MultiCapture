#include "GenericTreeViewModel.h"
#include <QStringList>

GenericTreeViewModel::GenericTreeViewModel(GenericTreeItem* rootItem, QObject *parent)
	: QAbstractItemModel(parent)
{
	this->rootItem = rootItem;
}

GenericTreeViewModel::~GenericTreeViewModel()
{
	delete rootItem;
}

int GenericTreeViewModel::columnCount(const QModelIndex &parent) const
{
	if (parent.isValid())
		return static_cast<GenericTreeItem*>(parent.internalPointer())->columnCount();
	else
		return rootItem->columnCount();
}

QVariant GenericTreeViewModel::data(const QModelIndex &index, int role) const
{
	if (!index.isValid())
		return QVariant();

	if (role != Qt::DisplayRole)
		return QVariant();

	GenericTreeItem *item = static_cast<GenericTreeItem*>(index.internalPointer());

	return item->data(index.column());
}

Qt::ItemFlags GenericTreeViewModel::flags(const QModelIndex &index) const
{
	if (!index.isValid())
		return 0;

	return QAbstractItemModel::flags(index);
}

QVariant GenericTreeViewModel::headerData(int section, Qt::Orientation orientation,
	int role) const
{
	if (orientation == Qt::Horizontal && role == Qt::DisplayRole)
		return rootItem->data(section);

	return QVariant();
}

QModelIndex GenericTreeViewModel::index(int row, int column, const QModelIndex &parent)
const
{
	if (!hasIndex(row, column, parent))
		return QModelIndex();

	GenericTreeItem *parentItem;

	if (!parent.isValid())
		parentItem = rootItem;
	else
		parentItem = static_cast<GenericTreeItem*>(parent.internalPointer());

	GenericTreeItem *childItem = parentItem->child(row);
	if (childItem)
		return createIndex(row, column, childItem);
	else
		return QModelIndex();
}

QModelIndex GenericTreeViewModel::parent(const QModelIndex &index) const
{
	if (!index.isValid())
		return QModelIndex();

	GenericTreeItem *childItem = static_cast<GenericTreeItem*>(index.internalPointer());
	GenericTreeItem *parentItem = childItem->parentItem();

	if (parentItem == rootItem)
		return QModelIndex();

	return createIndex(parentItem->row(), 0, parentItem);
}

void GenericTreeViewModel::addItem(GenericTreeItem* newItem, GenericTreeItem* parent)
{
	GenericTreeItem* gen;
	if (parent) gen = parent;
	else gen = rootItem;

	QModelIndex index;
	if (parent) index = createIndex(gen->row(), 0, gen);
	else index = createIndex(gen->row(), 0);

	beginInsertRows(index, gen->childCount(), gen->childCount());

	gen->appendChild(newItem);

	endInsertRows();
}

void GenericTreeViewModel::clearData()
{
	beginResetModel();

	rootItem->clearItems();

	endResetModel();
}

int GenericTreeViewModel::rowCount(const QModelIndex &parent) const
{
	GenericTreeItem *parentItem;
	if (parent.column() > 0)
		return 0;

	if (!parent.isValid() || typeid(parent.internalPointer()) != typeid(GenericTreeItem*) )
		parentItem = rootItem;
	else
		parentItem = static_cast<GenericTreeItem*>(parent.internalPointer());

	return parentItem->childCount();
}
