#pragma once
#include <qabstractitemmodel.h>
#include <GenericTreeItem.h>

class GenericTreeViewModel : public QAbstractItemModel
{
	Q_OBJECT

public:
	explicit GenericTreeViewModel(GenericTreeItem* rootItem, QObject *parent = 0);
	~GenericTreeViewModel();

	int rowCount(const QModelIndex &parent = QModelIndex()) const override;
	int columnCount(const QModelIndex &parent = QModelIndex()) const override;

	QVariant data(const QModelIndex &index, int role) const override;
	Qt::ItemFlags flags(const QModelIndex &index) const override;
	QVariant headerData(int section, Qt::Orientation orientation, int role = Qt::DisplayRole) const override;
	QModelIndex index(int row, int column, const QModelIndex &parent = QModelIndex()) const override;
	QModelIndex parent(const QModelIndex &index) const override;

	void addItem(GenericTreeItem* newItem, GenericTreeItem* parent = 0);
	void clearData();

private:

	GenericTreeItem *rootItem;
};

