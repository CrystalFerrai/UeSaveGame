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

namespace UeSaveGame.Json
{
	/// <summary>
	/// Json serializer for a property value
	/// </summary>
	internal interface IPropertySerializer
	{
		/// <summary>
		/// Serialize the value of a property to json
		/// </summary>
		/// <param name="property">The property with the value to serialize</param>
		/// <param name="writer">Where to write the serialized data</param>
		void ToJson(UProperty property, JsonWriter writer);

		/// <summary>
		/// Deserialize a property value from json
		/// </summary>
		/// <param name="property">The property to receive the value</param>
		/// <param name="reader">The reader containing the serialized value</param>
		void FromJson(UProperty property, JsonReader reader);
	}
}
