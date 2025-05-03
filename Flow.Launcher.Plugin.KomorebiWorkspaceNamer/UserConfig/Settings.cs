using System.Collections;
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer.UserConfig;

public class Settings
{
    private const char PresetNamesSeparator = ',';
    public IndexStyler.Kind IndexStyler { get; set; } = Plugin.KomorebiWorkspaceNamer.IndexStyler.Kind.ArabicNumerals;
    public string PresetNames { get; set; } = "";

    public IEnumerable<string> GetPresetNames()
    {
        foreach (var name in PresetNames.Split(PresetNamesSeparator))
        {
            yield return name.Trim();
        }
    }
}