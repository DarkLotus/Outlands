using System;
using Server;

namespace Server.Items
{
    [FlipableAttribute(5449, 5450)]
	public class AncestorMask : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.LeatherBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.LeatherMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.LeatherDurability; } }
        public override int InitMaxHits { get { return ArmorValues.LeatherDurability; } }

        public override int IconItemId { get { return 5449; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 0; } }
        public override int IconOffsetY { get { return 0; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Leather; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

		[Constructable]
        public AncestorMask(): base(5449)
		{
            Name = "ancestor mask";
			Weight = 2.0;

            LootType = LootType.Blessed;

            
		}

		public AncestorMask( Serial serial ) : base( serial )
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
		}
	}
}