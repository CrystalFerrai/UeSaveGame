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

namespace UeSaveGame.Json
{
	/// <summary>
	/// Json Serializer for EngineVersion
	/// </summary>
	public class EngineVersionSerializer
	{
		public void ToJson(EngineVersion data, JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(EngineVersion.Major));
			writer.WriteValue(data.Major);

			writer.WritePropertyName(nameof(EngineVersion.Minor));
			writer.WriteValue(data.Minor);

			writer.WritePropertyName(nameof(EngineVersion.Patch));
			writer.WriteValue(data.Patch);

			writer.WritePropertyName(nameof(EngineVersion.Build));
			writer.WriteValue(data.Build);

			writer.WritePropertyName(nameof(EngineVersion.BuildId));
			writer.WriteValue(data.BuildId);

			writer.WriteEndObject();
		}

		public EngineVersion FromJson(JsonReader reader)
		{
			EngineVersion data = new();

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
						case nameof(EngineVersion.Major):
							data.Major = (short)reader.ReadAsInt32()!.Value;
							break;
						case nameof(EngineVersion.Minor):
							data.Minor = (short)reader.ReadAsInt32()!.Value;
							break;
						case nameof(EngineVersion.Patch):
							data.Patch = (short)reader.ReadAsInt32()!.Value;
							break;
						case nameof(EngineVersion.Build):
							data.Build = reader.ReadAsInt32()!.Value;
							break;
						case nameof(EngineVersion.BuildId):
							data.BuildId = new(reader.ReadAsString()!);
							break;
					}
				}
			}

			return data;
		}
	}
}
