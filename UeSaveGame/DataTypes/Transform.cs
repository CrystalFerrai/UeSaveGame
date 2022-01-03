using System.IO;

namespace UeSaveGame.DataTypes
{
    public class Transform
    {
        public Quaternion Rotation;
        public Vector Translation;
        public Vector Scale;

        public static Transform Deserialize(BinaryReader reader)
        {
            Transform t = new Transform();

            t.Rotation.X = reader.ReadSingle();
            t.Rotation.Y = reader.ReadSingle();
            t.Rotation.Z = reader.ReadSingle();
            t.Rotation.W = reader.ReadSingle();

            t.Translation.X = reader.ReadSingle();
            t.Translation.Y = reader.ReadSingle();
            t.Translation.Z = reader.ReadSingle();

            t.Scale.X = reader.ReadSingle();
            t.Scale.Y = reader.ReadSingle();
            t.Scale.Z = reader.ReadSingle();

            return t;
        }

        public long Serialize(BinaryWriter writer)
        {
            writer.Write(Rotation.X);
            writer.Write(Rotation.Y);
            writer.Write(Rotation.Z);
            writer.Write(Rotation.W);

            writer.Write(Translation.X);
            writer.Write(Translation.Y);
            writer.Write(Translation.Z);

            writer.Write(Scale.X);
            writer.Write(Scale.Y);
            writer.Write(Scale.Z);

            return 40;
        }

        public override string ToString()
        {
            return $"R({Rotation}) T({Translation}) S({Scale})";
        }
    }
}
