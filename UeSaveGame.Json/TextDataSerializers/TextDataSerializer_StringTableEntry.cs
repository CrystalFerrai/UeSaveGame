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
using UeSaveGame.TextData;

namespace UeSaveGame.Json.TextDataSerializers
{
	internal class TextDataSerializer_StringTableEntry : ITextDataSerializer
	{
		public void ToJson(ITextData? data, JsonWriter writer)
		{
			if (data is null)
			{
				writer.WriteNull();
				return;
			}

			if (data is not TextData_StringTableEntry) throw new ArgumentException($"{nameof(TextDataSerializer_StringTableEntry)} does not support data type {data.GetType().Name}", nameof(data));

			TextData_StringTableEntry textData = (TextData_StringTableEntry)data;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(TextData_StringTableEntry.Table));
			writer.WriteFStringValue(textData.Table);

			writer.WritePropertyName(nameof(TextData_StringTableEntry.Key));
			writer.WriteFStringValue(textData.Key);

			writer.WriteEndObject();
		}

		public ITextData? FromJson(JsonReader reader)
		{
			TextData_StringTableEntry textData = new();

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
						case nameof(TextData_StringTableEntry.Table):
							textData.Table = reader.ReadAsFString();
							break;
						case nameof(TextData_StringTableEntry.Key):
							textData.Key = reader.ReadAsFString();
							break;
					}
				}
			}

			return textData;
		}
	}
}
