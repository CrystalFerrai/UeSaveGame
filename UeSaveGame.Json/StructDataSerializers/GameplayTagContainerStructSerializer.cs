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
using UeSaveGame.StructData;

namespace UeSaveGame.Json.StructDataSerializers
{
	internal class GameplayTagContainerStructSerializer : StructDataSerializerBase
	{
		public override IEnumerable<string> StructTypes
		{
			get
			{
				yield return "GameplayTagContainer";
			}
		}

		public override void ToJson(IStructData? data, JsonWriter writer)
		{
			if (data is null)
			{
				writer.WriteNull();
				return;
			}

			GameplayTagContainerStruct gameplayTagContainerStruct = (GameplayTagContainerStruct)data;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(GameplayTagContainerStruct.Tags));

			writer.WriteStartArray();

			foreach (FString? tag in gameplayTagContainerStruct.Tags)
			{
				writer.WriteFStringValue(tag);
			}

			writer.WriteEndArray();

			writer.WriteEndObject();
		}

		public override IStructData? FromJson(JsonReader reader)
		{
			GameplayTagContainerStruct gameplayTagContainerStruct = new();

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
						case nameof(GameplayTagContainerStruct.Tags):
							while (reader.Read())
							{
								if (reader.TokenType == JsonToken.EndArray)
								{
									break;
								}

								if (reader.TokenType == JsonToken.String)
								{
									gameplayTagContainerStruct.Tags.Add(reader.ReadAsFString());
								}
							}
							break;
					}
				}
			}

			return gameplayTagContainerStruct;
		}
	}
}
