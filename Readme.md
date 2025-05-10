![Project logo. It's a pixel art picture of a watermelon with the letters KWN stenciled on it. Al done in the commodore 64 color palette](/Flow.Launcher.Plugin.KomorebiWorkspaceNamer/icon.png)

Komorebi Workspace Namer
==================

A plugin for the [Flow launcher](https://github.com/Flow-Launcher/Flow.Launcher) that lets you quickly name your focused Komorebi workspace.

Current version is mvp and is not yet published on the flow plugin repo.

### Usage
Type: 
`wn <new workspace name>`
And hit enter to rename your currently selected workspace.
You can also select any option that shows up after you type wn.

These are currently generated from tiles of visible windows on the active worskpace. 

You can also add your own presets in settings which is really useful if you have some often needed names like "Work, Personal, Art, Development etc".


### Settings
![image](https://github.com/user-attachments/assets/17a765af-31d5-4c54-a7c5-df1e68803439)

#### Predefined names

If you want some commonly used names to appear as options when you type ve, add them into settings, separated by commas

#### Workspace indicator
By default any name will be suffixed with an arabic numeral in paranthesis (eg `(1)`). This is inteded to mark which workspace you are on. In settings you can change this to use
- None
- Arabic Numeral
- Roman Numeral
- Workspace names from your Komorebi config file. (`komrebi.json`)

Example when using Arabic numerals as indicators.

![image](https://github.com/user-attachments/assets/92cbdd90-5463-4fba-860e-0fdbe6501222)

Note that the indicator is only changed after you rename any workspace, just changing the setting does not automatically rename all your workspaces.
