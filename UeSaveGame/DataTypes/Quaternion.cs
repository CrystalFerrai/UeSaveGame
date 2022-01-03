namespace UeSaveGame.DataTypes
{
    public struct Quaternion
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public override string ToString()
        {
            return $"{X} {Y} {Z} {W}";
        }
    }
}
