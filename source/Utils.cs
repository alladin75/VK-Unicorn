﻿using System;
using System.Drawing;
using System.Collections.Generic;

namespace VK_Unicorn
{
    public enum LogLevel
    {
        NOTIFY,
        GENERAL,
        SUCCESS,
        WARNING,
        ERROR,
    }

    public enum StatusType
    {
        GENERAL,
        SUCCESS,
        ERROR,
    }

    public static class Utils
    {
        public static void Log(string text, LogLevel logLevel = LogLevel.GENERAL)
        {
            Color? color = null;
            var prefix = string.Empty;

            switch (logLevel)
            {
                case LogLevel.ERROR:
                    color = Color.Red;
                    prefix = "Ошибка: ";
                    break;

                case LogLevel.WARNING:
                    color = Color.Yellow;
                    break;

                case LogLevel.NOTIFY:
                    color = Color.Gray;
                    break;

                case LogLevel.SUCCESS:
                    color = Color.DarkGreen;
                    break;
            }

            Log(prefix + text, color);
        }

        public static void Log(string text, Color? color = null)
        {
            text = "[" + DateTime.Now.ToLongTimeString() + "] " + text;

            var logTextBox = MainForm.Instance.GetLogTextBox();

            logTextBox.SuspendLayout();

            var previousSelectionColor = logTextBox.SelectionColor;
            if (color != null)
            {
                logTextBox.SelectionColor = color.Value;
            }

            if (!string.IsNullOrWhiteSpace(logTextBox.Text))
            {
                logTextBox.AppendText($"{Environment.NewLine}{text}");
            }
            else
            {
                logTextBox.AppendText(text);
            }

            logTextBox.ScrollToCaret();
            logTextBox.SelectionColor = previousSelectionColor;
            logTextBox.ResumeLayout();
        }

        // Получаем "1,2,3" из листа "1" "2" 3", если разделитель ","
        public static string GenerateSeparatedString<T>(this List<T> self, string separator)
        {
            var result = "";

            var index = 0;
            foreach (var item in self)
            {
                result += item.ToString();

                if (index != self.Count - 1)
                {
                    result += separator;
                }

                ++index;
            }

            return result;
        }

        // Возвращает слово в зависимсти от числа
        // 21 год
        // 22 года
        // 27 лет
        public static string OneFewMany(this int number, string one, string few, string many)
        {
            number = Math.Abs(number);
            var mod100 = number % 100;

            var result = many;
            if (!(mod100 >= 11 && mod100 <= 19))
            {
                switch (number % 10)
                {
                    case 1:
                        result = one;
                        break;

                    case 2:
                    case 3:
                    case 4:
                        result = few;
                        break;
                }
            }

            return result;
        }

        public static string GetURL(this VkNet.Model.Group self)
        {
            return Constants.VK_WEB_PAGE + self.ScreenName;
        }
    }

    public delegate void Callback();
    public delegate void Callback<T0>(T0 arg0);
    public delegate void Callback<T0, T1>(T0 arg0, T1 arg1);
    public delegate void Callback<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);
    public delegate void Callback<T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, T3 arg3);
    public delegate void Callback<T0, T1, T2, T3, T4>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate void Callback<T0, T1, T2, T3, T4, T5>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate void Callback<T0, T1, T2, T3, T4, T5, T6>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate void Callback<T0, T1, T2, T3, T4, T5, T6, T7>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    public delegate void Callback<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

    public delegate U CallbackWithReturn<U>();
    public delegate U CallbackWithReturn<U, T0>(T0 arg0);
    public delegate U CallbackWithReturn<U, T0, T1>(T0 arg0, T1 arg1);
    public delegate U CallbackWithReturn<U, T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);
    public delegate U CallbackWithReturn<U, T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, T3 arg3);
    public delegate U CallbackWithReturn<U, T0, T1, T2, T3, T4>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate U CallbackWithReturn<U, T0, T1, T2, T3, T4, T5>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate U CallbackWithReturn<U, T0, T1, T2, T3, T4, T5, T6>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate U CallbackWithReturn<U, T0, T1, T2, T3, T4, T5, T6, T7>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    public delegate U CallbackWithReturn<U, T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
}