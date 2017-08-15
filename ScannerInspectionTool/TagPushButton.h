#pragma once

#include <QPushButton>

class TagPushButton : public QPushButton
{
	Q_OBJECT

public:
	TagPushButton(QWidget *parent);
	~TagPushButton();

	void setTag(int tag) { this->tag = tag; };
	int getTag() { return tag; };

	signals:
	void triggered(const int &tag);

	private slots:
	void basePressed();

private:
	int tag = -1;
};
