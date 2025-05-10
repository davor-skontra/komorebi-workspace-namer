using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Flow.Launcher.Plugin.KomorebiWorkspaceNamer.UserConfig;
using SettingsControl = Flow.Launcher.Plugin.KomorebiWorkspaceNamer.UserConfig.SettingsControl;

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer
{
    public class KomorebiWorkspaceNamer : IPlugin, ISettingProvider
    {
        private const string IcoPath = "icon.png";
        private PluginInitContext _context = null!;
        private Settings _settings = null!;
        
        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
        }

        public List<Result> Query(Query query)
        {
            var search = query.Search;
            var workspaceInfo = GetWorkspaceInfo();

            List<Result> results = new();
            
            results.Add(GetRenameResult(query.Search, workspaceInfo, _settings.IndexStyler));

            var cleanWorkspaceName = IndexStyler.RemovePosition(workspaceInfo.Name);
            
            foreach (var name in GetMatchingNames(search, workspaceInfo))
            {
                if (name == cleanWorkspaceName)
                {
                    continue;
                }
                Result r = GetRenameResult(name, workspaceInfo, _settings.IndexStyler);
                results.Add(r); 
            }
            
            return results;
        }

        private IEnumerable<string> GetMatchingNames(string search, WorkspaceInfo info)
        {
            var presets = _settings.GetPresetNames().Where(p => p.StartsWith(search));
            var windowTitles = info.SortedWindowTitles.Where(p => p.StartsWith(search));
            foreach (var presetName in presets)
            {
                yield return presetName;
            }
            foreach (var title in windowTitles)
            {
                yield return title;
            }
        }

        private WorkspaceInfo GetWorkspaceInfo()
        {
            var stateJson = ProcessCalls.GetStateJson();
            return new WorkspaceInfo(stateJson);
        }
        
        private Result GetRenameResult(string rawName, WorkspaceInfo info, IndexStyler.Kind style)
        {
            var styledName = new IndexStyler(style, rawName, info);
            var nameWithPosition = styledName.GetMarkedName();
            var oldNameWithNoPosition = IndexStyler.RemovePosition(info.Name);
            
            var title = string.IsNullOrWhiteSpace(rawName)
                ? $"Rename workspace {oldNameWithNoPosition} to ..."
                : $"Rename workspace {oldNameWithNoPosition} to {rawName}";

            return new Result()
            {
                Title = title,
                Action = a =>
                {
                    ProcessCalls.RenameWorkspace(info, nameWithPosition);
                    _context.API.ChangeQuery("", true);
                    return true;
                },
                IcoPath = IcoPath
            };
        }

        public Control CreateSettingPanel()
        {
            SettingsControl sc = new(_settings);
            return sc;
        }
    }
}