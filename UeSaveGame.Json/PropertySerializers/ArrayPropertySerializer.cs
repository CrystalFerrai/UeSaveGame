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
using UeSaveGame.PropertyTypes;

namespace UeSaveGame.Json.PropertySerializers
{
	internal class ArrayPropertySerializer : IPropertySerializer
	{
		public void ToJson(UProperty property, JsonWriter writer)
		{
			ArrayProperty arrayProperty = (ArrayProperty)property;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(ArrayProperty.ItemType));
			writer.WriteFStringValue(arrayProperty.ItemType);

			writer.WritePropertyName(nameof(ArrayProperty.StructPrototype));
			if (arrayProperty.ItemType == nameof(StructProperty))
			{
				if (arrayProperty.StructPrototype is null) throw new InvalidDataException("Array property has struct data but is missing a struct prototype.");

				writer.WriteStartObject();

				writer.WritePropertyName(nameof(StructProperty.Name));
				writer.WriteFStringValue(arrayProperty.StructPrototype.Name);

				writer.WritePropertyName(nameof(StructProperty.Type));
				writer.WriteFStringValue(arrayProperty.StructPrototype.Type);

				writer.WritePropertyName(nameof(StructProperty.StructType));
				writer.WriteFStringValue(arrayProperty.StructPrototype.StructType);

				writer.WritePropertyName(nameof(StructProperty.StructGuid));
				writer.WriteValue(arrayProperty.StructPrototype.StructGuid.ToString("D"));

				writer.WriteEndObject();
			}
            else
            {
                writer.WriteNull();
            }

			writer.WritePropertyName("Items");
			if (arrayProperty.Value is IEnumerable<UProperty> uprops)
			{
				PropertiesSerializer.ToJson(uprops, writer);
			}
			else
			{
				writer.WriteStartArray();

				if (arrayProperty.Value is not null)
				{
					foreach (object? item in arrayProperty.Value)
					{
						if (item is FString fs)
						{
							writer.WriteFStringValue(fs);
						}
						else
						{
							writer.WriteValue(item);
						}
					}
				}

				writer.WriteEndArray();
			}

			writer.WriteEndObject();
		}

		public void FromJson(UProperty property, JsonReader reader)
		{
			ArrayProperty arrayProperty = (ArrayProperty)property;

			JToken? valueToken = null;

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
						case nameof(ArrayProperty.ItemType):
							arrayProperty.ItemType = reader.ReadAsFString();
							break;
						case nameof(ArrayProperty.StructPrototype):
							reader.ReadAndMoveToContent();
							if (reader.TokenType == JsonToken.Null)
							{
								arrayProperty.StructPrototype = null;
							}
							else
							{
								FString? name = null, type = null, structType = null;
								Guid structGuid = Guid.Empty;
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
											case nameof(StructProperty.Name):
												name = reader.ReadAsFString();
												break;
											case nameof(StructProperty.Type):
												type = reader.ReadAsFString();
												break;
											case nameof(StructProperty.StructType):
												structType = reader.ReadAsFString();
												break;
											case nameof(StructProperty.StructGuid):
												structGuid = Guid.Parse(reader.ReadAsString()!);
												break;
										}
									}
								}

								if (name is null && type is null && structType is null)
								{
									// Probably an empty object, treat it the same as null
									arrayProperty.StructPrototype = null;
									break;
								}

								if (name is null || type is null || structType is null)
								{
									throw new InvalidDataException("Array property struct prototype is missing information");
								}

								arrayProperty.StructPrototype = new(name, type)
								{
									StructType = structType,
									StructGuid = structGuid
								};
							}
							break;
						case "Items":
							if (reader.ReadAndMoveToContent())
							{
								valueToken = JToken.ReadFrom(reader);
							}
							break;
					}
				}
			}

			if (arrayProperty.ItemType is null)
			{
				throw new InvalidDataException("Array property is missing item type");
			}

			if (valueToken is not null)
			{
				if (valueToken is not JContainer arrayToken)
				{
					throw new InvalidDataException("Array property value is not an array");
				}

				JsonReader valueReader = valueToken.CreateReader();

				Type propType = UProperty.ResolveType(arrayProperty.ItemType);
				UProperty prototype = ((UProperty?)Activator.CreateInstance(propType, FString.Empty, arrayProperty.ItemType)) ?? throw new InvalidDataException($"Invalid array item type {arrayProperty.ItemType}");

				if (prototype.IsSimpleProperty)
				{
					if (prototype.SimpleValueType == typeof(FString))
					{
						string[]? value = arrayToken.ToObject<string[]>();
						if (value is not null)
						{
							arrayProperty.Value = value.Select(v => new FString(v)).ToArray();
						}
					}
					else
					{
						arrayProperty.Value = (Array?)arrayToken.ToObject(prototype.SimpleValueType.MakeArrayType());
					}
				}
				else
				{
					arrayProperty.Value = PropertiesSerializer.FromJson(valueReader)?.ToArray();
				}
			}
		}
	}
}
