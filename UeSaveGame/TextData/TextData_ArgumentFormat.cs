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

namespace UeSaveGame.TextData
{
	public class TextData_ArgumentFormat : ITextData
	{
		public FText? FormatString { get; set; }

		public TextArgument[]? Arguments { get; set; }

		public void Deserialize(BinaryReader reader)
		{
			FormatString = new();
			FormatString.Deserialize(reader);

			if (FormatString.Value is TextData_Base tdn && tdn.SourceString is not null && tdn.SourceString.Equals("{AAA} {BB}{X} {CCC}"))
			{
				System.Diagnostics.Debugger.Break();
			}

			int argumentCount = reader.ReadInt32();
			Arguments = new TextArgument[argumentCount];
			for (int i = 0; i < argumentCount; ++i)
			{
				Arguments[i] = TextArgument.Deserialize(reader);
			}
		}

		public long Serialize(BinaryWriter writer)
		{
			if (FormatString is null) throw new InvalidOperationException("TextData_ArgumentFormat has no format string");
			long size = FormatString.Serialize(writer);

			if (Arguments is null)
			{
				writer.Write(0);
				return size + 4;
			}

			writer.Write(Arguments.Length);
			size += 4;
			for (int i = 0; i < Arguments.Length; ++i)
			{
				size += Arguments[i].Serialize(writer);
			}

			return size;
		}

		public override string ToString()
		{
			return FormatString?.ToString() ?? string.Empty;
		}
	}

	public struct TextArgument
	{
		public FString? Name;
		public EFormatArgumentType Type;
		public object Value;

		public static TextArgument Deserialize(BinaryReader reader)
		{
			TextArgument result = new();
			result.Name = reader.ReadUnrealString();
			result.Type = (EFormatArgumentType)reader.ReadByte();
			switch (result.Type)
			{
				case EFormatArgumentType.Int:
					result.Value = reader.ReadInt32();
					break;
				case EFormatArgumentType.UInt:
					result.Value = reader.ReadUInt32();
					break;
				case EFormatArgumentType.Float:
					result.Value = reader.ReadSingle();
					break;
				case EFormatArgumentType.Double:
					result.Value = reader.ReadDouble();
					break;
				case EFormatArgumentType.Text:
					result.Value = new FText();
					((FText)result.Value).Deserialize(reader);
					break;
				case EFormatArgumentType.Gender:
					result.Value = (ETextGender)reader.ReadByte();
					break;
			}

			return result;
		}

		public long Serialize(BinaryWriter writer)
		{
			writer.WriteUnrealString(Name);
			writer.Write((byte)Type);
			long size = 4 + (Name?.SizeInBytes ?? 0) + 1;

			switch (Type)
			{
				case EFormatArgumentType.Int:
					writer.Write((int)Value);
					size += 4;
					break;
				case EFormatArgumentType.UInt:
					writer.Write((uint)Value);
					size += 4;
					break;
				case EFormatArgumentType.Float:
					writer.Write((float)Value);
					size += 4;
					break;
				case EFormatArgumentType.Double:
					writer.Write((double)Value);
					size += 8;
					break;
				case EFormatArgumentType.Text:
					size += ((FText)Value).Serialize(writer);
					break;
				case EFormatArgumentType.Gender:
					writer.Write((byte)(ETextGender)Value);
					size += 1;
					break;
			}

			return size;
		}

		public override string ToString()
		{
			return Name ?? string.Empty;
		}
	}

	public enum EFormatArgumentType
	{
		Int,
		UInt,
		Float,
		Double,
		Text,
		Gender
	}

	public enum ETextGender
	{
		Masculine,
		Feminine,
		Neuter
	};
}
