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

using System.Diagnostics.CodeAnalysis;

namespace UeSaveGame
{
	/// <summary>
	/// Implement this if you need custom serialization for a save class. Attach a [SaveClassPath] attribute
	/// with the exact path of the custom save path as it appears in the save game.
	/// </summary>
	public abstract class SaveClassBase
	{
		/// <summary>
		/// If true, will call DeserializeHeader or SerializeHeader before performing data serialization
		/// </summary>
		public virtual bool HasCustomHeader { get; } = false;

		/// <summary>
		/// If true, will call DeserializeData and SerializeData instead of performing standard serialization
		/// </summary>
		public virtual bool HasCustomData { get; } = false;

		/// <summary>
		/// Return the size of the custom header, in bytes. Called if HasCustomHeader = true
		/// </summary>
		/// <remarks>
		/// This is called before serializing data in order to allocate sufficient space in the save game.
		/// SerializeHeader will later be called with the writer positioned at the start of the header.
		/// The size returned must match the amount of data written by SerializeHeader.
		/// </remarks>
		public virtual long GetHeaderSize()
		{
			return 0;
		}

		/// <summary>
		/// Called before deserializing data, only if HasCustomHeader = true
		/// </summary>
		/// <param name="reader">A reader positioned at the start of the header</param>
		public virtual void DeserializeHeader(BinaryReader reader)
		{
		}

		/// <summary>
		/// Called after serializing data, only if HasCustomHeader = true
		/// </summary>
		/// <param name="writer">A writer positioned at the start of the header</param>
		/// <param name="dataLength">The size of the serialized data, in bytes, not including the header</param>
		/// <remarks>
		/// This is called after data has been serialized. This function must write the same number of bytes
		/// as was returned by GetHeaderSize.
		/// </remarks>
		public virtual void SerializeHeader(BinaryWriter writer, long dataLength)
		{
		}

		/// <summary>
		/// Called instead of normal data deserialization, only if HasCustomData = true
		/// </summary>
		/// <param name="reader">A reader positioned at the start of the data</param>
		public virtual void DeserializeData(BinaryReader reader)
		{
		}

		/// <summary>
		/// Called instead of normal data serialization, only if HasCustomData = true
		/// </summary>
		/// <param name="writer">A writer positioned at the start of the data</param>
		/// <returns>The size of the serialized data, in bytes</returns>
		public virtual long SerializeData(BinaryWriter writer)
		{
			return 0;
		}
	}

	/// <summary>
	/// Apply this attribute to implementations of SaveClassBase
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class SaveClassPathAttribute : Attribute
	{
		/// <summary>
		/// The exact path of the save class, as it appears in the save game
		/// </summary>
		public string ClassPath { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="classPath">The exact path of the save class, as it appears in the save game</param>
		public SaveClassPathAttribute(string classPath)
		{
			if (classPath is null) throw new ArgumentNullException(nameof(classPath));
			if (classPath.Length == 0) throw new ArgumentException($"{nameof(classPath)} value cannot be empty", nameof(classPath));

			ClassPath = classPath;
		}
	}
}
