namespace UeSaveGame.DataTypes
{
    public struct Vector
    {
        public float X;
        public float Y;
        public float Z;

        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }
    }
}
