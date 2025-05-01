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
                var stateJson = GetStateJson();
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

            return new()
            {
                Title = $"Rename workspace: '{info.Name}' to '{newName}'",
                Action = _ =>
                {
                    if (_workspaceInfo == null)
                    {
                        var stateJson = GetStateJson();
                        _workspaceInfo ??= new WorkspaceInfo(stateJson);
                        newName = rawName;
                    }

                    RenameWorkspace(_workspaceInfo with { Name = newName });
                    _workspaceInfo = null;
                    return true;
                }
            };
        }

        private string GetNewWorkspaceName(string userInput, int idx, bool appendWorkspacePosition) =>
            appendWorkspacePosition
                ? $"{userInput} ({idx + 1})"
                : userInput;

        private string GetStateJson()
        {
            using Process process = new();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = "-c komorebic state";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            return output;
        }

        private void RenameWorkspace(WorkspaceInfo info)
        {
            using Process process = new();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = $"-c komorebic workspace-name {info.MonitorIdx} {info.WorkspaceIdx} \"\"\"{info.Name}\"\"\"";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
        }

        public Control CreateSettingPanel()
        {
            SettingsControl sc = new(_settings);
            return sc;
        }
    }
}