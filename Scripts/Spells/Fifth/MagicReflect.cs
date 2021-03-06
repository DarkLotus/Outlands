using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Fifth
{
	public class MagicReflectSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Magic Reflection", "In Jux Sanct",
				242,
				9012,
				Reagent.Garlic,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public MagicReflectSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			return true;
		}

		private static Hashtable m_Table = new Hashtable();

        public static bool IsUnderProtection(Mobile m)
        {
            object[] mods = (object[])m_Table[m];
            return mods != null;
        }
        public static void Dispel(Mobile m)
        {
            m_Table.Remove(m);

            ResistanceMod[] mods = (ResistanceMod[])m_Table[m];
            for (int i = 0; i < mods.Length; ++i)
                m.RemoveResistanceMod(mods[i]);
        }
		public override void OnCast()
        {
            #region AOS - NOT USED
            if (Core.AOS)
            {
                /* The magic reflection spell decreases the caster's physical resistance, while increasing the caster's elemental resistances.
                 * Physical decrease = 25 - (Inscription/20).
                 * Elemental resistance = +10 (-20 physical, +10 elemental at GM Inscription)
                 * The magic reflection spell has an indefinite duration, becoming active when cast, and deactivated when re-cast.
                 * Reactive Armor, Protection, and Magic Reflection will stay on—even after logging out, even after dying—until you “turn them off” by casting them again. 
                 */

                if (CheckSequence())
                {
                    Mobile targ = Caster;

                    ResistanceMod[] mods = (ResistanceMod[])m_Table[targ];

                    if (mods == null)
                    {
                        targ.PlaySound(0x1E9);
                        targ.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);

                        //Removed by IPY
                        //int physiMod = -25 + (int)(targ.Skills[SkillName.Inscribe].Value / 20);
                        //int otherMod = 10;

                        mods = new ResistanceMod[5]
					 {
						 new ResistanceMod( ResistanceType.Physical, -25 + (int)(targ.Skills[SkillName.Inscribe].Value / 20) ),
						 new ResistanceMod( ResistanceType.Fire, 10 ),
						 new ResistanceMod( ResistanceType.Cold, 10 ),
						 new ResistanceMod( ResistanceType.Poison, 10 ),
						 new ResistanceMod( ResistanceType.Energy, 10 )
					 };

                        m_Table[targ] = mods;

                        for (int i = 0; i < mods.Length; ++i)
                            targ.AddResistanceMod(mods[i]);
                        //Removed by IPY
                        //string buffFormat = String.Format( "{0}\t+{1}\t+{1}\t+{1}\t+{1}", physiMod, otherMod );

                        //BuffInfo.AddBuff( targ, new BuffInfo( BuffIcon.MagicReflection, 1075817, buffFormat, true ) );
                    }
                    else
                    {
                        targ.PlaySound(0x1ED);
                        targ.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);

                        m_Table.Remove(targ);

                        for (int i = 0; i < mods.Length; ++i)
                            targ.RemoveResistanceMod(mods[i]);

                        //Removed by IPY
                        //BuffInfo.RemoveBuff( targ, BuffIcon.MagicReflection );
                    }
                }

                FinishSequence();
            }
            #endregion
            
            else
            {
                BaseCreature casterCreature = Caster as BaseCreature;

                if (casterCreature != null)
                {
                    if (casterCreature.SpellTarget != null)
                    {
                        this.Target(casterCreature.SpellTarget);
                    }
                }

                else
                {
                    Caster.Target = new InternalTarget(this);
                }
            }
		}

        public void Target(Mobile m)
        {
            if (!m.CanSee(m) || m.Hidden)
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }

            else if ( m.MagicDamageAbsorb > 0 )
            {
                Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
			}

			else if ( !m.CanBeginAction( typeof( DefensiveSpell ) ))
			{
				Caster.SendMessage( "The spell will not adhere to them at this time." ); // The spell will not adhere to you at this time.
			}

            else if (CheckBSequence(m))
            {
                if (Caster.MagicDamageAbsorb >= 0)
                {
                    int value = 0;                    
                                       
                    bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Wizard, false, true);

                    //Spirit Speak Bonus
                    int spiritSpeakBonus = (int)(Math.Floor(8 * Caster.Skills[SkillName.SpiritSpeak].Value / 100));

                    value += spiritSpeakBonus;

                    if (enhancedSpellcast)
                        value += 8;

                    int spellHue = Enhancements.GetMobileSpellHue(Caster, Enhancements.SpellType.MagicReflect);      

                    if (enhancedSpellcast)
                    {
                        m.FixedParticles(0x375A, 10, 30, 5037, spellHue, 0, EffectLayer.Waist);
                        m.PlaySound(0x1E9);
                    }

                    else
                    {
                        m.FixedParticles(0x375A, 10, 15, 5037, spellHue, 0, EffectLayer.Waist);
                        m.PlaySound(0x1E9);
                    }

                    if (value < 1)
                        value = 1;

                    m.MagicDamageAbsorb = value;
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private MagicReflectSpell m_Owner;

            public InternalTarget(MagicReflectSpell owner)
                : base(12, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
	}
}