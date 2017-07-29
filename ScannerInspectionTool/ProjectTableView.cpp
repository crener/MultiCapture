#include "ProjectTableView.h"
#include <qsize.h>


ProjectTableView::ProjectTableView()
{
}


ProjectTableView::~ProjectTableView()
{
	delete projects;
}

int ProjectTableView::rowCount(const QModelIndex& parent) const
{
	return projects->size();
}

int ProjectTableView::columnCount(const QModelIndex& parent) const
{
	return 3;
}

QVariant ProjectTableView::data(const QModelIndex& index, int role) const
{
	if (!validIndex(index)) return QVariant();

	if (role == Qt::DisplayRole)
		switch (index.column())
		{
		case 0:
			return QString::fromStdString(projects->at(index.row()).name);
		case 1:
			return projects->at(index.row()).imageCount;
		case 2:
			return projects->at(index.row()).imagesAvaliable;
		default:
			return QVariant();
		}
	else if (role == Qt::EditRole) return false;

	return QVariant();
}

Qt::ItemFlags ProjectTableView::flags(const QModelIndex& index) const
{
	return Qt::ItemIsEnabled | Qt::ItemIsSelectable;
}

bool ProjectTableView::validIndex(const QModelIndex& index) const
{
	if (index.column() > columnCount()) return false;
	if (index.row() > rowCount()) return false;

	return true;
}

QVariant ProjectTableView::headerData(int section, Qt::Orientation orientation, int role) const
{
	if (role == Qt::EditRole) return false;
	if (role == Qt::DisplayRole)
		switch (section)
		{
		case 0:
			return "Name";
		case 1:
			return "Total";
		case 2:
			return "Avaliable";
		default:
			return "Unknown";
		}
	if(role == Qt::SizeHintRole)
	{
		if (section == 0) return QSize(headerNameSize, headerHeight);
		return QSize(headerSize, headerHeight);
	}

	return QVariant();
}

void ProjectTableView::addItem(project project)
{
	int rows = rowCount();
	beginInsertRows(QModelIndex(), rows, rows);

	projects->append(project);

	endInsertRows();
}

void ProjectTableView::clearData()
{
	beginResetModel();

	projects->clear();

	endResetModel();
}

void ProjectTableView::updateTable()
{
	QModelIndex topLeft = createIndex(0, 0);
	QModelIndex bottomRight = createIndex(rowCount() - 1, columnCount());

	emit dataChanged(topLeft, bottomRight);
}

bool ProjectTableView::canChangeName(const QModelIndex& index)
{
	return index.column() == 0;
}

int ProjectTableView::getProjectId(const QModelIndex& index)
{
	return projects->at(index.row()).id;
}
