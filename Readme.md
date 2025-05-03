Flow.Launcher.Plugin.KomorebiWorkspaceNamer
==================

A plugin for the [Flow launcher](https://github.com/Flow-Launcher/Flow.Launcher) that lets you quickly name your focused Komorebi workspace.

Current version is mvp and is not yet published on the flow plugin repo.

### Usage
Type: 
'wn <new workspace name>'
And hit enter to rename your currently selected workspace.
You can also select any option that shows up after you type wn. As of version 0.0.2 these are currently generated from tiles of visible windows on the active worskpace.


### Settings

By default any name will be suffixed with an arabic numeral in paranthesis (eg '(1)'). This is inteded to mark which workspace you are on. In settings you can change this to use
- None
- Arabic Numeral
- Roman Numeral
- Workspace names from your Komorebi config file. ('komrebi.json')