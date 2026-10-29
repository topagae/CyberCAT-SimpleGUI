# CyberCAT-SimpleGUI v0.29a: save editor updated for Cyberpunk 2077 patch 2.5

The CyberCAT-SimpleGUI save editor works again. This is an unofficial fork of Deweh's [CyberCAT-SimpleGUI](https://github.com/Deweh/CyberCAT-SimpleGUI), which stopped working with patch 2.5 saves.

The full source is at https://github.com/topagae/CyberCAT-SimpleGUI.

## ⬇️ Download (no coding needed)

1. Go to the **[latest release](https://github.com/topagae/CyberCAT-SimpleGUI/releases/latest)** and click **CP2077SaveEditor-v0.29a-win-x64.zip** under **Assets**.
2. Right-click the downloaded zip and choose **Extract All...**. Don't run the editor from inside the zip.
3. Open the extracted `CP2077SaveEditor-v0.29a` folder and double-click **CP2077SaveEditor.exe**. There's nothing to install.
4. If Windows shows "Windows protected your PC", click **More info**, then **Run anyway**. This appears because the program isn't signed with a paid certificate.

Requires 64-bit Windows 10 or 11.

## ⚠️ Read this first

- **Back up your saves.** Copy your save folder from `%USERPROFILE%\Saved Games\CD Projekt Red\Cyberpunk 2077\` somewhere safe before you edit anything.
- **It has had very little in-game testing.** One patch 2.5 save was edited (stats, attributes and appearance) and loaded fine in the game.
- **Everything else is tested by code only.** That covers inventory, vehicles, quest facts, extras, patch 2.0–2.31 saves and modded saves. Automated tests on real saves (2.0, 2.01, 2.31, 2.5) load, edit, save and reload them without losing data, but that doesn't guarantee the game accepts every edit.
- Save into a new slot, not over your only copy.
- Editing quest facts and item flags can break things hours later. Keep your backup for a while.

## What's new

- **Patch 2.5 support:** loading, saving and stat editing all work with the new stats format the game uses.
- **Mod data is no longer deleted.** Older versions silently dropped stat and perk values they didn't recognise, for example from mods, whenever you saved. This affected patch 2.x saves too. Those values are now kept as they are.
- **Unchanged data stays unchanged.** Parts of the save you don't edit are written back as they were.
- **Safer saving:** saves go to a temporary file first, so a crash mid-save can't leave a broken `sav.dat`. Your previous save is kept as `sav.old`.
- **Faster and lighter:** the window opens instantly, it's ready in about 4 seconds, and it uses about 1.7 GB of RAM instead of 2.5 GB.
- **Fixes:**
  - adding stats to new items wrote to the wrong field;
  - the money edit could crash;
  - the status bar said "File saved." even when saving failed;
  - saves can now be opened while the game is running.

## What you can edit

- **Player:** attributes, perk, attribute and relic points, level, street cred and skill levels.
- **Appearance:** edit your character's appearance, and save or load appearance presets.
- **Inventory:** quantities, flags, stats and mod trees of your items.
- **Vehicles.**
- **Quest facts.**
- **Quick actions:** money, legendary items, unlock fast travel, clear the police blacklist, and more.

## Supported versions

- **Patch 2.0 – 2.5:** supported.
- **Patch 1.x:** use the [original releases](https://github.com/Deweh/CyberCAT-SimpleGUI/releases).

**Known limitation:** changes to *modifier groups* (in the advanced stats editor) can't be saved on patch 2.5 saves yet. The editor warns you about this when you save.

## How to use it

1. Extract the zip and run `CP2077SaveEditor.exe`. No .NET install needed.
2. Click **Load Save** and pick a `sav.dat`.
3. Make your changes, click **Save Changes**, and choose a save folder.

If something goes wrong, the editor writes an `error.txt` next to the exe. Please report it along with your game version and what you changed.

## Credits

- **CyberCAT-SimpleGUI** by Deweh (DeweySalt), with seberoth, Merijn Hendriks, Alain Dupuis and other contributors. All credit for the editor itself goes to them.
- **CyberCAT** by SirBitesalot and contributors, the project it grew out of.
- **WolvenKit** by the WolvenKit team, which reads and writes the save files.
- **This update:** written about 95% by Claude (Anthropic's AI model, using Claude Code), and about 5% by topagae, who directed and tested it.

This fork isn't endorsed by the original authors. I've contacted them, and this note will be updated.

## License

The editor's code is MIT licensed. Because it's built with WolvenKit (GPL-3.0), the released program is distributed under the GPL-3.0, and the full source, including the WolvenKit changes, is in this repository. [NOTICE.md](NOTICE.md) lists every change.
