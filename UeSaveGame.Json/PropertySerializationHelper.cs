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
using System.Numerics;
using System.Reflection;

namespace UeSaveGame.Json
{
	/// <summary>
	/// Helper methods for reading and writing properties as json
	/// </summary>
	internal static class PropertySerializationHelper
	{
		private static readonly MethodInfo sMoveToContentMethod;

		static PropertySerializationHelper()
		{
			sMoveToContentMethod = typeof(JsonReader).GetMethod("MoveToContent", BindingFlags.NonPublic | BindingFlags.Instance)!;
		}

		/// <summary>
		/// Read the next token from the JsonReader as an FString
		/// </summary>
		/// <param name="reader">The reader to read from</param>
		public static FString? ReadAsFString(this JsonReader reader)
		{
			string? value = reader.ReadAsString();
			if (value is null)
			{
				return null;
			}
			return new(value);
		}

		/// <summary>
		/// Get the current token from the JsonReader as an FString
		/// </summary>
		/// <param name="reader">The reader to read from</param>
		public static FString? ValueAsFString(this JsonReader reader)
		{
			string? value = reader.Value as string;
			if (value is null)
			{
				return null;
			}
			return new(value);
		}

		/// <summary>
		/// Write an FString to the JsonWriter as a string
		/// </summary>
		/// <param name="writer">The writer to write to</param>
		/// <param name="value">The value to write</param>
		public static void WriteFStringValue(this JsonWriter writer, FString? value)
		{
			if (value is null)
			{
				writer.WriteNull();
			}
			else
			{
				writer.WriteValue(value.Value);
			}
		}

		/// <summary>
		/// Get the current token from the JsonReader as an integer
		/// </summary>
		/// <param name="reader">The reader to read from</param>
		public static long ValueAsInteger(this JsonReader reader)
		{
			switch (reader.TokenType)
			{
				case JsonToken.Integer:
					{
						if (reader.Value is int i)
						{
							return i;
						}
						if (reader.Value is long l)
						{
							return l;
						}
						if (reader.Value is BigInteger bi)
						{
							return (long)bi;
						}
					}
					break;
				case JsonToken.Float:
					{
						if (reader.Value is float f)
						{
							return (long)f;
						}
						if (reader.Value is double d)
						{
							return (long)d;
						}
					}
					break;
				case JsonToken.String:
					{
						if (reader.Value is string s)
						{
							long value;
							if (long.TryParse(s, out value))
							{
								return value;
							}
						}
					}
					break;
			}

			try
			{
				return Convert.ToInt64(reader.Value);
			}
			catch
			{
			}

			return 0;
		}

		/// <summary>
		/// Get the current token from the JsonReader as a floating point number
		/// </summary>
		/// <param name="reader">The reader to read from</param>
		public static double ValueAsFloat(this JsonReader reader)
		{
			switch (reader.TokenType)
			{
				case JsonToken.Integer:
					{
						if (reader.Value is int i)
						{
							return i;
						}
						if (reader.Value is long l)
						{
							return l;
						}
						if (reader.Value is BigInteger bi)
						{
							return (long)bi;
						}
					}
					break;
				case JsonToken.Float:
					{
						if (reader.Value is float f)
						{
							return f;
						}
						if (reader.Value is double d)
						{
							return d;
						}
					}
					break;
				case JsonToken.String:
					{
						if (reader.Value is string s)
						{
							double value;
							if (double.TryParse(s, out value))
							{
								return value;
							}
						}
					}
					break;
			}

			try
			{
				return Convert.ToDouble(reader.Value);
			}
			catch
			{
			}

			return 0.0;
		}

		/// <summary>
		/// Read the next token from the JsonReader as an enum
		/// </summary>
		/// <typeparam name="TEnum">The type of the enum to read</typeparam>
		/// <param name="reader">The reader to read from</param>
		public static TEnum ReadAsEnum<TEnum>(this JsonReader reader) where TEnum : struct, Enum
		{
			reader.ReadAndMoveToContent();
			return ValueAsEnum<TEnum>(reader);
		}

		/// <summary>
		/// Get the current token from the JsonReader as an enum
		/// </summary>
		/// <typeparam name="TEnum">The type of the enum to read</typeparam>
		/// <param name="reader">The reader to read from</param>
		public static TEnum ValueAsEnum<TEnum>(this JsonReader reader) where TEnum : struct, Enum
		{
			switch (reader.TokenType)
			{
				case JsonToken.Integer:
				case JsonToken.Float:
					{
						long value = ValueAsInteger(reader);
						return (TEnum)Enum.ToObject(typeof(TEnum), value);
					}
				case JsonToken.String:
					{
						string? strValue = (string?)reader.Value;
						if (Enum.TryParse(strValue, true, out TEnum value))
						{
							return value;
						}
					}
					break;
			}
			return default;
		}

		/// <summary>
		/// Read the next token from the JsonReader and continue reading until content is reached
		/// </summary>
		/// <param name="reader">The reader to advance</param>
		/// <returns>True if the reader reached content or false if the reader reached the end of the data</returns>
		public static bool ReadAndMoveToContent(this JsonReader reader)
		{
			if (!reader.Read()) return false;
			return (bool)sMoveToContentMethod.Invoke(reader, null)!;
		}
	}
}
