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

using System.IO;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using UeSaveGame.Util;

namespace UeSaveGame
{
	/// <summary>
	/// Represents a UE save game file
	/// </summary>
	public class SaveGame
	{
		private const uint sMagic = 0x53415647; // SAVG

		private static readonly Dictionary<string, Type> sSaveClassMap;

		internal SaveGameVersions Versions { get; set; }
		internal CustomFormatData CustomFormats { get; set; }

		/// <summary>
		/// The type of the save game
		/// </summary>
		public FString? SaveClass { get; internal set; }

		/// <summary>
		/// The save game's data/properties
		/// </summary>
		public IList<FPropertyTag>? Properties { get; internal set; }

		/// <summary>
		/// The custom save class for this save game type, if one exists
		/// </summary>
		public SaveClassBase? CustomSaveClass { get; private set; }

		static SaveGame()
		{
			sSaveClassMap = new();

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				AddSaveClassesFromAssembly(assembly);
			}

			AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
		}

		internal SaveGame()
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
			SaveGame instance = new();

			using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
			{
				if (reader.ReadUInt32() != sMagic) throw new InvalidDataException("Save game header is missing or invalid.");

				instance.Versions = SaveGameVersions.Deserialize(reader);

				instance.CustomFormats = CustomFormatData.Deserialize(reader);

				instance.SetSaveClass(reader.ReadUnrealString());

				if (instance.CustomSaveClass is not null && instance.CustomSaveClass.HasCustomHeader)
				{
					instance.CustomSaveClass.DeserializeHeader(reader, instance.Versions.PackageVersion);
				}

				if (instance.Versions.PackageVersion >= EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
				{
					byte unknown = reader.ReadByte();
					if (unknown != 0) throw new NotImplementedException("Unexpected value after save class name. Game might have custom save class serialization.");
				}

				if (instance.CustomSaveClass is not null && instance.CustomSaveClass.HasCustomData)
				{
					instance.CustomSaveClass.DeserializeData(reader, instance.Versions.PackageVersion);
				}
				else
				{
					instance.Properties = new List<FPropertyTag>(PropertySerializationHelper.ReadProperties(reader, instance.Versions.PackageVersion, true));
				}

				if (reader.BaseStream.CanSeek && reader.BaseStream.Position != reader.BaseStream.Length) throw new FormatException("Did not reach the end of the file when reading.");
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
				WritePart(SaveGamePart.Magic, writer);
				WritePart(SaveGamePart.Versions, writer);
				WritePart(SaveGamePart.CustomFormats, writer);
				WritePart(SaveGamePart.SaveClass, writer);
				WritePart(SaveGamePart.Data, writer);
			}
		}

		/// <summary>
		/// Write a specific part of the save game
		/// </summary>
		/// <param name="part">The part to write</param>
		/// <param name="writer">The writer to write to</param>
		public void WritePart(SaveGamePart part, BinaryWriter writer)
		{
			switch (part)
			{
				case SaveGamePart.Magic:
					writer.Write(sMagic);
					break;
				case SaveGamePart.Versions:
					Versions.Serialize(writer);
					break;
				case SaveGamePart.CustomFormats:
					CustomFormats.Serialize(writer);
					break;
				case SaveGamePart.SaveClass:
					writer.WriteUnrealString(SaveClass!);
					break;
				case SaveGamePart.Data:
					WriteDataPart(writer);
					break;
			}
		}

		internal void SetSaveClass(FString? saveClass)
		{
			SaveClass = saveClass;
			if (SaveClass is not null && sSaveClassMap.TryGetValue(SaveClass, out Type? saveClassType))
			{
				CustomSaveClass = (SaveClassBase)Activator.CreateInstance(saveClassType)!;
			}
		}

		private void WriteDataPart(BinaryWriter writer)
		{
			long headerPosition = writer.BaseStream.Position;

			long customHeaderLength = 0;
			if (CustomSaveClass is not null && CustomSaveClass.HasCustomHeader)
			{
				customHeaderLength = CustomSaveClass.GetHeaderSize(Versions.PackageVersion);
				byte[] placeholder = new byte[customHeaderLength];
				writer.Write(placeholder);
			}

			if (Versions.PackageVersion >= EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				writer.Write((byte)0);
			}

			if (CustomSaveClass is not null && CustomSaveClass.HasCustomData)
			{
				CustomSaveClass.SerializeData(writer, Versions.PackageVersion);
			}
			else
			{
				PropertySerializationHelper.WriteProperties(Properties!, writer, Versions.PackageVersion, true);
			}

			if (CustomSaveClass is not null && CustomSaveClass.HasCustomHeader)
			{
				writer.BaseStream.Seek(headerPosition, SeekOrigin.Begin);
				CustomSaveClass.SerializeHeader(writer, writer.BaseStream.Length - customHeaderLength - headerPosition, Versions.PackageVersion);

				if (writer.BaseStream.Position - headerPosition != customHeaderLength)
				{
					throw new InvalidOperationException($"{CustomSaveClass.GetType().FullName} returned {customHeaderLength} from GetHeaderSize, but SerializeHeader wrote {writer.BaseStream.Position - headerPosition} bytes.");
				}

				writer.BaseStream.Seek(0, SeekOrigin.End);
			}
		}

		public override string ToString()
		{
			return $"{SaveClass} - {Properties?.Count ?? 0} Properties";
		}

		#region Save class locating

		private static void CurrentDomain_AssemblyLoad(object? sender, AssemblyLoadEventArgs args)
		{
			AddSaveClassesFromAssembly(args.LoadedAssembly);
		}

		private static void AddSaveClassesFromAssembly(Assembly assembly)
		{
			foreach (Type type in TypeSearcher.FindDerivedTypes(typeof(SaveClassBase), assembly))
			{
				IEnumerable<SaveClassPathAttribute> classPathAttributes = type.GetCustomAttributes<SaveClassPathAttribute>();
				if (!classPathAttributes.Any())
				{
					throw new MissingAttributeException(type, typeof(SaveClassPathAttribute));
				}

				foreach (SaveClassPathAttribute classPathAttribute in classPathAttributes)
				{
					if (!sSaveClassMap.TryAdd(classPathAttribute.ClassPath, type))
					{
						throw new DuplicateRegistrationException(classPathAttribute.ClassPath, $"Cannot register class '{classPathAttribute.ClassPath}' with type '{type.FullName}' because it is already registered with another type.");
					}
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// Identifies a specific part of a save game
	/// </summary>
	public enum SaveGamePart
	{
		/// <summary>
		/// No part / nothing
		/// </summary>
		None,

		/// <summary>
		/// The magic number value usually at the start of a save file
		/// </summary>
		Magic,

		/// <summary>
		/// Version information
		/// </summary>
		Versions,

		/// <summary>
		/// The custom formats block
		/// </summary>
		CustomFormats,

		/// <summary>
		/// The save class name
		/// </summary>
		SaveClass,

		/// <summary>
		/// The save data
		/// </summary>
		Data
	}

	/// <summary>
	/// Block of version information from the file header
	/// </summary>
	internal struct SaveGameVersions
	{
		public SaveGameFileVersion SaveGameVersion;
		public PackageVersion PackageVersion;
		public EngineVersion EngineVersion;

		public static SaveGameVersions Deserialize(BinaryReader reader)
		{
			SaveGameVersions instance = new();

			instance.SaveGameVersion = (SaveGameFileVersion)reader.ReadInt32();
			if (instance.SaveGameVersion != SaveGameFileVersion.AddedCustomVersions && instance.SaveGameVersion != SaveGameFileVersion.PackageFileSummaryVersionChange)
			{
				throw new NotSupportedException($"Save game version {(int)instance.SaveGameVersion} is not supported at this time");
			}

			instance.PackageVersion.PackageVersionUE4 = (EObjectUE4Version)reader.ReadUInt32();

			if (instance.SaveGameVersion == SaveGameFileVersion.PackageFileSummaryVersionChange)
			{
				instance.PackageVersion.PackageVersionUE5 = (EObjectUE5Version)reader.ReadUInt32();
			}
			else
			{
				instance.PackageVersion.PackageVersionUE5 = EObjectUE5Version.INVALID;
			}

			instance.EngineVersion = EngineVersion.Deserialize(reader);

			return instance;
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write((int)SaveGameVersion);
			writer.Write((uint)PackageVersion.PackageVersionUE4);
			if (SaveGameVersion == SaveGameFileVersion.PackageFileSummaryVersionChange)
			{
				writer.Write((uint)PackageVersion.PackageVersionUE5);
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
