using System;
using Server.Items;

namespace Server.Items
{
	public class PlateGorget : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.PlatemailBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.PlatemailMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.PlatemailDurability; } }
        public override int InitMaxHits { get { return ArmorValues.PlatemailDurability; } }

        public override int IconItemId { get { return 5139; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 56; } }
        public override int IconOffsetY { get { return 42; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

		[Constructable]
		public PlateGorget() : base( 5139 )
		{
            Name = "platemail gorget";
			Weight = 2.0;
		}

		public PlateGorget( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}