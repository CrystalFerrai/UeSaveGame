namespace UeSaveGame.DataTypes
{
    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public override string ToString()
        {
            return $"{R:x2}{G:x2}{B:x2}{A:x2}";
        }
    }
}
