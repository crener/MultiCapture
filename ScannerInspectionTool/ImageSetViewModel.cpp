#include "ImageSetViewModel.h"
#include "Lib/json.hpp"


ImageSetViewModel::ImageSetViewModel()
{
}


ImageSetViewModel::~ImageSetViewModel()
{
}

int ImageSetViewModel::rowCount(const QModelIndex& parent) const
{
	if (parent.column() > 0)
		return 0;

	if(typeid(parent.internalPointer()) == typeid(Set*))
	{
		Set* set = static_cast<Set*>(parent.internalPointer());
		return set->images->size();
	}

	//amount of items in set
	if (parent.row() >= 0)
		return displayData->at(parent.row())->images->size();

	return displayData->size();
}

int ImageSetViewModel::columnCount(const QModelIndex& parent) const
{
	return 2;
}

QVariant ImageSetViewModel::data(const QModelIndex& index, int role) const
{
	//if (!hasIndex(index.row(), index.column(), index.parent()) || !index.isValid())
	//	return QVariant();

	if (role == Qt::DisplayRole)
	{
		if (typeid(index.internalPointer()) == typeid(Set*))
		{
			Set* instance = static_cast<Set*>(index.internalPointer());

			switch (index.column())
			{
			case 0:
				return QVariant(instance->images->at(index.row())->fileName);
			case 1:
				return QVariant(instance->images->at(index.row())->cameraId);
			default:
				return QVariant();
			}
		}

		switch (index.column())
		{
		case 0:
			return QVariant(displayData->at(index.row())->name);
		case 1:
			return QVariant(displayData->at(index.row())->setId);
		default:
			return QVariant();
		}
	}

	return QVariant();
}

QVariant ImageSetViewModel::headerData(int section, Qt::Orientation orientation, int role) const
{
	if (role != Qt::DisplayRole) return QVariant();
	if (orientation != Qt::Horizontal) return QVariant();

	if (section == 1) return "Name";
	else return "ID";
}

Qt::ItemFlags ImageSetViewModel::flags(const QModelIndex& index) const
{
	return Qt::ItemIsEnabled | Qt::ItemIsSelectable;
}

QModelIndex ImageSetViewModel::index(int row, int column, const QModelIndex& parent) const
{
	if (!hasIndex(row, column, parent))
		return QModelIndex();

	if (typeid(parent.internalPointer()) == typeid(Set*))
	{
		Set* instance = static_cast<Set*>(parent.internalPointer());
		createIndex(row, column, instance->images->at(row));
	}

	return createIndex(row, column, displayData->at(row));
}

QModelIndex ImageSetViewModel::parent(const QModelIndex& index) const
{
	if (!index.isValid())
		return QModelIndex();

	//return image parent (a Set)
	if (typeid(index.internalPointer()) == typeid(Image*))
	{
		Set* instance = static_cast<Image*>(index.internalPointer())->parent;
		createIndex(instance->row, 0, instance);
	}
	else if (typeid(index.internalPointer()) == typeid(Set*))
	{
		Set* instance = static_cast<Image*>(index.internalPointer())->parent;
		createIndex(instance->row, 0, instance);
	}

	return QModelIndex();
}

bool ImageSetViewModel::hasChildren(const QModelIndex& parent) const
{
	if (!parent.isValid())
		return rowCount() > 0;

	if (parent.row() >= 0)
	{
		//if(typeid(parent.internalPointer()) == typeid(ImageSetViewModel*))
			
		if(typeid(parent.internalPointer()) == typeid(Set*))
		{
			Set* set = static_cast<Set*>(parent.internalPointer());
			return set->images->size() > 0;
		}

		return displayData->at(parent.row())->images->size() > 0;
	}

	return false;
}

void ImageSetViewModel::clear()
{
	beginResetModel();
	displayData->clear();
	endResetModel();
}

bool ImageSetViewModel::addSet(int id, std::string name, std::string imageJson)
{
	if (setExists(id)) return false;

	int rows = rowCount();

	Set* set = new Set();
	set->setId = id;
	set->name = QString::fromStdString(name);
	set->row = rows;

	nlohmann::json data = nlohmann::json::parse(imageJson.c_str());
	for (int i = 0; i < data.size(); ++i)
	{
		Image* img = new Image();
		img->fileName = QString::fromStdString(data[i]["path"]);
		img->cameraId = data[i]["id"];

		set->images->append(img);
	}

	beginInsertRows(QModelIndex(), rows, rows);
	displayData->append(set);
	endInsertRows();

	return true;
}

bool ImageSetViewModel::setExists(int id)
{
	for (int i = displayData->size(); i < 0; --i)
		if (displayData->at(i)->setId == id) return true;

	return false;
}
