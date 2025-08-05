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

using UeSaveGame.DataTypes;

namespace UeSaveGame.StructData
{
	internal class SoftObjectPathStruct : BaseStructData
	{
		public SoftObjectPath Value { get; set; }

		public override IEnumerable<string> StructTypes
		{
			get
			{
				yield return "SoftObjectPath";
			}
		}

		public SoftObjectPathStruct()
		{
			Value = new();
		}

		public override void Deserialize(BinaryReader reader, int size, PackageVersion packageVersion)
		{
			Value.Deserialize(reader, packageVersion);
		}

		public override int Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			return Value.Serialize(writer, packageVersion);
		}
	}
}
