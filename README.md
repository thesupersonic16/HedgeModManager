# Sonic Lost World Mod Loader
A Mod Loader for Sonic Lost World on the PC! :D Currently a WIP.

##How do I use this?
After [downloading the latest binary](https://github.com/Radfordhound/SLW-Mod-Loader/releases/latest), simply make a "mods" folder within your Sonic Lost World installation directory, then place a bunch of folders inside said mods directory (one for each mod) each containing all the modified files you'd like the game to load. Then, just fire up the mod loader (SLWModLoader.exe), check the checkbox(es) next to the mods you'd like to use in-game, and click "Play!"

###Mod installation tutorial by "Mac" (made for 1.4 but still applies to later revisions):

[![Mod installation tutorial](http://img.youtube.com/vi/u-5uCVJ8Ci0/0.jpg)](https://www.youtube.com/watch?v=u-5uCVJ8Ci0 "Mod installation tutorial")

##How do I release mods for this?
**The following section is for mod developers only. If all you want to do is play with some mods made by others, simply follow the above steps.**

Mods designed for the mod loader come in the form of folders that contain the following:

- A "mod.ini" file (a file which describes your mod, as well as all it's various details).
- A "disk" folder
  - A "sonic2013_patch_0" folder
    - All your modified files/folders from the root of Sonic Lost World's .cpk files on in their raw form (typically .pac files).

So long as the structure of your mod remains in this way, virtually any file in the game can be modified and released as part of your mod.

As an example, the extremely basic "Tanic the Hedgehog" recolor mod has a file/folder structure that goes like so:

- A "mod.ini" file
- A "disk" folder
  - A "sonic2013_patch_0" folder
    - Sonic.pac
    - Sonic.pac.00

Wereas the "MLG Speedrun Zone 1" mod (which modifies certain files not on the root of the .cpk) has a file/folder structure that goes like so:

- A "mod.ini" file
- A "disk" folder
  - A "sonic2013_patch_0" folder
    - A "set" folder
      - w1a01_obj_00.orc
      - w1a01_obj_01.orc
      - w1a01_obj_02.orc
      - w1a01_obj_03.orc
    - actstgmission.lua


###The mod.ini file
The mod.ini file is a mod configuration file that details all the user-friendly information about your mod, as well as how CPKREDIR should load the mod.

The version of the format used in the SLW Mod Loader is a variation on the format used in SonicGMI, with some minor changes/additions here and there.

Here's an example of a mod.ini file:
```
[Main]
IncludeDir0="."
IncludeDirCount=1
UpdateServer="https://dl.dropboxusercontent.com/s/4389t7gn44x8u9h/MLGSpeedRunUpdateFile.txt"

[Desc]
Title="MLG Speedrun Zone 1"
Description="BEAT WINDY HILL 1 LIKE A PRO NOSCOPER!!1!!1!"
Version="1.0"
Date="11/06/15"
Author="Radfordhound"
AuthorURL="https://www.youtube.com/user/Radfordhound"
URL="https://www.dropbox.com/sh/2zgsu1rwjt7ld42/AAA99UvcLlRWxpbLWCeiECt3a"
```

The following is a list of the most important values that can be used in a mod.INI file:

###Main

**IncludeDir 0-??** Specifies which folders will be included with your mod, allowing you to modify the default file/folder structure mentioned above.

**IncludeDirCount** Specifies how many folders will be included with your mod.

**UpdateServer** A modification of the existing SonicGMI value that specifies the link to a raw .txt file containing URLs in a particular format. This feature has **not yet been added**, and will be further detailed once it is. However, I recommend linking a .txt file just in case anyway, as it will allow you to release auto-downloading updates to your mods once the mod loader has been updated to support this feature.

###Desc

**Title** The name of your mod as shown in the mod loader.

**Description** A description of your mod that is shown when your mod is highlighted in the mod loader.
Typing a "\n" in this value will indicate a new line within the mod loader, **which should be done to keep your mods loadable!**

**Date** The date the mod was originally created as shown in the mod loader.

**Author** The author(s) of the mod. **You can include multiple authors in this value!** Simply seperate the authors via a space, followed by an ampersand, and another space. (Like this: "Radfordhound & Gotta Play Fast") They will be loaded as seperate authors within the mod loader, allowing you to link to them seperately.

**AuthorURL** The URL(s) to the author(s) of the mod. (Such as websites, YouTube channels, and social media accounts.) **You can include multiple authors in this value!** Simply seperate the authors' URLs via a space, followed by an ampersand, and another space. (Like this: "https://www.youtube.com/user/Radfordhound & https://www.youtube.com/channel/UCZfOGBkXRKICFozWU5bE0Xg") They will be loaded as seperate URLs within the mod loader and automatically linked with the data contained in the "Author" value, allowing you to link to them seperately.

**URL** The URL of the mod (aka mod homepages/threads/release videos).


There are many other values that can be used in a mod.ini file, many of which are already being used in several mods. So keep an eye out for em' in other released mods! :)
