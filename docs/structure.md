# UeSaveGame Structure

This document describes the high level structure and API of the `UeSaveGame` library.

NOTE: This document only covers the core library. For the `UeSaveGame.Json` optional module, see [Json Module](json.md).

# Primary APIs

This covers the main APIs exposed by `UeSaveGame`, but there are additional APIs available which can be discovered as needed by browsing the source code.

## SaveGame

The `SaveGame` class can be used to load and save save game files.

A save game file usually has the extension `.sav`, but may be customized for some games. The easiest way to identify a save game file is to open it in a hex editor and look at the first four bytes, which should be "GVAS" (SAVG spelled backwards due to endianness). Following that will be a standard header block which ends with the name of the save class, then a list of serialized `FPropertyTag` representing the save data.

If the save file for a game you are working with does not follow this standard format, then you may need to implement [extension points](extension.md) for minor deviations or possibly write your own full replacement class if the file is completely different. There is a good chance that the save still contains standard `FPropertyTag` serialization that this library can help with, but the `SaveGame` class is designed specifically for the standard save format.

### Properties

    IList<FPropertyTag> Properties

Provides read/write access to the deserialized save data.

### Functions

    static SaveGame LoadFrom(Stream stream)

Creates a new instance of `SaveGame` by deserializing data from the passed in stream. Will throw various exceptions if issues are encountered during deserialization.

    void WriteTo(Stream stream)

Serializes the current instance and writes it to the passed in stream. Will throw various exceptions if issues are encounted during serialization.

## PropertySerializationHelper

Located within the `Util` namespace, `PropertySerializationHelper` is a static class providing methods for serializing lists of `FPropertyTag`. When implementing a custom save game class, this is primary entry point into the library when working with a property list.

    static IEnumerable<FPropertyTag> ReadProperties(BinaryReader reader, PackageVersion packageVersion, bool isNullTerminated)

Deserializes a list of properties from the passed in reader. It is necessary to determine the `PackageVersion` that corresponds to your game's engine version. Passing in the incorrect version may cause serialization to fail. `isNullTerminated` should be true if there is an additional 4 bytes of 0s after the property list and you want them to be read/skipped.

    static long WriteProperties(IEnumerable<FPropertyTag> properties, BinaryWriter writer, PackageVersion packageVersion, bool isNullTerminated)

Serializes a list of properties and writes them to the passed in writer. It is necessary to determine the `PackageVersion` that corresponds to your game's engine version. Passing in the incorrect version may cause serialization to fail. `isNullTerminated` should be true if there is an additional 4 bytes of 0s after the property list that should be written.

## PackageVersion

This represents the version of Unreal Engine's serialization code that should be assumed when serializing save files. Usually this value is read directly from the save game file header, but in cases where such a header is absent and you have created a custom save game implementation, you will need to determine the proper version for the game you are working with in order to use most of the API provided by the `UeSaveGame` library.

To determine the package version that corresponds to your game, you should first find the engine version. Examine the properties of the game's main executable and look for the "File version" on the details tab. That should indicate the version of the engine the game was built with. For example, 5.4.4.0 means Unreal Engine version 5.4 update 4 (usually the update number doesn't matter for this purpose since they don't usually change serialization formats within a single version's lifetime).

You can then look at the source code for Unreal Engine (available on Github but you need to register to access it). Locate the branch that corresponds to the version in use by the game - for example `5.4` - then browse to the file `Engine/Source/Runtime/Core/Public/UObject/ObjectVersion.h` on that branch. The latest/last versions listed in each of the enums in that file are the versions you are looking for. You can construct a new instance of `PackageVersion` using those values from the corresponding enums in `UeSaveGame`.

### Fields

    EObjectUE4Version PackageVersionUE4

The UE4 component of the version. For a UE4 game, set this appropriately based on the game's engine version. For a UE5 game, set this to the latest/last value in the enum: `VER_UE4_CORRECT_LICENSEE_FLAG`.

    EObjectUE5Version PackageVersionUE5

The UE5 component of the version. For a UE5 game, set this appropriately based on the game's engine version. For a UE4 game, set this to `INVALID`.;

# Class Relationships

The classes in `UeSaveGame` mostly fall into one of the following categories.

1. Utilities that provide functionality for manipulating or serializing data.
2. Representations of objects that have been deserialized from save game data.
3. Enstension points for injecting custom serialization code for game-specific use cases.

Utilities are covered in the API documentation, or within the source code itself. Extension points are covered in [Extension Points](extension.md). This section focuses on the data objects.

`SaveGame` is the top-most object representing an entire save file. It contains (among other things) a list of `FPropertyTag` instances.

`FPropertyTag` contains metadata for a single saved property as well as owning an instance of `FProperty`.

`FProperty` contains the data for a single property. There are many types of properties. This serves as a base class for all of them. Some examples of `FProperty` implementations include:

* ArrayProperty
* BoolProperty
* EnumProperty
* IntProperty
* MapProperty
* NameProperty
* ObjectProperty
* SetProperty
* StrProperty
* StructProperty
* TextProperty
* (List is not exhaustive.)

`ArrayProperty`, `SetProperty` and `MapProperty` are collections which contain lists of additional properties.

`StructProperty` can contain varied types of data. It may contain an implementation of `IStructData` such as `ColorStruct`, `GuidStruct` or `VectoreStruct`. Or it may contain a `PropertiesStruct` which itself contains a list of properties.

Other property types generally contain a specific value such as an integer, string, etc.

The `DataTypes` namespace contains various non-primitive data types that may be used by properties to store values.

In short, `SaveGame` contains `FPropertyTag`s which each contain `FProperty` which contains a value. That value may be additional properties, thus building a tree of properties and values.
