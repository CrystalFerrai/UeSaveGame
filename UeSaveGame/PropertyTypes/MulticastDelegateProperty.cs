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
	// Note: This type was found in Subverse. The format does not appear to match UE4 serialization code for this type.
	// If the type is ever found elsewhere, we should check if the format is the same.

	public class MulticastDelegateProperty : UProperty<UDelegate[]>
	{
		public MulticastDelegateProperty(FString name)
			: this(name, new(nameof(MulticastDelegateProperty)))
		{
		}

		public MulticastDelegateProperty(FString name, FString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader, PackageVersion packageVersion)
        {
            if (includeHeader)
            {
                reader.ReadByte();
            }

            int count = reader.ReadInt32();
            Value = new UDelegate[count];

            for (int i = 0; i < count; ++i)
            {
                Value[i].ClassName = reader.ReadUnrealString();
                Value[i].FunctionName = reader.ReadUnrealString();
            }
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader, PackageVersion packageVersion)
        {
            if (Value == null) throw new InvalidOperationException("Instance is not valid for serialization");

            if (includeHeader)
            {
                writer.Write((byte)0);
            }

            long startPosition = writer.BaseStream.Position;

            writer.Write(Value.Length);
            foreach (UDelegate dlgt in Value)
            {
                writer.WriteUnrealString(dlgt.ClassName);
                writer.WriteUnrealString(dlgt.FunctionName);
            }

            return writer.BaseStream.Position - startPosition;
        }
    }

    public struct UDelegate
    {
        public FString? ClassName { get; set; }

        public FString? FunctionName { get; set; }
    }
}
