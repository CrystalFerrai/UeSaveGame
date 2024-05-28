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
	internal static class TransformSerializer
	{
		public static void ToJson(FTransform value, JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(FTransform.Rotation));
			QuaternionSerializer.ToJson(value.Rotation, writer);

			writer.WritePropertyName(nameof(FTransform.Translation));
			VectorSerializer.ToJson(value.Translation, writer);

			writer.WritePropertyName(nameof(FTransform.Scale3D));
			VectorSerializer.ToJson(value.Scale3D, writer);

			writer.WriteEndObject();
		}

		public static FTransform FromJson(JsonReader reader)
		{
			FTransform value = new();

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
						case nameof(FTransform.Rotation):
							value.Rotation = QuaternionSerializer.FromJson(reader);
							break;
						case nameof(FTransform.Translation):
							value.Translation = VectorSerializer.FromJson(reader);
							break;
						case nameof(FTransform.Scale3D):
							value.Scale3D = VectorSerializer.FromJson(reader);
							break;
					}
				}
			}

			return value;
		}
	}
}
