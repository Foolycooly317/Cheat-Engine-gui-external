# Cheat Engine External GUI

A simple Windows Forms app that builds an **external GUI executable** for toggling memory patches while the game is running.

## What this does
- Loads toggle definitions from `config.json`.
- Attaches to a target process by name.
- Writes configured ON/OFF bytes to configured addresses when checkboxes are toggled.

## Build EXE
```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

The executable will be in:
`bin/Release/net8.0-windows/win-x64/publish/CheatEngineExternalGui.exe`

## Configure your toggles
Edit `config.json`:
- `processName`: target process (same as CE process list name, without `.exe`)
- `address`: absolute memory address in hex
- `on`: hex bytes to write when enabled
- `off`: hex bytes to restore when disabled

Example:
```json
{
  "processName": "Game",
  "toggles": [
    {
      "name": "Infinite Health",
      "description": "Patch NOP",
      "address": "0x01234567",
      "on": "90 90",
      "off": "89 50"
    }
  ]
}
```

## Notes
- Run as Administrator if the target process requires elevated rights.
- Addresses can change every launch if ASLR/pointers are involved; this basic sample assumes static addresses.
- For pointer chains or signatures, extend this app with pointer resolution before writing bytes.
