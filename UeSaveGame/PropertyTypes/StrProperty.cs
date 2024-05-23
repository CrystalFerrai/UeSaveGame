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
	public class StrProperty : UProperty<FString>
    {
        protected override long ContentSize => Value == null ? 4 : 4 + Value.SizeInBytes;

        public override bool IsSimpleProperty => true;

		public StrProperty(FString name)
			: this(name, new(nameof(StrProperty)))
		{
		}

		public StrProperty(FString name, FString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader, EngineVersion engineVersion)
        {
            if (includeHeader) reader.ReadByte();
            Value = reader.ReadUnrealString();
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader, EngineVersion engineVersion)
        {
            if (includeHeader) writer.Write((byte)0);
            writer.WriteUnrealString(Value);

            return ContentSize;
        }
    }
}
