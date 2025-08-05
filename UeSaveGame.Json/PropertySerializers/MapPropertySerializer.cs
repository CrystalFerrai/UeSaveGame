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
using UeSaveGame.PropertyTypes;

namespace UeSaveGame.Json.PropertySerializers
{
	internal class MapPropertySerializer : IPropertySerializer
	{
		public void ToJson(FProperty property, JsonWriter writer)
		{
			MapProperty mapProperty = (MapProperty)property;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(MapProperty.KeyType));
			PropertyTypeNameSerializer.Write(mapProperty.KeyType, writer);

			writer.WritePropertyName(nameof(MapProperty.ValueType));
			PropertyTypeNameSerializer.Write(mapProperty.ValueType, writer);

			writer.WritePropertyName(nameof(MapProperty.Value));

			writer.WriteStartArray();

			if (mapProperty.Value is not null)
			{
				foreach (var pair in mapProperty.Value)
				{
					writer.WriteStartObject();

					writer.WritePropertyName("Key");
					IPropertySerializer keySerializer = PropertiesSerializer.GetSerializer(mapProperty.KeyType!.Name);
					keySerializer.ToJson(pair.Key, writer);

					writer.WritePropertyName("Value");
					IPropertySerializer valueSerializer = PropertiesSerializer.GetSerializer(mapProperty.ValueType!.Name);
					valueSerializer.ToJson(pair.Value, writer);

					writer.WriteEndObject();
				}
			}

			writer.WriteEndArray();

			writer.WriteEndObject();
		}

		public void FromJson(FProperty property, JsonReader reader)
		{
			MapProperty mapProperty = (MapProperty)property;

			List<KeyValuePair<FProperty, FProperty>> data = new();

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
							mapProperty.KeyType = PropertyTypeNameSerializer.Read(reader);
							break;
						case nameof(MapProperty.ValueType):
							mapProperty.ValueType = PropertyTypeNameSerializer.Read(reader);
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
									FProperty? key = null;
									FProperty? value = null;

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
													{
														reader.Read();
														IPropertySerializer keySerializer = PropertiesSerializer.GetSerializer(mapProperty.KeyType!.Name);
														key = FProperty.Create(FString.Empty, mapProperty.KeyType!);
														keySerializer.FromJson(key, reader);
													}
													break;
												case "Value":
													{
														reader.Read();
														IPropertySerializer keySerializer = PropertiesSerializer.GetSerializer(mapProperty.ValueType!.Name);
														value = FProperty.Create(FString.Empty, mapProperty.ValueType!);
														keySerializer.FromJson(value, reader);
													}
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
