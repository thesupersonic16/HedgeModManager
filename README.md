<h1>
    <a href="#--------------------hedge-mod-manager">
        <img width="48" align="left" src="https://github.com/thesupersonic16/HedgeModManager/raw/rewrite/HedgeModManager/Resources/Graphics/icon256.png">
    </a>
    Hedge Mod Manager
</h1>

A mod manager for Hedgehog Engine games on PC.

### Current Supported Games
- [Sonic Generations](https://store.steampowered.com/app/71340)
- [Sonic Lost World](https://store.steampowered.com/app/329440)
- [Sonic Forces](https://store.steampowered.com/app/637100)
- [Puyo Puyo Tetris 2](https://store.steampowered.com/app/1259790)

## So how do I use this?
Its simple, just grab the [latest version from GitHub](https://github.com/thesupersonic16/HedgeModManager/releases/) (or grab the latest compile from [AppVeyor](https://ci.appveyor.com/project/thesupersonic16/slw-mod-loader/branch/rewrite)) then extract all the files anywhere and run HedgeModManager.exe.

## How do I install mods?
There are multiple ways of installing mods, one of the easy ways of installing mods is by dragging its zip/7z/rar/folder into the mod list along with also being able to drag and drop multiple files and/or folders.

You can also install mods using [GameBanana's](https://gamebanana.com/games/6059) 1-Click Install button. 

Once your done, you can start checking the checkbox(es) of the mods and codes you want to play and click "Save and Play".

## How do I release mods for this?
**The following section is for mod developers only. If all you want to do is play with some mods made by others, simply follow the above steps.**

Mods designed for HedgeModManager needs to come in the form of folders that contain the following:

- A "mod.ini" file (a file which describes your mod, as well as all it's various details).
- A "disk" folder
  - A "bb" folder (for Sonic Generations)
    - All your modified files/folders from the root of Sonic Generations' bb.cpk in their raw form.
  - A "bb2" folder (for Sonic Generations)
    - All your modified files/folders from the root of Sonic Generations' bb2.cpk in their raw form.
  - A "bb3" folder (for Sonic Generations)
    - All your modified files/folders from the root of Sonic Generations' bb3.cpk in their raw form.
  - A "sonic2013_patch_0" folder (for Sonic Lost World)
    - All your modified files/folders from the root of Sonic Lost World's sonic2013_0.cpk in their raw form.
  - A "wars_patch" folder (for Sonic Forces)
    - All your modified files/folders from the root of Sonic Forces' wars_0.cpk and wars_1.cpk in their raw form.
- A "raw" folder (for Puyo Puyo Tetris 2)
  - All your modified files/folders from the root of Puyo Puyo Tetris 2' raw folder in their raw form.

So long as the structure of your mod remains in this way, virtually any file in the game can be modified and released as part of your mod.

As an example, a basic (Modern) Sonic character mod would have a file/folder structure that goes like so:
- A "mod.ini" file
- A "disk" folder
  - A "bb3" folder (Sonic Generations)
    - Sonic.ar.00
    - Sonic.arl
  - A "sonic2013_patch_0" folder (Sonic Lost World)
    - Sonic.pac
    - Sonic.pac.00
  - A "wars_patch" folder (Sonic Forces)
    - A "character" folder
      - Sonic.pac
      - Sonic.pac.000
- A "raw" folder (Puyo Puyo Tetris 2)
  - A "ui" folder
    - A "cutin" folder
      - ui_cutin_2p_son.pac
      - ui_cutin_4p_son.pac

### The mod.ini file
The mod.ini file is a mod configuration file that details all the information about your mod, as well as how CPKREDIR should load the mod.

The version of the format used in the HedgeModManager is a variation on the format used in SLWModLoader and SonicGMI, with some minor changes/additions here and there.

Here's a mod.ini from a Sonic Generations mod by PTKickass:
```ini
[Main]
UpdateServer="https://raw.githubusercontent.com/PTKickass/ModUpdates/master/SonicGenerations/SonicUnleashedHUD/"
IncludeDir0="."
IncludeDirCount=1
DLLFile="UnleashedHUD.dll"

[Desc]
Title="Sonic Unleashed HUD"
Description="A Sonic Generations mod that partially ports the Sonic Unleashed HUD"
Version="1.3.2"
Date="2020/02/20"
Author="PTKickass"
AuthorURL="https://www.youtube.com/user/ENPTKickass"
```

The following is a list of the most important values that can be used in a mod.ini file:

### Main

**IncludeDir?** Specifies which folders the mod loader will load for your mod. The base of the path is your mod folder. For most mods this must point to the folder with the `disk` folder. If it's the root of your mod folder **YOU MUST USE** `.` like `IncludeDir0="."` as for example. Getting this wrong will result your mod **NOT** loading. (`?` is a Zero-based Number)
Example:
```ini
[Main]
IncludeDir0="." ; Will match to "mods/<MOD NAME>/"
IncludeDirCount=1 ; Amount of includes, if unsure keep at 1
```

**IncludeDirCount** Specifies how many folders will be included with your mod.

**UpdateServer** A URL to a directory on a HTTP or HTTPS server containing a mod_version.ini and mod_files.txt file using the SonicGMI update format.
Example:
```ini
UpdateServer="https://raw.githubusercontent.com/PTKickass/ModUpdates/master/SonicGenerations/SonicUnleashedHUD/"
```

**DLLFile** A path to the mod's DLL file. Only use this if the mod contains a dll file. If not remove this line.

**SaveFile** A path to a save file or to where you want the file save to be located. This only works if save redirection is enabled, and is recommended to be used if the mod needs a curtain progress or if the mod risks breaking save files.

### Desc

**Title** The name of your mod as shown in the manager.

**Description** A description of your mod that is shown in the description window in the manager.
Typing a "\n" in this value will indicate a new line within the manager, **which should be done to keep your mods loadable!**

**Date** The date the mod was originally created as shown in the manager.

**Author** The author(s) of the mod. **You can include multiple authors in this value!** Simply seperate the authors via a space, followed by an ampersand, and another space. (Like this: "Radfordhound & Gotta Play Fast") They will be loaded as seperate authors within the manager, allowing you to link to them seperately.

**AuthorURL** The URL(s) to the author(s) of the mod. (Such as websites, YouTube channels, and social media accounts.) **You can include multiple authors in this value!** Simply seperate the authors' URLs via a space, followed by an ampersand, and another space. (Like this: "https://www.youtube.com/user/Radfordhound & https://www.youtube.com/channel/UCZfOGBkXRKICFozWU5bE0Xg") They will be loaded as seperate URLs within the manager and automatically linked with the data contained in the "Author" value, allowing you to link to them seperately.

**URL** The URL of the mod (aka mod homepages/threads/release videos).

There are many other values that can be used in a mod.ini file, many of which are already being used in several mods. So keep an eye out for them in other released mods!
 
## How do I allow mod updating?
HedgeModManager uses a custom mod updater which is also backwards compatible with SonicGMI.
 
To get started, before releasing your mod, you will need access to a HTTP server with a folder for your mod. From here this folder will be refered as the update server.
 
In your mod.ini file add a field in the `[Main]` section called `UpdateServer` and set the value to the URL of your mod update folder including the forward slash at the end.
Example:
```ini
UpdateServer="https://raw.githubusercontent.com/PTKickass/ModUpdates/master/SonicGenerations/SonicUnleashedHUD/"
```

### Preparing the server
On your server create a folder for the mod you want to allow updating for and create two files `mod_files.txt` which will be blank for now and a file called `mod_version.ini` This file will contain the information about the update and the changelog for SonicGMI users. 
 
Here is an example of a mod_version.ini for HedgeModManager and SonicGMI:
```ini
[Main]
VersionString="2.0"
DownloadSizeString="0.7 MB"
Markdown="changelog.md"

[Changelog]
StringCount=1
String0="Updated for GCL v2.1"
```

The following is a list of values that can be used in a mod_version.ini file:

### Main

**VersionString** The version of the mod you will be publishing.

**DownloadSizeString** The size of the mod update that will be shown to the SonicGMI users.

**Markdown** The name of the file on the update server containing markdown code for showing information about the mod update like the changelog. This value is optional, if this is defined then HedgeModManager will use the markdown code to show the information instead, if not the information in `[Changelog]` will be used instead.

### Changelog 

**StringCount** The amount of strings to show to the user. Tip: Set this to the last string number + 1.

**String?** Line about a change that was made in the mod that is going to be updated. You can have many of these by changing the *?* to a sequential number starting from 0. e.g. String0, String1, String2, String3 
 
If you have not made an update just yet then I would recommend:
- Setting `VersionString` to the current version of the mod
- Setting `DownloadSizeString` to "0 MB"
- Setting `StringCount` to `0`
- Creating a black file called `mod_files.txt` in the update server
- Including a blank file in `Markdown` (If you want markdown support)
 
Once you have made an update, you will be modifying all these.
 
### Preparing to publish an update
Once you have made some changes to your mod and have pick out the files you have changed (Do not include files that are untouched from the first release to the current!)

Now you can start moving the files into your update server folder (next to the mod_version.ini and mod_files.txt file) and record down the changes in your mod_files.txt file

### mod_files.txt
The mod_files.txt file is a simple file containing simple commands for HedgeModManager and SonicGMI to follow to push your update to all the users. 
 
the mod_files.txt format contains two commands:
 - **add** Downloads a file from the update server into the mod folder on the user's machine 
 - **delete** Deletes a file from the user's mod folder, Use this if you nolonger need the file being replaced.
 
Here is an example of a mod_files.txt
```
add mod.ini
add UnleashedHUD.ini
add UnleashedHUD.dll
add disk/bb/SonicActionCommonHud.ar.00
add disk/bb/SonicActionCommonHud.ar.01
add disk/bb/SonicActionCommonHud.arl
add disk/bb/SonicActionCommon.ar.00
add disk/bb/SonicActionCommon.arl
add disk/bb/SystemCommon.ar.00
add disk/bb/SystemCommon.arl
add disk/bb/Languages/English/SonicActionCommonHud.ar.00
add disk/bb/Languages/English/SonicActionCommonHud.arl
add disk/bb/Languages/French/SonicActionCommonHud.ar.00
add disk/bb/Languages/French/SonicActionCommonHud.arl
add disk/bb/Languages/German/SonicActionCommonHud.ar.00
add disk/bb/Languages/German/SonicActionCommonHud.arl
add disk/bb/Languages/Italian/SonicActionCommonHud.ar.00
add disk/bb/Languages/Italian/SonicActionCommonHud.arl
add disk/bb/Languages/Japanese/SonicActionCommonHud.ar.00
add disk/bb/Languages/Japanese/SonicActionCommonHud.arl
add disk/bb/Languages/Spanish/SonicActionCommonHud.ar.00
add disk/bb/Languages/Spanish/SonicActionCommonHud.arl
add disk/bb3/SonicActionCommonHud.ar.00
add disk/bb3/SonicActionCommonHud.arl
add disk/bb3/SonicActionCommon.ar.00
add disk/bb3/SonicActionCommon.ar.01
add disk/bb3/SonicActionCommon.arl
add disk/bb2/HowTo.ar.00
add disk/bb2/HowTo.arl
add disk/bb2/pam_cmn.ar.00
add disk/bb2/pam_cmn.arl
add disk/bb3/Application.ar.00
add disk/bb3/Application.arl
delete disk/bb/SonicActionCommon.ar.01
```
 
Once you have finished recording your mod_files.txt file, you can now start modifying your mod_version.ini file that you have created at the start.

In your mod_version.ini file you would want to perform the following changes:
- Set `VersionString` to the new version of the mod
- Add up all the files that will be downloaded and set `DownloadSizeString` to the file size e.g. "48 MB"
- Write your changelog in `[Changelog]`

After writing your changes in mod_version.ini you can now start writing your changelog in markdown (This is if you want to have a markdown changelog for HedgeModManager users)
 
Here is an example of a markdown changelog:
```md
## Sonic Unleashed HUD v1.3.2
Unleashed Menu Sounds fixes:
- Fixed press start volume. Now it's full stereo on it's own without relying on two Synths;
- Fixed stage title reveal sound. Sonic Team used two synths to make the sound stereo, so now it's real stereo with only one sound.

## Sonic Unleashed HUD v1.3.1
- *forgot to include the Bumper Textures woops...*

## Sonic Unleashed HUD v1.3
- Remade PlayStation buttons to be higher res and have even better alignement;
- Overhauled menu sounds by Mario Tainaka.

## Sonic Unleashed HUD v1.2.1
- Adjusted PlayStation bumper alignement/size.

## Sonic Unleashed HUD v1.2
- Added support for PlayStation button prompts. Please edit the "UnleashedHUD.ini" file in the mod's root directory.

## Sonic Unleashed HUD v1.1.3
- Fixed **AGAIN** the broken "Ready GO!" intro animation when the game is set to English.

## Sonic Unleashed HUD v1.1.2
- Fixed broken "Ready GO!" intro animation when the game is set to English.

## Sonic Unleashed HUD v1.1
- Fixed boost bar "Ring Energy" text not displaying in languages other than English and Japanese;
- Added multiple language support for the "Ready GO!" intro animation.

## Sonic Unleashed HUD v1.0.1 (RC2)
- Adjusted slant angle on boost bar;
- Public release!

## Sonic Unleashed HUD v1.0 (RC1)
- First release candidate.
```
