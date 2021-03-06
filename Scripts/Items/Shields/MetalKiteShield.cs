using System;
using Server;

namespace Server.Items
{
    public class MetalKiteShield : BaseShield, IDyable
    {
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.MetalKiteShieldArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.MetalKiteShieldMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.MetalKiteShieldDurability; } }
        public override int InitMaxHits { get { return ArmorValues.MetalKiteShieldDurability; } }

        public override int IconItemId { get { return 7029; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 55; } }
        public override int IconOffsetY { get { return 32; } }

        [Constructable]
        public MetalKiteShield(): base(7029)
        {
            Name = "metal kite shield";
            Weight = 6.0;
        }

        public MetalKiteShield(Serial serial): base(serial)
        {
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Weight == 5.0)
                Weight = 7.0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}
