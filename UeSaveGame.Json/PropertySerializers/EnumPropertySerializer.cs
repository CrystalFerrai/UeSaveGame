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
	internal class EnumPropertySerializer : IPropertySerializer
	{
		public void ToJson(FProperty property, JsonWriter writer)
		{
			EnumProperty enumProperty = (EnumProperty)property;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(EnumProperty.EnumType));
			PropertyTypeNameSerializer.Write(enumProperty.EnumType, writer);

			writer.WritePropertyName(nameof(EnumProperty.Value));
			writer.WriteValue(enumProperty.Value?.Value);

			writer.WriteEndObject();
		}

		public void FromJson(FProperty property, JsonReader reader)
		{
			EnumProperty enumProperty = (EnumProperty)property;

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
						case nameof(EnumProperty.EnumType):
							enumProperty.EnumType = PropertyTypeNameSerializer.Read(reader);
							break;
						case nameof(EnumProperty.Value):
							enumProperty.Value = reader.ReadAsFString();
							break;
					}
				}
			}
		}
	}
}
