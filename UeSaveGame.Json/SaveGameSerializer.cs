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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text;
using UeSaveGame.Util;

namespace UeSaveGame.Json
{
	/// <summary>
	/// Serializes an Unreal Engine save file to or from json
	/// </summary>
	public class SaveGameSerializer
	{
		private static readonly Dictionary<string, Type> sSaveClassSerializerMap;

		private readonly Formatting mFormatting;
		private readonly int mIndentation;
		private readonly char mIndentChar;
		private readonly Encoding mEncoding;

		private readonly SaveGameHeaderSerializer mHeaderSerializer;
		private readonly CustomFormatDataSerializer mCustomFormatDataSerializer;

		static SaveGameSerializer()
		{
			sSaveClassSerializerMap = new();

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				AddSaveClassSerializersFromAssembly(assembly);
			}

			AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public SaveGameSerializer()
			: this(true, 2, ' ')
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="indented">Wether written json should have indents and new lines</param>
		/// <param name="indentation">The number of indentation characters per level in written json</param>
		/// <param name="indentChar">The character to use when indenting written json</param>
		public SaveGameSerializer(bool indented, int indentation, char indentChar)
		{
			mFormatting = indented ? Formatting.Indented : Formatting.None;
			mIndentation = indentation;
			mIndentChar = indentChar;
			mEncoding = new UTF8Encoding(false); // No BOM

			mHeaderSerializer = new();
			mCustomFormatDataSerializer = new();
		}

		/// <summary>
		/// Converts a save game to json
		/// </summary>
		/// <param name="input">A stream containing a complete binary save game</param>
		/// <param name="output">A stream that will receive the converted json save game</param>
		public void ConvertToJson(Stream input, Stream output)
		{
			SaveGame save = SaveGame.LoadFrom(input);

			using StreamWriter sw = new(output, mEncoding, leaveOpen: true);
			using JsonWriter writer = new JsonTextWriter(sw)
			{
				AutoCompleteOnClose = true,
				CloseOutput = false,
				Formatting = mFormatting,
				Indentation = mIndentation,
				IndentChar = mIndentChar
			};

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(SaveGame.Versions));
			mHeaderSerializer.ToJson(save.Versions, writer);

			writer.WritePropertyName(nameof(SaveGame.CustomFormats));
			mCustomFormatDataSerializer.ToJson(save.CustomFormats, writer);

			writer.WritePropertyName(nameof(SaveGame.SaveClass));
			writer.WriteValue(save.SaveClass?.Value);

			ISaveClassSerializer? customSaveClassSerializer = null;
			if (save.CustomSaveClass is not null && sSaveClassSerializerMap.TryGetValue(save.SaveClass!, out Type? saveClassSerializerType))
			{
				customSaveClassSerializer = (ISaveClassSerializer)Activator.CreateInstance(saveClassSerializerType)!;
				if (customSaveClassSerializer.HasCustomHeader)
				{
					writer.WritePropertyName("CustomHeader");
					customSaveClassSerializer.HeaderToJson(writer, save.CustomSaveClass);
				}
			}

			if (customSaveClassSerializer is not null && customSaveClassSerializer.HasCustomData)
			{
				writer.WritePropertyName("CustomData");
				customSaveClassSerializer.DataToJson(writer, save.CustomSaveClass!);
			}
			else
			{
				writer.WritePropertyName(nameof(SaveGame.Properties));
				PropertiesSerializer.ToJson(save.Properties, writer);
			}

			writer.WriteEndObject();

			writer.Close();
		}

		/// <summary>
		/// Converts a save game from json
		/// </summary>
		/// <param name="input">A stream containing a complete json save game</param>
		/// <param name="output">A stream that will received the converted binary save game</param>
		public void ConvertFromJson(Stream input, Stream output)
		{
			SaveGame save = new();

			using StreamReader sr = new(input, mEncoding, leaveOpen: true);
			using JsonReader reader = new JsonTextReader(sr)
			{
				CloseInput = false
			};

			JToken? customHeader = null, customData = null;

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.PropertyName)
				{
					switch ((string)reader.Value!)
					{
						case nameof(SaveGame.Versions):
							save.Versions = mHeaderSerializer.FromJson(reader);
							break;
						case nameof(SaveGame.CustomFormats):
							save.CustomFormats = mCustomFormatDataSerializer.FromJson(reader);
							break;
						case nameof(SaveGame.SaveClass):
							save.SetSaveClass(reader.ReadAsFString());
							break;
						case "CustomHeader":
							if (reader.ReadAndMoveToContent())
							{
								customHeader = JToken.ReadFrom(reader);
							}
							break;
						case "CustomData":
							if (reader.ReadAndMoveToContent())
							{
								customData = JToken.ReadFrom(reader);
							}
							break;
						case nameof(SaveGame.Properties):
							save.Properties = PropertiesSerializer.FromJson(reader);
							break;
					}
				}
			}

			if (save.CustomSaveClass is not null && sSaveClassSerializerMap.TryGetValue(save.SaveClass!, out Type? saveClassSerializerType))
			{
				ISaveClassSerializer customSaveClassSerializer = (ISaveClassSerializer)Activator.CreateInstance(saveClassSerializerType)!;
				if (customSaveClassSerializer.HasCustomHeader && customHeader is not null)
				{
					customSaveClassSerializer.HeaderFromJson(customHeader.CreateReader(), save.CustomSaveClass);
				}
				if (customSaveClassSerializer.HasCustomData && customData is not null)
				{
					customSaveClassSerializer.DataFromJson(customData.CreateReader(), save.CustomSaveClass);
				}
			}

			save.WriteTo(output);

			reader.Close();
		}

		private static void CurrentDomain_AssemblyLoad(object? sender, AssemblyLoadEventArgs args)
		{
			AddSaveClassSerializersFromAssembly(args.LoadedAssembly);
		}

		private static void AddSaveClassSerializersFromAssembly(Assembly assembly)
		{
			foreach (Type type in TypeSearcher.FindDerivedTypes(typeof(SaveClassSerializerBase<>), assembly))
			{
				Type saveClassType = type.BaseType!.GenericTypeArguments[0];

				IEnumerable<SaveClassPathAttribute> classPathAttributes = saveClassType.GetCustomAttributes<SaveClassPathAttribute>();
				if (!classPathAttributes.Any())
				{
					throw new MissingAttributeException(saveClassType, typeof(SaveClassPathAttribute));
				}

				foreach (SaveClassPathAttribute classPathAttribute in classPathAttributes)
				{
					if (!sSaveClassSerializerMap.TryAdd(classPathAttribute.ClassPath, type))
					{
						throw new DuplicateRegistrationException(classPathAttribute.ClassPath, $"Cannot register class '{classPathAttribute.ClassPath}' with type '{type.FullName}' because it is already registered with another type.");
					}
				}
			}
		}
	}

	internal class SaveGameHeaderSerializer
	{
		EngineVersionSerializer mEngineVersionSerializer;

		public SaveGameHeaderSerializer()
		{
			mEngineVersionSerializer = new();
		}

		public void ToJson(SaveGameVersions data, JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(SaveGameVersions.SaveGameVersion));
			writer.WriteValue((int)data.SaveGameVersion);

			writer.WritePropertyName(nameof(PackageVersion.PackageVersionUE4));
			writer.WriteValue(data.PackageVersion.PackageVersionUE4);

			writer.WritePropertyName(nameof(PackageVersion.PackageVersionUE5));
			writer.WriteValue(data.PackageVersion.PackageVersionUE5);

			writer.WritePropertyName(nameof(SaveGameVersions.EngineVersion));
			mEngineVersionSerializer.ToJson(data.EngineVersion, writer);

			writer.WriteEndObject();
		}

		public SaveGameVersions FromJson(JsonReader reader)
		{
			SaveGameVersions data = new();

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.EndObject)
				{
					break;
				}

				if (reader.TokenType == JsonToken.PropertyName)
				{
					switch ((string)reader.Value!)
					{
						case nameof(SaveGameVersions.SaveGameVersion):
							data.SaveGameVersion = (SaveGameFileVersion)reader.ReadAsInt32()!.Value;
							break;
						case nameof(PackageVersion.PackageVersionUE4):
							data.PackageVersion.PackageVersionUE4 = (EObjectUE4Version)reader.ReadAsInt32()!.Value;
							break;
						case nameof(PackageVersion.PackageVersionUE5):
							data.PackageVersion.PackageVersionUE5 = (EObjectUE5Version)reader.ReadAsInt32()!.Value;
							break;
						case nameof(SaveGameVersions.EngineVersion):
							data.EngineVersion = mEngineVersionSerializer.FromJson(reader);
							break;
					}
				}
			}

			return data;
		}
	}

	internal class CustomFormatDataSerializer
	{
		public void ToJson(CustomFormatData data, JsonWriter writer)
		{
			Formatting formatting = writer.Formatting;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(CustomFormatData.Version));
			writer.WriteValue(data.Version);

			writer.WritePropertyName(nameof(CustomFormatData.Formats));
			writer.WriteStartArray();
			foreach (CustomFormatEntry entry in data.Formats)
			{
				writer.WriteStartObject();
				writer.Formatting = Formatting.None;

				writer.WritePropertyName(nameof(CustomFormatEntry.Id));
				writer.WriteValue(entry.Id.ToString("D"));

				writer.WritePropertyName(nameof(CustomFormatEntry.Value));
				writer.WriteValue(entry.Value);

				writer.WriteEndObject();
				writer.Formatting = formatting;
			}
			writer.WriteEndArray();

			writer.WriteEndObject();
		}

		public CustomFormatData FromJson(JsonReader reader)
		{
			CustomFormatData data = new();

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.EndObject)
				{
					break;
				}

				if (reader.TokenType == JsonToken.PropertyName)
				{
					switch ((string)reader.Value!)
					{
						case nameof(CustomFormatData.Version):
							data.Version = reader.ReadAsInt32()!.Value;
							break;
						case nameof(CustomFormatData.Formats):
							{
								List<CustomFormatEntry> formats = new();
								while (reader.Read())
								{
									if (reader.TokenType == JsonToken.EndArray)
									{
										break;
									}

									if (reader.TokenType == JsonToken.StartObject)
									{
										CustomFormatEntry entry = new();

										while (reader.Read())
										{
											if (reader.TokenType == JsonToken.EndObject)
											{
												break;
											}

											if (reader.TokenType == JsonToken.PropertyName)
											{
												switch ((string)reader.Value!)
												{
													case nameof(CustomFormatEntry.Id):
														entry.Id = Guid.Parse(reader.ReadAsString()!);
														break;
													case nameof(CustomFormatEntry.Value):
														entry.Value = reader.ReadAsInt32()!.Value;
														break;
												}
											}
										}

										formats.Add(entry);
									}
								}
								data.Formats = formats;
							}
							break;
					}
				}
			}

			return data;
		}
	}
}
