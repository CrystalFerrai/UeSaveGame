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

namespace UeSaveGame.DataTypes
{
	public class Sphere
    {
        public Vector Center;
        public float Radius;

        public static Sphere Deserialize(BinaryReader reader)
        {
            Sphere m = new Sphere();

            m.Center.X = reader.ReadSingle();
            m.Center.Y = reader.ReadSingle();
            m.Center.Z = reader.ReadSingle();
            m.Radius = reader.ReadSingle();

            return m;
        }

        public long Serialize(BinaryWriter writer)
        {
            writer.Write(Center.X);
            writer.Write(Center.Y);
            writer.Write(Center.Z);
            writer.Write(Radius);

            return 16;
        }

        public override string ToString()
        {
            return $"Center = {Center}, Radius = {Radius}";
        }
    }
}
