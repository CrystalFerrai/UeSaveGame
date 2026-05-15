# UeSaveGame Extension Points

The `UeSaveGame` library is designed to handle standard Unreal Engine data serialization as it is used within save game files. However, some games extend or even replace the standard serialization with something custom. This library provides exension points where you can inject custom serialization code into places where it is more commonly found within games.

## Custom Struct Serializers (IStructData and BaseStructData)

The most common form of custom serialization in use by games is when they create a `USTRUCT` and override its serialization. `UeSaveGame` may detect this and throw an exception, or it may attempt to interpret it as a standard struct and fail with an unexpected error. In either case, the solution is to implement serialization for the custom type.

Another feature is overriding the serialization of a struct based on specific property names. This can be particularly useful when working with `TMap` or `TSet` properties in versions of UE prior to 5.4 where the struct type is not included in the save data for maps and sets. The library will try to guess at the type when possible, but may fail or guess incorrectly. You can override the serialization in these cases based on the names of the properties.

Any class defined in a loaded assembly that references `UeSaveGame` which implements the `IStructData` interface will be detected and called when serializing structs of the specific type(s) indicated by the class. The `BaseStructData` class is offered as a convenient base class which implements the interface and allows you to override what you need to.

Here is a very basic implementation of a custom struct serializer. All it does is copy the bytes into a buffer during desrialize then write them back during serialize.

```cs
internal class BlobStruct : BaseStructData
{
    public byte[]? Value;

	// Note: Usually you want to specify either types or property names depending
	// on the use case, rarely is there a case to specify both.

    public override IEnumerable<string> StructTypes
    {
        get
        {
            // List every type of struct you expect this class to handle
            yield return "FMyGameStructA";
            yield return "FMyGameStructB";
        }
    }
	
	public override ISet<string>? KnownPropertyNames => new HashSet<string>()
	{
		// List names of properties that should be handled by this class
		"MyProperty1",
		"MyProperty2"
	};

    public override void Deserialize(BinaryReader reader, int size, PackageVersion engineVersion)
    {
        Value = reader.ReadBytes(size);
    }

    public override int Serialize(BinaryWriter writer, PackageVersion engineVersion)
    {
        if (Value is null)
        {
            return 0;
        }

        writer.Write(Value);
        return Value.Length;
    }
}
```

The above example is a good starting point to work from. It will allow the library to get past the data and move on. You can then try to decode the data into something more meaningful later.

## Custom Property Serializers (FProperty)

Some games may implement custom serialization for certain property types. This most commonly happens with `ObjectProperty` where a game may want to store additional information about game objects. You can create a custom `FProperty` implementation and register it for a specific property type by name like this.

```cs
// Register MyObjectProperty as the serializer for ObjectProperty instances
FProperty.RegisterPropertyType(nameof(ObjectProperty), typeof(MyObjectProperty));
```

Here is an example implemenmtation of a custom object property.

```cs
internal class MyObjectProperty : ObjectProperty
{
	// Add properties to store custom data

	protected override void DeserializeValue(BinaryReader reader, int size, PackageVersion packageVersion)
	{
		// Optionally call base if it makes sense
		base.DeserializeValue(reader, size, packageVersion);

		// Add custom deserializtion code as needed
	}

	protected override int SerializeValue(BinaryWriter writer, PackageVersion packageVersion)
	{
		// Optionally call base if it makes sense
		base.SerializeValue(writer, packageVersion);

		// Add custom serializtion code as needed
	}
}
```

## Custom Property Json Serializers (IPropertySerializer)

Part of the `UeSaveGame.Json` module. See [Json Module](json.md) for more information.

## Custom Save Class Serializers (SaveClassBase)

In some cases, a game might override the serialization for one or more of their save classes. `UeSaveGame` provides the `SaveClassBase` base class for this purpose. If any loaded assembly which references `UeSaveGame` contains a class that extends from `SaveClassBase`, it will check for `SaveClassPath` attributes attached to the class. If a save file contains a save class with a matching path, it will create an instance of the custom class to handle serialization.

Currently, a custom save class can opt to read/write custom header data. It may also opt to handle serialization of the main data for cases where the data is not a standard property list.

Here is an example of a custom save class that handles header data specific to certain save game classes.

```cs
// Can add multiple attributes if multiple classes can be handled by the same implementation
[SaveClassPath("/Game/Blueprints/Saves/WorldSave.WorldSave_C")]
[SaveClassPath("/Game/Blueprints/Saves/WorldMetadataSave.WorldMetadataSave_C")]
internal class WorldSave : SaveClassBase
{
	private static readonly FString VersionPropertyName = new("MY_SAVE_VERSION");

	private int mDataLength;

	public int Version { get; set; }

	public int Id { get; set; }

    // Returning true here means DeserializeHeader and SerializeHeader will get called
	public override bool HasCustomHeader => true;

	public override long GetHeaderSize(PackageVersion packageVersion)
	{
		return VersionPropertyName.Length + 5 + 4 + 4 + 4 : 0;
	}

	public override void DeserializeHeader(BinaryReader reader, PackageVersion packageVersion)
	{
		FString? versionPropertyName = reader.ReadUnrealString();
		if (versionPropertyName is null || versionPropertyName != VersionPropertyName)
		{
			throw new InvalidDataException($"File is missing expected custom header {VersionPropertyName}");
		}

		Version = reader.ReadInt32();
		Id = reader.ReadInt32();
		mDataLength = reader.ReadInt32();
	}

	public override void SerializeHeader(BinaryWriter writer, long dataLength, PackageVersion packageVersion)
	{
		mDataLength = (int)dataLength;

		writer.WriteUnrealString(VersionPropertyName);
		writer.Write(Version);
		writer.Write(Id);
		writer.Write(mDataLength);
	}
}
```

The example above only implements a custom header. It is also possible to implement custom data serialization. When doing so, you can make use of `PropertySerializationHelper` to handle serialization of any property lists that exist within the data.

## Custom Save Class Json Serializers (SaveClassSerializerBase)

Part of the `UeSaveGame.Json` module. See [Json Module](json.md) for more information.
