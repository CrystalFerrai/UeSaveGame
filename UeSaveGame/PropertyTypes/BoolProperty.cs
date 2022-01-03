using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class BoolProperty : UProperty<bool>
    {
        protected override long ContentSize => 0; // Size is technically 2, but 0 is always written

        public BoolProperty(UString name, UString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            Value = reader.ReadByte() != 0;
            if (includeHeader)
            {
                reader.ReadByte();
            }
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            writer.Write((byte)(Value ? 1 : 0));
            if (includeHeader)
            {
                writer.Write((byte)0);
            }
            return ContentSize;
        }
    }
}
