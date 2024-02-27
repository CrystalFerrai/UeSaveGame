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

using UeSaveGame.DataTypes;

namespace UeSaveGame.StructData
{
	public class IntPointStruct : BaseStructData
    {
        FIntPoint Value { get; set; }

        public override IEnumerable<string> StructTypes
        {
            get
            {
                yield return "IntPoint";
            }
        }

        public override void Deserialize(BinaryReader reader, long size)
        {
            FIntPoint value = new();
            value.X = reader.ReadInt32();
            value.Y = reader.ReadInt32();
            Value = value;
        }

        public override long Serialize(BinaryWriter writer)
        {
            writer.Write(Value.X);
            writer.Write(Value.Y);

            return 8;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
