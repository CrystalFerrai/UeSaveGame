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
	internal class TextDataSerializer_NamedFormat : ITextDataSerializer
	{
		public void ToJson(ITextData? data, JsonWriter writer)
		{
			if (data is null)
			{
				writer.WriteNull();
				return;
			}

			if (data is not TextData_NamedFormat) throw new ArgumentException($"{nameof(TextDataSerializer_NamedFormat)} does not support data type {data.GetType().Name}", nameof(data));

			TextData_NamedFormat textData = (TextData_NamedFormat)data;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(TextData_NamedFormat.FormatString));
			TextSerializer.ToJson(textData.FormatString, writer);

			writer.WritePropertyName(nameof(TextData_NamedFormat.Arguments));
			writer.WriteStartArray();
			if (textData.Arguments is not null)
			{
				foreach (var pair in textData.Arguments)
				{
					writer.WriteStartObject();

					writer.WritePropertyName("Name");
					writer.WriteValue(pair.Key);

					writer.WritePropertyName("Value");
					TextArgumentSerializer.WriteArgumentValue(pair.Value, writer);

					writer.WriteEndObject();
				}
			}
			writer.WriteEndArray();

			writer.WriteEndObject();
		}

		public ITextData? FromJson(JsonReader reader)
		{
			TextData_NamedFormat textData = new();

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
						case nameof(TextData_ArgumentFormat.FormatString):
							textData.FormatString = TextSerializer.FromJson(reader);
							break;
						case nameof(TextData_ArgumentFormat.Arguments):
							{
								List<KeyValuePair<FString, TextArgumentValue>> arguments = new();
								while (reader.Read())
								{
									if (reader.TokenType == JsonToken.EndArray)
									{
										break;
									}

									if (reader.TokenType == JsonToken.StartObject)
									{
										FString? key = null;
										TextArgumentValue? value = null;
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
													case "Name":
														key = reader.ReadAsFString();
														break;
													case "Value":
														value = TextArgumentSerializer.ReadArgumentValue(reader);
														break;
												}
											}
										}

										if (key is null || !value.HasValue)
										{
											throw new InvalidDataException("Unable to read name and value for text argument");
										}

										arguments.Add(new(key, value.Value));
									}
								}

								textData.Arguments = arguments;
							}
							break;
					}
				}
			}

			return textData;
		}
	}
}
