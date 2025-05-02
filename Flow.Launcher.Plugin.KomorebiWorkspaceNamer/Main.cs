using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Flow.Launcher.Plugin.KomorebiWorkspaceNamer.UserConfig;
using SettingsControl = Flow.Launcher.Plugin.KomorebiWorkspaceNamer.UserConfig.SettingsControl;

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer
{
    public class KomorebiWorkspaceNamer : IPlugin, ISettingProvider
    {
        private PluginInitContext _context = null!;
        private Settings _settings = null!;
        
        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
        }

        public List<Result> Query(Query query)
        {
            var workspaceInfo = GetWorkspaceInfo();

            List<Result> results = new();
            
            var manualRename = GetRenameResult(query.Search, workspaceInfo, _settings.IndexStyler);
            results.Add(manualRename);
            results.AddRange(GetAppRenameResults(workspaceInfo, _settings.IndexStyler));
            
            return results;
        }

        private WorkspaceInfo GetWorkspaceInfo()
        {
            var stateJson = ProcessCalls.GetStateJson();
            return new WorkspaceInfo(stateJson);
        }

        private IEnumerable<Result> GetAppRenameResults(WorkspaceInfo info, IndexStyler.Kind appendPosition)
        {
            foreach (var title in info.SortedWindowTitles)
            {
                yield return GetRenameResult(title, info, appendPosition);
            }
        }

        private Result GetRenameResult(string rawName, WorkspaceInfo info, IndexStyler.Kind appendPosition)
        {
            var styledName = new IndexStyler(appendPosition, rawName, info);
            var nameWithPosition = styledName.GetMarkedName();
            var oldNameWithNoPosition = IndexStyler.RemovePosition(info.Name);
            
            var title = string.IsNullOrWhiteSpace(rawName)
                ? $"Rename workspace '{oldNameWithNoPosition}' to ..."
                : $"Rename workspace: '{oldNameWithNoPosition}' to '{rawName}'";

            return new Result()
            {
                Title = title,
                Action = a =>
                {
                    ProcessCalls.RenameWorkspace(info, nameWithPosition);
                    _context.API.ChangeQuery("", true);
                    return true;
                }
            };
        }

        public Control CreateSettingPanel()
        {
            SettingsControl sc = new(_settings);
            return sc;
        }
    }
}