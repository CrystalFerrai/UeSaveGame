# UeSaveGame Documentation

This documentation is provided for people who want to use the `UeSaveGame` library in their own projects. It describes the high level structure of the library and provides examples for different use cases. For a more in-depth understanding of any specific type or system, refer to the source code directly.

The API of the library should be considered unstable for the foreseeable future. For now, all it provides is a deserialized version of a save file for you to work with as you see fit. You can load save data, manipulate it, then save it back out.

WARNING: Several parts of the library remain unimplemented. Things are only implemented when they are encountered in some game's save file. There is a chance that the library will fail to load or properly save a file from a game it has not yet been tested on.

# Getting Started

`UeSaveGame` has no releases. The recommended method of using the library is to include it as a Git submodule in your own repository. Alternatively, you can clone and build it, then copy the binaries to your project.

Once you have the library available, the first thing you should do is load a save file created by the game you are working with, write it back out unchanged, then do a binary comparison to make sure the files are identical. If there are any diffs, then the library did not properly handle that file. In such a case, there may be a bug or unimplemented feature in the library, or the save file may have game specific custom data in it.

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

While some games' save files can be directly understood by `UeSaveGame`, others utilize custom serialization methods that the library will not recognize. A standard save file will start with "GVAS" as the first four bytes when viewed in a hex editor. Following should be a standard header block that the library should be able to understand.

After the header, the rest of a standard save file is a list of saved properties (`FPropertyTag`). However, this is where games often implement something custom either before or within the property list. `UeSaveGame` offers extension points where you can inject your own code to manage serialization of the custom bits.

To learn more about available extension points, see [Extension Points](extension.md).

However, if your game does not start with the standard save game file header, then you cannot use the `SaveGame` class from `UeSaveGame` to read it. Instead, you will need to implement your own serialization code. You can then make use of other parts of the library's API to handle any portions of the save that make use of Unreal Engine's standard property serialization.

For an example of a library that manages a custom file format while relying on `UeSaveGame` to handle the work of serializing properties and their data, take a look at [IcarusSaveLib](https://github.com/CrystalFerrai/IcarusSaveLib), developed for the game Icarus.

# Further Reading

See the other documents in this directory for more information about the library.

* [structure.md](structure.md) - An overview of the structure and API of the library.
* [extension.md](extension.md) - Information on the available extension points and how to use them.
* [json.md](json.md) - Documentation for the `UeSaveGame.Json` optional code module which can be used to convert binary save data to and from JSON.
