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

using System.Reflection;
using UeSaveGame.StructData;
using UeSaveGame.Util;

namespace UeSaveGame.PropertyTypes
{
	public class StructProperty : FProperty<IStructData>
	{
		private static readonly Dictionary<string, Type> sTypeMap;
		private static readonly Dictionary<string, Type> sNameMap;

		public FPropertyTypeName? StructType { get; set; }

		public Guid StructGuid { get; set; }

		static StructProperty()
		{
			sTypeMap = new Dictionary<string, Type>();
			sNameMap = new Dictionary<string, Type>();

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				AddStructDataFromAssembly(assembly);
			}

			AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
		}

		public StructProperty(FString name)
			: base(name)
		{
		}

		protected internal override void ProcessTypeName(FPropertyTypeName typeName, PackageVersion packageVersion)
		{
			if (packageVersion >= EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				switch (typeName.Parameters.Count)
				{
					case 0:
						break;
					case 1:
						StructType = typeName.Parameters[0];
						break;
					case 2:
						StructType = typeName.Parameters[0];
						StructGuid = Guid.Parse(typeName.Parameters[1].Name);
						break;
					default:
						throw new InvalidDataException("Failed to read parameters for StructProperty");
				}
			}
		}

		protected internal override void DeserializeHeader(BinaryReader reader, PackageVersion packageVersion)
		{
			if (packageVersion < EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				StructType = new(reader.ReadUnrealString()!);
				byte[] guidBytes = reader.ReadBytes(16);
				StructGuid = new Guid(guidBytes);
			}
		}

		protected internal override void DeserializeValue(BinaryReader reader, int size, PackageVersion packageVersion)
		{
			IStructData instance;
			Type? type;
			if (StructType != null && sTypeMap.TryGetValue(StructType.Name, out type) ||
				StructType == null && sNameMap.TryGetValue(mPropertyName, out type))
			{
				instance = (IStructData?)Activator.CreateInstance(type) ?? throw new MissingMethodException($"Could not construct an instance of struct data type {type.FullName}.");
			}
			else
			{
				if (!reader.BaseStream.CanSeek || reader.IsUnrealStringAndNotNull())
				{
					instance = new PropertiesStruct();
				}
				else
				{
					throw new NotSupportedException($"Unable to interpret struct data for type '{StructType?.Name ?? "Unknown"}'. If this is a custom struct type, you should implement a custom IStructData to handle serialization.");
				}
			}
			instance.Deserialize(reader, size, packageVersion);
			Value = instance;
		}

		protected internal override void SerializeHeader(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (packageVersion < EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				writer.WriteUnrealString(StructType!.Name);
				writer.Write(StructGuid.ToByteArray());
			}
		}

		protected internal override int SerializeValue(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (Value != null)
			{
				return Value.Serialize(writer, packageVersion);
			}
			return 0;
		}

		public override string? ToString()
		{
			return $"[{StructType!.Name ?? "no type"}] {Value?.ToString() ?? "Null"}";
		}

		#region Struct data searching

		private static void CurrentDomain_AssemblyLoad(object? sender, AssemblyLoadEventArgs args)
		{
			AddStructDataFromAssembly(args.LoadedAssembly);
		}

		private static void AddStructDataFromAssembly(Assembly assembly)
		{
			foreach (Type type in TypeSearcher.FindDerivedTypes(typeof(IStructData), assembly))
			{
				IStructData instance = (IStructData?)Activator.CreateInstance(type) ?? throw new MissingMethodException($"Could not construct an instance of struct data type {type.FullName}.");
				foreach (string structType in instance.StructTypes)
				{
					sTypeMap.Add(structType, type);
				}
				if (instance.KnownPropertyNames != null)
				{
					foreach (string structType in instance.KnownPropertyNames)
					{
						sNameMap.Add(structType, type);
					}
				}
			}
		}

		#endregion
	}
}
