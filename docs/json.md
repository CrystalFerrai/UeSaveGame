# UeSaveGame JSON Module

`UeSaveGame` has an optional code module named `UeSaveGame.Json` which adds JSON serialization functionality for save games.

# Usage

This optional module provides utilities for serializing save files to or from a custom json format. This format cannot be used by Unreal Engine. The purpose of the library is to provide a human editable view of save files which can then be converted back into binary files.

First, create input and output streams using whatever method suits your needs. For example using files:

    using FileStream inStream = File.OpenRead("some_path");
    using FileStream outStream = File.Create("some_path");

To convert binary to json:

    SaveGameSerializer serializer = new();
    serializer.ConvertToJson(inStream, outStream);

To convert json to binary:

    SaveGameSerializer serializer = new();
    serializer.ConvertFromJson(inStream, outStream);

Note: I also have a CLI program which makes use of this library to convert save files. It can be found here: [UeSaveConverter](https://github.com/CrystalFerrai/UeSaveConverter.git).

# API

The API consists primarily of the `SaveGameSerializer` class.

## SaveGameSerializer

This is the primary API for this module.

    SaveGameSerializer()
    SaveGameSerializer(bool indented, int indentation, char indentChar)

Constructors. One takes in paramaters to modify the formatting of the json output from `ConvertToJson`.

    void ConvertToJson(Stream input, Stream output)

Reads an input stream containing a binary save game and writes a JSON representation of that data to the output stream.

    void ConvertFromJson(Stream input, Stream output)

Reads an input stream containing a JSON representation of a save game and writes a binary representation of that data to the output stream.

# Extension Points

Extension points exist to handle game-specific custom serialization. These extension points coincide with the extension points found in the main library. Generally, you will have a one for one match of custom serialization classes for the main module and the json module.

Read more about main module extension points, including when to use them, here: [Extension Points](extension.md).

## Custom Struct Serializers (IStructDataSerializaer and StructDataSerializerBase)

Whenever you have a custom `IStructData` serializer, you will usually need a corresponding json serializer if you are using the json module. Implement the `IStructDataSerializer` interface or extend the `StructDataSerializerBase` convenience class.

The following example corresponds to the example in the [Extension Points](extension.md) document. It simply writes a Base64 representation of the raw bytes.

```cs
internal class BlobStructSerializer : StructDataSerializerBase
{
    public override IEnumerable<string> StructTypes
    {
        get
        {
            // List every type of struct you expect this class to handle
            yield return "FMyGameStructA";
            yield return "FMyGameStructB";
        }
    }

    public override IStructData? FromJson(JsonReader reader)
    {
        BlobStruct blob = new();
        string? value = reader.Value is string sv ? sv : reader.ReadAsString();
        if (value is not null)
        {
            blob.Value = Convert.FromBase64String(value);
        }
        return blob;
    }

    public override void ToJson(IStructData? data, JsonWriter writer)
    {
        BlobStruct? blob = data as BlobStruct;
        if (blob is null) throw new ArgumentException("Unexpected data type", nameof(data));

        if (blob.Value is null)
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteValue(Convert.ToBase64String(blob.Value));
        }
    }
}
```

## Custom Save Class Serializers (SaveClassSerializerBase)

Whenever you have a custom `SaveClassBase` serializer, you will usually need a corresponding json serializer if you are using the json module. Extend the `SaveClassSerializerBase` generic class.

The following example corresponds to the example in the [Extension Points](extension.md) document.

NOTE: The generic argument type must be the matching `SaveClassBase` implementation, which in the case of this exmaple is `WorldSave`.

```cs
internal class WorldSaveSerializer : SaveClassSerializerBase<WorldSave>
{
    public override bool HasCustomHeader => true;

    public override void HeaderFromJson(JsonReader reader, WorldSave saveClass)
    {
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.PropertyName)
            {
                switch (reader.Value)
                {
                    case nameof(WorldSave.Version):
                        saveClass.Version = reader.ReadAsInt32() ?? 0;
                        break;
                    case nameof(WorldSave.Id):
                        saveClass.Id = reader.ReadAsInt32() ?? 0;
                        break;
                }
            }
        }
    }

    public override void HeaderToJson(JsonWriter writer, WorldSave saveClass)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(nameof(WorldSave.Version));
        writer.WriteValue(saveClass.Version);

        writer.WritePropertyName(nameof(WorldSave.Id));
        writer.WriteValue(saveClass.Id);

        writer.WriteEndObject();
    }
}
```
