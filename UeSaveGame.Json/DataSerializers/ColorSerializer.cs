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
	internal static class ColorSerializer
	{
		public static void ToJson(FColor value, JsonWriter writer)
		{
			writer.WriteValue(value.ToString());
		}

		public static FColor FromJson(JsonReader reader)
		{
			string? s = reader.Value as string;
			if (!FColor.TryParse(s, out FColor value))
			{
				throw new InvalidDataException($"Failed to parse color from string: {s}");
			}
			return value;
		}
	}
}
