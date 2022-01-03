using UeSaveGame.DataTypes;
using System.Collections.Generic;
using System.IO;

namespace UeSaveGame.StructData
{
    public class QuatStruct : BaseStructData
    {
        public override IEnumerable<string> StructTypes
        {
            get
            {
                yield return "Quat";
            }
        }

        public Quaternion Value { get; set; }

        public QuatStruct()
        {
        }

        public override void Deserialize(BinaryReader reader, long size)
        {
            Quaternion q;
            q.X = reader.ReadSingle();
            q.Y = reader.ReadSingle();
            q.Z = reader.ReadSingle();
            q.W = reader.ReadSingle();
            Value = q;
        }

        public override long Serialize(BinaryWriter writer)
        {
            writer.Write(Value.X);
            writer.Write(Value.Y);
            writer.Write(Value.Z);
            writer.Write(Value.W);

            return 16;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
