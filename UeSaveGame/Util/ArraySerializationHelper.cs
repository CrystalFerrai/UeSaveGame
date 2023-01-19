// Copyright 2022 Crystal Ferrai
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
        public static StructProperty? Deserialize(BinaryReader reader, int count, long size, FString itemType, bool includeHeader, out UProperty[] outData)
        {
            outData = new UProperty[count];

            if (itemType == "StructProperty")
            {
                // Standard UProperty header
                FString name = reader.ReadUnrealString() ?? throw new FormatException("Error reading struct property");
                FString type = reader.ReadUnrealString() ?? throw new FormatException("Error reading struct property");
                long dataSize = reader.ReadInt64();

                // Standard struct header
                FString? structItemType = null;
                Guid guid = Guid.Empty;
                if (includeHeader)
                {
                    structItemType = reader.ReadUnrealString();
                    byte[] guidBytes = reader.ReadBytes(16);
                    guid = new Guid(guidBytes);
                    reader.ReadByte(); // terminator
                }

                for (int i = 0; i < count; ++i)
                {
                    // Data only for each item - no headers
                    StructProperty sp = new StructProperty(name, type);
                    sp.StructType = structItemType;
                    sp.StructGuid = guid;
                    sp.Deserialize(reader, dataSize, false);
                    outData[i] = sp;
                }

                return new StructProperty(name, type) { StructType = structItemType, StructGuid = guid };
            }
            else if (count > 0)
            {
                // No standard UProperty header
                Type type = UProperty.ResolveType(itemType);
                long itemSize = size / count;
                for (int i = 0; i < count; ++i)
                {
                    // Data only for each item - no headers
                    outData[i] = (UProperty?)Activator.CreateInstance(type, FString.Empty, itemType) ?? throw new FormatException("Error reading array data");
                    outData[i]?.Deserialize(reader, itemSize, false);
                }
            }

            return null;
        }

        public static long Serialize(BinaryWriter writer, FString? itemType, bool includeHeader, StructProperty? prototype, IReadOnlyList<UProperty> inData)
        {
            long size = 0;
            if (itemType == "StructProperty")
            {
                if (prototype == null) throw new InvalidOperationException("Instance is not valid for serialization");

                // Standard UProperty header
                writer.WriteUnrealString(prototype.Name);
                writer.WriteUnrealString(prototype.Type);
                size += 8 + prototype.Name.SizeInBytes + prototype.Type.SizeInBytes;

                // We need to write the data size, but we don't know it yet. Write a 0 for now, then come back and change it after writing the data.
                long sizePosition = writer.BaseStream.Position;
                writer.Write((long)0);
                size += 8;

                // Standard struct header
                if (includeHeader)
                {
                    writer.WriteUnrealString(prototype.StructType);
                    size += 4 + (prototype.StructType?.SizeInBytes ?? 0);
                    writer.Write(prototype.StructGuid.ToByteArray());
                    writer.Write((byte)0);
                    size += 17;
                }

                if (inData.Count > 0)
                {
                    long startPosition = writer.BaseStream.Position;
                    foreach (UProperty item in inData)
                    {
                        // Data only for each item - no headers
                        item.Serialize(writer, false);
                    }
                    long dataSize = writer.BaseStream.Position - startPosition;

                    // Go back and write the size now that we know it
                    long returnPosition = writer.BaseStream.Position;
                    writer.BaseStream.Seek(sizePosition, SeekOrigin.Begin);
                    writer.Write(dataSize);
                    writer.BaseStream.Seek(returnPosition, SeekOrigin.Begin);

                    size += dataSize;
                }
            }
            else
            {
                foreach (UProperty item in inData)
                {
                    // Data only for each item - no headers
                    size += item.Serialize(writer, false);
                }
            }
            return size;
        }
    }
}
