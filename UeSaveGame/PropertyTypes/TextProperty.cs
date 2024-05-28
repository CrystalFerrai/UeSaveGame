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

using UeSaveGame.TextData;

namespace UeSaveGame.PropertyTypes
{
    public class TextProperty : UProperty<ITextData>
    {
        private static readonly Dictionary<TextHistoryType, Type> sTextDataTypes;

        public TextFlags Flags { get; set; }

        public TextHistoryType HistoryType { get; set; }

        static TextProperty()
        {
            sTextDataTypes = new Dictionary<TextHistoryType, Type>()
            {
                { TextHistoryType.None, typeof(TextData_None) },
                { TextHistoryType.Base, typeof(TextData_Base) },
                { TextHistoryType.AsDateTime, typeof(TextData_AsDateTime) },
                { TextHistoryType.StringTableEntry, typeof(TextData_StringTableEntry) }
            };
        }

        public TextProperty(FString name)
            : this(name, new(nameof(TextProperty)))
        {
        }

        public TextProperty(FString name, FString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader, EngineVersion engineVersion)
        {
            if (includeHeader) reader.ReadByte();

            Flags = (TextFlags)reader.ReadUInt32();
            HistoryType = (TextHistoryType)reader.ReadSByte();

            Type? dataType;
            if (!sTextDataTypes.TryGetValue(HistoryType, out dataType))
            {
                throw new NotImplementedException($"[TextProperty] Data type {HistoryType} is not implemented.");
            }

            Value = (ITextData?)Activator.CreateInstance(dataType);
            Value?.Deserialize(reader, size - (includeHeader ? 6 : 5));
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader, EngineVersion engineVersion)
        {
            if (includeHeader) writer.Write((byte)0);

            writer.Write((int)Flags);
            writer.Write((sbyte)HistoryType);

            return 5 + (Value?.Serialize(writer) ?? 0);
        }
    }
}
