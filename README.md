# Silksong In-Game Skin Selector

This repository now contains a Unity mod component that gives you an in-game menu to switch Hornet skins while playing.

## What it does

- Scans a `HornetSkins` folder for skin subfolders
- Each skin folder needs a spritesheet named `hornet.png` or `spritesheet.png`
- Opens an in-game IMGUI window with `F8`
- Reloads skin folders with `F5`
- Applies the selected skin immediately and stores it as the active choice

## Skin folder layout

Create skin folders in:

`<Game Persistent Data>/HornetSkins`

Each skin should look like:

```text
HornetSkins/
  Default/
    hornet.png
  Red/
    spritesheet.png
```

When you apply a skin, the selected spritesheet is copied to:

`HornetSkins/_active/hornet.png`

So your runtime patching logic can always read a single active spritesheet path.

## Source

- `src/SilksongIngameSkinselector/HornetSkinSelectorBehaviour.cs`