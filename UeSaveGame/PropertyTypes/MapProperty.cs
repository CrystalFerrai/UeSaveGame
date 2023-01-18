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

using UeSaveGame.Util;

namespace UeSaveGame.PropertyTypes
{
	public class MapProperty : UProperty<IList<KeyValuePair<UProperty, UProperty>>>
    {
        private int mRemovedCount;

        public FString? KeyType { get; private set; }
        public FString? ValueType { get; private set; }

        public MapProperty(FString name, FString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader)
            {
                KeyType = reader.ReadUnrealString();
                ValueType = reader.ReadUnrealString();
                reader.ReadByte();
            }

            if (KeyType == null || ValueType == null) throw new InvalidOperationException("Unknown map type cannot be read.");

            mRemovedCount = reader.ReadInt32();
            if (mRemovedCount != 0)
            {
                // Maps share some serialization code with Sets. Sets can store items to be removed as well as items to be added.
                // Not sure if such a feature exists for maps, but it has not yet been encountered if it does.
                throw new NotImplementedException();
            }

            int count = reader.ReadInt32();
            Value = new List<KeyValuePair<UProperty, UProperty>>(count);
            for (int i = 0; i < count; ++i)
            {
                UProperty? key;
                {
                    Type type = ResolveType(KeyType);
                    key = (UProperty?)Activator.CreateInstance(type, FString.Empty, KeyType);
                    if (key == null) throw new FormatException("Error reading map key");
                    key.Deserialize(reader, 0, false);
                }

                UProperty? value;
                {
                    Type type = ResolveType(ValueType);
                    value = (UProperty?)Activator.CreateInstance(type, Name, ValueType);
                    if (value == null) throw new FormatException("Error reading map value");
                    value.Deserialize(reader, 0, false);
                }
                Value.Add(new KeyValuePair<UProperty, UProperty>(key, value));
            }
            tempSize = size;
        }

        long tempSize;

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (Value == null) throw new InvalidOperationException("Instance is not valid for serialization");

            if (includeHeader)
            {
                writer.WriteUnrealString(KeyType);
                writer.WriteUnrealString(ValueType);
                writer.Write((byte)0);
            }

            long startPosition = writer.BaseStream.Position;

            writer.Write(mRemovedCount);

            writer.Write(Value.Count);
            foreach (var pair in Value)
            {
                pair.Key.Serialize(writer, false);
                pair.Value.Serialize(writer, false);
            }

            return writer.BaseStream.Position - startPosition;
        }

        public override string ToString()
        {
            return Value == null ? base.ToString() : $"{Name} [{nameof(MapProperty)}<{KeyType},{ValueType}>] Count = {Value.Count}";
        }
    }
}
