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
    public partial class AppSettingWindow
    {
        private Settings _settings;
        private string _id;
        private PluginInitContext _context;
        private bool _add;
        private int _index;

        public AppSettingWindow(Settings settings, string oldId, PluginInitContext context)
        {
            _settings = settings;
            _id = oldId;
            _context = context;
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            _add = _id == null;
            if (!_add)
            {
                App app = _settings.FindApp(_id);
                textboxPath.Text = app.Path;
                textboxID.Text = app.ID;
                textboxKey.Text = app.Key;
                textboxParam.Text = app.Param;
                radioFile.IsChecked = app.IsFile;
                radioFolder.IsChecked = !app.IsFile;
                _index = -1;
                for (int i = 0; i < _settings.Apps.Count; ++i)
                {
                    if (_settings.Apps[i].ID == _id)
                    {
                        _index = i;
                        break;
                    }
                }
            }
            else
            {
                radioFile.IsChecked = true;
                radioFolder.IsChecked = false;
            }
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            App app = new App();
            app.Path = textboxPath.Text;
            app.ID = textboxID.Text;
            app.Key = textboxKey.Text;
            app.Param = textboxParam.Text;
            app.IsFile = (bool) radioFile.IsChecked;

            if (app.ID == "" || app.Key == "")
            {
                MessageBox.Show("关键字和标志不能为空");
                return;
            }

            string id = _context.CurrentPluginMetadata.ID;
            if (_add)
            {
                _settings.Apps.Add(app);
                if (!_context.CurrentPluginMetadata.ActionKeywords.Contains(app.Key))
                {
                    PluginManager.AddActionKeyword(id, app.Key);
                }
            }
            else
            {
                string oldKey = _settings.Apps[_index].Key;
                if (app.Key != oldKey)
                {
                    PluginManager.ReplaceActionKeyword(id, oldKey, app.Key);
                }

                if (!_context.CurrentPluginMetadata.ActionKeywords.Contains(app.Key))
                {
                    PluginManager.AddActionKeyword(id, app.Key);
                }

                _settings.Apps[_index] = app;
            }

            Close();
        }

        private void OnSetPath(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textboxPath.Text = openFileDialog.FileName;
            }
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (_index != _settings.Apps.Count)
            {
                App app = _settings.Apps[_index];
                string id = _context.CurrentPluginMetadata.ID;
                if (_context.CurrentPluginMetadata.ActionKeywords.Contains(app.Key))
                {
                    PluginManager.RemoveActionKeyword(id, app.Key);
                }

                _settings.Apps.RemoveAt(_index);
            }

            Close();
        }
    }
}