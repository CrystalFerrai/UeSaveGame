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
	public class ByteProperty : UProperty
    {
        protected override long ContentSize => Value is byte ? 1 : 4 + (((FString?)Value)?.SizeInBytes ?? 0);

        public FString? EnumType { get; set; }

        public override bool IsSimpleProperty => true;

        public override Type SimpleValueType => Value is FString ? typeof(FString) : typeof(byte);

        public ByteProperty(FString name)
            : this(name, new(nameof(ByteProperty)))
        {
        }

		public ByteProperty(FString name, FString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader, EngineVersion engineVersion)
        {
            if (includeHeader)
            {
                EnumType = reader.ReadUnrealString();
                reader.ReadByte();
            }

            switch (size)
            {
                case 1:
                    Value = reader.ReadByte();
                    break;
                default:
                    Value = reader.ReadUnrealString();
                    break;
            }
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader, EngineVersion engineVersion)
        {
            if (includeHeader)
            {
                writer.WriteUnrealString(EnumType);
                writer.Write((byte)0);
            }

            if (Value is byte b)
            {
                writer.Write(b);
            }
            else if (Value is FString s)
            {
                writer.WriteUnrealString(s);
            }

            return ContentSize;
        }
    }
}
