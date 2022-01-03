using UeSaveGame.Util;
using System;
using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class ObjectProperty : UProperty
    {
        protected override long ContentSize => 4 + ObjectType.SizeInBytes;

        public UString ObjectType { get; private set; }

        public ObjectProperty(UString name, UString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader) reader.ReadByte();

            ObjectType = reader.ReadUnrealString();
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader) writer.Write((byte)0);

            writer.WriteUnrealString(ObjectType);

            return ContentSize;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash = hash * 23 + ObjectType.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) && obj is ObjectProperty op && ObjectType == op.ObjectType;
        }

        public override string ToString()
        {
            return $"{Name} [{Type}] {Value?.ToString() ?? ObjectType}";
        }
    }
}
