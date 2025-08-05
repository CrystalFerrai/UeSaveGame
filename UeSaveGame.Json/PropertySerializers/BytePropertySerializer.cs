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
	internal class BytePropertySerializer : IPropertySerializer
	{
		public void ToJson(FProperty property, JsonWriter writer)
		{
			ByteProperty byteProperty = (ByteProperty)property;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(ByteProperty.EnumType));
			PropertyTypeNameSerializer.Write(byteProperty.EnumType, writer);

			writer.WritePropertyName(nameof(ByteProperty.Value));
			if (byteProperty.Value is byte b)
			{
				writer.WriteValue((int)b);
			}
			else if (byteProperty.Value is FString s)
			{
				writer.WriteValue(s.Value);
			}

			writer.WriteEndObject();
		}

		public void FromJson(FProperty property, JsonReader reader)
		{
			ByteProperty byteProperty = (ByteProperty)property;

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
						case nameof(ByteProperty.EnumType):
							byteProperty.EnumType = PropertyTypeNameSerializer.Read(reader);
							break;
						case nameof(ByteProperty.Value):
							reader.ReadAndMoveToContent();
							switch (reader.TokenType)
							{
								case JsonToken.Float:
								case JsonToken.Integer:
									byteProperty.Value = (byte)reader.ValueAsInteger();
									break;
								case JsonToken.String:
									byteProperty.Value = reader.ValueAsFString();
									break;
								default:
									byteProperty.Value = null;
									break;
							}
							break;
					}
				}
			}
		}
	}
}
