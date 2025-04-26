
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618
#pragma warning disable CS8601
#pragma warning disable CS8603

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer.StateTypes
{
    using System;
    using System.Collections.Generic;

    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Globalization;

    public class State
    {
        [JsonPropertyName("monitors")] 
        public Monitors Monitors { get; set; }
    }

    public class Monitors
    {
        [JsonPropertyName("elements")]
        public MonitorsElement[] Elements { get; set; }

        [JsonPropertyName("focused")]
        public long Focused { get; set; }
    }

    public class MonitorsElement
    {

        [JsonPropertyName("workspaces")]
        public Workspaces Workspaces { get; set; }
        
    }
    

    public class Workspaces
    {
        [JsonPropertyName("elements")]
        public WorkspacesElement[] Elements { get; set; }

        [JsonPropertyName("focused")]
        public long Focused { get; set; }
    }

    public class WorkspacesElement
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
   