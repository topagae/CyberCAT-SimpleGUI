# Privacy policy

This program will not transfer any information to other networked systems unless specifically requested by the user or the person installing or operating it.

In detail:

- **No network access.** The editor doesn't connect to the internet: no telemetry, no analytics, no update checks and no crash reporting.
- **Files it reads:** only the save files you open (`sav.dat`, plus `metadata.9.json` and `screenshot.png` from the same folder), the appearance preset files you choose to load, and an optional `config.json` next to the program.
- **Files it writes:** only where you tell it to save (`sav.dat`, a backup `sav.old`, and the save's metadata and screenshot if they are missing), preset files you choose to save, and an `error.txt` next to the program when something goes wrong. `error.txt` stays on your computer unless you decide to share it.
- **Third-party components** (WolvenKit and the .NET runtime) are used offline and don't transfer data either.

Questions: open an issue at https://github.com/topagae/CyberCAT-SimpleGUI/issues.
