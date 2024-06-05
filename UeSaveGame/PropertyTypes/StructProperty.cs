// Copyright 2022 Crystal Ferrai
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

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UeSaveGame.StructData;
using UeSaveGame.Util;

namespace UeSaveGame.PropertyTypes
{
	public class StructProperty : UProperty<IStructData>
    {
        private static readonly Dictionary<string, Type> sTypeMap;
        private static readonly Dictionary<string, Type> sNameMap;

        public FString? StructType { get; set; }

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
			: this(name, new(nameof(StructProperty)))
		{
		}

		public StructProperty(FString name, FString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader, PackageVersion packageVersion)
        {
            if (includeHeader)
            {
                StructType = reader.ReadUnrealString();
                byte[] guidBytes = reader.ReadBytes(16);
                StructGuid = new Guid(guidBytes);
                reader.ReadByte(); // terminator
            }

            if (Name == "FGuid")
            {
                StructType = new FString("Guid");
                IStructData instance;
                Type? type = typeof(GuidStruct);
                instance = (IStructData?)Activator.CreateInstance(type);
                instance.Deserialize(reader, 16, packageVersion);
                Value = instance;
                return;
            }

            if (size > 0 || StructType == null && !includeHeader)
            {
                IStructData instance;
                Type? type;
                if (StructType != null && sTypeMap.TryGetValue(StructType!, out type) ||
                    StructType == null && Name != null && sNameMap.TryGetValue(Name!, out type))
                {
                    instance = (IStructData?)Activator.CreateInstance(type) ?? throw new MissingMethodException($"Could not construct an instance of struct data type {type.FullName}.");
                }
                else
                {
                    instance = new PropertiesStruct();
                }
                instance.Deserialize(reader, size, packageVersion);
                Value = instance;
            }
            else
            {
                Value = null;
            }
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader, PackageVersion packageVersion)
        {
            if (includeHeader)
            {
                writer.WriteUnrealString(StructType);
                writer.Write(StructGuid.ToByteArray());
                writer.Write((byte)0);
            }

            if (Value != null)
            {
                return Value.Serialize(writer, packageVersion);
            }
            return 0;
        }

        public override string ToString()
        {
            return Value == null ? base.ToString() : $"{Name} [{nameof(StructProperty)} - {StructType??"no type"}] {Value?.ToString() ?? "Null"}";
        }

		#region Struct data searching

		private static void CurrentDomain_AssemblyLoad(object? sender, AssemblyLoadEventArgs args)
		{
            AddStructDataFromAssembly(args.LoadedAssembly);
		}

		private static void AddStructDataFromAssembly(Assembly assembly)
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

			IEnumerable<Type> types = assembly.GetTypes().Where(t => !t.IsAbstract && t.GetInterfaces().Contains(typeof(IStructData)));
			foreach (Type type in types)
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
