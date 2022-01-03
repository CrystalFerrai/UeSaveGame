using UeSaveGame.PropertyTypes;
using System;
using System.Collections.Generic;
using System.IO;
using UeSaveGame.DataTypes;
using System.Text;

namespace UeSaveGame.Util
{
    internal static class PropertySerializationHelper
    {
        public static IEnumerable<UProperty> ReadProperties(BinaryReader reader, bool isNullTerminated)
        {
            for (; ; )
            {
                UProperty prop = UProperty.Deserialize(reader);
                if (prop is NoneProperty)
                {
                    if (isNullTerminated) reader.ReadInt32();
                    break;
                }
                yield return prop;
            }
        }

        public static long WriteProperties(IEnumerable<UProperty> properties, BinaryWriter writer, bool isNullTerminated)
        {
            long size = 0;

            foreach (UProperty prop in properties)
            {
                size += prop.Serialize(writer);
            }
            size += 4;
            writer.WriteUnrealString(new UString("None", Encoding.ASCII));
            if (isNullTerminated)
            {
                size += 4;
                writer.Write((Int32)0);
            }

            return size;
        }
    }
}
