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

using System.Numerics;
using UeSaveGame.DataTypes;

namespace UeSaveGame.StructData
{
	public class Vector2DStruct : BaseStructData
	{
		public override IEnumerable<string> StructTypes
		{
			get
			{
				yield return "Vector2D";
			}
		}

		public FVector2D Value { get; set; }

		public Vector2DStruct()
		{
		}

		public override void Deserialize(BinaryReader reader, int size, PackageVersion packageVersion)
		{
			FVector2D v;

			if (packageVersion >= EObjectUE5Version.LARGE_WORLD_COORDINATES)
			{
				v.X = reader.ReadDouble();
				v.Y = reader.ReadDouble();
			}
			else
			{
				v.X = reader.ReadSingle();
				v.Y = reader.ReadSingle();
			}

			Value = v;
		}

		public override int Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (packageVersion >= EObjectUE5Version.LARGE_WORLD_COORDINATES)
			{
				writer.Write(Value.X);
				writer.Write(Value.Y);

				return 16;
			}

			writer.Write((float)Value.X);
			writer.Write((float)Value.Y);

			return 8;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
