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
	public class SoftObjectProperty : UProperty
	{
        public FString? AssetPath { get; private set; }

        public int Unknown { get; private set; } // Maybe an import/export index in the referenced asset? Have only seen 0

        protected override long ContentSize => (AssetPath?.SizeInBytes ?? 0) + 8;

        public SoftObjectProperty(FString name, FString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader) reader.ReadByte();

            AssetPath = reader.ReadUnrealString();
            Unknown = reader.ReadInt32();
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader) writer.Write((byte)0);

            writer.WriteUnrealString(AssetPath);
            writer.Write(Unknown);

            return ContentSize;
        }
    }
}
