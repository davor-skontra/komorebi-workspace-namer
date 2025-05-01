using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using Flow.Launcher.Plugin.KomorebiWorkspaceNamer.UserConfig;
using SettingsControl = Flow.Launcher.Plugin.KomorebiWorkspaceNamer.UserConfig.SettingsControl;

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer
{
    public class KomorebiWorkspaceNamer : IPlugin, ISettingProvider
    {
        private PluginInitContext _context = null!;
        private WorkspaceInfo? _workspaceInfo;
        private Settings _settings;
        
        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
        }

        public List<Result> Query(Query query)
        {
            if (_workspaceInfo == null)
            {
                var stateJson = ProcessCalls.GetStateJson();
                _workspaceInfo ??= new WorkspaceInfo(stateJson);
            }
            
            var search = query.Search;
            var appendPosition = _settings.AppendPosition;
            List<Result> results = new();
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                var manualRename = GetRenameResult(query.Search, _workspaceInfo, appendPosition);
                results.Add(manualRename);
            }
            
            results.AddRange(GetAppRenameResults(_workspaceInfo, appendPosition));
            
            return results;
        }

        private IEnumerable<Result> GetAppRenameResults(WorkspaceInfo info, bool appendPosition)
        {
            foreach (var title in info.SortedWindowTitles)
            {
                yield return GetRenameResult(title, info, appendPosition);
            }
        }

        private Result GetRenameResult(string rawName, WorkspaceInfo info, bool appendPosition)
        {
            var newName = GetNewWorkspaceName(rawName, info.WorkspaceIdx, appendPosition);

            var title = string.IsNullOrWhiteSpace(rawName)
                ? $"Rename workspace: '{info.Name}' to '{newName}'"
                : $"Rename workspace '{info.Name}' to ...";

            return new()
            {
                Title = title,
                Action = _ =>
                {
                    if (_workspaceInfo == null)
                    {
                        var stateJson = ProcessCalls.GetStateJson();
                        _workspaceInfo ??= new WorkspaceInfo(stateJson);
                        newName = rawName;
                    }

                    ProcessCalls.RenameWorkspace(_workspaceInfo with { Name = newName });
                    _workspaceInfo = null;
                    return true;
                }
            };
        }

        private string GetNewWorkspaceName(string userInput, int idx, bool appendWorkspacePosition) =>
            appendWorkspacePosition
                ? $"{userInput} ({idx + 1})"
                : userInput;

        public Control CreateSettingPanel()
        {
            SettingsControl sc = new(_settings);
            return sc;
        }
    }
}