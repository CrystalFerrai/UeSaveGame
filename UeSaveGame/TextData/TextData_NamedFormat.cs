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

using System.Xml.Linq;
using UeSaveGame.DataTypes;
using UeSaveGame.Util;

namespace UeSaveGame.TextData
{
	public class TextData_NamedFormat : ITextData
	{
		public FText? FormatString { get; set; }

		public List<KeyValuePair<FString, TextArgumentValue>>? Arguments { get; set; }

		public void Deserialize(BinaryReader reader, PackageVersion packageVersion)
		{
			FormatString = new();
			FormatString.Deserialize(reader, packageVersion);

			int argumentCount = reader.ReadInt32();
			Arguments = new(argumentCount);
			for (int i = 0; i < argumentCount; ++i)
			{
				FString? name = reader.ReadUnrealString();
				if (name is null) throw new InvalidDataException($"TextData_NamedFormat argument {i} has a null name");

				TextArgumentValue value = TextArgumentValue.Deserialize(reader, packageVersion);

				Arguments.Add(new(name, value));
			}
		}

		public int Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (FormatString is null) throw new InvalidOperationException("TextData_NamedFormat has no format string");

			int size = 0;
			if (Arguments is null)
			{
				writer.Write(0);
			}
			else
			{
				size += FormatString.Serialize(writer, packageVersion);

				writer.Write(Arguments.Count);
				size += 4;
				long argPos = writer.BaseStream.Position;
				for (int i = 0; i < Arguments.Count; ++i)
				{
					writer.WriteUnrealString(Arguments[i].Key);
					Arguments[i].Value.Serialize(writer, packageVersion);
				}
				size += (int)(writer.BaseStream.Position - argPos);
			}

			return size;
		}

		public override string ToString()
		{
			return FormatString?.ToString() ?? string.Empty;
		}
	}
}
