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

namespace UeSaveGame.PropertyTypes
{
	public class UInt64Property : UProperty<ulong>
    {
        protected override long ContentSize => 8;

        public override bool IsSimpleProperty => true;

		public UInt64Property(FString name)
			: this(name, new(nameof(UInt64Property)))
		{
		}

		public UInt64Property(FString name, FString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader, EngineVersion engineVersion)
        {
            if (includeHeader) reader.ReadByte();
            Value = reader.ReadUInt64();
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader, EngineVersion engineVersion)
        {
            if (includeHeader) writer.Write((byte)0);
            writer.Write(Value);

            return ContentSize;
        }
    }
}
