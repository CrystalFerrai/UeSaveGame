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
using Newtonsoft.Json.Linq;
using UeSaveGame.DataTypes;
using UeSaveGame.TextData;

namespace UeSaveGame.Json.TextDataSerializers
{
	internal class TextDataSerializer_OrderedFormat : ITextDataSerializer
	{
		public void ToJson(ITextData? data, JsonWriter writer)
		{
			if (data is null)
			{
				writer.WriteNull();
				return;
			}

			if (data is not TextData_OrderedFormat) throw new ArgumentException($"{nameof(TextDataSerializer_OrderedFormat)} does not support data type {data.GetType().Name}", nameof(data));

			TextData_OrderedFormat textData = (TextData_OrderedFormat)data;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(TextData_ArgumentFormat.FormatString));
			TextSerializer.ToJson(textData.FormatString, writer);

			writer.WritePropertyName(nameof(TextData_ArgumentFormat.Arguments));
			writer.WriteStartArray();
			if (textData.Arguments is not null)
			{
				foreach (TextArgumentValue argument in textData.Arguments)
				{
					TextArgumentSerializer.WriteArgumentValue(argument, writer);
				}
			}
			writer.WriteEndArray();

			writer.WriteEndObject();
		}

		public ITextData? FromJson(JsonReader reader)
		{
			TextData_OrderedFormat textData = new();

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
						case nameof(TextData_OrderedFormat.FormatString):
							textData.FormatString = TextSerializer.FromJson(reader);
							break;
						case nameof(TextData_OrderedFormat.Arguments):
							{
								List<TextArgumentValue> arguments = new();
								while (reader.Read())
								{
									if (reader.TokenType == JsonToken.EndArray)
									{
										break;
									}

									if (reader.TokenType == JsonToken.StartObject)
									{
										arguments.Add(TextArgumentSerializer.ReadArgumentValue(reader));
									}
								}

								textData.Arguments = arguments.ToArray();
							}
							break;
					}
				}
			}

			return textData;
		}
	}
}
