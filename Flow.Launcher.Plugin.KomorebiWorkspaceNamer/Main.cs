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
            var newName = GetWorkspaceNameWithPos(rawName, info.WorkspaceIdx, appendPosition);
            var oldNameWithNoPosition = GetWorkspaceNameWithoutPos(rawName, appendPosition);
            
            var title = string.IsNullOrWhiteSpace(rawName)
                ? $"Rename workspace '{info.Name}' to ..."
                : $"Rename workspace: '{info.Name}' to '{newName}'";

            return new()
            {
                Title = title,
                Action = _ =>
                {
                    if (_workspaceInfo == null)
                    {
                        var stateJson = ProcessCalls.GetStateJson();
                        _workspaceInfo ??= new WorkspaceInfo(stateJson);
                        newName = GetWorkspaceNameWithPos(rawName, info.WorkspaceIdx, appendPosition);;
                    }

                    ProcessCalls.RenameWorkspace(_workspaceInfo with { Name = newName });
                    _context.API.ChangeQuery("", true);
                    _workspaceInfo = null;
                    return true;
                }
            };
        }
        

        private string GetWorkspaceNameWithPos(string rawName, int idx, bool appendWorkspacePosition) =>
            appendWorkspacePosition
                ? AppendPosition(rawName, idx)
                : rawName;

        private string AppendPosition(string text, int idx) => $"{text} ({idx + 1})";

        private string GetWorkspaceNameWithoutPos(string rawName, bool appendWorkspacePosition) =>
            appendWorkspacePosition
                ? RemovePosition(rawName)
                : rawName;

        private string RemovePosition(string text)
        {
            var endAt = text.LastIndexOf('(');

            if (endAt == -1)
            {
                return text;
            }
            
            Range selectionRange = new(0, endAt - 1);
            return text[selectionRange];
        }
        public Control CreateSettingPanel()
        {
            SettingsControl sc = new(_settings);
            return sc;
        }
    }
}