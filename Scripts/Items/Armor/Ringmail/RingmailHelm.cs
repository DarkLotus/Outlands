using System;
using Server;

namespace Server.Items
{
	public class RingmailHelm : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.RingmailBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.RingmailMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.RingmailDurability; } }
        public override int InitMaxHits { get { return ArmorValues.RingmailDurability; } }

        public override int IconItemId { get { return 5131; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 53; } }
        public override int IconOffsetY { get { return 41; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

		[Constructable]
		public RingmailHelm() : base( 5131 )
		{
            Name = "ringmail helm";
            Hue = 0; //1812

			Weight = 2.0;
		}

		public RingmailHelm( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}