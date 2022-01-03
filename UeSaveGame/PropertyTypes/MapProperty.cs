using UeSaveGame.Util;
using System;
using System.Collections.Generic;
using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class MapProperty : UProperty<IList<KeyValuePair<UProperty, UProperty>>>
    {
        private int mRemovedCount;

        public UString KeyType { get; private set; }
        public UString ValueType { get; private set; }

        public MapProperty(UString name, UString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader)
            {
                KeyType = reader.ReadUnrealString();
                ValueType = reader.ReadUnrealString();
                reader.ReadByte();
            }

            mRemovedCount = reader.ReadInt32();
            if (mRemovedCount != 0)
            {
                // Maps share some serialization code with Sets. Sets can store items to be removed as well as items to be added.
                // Not sure if such a feature exists for maps, but it has not yet been encountered if it does.
                throw new NotImplementedException();
            }

            int count = reader.ReadInt32();
            Value = new List<KeyValuePair<UProperty, UProperty>>(count);
            for (int i = 0; i < count; ++i)
            {
                UProperty key;
                {
                    Type type = ResolveType(KeyType);
                    key = (UProperty)Activator.CreateInstance(type, UString.Empty, KeyType);
                    key.Deserialize(reader, 0, false);
                }

                UProperty value;
                {
                    Type type = ResolveType(ValueType);
                    value = (UProperty)Activator.CreateInstance(type, Name, ValueType);
                    value.Deserialize(reader, 0, false);
                }
                Value.Add(new KeyValuePair<UProperty, UProperty>(key, value));
            }
            tempSize = size;
        }

        long tempSize;

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader)
            {
                writer.WriteUnrealString(KeyType);
                writer.WriteUnrealString(ValueType);
                writer.Write((byte)0);
            }

            long startPosition = writer.BaseStream.Position;

            writer.Write(mRemovedCount);

            writer.Write(Value.Count);
            foreach (var pair in Value)
            {
                pair.Key.Serialize(writer, false);
                pair.Value.Serialize(writer, false);
            }

            return writer.BaseStream.Position - startPosition;
        }

        public override string ToString()
        {
            return Value == null ? base.ToString() : $"{Name} [{nameof(MapProperty)}<{KeyType},{ValueType}>] Count = {Value.Count}";
        }
    }
}
