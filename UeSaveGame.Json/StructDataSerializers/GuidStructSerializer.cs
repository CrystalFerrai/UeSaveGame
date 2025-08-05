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
using UeSaveGame.StructData;

namespace UeSaveGame.Json.StructDataSerializers
{
	internal class GuidStructSerializer : StructDataSerializerBase
	{
		public override IEnumerable<string> StructTypes
		{
			get
			{
				yield return "Guid";
			}
		}

		public override void ToJson(IStructData? data, JsonWriter writer)
		{
			if (data is null)
			{
				writer.WriteNull();
				return;
			}

			GuidStruct guidStruct = (GuidStruct)data;
			writer.WriteValue(guidStruct.Value.ToString("D"));
		}

		public override IStructData? FromJson(JsonReader reader)
		{
			string? s = reader.Value as string;
			Guid value;
			if (s is null)
			{
				value = Guid.Empty;
			}
			else
			{
				value = Guid.Parse(s);
			}

			return new GuidStruct()
			{
				Value = value
			};
		}
	}
}
