using UeSaveGame.DataTypes;
using System.Collections.Generic;
using System.IO;

namespace UeSaveGame.StructData
{
    public class VectorStruct : BaseStructData
    {
        public override IEnumerable<string> StructTypes
        {
            get
            {
                yield return "Vector";
                yield return "Rotator";
            }
        }

        public Vector Value { get; set; }

        public VectorStruct()
        {
        }

        public override void Deserialize(BinaryReader reader, long size)
        {
            Vector v;
            v.X = reader.ReadSingle();
            v.Y = reader.ReadSingle();
            v.Z = reader.ReadSingle();
            Value = v;
        }

        public override long Serialize(BinaryWriter writer)
        {
            writer.Write(Value.X);
            writer.Write(Value.Y);
            writer.Write(Value.Z);

            return 12;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
