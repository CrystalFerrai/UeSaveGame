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
	public class EnumProperty : UProperty<FString>
    {
        protected override long ContentSize => 4 + (Value?.SizeInBytes ?? 0);

        public FString? EnumType { get; private set; }

        public override bool IsSimpleProperty => true;

        public EnumProperty(FString name)
            : this(name, new(nameof(EnumProperty)))
        {
        }

        public EnumProperty(FString name, FString type)
            : base(name, type)
        {
        }

        public EnumProperty(FString name, FString type, FString? enumType)
            : this(name, type)
        {
            EnumType = enumType;
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader, EngineVersion engineVersion)
        {
            if (includeHeader)
            {
                EnumType = reader.ReadUnrealString();
                reader.ReadByte();
            }

            Value = reader.ReadUnrealString();

            if (Value?.Value != null && !includeHeader)
            {
                EnumType = new FString(Value.Value.Substring(0, Value.Value.IndexOf(":")), Value.Encoding);
            }
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader, EngineVersion engineVersion)
        {
            if (Value == null) throw new InvalidOperationException("Instance is not valid for serialization");

            if (includeHeader)
            {
                if (EnumType == null) throw new InvalidOperationException("Instance is not valid for serialization");

                writer.WriteUnrealString(EnumType);
                writer.Write((byte)0);
            }
            writer.WriteUnrealString(Value);

            return ContentSize;
        }
    }
}
