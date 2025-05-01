using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Flow.Launcher.Plugin.KomorebiWorkspaceNamer.StateTypes;

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer;

public record WorkspaceInfo
{
    public int MonitorIdx { get; init; }
    public int WorkspaceIdx { get; init; }
    public string Name { get; init; }

    private HashSet<string> _windowTitlesSorted;
    public IEnumerable<string> SortedWindowTitles => _windowTitlesSorted;
    
    public WorkspaceInfo(string stateJson)
    {
        var state = JsonSerializer.Deserialize<State>(stateJson)!;
        var activeMonitorIdx = (int) state
            .Monitors.Focused;
        var activeWorkspaceIdx = (int) state
            .Monitors.Elements[activeMonitorIdx]
            .Workspaces.Focused;
        var activeWorkspace = state
            .Monitors.Elements[activeMonitorIdx]
            .Workspaces.Elements[activeWorkspaceIdx];
            
            var name = activeWorkspace.Name;
                
        MonitorIdx = activeMonitorIdx;
        WorkspaceIdx = activeWorkspaceIdx;
        Name = name;

        _windowTitlesSorted = GetSortedWindowTitles(state, activeWorkspace);
    }

    private HashSet<string> GetSortedWindowTitles(State state, WorkspacesElement workspace)
    {
        var containerIdx =  workspace.Containers.Focused;
        var windows = workspace
            .Containers
            .Elements[containerIdx]
            .Windows;
        
        var windowIdx = windows.Focused;
        var windowTitle = windows.Elements[windowIdx].Title;

        HashSet<string> titles = new() { windowTitle };
        
        foreach (var containerElement in workspace.Containers.Elements)
        {
            foreach (var windowsElement in containerElement.Windows.Elements)
            {
                titles.Add(windowsElement.Title);
            }
        }

        return titles;
    }
}