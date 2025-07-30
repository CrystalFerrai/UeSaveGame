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

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace UeSaveGame.Util
{
	/// <summary>
	/// Utility to search for types in assemblies
	/// </summary>
	internal static class TypeSearcher
	{
		/// <summary>
		/// Find all concrete types which derive from the passed in base type or interface
		/// </summary>
		/// <param name="baseType">The base type or interface</param>
		/// <param name="assembly">the assembly to search</param>
		public static IEnumerable<Type> FindDerivedTypes(Type baseType, Assembly assembly)
		{
			// Skip digging through assemblies which do not reference this assembly
			{
				AssemblyNameEqualityComparer assemblyComparer = new();

				AssemblyName assemblyName = assembly.GetName();
				AssemblyName thisAssemblyName = Assembly.GetExecutingAssembly().GetName();

				if (!assemblyComparer.Equals(assemblyName, thisAssemblyName) &&
					!assembly.GetReferencedAssemblies().Contains(thisAssemblyName, assemblyComparer))
				{
					yield break;
				}
			}

			if (baseType.IsInterface)
			{
				foreach (Type type in assembly.GetTypes().Where(t => !t.IsAbstract && t.GetInterfaces().Contains(typeof(IStructData))))
				{
					yield return type;
				}
			}
			else
			{
				foreach (Type type in assembly.GetTypes().Where(t => !t.IsAbstract))
				{
					for (Type? current = type; current is not null; current = current.BaseType)
					{
						if (current == baseType)
						{
							yield return type;
							break;
						}
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
	}
}
