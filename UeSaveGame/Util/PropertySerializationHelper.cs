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

using System.Text;
using UeSaveGame.PropertyTypes;

namespace UeSaveGame.Util
{
    /// <summary>
    /// Utility for serialization of standard UE property lists
    /// </summary>
	public static class PropertySerializationHelper
    {
        public static IEnumerable<UProperty> ReadProperties(BinaryReader reader, PackageVersion packageVersion, bool isNullTerminated)
        {
            for (; ; )
            {
                UProperty prop = UProperty.Deserialize(reader, packageVersion);
                if (prop is NoneProperty)
                {
                    if (isNullTerminated) reader.ReadInt32();
                    break;
                }
                yield return prop;
            }
        }

        public static long WriteProperties(IEnumerable<UProperty> properties, BinaryWriter writer, PackageVersion packageVersion, bool isNullTerminated)
        {
            long size = 0;

            foreach (UProperty prop in properties)
            {
                size += prop.Serialize(writer, packageVersion);
            }
            size += 4;
            writer.WriteUnrealString(new FString("None", Encoding.ASCII));
            if (isNullTerminated)
            {
                size += 4;
                writer.Write((Int32)0);
            }

            return size;
        }
    }
}
