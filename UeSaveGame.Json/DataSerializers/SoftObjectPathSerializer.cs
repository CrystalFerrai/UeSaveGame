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
	internal static class SoftObjectPathSerializer
	{
		public static void ToJson(SoftObjectPath? data, JsonWriter writer)
		{
			if (data is null)
			{
				writer.WriteNull();
				return;
			}

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(SoftObjectPath.PackageName));
			writer.WriteFStringValue(data.PackageName);

			writer.WritePropertyName(nameof(SoftObjectPath.AssetName));
			writer.WriteFStringValue(data.AssetName);

			writer.WritePropertyName(nameof(SoftObjectPath.SubPathString));
			writer.WriteFStringValue(data.SubPathString);

			writer.WriteEndObject();
		}

		public static SoftObjectPath? FromJson(JsonReader reader)
		{
			SoftObjectPath data = new();

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
						case nameof(SoftObjectPath.PackageName):
							data.PackageName = reader.ReadAsFString();
							break;
						case nameof(SoftObjectPath.AssetName):
							data.AssetName = reader.ReadAsFString();
							break;
						case nameof(SoftObjectPath.SubPathString):
							data.SubPathString = reader.ReadAsFString();
							break;
					}
				}
			}

			if (data.PackageName is null && data.AssetName is null && data.SubPathString is null)
			{
				return null;
			}

			return data;
		}
	}
}
