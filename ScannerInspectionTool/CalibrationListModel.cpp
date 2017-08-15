#include "CalibrationListModel.h"


CalibrationListModel::CalibrationListModel()
{
}


CalibrationListModel::~CalibrationListModel()
{
}

int CalibrationListModel::rowCount(const QModelIndex& parent) const
{
	return sets->size();
}

QVariant CalibrationListModel::data(const QModelIndex& index, int role) const
{
	if (role == Qt::DisplayRole)
		return sets->at(index.row())->name;

	if (role == Qt::DecorationRole)
	{
		CalibrationValidity icon = Missing;
		for (int i = 0; i < sets->at(index.row())->pairs->size(); ++i)
		{
			switch (sets->at(index.row())->pairs->at(i))
			{
			case Pending:
				icon = Pending;
				break;
			case Invalid:
				return failed;
			case Valid:
				if (icon != Pending) icon = Valid;
				break;
			case Missing:
				continue;
			}
		}

		switch (icon)
		{
		case Pending:
			return pending;
		case Invalid:
			return failed;
		case Valid:
			return done;
		case Missing:
			return pending;
		}
	}

	return QVariant();
}

Qt::ItemFlags CalibrationListModel::flags(const QModelIndex& index) const
{
	return Qt::ItemIsEnabled;
}

void CalibrationListModel::addItem(CalibrationSet* set)
{
	beginInsertRows(QModelIndex(), rowCount(), rowCount());
	sets->append(set);
	endInsertRows();
}

void CalibrationListModel::clearData()
{
	beginResetModel();
	sets->clear();
	endResetModel();
}

bool CalibrationListModel::containsSet(int id) const
{
	for (int i = 0; i < sets->size(); ++i)
		if (sets->at(i)->setId == id) return true;
	return false;
}

CalibrationSet* CalibrationListModel::getSet(int row)
{
	return sets->at(row);
}
