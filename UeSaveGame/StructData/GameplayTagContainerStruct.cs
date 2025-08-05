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

using UeSaveGame.Util;

namespace UeSaveGame.StructData
{
	public class GameplayTagContainerStruct : BaseStructData
	{
		public IList<FString?> Tags { get; }

		public GameplayTagContainerStruct()
		{
			Tags = new List<FString?>();
		}

		public override IEnumerable<string> StructTypes
		{
			get
			{
				yield return "GameplayTagContainer";
			}
		}

		public override void Deserialize(BinaryReader reader, int size, PackageVersion packageVersion)
		{
			Tags.Clear();

			int count = reader.ReadInt32();
			for (int i = 0; i < count; ++i)
			{
				Tags.Add(reader.ReadUnrealString());
			}
		}

		public override int Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			long startPos = writer.BaseStream.Position;

			writer.Write(Tags.Count);
			foreach (FString? tag in Tags)
			{
				writer.WriteUnrealString(tag);
			}

			return (int)(writer.BaseStream.Position - startPos);
		}
	}
}
