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
using UeSaveGame.Json.PropertySerializers;
using UeSaveGame.PropertyTypes;

namespace UeSaveGame.Json
{
	/// <summary>
	/// Helper for serializing properties as json
	/// </summary>
	public static class PropertiesSerializer
	{
		// Map of type names to propery serializer types
		private static readonly Dictionary<string, IPropertySerializer> sSerializerMap;

		static PropertiesSerializer()
		{
			// Add to this map if a new property type is added to UeSaveGame
			sSerializerMap = new Dictionary<string, IPropertySerializer>()
			{
                // Commented types have no current implementation in UeSaveGame
				{ nameof(ArrayProperty), new ArrayPropertySerializer() },
				{ nameof(BoolProperty), new BoolPropertySerializer() },
				{ nameof(ByteProperty), new BytePropertySerializer() },
				//{ nameof(DelegateProperty), new DelegatePropertySerializer() },
				{ nameof(DoubleProperty), new DoublePropertySerializer() },
				{ nameof(EnumProperty), new EnumPropertySerializer() },
				{ nameof(FloatProperty), new FloatPropertySerializer() },
				//{ nameof(Int16Property), new Int16PropertySerializer() },
				//{ nameof(Int32Property), new Int32PropertySerializer() },
				{ nameof(Int64Property), new Int64PropertySerializer() },
				//{ nameof(Int8Property), new Int8PropertySerializer() },
				{ nameof(IntProperty), new IntPropertySerializer() },
				//{ nameof(InterfaceProperty), new InterfacePropertySerializer() },
				//{ nameof(LazyObjectProperty), new LazyObjectPropertySerializer() },
				{ nameof(MapProperty), new MapPropertySerializer() },
				{ nameof(MulticastDelegateProperty), new MulticastDelegatePropertySerializer() },
				{ nameof(MulticastInlineDelegateProperty), new MulticastInlineDelegatePropertySerializer() },
				{ nameof(NameProperty), new NamePropertySerializer() },
				{ nameof(ObjectProperty), new ObjectPropertySerializer() },
				//{ nameof(RotatorProperty), new RotatorPropertySerializer() },
				{ nameof(SetProperty), new SetPropertySerializer() },
				{ nameof(SoftObjectProperty), new SoftObjectPropertySerializer() },
				{ nameof(StrProperty), new StrPropertySerializer() },
				{ nameof(StructProperty), new StructPropertySerializer() },
				{ nameof(TextProperty), new TextPropertySerializer() },
				//{ nameof(UInt16Property), new UInt16PropertySerializer() },
				{ nameof(UInt32Property), new UInt32PropertySerializer() },
				{ nameof(UInt64Property), new UInt64PropertySerializer() },
				//{ nameof(VectorProperty), new VectorPropertySerializer() }
			};
		}

		/// <summary>
		/// Serialize a list of properties to json
		/// </summary>
		/// <param name="data">The properties to serialize</param>
		/// <param name="writer">Where to write the serialized data</param>
		public static void ToJson(IEnumerable<FPropertyTag>? data, JsonWriter writer)
		{
			writer.WriteStartArray();

			if (data is not null)
			{
				foreach (FPropertyTag property in data)
				{
					WriteProperty(property, writer);
				}
			}

			writer.WriteEndArray();
		}

		/// <summary>
		/// Deserialize a list of properties from json
		/// </summary>
		/// <param name="reader">The reader containing the serialized data</param>
		/// <returns>The deserialized properties</returns>
		public static IList<FPropertyTag> FromJson(JsonReader reader)
		{
			List<FPropertyTag> data = new();

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.EndArray)
				{
					break;
				}

				if (reader.TokenType == JsonToken.StartObject)
				{
					FPropertyTag? property = ReadProperty(reader);

					if (property is not null)
					{
						data.Add(property);
					}
				}
			}
			return data;
		}

		/// <summary>
		/// Serialize a property to json
		/// </summary>
		/// <param name="property">The property to serialize</param>
		/// <param name="writer">Where to write the serialized data</param>
		public static void WriteProperty(FPropertyTag property, JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(FPropertyTag.Name));
			writer.WriteValue(property.Name);

			writer.WritePropertyName(nameof(FPropertyTag.Type));
			PropertyTypeNameSerializer.Write(property.Type, writer);

			writer.WritePropertyName(nameof(FPropertyTag.Flags));
			writer.WriteValue((byte)property.Flags);

			writer.WritePropertyName(nameof(FProperty.Value));
			IPropertySerializer serializer = GetSerializer(property.Type.Name);
			serializer.ToJson(property.Property!, writer);

			writer.WriteEndObject();
		}

		/// <summary>
		/// Deserialize a property from json
		/// </summary>
		/// <param name="reader">The reader containing the serialized data</param>
		/// <returns>The deserialized property</returns>
		public static FPropertyTag? ReadProperty(JsonReader reader)
		{
			FString? propertyName = null;
			FPropertyTypeName? propertyType = null;
			EPropertyTagFlags propertyFlags = EPropertyTagFlags.None;
			JToken? propertyValue = null;

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
						case nameof(FPropertyTag.Name):
							propertyName = reader.ReadAsFString();
							break;
						case nameof(FPropertyTag.Type):
							propertyType = PropertyTypeNameSerializer.Read(reader);
							break;
						case nameof(FPropertyTag.Flags):
							propertyFlags = (EPropertyTagFlags)reader.ReadAsInt32()!.Value;
							break;
						case nameof(FProperty.Value):
							if (reader.ReadAndMoveToContent())
							{
								propertyValue = JToken.ReadFrom(reader);
							}
							break;
					}
				}
			}

			if (propertyName is null || propertyType is null)
			{
				if (System.Diagnostics.Debugger.IsAttached)
				{
					// Seems to be an incomplete property. Investigate if you get here.
					System.Diagnostics.Debugger.Break();
				}
				return null;
			}

			FPropertyTag property = new(propertyName, propertyType, 0, 0, FProperty.Create(propertyName, propertyType), propertyFlags);

			if (propertyValue is not null)
			{
				JsonReader valueReader = propertyValue.CreateReader();
				if (valueReader.Read())
				{
					IPropertySerializer serializer = GetSerializer(property.Type.Name);
					serializer.FromJson(property.Property!, valueReader);
				}
			}

			return property;
		}

		internal static IPropertySerializer GetSerializer(string typeName)
		{
			IPropertySerializer? serializer;
			if (!sSerializerMap.TryGetValue(typeName, out serializer))
			{
				throw new NotImplementedException($"Serializer for property type {typeName} has not been implemented.");
			}
			return serializer;
		}
	}
}
