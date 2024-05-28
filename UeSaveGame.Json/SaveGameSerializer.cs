// Copyright 2024 Crystal Ferrai
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
using System.Text;

namespace UeSaveGame.Json
{
	/// <summary>
	/// Serializes an Unreal Engine save file to or from json
	/// </summary>
	public class SaveGameSerializer
	{
		private readonly Formatting mFormatting;
		private readonly int mIndentation;
		private readonly char mIndentChar;
		private readonly Encoding mEncoding;

		private readonly SaveGameHeaderSerializer mHeaderSerializer;
		private readonly CustomFormatDataSerializer mCustomFormatDataSerializer;

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

			using StreamWriter sw = new(output, mEncoding);
			using JsonWriter writer = new JsonTextWriter(sw)
			{
				AutoCompleteOnClose = true,
				CloseOutput = false,
				Formatting = mFormatting,
				Indentation = mIndentation,
				IndentChar = mIndentChar
			};

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(SaveGame.Header));
			mHeaderSerializer.ToJson(save.Header, writer);

			writer.WritePropertyName(nameof(SaveGame.CustomFormats));
			mCustomFormatDataSerializer.ToJson(save.CustomFormats, writer);

			writer.WritePropertyName(nameof(SaveGame.SaveClass));
			writer.WriteValue(save.SaveClass?.ToString());

			writer.WritePropertyName(nameof(SaveGame.Properties));
			PropertiesSerializer.ToJson(save.Properties, writer);

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

			using StreamReader sr = new(input, mEncoding);
			using JsonReader reader = new JsonTextReader(sr)
			{
				CloseInput = false
			};

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.PropertyName)
				{
					switch ((string)reader.Value!)
					{
						case nameof(SaveGame.Header):
							save.Header = mHeaderSerializer.FromJson(reader);
							break;
						case nameof(SaveGame.CustomFormats):
							save.CustomFormats = mCustomFormatDataSerializer.FromJson(reader);
							break;
						case nameof(SaveGame.SaveClass):
							save.SaveClass = new(reader.ReadAsString()!);
							break;
						case nameof(SaveGame.Properties):
							save.Properties = PropertiesSerializer.FromJson(reader);
							break;
					}
				}
			}

			save.WriteTo(output);

			reader.Close();
		}
	}

	internal class SaveGameHeaderSerializer
	{
		EngineVersionSerializer mEngineVersionSerializer;

		public SaveGameHeaderSerializer()
		{
			mEngineVersionSerializer = new();
		}

		public void ToJson(SaveGameHeader data, JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(SaveGameHeader.SaveGameVersion));
			writer.WriteValue((int)data.SaveGameVersion);

			writer.WritePropertyName(nameof(SaveGameHeader.PackageVersionUE4));
			writer.WriteValue(data.PackageVersionUE4);

			writer.WritePropertyName(nameof(SaveGameHeader.PackageVersionUE5));
			writer.WriteValue(data.PackageVersionUE5);

			writer.WritePropertyName(nameof(SaveGameHeader.EngineVersion));
			mEngineVersionSerializer.ToJson(data.EngineVersion, writer);

			writer.WriteEndObject();
		}

		public SaveGameHeader FromJson(JsonReader reader)
		{
			SaveGameHeader data = new();

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
						case nameof(SaveGameHeader.SaveGameVersion):
							data.SaveGameVersion = (SaveGameFileVersion)reader.ReadAsInt32()!.Value;
							break;
						case nameof(SaveGameHeader.PackageVersionUE4):
							data.PackageVersionUE4 = reader.ReadAsInt32()!.Value;
							break;
						case nameof(SaveGameHeader.PackageVersionUE5):
							data.PackageVersionUE5 = reader.ReadAsInt32()!.Value;
							break;
						case nameof(SaveGameHeader.EngineVersion):
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