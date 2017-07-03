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
#include <QtWidgets/QGraphicsView>
#include <QtWidgets/QHBoxLayout>
#include <QtWidgets/QHeaderView>
#include <QtWidgets/QLabel>
#include <QtWidgets/QLineEdit>
#include <QtWidgets/QListView>
#include <QtWidgets/QMainWindow>
#include <QtWidgets/QMenu>
#include <QtWidgets/QMenuBar>
#include <QtWidgets/QPushButton>
#include <QtWidgets/QSplitter>
#include <QtWidgets/QVBoxLayout>
#include <QtWidgets/QWidget>

QT_BEGIN_NAMESPACE

class Ui_ScannerInspectionToolClass
{
public:
    QAction *action5;
    QAction *action888;
    QAction *actionDirect_Interaction_Window;
    QWidget *centralWidget;
    QVBoxLayout *verticalLayout;
    QSplitter *top;
    QWidget *layoutWidget;
    QVBoxLayout *connections;
    QVBoxLayout *connectionControl;
    QLabel *currentLbl;
    QListView *deviceList;
    QHBoxLayout *horizontalLayout;
    QPushButton *deviceScanBtn;
    QPushButton *deviceConnectBtn;
    QHBoxLayout *nameControl;
    QLineEdit *nameText;
    QPushButton *nameUpdateBtn;
    QGraphicsView *deviceImagePreview;
    QVBoxLayout *logs;
    QPushButton *deviceLogsBtn;
    QListView *deviceLogs;
    QMenuBar *menuBar;
    QMenu *menuDirectInteraction;

    void setupUi(QMainWindow *ScannerInspectionToolClass)
    {
        if (ScannerInspectionToolClass->objectName().isEmpty())
            ScannerInspectionToolClass->setObjectName(QStringLiteral("ScannerInspectionToolClass"));
        ScannerInspectionToolClass->resize(746, 630);
        QSizePolicy sizePolicy(QSizePolicy::Preferred, QSizePolicy::Preferred);
        sizePolicy.setHorizontalStretch(0);
        sizePolicy.setVerticalStretch(0);
        sizePolicy.setHeightForWidth(ScannerInspectionToolClass->sizePolicy().hasHeightForWidth());
        ScannerInspectionToolClass->setSizePolicy(sizePolicy);
        QIcon icon;
        QString iconThemeName = QStringLiteral("icon");
        if (QIcon::hasThemeIcon(iconThemeName)) {
            icon = QIcon::fromTheme(iconThemeName);
        } else {
            icon.addFile(QStringLiteral(":/ScannerInspectionTool/Resources/icon.png"), QSize(), QIcon::Normal, QIcon::Off);
        }
        ScannerInspectionToolClass->setWindowIcon(icon);
        action5 = new QAction(ScannerInspectionToolClass);
        action5->setObjectName(QStringLiteral("action5"));
        action888 = new QAction(ScannerInspectionToolClass);
        action888->setObjectName(QStringLiteral("action888"));
        actionDirect_Interaction_Window = new QAction(ScannerInspectionToolClass);
        actionDirect_Interaction_Window->setObjectName(QStringLiteral("actionDirect_Interaction_Window"));
        QIcon icon1;
        icon1.addFile(QStringLiteral(":/ScannerInspectionTool/Resources/icon-inspector.png"), QSize(), QIcon::Normal, QIcon::Off);
        actionDirect_Interaction_Window->setIcon(icon1);
        centralWidget = new QWidget(ScannerInspectionToolClass);
        centralWidget->setObjectName(QStringLiteral("centralWidget"));
        verticalLayout = new QVBoxLayout(centralWidget);
        verticalLayout->setSpacing(6);
        verticalLayout->setContentsMargins(11, 11, 11, 11);
        verticalLayout->setObjectName(QStringLiteral("verticalLayout"));
        top = new QSplitter(centralWidget);
        top->setObjectName(QStringLiteral("top"));
        top->setOrientation(Qt::Horizontal);
        layoutWidget = new QWidget(top);
        layoutWidget->setObjectName(QStringLiteral("layoutWidget"));
        connections = new QVBoxLayout(layoutWidget);
        connections->setSpacing(6);
        connections->setContentsMargins(11, 11, 11, 11);
        connections->setObjectName(QStringLiteral("connections"));
        connections->setContentsMargins(5, 5, 5, 0);
        connectionControl = new QVBoxLayout();
        connectionControl->setSpacing(6);
        connectionControl->setObjectName(QStringLiteral("connectionControl"));
        connectionControl->setSizeConstraint(QLayout::SetDefaultConstraint);
        currentLbl = new QLabel(layoutWidget);
        currentLbl->setObjectName(QStringLiteral("currentLbl"));
        QFont font;
        font.setPointSize(14);
        currentLbl->setFont(font);
        currentLbl->setStyleSheet(QStringLiteral("color: rgb(237, 20, 61);"));
        currentLbl->setLineWidth(1);
        currentLbl->setScaledContents(true);
        currentLbl->setAlignment(Qt::AlignCenter);
        currentLbl->setWordWrap(false);
        currentLbl->setOpenExternalLinks(false);

        connectionControl->addWidget(currentLbl);

        deviceList = new QListView(layoutWidget);
        deviceList->setObjectName(QStringLiteral("deviceList"));
        deviceList->setEditTriggers(QAbstractItemView::NoEditTriggers);
        deviceList->setResizeMode(QListView::Fixed);

        connectionControl->addWidget(deviceList);

        horizontalLayout = new QHBoxLayout();
        horizontalLayout->setSpacing(6);
        horizontalLayout->setObjectName(QStringLiteral("horizontalLayout"));
        deviceScanBtn = new QPushButton(layoutWidget);
        deviceScanBtn->setObjectName(QStringLiteral("deviceScanBtn"));
        deviceScanBtn->setMinimumSize(QSize(80, 23));

        horizontalLayout->addWidget(deviceScanBtn);

        deviceConnectBtn = new QPushButton(layoutWidget);
        deviceConnectBtn->setObjectName(QStringLiteral("deviceConnectBtn"));
        deviceConnectBtn->setEnabled(false);
        deviceConnectBtn->setMinimumSize(QSize(80, 23));

        horizontalLayout->addWidget(deviceConnectBtn);


        connectionControl->addLayout(horizontalLayout);


        connections->addLayout(connectionControl);

        nameControl = new QHBoxLayout();
        nameControl->setSpacing(6);
        nameControl->setObjectName(QStringLiteral("nameControl"));
        nameText = new QLineEdit(layoutWidget);
        nameText->setObjectName(QStringLiteral("nameText"));

        nameControl->addWidget(nameText);

        nameUpdateBtn = new QPushButton(layoutWidget);
        nameUpdateBtn->setObjectName(QStringLiteral("nameUpdateBtn"));

        nameControl->addWidget(nameUpdateBtn);


        connections->addLayout(nameControl);

        top->addWidget(layoutWidget);
        deviceImagePreview = new QGraphicsView(top);
        deviceImagePreview->setObjectName(QStringLiteral("deviceImagePreview"));
        deviceImagePreview->setMinimumSize(QSize(270, 0));
        top->addWidget(deviceImagePreview);

        verticalLayout->addWidget(top);

        logs = new QVBoxLayout();
        logs->setSpacing(6);
        logs->setObjectName(QStringLiteral("logs"));
        logs->setSizeConstraint(QLayout::SetNoConstraint);
        logs->setContentsMargins(5, 5, 0, 5);
        deviceLogsBtn = new QPushButton(centralWidget);
        deviceLogsBtn->setObjectName(QStringLiteral("deviceLogsBtn"));

        logs->addWidget(deviceLogsBtn);

        deviceLogs = new QListView(centralWidget);
        deviceLogs->setObjectName(QStringLiteral("deviceLogs"));

        logs->addWidget(deviceLogs);


        verticalLayout->addLayout(logs);

        ScannerInspectionToolClass->setCentralWidget(centralWidget);
        menuBar = new QMenuBar(ScannerInspectionToolClass);
        menuBar->setObjectName(QStringLiteral("menuBar"));
        menuBar->setGeometry(QRect(0, 0, 746, 21));
        menuDirectInteraction = new QMenu(menuBar);
        menuDirectInteraction->setObjectName(QStringLiteral("menuDirectInteraction"));
        menuDirectInteraction->setTearOffEnabled(false);
        ScannerInspectionToolClass->setMenuBar(menuBar);

        menuBar->addAction(menuDirectInteraction->menuAction());
        menuDirectInteraction->addAction(actionDirect_Interaction_Window);

        retranslateUi(ScannerInspectionToolClass);

        QMetaObject::connectSlotsByName(ScannerInspectionToolClass);
    } // setupUi

    void retranslateUi(QMainWindow *ScannerInspectionToolClass)
    {
        ScannerInspectionToolClass->setWindowTitle(QApplication::translate("ScannerInspectionToolClass", "Scanner Inspection Tool", Q_NULLPTR));
        action5->setText(QApplication::translate("ScannerInspectionToolClass", "5", Q_NULLPTR));
        action888->setText(QApplication::translate("ScannerInspectionToolClass", "888", Q_NULLPTR));
        actionDirect_Interaction_Window->setText(QApplication::translate("ScannerInspectionToolClass", "Direct Interaction Window", Q_NULLPTR));
        currentLbl->setText(QApplication::translate("ScannerInspectionToolClass", "Disconnected", Q_NULLPTR));
        deviceScanBtn->setText(QApplication::translate("ScannerInspectionToolClass", "Refresh", Q_NULLPTR));
        deviceConnectBtn->setText(QApplication::translate("ScannerInspectionToolClass", "Connect", Q_NULLPTR));
        nameUpdateBtn->setText(QApplication::translate("ScannerInspectionToolClass", "Update Name", Q_NULLPTR));
        deviceLogsBtn->setText(QApplication::translate("ScannerInspectionToolClass", "Refresh Scanner Logs", Q_NULLPTR));
        menuDirectInteraction->setTitle(QApplication::translate("ScannerInspectionToolClass", "Tools", Q_NULLPTR));
    } // retranslateUi

};

namespace Ui {
    class ScannerInspectionToolClass: public Ui_ScannerInspectionToolClass {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_SCANNERINSPECTIONTOOL_H
