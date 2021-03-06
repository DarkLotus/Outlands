using System;

namespace Server.Items
{
    public class SheafOfWheat : Item
    {
        [Constructable]
        public SheafOfWheat(): base(7869)
        {
            Name = "sheaf of wheat";

            Weight = 5;
        }

        public SheafOfWheat(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}