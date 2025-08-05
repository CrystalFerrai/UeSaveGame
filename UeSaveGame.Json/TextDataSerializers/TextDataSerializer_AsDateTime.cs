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
using UeSaveGame.Json.DataSerializers;
using UeSaveGame.TextData;

namespace UeSaveGame.Json.TextDataSerializers
{
	internal class TextDataSerializer_AsDateTime : ITextDataSerializer
	{
		public void ToJson(ITextData? data, JsonWriter writer)
		{
			if (data is null)
			{
				writer.WriteNull();
				return;
			}

			if (data is not TextData_AsDateTime) throw new ArgumentException($"{nameof(TextDataSerializer_AsDateTime)} does not support data type {data.GetType().Name}", nameof(data));

			TextData_AsDateTime textData = (TextData_AsDateTime)data;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(TextData_AsDateTime.DateTime));
			DateTimeSerializer.ToJson(textData.DateTime, writer);

			writer.WritePropertyName(nameof(TextData_AsDateTime.DateStyle));
			writer.WriteValue(textData.DateStyle);

			writer.WritePropertyName(nameof(TextData_AsDateTime.TimeStyle));
			writer.WriteValue(textData.TimeStyle);

			writer.WritePropertyName(nameof(TextData_AsDateTime.TimeZone));
			writer.WriteFStringValue(textData.TimeZone);

			writer.WritePropertyName(nameof(TextData_AsDateTime.CultureName));
			writer.WriteFStringValue(textData.CultureName);

			writer.WriteEndObject();
		}

		public ITextData? FromJson(JsonReader reader)
		{
			TextData_AsDateTime textData = new();

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
						case nameof(TextData_AsDateTime.DateTime):
							textData.DateTime = DateTimeSerializer.FromJson(reader);
							break;
						case nameof(TextData_AsDateTime.DateStyle):
							textData.DateStyle = reader.ReadAsEnum<DateTimeStyle>();
							break;
						case nameof(TextData_AsDateTime.TimeStyle):
							textData.TimeStyle = reader.ReadAsEnum<DateTimeStyle>();
							break;
						case nameof(TextData_AsDateTime.TimeZone):
							textData.TimeZone = reader.ReadAsFString();
							break;
						case nameof(TextData_AsDateTime.CultureName):
							textData.CultureName = reader.ReadAsFString();
							break;
					}
				}
			}

			return textData;
		}
	}
}
