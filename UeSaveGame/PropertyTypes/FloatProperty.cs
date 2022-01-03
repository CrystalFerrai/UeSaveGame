using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class FloatProperty : UProperty<float>
    {
        protected override long ContentSize => 4;

        public FloatProperty(UString name, UString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader) reader.ReadByte();
            Value = reader.ReadSingle();
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader) writer.Write((byte)0);
            writer.Write(Value);

            return ContentSize;
        }
    }
}
