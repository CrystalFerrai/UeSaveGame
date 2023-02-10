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

using System.Collections;
using System.Text;

namespace UeSaveGame
{
    /// <summary>
    /// Represents an Unreal engine string type
    /// </summary>
    /// <remarks>
    /// In many ways, this can be treated the same as a string. You can also get a string representation via the <see cref="Value"/> property.
    /// </remarks>
	public class FString : IComparable, IComparable<FString>, ICloneable, IEquatable<FString>, IEquatable<string>, IEnumerable<char>, IEnumerable
    {
        private readonly ulong mHashSeed;

        public static FString Empty { get; } = new FString(string.Empty, Encoding.ASCII, 0);

        public string Value { get; }

        public int Length => Value?.Length ?? 0;

        public Encoding Encoding { get; }

        public int SizeInBytes => Encoding.GetByteCount(Value) + Encoding.GetByteCount("\0");

        public char this[int index]
        {
            get => Value[index];
        }

        public FString this[Range range]
        {
            get => new FString(Value[range], Encoding, mHashSeed);
        }

        public FString(string value, ulong hashSeed = 0)
        {
            mHashSeed = hashSeed;
            Value = value;
            Encoding = DetectEncoding(value);
        }

        public FString(string value, Encoding encoding, ulong hashSeed = 0)
        {
            mHashSeed = hashSeed;
            Value = value;
            Encoding = encoding;
        }

        public FString(FString other)
        {
            mHashSeed = other.mHashSeed;
            Value = other.Value;
            Encoding = other.Encoding;
        }

        public override int GetHashCode()
        {
            return (int)(uint)GetFullHash();
        }

        public ulong GetFullHash()
        {
            // Reference: UE4 Fnv.cpp - FFnv::MemFnv64

            const ulong Offset = 0xcbf29ce484222325;
            const ulong Prime = 0x00000100000001b3;

            string lower = Value.ToLowerInvariant();

            ulong Fnv = Offset + mHashSeed;
            for (int i = 0; i < Length; ++i)
            {
                Fnv ^= (byte)lower[i];
                Fnv *= Prime;
                Fnv ^= (byte)(lower[i] >> 8);
                Fnv *= Prime;
            }

            return Fnv;
        }

        public override bool Equals(object? obj)
        {
            return obj is FString other && Equals(other) || obj is string other2 && Equals(other2);
        }

        public bool Equals(FString? other)
        {
            return !(other is null) && Encoding.Equals(other.Encoding) && Value.Equals(other.Value);
        }

        public bool Equals(string? other)
        {
            return !(other is null) && Value == other;
        }

        public static bool operator ==(FString? a, FString? b)
        {
            return a?.Equals(b) ?? b is null;
        }

        public static bool operator !=(FString? a, FString? b)
        {
            return !(a == b);
        }

        public static bool operator ==(FString? a, string? b)
        {
            return a?.Value?.Equals(b) ?? b is null;
        }

        public static bool operator !=(FString? a, string? b)
        {
            return !(a == b);
        }

        public static bool operator ==(string? a, FString? b)
        {
            return b?.Value?.Equals(a) ?? a is null;
        }

        public static bool operator !=(string? a, FString? b)
        {
            return !(a == b);
        }

        public static FString operator +(FString a, FString b)
        {
            Encoding encoding = a.Encoding == Encoding.Unicode || b.Encoding == Encoding.Unicode ? Encoding.Unicode : Encoding.ASCII;
            return new FString(a.Value + b.Value, encoding, a.mHashSeed);
        }

        public static FString operator +(FString a, string b)
        {
            Encoding encoding = a.Encoding == Encoding.Unicode || DetectEncoding(b) == Encoding.Unicode ? Encoding.Unicode : Encoding.ASCII;
            return new FString(a.Value + b, encoding, a.mHashSeed);
        }

        public static FString operator +(string a, FString b)
        {
            Encoding encoding = DetectEncoding(a) == Encoding.Unicode || b.Encoding == Encoding.Unicode ? Encoding.Unicode : Encoding.ASCII;
            return new FString(a + b.Value, encoding, b.mHashSeed);
        }

        public static implicit operator string(FString instance)
        {
            return instance.Value;
        }

        public static explicit operator FString(string instance)
        {
            return new FString(instance);
        }

        public override string ToString()
        {
            return Value;
        }

        int IComparable.CompareTo(object? obj)
        {
            return obj is FString other ? Value.CompareTo(other.Value) : -1;
        }

        public int CompareTo(FString? other)
        {
            return Value.CompareTo(other?.Value);
        }

        public object Clone()
        {
            return new FString(this);
        }

        public IEnumerator<char> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static Encoding DetectEncoding(string value)
        {
            return Encoding.UTF8.GetByteCount(value) == value.Length ? Encoding.ASCII : Encoding.Unicode;
        }
    }
}
