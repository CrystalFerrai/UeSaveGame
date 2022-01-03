using System;
using System.Text;

namespace UeSaveGame.DataTypes
{
    public class UString : IComparable, IComparable<UString>, ICloneable, IEquatable<UString>, IEquatable<string>
    {
        public static UString Empty { get; } = new UString(string.Empty, Encoding.ASCII);

        public string Value { get; }

        public int Length => Value.Length;

        public Encoding Encoding { get; }

        public int SizeInBytes => Encoding.GetByteCount(Value) + Encoding.GetByteCount("\0");

        public UString()
        {
            Value = null;
            Encoding = Encoding.ASCII;
        }

        public UString(string value, Encoding encoding)
        {
            Value = value;
            Encoding = encoding;
        }

        public UString(UString other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));

            Value = other.Value;
            Encoding = other.Encoding;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash += Value.GetHashCode() * 17;
            hash += Encoding.GetHashCode() * 17;
            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is UString other && Equals(other) || obj is string other2 && Equals(other2);
        }

        public bool Equals(UString other)
        {
            return !(other is null) && Encoding.Equals(other.Encoding) && Value.Equals(other.Value);
        }

        public bool Equals(string other)
        {
            return !(other is null) && Value == other;
        }

        public static bool operator==(UString a, UString b)
        {
            return a?.Equals(b) ?? b is null;
        }

        public static bool operator!=(UString a, UString b)
        {
            return !(a == b);
        }

        public static bool operator ==(UString a, string b)
        {
            return a?.Value?.Equals(b) ?? b is null;
        }

        public static bool operator !=(UString a, string b)
        {
            return !(a == b);
        }

        public static bool operator ==(string a, UString b)
        {
            return b?.Value?.Equals(a) ?? a is null;
        }

        public static bool operator !=(string a, UString b)
        {
            return !(a == b);
        }

        public static implicit operator string(UString instance)
        {
            return instance.Value;
        }

        public override string ToString()
        {
            return Value;
        }

        int IComparable.CompareTo(object obj)
        {
            return obj is UString other ? Value.CompareTo(other.Value) : -1;
        }

        public int CompareTo(UString other)
        {
            return Value.CompareTo(other.Value);
        }

        public object Clone()
        {
            return new UString(this);
        }
    }
}
