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
	/// Base class for specialized struct data deserializers
	/// </summary>
	public abstract class StructDataSerializerBase : IStructDataSerializer
	{
		public abstract IEnumerable<string> StructTypes { get; }

		public virtual ISet<string>? KnownPropertyNames => null;

		public abstract IStructData? FromJson(JsonReader reader);

		public abstract void ToJson(IStructData? data, JsonWriter writer);
	}
}
