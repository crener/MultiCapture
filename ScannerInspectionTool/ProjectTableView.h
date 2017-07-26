#pragma once
#include <qabstractitemmodel.h>
#include "Project.h"

class ProjectTableView : public QAbstractTableModel
{
	Q_OBJECT
public:
	ProjectTableView();
	~ProjectTableView();

	int rowCount(const QModelIndex &parent = QModelIndex()) const override;
	int columnCount(const QModelIndex &parent = QModelIndex()) const override;
	QVariant data(const QModelIndex &index, int role = Qt::DisplayRole) const override;
	QVariant headerData(int section, Qt::Orientation orientation, int role = Qt::DisplayRole) const override;
	Qt::ItemFlags flags(const QModelIndex &index) const override;

	void addItem(project);
	void clearData();
	void updateTable();

private:
	const int headerNameSize = 50;
	const int headerSize = 25;
	const int headerHeight = 30;

	bool validIndex(const QModelIndex& index) const;

	QList<project>* projects = new QList<project>();
};

