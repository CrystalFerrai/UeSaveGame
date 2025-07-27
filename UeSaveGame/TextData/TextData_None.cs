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
	public class TextData_None : ITextData
	{
		public FString? Value { get; set; }

		public void Deserialize(BinaryReader reader)
		{
			bool hasInvariantString = reader.ReadInt32() != 0;
			if (hasInvariantString)
			{
				Value = reader.ReadUnrealString();
			}
		}

		public long Serialize(BinaryWriter writer)
		{
			if (Value is null)
			{
				writer.Write(0);
				return 4;
			}

			writer.Write(1);
			writer.WriteUnrealString(Value);
			return 8 + Value.SizeInBytes;
		}

		public override string ToString()
		{
			return Value?.ToString() ?? string.Empty;
		}
	}
}
