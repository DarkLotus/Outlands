using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;

using Server.Multis;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.Spells;
using Server.Network;

namespace Server.Misc
{
    public class NotorietyHandlers
    {
        public static void Initialize()
        {
            Notoriety.Hues[Notoriety.Innocent] = 0x59;
            Notoriety.Hues[Notoriety.Ally] = 0x3F;
            Notoriety.Hues[Notoriety.CanBeAttacked] = 0x3B2;
            Notoriety.Hues[Notoriety.Criminal] = 0x3B2;
            Notoriety.Hues[Notoriety.Enemy] = 0x90;
            Notoriety.Hues[Notoriety.Murderer] = 0x22;
            Notoriety.Hues[Notoriety.Invulnerable] = 0x35;

            Notoriety.Handler = new NotorietyHandler(MobileNotoriety);

            Mobile.AllowBeneficialHandler = new AllowBeneficialHandler(Mobile_AllowBeneficial);
            Mobile.AllowHarmfulHandler = new AllowHarmfulHandler(Mobile_AllowHarmful);
        }

        private enum GuildStatus 
        { 
            None,
            Peaceful,
            Waring 
        }

        private static GuildStatus GetGuildStatus(Mobile m)
        {
            //TEST: GUILD
            /*
            if (m.Guild == null)
                return GuildStatus.None;
            
            else if (((Guild)m.Guild).Enemies.Count == 0 && m.Guild.Type == GuildType.Regular)
                return GuildStatus.Peaceful;
            */

            return GuildStatus.Waring;
        }

        private static bool CheckBeneficialStatus(GuildStatus from, GuildStatus target)
        {
            if (from == GuildStatus.Waring || target == GuildStatus.Waring)
                return false;

            return true;
        }

        public static bool Mobile_AllowBeneficial(Mobile from, Mobile target)
        {
            if (from == null || target == null || from.AccessLevel > AccessLevel.Player || target.AccessLevel > AccessLevel.Player)
                return true;

            PlayerMobile pm_From = from as PlayerMobile;
            PlayerMobile pm_Target = target as PlayerMobile;
            BaseCreature bc_Target = target as BaseCreature;

            Map map = from.Map;
            
            //Young Player Handling
            bool fromYoung = from is PlayerMobile && ((PlayerMobile)from).Young;

            if (fromYoung)
            {
                bool self = target == from;
                bool targetIsYoung = target is PlayerMobile && ((PlayerMobile)target).Young;
                bool youngPet = target is BaseCreature &&
                                ((BaseCreature)target).ControlMaster is PlayerMobile &&
                                ((PlayerMobile)((BaseCreature)target).ControlMaster).Young;

                if (self || youngPet || targetIsYoung)
                    return true;

                else
                    return false; // Young players cannot perform beneficial actions towards older players
            }            

            if (map != null && (map.Rules & MapRules.BeneficialRestrictions) == 0)
                return true; // In felucca, anything goes

            if (!from.Player)
                return true; // NPCs have no restrictions

            if (target is BaseCreature && !((BaseCreature)target).Controlled)
                return false; // Players cannot heal uncontrolled mobiles

            //TEST: GUILD
            /*
            Guild fromGuild = from.Guild as Guild;
            Guild targetGuild = target.Guild as Guild;

            if (fromGuild != null && targetGuild != null && (targetGuild == fromGuild || fromGuild.IsAlly(targetGuild)))
                return true; // Guild members can be beneficial
            */

            return CheckBeneficialStatus(GetGuildStatus(from), GetGuildStatus(target));
        }

        public static bool Mobile_AllowHarmful(Mobile from, Mobile target)
        {
            if (from == null || target == null || from.AccessLevel > AccessLevel.Player || target.AccessLevel > AccessLevel.Player)
                return true;

            PlayerMobile pm_From = from as PlayerMobile;
            PlayerMobile pm_Target = target as PlayerMobile;

            BaseCreature bc_From = from as BaseCreature;
            BaseCreature bc_Target = target as BaseCreature;

            PlayerMobile fromRootPlayer = null;
            PlayerMobile targetRootPlayer = null;

            if (pm_From != null)
                fromRootPlayer = pm_From;

            if (pm_Target != null)
                targetRootPlayer = pm_Target;

            if (bc_From != null)
            {
                if (bc_From.ControlMaster is PlayerMobile)
                    fromRootPlayer = bc_From.ControlMaster as PlayerMobile;
            }

            if (bc_Target != null)
            {
                if (bc_Target.ControlMaster is PlayerMobile)
                    targetRootPlayer = bc_Target.ControlMaster as PlayerMobile;
            }

            #region Arena 

            ArenaGroupController fromArenaGroupController = ArenaGroupController.GetArenaGroupRegionAtLocation(from.Location, from.Map);
            ArenaGroupController targetArenaGroupController = ArenaGroupController.GetArenaGroupRegionAtLocation(target.Location, target.Map);

            if (fromArenaGroupController != null || targetArenaGroupController != null)
            {
                if (fromRootPlayer != null && targetRootPlayer != null)
                {
                    ArenaFight fromArenaFight = fromRootPlayer.m_ArenaFight;
                    ArenaFight targetArenaFight = targetRootPlayer.m_ArenaFight;

                    if (fromArenaFight != null && targetArenaFight != null && fromArenaFight == targetArenaFight)
                    {
                        if (fromArenaFight.m_FightPhase == ArenaFight.FightPhaseType.Fight)
                        {
                            if (fromArenaFight.IsWithinArena(from.Location, from.Map) && fromArenaFight.IsWithinArena(target.Location, target.Map))
                                return true;                            
                        }
                    }
                }

                return false;
            }

            #endregion

            #region Ress Penalty

            if (fromRootPlayer != null && targetRootPlayer != null)
            {
                if (fromRootPlayer.RessPenaltyExpiration > DateTime.UtcNow && fromRootPlayer.m_RessPenaltyAccountWideAggressionRestriction)
                    return false;
            }

            #endregion

            Map map = from.Map;

            // Young Players in Felucca
            if (from.Player && target.Player && (((PlayerMobile)target).Young || ((PlayerMobile)from).Young) && !(target.Criminal || from.Criminal))
                return false;   // Old players cannot attack youngs and vice versa unless young is crim
            
            //Felucca
            if (map != null && ((map.Rules & MapRules.BeneficialRestrictions) == 0))
                return true; // In felucca, anything goes. Special case fire dungeon as players can access it in fel or trammel under fel rules

            //Other Maps
            BaseCreature bc = from as BaseCreature;

            if (!from.Player && !(bc != null && bc.GetMaster() != null && bc.GetMaster().AccessLevel == AccessLevel.Player))
            {
                if (!CheckAggressor(from.Aggressors, target) && !CheckAggressed(from.Aggressed, target) && target is PlayerMobile && ((PlayerMobile)target).CheckYoungProtection(from))
                    return false;

                return true; // Uncontrolled NPCs are only restricted by the young system
            }       

            if (target is BaseCreature && (((BaseCreature)target).Controlled || (((BaseCreature)target).Summoned && from != ((BaseCreature)target).SummonMaster)))
                return false; // Cannot harm other controlled mobiles

            if (target.Player)
                return false; // Cannot harm other players

            if (!(target is BaseCreature && ((BaseCreature)target).InitialInnocent))
            {
                if (Notoriety.Compute(from, target) == Notoriety.Innocent)
                    return false; // Cannot harm innocent mobiles
            }

            return true;
        }        

        public static int CorpseNotoriety(Mobile source, Corpse target)
        {
            if (target.AccessLevel > AccessLevel.Player)
                return Notoriety.CanBeAttacked;

            Body body = (Body)target.Amount;

            BaseCreature cretOwner = target.Owner as BaseCreature;

            if (cretOwner != null)
            {
                //TEST: GUILD
                /*
                Guild sourceGuild = GetGuildFor(source.Guild as Guild, source);
                Guild targetGuild = GetGuildFor(target.Guild as Guild, target.Owner);

                if (sourceGuild != null && targetGuild != null)
                {
                    if (sourceGuild == targetGuild || sourceGuild.IsAlly(targetGuild))
                        return Notoriety.Ally;

                    else if (sourceGuild.IsEnemy(targetGuild))
                        return Notoriety.Enemy;
                }
                 */

                if (cretOwner.IsLoHBoss() || cretOwner.FreelyLootable)
                    return Notoriety.CanBeAttacked;

                if (CheckHouseFlag(source, target.Owner, target.Location, target.Map))
                    return Notoriety.CanBeAttacked;

                int actual = Notoriety.CanBeAttacked;

                if (target.Kills >= Mobile.MurderCountsRequiredForMurderer || (body.IsMonster && IsSummoned(target.Owner as BaseCreature)) || (target.Owner is BaseCreature && (((BaseCreature)target.Owner).IsMurderer() || ((BaseCreature)target.Owner).IsAnimatedDead)))
                    actual = Notoriety.Murderer;

                if (DateTime.UtcNow >= (target.TimeOfDeath + Corpse.MonsterLootRightSacrifice))
                    return actual;

                Party sourceParty = Party.Get(source);

                List<Mobile> list = target.Aggressors;

                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i] == source || (sourceParty != null && Party.Get(list[i]) == sourceParty))
                        return actual;
                }

                return Notoriety.Innocent;
            }

            else
            {
                if (target.Kills >= Mobile.MurderCountsRequiredForMurderer || (body.IsMonster && IsSummoned(target.Owner as BaseCreature)) || (target.Owner is BaseCreature && (((BaseCreature)target.Owner).IsMurderer() || ((BaseCreature)target.Owner).IsAnimatedDead)))
                    return Notoriety.Murderer;

                if (target.Criminal)
                    return Notoriety.Criminal;
                
                //TEST: GUILD
                /*
                Guild sourceGuild = GetGuildFor(source.Guild as Guild, source);
                Guild targetGuild = GetGuildFor(target.Guild as Guild, target.Owner);

                if (sourceGuild != null && targetGuild != null)
                {
                    if (sourceGuild == targetGuild || sourceGuild.IsAlly(targetGuild))
                        return Notoriety.Ally;

                    else if (sourceGuild.IsEnemy(targetGuild))
                        return Notoriety.Enemy;
                }
                */

                if (target.Owner != null && target.Owner is BaseCreature && ((BaseCreature)target.Owner).AlwaysAttackable)
                    return Notoriety.CanBeAttacked;

                if (CheckHouseFlag(source, target.Owner, target.Location, target.Map))
                    return Notoriety.CanBeAttacked;

                if (!(target.Owner is PlayerMobile) && !IsPet(target.Owner as BaseCreature))
                    return Notoriety.CanBeAttacked;

                List<Mobile> list = target.Aggressors;

                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i] == source)
                        return Notoriety.CanBeAttacked;
                }

                if (SpellHelper.InBuccs(target.Map, target.Location) || SpellHelper.InYewOrcFort(target.Map, target.Location) || SpellHelper.InYewCrypts(target.Map, target.Location))
                    return Notoriety.CanBeAttacked;

                if (GreyZoneTotem.InGreyZoneTotemArea(target.Location, target.Map))
                    return Notoriety.CanBeAttacked;

                //Hotspot Nearby
                if (Custom.Hotspot.InHotspotArea(target.Location, target.Map, true))
                    return Notoriety.CanBeAttacked;

                if (target.IsBones)
                    return Notoriety.CanBeAttacked;

                return Notoriety.Innocent;
            }
        }

        public static int MobileNotoriety(Mobile source, Mobile target)
        {
            return DetermineMobileNotoriety(source, target, false);
        }

        public static int DetermineMobileNotoriety(Mobile source, Mobile target, bool useVengeance)
        {  
            BaseCreature bc_Source = source as BaseCreature;
            PlayerMobile pm_Source = source as PlayerMobile;

            Mobile m_SourceController = null;
            BaseCreature bc_SourceController = null;
            PlayerMobile pm_SourceController = null;

            BaseCreature bc_Target = target as BaseCreature;
            PlayerMobile pm_Target = target as PlayerMobile;

            Mobile m_TargetController = null;
            BaseCreature bc_TargetController = null;
            PlayerMobile pm_TargetController = null;
            
            if (bc_Source != null)
            {
                m_SourceController = bc_Source.ControlMaster as Mobile;
                bc_SourceController = bc_Source.ControlMaster as BaseCreature;
                pm_SourceController = bc_Source.ControlMaster as PlayerMobile;
            }

            if (bc_Target != null)
            {
                m_TargetController = bc_Target.ControlMaster as Mobile;
                bc_TargetController = bc_Target.ControlMaster as BaseCreature;
                pm_TargetController = bc_Target.ControlMaster as PlayerMobile;
            }

            ArenaController fromArenaController = ArenaController.GetArenaAtLocation(source.Location, source.Map);
            ArenaController targetArenaController = ArenaController.GetArenaAtLocation(target.Location, target.Map);

            if (fromArenaController != null && targetArenaController != null && fromArenaController == targetArenaController)
            {
                if (fromArenaController.m_ArenaFight != null)
                {
                    if (fromArenaController.m_ArenaFight.m_FightPhase == ArenaFight.FightPhaseType.Fight)
                    {
                        if (fromArenaController.m_ArenaFight.m_ArenaMatch != null)
                        {
                            ArenaMatch arenaMatch = fromArenaController.m_ArenaFight.m_ArenaMatch;

                            if (ArenaMatch.IsValidArenaMatch(arenaMatch, null, false))
                            {
                                PlayerMobile rootPlayerFrom = null;
                                PlayerMobile rootPlayerTarget = null;

                                if (pm_Source != null)
                                    rootPlayerFrom = pm_Source;

                                if (pm_Target != null)
                                    rootPlayerTarget = pm_Target;

                                if (pm_SourceController != null)
                                    rootPlayerFrom = pm_SourceController;

                                if (pm_TargetController != null)                            
                                    rootPlayerTarget = pm_TargetController;                            

                                ArenaParticipant fromArenaParticipant = fromArenaController.m_ArenaFight.m_ArenaMatch.GetParticipant(rootPlayerFrom);
                                ArenaParticipant targetArenaParticipant = fromArenaController.m_ArenaFight.m_ArenaMatch.GetParticipant(rootPlayerTarget);
                                
                                if (fromArenaParticipant != null && targetArenaParticipant != null)
                                {
                                    if (fromArenaParticipant.m_FightStatus == ArenaParticipant.FightStatusType.Alive && targetArenaParticipant.m_FightStatus == ArenaParticipant.FightStatusType.Alive)
                                    {
                                        ArenaTeam fromTeam = null;
                                        ArenaTeam targetTeam = null;

                                        foreach (ArenaTeam team in arenaMatch.m_Teams)
                                        {
                                            if (team == null) continue;
                                            if (team.Deleted) continue;

                                            ArenaParticipant participant = team.GetPlayerParticipant(rootPlayerFrom);

                                            if (participant != null)
                                                fromTeam = team;

                                            participant = team.GetPlayerParticipant(rootPlayerTarget);

                                            if (participant != null)
                                                targetTeam = team;
                                        }

                                        if (fromTeam != null && targetTeam != null)
                                        {
                                            if (fromTeam == targetTeam)
                                                return Notoriety.Ally;

                                            if (fromTeam != targetTeam)
                                                return Notoriety.Enemy;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Berserk Creatures
            if (bc_Source != null && (source is BladeSpirits || source is EnergyVortex))
            {
                if (bc_Source.ControlMaster != null && pm_Target != null)
                {
                    //Blade Spirits + Energy Vortexes Can Freely Attack Their Control Master Without Causing Criminal Action
                    if (bc_Source.ControlMaster == pm_Target)
                        return Notoriety.CanBeAttacked;
                }

                if (bc_Source.ControlMaster != null && bc_Target != null)
                {
                    //Blade Spirits + Energy Vortexes Can Freely Attack Other Followers Of Their Control Master Without Causing Criminal Action
                    if (bc_Source.ControlMaster == bc_Target.ControlMaster) 
                        return Notoriety.CanBeAttacked;
                }                
            }
                        
            if (target is BladeSpirits || target is EnergyVortex)
                return Notoriety.Murderer;

            //Staff Members Always Attackable
            if (target.AccessLevel > AccessLevel.Player)
                return Notoriety.CanBeAttacked;

            if (m_TargetController != null)
            {
                //Creature Controlled By Staff Member
                if (m_TargetController.AccessLevel > AccessLevel.Player)
                    return Notoriety.CanBeAttacked;
            }
            
            //Enemy of One
            if (pm_Source != null && bc_Target != null)
            {
                if (!bc_Target.Summoned && !bc_Target.Controlled && pm_Source.EnemyOfOneType == target.GetType())
                    return Notoriety.Enemy;
            }
            
            //Justice Free Zone
            if (SpellHelper.InBuccs(target.Map, target.Location) || SpellHelper.InYewOrcFort(target.Map, target.Location) || SpellHelper.InYewCrypts(target.Map, target.Location))
                return Notoriety.CanBeAttacked;

            //Grey Zone Totem Nearby
            if (GreyZoneTotem.InGreyZoneTotemArea(target.Location, target.Map))
                    return Notoriety.CanBeAttacked;

            //Hotspot Nearby
            if (Custom.Hotspot.InHotspotArea(target.Location, target.Map, true))
                return Notoriety.CanBeAttacked;

            //Player Notoriety
            if (pm_Target != null)
            {
                //Friendly
                if (pm_SourceController != null)
                {
                    if (pm_SourceController == pm_Target)                    
                        return Notoriety.Ally;                    
                }

                //Murderer
                if (pm_Target.Murderer && !pm_Target.HideMurdererStatus)                
                    return Notoriety.Murderer;                

                //Criminal
                if (pm_Target.Criminal)                
                    return Notoriety.Criminal;                

                //Perma-Grey
                if (SkillHandlers.Stealing.ClassicMode && pm_Target.PermaFlags.Contains(source))                
                    return Notoriety.CanBeAttacked;                

                if (pm_SourceController != null)
                {
                    //Target is Perma-Grey to Source Creature's Controller
                    if (SkillHandlers.Stealing.ClassicMode && pm_Target.PermaFlags.Contains(pm_SourceController))                    
                        return Notoriety.CanBeAttacked;                    
                }                
            }

            //Guilds
            //TEST: GUILD
            /*
            Guild sourceGuild = GetGuildFor(source.Guild as Guild, source);
            Guild targetGuild = GetGuildFor(target.Guild as Guild, target);

            if (sourceGuild != null && targetGuild != null)
            {
                if (sourceGuild == targetGuild || sourceGuild.IsAlly(targetGuild))
                    return Notoriety.Ally;

                else if (sourceGuild.IsEnemy(targetGuild))
                    return Notoriety.Enemy;
            }
            */

            //Creature Notoriety
            if (bc_Target != null)
            {
                //Friendly
                if (m_TargetController != null)
                {
                    //Target is Source's Controller
                    if (source == m_TargetController)                    
                        return Notoriety.Ally;                    
                }

                if (m_SourceController != null)
                {
                    //Source is Target's Controller
                    if (m_SourceController == bc_Target)                    
                        return Notoriety.Ally;                    
                }

                //Murderer
                if (bc_Target.IsMurderer())                
                    return Notoriety.Murderer;                

                if (pm_TargetController != null)
                {
                    if (pm_TargetController.Murderer)                    
                        return Notoriety.Murderer;                    
                }

                if (bc_TargetController != null)
                {
                    if (bc_TargetController.IsMurderer())                    
                        return Notoriety.Murderer;                    
                }

                //Criminal
                if (bc_Target.Criminal)                
                    return Notoriety.Criminal;                

                if (pm_TargetController != null)
                {
                    if (pm_TargetController.Criminal)                    
                        return Notoriety.Criminal;                    
                }

                if (bc_TargetController != null)
                {
                    if (bc_TargetController.Criminal)                    
                        return Notoriety.Criminal;                    
                }

                //Perma-Grey
                if (pm_TargetController != null)
                {
                    if (SkillHandlers.Stealing.ClassicMode && pm_TargetController.PermaFlags.Contains(source))                    
                        return Notoriety.CanBeAttacked;                    

                    if (pm_SourceController != null)
                    {
                        //Target is Perma-Grey to Source Creature's Controller
                        if (SkillHandlers.Stealing.ClassicMode && pm_TargetController.PermaFlags.Contains(pm_SourceController))                        
                            return Notoriety.CanBeAttacked;                        
                    }
                }                                
            }

            //Housing
            if (CheckHouseFlag(source, target, target.Location, target.Map))
                return Notoriety.CanBeAttacked;

            //Aggressor: Source to Target
            if (CheckAggressor(source.Aggressors, target))
                return Notoriety.CanBeAttacked;

            if (CheckAggressed(source.Aggressed, target) && useVengeance)
                return Notoriety.CanBeAttacked;

            //Aggressor: Source Controller to Target
            if (m_SourceController != null)
            {
                if (CheckAggressor(m_SourceController.Aggressors, target))
                    return Notoriety.CanBeAttacked;

                if (CheckAggressed(m_SourceController.Aggressed, target) && useVengeance)
                    return Notoriety.CanBeAttacked;
            }

            //Aggressor: Source to Target's Controller
            if (m_TargetController != null)
            {
                if (CheckAggressor(source.Aggressors, m_TargetController))
                    return Notoriety.CanBeAttacked;

                if (CheckAggressed(source.Aggressed, m_TargetController) && useVengeance)
                    return Notoriety.CanBeAttacked;
            }

            //Aggressor: Source Controller to Target's Controller
            if (m_SourceController != null && m_TargetController != null)
            {
                if (CheckAggressor(m_SourceController.Aggressors, m_TargetController))
                    return Notoriety.CanBeAttacked;

                if (CheckAggressed(m_SourceController.Aggressed, m_TargetController) && useVengeance)
                    return Notoriety.CanBeAttacked;
            }

            //Player Followers: If A Player or Any of Their Followers Have been Aggressed or Barded, the Player and All Other Followers Can Attack the Aggressor
            PlayerMobile pm_Player = null;

            if (pm_Source != null)
                pm_Player = pm_Source;

            if (pm_SourceController != null)
                pm_Player = pm_SourceController;

            if (pm_Player != null)
            {
                if (pm_Player.AllFollowers.Count > 0)
                {
                    //Any of the Player's Other Followers
                    foreach (Mobile follower in pm_Player.AllFollowers)
                    {
                        BaseCreature bc_Follower = follower as BaseCreature;

                        if (bc_Follower == null)
                            continue;

                        //Follower Has Been Aggressed/Aggresor to Target
                        if (CheckAggressor(bc_Follower.Aggressors, target))
                            return Notoriety.CanBeAttacked;

                        if (CheckAggressed(bc_Follower.Aggressed, target) && useVengeance)
                            return Notoriety.CanBeAttacked;

                        //Follower Has Been Aggressed/Aggresor by/to Target's Controller
                        if (m_TargetController != null)
                        {
                            if (CheckAggressor(bc_Follower.Aggressors, m_TargetController))
                                return Notoriety.CanBeAttacked;

                            if (CheckAggressed(bc_Follower.Aggressed, m_TargetController) && useVengeance)
                                return Notoriety.CanBeAttacked;
                        }
                    }
                }
            }

            //Ships: Players and Creatures Friendly to a Ship Can Freely Attack Non-Friendly Mobiles on their Ship
            BaseShip sourceShip = null;

            if (bc_Source != null)
            {
                if (bc_Source.ShipOccupied != null)
                    sourceShip = bc_Source.ShipOccupied;
            }

            if (pm_Source != null)
            {
                if (pm_Source.ShipOccupied != null)
                    sourceShip = pm_Source.ShipOccupied;
            }

            if (sourceShip != null)
            {
                BaseShip targetShip = null;

                if (bc_Target != null)
                {
                    if (bc_Target.ShipOccupied != null)
                        targetShip = bc_Target.ShipOccupied;
                }

                if (pm_Target != null)
                {
                    if (pm_Target.ShipOccupied != null)
                        targetShip = pm_Target.ShipOccupied;
                }

                //On Same Ship
                if (sourceShip != null && targetShip != null && !sourceShip.Deleted && !targetShip.Deleted && sourceShip == targetShip)
                {
                    bool sourceBelongs = false;
                    bool targetBelongs = false;

                    //Source Belongs n the Ship
                    if (sourceShip.Crew.Contains(source) || sourceShip.IsFriend(source) || sourceShip.IsCoOwner(source) || sourceShip.IsOwner(source))
                        sourceBelongs = true;

                    //Source's Owner Belongs on the Ship                    
                    else if (bc_Source != null)
                    {
                        if (m_SourceController != null)
                        {
                            if (sourceShip.Crew.Contains(m_SourceController) || sourceShip.IsFriend(m_SourceController) || sourceShip.IsCoOwner(m_SourceController) || sourceShip.IsOwner(m_SourceController))
                                sourceBelongs = true;
                        }
                    }

                    //Target Belongs On The Ship
                    if (sourceShip.Crew.Contains(target) || sourceShip.IsFriend(target) || sourceShip.IsCoOwner(target) || sourceShip.IsOwner(target))
                        targetBelongs = true;

                    //Target's Owner Belongs On the Ship
                    else if (bc_Target != null)
                    {
                        if (m_TargetController != null)
                        {
                            if (sourceShip.Crew.Contains(m_TargetController) || sourceShip.IsFriend(m_TargetController) || sourceShip.IsCoOwner(m_TargetController) || sourceShip.IsOwner(m_TargetController))
                                targetBelongs = true;
                        }
                    }

                    //Target May Be Freely Attacked on Ship
                    if (sourceBelongs && !targetBelongs)
                        return Notoriety.CanBeAttacked;
                }
            }

            //Polymorph or Body Transformation
            if (!(bc_Target != null && bc_Target.InitialInnocent))
            {
                if (target.Player && target.BodyMod > 0)
                {
                }

                else if (!target.Body.IsHuman && !target.Body.IsGhost && !IsPet(bc_Target) && !TransformationSpellHelper.UnderTransformation(target))
                    return Notoriety.CanBeAttacked;
            }

            //If somehow a player is attacking us with their tamed creatures, and their creatures are flagged to us but the player isn't
            //if (pm_Source != null && pm_Target != null)
            //{
            //    if (pm_Target.AllFollowers.Count > 0)
            //    {
            //        //Any of the Player's Other Followers
            //        foreach (Mobile follower in pm_Target.AllFollowers)
            //        {
            //            int notorietyResult = Notoriety.Compute(source, follower);

            //            //Enemy Tamer Adopts Notoriety of Their Creature (Anything other than Innocent)
            //            if (notorietyResult != 1)
            //            {
            //                foreach(var aggressor in source.Aggressors)
            //                {
            //                    if (aggressor.Attacker == follower)
            //                        return notorietyResult;
            //                }
            //            }
            //        } 
            //    }
            //}
            
            return Notoriety.Innocent;
        }

        public static void PushNotoriety(Mobile from, Mobile to, bool aggressor)
        {
            BaseCreature bc_From = from as BaseCreature;
            PlayerMobile pm_From = from as PlayerMobile;

            BaseCreature bc_To = to as BaseCreature;
            PlayerMobile pm_To = to as PlayerMobile;

            PlayerMobile pm_First = null;
            PlayerMobile pm_Second = null;

            if (from == null || to == null)
                return;

            if (pm_From != null)
                pm_First = pm_From;

            if (bc_From != null)
            {
                if (bc_From.Controlled && bc_From.ControlMaster is PlayerMobile)
                    pm_First = bc_From.ControlMaster as PlayerMobile;
            }

            if (pm_To != null)
                pm_Second = pm_To;

            if (bc_To != null)
            {
                if (bc_To.Controlled && bc_To.ControlMaster is PlayerMobile)
                    pm_Second = bc_To.ControlMaster as PlayerMobile;
            }

            //First Player is Online
            if (pm_First != null)
            {
                if (pm_First.NetState != null)
                {
                    List<Mobile> m_Viewables = new List<Mobile>();

                    if (pm_First.AllFollowers.Count > 0)
                    {
                        foreach (Mobile follower in pm_First.AllFollowers)
                        {
                            if (follower != null)
                                m_Viewables.Add(follower);
                        }
                    }

                    if (pm_Second != null)
                    {
                        m_Viewables.Add(pm_Second);

                        if (pm_Second.AllFollowers.Count > 0)
                        {
                            foreach (Mobile follower in pm_Second.AllFollowers)
                            {
                                if (follower != null)
                                    m_Viewables.Add(follower);
                            }
                        }
                    }

                    if (bc_To != null)
                        m_Viewables.Add(bc_To);

                    //Update Data for All Things Viewable By This Player
                    foreach (Mobile mobile in m_Viewables)
                    {
                        if (mobile != null)
                        {
                            if (pm_First.CanSee(mobile))
                                pm_First.NetState.Send(MobileIncoming.Create(pm_First.NetState, pm_First, mobile));
                        }
                    }
                }
            }

            //Second Player is Online: 
            if (pm_Second != null && pm_Second != pm_First)
            {
                if (pm_Second.NetState != null)
                {
                    List<Mobile> m_Viewables = new List<Mobile>();

                    if (pm_Second.AllFollowers.Count > 0)
                    {
                        foreach (Mobile follower in pm_Second.AllFollowers)
                        {
                            if (follower != null)
                                m_Viewables.Add(follower);
                        }
                    }

                    if (pm_First != null)
                    {
                        m_Viewables.Add(pm_First);

                        if (pm_First.AllFollowers.Count > 0)
                        {
                            foreach (Mobile follower in pm_First.AllFollowers)
                            {
                                if (follower != null)
                                    m_Viewables.Add(follower);
                            }
                        }
                    }

                    if (bc_From != null)
                        m_Viewables.Add(bc_From);

                    //Update Data for All Things Viewable By This Player
                    foreach (Mobile mobile in m_Viewables)
                    {
                        if (mobile != null)
                        {
                            if (pm_Second.CanSee(mobile))
                                pm_Second.NetState.Send(MobileIncoming.Create(pm_Second.NetState, pm_Second, mobile));
                        }
                    }
                }
            }
        }

        public static bool CheckHouseFlag(Mobile from, Mobile m, Point3D p, Map map)
        {
            BaseHouse house = BaseHouse.FindHouseAt(p, map, 16);

            if (house == null || house.Public || !house.IsFriend(from))
                return false;

            if (m != null && house.IsFriend(m))
                return false;

            if (house.IsInGuardedRegion())
                return false;

            BaseCreature c = m as BaseCreature;

            if (c != null && !c.Deleted && c.Controlled && c.ControlMaster != null)
                return !house.IsFriend(c.ControlMaster);

            return true;
        }

        public static bool IsPet(BaseCreature c)
        {
            return (c != null && c.Controlled);
        }

        public static bool IsSummoned(BaseCreature c)
        {
            return (c != null && /*c.Controlled &&*/ c.Summoned);
        }

        public static bool CheckAggressor(List<AggressorInfo> list, Mobile target)
        {
            for (int i = 0; i < list.Count; ++i)
                if (list[i].Attacker == target)
                    return true;

            return false;
        }

        public static bool CheckAggressed(List<AggressorInfo> list, Mobile target)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                AggressorInfo info = list[i];

                if (!info.CriminalAggression && info.Defender == target)
                    return true;
            }

            return false;
        }
    }
}