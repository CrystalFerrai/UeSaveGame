# Unreal Engine Save Game Library

This is a .NET library for reading and writing standard Unreal Engine 4 and 5 save game files. This library should work with most games which do not use any custom serialization, but may require updating for games it hasn't seen before.

Currently supports Unreal Engine versions 4.26 - 5.6. Other versions may work, but have not been tested.

## Releases

There are no releases of this library at this time nor are any planned any time soon.

## How to build

The repo contains Visual Studio 2022 projects and is configured to build using .NET 6. There are no third party libraries in use. You should be able to add the desired projects to a solution and build them. There are two projects:

1. UeSaveGame: This is the main library.
2. UeSaveGame.Json: This is an optional library that provides json serialization for save game data.

The Json library has NuGet dependencies, so be sure to either run `dotnet restore` or "Restore NuGet Packages" from the Visual Studio solution context menu.

## How to use

Documentation can be found in the [docs](docs) directory.

## Support

This is just one of my many free time projects. No support or documentation is offered for this library beyond this readme.

## Games tested

The following games have been tested using the library. Testing consists of loading a file, saving it, and checking that the output is binary equal to the input. Testing of the Json library is less comprehensive, so there may be games listed as working which will not convert to/from json properly.

### Working
* Abiotic Factor - Working but requires custom save classes to serialize custom headers.
* Aven Colony - Technically works, but the data is just a couple large byte arrays that would need further decoding
* Carnal Instinct - Fully working
* Icarus - Fully working. There is a block of base64 in prospect json files. Convert it to binary, then decompress with zlib, then use `PropertySerializationHelper.ReadProperties` to read the data. (It is basically a save file with no header.) Or just use [IcarusSaveLib](https://github.com/CrystalFerrai/IcarusSaveLib) which handles all this.
* Parcel Simulator - Fully working

### Not working
* Dragon Quest XI - Files are compressed. Have not attempted to decompress and examine
* Moss - Crashes, cause unknown, needs investigation
* Satisfactory - Missing file headers, could possibly be made to work but tools exist for this game already

### Will never work
* Astroneer - Uses a hefty amount of custom serialization as well as compresses the file. Needs an entirely custom library to handle.
