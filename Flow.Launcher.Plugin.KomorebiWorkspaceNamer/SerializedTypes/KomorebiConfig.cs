using System;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer.StateTypes;

public class KomorebiConfig
{
    [JsonPropertyName("monitors")]
    public Monitor[] Monitors { get; set; }
}

public class Monitor
{
    [JsonPropertyName("workspaces")]
    public Workspace[] Workspaces { get; set; }
}

public class Workspace
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}