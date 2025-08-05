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

using System.Text;

namespace UeSaveGame.Util
{
	/// <summary>
	/// Extension methods for reading/writing custom types using standard binary readers and writers
	/// </summary>
	public static class BinaryIOExtensions
	{
		/// <summary>
		/// Checks if the reader is positioned on data that can be read as a non-null FString
		/// </summary>
		/// <param name="maxLength">The maximum string length to consider valid</param>
		public static bool IsUnrealStringAndNotNull(this BinaryReader reader, int maxLength = 2048)
		{
			long bytesUntilEnd = reader.BaseStream.Length - reader.BaseStream.Position;
			if (bytesUntilEnd < 5)
			{
				return false;
			}

			bool result = false;

			long originalPosition = reader.BaseStream.Position;

			int length = reader.ReadInt32();
			int absLength = Math.Abs(length);
			if (absLength > 0 && absLength < maxLength)
			{
				if (length < 0)
				{
					int byteCount = -length * 2;
					if (byteCount <= bytesUntilEnd)
					{
						byte[] data = reader.ReadBytes(byteCount);
						if (data.Length > 1 && data[^2] == 0 && data[^1] == 0)
						{
							result = true;
						}
					}
				}
				else
				{
					if (length <= bytesUntilEnd)
					{
						byte[] data = reader.ReadBytes(length);
						if (data[^1] == 0)
						{
							result = true;
						}
					}
				}
			}

			reader.BaseStream.Seek(originalPosition, SeekOrigin.Begin);

			return result;
		}

		public static FString? ReadUnrealString(this BinaryReader reader, int maxLength = 2048)
		{
			int length = reader.ReadInt32();
			switch (length)
			{
				case 0:
					return null;
				case 1:
					if (reader.ReadByte() != 0)
					{
						throw new InvalidOperationException("String is missing null terminator. This is not a valid string.");
					}
					return FString.Empty;
				default:
					if (length < 0)
					{
						int count = -length;
						if (count > maxLength)
						{
							throw new InvalidOperationException($"String length {count} exceeds the maximum passed in string length {maxLength}");
						}
						if (count * 2 > reader.BaseStream.Length - reader.BaseStream.Position)
						{
							throw new InvalidOperationException($"Attempting to read a string of length {count} would read beyond the end of the stream.");
						}
						byte[] data = reader.ReadBytes(count * 2);
						if (data[^1] != 0 || data[^2] != 0)
						{
							throw new InvalidOperationException("String is missing null terminator. This is not a valid string.");
						}
						return new FString(Encoding.Unicode.GetString(data, 0, data.Length - 2), Encoding.Unicode);
					}
					else
					{
						if (length > maxLength)
						{
							throw new InvalidOperationException($"String length {length} exceeds the maximum passed in string length {maxLength}");
						}
						if (length > reader.BaseStream.Length - reader.BaseStream.Position)
						{
							throw new InvalidOperationException($"Attempting to read a string of length {length} would read beyond the end of the stream.");
						}
						byte[] data = reader.ReadBytes(length);
						if (data[^1] != 0)
						{
							throw new InvalidOperationException("String is missing null terminator. This is not a valid string.");
						}
						return new FString(Encoding.ASCII.GetString(data, 0, data.Length - 1), Encoding.ASCII);
					}
			}
		}

		public static void WriteUnrealString(this BinaryWriter writer, FString? value)
		{
			if (value is null || value.Value is null)
			{
				writer.Write(0);
				return;
			}

			int len = value.Length + 1;
			if (value.Encoding.GetByteCount("A") > 1) len = -len;
			writer.Write(len);

			byte[] data = value.Encoding.GetBytes(value.Value);
			if (data.Length > 0)
			{
				writer.Write(data);
			}
			writer.Write(value.Encoding.GetBytes("\0"));
		}
	}
}
