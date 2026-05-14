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

using UeSaveGame.Util;

namespace UeSaveGame.TextData
{
	internal abstract class NumberTextData
	{
		public TextArgumentValue SourceValue;
		public FNumberFormattingOptions? FormatOptions;
		public FString? TargetCulture;

		protected void Deserialize(BinaryReader reader, PackageVersion version)
		{
			SourceValue = TextArgumentValue.Deserialize(reader, version);
			
			bool hasFormatOptions = reader.ReadInt32() != 0;
			if (hasFormatOptions)
			{
				FormatOptions = FNumberFormattingOptions.Deserialize(reader, version);
			}

			TargetCulture = reader.ReadUnrealString();
		}

		protected int Serialize(BinaryWriter writer, PackageVersion version)
		{
			int size = SourceValue.Serialize(writer, version);

			size += 4;
			if (FormatOptions is not null)
			{
				writer.Write(1); // HasFormatOptions
				size += FormatOptions.Serialize(writer, version);
			}
			else
			{
				writer.Write(0); // HasFormatOptions
			}

			writer.WriteUnrealString(TargetCulture);
			size += 4 + (TargetCulture?.SizeInBytes ?? 0);

			return size;
		}
	}

	internal class FNumberFormattingOptions
	{
		private const int DBL_DIG = 15;
		private const int DBL_MAX_10_EXP = 308;

		public bool AlwaysSign;
		public bool UseGrouping;
		public ERoundingMode RoundingMode;
		public int MinimumIntegralDigits;
		public int MaximumIntegralDigits;
		public int MinimumFractionalDigits;
		public int MaximumFractionalDigits;

		public FNumberFormattingOptions()
		{
			AlwaysSign = false;
			UseGrouping = true;
			RoundingMode = ERoundingMode.HalfToEven;
			MinimumIntegralDigits = 1;
			MaximumIntegralDigits = DBL_MAX_10_EXP + DBL_DIG + 1;
			MinimumFractionalDigits = 0;
			MaximumFractionalDigits = 3;
		}

		public static FNumberFormattingOptions Deserialize(BinaryReader reader, PackageVersion version)
		{
			FNumberFormattingOptions result = new();

			// Version corresponds to FEditorObjectVersion::AddedAlwaysSignNumberFormattingOption (UE 4.20)
			if (version >= EObjectUE4Version.VER_UE4_ADDED_PACKAGE_SUMMARY_LOCALIZATION_ID)
			{
				result.AlwaysSign = reader.ReadInt32() != 0;
			}
			result.UseGrouping = reader.ReadInt32() != 0;
			result.RoundingMode = (ERoundingMode)reader.ReadSByte();
			result.MinimumIntegralDigits = reader.ReadInt32();
			result.MaximumIntegralDigits = reader.ReadInt32();
			result.MinimumFractionalDigits = reader.ReadInt32();
			result.MaximumFractionalDigits = reader.ReadInt32();

			return result;
		}

		public int Serialize(BinaryWriter writer, PackageVersion version)
		{
			int size = 21;

			// Version corresponds to FEditorObjectVersion::AddedAlwaysSignNumberFormattingOption (UE 4.20)
			if (version >= EObjectUE4Version.VER_UE4_ADDED_PACKAGE_SUMMARY_LOCALIZATION_ID)
			{
				writer.Write(AlwaysSign ? 1 : 0);
				size += 4;
			}
			writer.Write(UseGrouping ? 1 : 0);
			writer.Write((sbyte)RoundingMode);
			writer.Write(MinimumIntegralDigits);
			writer.Write(MaximumIntegralDigits);
			writer.Write(MinimumFractionalDigits);
			writer.Write(MaximumFractionalDigits);

			return size;
		}
	}

	public enum ERoundingMode : sbyte
	{
		/** Rounds to the nearest place, equidistant ties go to the value which is closest to an even value: 1.5 becomes 2, 0.5 becomes 0 */
		HalfToEven,
		/** Rounds to nearest place, equidistant ties go to the value which is further from zero: -0.5 becomes -1.0, 0.5 becomes 1.0 */
		HalfFromZero,
		/** Rounds to nearest place, equidistant ties go to the value which is closer to zero: -0.5 becomes 0, 0.5 becomes 0. */
		HalfToZero,
		/** Rounds to the value which is further from zero, "larger" in absolute value: 0.1 becomes 1, -0.1 becomes -1 */
		FromZero,
		/** Rounds to the value which is closer to zero, "smaller" in absolute value: 0.1 becomes 0, -0.1 becomes 0 */
		ToZero,
		/** Rounds to the value which is more negative: 0.1 becomes 0, -0.1 becomes -1 */
		ToNegativeInfinity,
		/** Rounds to the value which is more positive: 0.1 becomes 1, -0.1 becomes 0 */
		ToPositiveInfinity
	}
}
