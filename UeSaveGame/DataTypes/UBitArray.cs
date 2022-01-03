using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UeSaveGame.DataTypes
{
    public class UBitArray : IList<byte>, IReadOnlyList<byte>
    {
        private BitArray mArray;

        public int Count { get; private set; }

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
                return (byte)(mArray[index] ? 1 : 0);
            }
            set
            {
                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
                mArray[index] = value != 0;
            }
        }

        private UBitArray()
        {
        }

        public static UBitArray Deserialize(BinaryReader reader)
        {
            UBitArray instance = new UBitArray();

            instance.Count = reader.ReadInt32();
            int[] values = new int[(int)Math.Ceiling(instance.Count / 32.0f)];
            for (int i = 0; i < values.Length; ++i)
            {
                values[i] = reader.ReadInt32();
            }
            instance.mArray = new BitArray(values);

            return instance;
        }

        public long Serialize(BinaryWriter writer)
        {
            writer.Write(Count);

            int intCount = (int)Math.Ceiling(Count / 32.0f);

            byte[] bytes = new byte[intCount * 4];
            mArray.CopyTo(bytes, 0);

            for (int i = 0; i < intCount; ++i)
            {
                writer.Write(BitConverter.ToInt32(bytes, i * 4));
            }

            return 4 + intCount * 4;
        }

        public override string ToString()
        {
            return $"Count = {Count}";
        }

        #region Interfaces
        bool ICollection<byte>.IsReadOnly => false;

        byte IReadOnlyList<byte>.this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
                return (byte)(mArray[index] ? 1 : 0);
            }
        }

        public IEnumerator<byte> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return (byte)(mArray[i] ? 1 : 0);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return (byte)(mArray[i] ? 1 : 0);
            }
        }

        public void CopyTo(byte[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; ++i)
            {
                array[i] = (byte)(mArray[i] ? 1 : 0);
            }
        }

        public int IndexOf(byte item)
        {
            throw new NotSupportedException();
        }

        public void Insert(int index, byte item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public void Add(byte item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(byte item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(byte item)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
