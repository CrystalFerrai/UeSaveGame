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
using UeSaveGame.PropertyTypes;

namespace UeSaveGame.Json.PropertySerializers
{
	internal class MapPropertySerializer : IPropertySerializer
	{
		public void ToJson(UProperty property, JsonWriter writer)
		{
			MapProperty mapProperty = (MapProperty)property;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(MapProperty.KeyType));
			writer.WriteFStringValue(mapProperty.KeyType);

			writer.WritePropertyName(nameof(MapProperty.ValueType));
			writer.WriteFStringValue(mapProperty.ValueType);

			writer.WritePropertyName(nameof(MapProperty.Value));
			
			writer.WriteStartArray();

			if (mapProperty.Value is not null)
			{
				foreach (var pair in mapProperty.Value)
				{
					writer.WriteStartObject();

					writer.WritePropertyName("Key");
					PropertiesSerializer.WriteProperty(pair.Key, writer);

					writer.WritePropertyName("Value");
					PropertiesSerializer.WriteProperty(pair.Value, writer);

					writer.WriteEndObject();
				}
			}

			writer.WriteEndArray();

			writer.WriteEndObject();
		}

		public void FromJson(UProperty property, JsonReader reader)
		{
			MapProperty mapProperty = (MapProperty)property;

			List<KeyValuePair<UProperty, UProperty>> data = new();

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
						case nameof(MapProperty.KeyType):
							mapProperty.KeyType = reader.ReadAsFString();
							break;
						case nameof(MapProperty.ValueType):
							mapProperty.ValueType = reader.ReadAsFString();
							break;
						case nameof(MapProperty.Value):
							while (reader.Read())
							{
								if (reader.TokenType == JsonToken.EndArray)
								{
									break;
								}

								if (reader.TokenType == JsonToken.StartObject)
								{
									UProperty? key = null;
									UProperty? value = null;

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
												case "Key":
													key = PropertiesSerializer.ReadProperty(reader);
													break;
												case "Value":
													value = PropertiesSerializer.ReadProperty(reader);
													break;
											}
										}
									}

									if (key is null || value is null)
									{
										throw new InvalidDataException("Map entry must contain both a key and a value");
									}

									data.Add(new(key, value));
								}
							}
							break;
					}
				}
			}

			mapProperty.Value = data;
		}
	}
}
