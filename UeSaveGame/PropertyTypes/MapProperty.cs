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

using UeSaveGame.Util;

namespace UeSaveGame.PropertyTypes
{
	public class MapProperty : FProperty<IList<KeyValuePair<FProperty, FProperty>>>
	{
		private int mRemovedCount;

		public FPropertyTypeName? KeyType { get; set; }

		public FPropertyTypeName? ValueType { get; set; }

		public MapProperty(FString name)
			: base(name)
		{
		}

		protected internal override void ProcessTypeName(FPropertyTypeName typeName, PackageVersion packageVersion)
		{
			if (packageVersion >= EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				if (typeName.Parameters.Count != 2)
				{
					throw new InvalidDataException("Failed to read key and value types for MapProperty");
				}
				KeyType = typeName.Parameters[0];
				ValueType = typeName.Parameters[1];
			}
		}

		protected internal override void DeserializeHeader(BinaryReader reader, PackageVersion packageVersion)
		{
			if (packageVersion < EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				KeyType = new(reader.ReadUnrealString()!);
				ValueType = new(reader.ReadUnrealString()!);
			}
		}

		protected internal override void DeserializeValue(BinaryReader reader, int size, PackageVersion packageVersion)
		{
			if (KeyType == null || ValueType == null) throw new InvalidOperationException("Unknown map type cannot be read.");

			mRemovedCount = reader.ReadInt32();
			if (mRemovedCount != 0)
			{
				// Maps share some serialization code with Sets. Sets can store items to be removed as well as items to be added.
				// Not sure if such a feature exists for maps, but it has not yet been encountered if it does.
				throw new NotImplementedException();
			}

			int count = reader.ReadInt32();
			Value = new List<KeyValuePair<FProperty, FProperty>>(count);
			for (int i = 0; i < count; ++i)
			{
				FProperty? key;
				{
					Type type = ResolveType(KeyType!.Name);
					key = (FProperty?)Activator.CreateInstance(type, new FString($"{mPropertyName}_Key"));
					if (key == null) throw new FormatException("Error reading map key");

					int keySize = 0;
					if (key is StructProperty structKey)
					{
						if (reader.BaseStream.CanSeek && !reader.IsUnrealStringAndNotNull())
						{
							// Guid is the only known struct type used in map keys aside from generic properties structs
							structKey.StructType = new(new("Guid"));
							keySize = 16;
						}
					}
					key.DeserializeValue(reader, keySize, packageVersion);
				}

				FProperty? value;
				{
					Type type = ResolveType(ValueType!.Name);
					value = (FProperty?)Activator.CreateInstance(type, mPropertyName);
					if (value == null) throw new FormatException("Error reading map value");
					value.DeserializeValue(reader, 0, packageVersion);
				}
				Value.Add(new KeyValuePair<FProperty, FProperty>(key, value));
			}
		}

		protected internal override void SerializeHeader(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (packageVersion < EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				writer.WriteUnrealString(KeyType!.Name);
				writer.WriteUnrealString(ValueType!.Name);
			}
		}

		protected internal override int SerializeValue(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (Value == null) throw new InvalidOperationException("Instance is not valid for serialization");

			long startPosition = writer.BaseStream.Position;

			writer.Write(mRemovedCount);

			writer.Write(Value.Count);
			foreach (var pair in Value)
			{
				pair.Key.SerializeValue(writer, packageVersion);
				pair.Value.SerializeValue(writer, packageVersion);
			}

			return (int)(writer.BaseStream.Position - startPosition);
		}

		public override string? ToString()
		{
			return $"<{KeyType},{ValueType}> Count = {Value?.Count ?? 0}";
		}
	}
}
