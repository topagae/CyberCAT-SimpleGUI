# Notices and attributions

## Authors

- **CyberCAT-SimpleGUI** was created by **Deweh (DeweySalt)**, with contributions from seberoth, Merijn Hendriks, Alain Dupuis and others (see the git history). Original repository: https://github.com/Deweh/CyberCAT-SimpleGUI
- It started as a simplified offshoot of **CyberCAT** by **SirBitesalot** and contributors: https://github.com/WolvenKit/CyberCAT
- Save parsing is done by **WolvenKit** by the **WolvenKit team**: https://github.com/WolvenKit/WolvenKit
- The **patch 2.5 update (v0.29a and later)** was written about 95% by **Claude** (Anthropic's AI model, via Claude Code) and about 5% by **topagae**, who directed the work and tested it in game. It is an unofficial fork and is not endorsed by the original authors.

## Licenses

| Component | License | File |
|-----------|---------|------|
| CyberCAT-SimpleGUI | MIT, Copyright (c) 2021 DeweySalt | [LICENSE](LICENSE), [licenses/LICENSE.txt](licenses/LICENSE.txt) |
| CyberCAT.Core | MIT, Copyright (c) 2020 Mathies Föls | [licenses/CyberCAT.Core.LICENSE.txt](licenses/CyberCAT.Core.LICENSE.txt) |
| K4os.Compression.LZ4 | MIT, Copyright (c) 2017 Milosz Krajewski | [licenses/K4os.Compression.LZ4.LICENSE.txt](licenses/K4os.Compression.LZ4.LICENSE.txt) |
| WolvenKit | GPL-3.0 | [licenses/WolvenKit.LICENSE.txt](licenses/WolvenKit.LICENSE.txt) |

The editor's own source code is MIT licensed. The released `CP2077SaveEditor.exe` is built together with WolvenKit, which is GPL-3.0, so the program as distributed is covered by the GPL-3.0. Anyone who redistributes the binaries must also make the full corresponding source available, including the modified WolvenKit files listed below.

## Changes in v0.29e (2026-11)

### Editor (`CP2077SaveEditor`)

- `CP2077SaveEditor/CP2077SaveEditor.csproj`: new version number.

### WolvenKit (`patches/WolvenKit.patch`)

- `WolvenKit.RED4/Save/Helper/Compression.cs`: saves whose chunk table has a size other than 0x100, 0x200 or 0x400 keep the table size and chunk size they were read with, instead of throwing. New `FitSettings` doubles the table when the data needs more chunks than it holds. Previously writing such a save allocated a negative-size array. Missing or misplaced `FZLC` headers throw `InvalidDataException` with a message instead of a bare `Exception`.
- `WolvenKit.RED4/Save/IO/CyberpunkSaveWriter.cs`: sizes the chunk table with `FitSettings` before the node offsets are computed, since they depend on it.

## Changes in v0.29d (2026-11)

### Editor (`CP2077SaveEditor`)

- `CP2077SaveEditor/CP2077SaveEditor.csproj`: new `DisplayVersion` property for the version users see. `InformationalVersion` is now the numeric `Version`, because WolvenKit parses it as strict SemVer whenever it reads or writes JSON (presets, body gender changes), and `0.29c` made that throw. New version number.
- `CP2077SaveEditor/Utils/AppInfo.cs` (new), `CP2077SaveEditor/Views/Form2.cs`, `CP2077SaveEditor/Properties/PublishProfiles/win-x64.pubxml`: the title bar, status bar, credits dialog and release folder use `DisplayVersion`.
- `CP2077SaveEditor/Views/Controls/PlayerStatsControl.cs`: lifepath values other than Corporate, Nomad and StreetKid (written by lifepath mods) show as "Modded" and are written back unchanged. Previously they threw on load, and the value 3 was rewritten as `Count`.
- `CP2077SaveEditor/Views/Controls/ExtrasControl.cs`: unlocking all fast travel points adds the missing ones, instead of clearing the list and re-adding the ones in `FastTravel.json`.
- `CP2077SaveEditor/Views/Form2.cs`: Ctrl+S and Ctrl+O shortcuts. The error shown for a `DllNotFoundException` explains that `kraken.dll` is needed and how it usually goes missing.
- `CP2077SaveEditor/Views/Controls/QuestFactsControl.cs`: the romance button's message says that some romances also need a mod.
- `README.md`: how the two version properties work.

## Changes in v0.29c (2026-11)

### Editor (`CP2077SaveEditor`)

- `CP2077SaveEditor/Utils/AppearanceHelper2.cs`: changing body gender checks everything before it changes the save, so a failed change leaves it as it was. Previously the body gender was switched before the check for the player data, and a failed change left a body of one gender with the head and body parts of the other. The voice is kept, and the warning is only skipped for the one change it was suppressed for. Removed dead code.
- `CP2077SaveEditor/Views/Controls/AppearanceControl.cs`: a preset for the other body gender is only loaded when the body gender change worked.
- `CP2077SaveEditor/Resources/MaleDefault.preset`, `FemaleDefault.preset`: replaced with patch 2.5 presets taken from real saves. The old ones were from before patch 2.0 and lacked groups the game uses (for example `face`, `beards` and the newer arm cyberware). Eye, eyelash and eyebrow resources point at the base game files, all resources are in the base game archives.
- `CP2077SaveEditor/CP2077SaveEditor.csproj`: changed files in `Resources\` are embedded again by incremental builds. New version number.

### WolvenKit (`patches/WolvenKit.patch`)

- `WolvenKit.RED4/Save/Parser/PersistencySystem2Parser.cs`: fields that script mods add to game classes (redscript `@addField`), including ones holding classes defined by the mod, are kept and written back unchanged, in their original place. Previously an entry with such a field wasn't parsed at all, which for the player's entry made body gender changes impossible.

## Changes in v0.29b (2026-11)

- `CP2077SaveEditor/CP2077SaveEditor.csproj`: release builds leave out WolvenKit's `wwtools.dll`, `libwwtools.so`, `libkraken.so` and `libkraken.dylib`, which the editor never loads. New version number.

## Changes in v0.29a (2026-10)

### Editor (`CP2077SaveEditor`, `ResourceGenerator`)

- `CP2077SaveEditor/Utils/OptimizedStatsHelper.cs` (new): reads and writes the patch 2.5 optimized stats format (`valuesOptimized` and the `shared*` tables). It shows the editor the same `gameSavedStatsData` objects as older patches. Only edited entries are written back, as new slices, and shared slices are never modified in place.
- `CP2077SaveEditor/Utils/SaveFileHelper.cs`:
  - stat lookups and stat creation go through `OptimizedStatsHelper` for patch 2.5 saves;
  - added `PrepareForSave()`;
  - `CreateStatDataFromEntityId` now uses its `id` argument;
  - fixed `CreateStatData` writing the unique counter into `RngSeed`.
- `CP2077SaveEditor/Views/Form2.cs`, `Form2.Designer.cs`:
  - the app calls `PrepareForSave()` before writing and shows its warnings;
  - new version number, window title and credits dialog.
- `ResourceGenerator/Program.cs`: TweakDB names are loaded through `HashService`, because the old API was removed from WolvenKit.
- Clean-up and performance:
  - startup no longer loads the TweakDB name table a second time (about 800 MB less RAM);
  - type registration runs in the background, in parallel with `HashService`, so the window opens right away;
  - saves are opened read-only, and are written to a temporary file first, so a failed save never leaves a broken `sav.dat`;
  - removed dead code: the pre-2.0 `AppearanceHelper`, `AppearancePreset`, `WrongDefaultDialog`, the de-bloat stubs, and the unused `CyberCAT/` binaries and `items.bin`;
  - `AppearanceValueLists` and `AppearanceGender` were kept, each in its own file;
  - file-scoped namespaces throughout;
  - the version is set once in the project file;
  - releases are built with a publish profile.

### WolvenKit (submodule, based on upstream `a58dc27d8`, 2025-07-22)

Each modified file has a dated notice in its first line.

- `WolvenKit.RED4/Types/Classes/gameBufferSlice.cs`, `gameSavedStatsDataOptimized.cs`: new, backported unchanged from upstream WolvenKit `main`.
- `WolvenKit.RED4/Types/Classes/gameStatsStateMapStructure.cs`: the patch 2.5 fields were backported from upstream WolvenKit `main`.
- `WolvenKit.RED4/Types/Enums/cp77enums.cs`: added `gamedataEquipmentArea.ArmsCWStyle`, `gamedataStatType.BaseNeuralCapacityExceeded` and `gamedataStatType.CalculatedCurrentNeuralCapacity`.
- `WolvenKit.RED4/Types/Reflection/ExtendedEnumInfo.cs`, `Types/IO/Red4Reader.cs`: enum values that the generated enums don't know (newer patches, mods) are now kept and written back unchanged. Previously the fields holding them were dropped from the save.
- `WolvenKit.RED4/Types/Reflection/ExtendedPropertyInfo.cs`, `Types/RedBaseClass.cs`, `Archive/IO/RedPackageReader.cs`: unknown package fields keep their original RED type name. Previously they were written back as `DynamicBaseClass`.
- `WolvenKit.RED4/Save/Parser/PersistencySystem2Parser.cs`: unchanged entries are written back byte-for-byte.
