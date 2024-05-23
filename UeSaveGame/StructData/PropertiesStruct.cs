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

using UeSaveGame.Util;

namespace UeSaveGame.StructData
{
	public class PropertiesStruct : BaseStructData
    {
        public override IEnumerable<string> StructTypes
        {
            get
            {
                yield break;
            }
        }

        public IList<UProperty> Properties { get; private set; }

        public PropertiesStruct()
        {
            Properties = new List<UProperty>();
        }

        public override void Deserialize(BinaryReader reader, long size, EngineVersion engineVersion)
        {
            Properties = new List<UProperty>(PropertySerializationHelper.ReadProperties(reader, engineVersion, false));
        }

        public override long Serialize(BinaryWriter writer, EngineVersion engineVersion)
        {
            long startPosition = writer.BaseStream.Position;

            PropertySerializationHelper.WriteProperties(Properties, writer, engineVersion, false);

            return writer.BaseStream.Position - startPosition;
        }

        public override string ToString()
        {
            if (Properties.Count == 1)
            {
                return Properties[0].ToString();
            }
            return $"{Properties.Count} Properties";
        }
    }
}
