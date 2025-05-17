using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Flow.Launcher.Plugin.KomorebiWorkspaceNamer.StateTypes;

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer;

public record WorkspaceInfo
{
    public int MonitorIdx { get; init; }
    public int WorkspaceIdx { get; init; }
    public string Name { get; init; }

    private HashSet<string>? _windowTitlesSorted;
    public IEnumerable<string>? SortedWindowTitles => _windowTitlesSorted;
    
    private WorkspaceInfo(State state, int monitorIdx, int workspaceIdx, bool trackSortedWindows)
    {
        
        var workspace = state
            .Monitors.Elements[monitorIdx]
            .Workspaces.Elements[workspaceIdx];

        MonitorIdx = monitorIdx;
        WorkspaceIdx = workspaceIdx;
        
        Name = workspace.Name ?? (WorkspaceIdx + 1).ToString();
        
        if (trackSortedWindows)
        {
            _windowTitlesSorted = GetSortedWindowTitles(workspace) 
                                  ?? new HashSet<string>();
        }

    }

    public static IEnumerable<WorkspaceInfo> CreateAllFrom(State state)
    {
        for (var monitorIdx = 0; monitorIdx < state.Monitors.Elements.Length; monitorIdx++)
        {
            var monitorsElement = state.Monitors.Elements[monitorIdx];
            for (var workspaceIdx = 0; workspaceIdx < monitorsElement.Workspaces.Elements.Length; workspaceIdx++)
            {
                const bool shouldTrackWindows = false;
                WorkspaceInfo info = new(state, monitorIdx, workspaceIdx, shouldTrackWindows);
                yield return info;
            }
        }
    }

    public static WorkspaceInfo CreateFrom(State state)
    {
        var monitorIdx = (int) state
            .Monitors.Focused;
        var workspaceIdx = (int) state
            .Monitors.Elements[monitorIdx]
            .Workspaces.Focused;

        const bool trackWindows = true;
        WorkspaceInfo info = new(state, monitorIdx, workspaceIdx, trackWindows);
        return info;
    }

    private HashSet<string>? GetSortedWindowTitles(WorkspacesElement workspace)
    {
        var containerIdx =  workspace.Containers.Focused;
        var containerElements = workspace
            .Containers
            .Elements;

        if (!containerElements.Any())
        {
            return null;
        }
        
        var windows = containerElements[containerIdx].Windows;
        
        var windowIdx = windows.Focused;
        var windowElements = windows.Elements;

        if (!windowElements.Any())
        {
            return null;
        }
        
        var windowTitle = windowElements[windowIdx].Title;

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