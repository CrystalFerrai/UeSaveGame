using System;
using System.IO;
using System.Text;
using UeSaveGame.DataTypes;

namespace UeSaveGame.Util
{
    public static class BinaryIOExtensions
    {
        public static UString ReadUnrealString(this BinaryReader reader)
        {
            var length = reader.ReadInt32();
            switch (length)
            {
                case 0:
                    return null;
                case 1:
                    reader.ReadByte();
                    return UString.Empty;
                default:
                    if (length < 0)
                    {
                        byte[] data = reader.ReadBytes(-length * 2);
                        return new UString(Encoding.Unicode.GetString(data, 0, data.Length - 2), Encoding.Unicode);
                    }
                    else
                    {
                        byte[] data = reader.ReadBytes(length);
                        return new UString(Encoding.ASCII.GetString(data, 0, data.Length - 1), Encoding.ASCII);
                    }
            }
        }

        public static void WriteUnrealString(this BinaryWriter writer, UString value)
        {
            if (value is null || value.Value is null)
            {
                writer.Write(0);
                return;
            }

            int len = value.Length + 1;
            if (value.Encoding.GetByteCount("A") > 1) len = -len;
            writer.Write(len);

            byte[] data = value.Encoding.GetBytes(value.Value);
            if (data.Length > 0)
            {
                writer.Write(data);
            }
            writer.Write(value.Encoding.GetBytes("\0"));
        }
    }
}
