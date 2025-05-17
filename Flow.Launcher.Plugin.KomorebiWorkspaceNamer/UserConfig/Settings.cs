using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer.UserConfig;

public class Settings
{
    private const char PresetNamesSeparator = ',';

    public delegate void StyleChangedDelegate(IndexStyler.Kind previous, IndexStyler.Kind current);

    public event StyleChangedDelegate? StyleChangeEvent;
    
    private IndexStyler.Kind _indexStyler = Plugin.KomorebiWorkspaceNamer.IndexStyler.Kind.ArabicNumerals;
    public IndexStyler.Kind IndexStyler
    {
        get => _indexStyler;
        set
        {
            var previous = _indexStyler;
            _indexStyler = value;
            StyleChangeEvent?.Invoke(previous, _indexStyler);
        }
    }

    public string PresetNames { get; set; } = "";

    public IEnumerable<string> GetPresetNames()
    {
        foreach (var name in PresetNames.Split(PresetNamesSeparator))
        {
            if (name == "")
            {
                continue;
            }
            yield return name.Trim();
        }
    }
}