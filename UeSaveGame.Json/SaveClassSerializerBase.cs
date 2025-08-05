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
	/// Implement this if you need custom json serialization for a custom save class.
	/// </summary>
	public abstract class SaveClassSerializerBase<SaveClass> : ISaveClassSerializer where SaveClass : SaveClassBase
	{
		/// <summary>
		/// If true, will call DeserializeHeader or SerializeHeader before performing data serialization
		/// </summary>
		public virtual bool HasCustomHeader { get; } = false;

		/// <summary>
		/// If true, will call DeserializeData and SerializeData instead of performing standard serialization
		/// </summary>
		public virtual bool HasCustomData { get; } = false;

		/// <summary>
		/// Called before deserializing data, only if HasCustomHeader = true
		/// </summary>
		/// <param name="reader">A reader positioned at the start of the header</param>
		public virtual void HeaderFromJson(JsonReader reader, SaveClass saveClass)
		{
		}

		/// <summary>
		/// Called before serializing data, only if HasCustomHeader = true
		/// </summary>
		/// <param name="writer">A writer positioned at the start of the header</param>
		public virtual void HeaderToJson(JsonWriter writer, SaveClass saveClass)
		{
		}

		/// <summary>
		/// Called instead of normal data deserialization, only if HasCustomData = true
		/// </summary>
		/// <param name="reader">A reader positioned at the start of the data</param>
		public virtual void DataFromJson(JsonReader reader, SaveClass saveClass)
		{
		}

		/// <summary>
		/// Called instead of normal data serialization, only if HasCustomData = true
		/// </summary>
		/// <param name="writer">A writer positioned at the start of the data</param>
		public virtual void DataToJson(JsonWriter writer, SaveClass saveClass)
		{
		}

		void ISaveClassSerializer.HeaderFromJson(JsonReader reader, SaveClassBase saveClass)
		{
			HeaderFromJson(reader, (SaveClass)saveClass);
		}

		void ISaveClassSerializer.HeaderToJson(JsonWriter writer, SaveClassBase saveClass)
		{
			HeaderToJson(writer, (SaveClass)saveClass);
		}

		void ISaveClassSerializer.DataFromJson(JsonReader reader, SaveClassBase saveClass)
		{
			DataFromJson(reader, (SaveClass)saveClass);
		}

		void ISaveClassSerializer.DataToJson(JsonWriter writer, SaveClassBase saveClass)
		{
			DataToJson(writer, (SaveClass)saveClass);
		}
	}

	internal interface ISaveClassSerializer
	{
		bool HasCustomHeader { get; }

		bool HasCustomData { get; }

		void HeaderFromJson(JsonReader reader, SaveClassBase saveClass);

		public void HeaderToJson(JsonWriter writer, SaveClassBase saveClass);

		public void DataFromJson(JsonReader reader, SaveClassBase saveClass);

		public void DataToJson(JsonWriter writer, SaveClassBase saveClass);
	}
}
