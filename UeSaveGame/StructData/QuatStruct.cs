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
	public class QuatStruct : BaseStructData
	{
		public override IEnumerable<string> StructTypes
		{
			get
			{
				yield return "Quat";
			}
		}

		public FQuat Value { get; set; }

		public QuatStruct()
		{
		}

		public override void Deserialize(BinaryReader reader, int size, PackageVersion packageVersion)
		{
			FQuat q;

			if (packageVersion >= EObjectUE5Version.LARGE_WORLD_COORDINATES)
			{
				q.X = reader.ReadDouble();
				q.Y = reader.ReadDouble();
				q.Z = reader.ReadDouble();
				q.W = reader.ReadDouble();
			}
			else
			{
				q.X = reader.ReadSingle();
				q.Y = reader.ReadSingle();
				q.Z = reader.ReadSingle();
				q.W = reader.ReadSingle();
			}

			Value = q;
		}

		public override int Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (packageVersion >= EObjectUE5Version.LARGE_WORLD_COORDINATES)
			{
				writer.Write(Value.X);
				writer.Write(Value.Y);
				writer.Write(Value.Z);
				writer.Write(Value.W);

				return 32;
			}

			writer.Write((float)Value.X);
			writer.Write((float)Value.Y);
			writer.Write((float)Value.Z);
			writer.Write((float)Value.W);

			return 16;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
