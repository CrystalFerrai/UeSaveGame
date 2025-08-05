// Copyright 2025 Crystal Ferrai
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UeSaveGame.PropertyTypes;

namespace UeSaveGame.Util
{
	/// <summary>
	/// Utility for serializing properties that contain standard UE array data
	/// </summary>
	internal static class ArraySerializationHelper
	{
		public static FPropertyTag? Deserialize(BinaryReader reader, int count, int size, FPropertyTypeName itemType, PackageVersion packageVersion, out Array outData)
		{
			if (itemType.Name == "StructProperty")
			{
				FProperty[] data = new FProperty[count];

				FPropertyTag? prototype = null;
				FString structName;
				FPropertyTypeName structType;
				Guid structGuid = Guid.Empty;
				if (packageVersion >= EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
				{
					switch (itemType.Parameters.Count)
					{
						case 1:
							structType = itemType.Parameters[0];
							break;
						case 2:
							structType = itemType.Parameters[0];
							structGuid = Guid.Parse(itemType.Parameters[1].Name);
							break;
						default:
							throw new InvalidDataException("Failed to read parameters for StructProperty");
					}
					structName = structType.Name;
				}
				else
				{
					prototype = FPropertyTag.Deserialize(reader, packageVersion);
					StructProperty prototypeProperty = (StructProperty)prototype.Property!;

					structName = prototype.Name;
					structType = prototypeProperty.StructType!;
					structGuid = prototypeProperty.StructGuid;
				}

				for (int i = 0; i < count; ++i)
				{
					// Data only for each item - no headers
					StructProperty sp = new(structName);
					sp.StructType = structType;
					sp.StructGuid = structGuid;
					sp.DeserializeValue(reader, prototype?.Size ?? 0, packageVersion);
					data[i] = sp;
				}

				outData = data;
				return prototype;
			}
			else
			{
				Type propType = FProperty.ResolveType(itemType.Name);
				FProperty prototype = ((FProperty?)Activator.CreateInstance(propType, FString.Empty)) ?? throw new FormatException("Error reading array data");

				if (prototype.IsSimpleProperty)
				{
					Type initialItemType = prototype.SimpleValueType;
					Array data = (Array?)Activator.CreateInstance(prototype.SimpleValueType.MakeArrayType(), count) ?? throw new FormatException("Error building array data");

					if (count > 0)
					{
						int itemSize = size / count;
						for (int i = 0; i < count; ++i)
						{
							prototype.DeserializeValue(reader, itemSize, packageVersion);

							if (i == 0 && prototype.SimpleValueType != initialItemType)
							{
								// The type changed after deserializtion. Recreate the array. This happens when a ByteProperty is referencing strings instead of bytes.
								data = (Array?)Activator.CreateInstance(prototype.SimpleValueType.MakeArrayType(), count) ?? throw new FormatException("Error building array data");
							}

							data.SetValue(prototype.Value, i);
						}
					}

					outData = data;
				}
				else
				{
					FProperty[] data = new FProperty[count];

					if (count > 0)
					{
						Type type = FProperty.ResolveType(itemType.Name);
						int itemSize = size / count;
						for (int i = 0; i < count; ++i)
						{
							// Data only for each item - no headers
							data[i] = (FProperty?)Activator.CreateInstance(type, FString.Empty) ?? throw new FormatException("Error reading array data");
							data[i].DeserializeValue(reader, itemSize, packageVersion);
						}
					}

					outData = data;
				}
			}

			return null;
		}

		public static int Serialize(BinaryWriter writer, FPropertyTypeName itemType, PackageVersion packageVersion, FPropertyTag? prototype, Array inData)
		{
			int size = 0;
			if (itemType.Name == "StructProperty")
			{
				if (packageVersion < EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME && prototype is null) throw new InvalidOperationException("Instance is not valid for serialization");

				size += prototype?.Serialize(writer, packageVersion) ?? 0;

				if (inData.Length > 0)
				{
					long startPosition = writer.BaseStream.Position;
					foreach (FProperty item in inData)
					{
						// Data only for each item - no headers
						item.SerializeValue(writer, packageVersion);
					}
					int dataSize = (int)(writer.BaseStream.Position - startPosition);

					// Go back and write the size now that we know it
					prototype?.WriteSize(writer, dataSize, packageVersion);

					size += dataSize;
				}
			}
			else
			{
				Type propType = FProperty.ResolveType(itemType.Name);
				FProperty itemPrototype = ((FProperty?)Activator.CreateInstance(propType, FString.Empty)) ?? throw new FormatException("Error reading array data");

				if (itemPrototype.IsSimpleProperty)
				{
					foreach (object item in inData)
					{
						itemPrototype.Value = item;
						size += itemPrototype.SerializeValue(writer, packageVersion);
					}
				}
				else
				{
					foreach (FProperty item in inData)
					{
						// Data only for each item - no headers
						size += item.SerializeValue(writer, packageVersion);
					}
				}
			}
			return size;
		}
	}
}
