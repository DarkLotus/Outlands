using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a cat corpse")]
    public class Cat : BaseCreature
    {
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return 1; }

        [Constructable]
        public Cat(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a cat";
            Body = 0xC9;
            Hue = Utility.RandomAnimalHue();
            BaseSoundID = 0x69;

            SetStr(25);
            SetDex(50);
            SetInt(25);

            SetHits(50);

            SetDamage(3, 6);

            SetSkill(SkillName.Wrestling, 25);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 200;
            Karma = 0;            

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 25;
        }

        public override string TamedDisplayName { get { return "Cat"; } }

        public override int TamedItemId { get { return 8475; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 20; } }

        public override int TamedBaseMaxHits { get { return 50; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 50; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Beastial; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.VeryFast; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.NeutralMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public Cat(Serial serial): base(serial)
        {
        }        

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}