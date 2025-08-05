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
using UeSaveGame.TextData;

namespace UeSaveGame.Json.TextDataSerializers
{
	internal class TextDataSerializer_Base : ITextDataSerializer
	{
		public void ToJson(ITextData? data, JsonWriter writer)
		{
			if (data is null)
			{
				writer.WriteNull();
				return;
			}

			if (data is not TextData_Base) throw new ArgumentException($"{nameof(TextDataSerializer_Base)} does not support data type {data.GetType().Name}", nameof(data));

			TextData_Base textData = (TextData_Base)data;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(TextData_Base.Namespace));
			writer.WriteFStringValue(textData.Namespace);

			writer.WritePropertyName(nameof(TextData_Base.Key));
			writer.WriteFStringValue(textData.Key);

			writer.WritePropertyName(nameof(TextData_Base.SourceString));
			writer.WriteFStringValue(textData.SourceString);

			writer.WriteEndObject();
		}

		public ITextData? FromJson(JsonReader reader)
		{
			TextData_Base textData = new();

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
						case nameof(TextData_Base.Namespace):
							textData.Namespace = reader.ReadAsFString();
							break;
						case nameof(TextData_Base.Key):
							textData.Key = reader.ReadAsFString();
							break;
						case nameof(TextData_Base.SourceString):
							textData.SourceString = reader.ReadAsFString();
							break;
					}
				}
			}

			return textData;
		}
	}
}
