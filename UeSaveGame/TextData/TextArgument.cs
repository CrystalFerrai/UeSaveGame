// Copyright 2026 Crystal Ferrai
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
	public struct TextArgument
	{
		public FString? Name;
		public TextArgumentValue Value;

		public static TextArgument Deserialize(BinaryReader reader, PackageVersion packageVersion)
		{
			TextArgument result = new();
			result.Name = reader.ReadUnrealString();
			result.Value = TextArgumentValue.Deserialize(reader, packageVersion, true);

			return result;
		}

		public int Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			writer.WriteUnrealString(Name);
			int size = 4 + (Name?.SizeInBytes ?? 0);

			size += Value.Serialize(writer, packageVersion, true);

			return size;
		}

		public override string ToString()
		{
			return Name ?? string.Empty;
		}
	}

	public struct TextArgumentValue
	{
		public EFormatArgumentType Type;
		public object Value;

		public static TextArgumentValue Deserialize(BinaryReader reader, PackageVersion packageVersion, bool isArgumentTextData = false)
		{
			TextArgumentValue result = new();
			result.Type = (EFormatArgumentType)reader.ReadByte();
			switch (result.Type)
			{
				case EFormatArgumentType.Int:
					// FUE5ReleaseStreamObjectVersion.TextFormatArgumentData64bitSupport is checked in the engine, which corresponds
					// to the same engine release as EObjectUE5Version.LARGE_WORLD_COORDINATES
					if (isArgumentTextData && packageVersion >= EObjectUE5Version.LARGE_WORLD_COORDINATES)
					{
						result.Value = reader.ReadInt32();
					}
					else
					{
						result.Value = reader.ReadInt64();
					}
					break;
				case EFormatArgumentType.UInt:
					result.Value = reader.ReadUInt64();
					break;
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

		public int Serialize(BinaryWriter writer, PackageVersion packageVersion, bool isArgumentTextData = false)
		{
			writer.Write((byte)Type);
			int size = 1;

			switch (Type)
			{
				case EFormatArgumentType.Int:
					// FUE5ReleaseStreamObjectVersion.TextFormatArgumentData64bitSupport is checked in the engine, which corresponds
					// to the same engine release as EObjectUE5Version.LARGE_WORLD_COORDINATES
					if (isArgumentTextData && packageVersion >= EObjectUE5Version.LARGE_WORLD_COORDINATES)
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
					else
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
					break;
				case EFormatArgumentType.UInt:
					writer.Write((long)Value);
					size += 8;
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
					size += ((FText)Value).Serialize(writer, packageVersion);
					break;
				case EFormatArgumentType.Gender:
					writer.Write((byte)(ETextGender)Value);
					size += 1;
					break;
			}

			return size;
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
