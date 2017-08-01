#pragma once
#include <qabstractitemmodel.h>

class ImageSetViewModel : public QAbstractItemModel
{
	struct Set;

	struct Image
	{
		int cameraId;
		QString fileName;

		Set* parent;
	};

	struct Set
	{
		int setId;
		int row;
		QString name;
		QList<Image*>* images = new QList<Image*>();
	};

	Q_OBJECT
public:
	ImageSetViewModel();
	~ImageSetViewModel();

	int rowCount(const QModelIndex &parent = QModelIndex()) const override;
	int columnCount(const QModelIndex &parent = QModelIndex()) const override;

	QVariant data(const QModelIndex &index, int role = Qt::DisplayRole) const override;
	QVariant headerData(int section, Qt::Orientation orientation, int role = Qt::DisplayRole) const override;
	Qt::ItemFlags flags(const QModelIndex &index) const override;
	QModelIndex index(int row, int column, const QModelIndex &parent = QModelIndex()) const override;
	QModelIndex parent(const QModelIndex &index) const override;
	bool hasChildren(const QModelIndex &parent = QModelIndex()) const override;

	void clear();
	bool addSet(int, std::string, std::string);

private:
	bool setExists(int id);

	QList<Set*>* displayData = new QList<Set*>();
};

