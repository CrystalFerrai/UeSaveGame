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

using UeSaveGame.DataTypes;
using UeSaveGame.Util;

namespace UeSaveGame.TextData
{
	public class TextData_AsDateTime : ITextData
    {
        public UDateTime DateTime { get; set; }

        public DateTimeStyle DateStyle { get; set; }

        public DateTimeStyle TimeStyle { get; set; }

        public FString? TimeZone { get; set; }

        public FString? CultureName { get; set; }

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
