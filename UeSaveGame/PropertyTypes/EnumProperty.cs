using UeSaveGame.Util;
using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class EnumProperty : UProperty<UString>
    {
        protected override long ContentSize => 4 + Value.SizeInBytes;

        public UString EnumType { get; private set; }

        public EnumProperty(UString name, UString type)
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

            Value = reader.ReadUnrealString();

            if (!includeHeader)
            {
                EnumType = new UString(Value.Value.Substring(0, Value.Value.IndexOf(":")), Value.Encoding);
            }
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader)
            {
                writer.WriteUnrealString(EnumType);
                writer.Write((byte)0);
            }
            writer.WriteUnrealString(Value);

            return ContentSize;
        }
    }
}
