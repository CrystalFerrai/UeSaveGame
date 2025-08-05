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
using UeSaveGame.DataTypes;
using UeSaveGame.TextData;

namespace UeSaveGame.Json.TextDataSerializers
{
	internal class TextDataSerializer_ArgumentFormat : ITextDataSerializer
	{
		public void ToJson(ITextData? data, JsonWriter writer)
		{
			if (data is null)
			{
				writer.WriteNull();
				return;
			}

			if (data is not TextData_ArgumentFormat) throw new ArgumentException($"{nameof(TextDataSerializer_ArgumentFormat)} does not support data type {data.GetType().Name}", nameof(data));

			TextData_ArgumentFormat textData = (TextData_ArgumentFormat)data;

			writer.WriteStartObject();

			writer.WritePropertyName(nameof(TextData_ArgumentFormat.FormatString));
			TextSerializer.ToJson(textData.FormatString, writer);

			writer.WritePropertyName(nameof(TextData_ArgumentFormat.Arguments));
			writer.WriteStartArray();
			if (textData.Arguments is not null)
			{
				foreach (TextArgument argument in textData.Arguments)
				{
					writer.WriteStartObject();

					writer.WritePropertyName(nameof(TextArgument.Name));
					writer.WriteFStringValue(argument.Name);

					writer.WritePropertyName(nameof(TextArgument.Type));
					writer.WriteValue((int)argument.Type);

					writer.WritePropertyName(nameof(TextArgument.Value));
					switch (argument.Type)
					{
						case EFormatArgumentType.Int:
						case EFormatArgumentType.UInt:
						case EFormatArgumentType.Float:
						case EFormatArgumentType.Double:
							writer.WriteValue(argument.Value);
							break;
						case EFormatArgumentType.Text:
							TextSerializer.ToJson((FText)argument.Value, writer);
							break;
						case EFormatArgumentType.Gender:
							writer.WriteValue((byte)(ETextGender)argument.Value);
							break;
					}

					writer.WriteEndObject();
				}
			}
			writer.WriteEndArray();

			writer.WriteEndObject();
		}

		public ITextData? FromJson(JsonReader reader)
		{
			TextData_ArgumentFormat textData = new();

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.EndObject)
				{
					break;
				}

				if (reader.TokenType == JsonToken.PropertyName)
				{
					switch (reader.Value)
					{
						case nameof(TextData_ArgumentFormat.FormatString):
							textData.FormatString = TextSerializer.FromJson(reader);
							break;
						case nameof(TextData_ArgumentFormat.Arguments):
							{
								List<TextArgument> arguments = new();
								while (reader.Read())
								{
									if (reader.TokenType == JsonToken.EndArray)
									{
										break;
									}

									if (reader.TokenType == JsonToken.StartObject)
									{
										TextArgument argument = new();
										JToken? argumentValue = null;
										while (reader.Read())
										{
											if (reader.TokenType == JsonToken.EndObject)
											{
												break;
											}

											if (reader.TokenType == JsonToken.PropertyName)
											{
												switch (reader.Value)
												{
													case nameof(TextArgument.Name):
														argument.Name = reader.ReadAsFString();
														break;
													case nameof(TextArgument.Type):
														argument.Type = (EFormatArgumentType)reader.ReadAsInt32()!;
														break;
													case nameof(TextArgument.Value):
														if (reader.ReadAndMoveToContent())
														{
															argumentValue = JToken.ReadFrom(reader);
														}
														break;
												}
											}
										}

										if (argumentValue is not null)
										{
											switch (argument.Type)
											{
												case EFormatArgumentType.Int:
													argument.Value = (int)argumentValue;
													break;
												case EFormatArgumentType.UInt:
													argument.Value = (uint)argumentValue;
													break;
												case EFormatArgumentType.Float:
													argument.Value = (float)argumentValue;
													break;
												case EFormatArgumentType.Double:
													argument.Value = (double)argumentValue;
													break;
												case EFormatArgumentType.Text:
													argument.Value = TextSerializer.FromJson(argumentValue.CreateReader());
													break;
												case EFormatArgumentType.Gender:
													argument.Value = (ETextGender)(int)argumentValue;
													break;
											}
										}

										arguments.Add(argument);
									}

									textData.Arguments = arguments.ToArray();
								}
							}
							break;
					}
				}
			}

			return textData;
		}
	}
}
