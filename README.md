# CyberCAT-SimpleGUI: Patch 2.5 update (v0.29a)

A save editor for Cyberpunk 2077, updated to work with **patch 2.5** saves.

This is an **unofficial fork** of [CyberCAT-SimpleGUI by Deweh (DeweySalt)](https://github.com/Deweh/CyberCAT-SimpleGUI). The original editor was a simplified offshoot of SirBitesalot's [CyberCAT](https://github.com/WolvenKit/CyberCAT). The original stopped working with patch 2.5 saves, and this fork fixes that. All credit for the editor itself goes to the original authors, see [Credits](#credits).

## ⚠️ Read this first

- **Back up your saves before using this.** Copy the whole save folder from `%USERPROFILE%\Saved Games\CD Projekt Red\Cyberpunk 2077\` to somewhere safe.
- **This update has had very little in-game testing.** Only one patch 2.5 save has been loaded in the game after editing, with edits to stats and attributes (numbers) and appearance.
- **Everything else has been tested by code only, not in the game.** That covers inventory, vehicles, quest facts, the extras tab, saves from patches 2.0 to 2.31, and modded saves. Automated tests on real saves from patches 2.0, 2.01, 2.31 and 2.5 load each save, edit it, write it and read it back without losing data. That makes a working save likely, but it doesn't prove the game will accept it.
- Save into a **new save slot** rather than over your only copy. When you overwrite a `sav.dat`, the editor keeps the previous file as `sav.old`.
- Editing quest facts and inventory flags can break quests or items in ways that only show up hours later. Keep your backup for a while.
- Please report problems with the `error.txt` the editor creates next to the exe, the game version, and what you changed.

## What's new in v0.29a

[RELEASE_NOTES.md](RELEASE_NOTES.md) has the full release post.

- **Patch 2.5 support.** Patch 2.5 saves load and save, including stat editing. Patch 2.5 changed the format the game uses to store stats.
- **Mod data is kept.** Older versions silently deleted stat and perk values they didn't recognise (for example values added by mods) when saving, and that affected patch 2.x saves too. These values are now kept and written back unchanged.
- **Unchanged data stays unchanged.** Parts of the save you don't edit are written back as they were.
- **Fix:** adding stats to a new item wrote its unique counter into the wrong field.
- The window title and status bar show the version, and clicking the version in the status bar opens the credits.

## Features

**Feature**                                                                     | **Stability**
------------------------------------------------------------------------------- | -----------
Edit attributes, perk / attribute / relic points, level, street cred and skill levels. | Stable (tested in game on 2.5)
Edit your character's appearance, and save / load appearance presets.           | Stable (tested in game on 2.5)
Edit the quantity, flags, stats & mod tree of items in your inventory.          | Stable (2.5: not tested in game)
Edit vehicles.                                                                  | 2.5: not tested in game
Edit quest facts.                                                               | Stable (2.5: not tested in game)
Quick actions: money, legendary items, unlock fast travel, clear blacklist, and more. | Semi-Stable (2.5: not tested in game)

### Use cases

- Respec your attributes or fix lost perk points without starting over.
- Change V's appearance after character creation.
- Adjust item quantities, quality or mods, or recover items.
- Fix stuck quests by editing quest facts (be careful, see the warnings above).
- Unlock fast travel points or clear the police blacklist.

## Game version support

**Game Version**    | **Support**    | **CyberCAT-SimpleGUI Version**
--------------------| ---------------|-----------------------------
Patch 2.5           | Full*          | v0.29a (this fork)
Patch 2.0 - 2.31    | Full           | v0.29a (this fork), original releases for older versions
Patch 1.x           | Not in v0.29a  | Use the [original releases](https://github.com/Deweh/CyberCAT-SimpleGUI/releases) (1.5X: >=v0.20a, 1.3: >=v0.10a_r1, 1.23: >=v0.10a)

\* **Known limitation:** changes to *modifier groups* (in the advanced stats editor) can't be saved for patch 2.5 saves yet. The editor warns you when you save instead of silently dropping them. All other stat changes are saved.

## Usage

1. Extract the zip anywhere and run **CP2077SaveEditor.exe**. It's self-contained, so you don't need to install .NET.
2. Click **"Load Save"** and pick a `sav.dat` from your save folder.
3. Make your changes.
    - Double-click items in your inventory to edit them.
    - Double-click nodes in an item's mod tree to edit them.
4. Click **"Save Changes"** and choose a save folder. Saving into the root save folder isn't allowed.

The window opens right away. Loading the game's name tables takes a few more seconds, and **"Load Save"** becomes clickable when that finishes. The editor uses around 1.7 GB of RAM.

## Credits

- **[CyberCAT-SimpleGUI](https://github.com/Deweh/CyberCAT-SimpleGUI)** by Deweh (DeweySalt), with contributions from seberoth, Merijn Hendriks, Alain Dupuis and others. It's the editor this fork is based on.
- **[CyberCAT](https://github.com/WolvenKit/CyberCAT)** by SirBitesalot and contributors. It's the project the original editor came from.
- **[WolvenKit](https://github.com/WolvenKit/WolvenKit)** by the WolvenKit team. It reads and writes the save files, and this fork adds a few patch 2.5 fixes to it (see [NOTICE.md](NOTICE.md)).
- **[K4os.Compression.LZ4](https://github.com/MiloszKrajewski/K4os.Compression.LZ4)** by Milosz Krajewski.
- **Patch 2.5 update (v0.29a):** written about 95% by **Claude** (Anthropic's AI model, using Claude Code), and about 5% by **topagae**, who directed and tested it.

## License

The editor's source code is under the [MIT License](LICENSE). It is built together with WolvenKit, which is under the [GPL-3.0](licenses/WolvenKit.LICENSE.txt), so the released program as a whole is distributed under the GPL-3.0, and its full source, including the modified WolvenKit files, is available. Third-party licenses are in [licenses/](licenses/). [NOTICE.md](NOTICE.md) lists every change made in this fork.

## Building

**Requires:** .NET 8 SDK and git, on Windows.

```
git clone --recurse-submodules <this repository>
dotnet build CP2077SaveEditor.sln -c Release -p:Platform=x64
```

The build applies this fork's WolvenKit changes automatically (see below).

To build a self-contained release into `releases/v<version>/`, with the docs and licenses included:

```
dotnet publish CP2077SaveEditor/CP2077SaveEditor.csproj -p:PublishProfile=win-x64
```

The version is set in one place, `InformationalVersion` in [CP2077SaveEditor.csproj](CP2077SaveEditor/CP2077SaveEditor.csproj). The window title, status bar and credits dialog all read it from there.

### Updating WolvenKit

The `WolvenKit` folder is a git submodule pointing at upstream [WolvenKit](https://github.com/WolvenKit/WolvenKit). This fork's changes to it are kept in [patches/WolvenKit.patch](patches/WolvenKit.patch). [Directory.Build.targets](Directory.Build.targets) applies the patch before WolvenKit builds, and skips it when it's already applied. Each modified WolvenKit file is marked in its first line.

To change WolvenKit code: edit the files in `WolvenKit/`, then run `tools/update-wolvenkit-patch.ps1` and commit the updated patch.

To move to a newer WolvenKit version (for example after a game patch):

```
git -C WolvenKit apply -R ../patches/WolvenKit.patch
git -C WolvenKit fetch origin
git -C WolvenKit checkout <new commit>
git -C WolvenKit apply --3way ../patches/WolvenKit.patch
```

Resolve any conflicts. Drop the changes that upstream already includes, such as the backported patch 2.5 classes. Then run `tools/update-wolvenkit-patch.ps1`, build, and commit the patch together with the new submodule commit. Once upstream has every change, delete the patch file.

## Todo

- Save modifier group changes for patch 2.5 saves.
- Get the WolvenKit fixes upstream.
- Refactor, refactor, refactor.
- Eat some of the spaghetti.
- Improve the functionality of the appearance tab.
