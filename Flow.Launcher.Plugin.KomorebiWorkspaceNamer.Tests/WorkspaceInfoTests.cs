namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer.Tests;


public class Tests
{
    [Test]
    public void ShouldBeAbleToCreateFromState()
    {
        var state = ProcessCalls.GetStateJson();
        var workspaceInfo = new WorkspaceInfo(state);
    }
}