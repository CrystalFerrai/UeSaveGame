using UeSaveGame.Util;
using System;
using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class SetProperty : UProperty<UProperty[]>
    {
        private StructProperty mPrototype;

        private int mRemovedCount;

        public UString ItemType { get; private set; }

        public SetProperty(UString name, UString type)
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

            mRemovedCount = reader.ReadInt32();
            if (mRemovedCount != 0)
            {
                // Sets can store items to be removed as well as items to be added. Have not encountered the removed case yet.
                throw new NotImplementedException();
            }

            int count = reader.ReadInt32();

            UProperty[] data;
            mPrototype = ArraySerializationHelper.Deserialize(reader, count, size - 8, ItemType, includeHeader, out data);
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
            writer.Write(mRemovedCount);
            if (mRemovedCount != 0)
            {
                throw new NotImplementedException();
            }

            size += 4;
            writer.Write(Value.Length);

            size += ArraySerializationHelper.Serialize(writer, ItemType, includeHeader, mPrototype, Value);

            return size;
        }

        public override string ToString()
        {
            return Value == null ? base.ToString() : $"{Name} [{nameof(SetProperty)}<{ItemType}>] Count = {Value.Length}";
        }
    }
}
