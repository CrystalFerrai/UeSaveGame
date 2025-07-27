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

namespace UeSaveGame.TextData
{
	public class TextData_Base : ITextData
	{
		public FString? Namespace { get; set; }
		public FString? Key { get; set; }
		public FString? SourceString { get; set; }

		public void Deserialize(BinaryReader reader)
		{
			Namespace = reader.ReadUnrealString();
			Key = reader.ReadUnrealString();
			SourceString = reader.ReadUnrealString();
		}

		public long Serialize(BinaryWriter writer)
		{
			writer.WriteUnrealString(Namespace);
			writer.WriteUnrealString(Key);
			writer.WriteUnrealString(SourceString);

			return 12 + (Namespace?.SizeInBytes ?? 0) + (Key?.SizeInBytes ?? 0) + (SourceString?.SizeInBytes ?? 0);
		}

		public override string ToString()
		{
			return SourceString?.Value ?? string.Empty;
		}
	}
}
