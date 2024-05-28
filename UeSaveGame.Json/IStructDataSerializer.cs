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
    /// Json serializer for the data of a specialized StructProperty
    /// </summary>
	/// <remarks>
	/// Any class that implements this interface will be automatically discoverd by reflection and instantiated.
	/// Classes implementing this interface must have a default constructor.
	/// </remarks>
	public interface IStructDataSerializer
    {
        /// <summary>
        /// Names of struct types this serializer should handle
        /// </summary>
        IEnumerable<string> StructTypes { get; }

		/// <summary>
		/// Names of properties this serializer should handle.
		/// </summary>
		/// <remarks>
		/// Only needed for cases where type name is not saved for a custom struct type (due to being in a map or something)
		/// </remarks>
		ISet<string>? KnownPropertyNames { get; }

		/// <summary>
		/// Serialize struct data to json
		/// </summary>
		/// <param name="data">The data to serialize</param>
		/// <param name="writer">Where to write the serialized data</param>
		void ToJson(IStructData? data, JsonWriter writer);

		/// <summary>
		/// Deserialize struct data from json
		/// </summary>
		/// <param name="reader">The reader containing the serialized data</param>
		/// <returns>The deserialized data</returns>
		IStructData? FromJson(JsonReader reader);
    }
}
