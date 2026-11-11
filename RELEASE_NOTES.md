# CyberCAT-SimpleGUI v0.29e: save editor updated for Cyberpunk 2077 patch 2.5

The CyberCAT-SimpleGUI save editor works again. This is an unofficial fork of Deweh's [CyberCAT-SimpleGUI](https://github.com/Deweh/CyberCAT-SimpleGUI), which stopped working with patch 2.5 saves.

The full source is at https://github.com/topagae/CyberCAT-SimpleGUI.

## ⬇️ Download (no coding needed)

1. Go to the **[latest release](https://github.com/topagae/CyberCAT-SimpleGUI/releases/latest)** and click **CP2077SaveEditor-v0.29e-win-x64.zip** under **Assets**.
2. Right-click the downloaded zip and choose **Extract All...**. Don't run the editor from inside the zip.
3. Open the extracted `CP2077SaveEditor-v0.29e` folder and double-click **CP2077SaveEditor.exe**. There's nothing to install.
4. If Windows shows "Windows protected your PC", click **More info**, then **Run anyway**. This appears because the program isn't signed with a paid certificate.

Requires 64-bit Windows 10 or 11.

## ⚠️ Read this first

- **Back up your saves.** Copy your save folder from `%USERPROFILE%\Saved Games\CD Projekt Red\Cyberpunk 2077\` somewhere safe before you edit anything.
- **It has had very little in-game testing.** One patch 2.5 save was edited (stats, attributes and appearance) and loaded fine in the game.
- **Everything else is tested by code only.** That covers inventory, vehicles, quest facts, extras, body gender changes, patch 2.0–2.31 saves and modded saves. Automated tests on real saves (2.0, 2.01, 2.31, 2.5) load, edit, save and reload them without losing data, but that doesn't guarantee the game accepts every edit.
- Save into a new slot, not over your only copy.
- Editing quest facts and item flags can break things hours later. Keep your backup for a while.

## What's new in v0.29e

- **Large saves load and save.** Saves the editor couldn't open failed with "Exception of type 'System.Exception' was thrown" (in `CompressionHeader.get_Settings`). This happened with saves that store their data in a layout the editor didn't know, usually very large modded saves. They now load, and are written back the same way.
- **Saves that grow while editing can be saved.** If your edits made a save bigger than its original layout allowed, saving failed. The editor now makes room, like the game does.
- **Clearer errors** when a file isn't a PC save or is damaged, instead of "Exception of type 'System.Exception' was thrown".

## What's new in v0.29d

- **Appearance presets and body gender changes work again.** In v0.29a to v0.29c, loading or saving a preset, or changing body gender, failed with "Minor version contains invalid character". That was caused by the editor's version number, not by your save.
- **Saves using lifepath mods load.** Saves from mods like Fresh Start, which combine lifepaths, failed with "Specified argument was out of the range of valid values". They now load, the lifepath shows as "Modded", and it's kept as it is.
- **"Unlock all fast travel points" keeps the ones you have.** It used to replace the whole list, which could remove unlocked points the editor didn't know about, like Dogtown ones or points added by mods. Now it only adds the missing ones.
- **Ctrl+S saves and Ctrl+O opens a save**, which helps when the buttons don't show, for example under Wine or Proton.
- **A clearer message when `kraken.dll` can't be loaded.** That file decompresses saves. It's usually missing because the zip wasn't fully extracted, or because antivirus quarantined it.
- **The "Make all characters romanceable" button explains its limit.** Some romances outside the game's rules, like Panam with a female V, also need a romance mod.

## What's new in v0.29c

- **Changing body gender is fixed.** It used to leave V with a mix of both body types and the old gender's head, and the game could crash when you used a mirror. Now:
  - the new body gets a complete default appearance made for patch 2.5 (the old defaults were from before patch 2.0);
  - every part of that appearance comes from the base game, so it doesn't need any mods;
  - if the change can't be made, the save is left exactly as it was;
  - V's voice stays the same.
- **Better support for script mods.** Some mods (for example BetterNetrunning, StealthRunner and MuchBetterContracts) store their own data on V and on NPCs. The editor used to skip that data entirely, so on those saves body gender couldn't be changed. That data is now read, and written back exactly as it was.
- **Loading an appearance preset for the other body gender** now only applies it if the body gender change worked.

## What's new in v0.29b

- **Fewer antivirus false positives:** the release no longer includes `wwtools.dll` or the Linux and macOS libraries. The editor never used them. The editor itself is unchanged from v0.29a.

## What's new in v0.29a

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

## Code signing policy

See the [code signing policy](https://github.com/topagae/CyberCAT-SimpleGUI#code-signing-policy) in the README. This release is unsigned.

## License

The editor's code is MIT licensed. Because it's built with WolvenKit (GPL-3.0), the released program is distributed under the GPL-3.0, and the full source, including the WolvenKit changes, is in this repository. [NOTICE.md](NOTICE.md) lists every change.
