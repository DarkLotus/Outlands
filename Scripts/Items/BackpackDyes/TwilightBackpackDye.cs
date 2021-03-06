﻿using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    class TwilightBackpackDye : BackpackDye
    {
        [Constructable]
        public TwilightBackpackDye(): base()
        {
            Name = "Twilight backpack dye";
            DyeHue = 2587;
        }

        public TwilightBackpackDye(Serial serial): base(serial)
        {
        }       

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
