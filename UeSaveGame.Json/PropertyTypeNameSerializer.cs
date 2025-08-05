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

namespace UeSaveGame.Json
{
	internal static class PropertyTypeNameSerializer
	{
		public static void Write(FPropertyTypeName? typeName, JsonWriter writer)
		{
			if (typeName is null)
			{
				writer.WriteNull();
				return;
			}

			writer.WriteStartObject();
			{
				writer.WritePropertyName(nameof(FPropertyTypeName.Name));
				writer.WriteFStringValue(typeName.Name);
				if (typeName.Parameters.Count > 0)
				{
					writer.WritePropertyName(nameof(FPropertyTypeName.Parameters));
					writer.WriteStartArray();
					foreach (FPropertyTypeName parameter in typeName.Parameters)
					{
						Write(parameter, writer);
					}
					writer.WriteEndArray();
				}
			}
			writer.WriteEndObject();
		}

		public static FPropertyTypeName? Read(JsonReader reader)
		{
			if (!reader.ReadAndMoveToContent())
			{
				return null;
			}

			FString? name = null;
			List<FPropertyTypeName> parameters = new();
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
						case nameof(FPropertyTypeName.Name):
							name = reader.ReadAsFString();
							break;
						case nameof(FPropertyTypeName.Parameters):
							if (reader.ReadAndMoveToContent())
							{
								while (reader.Read())
								{
									if (reader.TokenType == JsonToken.EndArray)
									{
										break;
									}

									if (reader.TokenType == JsonToken.StartObject)
									{
										JToken content = JToken.ReadFrom(reader);
										parameters.Add(Read(content.CreateReader())!);
									}
								}
							}
							break;
					}
				}
			}

			if (name is null) throw new InvalidDataException("Type is missing name");

			return new FPropertyTypeName(name, parameters);
		}
	}
}
