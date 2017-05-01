/********************************************************************************
** Form generated from reading UI file 'ScannerInspectionTool.ui'
**
** Created by: Qt User Interface Compiler version 5.8.0
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_SCANNERINSPECTIONTOOL_H
#define UI_SCANNERINSPECTIONTOOL_H

#include <QtCore/QVariant>
#include <QtWidgets/QAction>
#include <QtWidgets/QApplication>
#include <QtWidgets/QButtonGroup>
#include <QtWidgets/QHeaderView>
#include <QtWidgets/QListView>
#include <QtWidgets/QMainWindow>
#include <QtWidgets/QMenuBar>
#include <QtWidgets/QPushButton>
#include <QtWidgets/QStatusBar>
#include <QtWidgets/QToolBar>
#include <QtWidgets/QWidget>

QT_BEGIN_NAMESPACE

class Ui_ScannerInspectionToolClass
{
public:
    QWidget *centralWidget;
    QListView *deviceList;
    QPushButton *deviceScanBtn;
    QPushButton *deviceConnectBtn;
    QMenuBar *menuBar;
    QToolBar *mainToolBar;
    QStatusBar *statusBar;

    void setupUi(QMainWindow *ScannerInspectionToolClass)
    {
        if (ScannerInspectionToolClass->objectName().isEmpty())
            ScannerInspectionToolClass->setObjectName(QStringLiteral("ScannerInspectionToolClass"));
        ScannerInspectionToolClass->resize(600, 400);
        centralWidget = new QWidget(ScannerInspectionToolClass);
        centralWidget->setObjectName(QStringLiteral("centralWidget"));
        deviceList = new QListView(centralWidget);
        deviceList->setObjectName(QStringLiteral("deviceList"));
        deviceList->setGeometry(QRect(10, 10, 256, 192));
        deviceScanBtn = new QPushButton(centralWidget);
        deviceScanBtn->setObjectName(QStringLiteral("deviceScanBtn"));
        deviceScanBtn->setGeometry(QRect(10, 210, 75, 23));
        deviceConnectBtn = new QPushButton(centralWidget);
        deviceConnectBtn->setObjectName(QStringLiteral("deviceConnectBtn"));
        deviceConnectBtn->setEnabled(false);
        deviceConnectBtn->setGeometry(QRect(90, 210, 171, 23));
        ScannerInspectionToolClass->setCentralWidget(centralWidget);
        menuBar = new QMenuBar(ScannerInspectionToolClass);
        menuBar->setObjectName(QStringLiteral("menuBar"));
        menuBar->setGeometry(QRect(0, 0, 600, 21));
        ScannerInspectionToolClass->setMenuBar(menuBar);
        mainToolBar = new QToolBar(ScannerInspectionToolClass);
        mainToolBar->setObjectName(QStringLiteral("mainToolBar"));
        ScannerInspectionToolClass->addToolBar(Qt::TopToolBarArea, mainToolBar);
        statusBar = new QStatusBar(ScannerInspectionToolClass);
        statusBar->setObjectName(QStringLiteral("statusBar"));
        ScannerInspectionToolClass->setStatusBar(statusBar);

        retranslateUi(ScannerInspectionToolClass);

        QMetaObject::connectSlotsByName(ScannerInspectionToolClass);
    } // setupUi

    void retranslateUi(QMainWindow *ScannerInspectionToolClass)
    {
        ScannerInspectionToolClass->setWindowTitle(QApplication::translate("ScannerInspectionToolClass", "Scanner Inspection Tool", Q_NULLPTR));
        deviceScanBtn->setText(QApplication::translate("ScannerInspectionToolClass", "Refresh", Q_NULLPTR));
        deviceConnectBtn->setText(QApplication::translate("ScannerInspectionToolClass", "Connect", Q_NULLPTR));
    } // retranslateUi

};

namespace Ui {
    class ScannerInspectionToolClass: public Ui_ScannerInspectionToolClass {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_SCANNERINSPECTIONTOOL_H
