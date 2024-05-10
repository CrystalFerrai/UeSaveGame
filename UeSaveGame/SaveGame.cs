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
using UeSaveGame.Util;

namespace UeSaveGame
{
    /// <summary>
    /// Represents a UE save game file
    /// </summary>
	public class SaveGame
    {
        private static readonly uint sMagic = 0x53415647; // SAVG

        internal SaveGameHeader Header { get; private set; }
        internal CustomFormatData CustomFormats { get; private set; }

        /// <summary>
        /// The type of the save game
        /// </summary>
        public FString? SaveClass { get; private set; }

        /// <summary>
        /// The save game's data/properties
        /// </summary>
        public IList<UProperty>? Properties { get; private set; }

        private SaveGame()
        {
        }

        /// <summary>
        /// Loads a save game from the given stream
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <exception cref="NotSupportedException">Unsupported save game version</exception>
        /// <exception cref="FormatException">A problem occurred trying to parse the save game</exception>
        public static SaveGame LoadFrom(Stream stream)
        {
            SaveGame instance = new SaveGame();

            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                if (reader.ReadUInt32() != sMagic) throw new InvalidDataException("Save game header is missing or invalid.");

                instance.Header = SaveGameHeader.Deserialize(reader);

                instance.CustomFormats = CustomFormatData.Deserialize(reader);

                instance.SaveClass = reader.ReadUnrealString();

                instance.Properties = new List<UProperty>(PropertySerializationHelper.ReadProperties(reader, true));

                if (reader.BaseStream.Position != reader.BaseStream.Length) throw new FormatException("Did not reach the end of the file when reading.");
            }

            return instance;
        }

        /// <summary>
        /// Saves a save game to the given stream
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void WriteTo(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                writer.Write(sMagic);

                Header.Serialize(writer);
                CustomFormats.Serialize(writer);
                writer.WriteUnrealString(SaveClass!);

                PropertySerializationHelper.WriteProperties(Properties!, writer, true);
            }
        }

        public override string ToString()
        {
            return $"{SaveClass} - {Properties?.Count ?? 0} Properties";
        }
    }

    /// <summary>
    /// Block of version information from the file header
    /// </summary>
    internal struct SaveGameHeader
    {
        public SaveGameFileVersion SaveGameVersion;
        public int PackageVersionUE4;
        public int PackageVersionUE5;
        public EngineVersion EngineVersion;

        public static SaveGameHeader Deserialize(BinaryReader reader)
        {
            SaveGameHeader instance = new();

            instance.SaveGameVersion = (SaveGameFileVersion)reader.ReadInt32();
            if (instance.SaveGameVersion != SaveGameFileVersion.AddedCustomVersions && instance.SaveGameVersion != SaveGameFileVersion.PackageFileSummaryVersionChange)
            {
                throw new NotSupportedException($"Save game version {(int)instance.SaveGameVersion} is not supported at this time");
            }

            instance.PackageVersionUE4 = reader.ReadInt32();

            if (instance.SaveGameVersion == SaveGameFileVersion.PackageFileSummaryVersionChange)
            {
                instance.PackageVersionUE5 = reader.ReadInt32();
            }
            else
			{
                instance.PackageVersionUE5 = 0;
			}

            instance.EngineVersion = EngineVersion.Deserialize(reader);

            return instance;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write((int)SaveGameVersion);
            writer.Write(PackageVersionUE4);
            if (SaveGameVersion == SaveGameFileVersion.PackageFileSummaryVersionChange)
            {
                writer.Write(PackageVersionUE5);
            }
			EngineVersion.Serialize(writer);
        }
    }

    /// <summary>
    /// Overall save file format version
    /// </summary>
    internal enum SaveGameFileVersion
    {
		InitialVersion = 1,
		AddedCustomVersions = 2,
		PackageFileSummaryVersionChange = 3
	}

	/// <summary>
	/// Represents a version of the engine
	/// </summary>
	internal struct EngineVersion
    {
        public short Major;
        public short Minor;
        public short Patch;
        public int Build;
        public FString BuildId;

        public static EngineVersion Deserialize(BinaryReader reader)
        {
            EngineVersion version = new EngineVersion();

            version.Major = reader.ReadInt16();
            version.Minor = reader.ReadInt16();
            version.Patch = reader.ReadInt16();
            version.Build = reader.ReadInt32();
            version.BuildId = reader.ReadUnrealString()!;

            return version;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Major);
            writer.Write(Minor);
            writer.Write(Patch);
            writer.Write(Build);
            writer.WriteUnrealString(BuildId);
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}.{Build}";
        }
    }

    /// <summary>
    /// List of system versions in use while saving
    /// </summary>
    internal struct CustomFormatData
    {
        public int Version;
        public IReadOnlyList<CustomFormatEntry> Formats;

        public static CustomFormatData Deserialize(BinaryReader reader)
        {
            CustomFormatData instance = new CustomFormatData();

            instance.Version = reader.ReadInt32();

            uint count = reader.ReadUInt32();
            CustomFormatEntry[] formats = new CustomFormatEntry[count];
            for (uint i = 0; i < count; i++)
            {
                formats[i].Id = new Guid(reader.ReadBytes(16));
                formats[i].Value = reader.ReadInt32();
            }
            instance.Formats = formats;

            return instance;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(Formats.Count);
            foreach (CustomFormatEntry format in Formats)
            {
                writer.Write(format.Id.ToByteArray());
                writer.Write(format.Value);
            }
        }
    }

    /// <summary>
    /// Specifies which version of a particular system was being used while saving
    /// </summary>
    /// <remarks>
    /// See DevObjectVersion.cpp in the engine for what a lot of these GUIDs map to.
    /// </remarks>
    internal struct CustomFormatEntry
    {
        public Guid Id;
        public int Value;

        public override string ToString()
        {
            return $"{Id} - {Value}";
        }
    }
}
