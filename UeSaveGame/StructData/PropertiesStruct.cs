using UeSaveGame.Util;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UeSaveGame.StructData
{
    public class PropertiesStruct : BaseStructData
    {
        public override IEnumerable<string> StructTypes
        {
            get
            {
                yield break;
            }
        }

        public IList<UProperty> Properties { get; private set; }

        public PropertiesStruct()
        {
            Properties = new List<UProperty>();
        }

        public override void Deserialize(BinaryReader reader, long size)
        {
            Properties = new List<UProperty>(PropertySerializationHelper.ReadProperties(reader, false));
        }

        public override long Serialize(BinaryWriter writer)
        {
            long startPosition = writer.BaseStream.Position;

            PropertySerializationHelper.WriteProperties(Properties, writer, false);

            return writer.BaseStream.Position - startPosition;
        }

        public override string ToString()
        {
            if (Properties.Count == 1)
            {
                return Properties[0].ToString();
            }
            return $"{Properties.Count} Properties";
        }
    }
}
