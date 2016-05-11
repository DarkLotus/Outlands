using System;
using Server;

namespace Server.Items
{
	public class LethalPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.Lethal; } }

		public override double MinPoisoningSkill{ get{ return 100; } }
		public override double MaxPoisoningSkill{ get{ return 120; } }

		[Constructable]
		public LethalPoisonPotion() : base( PotionEffect.PoisonDeadly )
		{
            Name = "Lethal Poison potion";
		}

		public LethalPoisonPotion( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}