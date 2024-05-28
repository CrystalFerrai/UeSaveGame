// Copyright 2024 Crystal Ferrai
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
using UeSaveGame.Json.TextDataSerializers;
using UeSaveGame.PropertyTypes;
using UeSaveGame.TextData;

namespace UeSaveGame.Json.PropertySerializers
{
	internal class TextPropertySerializer : IPropertySerializer
	{
		private static readonly Dictionary<TextHistoryType, ITextDataSerializer> sDataSerializers;

		static TextPropertySerializer()
		{
			sDataSerializers = new()
			{
				{ TextHistoryType.None, new TextDataSerializer_None() },
				{ TextHistoryType.Base, new TextDataSerializer_Base() },
				{ TextHistoryType.AsDateTime, new TextDataSerializer_AsDateTime() },
				{ TextHistoryType.StringTableEntry, new TextDataSerializer_StringTableEntry() }
			};
		}

		public void ToJson(UProperty property, JsonWriter writer)
		{
			TextProperty textProperty = (TextProperty)property;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(TextProperty.Flags));
			writer.WriteValue(textProperty.Flags);

			writer.WritePropertyName(nameof(TextProperty.HistoryType));
			writer.WriteValue(textProperty.HistoryType);

			writer.WritePropertyName(nameof(TextProperty.Value));
			if (sDataSerializers.TryGetValue(textProperty.HistoryType, out ITextDataSerializer? dataSerializer))
			{
				dataSerializer.ToJson(textProperty.Value, writer);
			}
			else
			{
				throw new NotImplementedException($"Text serializer for history type {textProperty.HistoryType} has not been implemented");
			}

			writer.WriteEndObject();
		}

		public void FromJson(UProperty property, JsonReader reader)
		{
			TextProperty textProperty = (TextProperty)property;

			JToken? propertyValue = null;

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
						case nameof(TextProperty.Flags):
							textProperty.Flags = reader.ReadAsEnum<TextFlags>();
							break;
						case nameof(TextProperty.HistoryType):
							textProperty.HistoryType = reader.ReadAsEnum<TextHistoryType>();
							break;
						case nameof(TextProperty.Value):
							if (reader.ReadAndMoveToContent())
							{
								propertyValue = JToken.ReadFrom(reader);
							}
							break;
					}
				}
			}

			if (propertyValue is not null && sDataSerializers.TryGetValue(textProperty.HistoryType, out ITextDataSerializer? dataSerializer))
			{
				JsonReader valueReader = propertyValue.CreateReader();
				if (valueReader.Read())
				{
					textProperty.Value = dataSerializer.FromJson(valueReader);
				}
			}
			else
			{
				throw new NotImplementedException($"Text serializer for history type {textProperty.HistoryType} has not been implemented");
			}
		}
	}
}
