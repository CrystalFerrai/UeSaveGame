using System;
using System.IO;
using UeSaveGame.DataTypes;
using UeSaveGame.Util;

namespace UeSaveGame.TextData
{
    public class TextData_AsDateTime : ITextData
    {
        public UDateTime DateTime { get; set; }

        public DateTimeStyle DateStyle { get; set; }

        public DateTimeStyle TimeStyle { get; set; }

        public UString TimeZone { get; set; }

        public UString CultureName { get; set; }

        public void Deserialize(BinaryReader reader, long size)
        {
            UDateTime dateTime = new UDateTime();
            dateTime.Ticks = reader.ReadInt64();
            DateTime = dateTime;

            DateStyle = (DateTimeStyle)reader.ReadSByte();
            TimeStyle = (DateTimeStyle)reader.ReadSByte();

            TimeZone = reader.ReadUnrealString();
            CultureName = reader.ReadUnrealString();
        }

        public long Serialize(BinaryWriter writer)
        {
            long startPosition = writer.BaseStream.Position;

            writer.Write(DateTime.Ticks);

            writer.Write((sbyte)DateStyle);
            writer.Write((sbyte)TimeStyle);

            writer.WriteUnrealString(TimeZone);
            writer.WriteUnrealString(CultureName);

            return writer.BaseStream.Position - startPosition;
        }

        public override string ToString()
        {
            return DateTime.ToString();
        }
    }

    public enum DateTimeStyle : sbyte
    {
        Default,
        Short,
        Medium,
        Long,
        Full
    }
}
