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
using UeSaveGame.PropertyTypes;

namespace UeSaveGame.Json.PropertySerializers
{
	internal class ArrayPropertySerializer : IPropertySerializer
	{
		public void ToJson(FProperty property, JsonWriter writer)
		{
			ArrayProperty arrayProperty = (ArrayProperty)property;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(ArrayProperty.ItemType));
			PropertyTypeNameSerializer.Write(arrayProperty.ItemType!, writer);

			writer.WritePropertyName(nameof(ArrayProperty.StructPrototype));
			if (arrayProperty.ItemType!.Name == nameof(StructProperty) && arrayProperty.StructPrototype is not null)
			{
				StructProperty structProperty = (StructProperty)arrayProperty.StructPrototype.Property!;

				writer.WriteStartObject();

				writer.WritePropertyName(nameof(FPropertyTag.Name));
				writer.WriteFStringValue(arrayProperty.StructPrototype.Name);

				writer.WritePropertyName(nameof(FPropertyTag.Type));
				PropertyTypeNameSerializer.Write(arrayProperty.StructPrototype.Type, writer);

				writer.WritePropertyName(nameof(FPropertyTag.Flags));
				writer.WriteValue((byte)arrayProperty.StructPrototype.Flags);

				writer.WritePropertyName(nameof(StructProperty.StructType));
				PropertyTypeNameSerializer.Write(structProperty.StructType!, writer);

				writer.WritePropertyName(nameof(StructProperty.StructGuid));
				writer.WriteValue(structProperty.StructGuid.ToString("D"));

				writer.WriteEndObject();
			}
			else
			{
				writer.WriteNull();
			}

			writer.WritePropertyName("Items");
			if (arrayProperty.Value is IEnumerable<FProperty> props)
			{
				writer.WriteStartArray();

				IPropertySerializer serializer = PropertiesSerializer.GetSerializer(arrayProperty.ItemType.Name);
				foreach (FProperty item in props)
				{
					serializer.ToJson(item, writer);
				}

				writer.WriteEndArray();
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

		public void FromJson(FProperty property, JsonReader reader)
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
							arrayProperty.ItemType = PropertyTypeNameSerializer.Read(reader);
							break;
						case nameof(ArrayProperty.StructPrototype):
							reader.ReadAndMoveToContent();
							if (reader.TokenType == JsonToken.Null)
							{
								arrayProperty.StructPrototype = null;
							}
							else
							{
								FString? name = null;
								FPropertyTypeName? type = null, structType = null;
								EPropertyTagFlags flags = EPropertyTagFlags.None;
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
											case nameof(FPropertyTag.Name):
												name = reader.ReadAsFString();
												break;
											case nameof(FPropertyTag.Type):
												type = PropertyTypeNameSerializer.Read(reader);
												break;
											case nameof(FPropertyTag.Flags):
												flags = (EPropertyTagFlags)reader.ReadAsInt32()!.Value;
												break;
											case nameof(StructProperty.StructType):
												structType = PropertyTypeNameSerializer.Read(reader);
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

								arrayProperty.StructPrototype = new(name, type, 0, 0,
									new StructProperty(name)
									{
										StructType = structType,
										StructGuid = structGuid
									},
									EPropertyTagFlags.None);
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

				FProperty prototype = FProperty.Create(FString.Empty, arrayProperty.ItemType) ?? throw new InvalidDataException($"Invalid array item type {arrayProperty.ItemType}");

				if (prototype.IsSimpleProperty)
				{
					if (prototype.SimpleValueType == typeof(FString) || prototype is ByteProperty && arrayToken.First is JValue jv && jv.Value is string)
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
					List<FProperty> itemList = new();

					IPropertySerializer serializer = PropertiesSerializer.GetSerializer(arrayProperty.ItemType.Name);

					JsonReader valueReader = valueToken.CreateReader();
					while (valueReader.Read())
					{
						if (valueReader.TokenType == JsonToken.EndArray)
						{
							break;
						}

						if (valueReader.TokenType == JsonToken.StartObject)
						{
							FProperty item = FProperty.Create(FString.Empty, arrayProperty.ItemType);
							serializer.FromJson(item, valueReader);
							itemList.Add(item);
						}
					}

					arrayProperty.Value = itemList.ToArray();
				}
			}
		}
	}
}
