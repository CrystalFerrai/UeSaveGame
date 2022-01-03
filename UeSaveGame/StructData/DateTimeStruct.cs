using System.Collections.Generic;
using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.StructData
{
    public class DateTimeStruct : BaseStructData
    {
        public UDateTime DateTime { get; set; }

        public override IEnumerable<string> StructTypes
        {
            get
            {
                yield return "DateTime";
            }
        }

        public override void Deserialize(BinaryReader reader, long size)
        {
            UDateTime dateTime = new UDateTime();
            dateTime.Ticks = reader.ReadInt64();
            DateTime = dateTime;
        }

        public override long Serialize(BinaryWriter writer)
        {
            writer.Write(DateTime.Ticks);

            return 8;
        }

        public override string ToString()
        {
            return DateTime.ToString();
        }
    }
}
