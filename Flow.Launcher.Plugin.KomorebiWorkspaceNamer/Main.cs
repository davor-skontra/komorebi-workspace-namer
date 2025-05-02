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

            var appendPosition = _settings.AppendPosition;
            List<Result> results = new();
            
            var manualRename = GetRenameResult(query.Search, workspaceInfo, appendPosition);
            results.Add(manualRename);
            results.AddRange(GetAppRenameResults(workspaceInfo, appendPosition));
            
            return results;
        }

        private WorkspaceInfo GetWorkspaceInfo()
        {
            var stateJson = ProcessCalls.GetStateJson();
            return new WorkspaceInfo(stateJson);
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
            var nameWithPosition = GetWorkspaceNameWithPos(rawName, info.WorkspaceIdx, appendPosition);
            var oldNameWithNoPosition = GetWorkspaceNameWithoutPos(info.Name, appendPosition);
            
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