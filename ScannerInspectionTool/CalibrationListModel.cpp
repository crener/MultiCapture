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
		switch (sets->at(index.row())->valid)
		{
		case Pending:
			return pending;
		case Invalid:
			return failed;
		case Valid:
			return done;
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
