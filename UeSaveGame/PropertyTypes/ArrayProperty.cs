using UeSaveGame.Util;
using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class ArrayProperty : UProperty<UProperty[]>
    {
        private StructProperty mPrototype;

        public UString ItemType { get; private set; }

        public ArrayProperty(UString name, UString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader)
            {
                ItemType = reader.ReadUnrealString();
                reader.ReadByte();
            }

            int count = reader.ReadInt32();

            UProperty[] data;
            mPrototype = ArraySerializationHelper.Deserialize(reader, count, size - 4, ItemType, includeHeader, out data);
            Value = data;
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader)
            {
                writer.WriteUnrealString(ItemType);
                writer.Write((byte)0);
            }

            long size = 4;
            writer.Write(Value.Length);

            size += ArraySerializationHelper.Serialize(writer, ItemType, includeHeader, mPrototype, Value);

            return size;
        }

        public override string ToString()
        {
            string valueString = Value.Length == 1 ? Value[0].ToString() : $"Count = {Value.Length}";
            return Value == null ? base.ToString() : $"{Name} [{nameof(ArrayProperty)}<{ItemType}>] {valueString}";
        }
    }
}
