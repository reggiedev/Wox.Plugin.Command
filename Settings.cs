using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Wox.Infrastructure;

namespace Wox.Plugin.Command
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Command : BaseModel
    {

        [JsonProperty] public string AppId { get; set; }
        [JsonProperty] public string Object { get; set; }
        [JsonProperty] public string Desc { get; set; }

        public Result GetResult(Query q, Settings settings)
        {
            App app = settings.FindApp(AppId);
           
            string objName = Path.GetFileName(Object);
            string param = app.Param + Object;
            Result result = new Result
            {
                Title = objName,
                SubTitle = Object,
                IcoPath = app.Path,
                Score = StringMatcher.Score(objName, q.Search),
                Action = e =>
                {
                    Process.Start(app.Path, param);
                    return true;
                },
            };
            return result;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class App 
    {
        [JsonProperty] public string ID { get; set; }
        [JsonProperty] public string Path { get; set; }
        [JsonProperty] public string Key { get; set; }
        [JsonProperty] public string Param { get; set; }
        [JsonProperty] public string Desc { get; set; }
        [JsonProperty] public bool IsFile { get; set; }
    } 

    public class Settings
    {
        [JsonProperty] public List<Command> Commands { get; set; } = new List<Command>();
        [JsonProperty] public List<App> Apps { get; set; } = new List<App>();

        public void Sort()
        {
//            var list = new List<Command>();
//            foreach (var app in Apps)
//            {
//                foreach (var cmd in Commands)
//                {
//                    if (cmd.AppId == app.ID)
//                    {
//                        list.Add(cmd);
//                    }
//                }
//            }
//            Commands = list;
        }

        public App FindApp(string id)
        {
            foreach (var app in Apps)
            {
                if(app.ID == id)
                    return app;
            }
            return null;
        }
    }
}