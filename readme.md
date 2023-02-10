# Unreal Engine 4 Save Game Library

This is a .NET library for reading and writing standard Unreal Engine 4 save game files. This library should work with most games which do not use any custom serialization, but may require updating for games it hasn't seen before.

## Releases

There are no releases of this library at this time nor are any planned any time soon.

## How to Build

The repo contains a Visual Studio 2022 solution and is configured to build using .NET 6. There are no third party libraries in use. You should be able to just open the solution and build it. You will either want to import the project into your own solution or build it standalone and import the DLL into your own project.

## How to Use

WARNING: Several parts of the library remain unimplemented. Things are only implemented when they are encountered in some game's save file. There is a high chance that the library will fail to load or properly save a file from a game it has not yet been tested on.

The API of the library should be considered unstable for the foreseeable future. For now, all it provides is a deserialized version of a save file for you to work with as you see fit. There are no helper methods as of yet for accessing things in nice ways. You just dig into the raw data from code and manipulate it.

To load a file:

    SaveGame sg;
    using (FileStream file = File.OpenRead("some_path"))
    {
        sg = SaveGame.LoadFrom(file);
    }

To save a file:

    using (FileStream file = File.Create("some_path"))
    {
        sg.WriteTo(file);
    }

The first thing you should do is load a file, write it back out unchanged, then do a binary comparison to make sure the files are identical. If there are any diffs, then the library did not properly handle that file. In such a case, there may be a bug or unimplemented feature in the library, or the save file may have game specific custom data in it.

## Support

This is just one of my many free time projects. No support or documentation is offered for this library beyond this readme.

## Games Tested

The following games have been tested using this library. Testing consists of loading a file, saving it, and checking that the output is binary equal to the input.

### Working
* Aven Colony - Technically works, but the data is just a couple large byte arrays that would need further decoding
* Carnal Instinct - Fully working
* Subverse - Fully working
* Icarus: First Cohort - Fully working
    * For online play, only player achievement tracker save is stored locally
    * For offline saves, there is a block of base64 in one of the json files. Convert it to binary, then decompress with gzip, then use `PropertySerializationHelp.ReadProperties` to read the data. (It is basically a save file with no header.)

### Not Working
* Dragon Quest XI - Files are compressed. Have not attempted to decompress and examine
* Moss - Crashes, cause unknown, needs investigation
* Satisfactory - Missing file headers, could possibly be made to work but tools exist for this game already

### Will Never Work
* Astroneer - Uses a hefty amount of custom serialization as well as compresses the file. Needs an entirely custom library to handle.