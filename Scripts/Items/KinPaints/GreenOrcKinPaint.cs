using System;
using Server;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class GreenOrcKinPaint : KinPaint
	{
		[Constructable]
		public GreenOrcKinPaint() : base()
		{
            Name = "green orcish kin paint";
            Hue = 2211;
			Weight = 1.0;

            PaintHue = 1439; 
		}

        public GreenOrcKinPaint(Serial serial): base(serial)
		{           
		}     

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}