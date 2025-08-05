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

		public void Deserialize(BinaryReader reader, PackageVersion packageVersion)
		{
			FormatString = new();
			FormatString.Deserialize(reader, packageVersion);

			int argumentCount = reader.ReadInt32();
			Arguments = new TextArgument[argumentCount];
			for (int i = 0; i < argumentCount; ++i)
			{
				Arguments[i] = TextArgument.Deserialize(reader, packageVersion);
			}
		}

		public int Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (FormatString is null) throw new InvalidOperationException("TextData_ArgumentFormat has no format string");
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

	public struct TextArgument
	{
		public FString? Name;
		public EFormatArgumentType Type;
		public object Value;

		public static TextArgument Deserialize(BinaryReader reader, PackageVersion packageVersion)
		{
			TextArgument result = new();
			result.Name = reader.ReadUnrealString();
			result.Type = (EFormatArgumentType)reader.ReadByte();
			switch (result.Type)
			{
				case EFormatArgumentType.Int:
					// FUE5ReleaseStreamObjectVersion.TextFormatArgumentData64bitSupport is checked in the engine, which corresponds
					// to the same engine release as EObjectUE5Version.LARGE_WORLD_COORDINATES
					if (packageVersion >= EObjectUE5Version.LARGE_WORLD_COORDINATES)
					{
						result.Value = reader.ReadInt64();
					}
					else
					{
						result.Value = reader.ReadInt32();
					}
					break;
				case EFormatArgumentType.UInt:
					// This case is not implemented in the engine, so we shouldn't see it here unless the engine has been updated to add it
					throw new NotImplementedException();
				case EFormatArgumentType.Float:
					result.Value = reader.ReadSingle();
					break;
				case EFormatArgumentType.Double:
					result.Value = reader.ReadDouble();
					break;
				case EFormatArgumentType.Text:
					result.Value = new FText();
					((FText)result.Value).Deserialize(reader, packageVersion);
					break;
				case EFormatArgumentType.Gender:
					result.Value = (ETextGender)reader.ReadByte();
					break;
			}

			return result;
		}

		public int Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			writer.WriteUnrealString(Name);
			writer.Write((byte)Type);
			int size = 4 + (Name?.SizeInBytes ?? 0) + 1;

			switch (Type)
			{
				case EFormatArgumentType.Int:
					// FUE5ReleaseStreamObjectVersion.TextFormatArgumentData64bitSupport is checked in the engine, which corresponds
					// to the same engine release as EObjectUE5Version.LARGE_WORLD_COORDINATES
					if (packageVersion >= EObjectUE5Version.LARGE_WORLD_COORDINATES)
					{
						if (Value is int vi)
						{
							writer.Write((long)vi);
						}
						else
						{
							writer.Write((long)Value);
						}
						size += 8;
					}
					else
					{
						if (Value is long vl)
						{
							writer.Write((int)vl);
						}
						else
						{
							writer.Write((int)Value);
						}
						size += 4;
					}
					break;
				case EFormatArgumentType.UInt:
					// This case is not implemented in the engine, so we shouldn't see it here unless the engine has been updated to add it
					throw new NotImplementedException();
				case EFormatArgumentType.Float:
					writer.Write((float)Value);
					size += 4;
					break;
				case EFormatArgumentType.Double:
					writer.Write((double)Value);
					size += 8;
					break;
				case EFormatArgumentType.Text:
					size += ((FText)Value).Serialize(writer, packageVersion);
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
