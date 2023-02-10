using System;

namespace UeSaveGame.DataTypes
{
    public struct UDateTime : IFormattable
    {
		private DateTime mDateTime;

        public static UDateTime Now => new UDateTime(DateTime.Now);

        public static UDateTime UtcNow => new UDateTime(DateTime.UtcNow);

        public DateTime Value
        {
            get => mDateTime;
            set => mDateTime = value;
        }

        public long Ticks
		{
			get => mDateTime.Ticks;
			set => mDateTime = new DateTime(value);
		}

        public UDateTime(long ticks)
		{
            mDateTime = new DateTime(ticks);
		}

        public UDateTime(DateTime value)
		{
            mDateTime = value;
		}

        public override string ToString()
        {
            return mDateTime.ToString();
        }

        public string ToString(string format)
		{
            return mDateTime.ToString(format);
        }

        public string ToString(IFormatProvider provider)
        {
            return mDateTime.ToString(provider);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return mDateTime.ToString(format, provider);
        }
    }
}
