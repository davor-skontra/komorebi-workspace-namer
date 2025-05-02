using System.Diagnostics;

namespace Flow.Launcher.Plugin.KomorebiWorkspaceNamer;

public static class ProcessCalls
{
    public static string GetStateJson()
    {
        using Process process = new();
        process.StartInfo.FileName = "powershell.exe";
        process.StartInfo.Arguments = "-c komorebic state";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        return output;
    }

    public static void RenameWorkspace(WorkspaceInfo info, string name)
    {
        using Process process = new();
        process.StartInfo.FileName = "powershell.exe";
        process.StartInfo.Arguments = $"-c komorebic workspace-name {info.MonitorIdx} {info.WorkspaceIdx} \"\"\"{name}\"\"\"";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
    }
}