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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UeSaveGame.DataTypes;
using UeSaveGame.PropertyTypes;
using UeSaveGame.TextData;

namespace UeSaveGame.Json.TextDataSerializers
{
	internal static class TextSerializer
	{
		private static readonly Dictionary<TextHistoryType, ITextDataSerializer> sDataSerializers;

		static TextSerializer()
		{
			sDataSerializers = new()
			{
				{ TextHistoryType.None, new TextDataSerializer_None() },
				{ TextHistoryType.ArgumentFormat, new TextDataSerializer_ArgumentFormat() },
				{ TextHistoryType.Base, new TextDataSerializer_Base() },
				{ TextHistoryType.AsDateTime, new TextDataSerializer_AsDateTime() },
				{ TextHistoryType.StringTableEntry, new TextDataSerializer_StringTableEntry() }
			};
		}

		public static void ToJson(FText? text, JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(FText.Flags));
			writer.WriteValue(text?.Flags);

			writer.WritePropertyName(nameof(FText.HistoryType));
			writer.WriteValue(text?.HistoryType);

			writer.WritePropertyName(nameof(TextProperty.Value));
			if (text is null)
			{
				writer.WriteNull();
			}
			else
			{
				ITextDataSerializer dataSerializer = GetDataSerializer(text.HistoryType);
				dataSerializer.ToJson(text.Value, writer);
			}

			writer.WriteEndObject();
		}

		public static FText FromJson(JsonReader reader)
		{
			JToken? textValue = null;
			FText text = new();

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
						case nameof(FText.Flags):
							text.Flags = reader.ReadAsEnum<TextFlags>();
							break;
						case nameof(FText.HistoryType):
							text.HistoryType = reader.ReadAsEnum<TextHistoryType>();
							break;
						case nameof(FText.Value):
							if (reader.Read())
							{
								textValue = JToken.ReadFrom(reader);
							}
							break;
					}
				}
			}

			if (textValue is not null)
			{
				ITextDataSerializer dataSerializer = GetDataSerializer(text.HistoryType);
				JsonReader valueReader = textValue.CreateReader();
				if (valueReader.Read())
				{
					text.Value = dataSerializer.FromJson(valueReader);
				}
			}

			return text;
		}

		private static ITextDataSerializer GetDataSerializer(TextHistoryType historyType)
		{
			if (sDataSerializers.TryGetValue(historyType, out ITextDataSerializer? dataSerializer))
			{
				return dataSerializer;
			}
			else
			{
				throw new NotImplementedException($"Text serializer for history type {historyType} has not been implemented");
			}
		}
	}
}
