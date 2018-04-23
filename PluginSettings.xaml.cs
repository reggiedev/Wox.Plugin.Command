using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Wox.Core.Plugin;

namespace Wox.Plugin.Command
{
    public partial class PluginSettings : UserControl
    {
        private Settings _settings;
        private PluginInitContext _context;

        public PluginSettings(Settings settings,  PluginInitContext context)
        {
            InitializeComponent();
            _settings = settings;
            _context = context;
            listMain.ItemsSource = _settings.Commands;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int idx = listMain.SelectedIndex;
            if (listMain.SelectedIndex != -1)
            {
                _settings.Commands.RemoveAt(idx);
            }

            Refresh();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            int idx = listMain.SelectedIndex;
            var wnd = new CommandSettingWindow(_settings, idx ,_context);
            wnd.ShowDialog();
            Refresh();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new CommandSettingWindow(_settings, -1, _context);
            wnd.ShowDialog();
            Refresh();
        }

        public void Refresh()
        {
            _settings.Sort();
            listMain.Items.Refresh();
        }
    }
}
