using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13f0, 0x13f1 )]
	public class RingmailLegs : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.RingmailBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.RingmailMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.RingmailDurability; } }
        public override int InitMaxHits { get { return ArmorValues.RingmailDurability; } }

        public override int IconItemId { get { return 5105; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 50; } }
        public override int IconOffsetY { get { return 35; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

		[Constructable]
		public RingmailLegs() : base( 5105 )
		{
            Name = "ringmail leggings";
			Weight = 5.0;
		}

		public RingmailLegs( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            if (Weight == 15.0)
                Weight = 5.0;
		}
	}
}