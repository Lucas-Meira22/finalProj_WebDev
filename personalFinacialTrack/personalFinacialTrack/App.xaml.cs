using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using personalFinacialTrack.Resources.Model;

namespace personalFinacialTrack
{
    public partial class App : Application
    {
        private static SqliteHelper? _sqliteHelper;

        public static SqliteHelper SqliteHelper
        {
            get
            {
                if (_sqliteHelper == null)
                {
                    string dbPath = Path.Combine(FileSystem.AppDataDirectory, "personalFinance.db3");
                    _sqliteHelper = new SqliteHelper(dbPath);
                }
                return _sqliteHelper;
            }
        }

        public App()
        {
            InitializeComponent();

            Application.Current.UserAppTheme = AppTheme.Dark;

            RequestedThemeChanged += OnRequestedThemeChanged;

            MainPage = new AppShell();
        }

        private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
        {
            SetAppTheme(e.RequestedTheme);
        }

        private void SetAppTheme(AppTheme theme)
        {
            Application.Current.UserAppTheme = theme;
        }
    }
}

