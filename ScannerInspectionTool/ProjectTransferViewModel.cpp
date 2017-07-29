#include "ProjectTransferViewModel.h"



ProjectTransferViewModel::ProjectTransferViewModel()
{
	displayData = new QList<ImageSet>();
}


ProjectTransferViewModel::~ProjectTransferViewModel()
{
	displayData->clear();
	delete displayData;
}

int ProjectTransferViewModel::rowCount(const QModelIndex& parent) const
{
	return displayData->size();
}

int ProjectTransferViewModel::columnCount(const QModelIndex& parent) const
{
	return 2;
}

QVariant ProjectTransferViewModel::data(const QModelIndex& index, int role) const
{
	return displayData->at(index.row()).name;
}

QVariant ProjectTransferViewModel::headerData(int section, Qt::Orientation orientation, int role) const
{
	return QVariant("Name");
}

QModelIndex ProjectTransferViewModel::index(int row, int column, const QModelIndex& parent) const
{
	QModelIndex index = createIndex(row, column);
	return index;
}

QModelIndex ProjectTransferViewModel::parent(const QModelIndex& index) const
{
	if (index.column() <= 0) return QModelIndex();

	return QModelIndex();
}

Qt::ItemFlags ProjectTransferViewModel::flags(const QModelIndex& index) const
{
	return Qt::ItemIsEnabled | Qt::ItemIsSelectable;
}

void ProjectTransferViewModel::clearData()
{
	beginResetModel();
	displayData->clear();
	endResetModel();
}

void ProjectTransferViewModel::addImageSet(ImageSet set)
{
	if (containsImageSet(set.id)) {
		int rows = displayData->size();

		beginInsertRows(QModelIndex() , rows, rows);
		displayData->append(set);
		endInsertRows();
	}
}

bool ProjectTransferViewModel::containsImageSet(int project)
{
	for (int i = 0; i < displayData->size(); ++i)
	{
		if (displayData->at(i).id == project) return true;
	}
	return false;
}
