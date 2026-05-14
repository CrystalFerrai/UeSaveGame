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
	public class TextData_OrderedFormat : ITextData
	{
		public FText? FormatString { get; set; }

		public TextArgumentValue[]? Arguments { get; set; }

		public void Deserialize(BinaryReader reader, PackageVersion packageVersion)
		{
			FormatString = new();
			FormatString.Deserialize(reader, packageVersion);

			int argumentCount = reader.ReadInt32();
			Arguments = new TextArgumentValue[argumentCount];
			for (int i = 0; i < argumentCount; ++i)
			{
				Arguments[i] = TextArgumentValue.Deserialize(reader, packageVersion);
			}
		}

		public int Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (FormatString is null) throw new InvalidOperationException("TextData_OrderedFormat has no format string");
			int size = FormatString.Serialize(writer, packageVersion);

			if (Arguments is null)
			{
				writer.Write(0);
				return size + 4;
			}

			writer.Write(Arguments.Length);
			size += 4;
			for (int i = 0; i < Arguments.Length; ++i)
			{
				size += Arguments[i].Serialize(writer, packageVersion);
			}

			return size;
		}

		public override string ToString()
		{
			return FormatString?.ToString() ?? string.Empty;
		}
	}
}
