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
	public class ArrayProperty : UProperty<Array>
    {
        internal StructProperty? StructPrototype { get; set; }

        public FString? ItemType { get; set; }

        public ArrayProperty(FString name)
            : this(name, new(nameof(ArrayProperty)))
        {
        }

        public ArrayProperty(FString name, FString type)
            : base(name, type)
        {
        }

		public ArrayProperty(FString name, FString type, FString itemType)
			: this(name, type)
		{
            ItemType = itemType;
		}

		public override void Deserialize(BinaryReader reader, long size, bool includeHeader, PackageVersion packageVersion)
        {
            if (includeHeader)
            {
                ItemType = reader.ReadUnrealString();
                reader.ReadByte();
            }

            if (ItemType == null) throw new InvalidOperationException("Cannot read array with unknown item type");

            int count = reader.ReadInt32();

            Array? data;
            StructPrototype = ArraySerializationHelper.Deserialize(reader, count, size - 4, ItemType, packageVersion, includeHeader, out data);
            Value = data;
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader, PackageVersion packageVersion)
        {
            if (Value == null) throw new InvalidOperationException("Instance is not valid for serialization");
            if (ItemType == null) throw new InvalidOperationException("Cannot serialize array with unknown item type");

            if (includeHeader)
            {
                writer.WriteUnrealString(ItemType);
                writer.Write((byte)0);
            }

            long size = 4;
            writer.Write(Value.Length);

            size += ArraySerializationHelper.Serialize(writer, ItemType, packageVersion, includeHeader, StructPrototype, Value);

            return size;
        }

        public override string ToString()
        {
            string? valueString = Value?.Length == 1 && Value.GetValue(0) != null ? Value.GetValue(0)!.ToString() : $"Count = {Value?.Length ?? 0}";
            return Value == null ? base.ToString() : $"{Name} [{nameof(ArrayProperty)}<{ItemType}>] {valueString}";
        }
    }
}
