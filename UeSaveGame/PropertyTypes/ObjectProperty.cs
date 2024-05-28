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
	public class ObjectProperty : UProperty
    {
        protected override long ContentSize => 4 + (ObjectType?.SizeInBytes ?? 0);

        public FString? ObjectType { get; internal set; }

		public ObjectProperty(FString name)
			: this(name, new(nameof(ObjectProperty)))
		{
		}

		public ObjectProperty(FString name, FString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader, EngineVersion engineVersion)
        {
            if (includeHeader) reader.ReadByte();

            ObjectType = reader.ReadUnrealString();
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader, EngineVersion engineVersion)
        {
            if (includeHeader) writer.Write((byte)0);

            writer.WriteUnrealString(ObjectType);

            return ContentSize;
        }

        public override int GetHashCode()
        {
            if (ObjectType == null) throw new InvalidOperationException("Object property must be loaded before it can be used as a hash key.");

            int hash = base.GetHashCode();
            hash = hash * 23 + ObjectType.GetHashCode();
            return hash;
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj) && obj is ObjectProperty op && ObjectType == op.ObjectType;
        }

        public override string ToString()
        {
            return $"{Name} [{Type}] {(Value?.ToString() ?? ObjectType ?? string.Empty)}";
        }
    }
}
