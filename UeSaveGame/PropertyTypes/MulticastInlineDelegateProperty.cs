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

using UeSaveGame.DataTypes;
using UeSaveGame.Util;

namespace UeSaveGame.PropertyTypes
{
	public class MulticastInlineDelegateProperty : FProperty<UDelegate[]>
	{
		public MulticastInlineDelegateProperty(FString name)
			: base(name)
		{
		}

		protected internal override void DeserializeValue(BinaryReader reader, int size, PackageVersion engineVersion)
		{
			int count = reader.ReadInt32();
			Value = new UDelegate[count];

			for (int i = 0; i < count; ++i)
			{
				Value[i].ClassName = reader.ReadUnrealString();
				Value[i].FunctionName = reader.ReadUnrealString();
			}
		}

		protected internal override int SerializeValue(BinaryWriter writer, PackageVersion engineVersion)
		{
			if (Value == null) throw new InvalidOperationException("Instance is not valid for serialization");

			long startPosition = writer.BaseStream.Position;

			writer.Write(Value.Length);
			foreach (UDelegate dlgt in Value)
			{
				writer.WriteUnrealString(dlgt.ClassName);
				writer.WriteUnrealString(dlgt.FunctionName);
			}

			return (int)(writer.BaseStream.Position - startPosition);
		}
	}
}
