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
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UeSaveGame.PropertyTypes;
using UeSaveGame.StructData;

namespace UeSaveGame.Json.PropertySerializers
{
	internal class StructPropertySerializer : IPropertySerializer
	{
		private static readonly Dictionary<string, IStructDataSerializer> sTypeMap;
		private static readonly Dictionary<string, IStructDataSerializer> sNameMap;

		static StructPropertySerializer()
		{
			sTypeMap = new();
			sNameMap = new();

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				AddSerializersFromAssembly(assembly);
			}

			AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
		}

		public void ToJson(UProperty property, JsonWriter writer)
		{
			StructProperty structProperty = (StructProperty)property;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(StructProperty.StructType));
			writer.WriteFStringValue(structProperty.StructType);

			writer.WritePropertyName(nameof(StructProperty.StructGuid));
			writer.WriteValue(structProperty.StructGuid.ToString("D"));

			writer.WritePropertyName(nameof(StructProperty.Value));
			IStructDataSerializer? dataSerializer;
			if (structProperty.StructType is not null &&
				(
				sTypeMap.TryGetValue(structProperty.StructType!, out dataSerializer) ||
				sNameMap.TryGetValue(structProperty.StructType!, out dataSerializer))
				)
			{
				dataSerializer.ToJson(structProperty.Value, writer);
			}
			else
			{
				if (structProperty.Value is not PropertiesStruct propertiesStruct)
				{
					throw new NotImplementedException($"Struct serializer for type {structProperty.StructType} has not been implemented");
				}

				PropertiesSerializer.ToJson(propertiesStruct.Properties, writer);
			}

			writer.WriteEndObject();
		}

		public void FromJson(UProperty property, JsonReader reader)
		{
			StructProperty structProperty = (StructProperty)property;

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
						case nameof(StructProperty.StructType):
							structProperty.StructType = reader.ReadAsFString();
							break;
						case nameof(StructProperty.StructGuid):
							structProperty.StructGuid = Guid.Parse(reader.ReadAsString()!);
							break;
						case nameof(StructProperty.Value):
							if (reader.ReadAndMoveToContent())
							{
								propertyValue = JToken.ReadFrom(reader);
							}
							break;
					}
				}
			}

			if (propertyValue is not null)
			{
				JsonReader valueReader = propertyValue.CreateReader();
				if (valueReader.Read())
				{
					IStructDataSerializer? dataSerializer;
					if (structProperty.StructType is not null &&
						(
						sTypeMap.TryGetValue(structProperty.StructType!, out dataSerializer) ||
						sNameMap.TryGetValue(structProperty.StructType!, out dataSerializer))
						)
					{
						structProperty.Value = dataSerializer.FromJson(valueReader);
					}
					else
					{
						PropertiesStruct propertiesStruct = new();
						propertiesStruct.Properties = PropertiesSerializer.FromJson(valueReader);
						structProperty.Value = propertiesStruct;
					}
				}
			}
		}

		#region Struct serializer searching

		private static void CurrentDomain_AssemblyLoad(object? sender, AssemblyLoadEventArgs args)
		{
			AddSerializersFromAssembly(args.LoadedAssembly);
		}

		private static void AddSerializersFromAssembly(Assembly assembly)
		{
			// Skip digging through assemblies which do not reference this assembly
			{
				AssemblyNameEqualityComparer assemblyComparer = new();

				AssemblyName assemblyName = assembly.GetName();
				AssemblyName thisAssemblyName = Assembly.GetExecutingAssembly().GetName();

				if (!assemblyComparer.Equals(assemblyName, thisAssemblyName) &&
					!assembly.GetReferencedAssemblies().Contains(thisAssemblyName, assemblyComparer))
				{
					return;
				}
			}

			IEnumerable<Type> types = assembly.GetTypes().Where(t => !t.IsAbstract && t.GetInterfaces().Contains(typeof(IStructDataSerializer)));
			foreach (Type type in types)
			{
				IStructDataSerializer instance = (IStructDataSerializer?)Activator.CreateInstance(type) ?? throw new MissingMethodException($"Could not construct an instance of struct data type {type.FullName}.");
				foreach (string structType in instance.StructTypes)
				{
					sTypeMap.Add(structType, instance);
				}
				if (instance.KnownPropertyNames != null)
				{
					foreach (string structType in instance.KnownPropertyNames)
					{
						sNameMap.Add(structType, instance);
					}
				}
			}
		}

		private class AssemblyNameEqualityComparer : IEqualityComparer<AssemblyName>
		{
			public int GetHashCode([DisallowNull] AssemblyName obj)
			{
				return obj.Name?.GetHashCode() ?? 0;
			}

			public bool Equals(AssemblyName? x, AssemblyName? y)
			{
				if (x is null) return y is null;

				string? a = x?.Name;
				string? b = y?.Name;
				if (a is null) return b is null;

				return a.Equals(b);
			}
		}

		#endregion
	}
}
