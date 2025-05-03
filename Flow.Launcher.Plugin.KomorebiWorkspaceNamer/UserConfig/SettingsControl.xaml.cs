
using System;
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

    private void SetIndexingStyle(object sender, SelectionChangedEventArgs e)
    {
        var content = IndexingStyleLb.SelectedItems[0]?.ToString();
        (DataContext as Settings)!.IndexStyler = Enum.Parse<IndexStyler.Kind>(content!);
    }
    
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        (DataContext as Settings)!.PresetNames = PredefinedNamesTb.Text;
    }

    private void OnIndexingStyleLoaded(object sender, RoutedEventArgs e)
    {
        var names = Enum.GetNames<IndexStyler.Kind>();
        IndexingStyleLb.ItemsSource = names;
        IndexingStyleLb.SelectedItem = (DataContext as Settings)!.IndexStyler.ToString();
    }

    private void OnPredefinedNamesTbLoaded(object sender, RoutedEventArgs e)
    {
        PredefinedNamesTb.Text = (DataContext as Settings)!.PresetNames;
    }
}