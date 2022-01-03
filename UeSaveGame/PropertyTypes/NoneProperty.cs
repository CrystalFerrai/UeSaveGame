using System.IO;
using System.Text;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class NoneProperty : UProperty
    {
        protected override long ContentSize => 0;

        public NoneProperty()
            : base(new UString("None", Encoding.ASCII), new UString("None", Encoding.ASCII))
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            throw new InvalidDataException("Cannot deserialize a NoneProperty");
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            throw new InvalidDataException("Attempting to serialize a NoneProperty.");
        }
    }
}
