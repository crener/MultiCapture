#pragma once
#include "JsonTypes.h"

class CalibrationListModel : public QAbstractListModel
{
	Q_OBJECT

public:
	CalibrationListModel();
	~CalibrationListModel();

	int rowCount(const QModelIndex &parent = QModelIndex()) const override;
	QVariant data(const QModelIndex &index, int role = Qt::DisplayRole) const override;
	Qt::ItemFlags flags(const QModelIndex &index) const override;

	void addItem(CalibrationSet* set);
	void clearData();
	bool containsSet(int id) const;
	CalibrationSet* getSet(int row);

private:
	QList<CalibrationSet*>* sets = new QList<CalibrationSet*>();

	const QIcon pending = QIcon("pending");
	const QIcon done = QIcon("transferComplete");
	const QIcon failed = QIcon("transferNeeded");
};

