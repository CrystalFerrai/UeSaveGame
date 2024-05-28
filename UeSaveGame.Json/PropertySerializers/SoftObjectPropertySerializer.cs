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
using UeSaveGame.Json.DataSerializers;
using UeSaveGame.PropertyTypes;

namespace UeSaveGame.Json.PropertySerializers
{
	internal class SoftObjectPropertySerializer : IPropertySerializer
	{
		public void ToJson(UProperty property, JsonWriter writer)
		{
			SoftObjectProperty objectProperty = (SoftObjectProperty)property;

			SoftObjectPathSerializer.ToJson(objectProperty.Value, writer);
		}

		public void FromJson(UProperty property, JsonReader reader)
		{
			property.Value = SoftObjectPathSerializer.FromJson(reader);
		}
	}
}
