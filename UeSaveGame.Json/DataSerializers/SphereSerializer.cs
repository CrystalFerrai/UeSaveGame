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
	internal static class SphereSerializer
	{
		public static void ToJson(FSphere value, JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(FSphere.Center));
			VectorSerializer.ToJson(value.Center, writer);

			writer.WritePropertyName(nameof(FSphere.Radius));
			writer.WriteValue(value.Radius);

			writer.WriteEndObject();
		}

		public static FSphere FromJson(JsonReader reader)
		{
			FSphere value = new();

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
						case nameof(FSphere.Center):
							value.Center = VectorSerializer.FromJson(reader);
							break;
						case nameof(FSphere.Radius):
							value.Radius = ReadComponent(reader);
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
