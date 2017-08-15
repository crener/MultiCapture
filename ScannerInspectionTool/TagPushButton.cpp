#include "TagPushButton.h"

TagPushButton::TagPushButton(QWidget *parent)
	: QPushButton(parent)
{
	connect(this, &QPushButton::pressed, this, &TagPushButton::basePressed);
}

TagPushButton::~TagPushButton()
{
}

void TagPushButton::basePressed()
{
	emit triggered(tag);
}
