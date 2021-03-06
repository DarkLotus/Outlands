﻿using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class SpookyGhoulStatue : Item
    {
        public override string DefaultName { get { return "Spooky Ghoul Statue"; } }

        [Constructable]
        public SpookyGhoulStatue()
            : base(0x2109)
        {
            Hue = 2106;
        }

        public SpookyGhoulStatue(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
