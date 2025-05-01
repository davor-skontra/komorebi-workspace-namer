
using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer.UserConfig;

public partial class SettingsControl: UserControl
{
    public SettingsControl(Settings settings)
    {
        InitializeComponent();
        DataContext = settings;
    }
}