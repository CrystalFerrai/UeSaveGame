// Copyright 2022 Crystal Ferrai
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Text;

namespace UeSaveGame.Util
{
    /// <summary>
    /// Extension methods for reading/writing custom types using standard binary readers and writers
    /// </summary>
	public static class BinaryIOExtensions
    {
        public static FString? ReadUnrealString(this BinaryReader reader)
        {
            var length = reader.ReadInt32();
            switch (length)
            {
                case 0:
                    return null;
                case 1:
                    reader.ReadByte();
                    return FString.Empty;
                default:
                    if (length < 0)
                    {
                        byte[] data = reader.ReadBytes(-length * 2);
                        return new FString(Encoding.Unicode.GetString(data, 0, data.Length - 2), Encoding.Unicode);
                    }
                    else
                    {
                        byte[] data = reader.ReadBytes(length);
                        return new FString(Encoding.ASCII.GetString(data, 0, data.Length - 1), Encoding.ASCII);
                    }
            }
        }

        public static void WriteUnrealString(this BinaryWriter writer, FString? value)
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
