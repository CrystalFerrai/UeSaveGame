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
	internal static class QuaternionSerializer
	{
		public static void ToJson(FQuat value, JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(FQuat.X));
			writer.WriteValue(value.X);

			writer.WritePropertyName(nameof(FQuat.Y));
			writer.WriteValue(value.Y);

			writer.WritePropertyName(nameof(FQuat.Z));
			writer.WriteValue(value.Z);

			writer.WritePropertyName(nameof(FQuat.W));
			writer.WriteValue(value.W);

			writer.WriteEndObject();
		}

		public static FQuat FromJson(JsonReader reader)
		{
			FQuat value = new();

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
						case nameof(FQuat.X):
							value.X = ReadComponent(reader);
							break;
						case nameof(FQuat.Y):
							value.Y = ReadComponent(reader);
							break;
						case nameof(FQuat.Z):
							value.Z = ReadComponent(reader);
							break;
						case nameof(FQuat.W):
							value.W = ReadComponent(reader);
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
