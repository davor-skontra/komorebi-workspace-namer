using System;
using System.Text.Json;
using Flow.Launcher.Plugin.KomorebiWorkspaceNamer.StateTypes;
using RomanNumerals;

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer;

public class IndexStyler
{
    private readonly Kind _kind;
    private readonly string _rawText;
    private readonly WorkspaceInfo _info;

    private Lazy<KomorebiConfig> _komorebiConfig = new(GetKomorebiConfig);

    public enum Kind
    {
        None,
        ArabicNumerals,
        RomanNumerals,
        FromKomorebiConfig
    }

    private static KomorebiConfig GetKomorebiConfig()
    {
        var json = ProcessCalls.GetKomorebiConfigJson()!;
        return JsonSerializer.Deserialize<KomorebiConfig>(json)!;
    }

    public IndexStyler(Kind kind, string rawText, WorkspaceInfo info)
    {
        _kind = kind;
        _rawText = rawText;
        _info = info;
    }
    
    public string GetMarkedName() => _kind == Kind.None
        ? _rawText
        : $"{_rawText} ({GetMarker()})";

    private string GetMarker()
    {
        switch (_kind)
        {
            case Kind.None:
                return "";
            case Kind.ArabicNumerals:
                return (_info.WorkspaceIdx + 1).ToString();
            case Kind.RomanNumerals:
                return new RomanNumeral(_info.WorkspaceIdx + 1).ToString();
            case Kind.FromKomorebiConfig:
                return _komorebiConfig
                    .Value
                    .Monitors[_info.MonitorIdx]
                    .Workspaces[_info.WorkspaceIdx]
                    .Name;
            default:
                throw new ArgumentOutOfRangeException(nameof(_kind), _kind, null);
        }
    }
    
    public static string RemovePosition(string text)
    {
        var endAt = text.LastIndexOf('(');

        if (endAt == -1)
        {
            return text;
        }
            
        Range selectionRange = new(0, endAt - 1);
        return text[selectionRange];
    }
}