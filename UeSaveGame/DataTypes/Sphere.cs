using System.IO;

namespace UeSaveGame.DataTypes
{
    public class Sphere
    {
        public Vector Center;
        public float Radius;

        public static Sphere Deserialize(BinaryReader reader)
        {
            Sphere m = new Sphere();

            m.Center.X = reader.ReadSingle();
            m.Center.Y = reader.ReadSingle();
            m.Center.Z = reader.ReadSingle();
            m.Radius = reader.ReadSingle();

            return m;
        }

        public long Serialize(BinaryWriter writer)
        {
            writer.Write(Center.X);
            writer.Write(Center.Y);
            writer.Write(Center.Z);
            writer.Write(Radius);

            return 16;
        }

        public override string ToString()
        {
            return $"Center = {Center}, Radius = {Radius}";
        }
    }
}
