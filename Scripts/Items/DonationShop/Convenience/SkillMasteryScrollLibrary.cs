﻿using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;
using Server.Accounting;
using Server.Gumps;
using Server.Spells;

namespace Server.Items
{
    public class SkillMasteryScrollLibrary : Item
    {
        public enum LockedDownAccessLevelType
        {
            Owner,
            CoOwner,
            Friend,
            Anyone
        }     

        private LockedDownAccessLevelType m_LockedDownAccessLevel = LockedDownAccessLevelType.Owner;
        [CommandProperty(AccessLevel.GameMaster)]
        public LockedDownAccessLevelType LockedDownAccessLevel
        {
            get { return m_LockedDownAccessLevel; }
            set { m_LockedDownAccessLevel = value; }
        }

        private bool m_RemoveAllOnSelection = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RemoveAllOnSelection
        {
            get { return m_RemoveAllOnSelection; }
            set { m_RemoveAllOnSelection = value; }
        }

        public override bool AlwaysAllowDoubleClick { get { return true; } }

        public List<SkillMasteryScrollLibraryEntry> m_LibraryEntries = new List<SkillMasteryScrollLibraryEntry>();

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public int closeGumpSound = 0x058;
        public int addItemSound = 0x249;

        [Constructable]
        public SkillMasteryScrollLibrary(): base(8793)
        {
            Name = "a skill mastery scroll library";

            Hue = 2963;
            Weight = 5;

            

            CreateSkillEntries();
        }

        public SkillMasteryScrollLibrary(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(" + GetTotalCount().ToString() + " scrolls)");

            if (IsLockedDown)
            {
                switch (m_LockedDownAccessLevel)
                {
                    case LockedDownAccessLevelType.Owner: LabelTo(from, "[owner accessable]"); break;
                    case LockedDownAccessLevelType.CoOwner: LabelTo(from, "[co-owner accessable]"); break;
                    case LockedDownAccessLevelType.Friend: LabelTo(from, "[friend accessable]"); break;
                    case LockedDownAccessLevelType.Anyone: LabelTo(from, "[freely access]"); break;
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!CanUse(player))
                return;

            player.SendSound(openGumpSound);
            
            from.CloseGump(typeof(SkillMasteryScrollLibraryGump));
            from.SendGump(new SkillMasteryScrollLibraryGump(player, this, 1));
        }

        public int GetTotalCount()
        {
            int totalCount = 0;

            foreach (SkillMasteryScrollLibraryEntry entry in m_LibraryEntries)
            {
                if (entry == null)
                    continue;

                totalCount += entry.Count;
            }

            return totalCount;
        }

        public void CreateSkillEntries()
        {
            for (int a = 0; a < SkillMasteryScroll.Skills.Length; a++)
            {
                SkillMasteryScrollLibraryEntry entry = new SkillMasteryScrollLibraryEntry();
                entry.skillName = SkillMasteryScroll.Skills[a];

                m_LibraryEntries.Add(entry);
            }
        }

        public void AuditSkillEntries()
        {
            for (int a = 0; a < SkillMasteryScroll.Skills.Length; a++)
            {
                SkillName skillName = SkillMasteryScroll.Skills[a];

                SkillMasteryScrollLibraryEntry entry = GetLibraryEntry(SkillMasteryScroll.Skills[a]);

                if (entry == null)
                {
                    SkillMasteryScrollLibraryEntry newEntry = new SkillMasteryScrollLibraryEntry();
                    newEntry.skillName = skillName;

                    m_LibraryEntries.Add(newEntry);
                }
            }
        }

        public SkillMasteryScrollLibraryEntry GetLibraryEntry(SkillName skillName)
        {
            SkillMasteryScrollLibraryEntry targetEntry = null;

            foreach (SkillMasteryScrollLibraryEntry entry in m_LibraryEntries)
            {
                if (entry.skillName == skillName)
                    return entry;
            }

            return targetEntry;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is SkillMasteryScroll)
            {
                SkillMasteryScroll skillMasteryScroll = dropped as SkillMasteryScroll;

                SkillMasteryScrollLibraryEntry entry = GetLibraryEntry(skillMasteryScroll.Skill);

                if (entry != null)
                {
                    skillMasteryScroll.Delete();

                    entry.Count++;

                    from.SendMessage("You add a skill mastery scroll to the library.");
                    from.SendSound(addItemSound);
                }

                return true;
            }

            else
                return false;
        }

        public void AddAllScrollsInPack(Mobile from)
        {
            if (from == null) return;
            if (from.Backpack == null) 
                return;

            List<SkillMasteryScroll> m_SkillMasteryScrolls = from.Backpack.FindItemsByType<SkillMasteryScroll>();

            int totalCount = 0;

            Queue m_Queue = new Queue();

            foreach (SkillMasteryScroll skillMasteryScroll in m_SkillMasteryScrolls)
            {
                m_Queue.Enqueue(skillMasteryScroll);                
            }

            while (m_Queue.Count > 0)
            {
                SkillMasteryScroll skillMasteryScroll = (SkillMasteryScroll)m_Queue.Dequeue();

                if (skillMasteryScroll == null) continue;
                if (skillMasteryScroll.Deleted) continue;

                SkillMasteryScrollLibraryEntry entry = GetLibraryEntry(skillMasteryScroll.Skill);

                if (entry == null)
                    continue;

                entry.Count++;
                totalCount++;
                skillMasteryScroll.Delete();
            }

            if (totalCount > 1)
            {
                from.SendMessage("You add " + totalCount.ToString() + " skill mastery scrolls to the library.");
                from.SendSound(addItemSound);
            }

            else if (totalCount == 1)
            {
                from.SendMessage("You add a skill mastery scroll to the library.");
                from.SendSound(addItemSound);
            }

            else
                from.SendMessage("You do not have any skill mastery scrolls in your backpack.");
        }

        public void EjectScroll(Mobile from, SkillName skillName, bool ejectAll)
        {
            if (from == null)
                return;

            SkillMasteryScrollLibraryEntry entry = GetLibraryEntry(skillName);

            if (entry == null)
                return;

            if (entry.Count == 0)
            {
                from.SendMessage("You do not have any copies of that skill mastery scroll currently stored within this library.");
                return;
            }            

            if (from.Backpack == null)
                return;

            if (from.Backpack.TotalItems >= from.Backpack.MaxItems)
            {
                from.SendMessage("Your backpack is at maximum capacity. Please remove some items and try again.");
                return;
            }

            if (ejectAll)
            {
                int spaceAvailable = from.Backpack.MaxItems - from.Backpack.TotalItems;

                int retrievalAmount = 0;
                bool partialRetrieval = false;                

                if (spaceAvailable >= entry.Count)                
                    retrievalAmount = entry.Count;

                else
                {
                    partialRetrieval = true;
                    retrievalAmount = spaceAvailable;
                }

                for (int a = 0; a < retrievalAmount; a++)
                {
                    SkillMasteryScroll skillMasteryScroll = new SkillMasteryScroll(entry.skillName);
                    from.Backpack.DropItem(skillMasteryScroll);
                }

                entry.Count -= retrievalAmount;
                from.SendSound(addItemSound);

                if (entry.Count == 1)
                    from.SendMessage("You retrieve a skill mastery scroll from the library.");

                else if (partialRetrieval)
                    from.SendMessage("You retrieve several skill mastery scrolls from the library but require more backpack space in order to retrieve the remaining scrolls.");

                else
                    from.SendMessage("You retrieve several skill mastery scrolls from the library.");
            }

            else
            {
                SkillMasteryScroll skillMasteryScroll = new SkillMasteryScroll(entry.skillName);

                entry.Count--;

                from.Backpack.DropItem(skillMasteryScroll);
                from.SendSound(addItemSound);
                from.SendMessage("You retrieve a skill mastery scroll from the library."); 
            }
        }

        public bool CanUse(Mobile from)
        {
            if (from == null)
                return false;

            if (from.AccessLevel > AccessLevel.Player)
                return true;

            if (!from.Alive)
                return false;

            if (IsChildOf(from.Backpack) || IsChildOf(from.BankBox))
                return true;

            if (from.Map != Map || !from.InRange(GetWorldLocation(), 2))
            {
                from.SendMessage("That is too far away to use.");
                return false;
            }

            if (!from.InLOS(this))
            {
                from.SendMessage("That item is out of your line of sight.");
                return false;
            }

            if (IsLockedDown)
            {
                BaseHouse house = BaseHouse.FindHouseAt(Location, Map, 64);

                if (house == null)
                {
                    from.SendMessage("That is not accessible.");
                    return false;
                }

                switch (m_LockedDownAccessLevel)
                {
                    case LockedDownAccessLevelType.Owner:
                        if (house.Owner != null)
                        {
                            Account ownerAccount = house.Owner.Account as Account;
                            Account playerAccount = from.Account as Account;

                            if (ownerAccount != null && playerAccount != null && ownerAccount == playerAccount)
                                return true;

                            else
                            {
                                from.SendMessage("You do not have the neccessary access rights to use that.");
                                return false;
                            }
                        }
                        break;

                    case LockedDownAccessLevelType.CoOwner:
                        if (house.IsCoOwner(from))
                            return true;
                        else
                        {
                            from.SendMessage("You do not have the neccessary access rights to use that.");
                            return false;
                        }
                        break;

                    case LockedDownAccessLevelType.Friend:
                        if (house.IsFriend(from))
                            return true;

                        else
                        {
                            from.SendMessage("You do not have the neccessary access rights to use that.");
                            return false;
                        }

                        break;

                    case LockedDownAccessLevelType.Anyone:
                        return true;
                        break;
                }
            }

            if (RootParent is PlayerMobile && RootParent != from)
            {
                from.SendMessage("That is not accessible.");
                return false;
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 

            writer.Write((int)m_LockedDownAccessLevel);
            writer.Write(m_RemoveAllOnSelection);

            //Version 0
            writer.Write(m_LibraryEntries.Count);
            for (int a = 0; a < m_LibraryEntries.Count; a++)
            {
                SkillMasteryScrollLibraryEntry entry = m_LibraryEntries[a];

                writer.Write((int)entry.skillName);
                writer.Write(entry.Count);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version
            if (version >= 0)
            {
                m_LockedDownAccessLevel = (LockedDownAccessLevelType)reader.ReadInt();
                m_RemoveAllOnSelection = reader.ReadBool();

                int libraryEntryCount = reader.ReadInt();

                for (int a = 0; a < libraryEntryCount; a++)
                {
                    SkillName skillName = (SkillName)reader.ReadInt();
                    int count = reader.ReadInt();

                    SkillMasteryScrollLibraryEntry entry = new SkillMasteryScrollLibraryEntry();
                    entry.skillName = skillName;
                    entry.Count = count;

                    m_LibraryEntries.Add(entry);
                }
            }

            //-----

            AuditSkillEntries();
        }
    }

    public class SkillMasteryScrollLibraryEntry
    {
        public SkillName skillName = SkillName.Alchemy;
        public int Count = 0;
    }

    public class SkillMasteryScrollLibraryGump : Gump
    {
        PlayerMobile m_Player;
        SkillMasteryScrollLibrary m_Library;

        int m_PageNumber = 1;
        int m_TotalPages = 1;
        int m_TotalEntries = 1;

        int EntriesPerSide = 8;
        int EntriesPerPage = 16;

        int WhiteTextHue = 2499;

        public SkillMasteryScrollLibraryGump(PlayerMobile player, SkillMasteryScrollLibrary library, int pageNumber): base(10, 10)
        {
            if (player == null) return;
            if (library == null) return;
            if (library.Deleted) return;

            m_Player = player;
            m_Library = library;
            m_PageNumber = pageNumber;            

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(205, 193, 11015, 2963);
            AddImage(204, 1, 11015, 2963);
            AddImage(3, 192, 11015, 2963);
            AddImage(3, 1, 11015, 2963);

            AddImage(302, 75, 2081, 2499);
            AddImage(300, 270, 2081, 2499);
            AddImage(301, 141, 2081, 2499);
            AddImage(301, 5, 2081, 2499);
            AddImage(301, 206, 2081, 2499);
            AddImage(299, 338, 2081, 2499);
            AddImage(44, 6, 2081, 2499);
            AddImage(44, 75, 2081, 2499);
            AddImage(43, 141, 2081, 2499);
            AddImage(43, 206, 2081, 2499);
            AddImage(41, 335, 2081);
            AddImage(43, 274, 2081, 2499);            
            AddImage(41, 338, 2081, 2499);
            AddImage(49, 80, 3001, 2615);
            AddImage(56, 80, 3001, 2615);
            AddImage(306, 80, 3001, 2615);
            AddImage(315, 80, 3001, 2615);
            AddImageTiled(301, 2, 6, 405, 2701);

            AddItem(153, 24, 5360, 2963);

            AddLabel(95, 5, 2599, "Skill Mastery Scroll Library");

            AddLabel(88, 53, WhiteTextHue, "Add All in Backpack into Library");
            AddButton(65, 56, 2118, 2118, 1, GumpButtonType.Reply, 0);
            
            AddLabel(354, 5, 2615, "Locked Down Access Level");

            string accessName = "Owner";

            switch (m_Library.LockedDownAccessLevel)
            {
                case SkillMasteryScrollLibrary.LockedDownAccessLevelType.Owner: accessName = "Owner"; break;
                case SkillMasteryScrollLibrary.LockedDownAccessLevelType.CoOwner: accessName = "Co-Owner"; break;
                case SkillMasteryScrollLibrary.LockedDownAccessLevelType.Friend: accessName = "Friend"; break;
                case SkillMasteryScrollLibrary.LockedDownAccessLevelType.Anyone: accessName = "Anyone"; break;
            }

            AddLabel(Utility.CenteredTextOffset(435, accessName), 25, 2562, accessName);

            AddButton(366, 28, 2223, 2223, 2, GumpButtonType.Reply, 0);
            AddButton(488, 29, 2224, 2224, 3, GumpButtonType.Reply, 0);

            AddLabel(347, 53, WhiteTextHue, "Remove All Possible on Selection");
            if (m_Library.RemoveAllOnSelection)
                AddButton(313, 48, 2154, 2151, 4, GumpButtonType.Reply, 0);
            else
                AddButton(313, 48, 2151, 2154, 4, GumpButtonType.Reply, 0);

            //-----

            List<SkillName> m_SkillList = new List<SkillName>();

            foreach (SkillName skillName in SkillMasteryScroll.Skills)
            {
                m_SkillList.Add(skillName);
            }

            m_TotalEntries = m_SkillList.Count;
            m_TotalPages = (int)(Math.Ceiling((double)m_TotalEntries / (double)EntriesPerPage));

            if (m_TotalPages == 0)
                m_TotalPages = 1;

            if (m_PageNumber < 1)
                m_PageNumber = 1;

            if (m_PageNumber > m_TotalPages)
                m_PageNumber = m_TotalPages;

            int startIndex = (m_PageNumber - 1) * EntriesPerPage;
            int endIndex = startIndex + EntriesPerPage;

            if (endIndex > m_TotalEntries)
                endIndex = m_TotalEntries;

            int leftStartY = 88;
            int rightStartY = 88;

            int entryCount = 0;

            for (int a = startIndex; a < endIndex; a++)
            {
                if (a < m_SkillList.Count)
                {
                    SkillName skillName = m_SkillList[a];
                    SkillMasteryScrollLibraryEntry entry = m_Library.GetLibraryEntry(skillName);

                    int numberTextHue = WhiteTextHue;

                    if (entry.Count > 0)
                        numberTextHue = 2963;

                    //Left Side
                    if (entryCount < EntriesPerSide)
                    {
                        AddLabel(60, leftStartY, 2599, SkillCheck.GetSkillName(entry.skillName));
                        AddButton(231, leftStartY + 3, 2118, 2118, 10 + entryCount, GumpButtonType.Reply, 0);
                        AddLabel(249, leftStartY, numberTextHue, entry.Count.ToString());

                        leftStartY += 38;
                    }

                    //Right Side
                    else
                    {
                        AddLabel(317, rightStartY, 2599, SkillCheck.GetSkillName(entry.skillName));
                        AddButton(488, rightStartY + 3, 2118, 2118, 10 + entryCount, GumpButtonType.Reply, 0);
                        AddLabel(506, rightStartY, numberTextHue, entry.Count.ToString());

                        rightStartY += 38;
                    }

                    entryCount++;
                }
            }

            if (m_PageNumber > 1)
                AddButton(160, 380, 4014, 4016, 5, GumpButtonType.Reply, 0);

            if (m_PageNumber < m_TotalPages)
                AddButton(415, 380, 4005, 4007, 6, GumpButtonType.Reply, 0);    
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Backpack == null) return;

            if (m_Library == null) return;
            if (m_Library.Deleted) return;

            if (!m_Library.CanUse(m_Player))
                return;

            List<SkillName> m_SkillList = new List<SkillName>();

            foreach (SkillName skillName in SkillMasteryScroll.Skills)
            {
                m_SkillList.Add(skillName);
            }

            m_TotalEntries = m_SkillList.Count;
            m_TotalPages = (int)(Math.Ceiling((double)m_TotalEntries / (double)EntriesPerPage));

            if (m_TotalPages == 0)
                m_TotalPages = 1;

            if (m_PageNumber < 1)
                m_PageNumber = 1;

            if (m_PageNumber > m_TotalPages)
                m_PageNumber = m_TotalPages;

            bool closeGump = true;

            //-----

            switch (info.ButtonID)
            {
                case 1:
                    //Add All From Backpack
                    m_Library.AddAllScrollsInPack(m_Player);

                    closeGump = false;
                break;

                case 2:
                    //Previous Access Level
                    if (m_Library.IsLockedDown && m_Player.AccessLevel == AccessLevel.Player)    
                        m_Player.SendMessage("You may not change the access level of this item while it is currently locked down.");

                    else
                    {
                        switch (m_Library.LockedDownAccessLevel)
                        {
                            case SkillMasteryScrollLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = SkillMasteryScrollLibrary.LockedDownAccessLevelType.Anyone; break;
                            case SkillMasteryScrollLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = SkillMasteryScrollLibrary.LockedDownAccessLevelType.Owner; break;
                            case SkillMasteryScrollLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = SkillMasteryScrollLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case SkillMasteryScrollLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = SkillMasteryScrollLibrary.LockedDownAccessLevelType.Friend; break;
                        }
                    }

                    closeGump = false;
                break;

                case 3:
                    //Next Access Level
                    if (m_Library.IsLockedDown && m_Player.AccessLevel == AccessLevel.Player)    
                        m_Player.SendMessage("You may not change the access level of this item while it is currently locked down.");

                    else
                    {
                        switch (m_Library.LockedDownAccessLevel)
                        {
                            case SkillMasteryScrollLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = SkillMasteryScrollLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case SkillMasteryScrollLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = SkillMasteryScrollLibrary.LockedDownAccessLevelType.Friend; break;
                            case SkillMasteryScrollLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = SkillMasteryScrollLibrary.LockedDownAccessLevelType.Anyone; break;
                            case SkillMasteryScrollLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = SkillMasteryScrollLibrary.LockedDownAccessLevelType.Owner; break;
                        }
                    }

                    closeGump = false;
                break;

                case 4:
                    //Remove All Possible on Selection
                    m_Library.RemoveAllOnSelection = !m_Library.RemoveAllOnSelection;

                    closeGump = false;
                break;

                case 5:
                    //Previous Page
                    if (m_PageNumber > 1)
                        m_PageNumber--;

                    m_Player.SendSound(m_Library.changeGumpSound);

                    closeGump = false;
                break;

                case 6:
                    //Next Page
                    if (m_PageNumber < m_TotalPages)
                        m_PageNumber++;

                    m_Player.SendSound(m_Library.changeGumpSound);

                    closeGump = false;
                break;
            }

            //Eject Items
            if (info.ButtonID >= 10)
            {
                int index = ((m_PageNumber - 1) * EntriesPerPage) + (info.ButtonID - 10);

                if (index >= m_SkillList.Count || index < 0)
                    return;

                SkillName skillName = m_SkillList[index];
                SkillMasteryScrollLibraryEntry entry = m_Library.GetLibraryEntry(skillName);

                if (entry == null)
                    return;

                bool removeAll = m_Library.RemoveAllOnSelection;

                m_Library.EjectScroll(m_Player, entry.skillName, removeAll);

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(SkillMasteryScrollLibraryGump));
                m_Player.SendGump(new SkillMasteryScrollLibraryGump(m_Player, m_Library, m_PageNumber));
            }

            else
                m_Player.SendSound(m_Library.closeGumpSound);
        }
    }
}