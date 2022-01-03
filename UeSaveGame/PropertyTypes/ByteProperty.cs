using UeSaveGame.Util;
using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class ByteProperty : UProperty
    {
        protected override long ContentSize => Value is byte ? 1 : 4 + ((UString)Value).SizeInBytes;

        public UString EnumType { get; private set; }

        public ByteProperty(UString name, UString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader)
            {
                EnumType = reader.ReadUnrealString();
                reader.ReadByte();
            }

            switch (size)
            {
                case 1:
                    Value = reader.ReadByte();
                    break;
                default:
                    Value = reader.ReadUnrealString();
                    break;
            }
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader)
            {
                writer.WriteUnrealString(EnumType);
                writer.Write((byte)0);
            }

            if (Value is byte b)
            {
                writer.Write(b);
            }
            else if (Value is UString s)
            {
                writer.WriteUnrealString(s);
            }

            return ContentSize;
        }
    }
}
