#pragma once
#include <qabstractitemmodel.h>
#include "ImageSet.h"

class ProjectTransferViewModel : public QAbstractItemModel
{
	Q_OBJECT

public:
	ProjectTransferViewModel();
	~ProjectTransferViewModel();

	int rowCount(const QModelIndex &parent = QModelIndex()) const override;
	int columnCount(const QModelIndex &parent = QModelIndex()) const override;
	QVariant data(const QModelIndex &index, int role = Qt::DisplayRole) const override;
	QVariant headerData(int section, Qt::Orientation orientation, int role = Qt::DisplayRole) const override;
	Qt::ItemFlags flags(const QModelIndex &index) const override;
	QModelIndex index(int row, int column, const QModelIndex &parent = QModelIndex()) const override;
	QModelIndex parent(const QModelIndex &index) const override;

	void clearData();
	void addImageSet(ImageSet);

private:

	bool containsImageSet(int);

	QList<ImageSet>* displayData;

};

