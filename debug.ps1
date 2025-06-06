dotnet publish Flow.Launcher.Plugin.KomorebiWorkspaceNamer -c Debug -r win-x64 --no-self-contained

$AppDataFolder = [Environment]::GetFolderPath("ApplicationData")
$flowLauncherExe = "$env:LOCALAPPDATA\FlowLauncher\Flow.Launcher.exe"

if (Test-Path $flowLauncherExe) {
    Stop-Process -Name "Flow.Launcher" -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2

    if (Test-Path "$AppDataFolder\FlowLauncher\Plugins\KomorebiWorkspaceNamer") {
        Remove-Item -Recurse -Force "$AppDataFolder\FlowLauncher\Plugins\KomorebiWorkspaceNamer"
    }

    Copy-Item "Flow.Launcher.Plugin.KomorebiWorkspaceNamer\bin\Debug\win-x64\publish" "$AppDataFolder\FlowLauncher\Plugins\" -Recurse -Force
    Rename-Item -Path "$AppDataFolder\FlowLauncher\Plugins\publish" -NewName "KomorebiWorkspaceNamer"
    Copy-Item "Flow.Launcher.Plugin.KomorebiWorkspaceNamer\Icon.png" "$AppDataFolder\FlowLauncher\Plugins\KomorebiWorkspaceNamer\Icon.png" -Force

    Start-Sleep -Seconds 2
    Start-Process $flowLauncherExe
} else {
    Write-Host "Flow.Launcher.exe not found. Please install Flow Launcher first"
}
