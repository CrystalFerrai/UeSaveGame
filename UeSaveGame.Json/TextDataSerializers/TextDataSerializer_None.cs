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
using Newtonsoft.Json.Linq;
using UeSaveGame.TextData;

namespace UeSaveGame.Json.TextDataSerializers
{
	internal class TextDataSerializer_None : ITextDataSerializer
	{
		public void ToJson(ITextData? data, JsonWriter writer)
		{
			if (data is null)
			{
				writer.WriteNull();
				return;
			}

			if (data is not TextData_None) throw new ArgumentException($"{nameof(TextDataSerializer_None)} does not support data type {data.GetType().Name}", nameof(data));

			TextData_None textData = (TextData_None)data;
			writer.WriteFStringValue(textData.Value);
		}

		public ITextData? FromJson(JsonReader reader)
		{
			TextData_None textData = new();
			if (reader.Value is string sv)
			{
				textData.Value = new(sv);
			}
			else
			{
				textData.Value = reader.ReadAsFString();
			}
			return textData;
		}
	}
}
