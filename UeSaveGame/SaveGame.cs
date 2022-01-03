using UeSaveGame.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UeSaveGame.DataTypes;

namespace UeSaveGame
{
    public class SaveGame
    {
        private static readonly byte[] sHeader = Encoding.ASCII.GetBytes("GVAS");

        internal SaveGameHeader Header { get; private set; }
        internal CustomFormatData CustomFormats { get; private set; }

        public UString SaveClass { get; private set; }

        public IList<UProperty> Properties { get; private set; }

        private SaveGame()
        {
        }

        public static SaveGame LoadFrom(Stream stream)
        {
            SaveGame instance = new SaveGame();

            byte[] id = new byte[sHeader.Length];
            stream.Read(id, 0, sHeader.Length);

            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                instance.Header = SaveGameHeader.Deserialize(reader);
                if (instance.Header.SaveGameVersion != 2) throw new NotSupportedException($"Save game version {instance.Header.SaveGameVersion} cannot be read. Only version 2 is supported at this time");

                instance.CustomFormats = CustomFormatData.Deserialize(reader);

                instance.SaveClass = reader.ReadUnrealString();

                instance.Properties = new List<UProperty>(PropertySerializationHelper.ReadProperties(reader, true));

                if (reader.BaseStream.Position != reader.BaseStream.Length) throw new ApplicationException("Did not reach the end of the file when reading.");
            }

            return instance;
        }

        public void WriteTo(Stream stream)
        {
            stream.Write(sHeader, 0, sHeader.Length);

            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                Header.Serialize(writer);
                CustomFormats.Serialize(writer);
                writer.WriteUnrealString(SaveClass);

                PropertySerializationHelper.WriteProperties(Properties, writer, true);
            }
        }

        public override string ToString()
        {
            return $"{SaveClass} - {Properties?.Count ?? 0} Properties";
        }
    }

    internal struct SaveGameHeader
    {
        public int SaveGameVersion;
        public int PackageVersion;
        public EngineVersion EngineVersion;

        public static SaveGameHeader Deserialize(BinaryReader reader)
        {
            SaveGameHeader instance = new SaveGameHeader();

            instance.SaveGameVersion = reader.ReadInt32();
            instance.PackageVersion = reader.ReadInt32();
            instance.EngineVersion = EngineVersion.Deserialize(reader);

            return instance;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(SaveGameVersion);
            writer.Write(PackageVersion);
            EngineVersion.Serialize(writer);
        }
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
        public UString BuildId;

        public static EngineVersion Deserialize(BinaryReader reader)
        {
            EngineVersion version = new EngineVersion();

            version.Major = reader.ReadInt16();
            version.Minor = reader.ReadInt16();
            version.Patch = reader.ReadInt16();
            version.Build = reader.ReadInt32();
            version.BuildId = reader.ReadUnrealString();

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
