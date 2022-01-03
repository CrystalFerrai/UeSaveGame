namespace UeSaveGame.DataTypes
{
    public struct UDateTime
    {
        // See FDateTime in DateTime.h (UE4) for helpers to interpret ticks.

        public long Ticks { get; set; }

        public override string ToString()
        {
            return Ticks.ToString();
        }
    }
}
