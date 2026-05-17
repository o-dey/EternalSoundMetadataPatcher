# Eternal Sound Metadata Patcher

![license](https://img.shields.io/badge/License-CC%20BY--NC--SA%204.0-lightgrey.svg)
![build workflow](https://github.com/o-dey/EternalSoundMetadataPatcher/actions/workflows/build.yml/badge.svg)
![tests workflow](https://github.com/o-dey/EternalSoundMetadataPatcher/actions/workflows/tests.yml/badge.svg)

Eternal Sound Metadata Patcher is a small helper utility that makes it possible to have "unlimited", freely customizable
sounds in your idStudio based DOOM Eternal mods.

More specifically, this utility updates your mods `soundmetadata.bin` file with the relevant metadata of your custom sound
events defined in
[the sounds/music Wwise template that ships with idStudio](https://idstudio.idsoftware.com/audio/audio-file-modding-by-wwise)
for DOOM Eternal.

> [!WARNING]
> This application is currently in alpha state, so you should expect to run into problems!

## Download

You can [get the latest release here](https://github.com/o-dey/EternalSoundMetadataPatcher/releases/latest).

## How to use

> [!NOTE]
> for now it is expected that you already know how to use Wwise, a video tutorial that specifically incorporates the metadata
> patcher will be made available soon-ish!

After packaging the Wwise project, and copying over the generated `.pck` and potentially `.wem` files (in case you're using
external sounds instead of embedded ones), run the metadata patcher, passing the path of your DOOM Eternal mod folder, and
the path of the Wwise project, for example:

```cmd
EternalSoundMetadataPatcher "C:\idstudio_mods\doom-mod-1815067254" "C:\wwise_projects\Wwise Music Mod\ModsWwise"
```

This will update the `soundmetadata.bin` file with the relevant, new metadata. After that, start/restart idStudio, and you
should see your custom sound events being shown in the asset browser.
