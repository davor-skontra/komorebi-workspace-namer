namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer.Tests;

public class ProcessCallsTests
{
    [Test]
    public void ShouldFindConfigFile()
    {
        var result = ProcessCalls.GetKomorebiConfigJson();
        
        Assert.That(result, Is.Not.Empty);
    }
}