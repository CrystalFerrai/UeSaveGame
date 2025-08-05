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
	public class VectorStruct : BaseStructData
	{
		public override IEnumerable<string> StructTypes
		{
			get
			{
				yield return "Vector";
				yield return "Rotator";
			}
		}

		public FVector Value { get; set; }

		public VectorStruct()
		{
		}

		public override void Deserialize(BinaryReader reader, int size, PackageVersion packageVersion)
		{
			FVector v;

			if (packageVersion >= EObjectUE5Version.LARGE_WORLD_COORDINATES)
			{
				v.X = reader.ReadDouble();
				v.Y = reader.ReadDouble();
				v.Z = reader.ReadDouble();
			}
			else
			{
				v.X = reader.ReadSingle();
				v.Y = reader.ReadSingle();
				v.Z = reader.ReadSingle();
			}

			Value = v;
		}

		public override int Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (packageVersion >= EObjectUE5Version.LARGE_WORLD_COORDINATES)
			{
				writer.Write(Value.X);
				writer.Write(Value.Y);
				writer.Write(Value.Z);

				return 24;
			}

			writer.Write((float)Value.X);
			writer.Write((float)Value.Y);
			writer.Write((float)Value.Z);

			return 12;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
