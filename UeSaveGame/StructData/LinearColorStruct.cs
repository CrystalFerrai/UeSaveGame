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

namespace UeSaveGame.StructData
{
	public class LinearColorStruct : BaseStructData
    {
        public override IEnumerable<string> StructTypes
        {
            get
            {
                yield return "LinearColor";
            }
        }

        public LinearColor Value { get; set; }

        public LinearColorStruct()
        {
        }

        public override void Deserialize(BinaryReader reader, long size)
        {
            LinearColor c;
            c.R = reader.ReadSingle();
            c.G = reader.ReadSingle();
            c.B = reader.ReadSingle();
            c.A = reader.ReadSingle();
            Value = c;
        }

        public override long Serialize(BinaryWriter writer)
        {
            writer.Write(Value.R);
            writer.Write(Value.G);
            writer.Write(Value.B);
            writer.Write(Value.A);

            return 16;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public struct LinearColor
    {
        public float R;
        public float G;
        public float B;
        public float A;

		public override string ToString()
		{
            return $"{R},{G},{B},{A}";
		}
	}
}
