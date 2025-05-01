using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Controls;
using Flow.Launcher.Plugin.KomorebiWorkspaceNamer.StateTypes;
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
                var stateMsg = GetStateMsg();
                _workspaceInfo ??= GetActiveWorkspaceInfo(stateMsg);
            }

            var newTitle = GetNewTitle(query.Search, _workspaceInfo.WorkspaceIdx, _settings.AppendPosition);
            
            Result r = new()
            {
                Title = $"Renaming workspace: '{_workspaceInfo.Name}' to '{newTitle}'",
                Action = _ =>
                {
                    if (_workspaceInfo == null)
                    {
                        var stateMsg = GetStateMsg();
                        _workspaceInfo ??= GetActiveWorkspaceInfo(stateMsg);
                        newTitle = query.Search;
                    }

                    RenameWorkspace(_workspaceInfo with { Name = newTitle});
                    _workspaceInfo = null;
                    return true;
                }
            };
            return new List<Result>{r};
        }
        
            
        private WorkspaceInfo GetActiveWorkspaceInfo(string stateMsg)
        {
            Console.WriteLine($"State is: {stateMsg}");
            var state = JsonSerializer.Deserialize<State>(stateMsg)!;
            var activeMonitor = (int) state
                .Monitors.Focused;
            var activeWorkspace = (int) state
                .Monitors.Elements[activeMonitor]
                .Workspaces.Focused;
            var name = state
                .Monitors.Elements[activeMonitor]
                .Workspaces.Elements[activeWorkspace]
                .Name;
            return new WorkspaceInfo(activeMonitor, activeWorkspace, name);
        }

        private string GetNewTitle(string userInput, int idx, bool appendWorkspacePosition) =>
            appendWorkspacePosition
                ? $"{userInput} ({idx + 1})"
                : userInput;

        private record WorkspaceInfo(int MonitorIdx, int WorkspaceIdx, string Name);

        private string GetStateMsg()
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