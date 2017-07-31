#include "ProjectTransferViewModel.h"


/*
ProjectTransferViewModel::ProjectTransferViewModel()
{
	displayData = new QList<ImageSet*>();
}


ProjectTransferViewModel::~ProjectTransferViewModel()
{
	displayData->clear();
	delete displayData;
}

int ProjectTransferViewModel::rowCount(const QModelIndex& parent) const
{
	if (parent.isValid())
	{
		if (typeid(parent.internalPointer()) == typeid(ImageSet))
		{
			ImageSet* set = static_cast<ImageSet*>(parent.internalPointer());
			return set->row();
		}

		return 0;
	}
	else return displayData->size();
}

int ProjectTransferViewModel::columnCount(const QModelIndex& parent) const
{
	return 2;
}

QVariant ProjectTransferViewModel::data(const QModelIndex& index, int role) const
{
	if (role == Qt::EditRole) return false;
	if (role == Qt::DisplayRole)
	{
		//must be an image
		if (index.isValid() && typeid(index.internalPointer()) == typeid(Image))
		{
			Image* data = static_cast<Image*>(index.internalPointer());
			return data->data(index.column());
		}
		else displayData->at(index.row())->data(index.column());
	}

	return QVariant();
}

QVariant ProjectTransferViewModel::headerData(int section, Qt::Orientation orientation, int role) const
{
	if (role == Qt::DisplayRole)
		switch (section)
		{
		case 0:
			return "Name";
		case 1:
			return "";
		default:
			return "Unknown";
		}
	if (role == Qt::EditRole) return false;

	return QVariant();
}

QModelIndex ProjectTransferViewModel::index(int row, int column, const QModelIndex& parent) const
{
	if (!hasIndex(row, column, parent))
		return QModelIndex();

	if (!parent.isValid())
	{
		//must be trying to access a root item
		ImageSet* set = displayData->at(row);
		return createIndex(row, column, set);
	}

	if (typeid(parent.internalPointer()) == typeid(ImageSet))
	{
		ImageSet* set = static_cast<ImageSet*>(parent.internalPointer());
		return createIndex(row, column, set);
	}
	else if (typeid(parent.internalPointer()) == typeid(Image))
	{
		Image* set = static_cast<Image*>(parent.internalPointer());
		return createIndex(row, column, set);
	}

	return QModelIndex();
}

QModelIndex ProjectTransferViewModel::parent(const QModelIndex& index) const
{
	if (!index.isValid()) return QModelIndex();

	//figure out the type of the index
	if (typeid(index.internalPointer()) == typeid(ImageSet))
	{
		return QModelIndex();
	}
	else if (typeid(index.internalPointer()) == typeid(Image))
	{
		Image* set = static_cast<Image*>(index.internalPointer());
		return createIndex(index.row(), index.column(), set->getParentItem());
	}

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

bool ProjectTransferViewModel::addImageSet(ImageSet* set)
{
	if (!containsImageSet(set->id())) {
		int rows = displayData->size();

		set->bindChildren(set);

		beginInsertRows(QModelIndex(), rows, rows);

		displayData->append(set);
		endInsertRows();

		return true;
	}
	return false;
}

bool ProjectTransferViewModel::containsImageSet(int project) const
{
	for (int i = 0; i < displayData->size(); ++i)
	{
		if (displayData->at(i)->id() == project) return true;
	}
	return false;
}
*/