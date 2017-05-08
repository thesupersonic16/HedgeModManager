# Sonic Lost World Mod Loader
A mod loader for Sonic Lost World and for Sonic Generations on the PC! This is currently a work in progress.

## For those pirating Sonic Lost World/Sonic Generations:
Please purchase Sonic Lost World off the [Steam store](http://store.steampowered.com/app/329440/). This is the only legal way of purchasing Sonic Lost World.

Version 6.0 now has support for Sonic Generations, and the same deal applies to Generations as well. Please purchase Sonic Generations off the [Steam store](http://store.steampowered.com/app/71340/).

SLW Mod Loader does not support pirated copies of SLW/SG, nor will it ever support pirated copies.
<!---
## How do I use this?
After [downloading the latest binary](https://github.com/thesupersonic16/SLW-Mod-Loader/releases/latest), Simply extract all the contents and run "SLWModLoader.exe" and click "OK" after that it should go through all of your Steam libraries on your computer searching for Sonic Lost World or Sonic Generations, Click "Yes" once its finds the game directory you want to install it into, By clicking "No" It will continue searching for other locations. After that it should ask to create a "mods" folder if you don't have one yet, Now you might get another dialog asking you if you want to patch your executable, If you get this then just click "Yes" as its required if you want to play with mods. Now If nothing goes wrong you be greeted with the main ModLoader Window, From here you can drag your mods (Folder, .zip, .7z, .rar) into the ModLoader and it should automatically figure out how to install it, Once you have atleast one mod a list should show up alowing you to choose what mods to load, check the checkbox(es) next to the mods you'd like to use in-game, and click "Save and Play"
-->

## How do I use this?
Its simple, Just get the [latest binary from here](https://github.com/thesupersonic16/SLW-Mod-Loader/releases/latest) (or compile the sourcecode yourself) and extract all the files anywhere and run SLWModLoader.exe and click "Yes"/"OK" on the dialogs.

If you haven't modded SG or SLW before, You might get alot of dialogs, which is not a problem as you can just click "Yes"/"OK" on pretty much all of them.

SLWModLoader can also attempt to find your game directory and copy its files there if you didn't extract the files into your game directory, which can make installation much easier if you end up forgetting where you installed SG or SLW or don't know how find it. This works by searching for SG or SLW in your Steam's libraries in the background. **Note**: This will not work if the game is not in a Steam library.

You should be at the main Modloader window If nothing went wrong. From here you can start adding mods by dragging its zip/7z/rar/folder into the modloader's Window and the ModLoader should automatically figure out how to install that mod (You can also drag and drop multiable files/folders).

Once your done, you can start checking the checkbox(es) and click "Save and Play".

<!---
### Mod installation tutorial by "Mac" (made for 1.4 but still applies to later revisions):

[![Mod installation tutorial](http://img.youtube.com/vi/u-5uCVJ8Ci0/0.jpg)](https://www.youtube.com/watch?v=u-5uCVJ8Ci0 "Mod installation tutorial")
-->
## How do I release mods for this?
**The following section is for mod developers only. If all you want to do is play with some mods made by others, simply follow the above steps.**

Mods designed for the mod loader come in the form of folders that contain the following:

- A "mod.ini" file (a file which describes your mod, as well as all it's various details).
- A "disk" folder
  - A "sonic2013_patch_0" folder (for Sonic Lost World)
    - All your modified files/folders from the root of Sonic Lost World's .cpk files on in their raw form (typically .pac files).
  - A "bb" folder (for Sonic Generations)
    -  All your modified files/folders from the root of Sonic Generations' bb.cpk in their raw form.
  - A "bb2" folder (for Sonic Generations)
    -  All your modified files/folders from the root of Sonic Generations' bb2.cpk in their raw form.
  - A "bb3" folder (for Sonic Generations)
    -  All your modified files/folders from the root of Sonic Generations' bb3.cpk in their raw form.

So long as the structure of your mod remains in this way, virtually any file in the game can be modified and released as part of your mod.

As an example, the extremely basic "Tanic the Hedgehog" recolor mod has a file/folder structure that goes like so:
- A "mod.ini" file
- A "disk" folder
  - A "sonic2013_patch_0" folder (Sonic Lost World)
    - Sonic.pac
    - Sonic.pac.00
  - A "bb3" folder (Sonic Generations)
    - Sonic.ar.00
    - Sonic.arl

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


### The mod.ini file
The mod.ini file is a mod configuration file that details all the user-friendly information about your mod, as well as how CPKREDIR should load the mod.

The version of the format used in the SLW Mod Loader is a variation on the format used in SonicGMI, with some minor changes/additions here and there.

Here's an example of a mod.ini file:
```
[Main]
IncludeDir0="."
IncludeDirCount=1
UpdateServer="https://dl.dropboxusercontent.com/s/xkapkbby1vu4snk/Sonic06UpdateFile.txt"

[Desc]
Title="SLW '06 Project"
Description="What if Sonic '06 stages were fully playable in an engine that not only perfectly fit it's level design/gameplay, but was actually GOOD? \n\nIntroducing the SLW '06 Project! A mega mod for Lost World that ports as much of Sonic '06 as humanly possible without the glitches and unattractive visuals!  \n\nCredits:\n UltimateDarkman for making the wonderful '06-esque animations.\n Death for his wonderful beta-testing work. \n Gotta Play Fast and Slash for porting the Sonic '06 player models to Lost World. \n Radfordhound for ripping the stages, rendering GIA/generating lightfields, drawing vertex colors, and porting the HUD/music. \n Radfordhound, Gotta Play Fast, Slash, and Beatz for porting the stages and doing object placement. "
Version="Alpha 1.0"
Date="4/20/16"
Author="Radfordhound & Beatz & GPF & Death & UltimateDarkman"
AuthorURL="https://www.youtube.com/user/Radfordhound & https://www.youtube.com/channel/UCEjwges-3BTaWsMwOGJDoGQ & https://www.youtube.com/channel/UCZfOGBkXRKICFozWU5bE0Xg & https://www.youtube.com/user/DeathwolvesProjects & https://www.youtube.com/user/UltimateDarkman2010"
URL="https://onedrive.live.com/redir?resid=A0D011638C5973B3!5011&authkey=!AJXTG3vsMq0OXFc&ithint=folder%2c"
```

The following is a list of the most important values that can be used in a mod.ini file:

### Main

**IncludeDir?** Specifies which folders will be included with your mod, allowing you to modify the default file/folder structure mentioned above. You can also set this to `../{Another Mod Name}` which should load files from another mod, But this is **NOT RECOMMEND**. (`?` is a Zero-based Number)

**IncludeDirCount** Specifies how many folders will be included with your mod.

**UpdateServer** A modification of the existing SonicGMI value that specifies the link to a raw .txt file containing URLs in a particular format. This feature has **not yet been fully added**, and will be further detailed once it is. However, I recommend linking a .txt file or a link to a folder on a http server just in case anyway, as it will allow you to release auto-downloading updates to your mods once the mod loader has been updated to support this feature.

### Desc

**Title** The name of your mod as shown in the mod loader.

**Description** A description of your mod that is shown in the description window in the mod loader.
Typing a "\n" in this value will indicate a new line within the mod loader, **which should be done to keep your mods loadable!**

**Date** The date the mod was originally created as shown in the mod loader.

**Author** The author(s) of the mod. **You can include multiple authors in this value!** Simply seperate the authors via a space, followed by an ampersand, and another space. (Like this: "Radfordhound & Gotta Play Fast") They will be loaded as seperate authors within the mod loader, allowing you to link to them seperately.

**AuthorURL** The URL(s) to the author(s) of the mod. (Such as websites, YouTube channels, and social media accounts.) **You can include multiple authors in this value!** Simply seperate the authors' URLs via a space, followed by an ampersand, and another space. (Like this: "https://www.youtube.com/user/Radfordhound & https://www.youtube.com/channel/UCZfOGBkXRKICFozWU5bE0Xg") They will be loaded as seperate URLs within the mod loader and automatically linked with the data contained in the "Author" value, allowing you to link to them seperately.

**URL** The URL of the mod (aka mod homepages/threads/release videos).


There are many other values that can be used in a mod.ini file, many of which are already being used in several mods. So keep an eye out for them in other released mods!
