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

using Newtonsoft.Json;
using UeSaveGame.Json.DataSerializers;
using UeSaveGame.TextData;

namespace UeSaveGame.Json.TextDataSerializers
{
	internal static class NumberTextDataSerializer
	{
		public static void ToJson(ITextData? data, JsonWriter writer)
		{
			if (data is null)
			{
				writer.WriteNull();
				return;
			}

			if (data is not NumberTextData) throw new ArgumentException($"{nameof(NumberTextDataSerializer)} does not support data type {data.GetType().Name}", nameof(data));

			NumberTextData textData = (NumberTextData)data;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(NumberTextData.SourceValue));
			TextArgumentSerializer.WriteArgumentValue(textData.SourceValue, writer);

			if (textData.FormatOptions is not null)
			{
				writer.WritePropertyName(nameof(NumberTextData.FormatOptions));
				FNumberFormattingOptionsSerlializer.Write(writer, textData.FormatOptions);
			}

			writer.WritePropertyName(nameof(NumberTextData.TargetCulture));
			writer.WriteFStringValue(textData.TargetCulture);

			writer.WriteEndObject();
		}

		public static void FromJson(JsonReader reader, NumberTextData textData)
		{
			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.EndObject)
				{
					break;
				}

				if (reader.TokenType == JsonToken.PropertyName)
				{
					switch ((string)reader.Value!)
					{
						case nameof(NumberTextData.SourceValue):
							textData.SourceValue = TextArgumentSerializer.ReadArgumentValue(reader);
							break;
						case nameof(NumberTextData.FormatOptions):
							textData.FormatOptions = FNumberFormattingOptionsSerlializer.Read(reader);
							break;
						case nameof(NumberTextData.TargetCulture):
							textData.TargetCulture = reader.ReadAsFString();
							break;
					}
				}
			}
		}
	}

	internal static class FNumberFormattingOptionsSerlializer
	{
		public static FNumberFormattingOptions Read(JsonReader reader)
		{
			FNumberFormattingOptions options = new();

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.EndObject)
				{
					break;
				}

				if (reader.TokenType == JsonToken.PropertyName)
				{
					switch (reader.Value)
					{
						case nameof(FNumberFormattingOptions.AlwaysSign):
							options.AlwaysSign = reader.ReadAsBoolean()!.Value;
							break;
						case nameof(FNumberFormattingOptions.UseGrouping):
							options.UseGrouping = reader.ReadAsBoolean()!.Value;
							break;
						case nameof(FNumberFormattingOptions.RoundingMode):
							options.RoundingMode = (ERoundingMode)reader.ReadAsInt32()!.Value;
							break;
						case nameof(FNumberFormattingOptions.MinimumIntegralDigits):
							options.MinimumIntegralDigits = reader.ReadAsInt32()!.Value;
							break;
						case nameof(FNumberFormattingOptions.MaximumIntegralDigits):
							options.MaximumIntegralDigits = reader.ReadAsInt32()!.Value;
							break;
						case nameof(FNumberFormattingOptions.MinimumFractionalDigits):
							options.MinimumFractionalDigits = reader.ReadAsInt32()!.Value;
							break;
						case nameof(FNumberFormattingOptions.MaximumFractionalDigits):
							options.MaximumFractionalDigits = reader.ReadAsInt32()!.Value;
							break;
					}
				}
			}

			return options;
		}

		public static void Write(JsonWriter writer, FNumberFormattingOptions value)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(FNumberFormattingOptions.AlwaysSign));
			writer.WriteValue(value.AlwaysSign);

			writer.WritePropertyName(nameof(FNumberFormattingOptions.UseGrouping));
			writer.WriteValue(value.UseGrouping);

			writer.WritePropertyName(nameof(FNumberFormattingOptions.RoundingMode));
			writer.WriteValue((int)value.RoundingMode);

			writer.WritePropertyName(nameof(FNumberFormattingOptions.MinimumIntegralDigits));
			writer.WriteValue(value.MinimumIntegralDigits);

			writer.WritePropertyName(nameof(FNumberFormattingOptions.MaximumIntegralDigits));
			writer.WriteValue(value.MaximumIntegralDigits);

			writer.WritePropertyName(nameof(FNumberFormattingOptions.MinimumFractionalDigits));
			writer.WriteValue(value.MinimumFractionalDigits);

			writer.WritePropertyName(nameof(FNumberFormattingOptions.MaximumFractionalDigits));
			writer.WriteValue(value.MaximumFractionalDigits);

			writer.WriteEndObject();
		}
	}
}
