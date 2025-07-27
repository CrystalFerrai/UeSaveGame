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

namespace UeSaveGame
{
    /// <summary>
    /// Interface for data types usable in UE structs
    /// </summary>
	public interface IStructData
    {
        IEnumerable<string> StructTypes { get; }

        // Only needed for cases where type name is not saved for a custom struct type (due to being in a map or something)
        ISet<string>? KnownPropertyNames { get; }

		void Deserialize(BinaryReader reader, long size, PackageVersion packageVersion);

		long Serialize(BinaryWriter writer, PackageVersion packageVersion);
	}
}
