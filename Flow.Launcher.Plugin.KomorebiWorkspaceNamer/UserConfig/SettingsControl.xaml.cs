
using System;
using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer.UserConfig;

public partial class SettingsControl: UserControl
{
    public SettingsControl(Settings settings)
    {
        InitializeComponent();
        IndexingStyleLb.ItemsSource = Enum.GetNames<Settings.IndexStyleKind>();
        DataContext = settings;
    }

    private void SetIndexingStyle(object sender, SelectionChangedEventArgs e)
    {
        var content = IndexingStyleLb.SelectedItems[0]?.ToString();
        (DataContext as Settings)!.IndexStyle = Enum.Parse<Settings.IndexStyleKind>(content!);
    }
}