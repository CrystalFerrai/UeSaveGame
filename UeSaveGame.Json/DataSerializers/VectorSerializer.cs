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
using UeSaveGame.DataTypes;

namespace UeSaveGame.Json.DataSerializers
{
	internal static class VectorSerializer
	{
		public static void ToJson(FVector value, JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(FVector.X));
			writer.WriteValue(value.X);

			writer.WritePropertyName(nameof(FVector.Y));
			writer.WriteValue(value.Y);

			writer.WritePropertyName(nameof(FVector.Z));
			writer.WriteValue(value.Z);

			writer.WriteEndObject();
		}

		public static FVector FromJson(JsonReader reader)
		{
			FVector value = new();

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
						case nameof(FVector.X):
							value.X = ReadComponent(reader);
							break;
						case nameof(FVector.Y):
							value.Y = ReadComponent(reader);
							break;
						case nameof(FVector.Z):
							value.Z = ReadComponent(reader);
							break;
					}
				}
			}

			return value;
		}

		private static double ReadComponent(JsonReader reader)
		{
			double? value = reader.ReadAsDouble();
			if (!value.HasValue) throw new InvalidDataException("Failed to read LinearColor value.");
			return value.Value;
		}
	}
}
