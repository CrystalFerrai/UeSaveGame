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
using UeSaveGame.PropertyTypes;

namespace UeSaveGame.Json.PropertySerializers
{
	internal class MulticastInlineDelegatePropertySerializer : IPropertySerializer
	{
		public void ToJson(FProperty property, JsonWriter writer)
		{
			MulticastInlineDelegateProperty delegateProperty = (MulticastInlineDelegateProperty)property;

			writer.WriteStartArray();

			if (delegateProperty.Value is not null)
			{
				foreach (UDelegate @delegate in delegateProperty.Value)
				{
					writer.WriteStartObject();

					writer.WritePropertyName(nameof(UDelegate.ClassName));
					writer.WriteFStringValue(@delegate.ClassName);

					writer.WritePropertyName(nameof(UDelegate.FunctionName));
					writer.WriteFStringValue(@delegate.FunctionName);

					writer.WriteEndObject();
				}
			}

			writer.WriteEndArray();
		}

		public void FromJson(FProperty property, JsonReader reader)
		{
			MulticastInlineDelegateProperty delegateProperty = (MulticastInlineDelegateProperty)property;

			List<UDelegate> data = new();

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.EndArray)
				{
					break;
				}

				if (reader.TokenType == JsonToken.StartObject)
				{
					UDelegate @delegate = new();

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
								case nameof(UDelegate.ClassName):
									@delegate.ClassName = reader.ReadAsFString();
									break;
								case nameof(UDelegate.FunctionName):
									@delegate.FunctionName = reader.ReadAsFString();
									break;
							}
						}
					}

					data.Add(@delegate);
				}
			}

			delegateProperty.Value = data.ToArray();
		}
	}
}
