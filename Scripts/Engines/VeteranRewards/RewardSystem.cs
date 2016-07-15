using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Accounting;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engines.VeteranRewards
{
	public class RewardSystem
	{
		private static RewardCategory[] m_Categories;
		private static RewardList[] m_Lists;

		public static RewardCategory[] Categories
		{
			get
			{
				if ( m_Categories == null )
					SetupRewardTables();

				return m_Categories;
			}
		}

		public static RewardList[] Lists
		{
			get
			{
				if ( m_Lists == null )
					SetupRewardTables();

				return m_Lists;
			}
		}
		// Az - Vet rewards disabled (hopefully?)
		public static bool Enabled = false; // change to true to enable vet rewards
		public static bool SkillCapRewards = false; // assuming vet rewards are enabled, should total skill cap bonuses be awarded? (720 skills total at 4th level)
		public static TimeSpan RewardInterval = TimeSpan.FromDays( 30.0 );

		public static bool HasAccess( Mobile mob, RewardCategory category )
		{
			List<RewardEntry> entries = category.Entries;

			for ( int j = 0; j < entries.Count; ++j )
			{
				//RewardEntry entry = entries[j];
				if ( RewardSystem.HasAccess( mob, entries[j] ) )
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasAccess( Mobile mob, RewardEntry entry )
		{
			if ( Core.Expansion < entry.RequiredExpansion )
			{
				return false;
			}

			TimeSpan ts;
			return HasAccess( mob, entry.List, out ts );
		}

		public static bool HasAccess( Mobile mob, RewardList list, out TimeSpan ts )
		{
			if ( list == null )
			{
				ts = TimeSpan.Zero;
				return false;
			}

			Account acct = mob.Account as Account;

			if ( acct == null )
			{
				ts = TimeSpan.Zero;
				return false;
			}

			TimeSpan totalTime = (DateTime.UtcNow - acct.Created);

			ts = ( list.Age - totalTime );

			if ( ts <= TimeSpan.Zero )
				return true;

			return false;
		}

		public static int GetRewardLevel( Mobile mob )
		{
			Account acct = mob.Account as Account;

			if ( acct == null )
				return 0;

			return GetRewardLevel( acct );
		}

		public static int GetRewardLevel( Account acct )
		{
			TimeSpan totalTime = (DateTime.UtcNow - acct.Created);

			int level = (int)(totalTime.TotalDays / RewardInterval.TotalDays);

			if ( level < 0 )
				level = 0;

			return level;
		}

		public static bool HasHalfLevel( Mobile mob )
		{
			Account acct = mob.Account as Account;

			if ( acct == null )
				return false;

			return HasHalfLevel( acct );
		}

		public static bool HasHalfLevel( Account acct )
		{
			TimeSpan totalTime = (DateTime.UtcNow - acct.Created);

			Double level = (totalTime.TotalDays / RewardInterval.TotalDays);

			return level >= 0.5;
		}

		public static bool ConsumeRewardPoint( Mobile mob )
		{
			int cur, max;

			ComputeRewardInfo( mob, out cur, out max );

			if ( cur >= max )
				return false;

			Account acct = mob.Account as Account;

			if ( acct == null )
				return false;

			//if ( mob.AccessLevel < AccessLevel.GameMaster )
				acct.SetTag( "numRewardsChosen", (cur + 1).ToString() );

			return true;
		}

		public static void ComputeRewardInfo( Mobile mob, out int cur, out int max )
		{
			int level;

			ComputeRewardInfo( mob, out cur, out max, out level );
		}

		public static void ComputeRewardInfo( Mobile mob, out int cur, out int max, out int level )
		{
			Account acct = mob.Account as Account;

			if ( acct == null )
			{
				cur = max = level = 0;
				return;
			}

			level = GetRewardLevel( acct );

			if ( level == 0 )
			{
				cur = max = 0;
				return;
			}

			string tag = acct.GetTag( "numRewardsChosen" );

			if ( String.IsNullOrEmpty( tag ) )
				cur = 0;
			else
				cur = Utility.ToInt32( tag );

			if ( level >= 6 )
				max = 9 + ((level - 6) * 2);
			else
				max = 2 + level;
		}

		public static bool CheckIsUsableBy( Mobile from, Item item, object[] args )
		{
			if ( m_Lists == null )
				SetupRewardTables();

			bool isRelaxedRules = ( item is DyeTub || item is MonsterStatuette );

			Type type = item.GetType();

			for ( int i = 0; i < m_Lists.Length; ++i )
			{
				RewardList list = m_Lists[i];
				RewardEntry[] entries = list.Entries;
				TimeSpan ts;

				for ( int j = 0; j < entries.Length; ++j )
				{
					if ( entries[j].ItemType == type )
					{
						if ( args == null && entries[j].Args.Length == 0 )
						{
							if ( (!isRelaxedRules || i > 0) && !HasAccess( from, list, out ts ) )
							{
								from.SendLocalizedMessage( 1008126, true, Math.Ceiling( ts.TotalDays / 30.0 ).ToString() ); // Your account is not old enough to use this item. Months until you can use this item : 
								return false;
							}

							return true;
						}

						if ( args.Length == entries[j].Args.Length )
						{
							bool match = true;

							for ( int k = 0; match && k < args.Length; ++k )
								match = ( args[k].Equals( entries[j].Args[k] ) );

							if ( match )
							{
								if ( (!isRelaxedRules || i > 0) && !HasAccess( from, list, out ts ) )
								{
									from.SendLocalizedMessage( 1008126, true, Math.Ceiling( ts.TotalDays / 30.0 ).ToString() ); // Your account is not old enough to use this item. Months until you can use this item : 
									return false;
								}

								return true;
							}
						}
					}
				}
			}

			// no entry?
			return true;
		}

		public static int GetRewardYearLabel( Item item, object[] args )
		{
			int level = GetRewardYear( item, args );

			return 1076216 + ( ( level < 10 ) ? level : ( level < 12 ) ? (( level - 9 ) + 4240 ) : (( level - 11 ) + 37585 ) );
		}

		public static int GetRewardYear( Item item, object[] args )
		{
			if ( m_Lists == null )
				SetupRewardTables();

			Type type = item.GetType();

			for ( int i = 0; i < m_Lists.Length; ++i )
			{
				RewardList list = m_Lists[i];
				RewardEntry[] entries = list.Entries;

				for ( int j = 0; j < entries.Length; ++j )
				{
					if ( entries[j].ItemType == type )
					{
						if ( args == null && entries[j].Args.Length == 0 )
							return i + 1;

						if ( args.Length == entries[j].Args.Length )
						{
							bool match = true;

							for ( int k = 0; match && k < args.Length; ++k )
								match = ( args[k].Equals( entries[j].Args[k] ) );

							if ( match )
								return i + 1;
						}
					}
				}
			}

			// no entry?
			return 0;
		}

		public static void SetupRewardTables()
		{
			RewardCategory monsterStatues = new RewardCategory( 1049750 );
			RewardCategory cloaksAndRobes = new RewardCategory( 1049752 );
			RewardCategory etherealSteeds = new RewardCategory( 1049751 );
			RewardCategory specialDyeTubs = new RewardCategory( 1049753 );
			RewardCategory houseAddOns    = new RewardCategory( 1049754 );
			RewardCategory miscellaneous  = new RewardCategory( 1078596 );

			m_Categories = new RewardCategory[]
				{
					monsterStatues,
					cloaksAndRobes,
					etherealSteeds,
					specialDyeTubs,
					houseAddOns,
					miscellaneous
				};

			const int Bronze = 0x972;
			const int Copper = 0x96D;
			const int Golden = 0x8A5;
			const int Agapite = 0x979;
			const int Verite = 0x89F;
			const int Valorite = 0x8AB;
            const int Lunite = 2603;
			const int IceGreen = 0x47F;
			const int IceBlue = 0x482;
			const int DarkGray = 0x497;
			const int Fire = 0x489;
			const int IceWhite = 0x47E;
			const int JetBlack = 0x001;
			const int Pink		= 0x490;
			const int Crimson	= 0x485;

			m_Lists = new RewardList[]
			{
					new RewardList( RewardInterval, 1, new RewardEntry[]
					{
						new RewardEntry( specialDyeTubs, 1006008, typeof( RewardBlackDyeTub ) ),
						new RewardEntry( specialDyeTubs, 1006013, typeof( FurnitureDyeTub ) ),
						new RewardEntry( specialDyeTubs, 1006047, typeof( SpecialDyeTub ) ),
						new RewardEntry( monsterStatues, 1006024, typeof( MonsterStatuette ), MonsterStatuetteType.Crocodile ),
						new RewardEntry( monsterStatues, 1006025, typeof( MonsterStatuette ), MonsterStatuetteType.Daemon ),
						new RewardEntry( monsterStatues, 1006026, typeof( MonsterStatuette ), MonsterStatuetteType.Dragon ),
						new RewardEntry( monsterStatues, 1006027, typeof( MonsterStatuette ), MonsterStatuetteType.EarthElemental ),
						new RewardEntry( monsterStatues, 1006028, typeof( MonsterStatuette ), MonsterStatuetteType.Ettin ),
						new RewardEntry( monsterStatues, 1006029, typeof( MonsterStatuette ), MonsterStatuetteType.Gargoyle ),
						new RewardEntry( monsterStatues, 1006030, typeof( MonsterStatuette ), MonsterStatuetteType.Gorilla ),
						new RewardEntry( monsterStatues, 1006031, typeof( MonsterStatuette ), MonsterStatuetteType.Lich ),
						new RewardEntry( monsterStatues, 1006032, typeof( MonsterStatuette ), MonsterStatuetteType.Lizardman ),
						new RewardEntry( monsterStatues, 1006033, typeof( MonsterStatuette ), MonsterStatuetteType.Ogre ),
						new RewardEntry( monsterStatues, 1006034, typeof( MonsterStatuette ), MonsterStatuetteType.Orc ),
						new RewardEntry( monsterStatues, 1006035, typeof( MonsterStatuette ), MonsterStatuetteType.Ratman ),
						new RewardEntry( monsterStatues, 1006036, typeof( MonsterStatuette ), MonsterStatuetteType.Skeleton ),
						new RewardEntry( monsterStatues, 1006037, typeof( MonsterStatuette ), MonsterStatuetteType.Troll ),
						new RewardEntry( miscellaneous,  1076155, typeof( RedSoulstone ), Expansion.ML ),
					} ),
					new RewardList( RewardInterval, 3, new RewardEntry[]
					{						
						new RewardEntry( monsterStatues, 1006038, typeof( MonsterStatuette ), MonsterStatuetteType.Cow ),
						new RewardEntry( monsterStatues, 1006039, typeof( MonsterStatuette ), MonsterStatuetteType.Zombie ),
						new RewardEntry( monsterStatues, 1006040, typeof( MonsterStatuette ), MonsterStatuetteType.Llama ),
						new RewardEntry( etherealSteeds, 1006019, typeof( EtherealHorse ) ),
						new RewardEntry( etherealSteeds, 1006050, typeof( EtherealOstard ) ),
						new RewardEntry( etherealSteeds, 1006051, typeof( EtherealLlama ) ),

					} ),
					new RewardList( RewardInterval, 4, new RewardEntry[]
					{
						new RewardEntry( specialDyeTubs, 1049740, typeof( RunebookDyeTub ) ),						
						new RewardEntry( monsterStatues, 1049742, typeof( MonsterStatuette ), MonsterStatuetteType.Ophidian ),
						new RewardEntry( monsterStatues, 1049743, typeof( MonsterStatuette ), MonsterStatuetteType.Reaper ),
						new RewardEntry( monsterStatues, 1049744, typeof( MonsterStatuette ), MonsterStatuetteType.Mongbat ),
						new RewardEntry( etherealSteeds, 1049746, typeof( EtherealKirin ) ),
						new RewardEntry( etherealSteeds, 1049745, typeof( EtherealUnicorn ) ),
						new RewardEntry( etherealSteeds, 1049747, typeof( EtherealRidgeback ) ),
					} ),
					new RewardList( RewardInterval, 5, new RewardEntry[]
					{
						new RewardEntry( specialDyeTubs, 1049741, typeof( StatuetteDyeTub ) ),
						
						new RewardEntry( monsterStatues, 1049768, typeof( MonsterStatuette ), MonsterStatuetteType.Gazer ),
						new RewardEntry( monsterStatues, 1049769, typeof( MonsterStatuette ), MonsterStatuetteType.FireElemental ),
						new RewardEntry( monsterStatues, 1049770, typeof( MonsterStatuette ), MonsterStatuetteType.Wolf ),
						new RewardEntry( etherealSteeds, 1049749, typeof( EtherealSwampDragon ) ),
						new RewardEntry( etherealSteeds, 1049748, typeof( EtherealBeetle ) ),
					} ),					
					new RewardList( RewardInterval, 6, new RewardEntry[]
					{
						new RewardEntry( houseAddOns,	1076188, typeof( CharacterStatueMaker ), Expansion.ML, StatueType.Jade ),
						new RewardEntry( houseAddOns,	1076189, typeof( CharacterStatueMaker ), Expansion.ML, StatueType.Marble ),
						new RewardEntry( houseAddOns,	1076190, typeof( CharacterStatueMaker ), Expansion.ML, StatueType.Bronze ),
					} ),		
					new RewardList( RewardInterval, 7, new RewardEntry[]
					{
					} ),
					new RewardList( RewardInterval, 8, new RewardEntry[]
					{
					} ),
					new RewardList( RewardInterval, 9, new RewardEntry[]
					{
						new RewardEntry( etherealSteeds,	1076159, typeof( RideablePolarBear ), Expansion.ML ),
					} ),
					new RewardList( RewardInterval, 10, new RewardEntry[]
					{												
						new RewardEntry( monsterStatues,	1080520, typeof( MonsterStatuette ), Expansion.ML, MonsterStatuetteType.Harrower ),
						new RewardEntry( monsterStatues,	1080521, typeof( MonsterStatuette ), Expansion.ML, MonsterStatuetteType.Efreet ),
                        												
						new RewardEntry( etherealSteeds,	1080386, typeof( EtherealCuSidhe ), Expansion.ML ),
					} ),

					new RewardList( RewardInterval, 11, new RewardEntry[]
					{
						new RewardEntry( etherealSteeds,	1113908, typeof( EtherealReptalon ), Expansion.ML ),
					} ),

					new RewardList( RewardInterval, 12, new RewardEntry[]
					{
						new RewardEntry( etherealSteeds,	1113813, typeof( EtherealHiryu ), Expansion.ML ),
					} ),
			};
		}
		public static void Initialize()
		{
			if ( Enabled )
				EventSink.Login += new LoginEventHandler( EventSink_Login );
		}

		private static void EventSink_Login( LoginEventArgs e )
		{
			if ( !e.Mobile.Alive )
				return;

			int cur, max, level;

			ComputeRewardInfo( e.Mobile, out cur, out max, out level );

            /*
			if ( e.Mobile.SkillsCap == 7000 || e.Mobile.SkillsCap == 7050 || e.Mobile.SkillsCap == 7100 || e.Mobile.SkillsCap == 7150 || e.Mobile.SkillsCap == 7200 )
			{
				if ( level > 4 )
					level = 4;
				else if ( level < 0 )
					level = 0;

				if ( SkillCapRewards )
					e.Mobile.SkillsCap = 7000 + (level * 50);
				else
					e.Mobile.SkillsCap = 7000;
			}

			if ( Core.ML && e.Mobile is PlayerMobile && !((PlayerMobile)e.Mobile).HasStatReward && HasHalfLevel( e.Mobile ) )
			{
				((PlayerMobile)e.Mobile).HasStatReward = true;
				e.Mobile.StatCap += 5;
			}
            */

			if ( cur < max )
				e.Mobile.SendGump( new RewardNoticeGump( e.Mobile ) );
		}
	}

	public interface IRewardItem
	{
		bool IsRewardItem{ get; set; }
	}
}
