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

        private TextFlags mFlags;
        private TextHistoryType mHistoryType;

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

            mFlags = (TextFlags)reader.ReadUInt32();
            mHistoryType = (TextHistoryType)reader.ReadSByte();

            Type? dataType;
            if (!sTextDataTypes.TryGetValue(mHistoryType, out dataType))
            {
                throw new NotImplementedException($"[TextProperty] Data type {mHistoryType} is not implemented.");
            }

            Value = (ITextData?)Activator.CreateInstance(dataType);
            Value?.Deserialize(reader, size - (includeHeader ? 6 : 5));
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader, EngineVersion engineVersion)
        {
            if (includeHeader) writer.Write((byte)0);

            writer.Write((int)mFlags);
            writer.Write((sbyte)mHistoryType);

            return 5 + (Value?.Serialize(writer) ?? 0);
        }

        [Flags]
        private enum TextFlags : int
        {
            Transient = (1 << 0),
            CultureInvariant = (1 << 1),
            ConvertedProperty = (1 << 2),
            Immutable = (1 << 3),
            InitializedFromString = (1 << 4)
        }

        private enum TextHistoryType : sbyte
        {
            None = -1,
            Base = 0,
            NamedFormat,
            OrderedFormat,
            ArgumentFormat,
            AsNumber,
            AsPercent,
            AsCurrency,
            AsDate,
            AsTime,
            AsDateTime,
            Transform,
            StringTableEntry,
            TextGenerator
        }
    }
}
