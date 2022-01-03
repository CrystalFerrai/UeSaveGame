using System;
using System.IO;
using UeSaveGame.DataTypes;
using UeSaveGame.Util;

namespace UeSaveGame.PropertyTypes
{
    // Note: This type was found in Subverse. The format does not appear to match UE4 serialization code for this type.
    // If the type is ever found elsewhere, we should check if the format is the same.

    public class MulticastDelegateProperty : UProperty<UDelegate[]>
    {
        public MulticastDelegateProperty(UString name, UString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader)
            {
                reader.ReadByte();
            }

            int count = reader.ReadInt32();
            Value = new UDelegate[count];

            for (int i = 0; i < count; ++i)
            {
                Value[i].ClassName = reader.ReadUnrealString();
                Value[i].FunctionName = reader.ReadUnrealString();
            }
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader)
            {
                writer.Write((byte)0);
            }

            long startPosition = writer.BaseStream.Position;

            writer.Write(Value.Length);
            foreach (UDelegate dlgt in Value)
            {
                writer.WriteUnrealString(dlgt.ClassName);
                writer.WriteUnrealString(dlgt.FunctionName);
            }

            return writer.BaseStream.Position - startPosition;
        }
    }

    public struct UDelegate
    {
        public UString ClassName { get; set; }

        public UString FunctionName { get; set; }
    }
}
