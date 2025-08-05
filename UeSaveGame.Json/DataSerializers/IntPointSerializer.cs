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
using UeSaveGame.DataTypes;

namespace UeSaveGame.Json.DataSerializers
{
	internal static class IntPointSerializer
	{
		public static void ToJson(FIntPoint value, JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(FIntPoint.X));
			writer.WriteValue(value.X);

			writer.WritePropertyName(nameof(FIntPoint.Y));
			writer.WriteValue(value.Y);

			writer.WriteEndObject();
		}

		public static FIntPoint FromJson(JsonReader reader)
		{
			FIntPoint value = new();

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
						case nameof(FIntPoint.X):
							value.X = ReadComponent(reader);
							break;
						case nameof(FIntPoint.Y):
							value.Y = ReadComponent(reader);
							break;
					}
				}
			}

			return value;
		}

		private static int ReadComponent(JsonReader reader)
		{
			int? value = reader.ReadAsInt32();
			if (!value.HasValue) throw new InvalidDataException("Failed to read IntPoint value.");
			return value.Value;
		}
	}
}
