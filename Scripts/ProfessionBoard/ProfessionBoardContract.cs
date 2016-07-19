﻿using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class ProfessionBoardContract : Item
    {
        [Constructable]
        public ProfessionBoardContract(): base(5357)
        {
            Name = "professional contract"; 
        }

        public ProfessionBoardContract(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            //string professionGroupName = ProfessionGroups.GetProfessionGroupName(m_ProfessionGroup);
            //string titleText = "Job Contract from The " + professionGroupName;
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(ProfessionBoardContractGump));
            player.SendGump(new ProfessionBoardContractGump(player));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version         
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {         
            }
        }
    }

    public class ProfessionBoardContractGump : Gump
    {
        public PlayerMobile m_Player;
        public ProfessionGroupType m_ProfessionGroup = ProfessionGroupType.ArtificersEnclave;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x3E6;
        public static int PurchaseSound = 0x2E6;
        public static int CloseGumpSound = 0x058;

        public ProfessionBoardContractGump(PlayerMobile player): base(10, 10)
        {
            if (player == null)
                return;

            m_Player = player;

            AddImage(0, 0, 1249);

            int WhiteTextHue = 2499;

            string professionGroupName = ProfessionGroups.GetProfessionGroupName(m_ProfessionGroup);
            int professionGroupTextHue = ProfessionGroups.GetProfessionGroupTextHue(m_ProfessionGroup);

            string titleText = "Job Contract from The " + professionGroupName;
            string timeRemaining = "23h 17m";
            string destinationText = "Any Alchemist in Prevalia";

            AddLabel(Utility.CenteredTextOffset(220, titleText), 37, professionGroupTextHue, titleText);

            int startX = 115;
            int startY = 52;

            #region Profession Images

            switch (m_ProfessionGroup)
            {
                case ProfessionGroupType.FishermansCircle:
                    AddItem(startX + 34, startY + 19, 3520);
                    AddItem(startX + 66, startY + 48, 3656);
                    AddItem(startX + 35, startY + 36, 2476);
                    AddItem(startX + 76, startY + 39, 2467);
                    AddItem(startX + 45, startY + 35, 15113);
                break;

                case ProfessionGroupType.SmithingOrder:
                    AddItem(startX + 36, startY + 29, 5073);
                    AddItem(startX + 86, startY + 29, 5096);
                    AddItem(startX + 50, startY + 39, 7035);
                    AddItem(startX + 54, startY + 37, 5050);
                    AddItem(startX + 47, startY + 33, 5181);
                break;

                case ProfessionGroupType.TradesmanUnion:
                    AddItem(startX + 29, startY + 27, 4142);
                    AddItem(startX + 37, startY + 23, 4150);
                    AddItem(startX + 61, startY + 35, 2920);
                    AddItem(startX + 49, startY + 25, 2921);
                    AddItem(startX + 67, startY + 47, 4148);
                    AddItem(startX + 48, startY + 31, 4189);
                    AddItem(startX + 57, startY + 27, 2581);
                    AddItem(startX + 36, startY + 20, 2503);
                    AddItem(startX + 45, startY + 14, 4172);
                break;

                case ProfessionGroupType.ArtificersEnclave:
                    AddItem(startX + 62, startY + 30, 2942, 2500);
                    AddItem(startX + 37, startY + 16, 2943, 2500);
                    AddItem(startX + 40, startY + 20, 4031);
                    AddItem(startX + 65, startY + 19, 6237);
                    AddItem(startX + 59, startY + 37, 3626);
                    AddItem(startX + 45, startY + 13, 3643, 2415);
                    AddItem(startX + 40, startY + 29, 5357);
                    AddItem(startX + 44, startY + 31, 5357);
                    AddItem(startX + 65, startY + 43, 3622);
                break;

                case ProfessionGroupType.SeafarersLeague:
                    AddItem(startX + 70, startY + 40, 5370);
                    AddItem(startX + 46, startY + 3, 709);
                break;

                case ProfessionGroupType.AdventurersLodge:
                    AddItem(startX + 57, startY + 24, 4967);
                    AddItem(startX + 49, startY + 35, 4970);
                    AddItem(startX + 64, startY + 49, 2648);
                    AddItem(startX + 34, startY + 38, 5356);
                    AddItem(startX + 40, startY + 45, 3922);
                    AddItem(startX + 1, startY + 30, 3898);
                    AddItem(startX + 50, startY + 25, 5365);
                break;

                case ProfessionGroupType.ZoologicalFoundation:
                    AddItem(startX + 50, startY + 40, 2476);
                    AddItem(startX + 47, startY + 31, 3191);
                    AddItem(startX + 51, startY + 29, 3191);
                    AddItem(startX + 50, startY + 30, 3713);
                break;

                case ProfessionGroupType.ThievesGuild:
                    AddItem(startX + 58, startY + 39, 5373);
                    AddItem(startX + 48, startY + 33, 3589);
                break;

                case ProfessionGroupType.FarmersCooperative:
                    AddItem(startX + 54, startY + 23, 18240);
                break;

                case ProfessionGroupType.MonsterHuntersSociety:
                    AddItem(startX + 32, startY + 26, 7433);
                    AddItem(startX + 34, startY + 38, 4655);
                    AddItem(startX + 54, startY + 23, 7438);
                    AddItem(startX + 27, startY + 40, 7782);
                    AddItem(startX + 44, startY + 38, 3910);
                break;
            }

            #endregion
            
            AddLabel(8, 14, 149, "Guide");
            AddButton(11, 30, 2094, 2095, 1, GumpButtonType.Reply, 0);
                        
            AddLabel(261, 81, 2603, "Job Expires In");
            AddLabel(Utility.CenteredTextOffset(305, timeRemaining), 102, WhiteTextHue, timeRemaining);

            AddLabel(142, 142, 149, "Job Description");

            AddItem(53, 173, 3847); //Image
            AddLabel(109, 163, WhiteTextHue, "Craft 300 Greater Cure Potions");
            AddLabel(119, 183, 2599, "(1 Profession Point Awarded)");

            AddLabel(69, 240, 149, "Turn In");
            AddButton(77, 258, 2151, 2154, 2, GumpButtonType.Reply, 0);

            AddLabel(170, 240, 149, "Completion Destination");
            AddLabel(Utility.CenteredTextOffset(245, destinationText), 260, 2550, destinationText);
        }
    }
}