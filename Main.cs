using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wox.Core.Plugin;
using Wox.Infrastructure;
using Wox.Infrastructure.Image;
using Wox.Infrastructure.Storage;

namespace Wox.Plugin.Command
{
    internal class Main : IPlugin, ISettingProvider, ISavable
    {
        private PluginInitContext _context;
        private readonly Settings _settings;
        private readonly PluginJsonStorage<Settings> _storage;

        public Main()
        {
            _storage = new PluginJsonStorage<Settings>();
            _settings = _storage.Load();
        }

        private void InitApp()
        {
//            _settings.Apps.Clear();
//            _settings.Commands.Clear();
            string id = _context.CurrentPluginMetadata.ID;
            foreach (var app in _settings.Apps)
            {
                if (!PluginManager.ActionKeywordRegistered(app.Key))
                {
                    PluginManager.AddActionKeyword(id, app.Key);
                }
            }
        }

        public void Save()
        {
            _storage.Save();
        }

        public void Init(PluginInitContext context)
        {
            _context = context;
            InitApp();
        }

        public Control CreateSettingPanel()
        {
            return new PluginSettings(_settings, _context);
        }

        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();
            foreach (var c in _settings.Commands)
            {
                App app = _settings.FindApp(c.AppId);
                if (query.ActionKeyword == app.Key)
                {
                    results.Add(c.GetResult(query, _settings));
                }
            }

            return results;
        }
    }
}