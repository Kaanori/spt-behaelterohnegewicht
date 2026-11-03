# BehaelterOhneGewicht (container without weight)

The contents of the secure container no longer weigh anything. They no longer count toward the player character's weight.

## Requirements

- SPT 4.1.6 (EFT 0.16.9). BepInEx ships with SPT.

## Installation

1. Download `BehaelterOhneGewicht-1.0.0.zip` from the [Releases](https://github.com/Kaanori/spt-behaelterohnegewicht/releases) page.
2. Extract it **into your SPT game folder** (the folder that contains `EscapeFromTarkov.exe`).
   The ZIP already has the right layout; the mod ends up in `BepInEx/plugins/`.
   On Windows, make sure "Extract All" does not add an extra subfolder.
3. Restart the game.

## Uninstall

Delete the mod's files from `BepInEx/plugins/` (settings: `BepInEx/config/` for client mods) and restart the game.

## Settings (F12 → BehaelterOhneGewicht)

| Setting | Effect |
|---|---|
| **Eingeschaltet** (enabled) | on/off |
| **Abzug ins Protokoll schreiben** (write deduction to log) | writes the deducted value to the BepInEx log (for troubleshooting) |

## Building from source

Needs the .NET 10 SDK and an SPT 4.1.6 installation to build against.

```bash
./build.sh             # Linux: build only
./build.sh --install   # build and copy into the game folder
```
```powershell
.\build.ps1            # Windows
.\build.ps1 -Install
```

The game folder is set in `Directory.Build.props` (Windows default `C:/SPT`)
or via the environment variable `SPT_DIR`.

## Support

Bugs and questions: [GitHub issues](https://github.com/Kaanori/spt-behaelterohnegewicht/issues).

## License

MIT — see [LICENSE](LICENSE).
