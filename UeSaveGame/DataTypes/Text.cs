// Copyright 2025 Crystal Ferrai
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

namespace UeSaveGame.DataTypes
{
	public class FText
	{
		private static readonly Dictionary<TextHistoryType, Type> sTextDataTypes;

		public TextFlags Flags { get; set; }

		public TextHistoryType HistoryType { get; set; }

		public ITextData? Value { get; set; }

		static FText()
		{
			sTextDataTypes = new Dictionary<TextHistoryType, Type>()
			{
				{ TextHistoryType.None, typeof(TextData_None) },
				{ TextHistoryType.Base, typeof(TextData_Base) },
				{ TextHistoryType.ArgumentFormat, typeof(TextData_ArgumentFormat) },
				{ TextHistoryType.AsDateTime, typeof(TextData_AsDateTime) },
				{ TextHistoryType.StringTableEntry, typeof(TextData_StringTableEntry) }
			};
		}

		public void Deserialize(BinaryReader reader, PackageVersion version)
		{
			Flags = (TextFlags)reader.ReadUInt32();
			HistoryType = (TextHistoryType)reader.ReadSByte();

			Type? dataType;
			if (!sTextDataTypes.TryGetValue(HistoryType, out dataType))
			{
				throw new NotImplementedException($"[TextProperty] Data type {HistoryType} is not implemented.");
			}

			Value = (ITextData?)Activator.CreateInstance(dataType);
			Value?.Deserialize(reader, version);
		}

		public int Serialize(BinaryWriter writer, PackageVersion version)
		{
			writer.Write((int)Flags);
			writer.Write((sbyte)HistoryType);

			return 5 + (Value?.Serialize(writer, version) ?? 0);
		}

		public override string ToString()
		{
			return Value?.ToString() ?? string.Empty;
		}
	}
}
