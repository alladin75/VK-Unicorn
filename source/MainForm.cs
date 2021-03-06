﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace VK_Unicorn
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; private set; }

        Database database;

        public MainForm()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            InitializeComponent();
        }

        void MainForm_Shown(object sender, EventArgs e)
        {
            Text = Constants.APP_NAME + " - " + Constants.APP_VERSION;
            VersionLabel.Text = Constants.APP_VERSION;
            Utils.Log(Text + " успешно загружен", LogLevel.SUCCESS);

            Utils.Log("Сайт проекта: " + Constants.PROJECT_WEB_PAGE, LogLevel.SUCCESS);
            Utils.Log("Поддержать проект: " + Constants.DONATION_ALERTS_WEB_PAGE, LogLevel.SUCCESS);

            // Готовим базу данных
            Utils.Log("Подготавливаем базу данных к работе", LogLevel.GENERAL);
            database = new Database();

            // Статистика
            database.ShowStatistics();

            // Запускаем веб сервер
            Utils.Log("Пытаемся запустить веб сервер на порт " + Constants.WEB_PORT, LogLevel.GENERAL);
            Utils.Log("Веб сервер нужен для просмотра результатов сканирования в браузере в виде привычной веб страницы", LogLevel.NOTIFY);

            try
            {
                new HTTPServer(Constants.WEB_PORT);

                StartWorkingButton.Enabled = true;
            }
            catch (Exception ex)
            {
                Utils.Log("Веб сервер не подключен на порт " + Constants.WEB_PORT + ". Причина: " + ex.Message, LogLevel.ERROR);
            }

            // Подготавливаем веб интерфейс
            var webInterface = new WebInterface();

            // Запускаем основной поток выполнения
            var worker = new Worker();
            worker.RunMainThread();
        }

        void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        void StartWorkingButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Constants.RESULTS_WEB_PAGE);
        }

        void VersionLabel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Constants.PROJECT_WEB_PAGE);
        }

        void LogTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        public RichTextBox GetLogTextBox()
        {
            return LogTextBox;
        }

        public void ShowWarningIfSettingsAreInvalid()
        {
            if (!database.IsSettingsValid())
            {
                Utils.Log("Программа пока не настроена. Сканирование начнётся только после установки правильных настроек. Нажмите \"Начать работу\" для первоначальной настройки", LogLevel.WARNING);
            }
        }

        public void SetStatus(string status, StatusType statusType)
        {
            var color = Color.Black;
            switch (statusType)
            {
                case StatusType.SUCCESS:
                    color = Color.DarkGreen;
                    break;

                case StatusType.ERROR:
                    color = Color.Red;
                    break;
            }

            Invoke((MethodInvoker)delegate
            {
                StatusLabel.Text = "Статус: " + status;
                StatusLabel.ForeColor = color;
            });
        }

        void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                TrayNotifyIcon.Visible = true;
                ShowInTaskbar = false;
            }
        }

        void TrayNotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;

            TrayNotifyIcon.Visible = false;

            LogTextBox.SelectionStart = LogTextBox.Text.Length;
            LogTextBox.ScrollToCaret();
        }

        public void DisableStartWorkingButton()
        {
            Invoke((MethodInvoker)delegate
            {
                StartWorkingButton.Enabled = false;
            });
        }
    }
}