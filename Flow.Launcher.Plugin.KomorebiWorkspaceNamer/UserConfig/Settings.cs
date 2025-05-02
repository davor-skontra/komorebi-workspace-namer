namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer.UserConfig;

public class Settings
{
    public enum IndexStyleKind
    {
        None,
        ArabicNumerals,
        RomanNumerals,
        FromKomorebiConfig
    }

    public IndexStyleKind IndexStyle = IndexStyleKind.ArabicNumerals;
}