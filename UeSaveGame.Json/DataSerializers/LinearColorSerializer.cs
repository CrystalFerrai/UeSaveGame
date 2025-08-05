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
	internal static class LinearColorSerializer
	{
		public static void ToJson(FLinearColor value, JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(FLinearColor.R));
			writer.WriteValue(value.R);

			writer.WritePropertyName(nameof(FLinearColor.G));
			writer.WriteValue(value.G);

			writer.WritePropertyName(nameof(FLinearColor.B));
			writer.WriteValue(value.B);

			writer.WritePropertyName(nameof(FLinearColor.A));
			writer.WriteValue(value.A);

			writer.WriteEndObject();
		}

		public static FLinearColor FromJson(JsonReader reader)
		{
			FLinearColor value = new();

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
						case nameof(FLinearColor.R):
							value.R = ReadComponent(reader);
							break;
						case nameof(FLinearColor.G):
							value.G = ReadComponent(reader);
							break;
						case nameof(FLinearColor.B):
							value.B = ReadComponent(reader);
							break;
						case nameof(FLinearColor.A):
							value.A = ReadComponent(reader);
							break;
					}
				}
			}

			return value;
		}

		private static float ReadComponent(JsonReader reader)
		{
			double? value = reader.ReadAsDouble();
			if (!value.HasValue) throw new InvalidDataException("Failed to read LinearColor value.");
			return (float)value.Value;
		}
	}
}
