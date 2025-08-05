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

using UeSaveGame.Util;

namespace UeSaveGame
{
	public class FPropertyTypeName
	{
		public FString Name { get; }

		public List<FPropertyTypeName> Parameters { get; }

		public FPropertyTypeName(FString name, IEnumerable<FPropertyTypeName>? nodes = null)
		{
			Name = name;
			if (nodes is null)
			{
				Parameters = new();
			}
			else
			{
				Parameters = new(nodes);
			}
		}

		internal static FPropertyTypeName Deserialize(BinaryReader reader, PackageVersion packageVersion)
		{
			FString name = reader.ReadUnrealString() ?? throw new InvalidDataException("Missing expected property type name");
			if (packageVersion >= EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				int innerCount = reader.ReadInt32();
				List<FPropertyTypeName> nodes = new(innerCount);
				for (int i = 0; i < innerCount; ++i)
				{
					nodes.Add(Deserialize(reader, packageVersion));
				}

				return new(name, nodes);
			}

			return new(name);
		}

		internal void Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			writer.WriteUnrealString(Name);
			if (packageVersion >= EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				writer.Write(Parameters.Count);
				for (int i = 0; i < Parameters.Count; ++i)
				{
					Parameters[i].Serialize(writer, packageVersion);
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
