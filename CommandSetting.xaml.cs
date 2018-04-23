using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Wox.Core.Plugin;
using MessageBox = System.Windows.MessageBox;

namespace Wox.Plugin.Command
{
    public partial class CommandSettingWindow
    {
        private Settings _settings;
        private int _index;
        private PluginInitContext _context;

        public CommandSettingWindow(Settings settings, int index, PluginInitContext context)
        {
            _settings = settings;
            _index = index;
            _context = context;
            InitializeComponent();
            InitData();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void InitData()
        {
            RefreshAppCombo();
            if (_index != -1)
            {
                Command cmd = _settings.Commands[_index];
                for (int i = 0; i < _settings.Apps.Count; ++i)
                {
                    if (cmd.AppId == _settings.Apps[i].ID)
                    {
                        comboBoxApp.SelectedIndex = i;
                    }
                }

                textboxObject.Text = cmd.Object;
            }
        }

        private void RefreshAppCombo()
        {
            comboBoxApp.Items.Clear();
            foreach (var app in _settings.Apps)
            {
                comboBoxApp.Items.Add(app.ID);
            }
            comboBoxApp.Items.Add("Add New Application");
        }

        private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            string appId = (string) comboBoxApp.SelectedItem;
            App app = _settings.FindApp(appId);
            Command cmd = new Command
            {
                AppId = (string)comboBoxApp.SelectedItem,
                Desc = Path.GetFileName(app.Path),
                Object = textboxObject.Text,
            };

            if (cmd.AppId == "" || cmd.Object == "")
            {
                MessageBox.Show("对象和应用程序不能为空");
                return;
            }

            if (_index == -1)
            {
                _settings.Commands.Add(cmd);
            }
            else
            {
                _settings.Commands[_index] = cmd;
            }

            Close();
        }

        private void OnSelectObject(object sender, RoutedEventArgs e)
        {
            string id = (string)comboBoxApp.SelectedItem;
            App app = _settings.FindApp(id);

            if (app == null)
            {
                MessageBox.Show("请先设置应用程序");
                return;
            }

            if (app.IsFile)
            {
                var openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textboxObject.Text = openFileDialog.FileName;
                }
            }
            else
            {
                var folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textboxObject.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void OnSelectApp(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int index = comboBoxApp.SelectedIndex;
            if (index == _settings.Apps.Count)
            {
                OpenAppSetting();
            }
        }

        private void OnSetApp(object sender, RoutedEventArgs e)
        {
            OpenAppSetting();
        }

        private void OpenAppSetting()
        {
            int index = comboBoxApp.SelectedIndex;
            if (index != -1)
            {
                string id = (string) comboBoxApp.SelectedItem;
                if (index == _settings.Apps.Count)
                {
                    id = null;
                }

                var wnd = new AppSettingWindow(_settings, id, _context);
                wnd.ShowDialog();
                RefreshAppCombo();
            }
        }
    }
}