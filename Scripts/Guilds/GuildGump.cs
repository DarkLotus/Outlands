﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server
{
    public enum GuildGumpPageType
    {
        //Non-Guild
        CreateGuild,
        Invitations,
        InvitationGuildPreview,

        //Guild
        Overview,
        Members,
        MemberManagement,
        Candidates,
        CandidatePreview,
        Diplomacy,
        DiplomacyManagement,
        Faction,
        Messages
    }        

    public class GuildGump : Gump
    {
        public PlayerMobile m_Player;

        public GuildGumpObject m_GuildGumpObject;        
        
        public GuildGump(Mobile from, GuildGumpObject guildGumpObject): base(10, 10)
        {
            m_Player = from as PlayerMobile;
            m_GuildGumpObject = guildGumpObject;            

            if (m_Player == null) return;
            if (m_GuildGumpObject == null) return;

            Guilds.CheckCreateGuildGuildSettings(m_Player);

            m_GuildGumpObject.m_GumpPageType = m_Player.m_GuildSettings.m_GuildGumpPage;
            
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;
            int GreenTextHue = 63;
            int YellowTextHue = 2550;
            int GreyTextHue = 2401;
            int RedTextHue = 2115;

            #region Background

            AddImage(132, 193, 103, 2499);
            AddImage(131, 98, 103, 2499);
            AddImage(132, 288, 103, 2499);
            AddImage(528, 369, 103, 2499);
            AddImage(395, 369, 103, 2499);
            AddImage(265, 369, 103, 2499);
            AddImage(132, 369, 103, 2499);
            AddImage(528, 3, 103, 2499);
            AddImage(528, 98, 103, 2499);
            AddImage(528, 193, 103, 2499);
            AddImage(527, 288, 103, 2499);
            AddImage(132, 3, 103, 2499);
            AddImage(265, 3, 103, 2499);
            AddImage(395, 3, 103, 2499);
            AddImage(395, 98, 103, 2499);
            AddImage(395, 193, 103, 2499);
            AddImage(394, 288, 103, 2499);
            AddImage(265, 98, 103, 2499);
            AddImage(265, 193, 103, 2499);
            AddImage(264, 288, 103, 2499);
            AddImage(3, 194, 103, 2499);
            AddImage(3, 272, 103, 2499);
            AddImage(3, 369, 103, 2499);
            AddImage(2, 3, 103, 2499);
            AddImage(2, 97, 103, 2499);
            AddImage(10, 15, 3604, 2052);
            AddImage(10, 108, 3604, 2052);
            AddImage(10, 213, 3604, 2052);
            AddImage(10, 330, 3604, 2052);
            AddImage(147, 15, 3604, 2052);
            AddImage(147, 108, 3604, 2052);
            AddImage(147, 213, 3604, 2052);
            AddImage(147, 330, 3604, 2052);
            AddImage(286, 15, 3604, 2052);
            AddImage(286, 108, 3604, 2052);
            AddImage(286, 213, 3604, 2052);
            AddImage(286, 330, 3604, 2052);
            AddImage(411, 15, 3604, 2052);
            AddImage(411, 108, 3604, 2052);
            AddImage(411, 213, 3604, 2052);
            AddImage(411, 330, 3604, 2052);
            AddImage(531, 15, 3604, 2052);
            AddImage(531, 108, 3604, 2052);
            AddImage(531, 213, 3604, 2052);
            AddImage(531, 330, 3604, 2052);
            AddImage(184, 15, 3604, 2052);
            AddImage(184, 108, 3604, 2052);
            AddImage(184, 213, 3604, 2052);
            AddImage(184, 330, 3604, 2052);

            #endregion

            #region Sidebar

            List<GuildGumpPageType> m_GuildTabs = Guilds.GetGuildPageTypeList(m_Player);

            int GuildTabsPerPage = 4;
            int TotalGuildTabs = m_GuildTabs.Count;
            int TotalGuildTabPages = (int)(Math.Ceiling((double)TotalGuildTabs / (double)GuildTabsPerPage));

            if (m_GuildGumpObject.m_GuildTabPage >= TotalGuildTabPages)
                m_GuildGumpObject.m_GuildTabPage = TotalGuildTabPages - 1;

            if (m_GuildGumpObject.m_GuildTabPage < 0)
                m_GuildGumpObject.m_GuildTabPage = 0;

            int guildTabStartIndex = m_GuildGumpObject.m_GuildTabPage * GuildTabsPerPage;
            int guildTabEndIndex = (m_GuildGumpObject.m_GuildTabPage * GuildTabsPerPage) + (GuildTabsPerPage - 1);

            if (guildTabEndIndex >= TotalGuildTabs)
                guildTabEndIndex = TotalGuildTabs - 1;

            int guildTabCount = guildTabEndIndex - guildTabStartIndex;

            int startX = 12;
            int startY = 50;

            int tabSpacing = 90;

            for (int a = 0; a < guildTabCount + 1; a++)
            {
                int guildTabIndex = guildTabStartIndex + a;
                int buttonIndex = 20 + guildTabIndex;

                GuildGumpPageType gumpPageType = m_GuildTabs[guildTabIndex];

                #region Guild Tab Images

                int pressedId = 9721;
                int releaseId = 9724;

                bool sameTab = false;

                if (gumpPageType == GuildGumpPageType.Invitations && m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.InvitationGuildPreview)
                    sameTab = true;

                if (gumpPageType == GuildGumpPageType.Candidates && m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.CandidatePreview)
                    sameTab = true;

                if (gumpPageType == GuildGumpPageType.Members && m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.MemberManagement)
                    sameTab = true;

                if (gumpPageType == GuildGumpPageType.Diplomacy && m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.DiplomacyManagement)
                    sameTab = true;

                if (sameTab || gumpPageType == m_Player.m_GuildSettings.m_GuildGumpPage)
                {
                    pressedId = 9724;
                    releaseId = 9721;
                }

                switch (gumpPageType)
                {
                    case GuildGumpPageType.Messages:
                        AddLabel(startX + 36, startY + 5, 2526, "Messages");
                        AddImage(startX + 60, startY + 32, 10550, 55);
                        AddImage(startX + 88, startY + 32, 10552, 55);
                        AddButton(startX + 74, startY + 46, pressedId, releaseId, buttonIndex, GumpButtonType.Reply, 0);
                        AddItem(startX + -1, startY + 18, 7774);
                    break;

                    case GuildGumpPageType.Overview:
                        AddLabel(startX + 34, startY + 5, 2401, "Overview");
                        AddImage(startX + 60, startY + 32, 10550, 2401);
                        AddImage(startX + 88, startY + 32, 10552, 2401);
                        AddButton(startX + 74, startY + 46, pressedId, releaseId, buttonIndex, GumpButtonType.Reply, 0);
                        AddImage(startX + 13, startY + 35, 11016);
                    break;

                    case GuildGumpPageType.Invitations:
                        AddItem(startX + 21, startY + 41, 2942);
                        AddItem(startX + -3, startY + 25, 2943);
                        AddLabel(startX + 12, startY + 5, 2564, "Guild Invitations");
                        AddImage(startX + 60, startY + 32, 10550, 2566);
                        AddImage(startX + 88, startY + 32, 10552, 2566);
                        AddButton(startX + 74, startY + 46, pressedId, releaseId, buttonIndex, GumpButtonType.Reply, 0);
                        AddItem(startX + 9, startY + 31, 5359);
                        AddItem(startX + 23, startY + 24, 4031);
                        AddItem(startX + 23, startY + 50, 5357);
                        AddItem(startX + 27, startY + 52, 5357);
                    break;

                    case GuildGumpPageType.Members:
                        AddLabel(startX + 35, startY + 5, 64, "Members");
                        AddItem(startX + 69, startY + 46, 6877);
                        AddImage(startX + 60, startY + 32, 10550, 2551);
                        AddImage(startX + 88, startY + 32, 10552, 2551);
                        AddButton(startX + 74, startY + 46, pressedId, releaseId, buttonIndex, GumpButtonType.Reply, 0);
                        AddItem(startX + 20, startY + 29, 10905);
                        AddItem(startX + 19, startY + 33, 3746);
                    break;

                    case GuildGumpPageType.Candidates:
                        AddLabel(startX + 27, startY + 4, 2553, "Candidates");
                        AddImage(startX + 60, startY + 32, 10550, 2566);
                        AddImage(startX + 88, startY + 32, 10552, 2566);
                        AddButton(startX + 74, startY + 45, pressedId, releaseId, buttonIndex, GumpButtonType.Reply, 0);
                        AddItem(startX + 28, startY + 45, 4647, 2563);
                        AddItem(startX + 11, startY + 36, 4645, 2563);
                        AddItem(startX + -5, startY + 42, 5018, 2563);
                    break;

                    case GuildGumpPageType.CreateGuild:
                        AddImage(startX + 88, startY + 32, 10552, 2401);
                        AddLabel(startX + 25, startY + 5, 2401, "Create Guild");
                        AddImage(startX + 60, startY + 32, 10550, 2401);
                        AddButton(startX + 74, startY + 46, pressedId, releaseId, buttonIndex, GumpButtonType.Reply, 0);
                        AddItem(startX + 15, startY + 29, 3796);
                    break;

                    case GuildGumpPageType.Faction:
                        AddLabel(startX + 44, startY + 5, 1256, "Faction");
                        AddItem(startX + -15, startY + 20, 3936);
                        AddItem(startX + 18, startY + 21, 18194);
                        AddItem(startX + 10, startY + 13, 5129);
                        AddItem(startX + 32, startY + 31, 18196);
                        AddItem(startX + 9, startY + 30, 5050);
                        AddItem(startX + 26, startY + 22, 5135, 2500);
                        AddImage(startX + 60, startY + 32, 10550, 1256);
                        AddImage(startX + 88, startY + 32, 10552, 1256);
                        AddItem(startX + 26, startY + 47, 7034);
                        AddButton(startX + 74, startY + 46, pressedId, releaseId, buttonIndex, GumpButtonType.Reply, 0);
                        AddItem(startX + -4, startY + 33, 543);
                        AddItem(startX + 14, startY + 45, 543);
                    break;

                    case GuildGumpPageType.Diplomacy:
                        AddItem(startX + 21, startY + 42, 2942);
                        AddItem(startX + -4, startY + 28, 2943);
                        AddItem(startX + 11, startY + 20, 4031);
                        AddItem(startX + 13, startY + 38, 4030);
                        AddItem(startX + 20, startY + 59, 2507);
                        AddItem(startX + 22, startY + 44, 2459);
                        AddLabel(startX + 35, startY + 5, 2606, "Diplomacy");
                        AddImage(startX + 60, startY + 32, 10550, 2606);
                        AddImage(startX + 88, startY + 32, 10552, 2606);
                        AddButton(startX + 74, startY + 46, pressedId, releaseId, buttonIndex, GumpButtonType.Reply, 0);
                    break;
                }

                #endregion

                startY += tabSpacing;
            }

            AddButton(3, 3, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(24, -2, 149, "Guide");

            if (m_GuildGumpObject.m_GuildTabPage > 0)
                AddButton(65, 22, 9900, 9900, 2, GumpButtonType.Reply, 0);

            if (m_GuildGumpObject.m_GuildTabPage < TotalGuildTabPages - 1)
                AddButton(65, 432, 9906, 9906, 3, GumpButtonType.Reply, 0);

            #endregion

            //-----

            #region Create Guild

            if (m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.CreateGuild)
            {
                #region Images

                AddImage(325, 115, 2328, 0);

                AddImage(159, 165, 9809, 0);
                AddItem(485, 115, 1928);
                AddItem(507, 136, 1928);
                AddItem(529, 158, 1928);
                AddItem(551, 180, 1928);
                AddItem(573, 202, 1928);
                AddItem(589, 219, 1928);
                AddItem(466, 134, 1928);
                AddItem(488, 155, 1928);
                AddItem(510, 177, 1928);
                AddItem(532, 199, 1928);
                AddItem(555, 220, 1928);
                AddItem(571, 237, 1928);
                AddItem(445, 154, 1928);
                AddItem(467, 175, 1928);
                AddItem(489, 197, 1928);
                AddItem(511, 219, 1928);
                AddItem(534, 240, 1928);
                AddItem(550, 257, 1928);
                AddItem(424, 176, 1928);
                AddItem(446, 197, 1928);
                AddItem(468, 219, 1928);
                AddItem(490, 241, 1928);
                AddItem(513, 262, 1928);
                AddItem(529, 279, 1928);
                AddItem(606, 235, 1930);
                AddItem(586, 255, 1930);
                AddItem(564, 276, 1930);
                AddItem(544, 295, 1930);
                AddItem(403, 198, 1929);
                AddItem(422, 217, 1929);
                AddItem(443, 238, 1929);
                AddItem(462, 256, 1929);
                AddItem(482, 275, 1929);
                AddItem(504, 295, 1929);
                AddItem(524, 315, 1934);
                AddLabel(212, 86, 149, "Guild Abbreviation");
                AddLabel(212, 46, 149, "Guild Name");
                AddImage(286, 44, 1141, 2400);
                AddImage(335, 85, 2444, 2401);
                AddItem(163, 43, 3796);
                AddLabel(212, 134, 149, "Guild Symbol");
                AddItem(156, 123, 7776);
                AddLabel(369, 16, 149, "Create Guild");
                AddLabel(319, 406, 63, "Confirm and Create Guild");
                AddItem(160, 187, 3823);
                AddLabel(212, 185, 149, "Registration Fee");
                AddItem(470, 146, 6918);
                AddItem(497, 267, 6914);
                AddItem(550, 202, 6916);
                AddItem(516, 291, 6920);
                AddItem(577, 289, 6915);
                AddItem(555, 214, 6917);
                AddItem(523, 344, 6918);
                AddItem(464, 279, 6913);
                AddItem(399, 229, 6916);
                AddItem(486, 158, 6916);
                AddItem(499, 161, 16645);
                AddItem(497, 193, 3823);
                AddItem(592, 232, 6913);
                AddItem(527, 207, 6918);
                AddItem(504, 113, 17074);
                AddItem(412, 170, 633, 2419);
                AddItem(585, 198, 635, 2419);
                AddItem(429, 142, 636, 2419);
                AddItem(604, 153, 16119);
                AddItem(563, 264, 634, 2419);
                AddItem(547, 274, 633, 2419);
                AddItem(592, 252, 6915);
                AddItem(471, 341, 3791);
                AddItem(463, 154, 3786);
                AddItem(378, 214, 3792);
                AddItem(609, 197, 3814);
                AddItem(562, 236, 3790);
                AddItem(442, 144, 3812);
                AddItem(243, 303, 3816);
                AddItem(265, 320, 3808);
                AddItem(276, 275, 3816);
                AddItem(298, 292, 3808);
                AddItem(243, 303, 3816);
                AddItem(265, 320, 3808);
                AddItem(308, 243, 3816);
                AddItem(331, 259, 3808);
                AddItem(297, 230, 3799);
                AddItem(206, 331, 3816);
                AddItem(228, 346, 3808);
                AddItem(199, 313, 4455);
                AddItem(287, 343, 2322);
                AddItem(312, 293, 2322);
                AddItem(208, 355, 2322);
                AddItem(351, 263, 7685, 2401);
                AddItem(330, 325, 7685, 2401);
                AddItem(252, 360, 7685, 2401);
                AddItem(379, 272, 7684, 2401);
                AddItem(475, 310, 7684, 2401);
                AddItem(231, 388, 7684, 2401);
                AddItem(385, 348, 7684, 2401);
                AddItem(447, 321, 7684, 2401);
                AddItem(421, 321, 7682, 2401);
                AddItem(243, 338, 7684, 1107);
                AddItem(292, 326, 7682, 2401);
                AddItem(469, 254, 7684, 2401);
                AddItem(331, 244, 7681, 2415);
                AddItem(232, 324, 7684, 2401);
                AddItem(269, 291, 7684, 2401);
                AddItem(224, 305, 7684, 2401);
                AddItem(331, 287, 7682, 2401);
                AddItem(365, 300, 7684, 2401);
                AddItem(418, 271, 7684, 2401);
                AddItem(601, 178, 7574);
                AddItem(496, 309, 7570);
                AddItem(240, 271, 4465);
                AddItem(272, 253, 4479);
                AddItem(505, 335, 4651);
                AddItem(441, 258, 6937);
                AddItem(397, 288, 6927);
                AddItem(311, 289, 6925);
                AddItem(348, 341, 6922);
                AddItem(214, 346, 6930);
                AddItem(226, 353, 6938);
                AddItem(276, 353, 3786);
                AddItem(391, 254, 6938);
                AddItem(472, 228, 3793);
                AddItem(295, 371, 7684, 2401);
                AddItem(196, 367, 7685, 2401);
                AddItem(368, 241, 7684, 2401);
                AddItem(368, 324, 7685, 2401);
                AddItem(492, 199, 2507, 2415);
                AddItem(507, 199, 3880);
                AddItem(493, 208, 4234, 2499);
                AddItem(436, 92, 3938);
                AddItem(476, 102, 18236);
                AddItem(470, 96, 5128, 2401);
                AddItem(457, 124, 7032);
                AddItem(549, 150, 18194);

                AddItem(541, 142, 5135, 2500);

                AddItem(530, 152, 5114);
                AddItem(532, 248, 18196);
                AddItem(531, 268, 7034);

                AddItem(522, 240, 5133, 2413);

                AddItem(513, 253, 5178);

                AddItem(442, 211, 18210, 2500);
                AddItem(445, 208, 5049, 2500);
                AddItem(437, 199, 5138, 2500);
                AddItem(426, 221, 7028, 2500);

                AddItem(550, 163, 7030);
                AddItem(345, 312, 6930);
                AddItem(261, 330, 6934);
                AddItem(400, 332, 6924);
                AddItem(424, 312, 6883);
                AddItem(310, 352, 6881);
                AddItem(405, 217, 7684, 2401);
                AddItem(341, 273, 3788);
                AddItem(441, 350, 7685, 2401);
                AddItem(467, 336, 7684, 2401);
                AddItem(518, 298, 17075);
                AddItem(538, 205, 3788);
                AddItem(334, 304, 3788);
                AddItem(369, 259, 3790);
                AddItem(263, 331, 6930);
                AddItem(298, 295, 6938);
                AddItem(276, 294, 6883);
                AddItem(480, 293, 7684, 2401);
                AddItem(405, 235, 7684, 2401);
                AddItem(453, 185, 7684, 2401);
                AddItem(304, 325, 3793);

                #endregion

                //-----

                if (m_Player.Guild != null) return;
                if (m_GuildGumpObject.m_Guild != null) return;

                AddTextEntry(295, 46, 248, 20, WhiteTextHue, 7, guildGumpObject.m_CreateGuildName, Guilds.GuildNameCharacterLimit);
                AddTextEntry(355, 86, 47, 20, WhiteTextHue, 8, guildGumpObject.m_CreateGuildAbbreviation, Guilds.GuildAbbreviationCharacterLimit);

                 List<GuildSymbolType> availableGuildSymbols = GuildSymbols.GetDefaultGuildSymbols();

                int totalGuildSymbols = availableGuildSymbols.Count;

                if (guildGumpObject.m_CreateGuildSymbolIndex < 0)
                    guildGumpObject.m_CreateGuildSymbolIndex = totalGuildSymbols - 1;

                if (guildGumpObject.m_CreateGuildSymbolIndex >= totalGuildSymbols)
                    guildGumpObject.m_CreateGuildSymbolIndex = 0;               

                GuildSymbolType symbolType = availableGuildSymbols[guildGumpObject.m_CreateGuildSymbolIndex];
                GuildSymbolDetail guildSymbolDetail = GuildSymbols.GetGuildSymbolDetail(symbolType);

                guildGumpObject.m_CreateGuildSymbol = symbolType;
                
                AddButton(298, 137, 2223, 2223, 4, GumpButtonType.Reply, 0); //Previous Symbol
                AddItem(325 + guildSymbolDetail.SymbolIconOffsetX, 115 + guildSymbolDetail.SymbolIconOffsetY, guildSymbolDetail.SymbolIconItemId, guildSymbolDetail.SymbolIconHue);
                AddButton(411, 137, 2224, 2224, 5, GumpButtonType.Reply, 0); //Next Symbol

                AddLabel(341, 185, GreenTextHue, Utility.CreateCurrencyString(Guilds.GuildRegistrationFee));

                AddButton(366, 429, 247, 249, 6, GumpButtonType.Reply, 0); //Create Guild
            }

            #endregion

            #region Invitations

            else if (m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.Invitations)
            {
                #region Images

                AddLabel(345, 15, 2563, "Guild Invitations");               
                AddImage(299, 39, 2446, 2401);
                AddLabel(508, 39, 149, "Search for Guild Name");
                AddLabel(360, 400, 149, "Page");
                AddLabel(356, 429, 63, "Ignore Guild Invitations");
                AddLabel(157, 400, 2599, "Total Invitations");
                AddLabel(289, 70, 149, "Guild Name");
                AddLabel(605, 70, 149, "Decline");
                AddLabel(534, 70, 149, "Guild Info");
                AddLabel(158, 70, 149, "Accept");
                AddLabel(458, 70, 149, "Expires In");

                #endregion

                //-----

                if (m_Player.Guild != null) return;
                if (m_GuildGumpObject.m_Guild != null) return;

                if (m_GuildGumpObject.m_InvitationSortCriteria == Guilds.InvitationSortCriteria.None)
                {
                    m_GuildGumpObject.m_InvitationSortCriteria = Guilds.InvitationSortCriteria.GuildName;
                    m_GuildGumpObject.m_InvitationSortAscending = true;
                }

                m_Player.m_GuildSettings.AuditInvitations();

                m_GuildGumpObject.m_InvitationsSorted = m_Player.m_GuildSettings.GetInvitations(m_GuildGumpObject.m_InvitationSortCriteria, m_GuildGumpObject.m_InvitationSortAscending);
                
                int invitationsPerPage = 12;
                int totalInvitations = m_GuildGumpObject.m_InvitationsSorted.Count;
                int totalInvitationPages = (int)(Math.Ceiling((double)totalInvitations / (double)invitationsPerPage));

                if (m_GuildGumpObject.m_InvitationPage >= totalInvitationPages)
                    m_GuildGumpObject.m_InvitationPage = totalInvitationPages - 1;

                if (m_GuildGumpObject.m_InvitationPage < 0)
                    m_GuildGumpObject.m_InvitationPage = 0;

                int invitationStartIndex = m_GuildGumpObject.m_InvitationPage * invitationsPerPage;
                int invitationEndIndex = (m_GuildGumpObject.m_InvitationPage * invitationsPerPage) + (invitationsPerPage - 1);

                if (invitationEndIndex >= totalInvitations)
                    invitationEndIndex = totalInvitations - 1;

                int invitationDisplayCount = invitationEndIndex - invitationStartIndex;

                int rowSpacing = 30;

                startY = 90;

                //-----                

                AddButton(271, 40, 9909, 9909, 4, GumpButtonType.Reply, 0); //Search Left
                AddTextEntry(308, 40, 158, 20, WhiteTextHue, 10, "Guild Name", Guilds.GuildNameCharacterLimit);
                AddButton(483, 39, 9903, 9903, 5, GumpButtonType.Reply, 0); //Search Right

                switch(guildGumpObject.m_InvitationSortCriteria)
                {
                    case Guilds.InvitationSortCriteria.GuildName:
                        if (guildGumpObject.m_InvitationSortAscending)
                            AddButton(268, 73, 5600, 5600, 6, GumpButtonType.Reply, 0); //Guild Name Sort

                        else
                            AddButton(268, 73, 5602, 5602, 6, GumpButtonType.Reply, 0); //Guild Name Sort

                        AddButton(438, 73, 2117, 2118, 7, GumpButtonType.Reply, 0); //Expiration Sort
                    break;

                    case Guilds.InvitationSortCriteria.Expiration:
                         AddButton(268, 73, 2117, 2118, 6, GumpButtonType.Reply, 0); //Guild Name Sort

                        if (guildGumpObject.m_InvitationSortAscending)
                            AddButton(438, 73, 5600, 5600, 7, GumpButtonType.Reply, 0); //Expiration Sort

                        else
                            AddButton(438, 73, 5602, 5602, 7, GumpButtonType.Reply, 0); //Expiration Sort
                    break;
                }

                for (int a = 0; a < invitationDisplayCount + 1; a++)
                {
                    int invitationIndex = invitationStartIndex + a;

                    if (invitationStartIndex >= totalInvitations)
                        continue;

                    GuildInvitation invitationItem = m_GuildGumpObject.m_InvitationsSorted[invitationIndex];                    

                    if (invitationItem != null)
                    {
                        int buttonIndex = m_GuildGumpObject.m_InvitationButtonIndexOffset + (a * 10);

                        DateTime expirationDate = invitationItem.m_InvitationTime + Guilds.InvitationExpiration;

                        string guildName = invitationItem.m_Guild.GetDisplayName(true);
                        string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, expirationDate, true, true, true, true, false);

                        if (invitationItem.m_Accepted)
                            AddButton(165, startY, 9724, 9721, buttonIndex, GumpButtonType.Reply, 0);

                        else
                            AddButton(165, startY, 9721, 9724, buttonIndex, GumpButtonType.Reply, 0);

                        AddLabel(Utility.CenteredTextOffset(330, guildName), startY + 7, WhiteTextHue, guildName);
                        AddLabel(Utility.CenteredTextOffset(485, timeRemaining), startY + 5, WhiteTextHue, timeRemaining);
                        AddButton(550, startY + 5, 4011, 4013, buttonIndex + 1, GumpButtonType.Reply, 0);
                        AddButton(614, startY, 2472, 2473, buttonIndex + 2, GumpButtonType.Reply, 0);
                    }

                    startY += rowSpacing;
                }               

                //--

                AddLabel(270, 400, WhiteTextHue, totalInvitations.ToString()); //Total Invitations

                if (totalInvitationPages > 0)
                    AddLabel(397, 400, WhiteTextHue, (guildGumpObject.m_InvitationPage + 1).ToString() + "/" + totalInvitationPages.ToString()); //Page

                if (guildGumpObject.m_InvitationPage > 0)
                {
                    AddLabel(188, 429, WhiteTextHue, "Previous Page");
                    AddButton(154, 429, 4014, 4016, 8, GumpButtonType.Reply, 0); //Previous Page
                }

                if (guildGumpObject.m_InvitationPage < totalInvitationPages - 1)
                {
                    AddLabel(552, 429, WhiteTextHue, "Next Page");
                    AddButton(620, 429, 4005, 4007, 9, GumpButtonType.Reply, 0); //Next Page
                }

                //Ignore Guild Invites
                if (m_Player.m_GuildSettings.m_IgnoreGuildInvitations)
                    AddButton(322, 426, 9724, 9721, 10, GumpButtonType.Reply, 0);
                else
                    AddButton(322, 426, 9721, 9724, 10, GumpButtonType.Reply, 0);
            }

            #endregion

            #region Invitation Guild Preview

            else if (m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.InvitationGuildPreview)
            {
                #region Images 

                AddLabel(197, 159, 149, "Guildmaster");
                AddLabel(197, 61, 149, "Guild Abbreviation");
                AddLabel(197, 25, 149, "Guild Name");
                AddImage(271, 23, 1141, 2400);                
                AddImage(320, 60, 2444, 2401);                
                AddItem(151, 22, 3796);                       
                AddLabel(197, 109, 149, "Guild Symbol");
                AddItem(144, 98, 7776);
                AddLabel(197, 203, 149, "Guildhouse");

                AddImage(278, 160, 2446, 2401);      
                AddImage(278, 202, 2446, 2401);

                AddLabel(515, 109, 149, "Faction");
                AddBackground(158, 335, 493, 23, 5100);
                AddLabel(299, 245, 2501, "Players");
                AddLabel(382, 245, 2505, "Characters");
                AddLabel(493, 245, 1256, "Wars");
                AddLabel(553, 245, 2599, "Alliances");               
                AddLabel(197, 245, 149, "Guild Age");                
                AddItem(164, 238, 3103);               
                AddItem(482, 80, 5402);
                AddImage(311, 90, 2328);                
                AddItem(296, 269, 8454, 2500);
                AddItem(383, 269, 8454, 2515);                
                AddItem(404, 269, 8455, 2515);
                AddItem(493, 280, 3914);
                AddItem(479, 281, 5049);
                AddItem(558, 283, 4030);
                AddItem(558, 275, 4031);
                AddItem(156, 191, 8900);
                AddItem(153, 150, 4810, 2563);

                #endregion

                //-----

                if (m_Player.Guild != null) return;
                if (m_GuildGumpObject.m_Guild != null) return;
                if (m_GuildGumpObject.m_InvitationGuildInvitation == null) return;
                if (m_GuildGumpObject.m_InvitationGuildInvitation.Deleted) return;

                Guild guild = m_GuildGumpObject.m_InvitationGuildInvitation.m_Guild;

                if (guild == null)
                    return;

                AddLabel(Utility.CenteredTextOffset(405, guild.Name), 25, WhiteTextHue, guild.Name);
                AddLabel(Utility.CenteredTextOffset(352, guild.m_Abbreviation), 61, WhiteTextHue, guild.m_Abbreviation);

                GuildSymbolDetail guildSymbolDetail = GuildSymbols.GetGuildSymbolDetail(guild.m_GuildSymbol);

                AddItem(311 + guildSymbolDetail.SymbolIconOffsetX, 90 + guildSymbolDetail.SymbolIconOffsetY, guildSymbolDetail.SymbolIconItemId, guildSymbolDetail.SymbolIconHue);

                //Faction
                AddItem(600, 91, 11009, 2603);
                AddItem(555, 58, 17099, 2603);
                AddLabel(586, 144, 2603, "Order");

                string guildmasterName = "";

                if (guild.m_Guildmaster != null)
                    guildmasterName = guild.m_Guildmaster.RawName;

                AddLabel(Utility.CenteredTextOffset(365, guildmasterName), 161, WhiteTextHue, guildmasterName);

                if (guild.m_Guildhouse != null)
                {
                    string locationText = "(located at " + guild.m_Guildhouse.Location.X.ToString() + "," + guild.m_Guildhouse.Location.Y.ToString() + ")";

                    AddLabel(Utility.CenteredTextOffset(365, locationText), 203, WhiteTextHue, locationText);
                    AddButton(464, 206, 2117, 248, 4, GumpButtonType.Reply, 0);
                    AddLabel(484, 203, 2550, "Show Location");
                }

                AddLabel(196, 270, 0, guild.GetGuildAge());

                AddLabel(332, 284, WhiteTextHue, guild.m_ActivePlayers.ToString());
                AddLabel(443, 285, WhiteTextHue, guild.m_ActiveCharacters.ToString());
                AddLabel(529, 285, WhiteTextHue, guild.GetWarCount(true).ToString());
                AddLabel(599, 285, WhiteTextHue, guild.GetAllyCount(true).ToString());

                if (guild.m_Website != null)
                {
                    AddLabel(197, 312, 149, "Website");
                    AddButton(165, 315, 30008, 248, 5, GumpButtonType.Reply, 0);
                    AddLabel(167, 336, WhiteTextHue, guild.m_Website);
                }
               
                if (m_GuildGumpObject.m_InvitationGuildInvitation.m_Accepted)                
                    AddButton(294, 373, 9724, 9721, 6, GumpButtonType.Reply, 0);
                else
                    AddButton(294, 373, 9721, 9724, 6, GumpButtonType.Reply, 0);
                AddLabel(327, 376, 63, "Accept Guild Invitation");

                AddButton(296, 410, 2472, 2473, 7, GumpButtonType.Reply, 0);
                AddLabel(327, 413, 2115, "Decline Guild Invitation");

                AddButton(154, 429, 4014, 4019, 8, GumpButtonType.Reply, 0);
                AddLabel(188, 429, WhiteTextHue, "Return");

                //-----
            }

            #endregion

            #region Overview

            else if (m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.Overview)
            {
                #region Images

                AddImage(311, 90, 2328, 0);
                AddItem(155, 384, 4647);
                AddItem(156, 191, 8900);
                AddItem(153, 150, 4810, 2563);
                AddLabel(197, 159, 149, "Guildmaster");
                AddLabel(197, 61, 149, "Guild Abbreviation");
                AddLabel(197, 25, 149, "Guild Name");
                AddImage(271, 23, 1141, 2400);
                AddImage(320, 60, 2444, 2401);
                AddItem(151, 22, 3796);
               
                AddLabel(197, 109, 149, "Guild Symbol");
                AddItem(144, 98, 7776);
                AddLabel(197, 203, 149, "Guildhouse");

                AddImage(278, 160, 2446, 2401);
                AddImage(278, 202, 2446, 2401);
                AddImage(278, 391, 2446, 2401);

                AddLabel(515, 109, 149, "Faction");
                AddBackground(158, 347, 493, 23, 5100);

                AddLabel(197, 324, 149, "Website");
                AddLabel(197, 392, 149, "My Rank");

                AddItem(459, 397, 3034, 2562);
                AddItem(463, 380, 2978);
                AddLabel(508, 392, 149, "Show Guild Title");                

                AddLabel(310, 429, 2115, "Resign from Guild");
                AddItem(482, 80, 5402);

                AddLabel(299, 257, 2506, "Players");
                AddLabel(382, 257, 2502, "Characters");
                AddLabel(496, 257, 1256, "Wars");
                AddLabel(556, 257, 2599, "Alliances");

                AddLabel(197, 257, 149, "Guild Age");

                AddItem(164, 250, 3103);
                AddItem(296, 281, 8454, 2500);
                AddItem(383, 281, 8454, 2515);

                AddItem(404, 281, 8455, 2515);
                AddItem(493, 292, 3914);
                AddItem(479, 293, 5049);
                AddItem(558, 295, 4030);
                AddItem(558, 287, 4031);

                #endregion

                Guild guild = m_GuildGumpObject.m_Guild;

                if (guild == null)
                    return;

                GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(m_Player);

                if (guildMemberEntry == null)
                    return;

                AddLabel(Utility.CenteredTextOffset(405, guild.Name), 25, WhiteTextHue, guild.Name);
                AddLabel(Utility.CenteredTextOffset(352, guild.m_Abbreviation), 61, WhiteTextHue, guild.m_Abbreviation);

                GuildSymbolDetail guildSymbolDetail = GuildSymbols.GetGuildSymbolDetail(guild.m_GuildSymbol);
                
                AddItem(311 + guildSymbolDetail.SymbolIconOffsetX, 90 + guildSymbolDetail.SymbolIconOffsetY, guildSymbolDetail.SymbolIconItemId, guildSymbolDetail.SymbolIconHue);  

                //Faction
                AddItem(555, 58, 17099, 2603); //Flag
                AddItem(600, 91, 11009, 2603); //Shield
                AddLabel(586, 144, 2603, "Order");

                string guildmasterName = "";

                if (guild.m_Guildmaster != null)
                    guildmasterName = guild.m_Guildmaster.RawName;

                AddLabel(Utility.CenteredTextOffset(365, guildmasterName), 161, WhiteTextHue, guildmasterName);
                                if (m_GuildGumpObject.m_Guild.m_Guildhouse != null)    
                {
                    string locationText = "(Located at " + guild.m_Guildhouse.Location.X.ToString() + "," + guild.m_Guildhouse.Location.Y.ToString() + ")";
                    AddLabel(Utility.CenteredTextOffset(365, locationText), 203, WhiteTextHue, locationText);
                
                    AddLabel(464, 203, 2550, "Show Location");
                    AddButton(484, 206, 2117, 2118, 4, GumpButtonType.Reply, 0); //Show Guildhouse Location
                }
                
                int activePlayers = guild.m_ActivePlayers;
                int activeCharacters = guild.m_ActiveCharacters;
                int wars = guild.GetWarCount(true);
                int alliances = guild.GetAllyCount(true);

                AddLabel(196, 282, WhiteTextHue, guild.GetGuildAge());
                AddLabel(332, 296, WhiteTextHue, activePlayers.ToString());
                AddLabel(443, 297, WhiteTextHue, activeCharacters.ToString());
                AddLabel(529, 297, WhiteTextHue, wars.ToString());
                AddLabel(599, 297, WhiteTextHue, alliances.ToString());

                AddButton(165, 327, 30008, 30009, 5, GumpButtonType.Reply, 0); //Launch Website
                AddLabel(167, 348, WhiteTextHue, guild.m_Website); //Website

                string rankName = "";

                int rankHue = WhiteTextHue;
                
                rankName = guild.GetRankName(guildMemberEntry.m_Rank);
                rankHue = guild.GetRankHue(guildMemberEntry.m_Rank);                

                AddLabel(Utility.CenteredTextOffset(365, rankName), 392, rankHue, rankName); //Guild Rank

                if (m_Player.m_GuildSettings.m_ShowGuildTitle)
                    AddButton(615, 389, 9724, 9721, 6, GumpButtonType.Reply, 0); //Show Guild Title
                else
                    AddButton(615, 389, 9721, 9724, 6, GumpButtonType.Reply, 0); //Show Guild Title

                AddButton(425, 425, 2472, 2473, 7, GumpButtonType.Reply, 0); //Resign from Guild               
            }

            #endregion

            #region Members

            else if (m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.Members)
            {
                #region Images

                AddLabel(365, 15, 2563, "Members");
                AddLabel(280, 70, 149, "Player Name");
                AddLabel(407, 70, 149, "Guild Rank");               
                AddLabel(373, 429, 63, "Invite Player to Guild");
                AddLabel(170, 70, 149, "Last Online");
                AddLabel(556, 70, 149, "Fealty");
                AddLabel(604, 70, 149, "Dismiss");
                AddLabel(503, 70, 149, "Manage");
                AddImage(299, 39, 2446, 2401);
                AddLabel(508, 39, 149, "Search for Player");
                AddLabel(360, 400, 149, "Page");
                AddLabel(526, 400, 2550, "Total Members");
                AddLabel(157, 400, 2599, "Online Members");

                #endregion

                //-----

                Guild guild = m_GuildGumpObject.m_Guild;

                if (guild == null)
                    return;

                GuildMemberEntry playerEntry = guild.GetGuildMemberEntry(m_Player);

                if (playerEntry == null)
                    return;

                if (m_GuildGumpObject.m_MemberSortCriteria == Guilds.MemberSortCriteria.None)
                {
                    m_GuildGumpObject.m_MemberSortCriteria = Guilds.MemberSortCriteria.LastOnline;
                    m_GuildGumpObject.m_MemberSortAscending = false;
                }

                guild.AuditMembers();

                m_GuildGumpObject.m_MembersSorted = guild.GetGuildMemberEntries(m_GuildGumpObject.m_MemberSortCriteria, m_GuildGumpObject.m_MemberSortAscending);

                int membersPerPage = 12;
                int totalMembers = m_GuildGumpObject.m_MembersSorted.Count;
                int totalMemberPages = (int)(Math.Ceiling((double)totalMembers / (double)membersPerPage));

                if (m_GuildGumpObject.m_MemberPage >= totalMemberPages)
                    m_GuildGumpObject.m_MemberPage = totalMemberPages - 1;

                if (m_GuildGumpObject.m_MemberPage < 0)
                    m_GuildGumpObject.m_MemberPage = 0;

                int memberStartIndex = m_GuildGumpObject.m_MemberPage * membersPerPage;
                int memberEndIndex = (m_GuildGumpObject.m_MemberPage * membersPerPage) + (membersPerPage - 1);

                if (memberEndIndex >= totalMembers)
                    memberEndIndex = totalMembers - 1;

                int memberDisplayCount = memberEndIndex - memberStartIndex;

                int rowSpacing = 30;

                startY = 90;

                //-----

                AddButton(271, 40, 9909, 9909, 4, GumpButtonType.Reply, 0); //Search Left
                AddTextEntry(308, 40, 158, 20, WhiteTextHue, 12, "Player Name", Guilds.GuildNameCharacterLimit);
                AddButton(483, 39, 9903, 9903, 5, GumpButtonType.Reply, 0); //Search Right

                switch (guildGumpObject.m_MemberSortCriteria)
                {
                    case Guilds.MemberSortCriteria.LastOnline:
                        if (guildGumpObject.m_MemberSortAscending)
                            AddButton(151, 73, 5600, 5600, 6, GumpButtonType.Reply, 0); //Sort Last Online

                        else
                            AddButton(151, 73, 5602, 5602, 6, GumpButtonType.Reply, 0); //Sort Online

                        AddButton(259, 73, 2117, 2118, 7, GumpButtonType.Reply, 0); //Sort Player Name
                        AddButton(387, 73, 2117, 2118, 8, GumpButtonType.Reply, 0); //Sort Rank
                    break;

                    case Guilds.MemberSortCriteria.PlayerName:
                        AddButton(151, 73, 2117, 2118, 6, GumpButtonType.Reply, 0); //Sort Last Online

                        if (guildGumpObject.m_MemberSortAscending)
                            AddButton(259, 73, 5600, 5600, 7, GumpButtonType.Reply, 0); //Sort Player Name

                        else
                            AddButton(259, 73, 5602, 5602, 7, GumpButtonType.Reply, 0); //Sort Player Name

                        AddButton(387, 73, 2117, 2118, 8, GumpButtonType.Reply, 0); //Sort Rank
                    break;

                    case Guilds.MemberSortCriteria.GuildRank:
                        AddButton(151, 73, 2117, 2118, 6, GumpButtonType.Reply, 0); //Sort Last Online
                        AddButton(259, 73, 2117, 2118, 7, GumpButtonType.Reply, 0); //Sort Player Name

                        if (guildGumpObject.m_MemberSortAscending)
                            AddButton(387, 73, 5600, 5600, 8, GumpButtonType.Reply, 0); //Sort Rank

                        else
                            AddButton(387, 73, 5602, 5602, 8, GumpButtonType.Reply, 0); //Sort Rank
                    break;
                }

                for (int a = 0; a < memberDisplayCount + 1; a++)
                {
                    int memberIndex = memberStartIndex + a;

                    if (memberStartIndex >= totalMembers)
                        continue;

                    GuildMemberEntry memberEntry = m_GuildGumpObject.m_MembersSorted[memberIndex];                                     

                    if (memberEntry != null)
                    {
                        int buttonIndex = m_GuildGumpObject.m_MemberButtonIndexOffset + (a * 10);   

                        PlayerMobile guildMember = memberEntry.m_Player;

                        if (guildMember != null)
                        {
                            string rankName = guild.GetRankName(memberEntry.m_Rank);
                            int rankHue = guild.GetRankHue(memberEntry.m_Rank);

                            string lastOnlineText = "";

                            TimeSpan timeSinceOnline = DateTime.UtcNow - guildMember.LastOnline;

                            //Online
                            if (guildMember.NetState != null)
                            {
                                AddButton(163, startY + 8, 2361, 2361, buttonIndex, GumpButtonType.Reply, 0);
                                AddLabel(185, startY + 5, GreenTextHue, "Online");
                            }

                            //Offline
                            else if (guild.IsCharacterActive(guildMember))
                            {
                                if (timeSinceOnline.TotalDays >= 1)
                                    lastOnlineText = Utility.CreateTimeRemainingString(guildMember.LastOnline, DateTime.UtcNow, true, true, true, false, false) + " Ago";
                                else
                                    lastOnlineText = Utility.CreateTimeRemainingString(guildMember.LastOnline, DateTime.UtcNow, true, true, true, true, false) + " Ago";
                                
                                AddButton(163, startY + 8, 2362, 2362, buttonIndex, GumpButtonType.Reply, 0);
                                AddLabel(185, startY + 5, GreyTextHue, lastOnlineText);
                            }

                            //Inactive
                            else
                            {
                                if (timeSinceOnline.TotalDays >= 1)
                                    lastOnlineText = Utility.CreateTimeRemainingString(guildMember.LastOnline, DateTime.UtcNow, true, true, true, false, false) + " Ago";
                                else
                                    lastOnlineText = Utility.CreateTimeRemainingString(guildMember.LastOnline, DateTime.UtcNow, true, true, true, true, false) + " Ago";

                                AddButton(163, startY + 8, 2360, 2360, buttonIndex, GumpButtonType.Reply, 0);
                                AddLabel(185, startY + 5, RedTextHue, lastOnlineText);
                            }

                            AddLabel(Utility.CenteredTextOffset(320, guildMember.RawName), startY + 5, WhiteTextHue, guildMember.RawName); //PlayerName
                            AddLabel(Utility.CenteredTextOffset(440, rankName), startY + 5, rankHue, rankName); //Rank
                            
                            //Manage Player
                            AddButton(512, startY + 5, 4011, 4013, buttonIndex + 1, GumpButtonType.Reply, 0);

                            //Declare Fealty
                            if (playerEntry.m_DeclaredFealty == guildMember)
                                AddButton(564, startY, 9724, 9721, buttonIndex + 2, GumpButtonType.Reply, 0);

                            else
                                AddButton(564, startY, 9721, 9724, buttonIndex + 2, GumpButtonType.Reply, 0);

                            //Dismiss Member
                            AddButton(614, startY, 2472, 2473, buttonIndex + 3, GumpButtonType.Reply, 0);
                        }
                    }

                    startY += rowSpacing;
                }

                int onlineMembers = 0;

                foreach (GuildMemberEntry memberEntry in m_GuildGumpObject.m_MembersSorted)
                {
                    if (memberEntry == null) continue;
                    if (memberEntry.m_Player == null) continue;
                    if (memberEntry.m_Player.Deleted) continue;
                    if (memberEntry.m_Player.NetState != null)
                        onlineMembers++;
                }

                AddLabel(263, 400, WhiteTextHue, onlineMembers.ToString()); //Online Members

                if (totalMembers > 0)
                    AddLabel(397, 400, WhiteTextHue, (guildGumpObject.m_MemberPage + 1).ToString() + "/" + totalMemberPages.ToString()); //Page

                AddLabel(623, 400, WhiteTextHue, totalMembers.ToString()); //Total Members

                if (guildGumpObject.m_MemberPage > 0)
                {
                    AddLabel(188, 429, WhiteTextHue, "Previous Page");
                    AddButton(154, 429, 4014, 4016, 9, GumpButtonType.Reply, 0); //Previous Page
                }

                if (guildGumpObject.m_MemberPage < totalMemberPages - 1)
                {
                    AddLabel(552, 429, WhiteTextHue, "Next Page");
                    AddButton(620, 429, 4005, 4007, 10, GumpButtonType.Reply, 0); //Next Page
                }

                AddButton(337, 429, 4002, 4004, 11, GumpButtonType.Reply, 0); //Invite Player
            }

            #endregion

            #region Member Management

            else if (m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.MemberManagement)
            {
                Guild guild = m_GuildGumpObject.m_Guild;

                if (guild == null)
                    return;

                GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(m_Player);               

                if (guildMemberEntry == null)
                    return;

                GuildMemberEntry guildMemberTargetEntry = m_GuildGumpObject.m_MemberManagementPlayer;

                if (guildMemberTargetEntry == null) return;
                if (guildMemberTargetEntry.m_Player == null) return;
                if (guildMemberTargetEntry.m_Player.Deleted) return;

                PlayerMobile playerTarget = guildMemberTargetEntry.m_Player;
                
                AddLabel(Utility.CenteredTextOffset(395, playerTarget.RawName), 23, 63, playerTarget.RawName);

                string timeText = "Member for " + Utility.CreateTimeRemainingString(playerTarget.CreatedOn, DateTime.UtcNow, false, true, true, false, false);
                AddLabel(Utility.CenteredTextOffset(395, timeText), 44, 2550, timeText);

                if (playerTarget.Female)
                    AddItem(369, 74, 8455, 2500);
                else
                    AddItem(369, 74, 8454, 2500);

                int possibleRanks = Enum.GetNames(typeof(GuildMemberRank)).Length;

                int newIndexRank = guildGumpObject.m_MemberManagementNewRankIndex;

                if (newIndexRank < 0)
                    newIndexRank = 0;

                if (newIndexRank >= possibleRanks)
                    newIndexRank = possibleRanks - 1;

                GuildMemberRank guildMemberRank = (GuildMemberRank)newIndexRank;

                string guildRankName = guild.GetRankName(guildMemberRank);
                int rankHue = guild.GetRankHue(guildMemberRank);

                AddLabel(209, 137, 149, "Guild Rank");
                AddLabel(217, 158, rankHue, guildRankName);

                bool canIncreaseRank = true;
                bool canDecreaseRank = true;

                if (newIndexRank <= 0)
                    canDecreaseRank = false;

                if (newIndexRank >= possibleRanks - 2)
                    canIncreaseRank = false;

                if ((int)guildMemberEntry.m_Rank < newIndexRank)
                    canIncreaseRank = false;

                if ((int)guildMemberEntry.m_Rank <= (int)guildMemberTargetEntry.m_Rank)
                {
                    canIncreaseRank = false;
                    canDecreaseRank = false;
                }

                if (!guild.CanPromoteMembers(guildMemberEntry.m_Rank))
                {
                    canIncreaseRank = false;
                    canDecreaseRank = false;
                }

                //TEST: Add Support for Lowering Own Rank (Add Need Guildmaster Check if Guildmaster Does it)
                if (m_Player == playerTarget)
                {
                    canIncreaseRank = false;
                    canDecreaseRank = false;
                }

                if (canIncreaseRank || canDecreaseRank)
                    AddLabel(204, 216, 0, "Change Rank");

                if (canIncreaseRank)
                    AddButton(230, 191, 9900, 2151, 4, GumpButtonType.Reply, 0);

                if (canDecreaseRank)
                    AddButton(231, 242, 9906, 2151, 5, GumpButtonType.Reply, 0);

                if (newIndexRank != (int)guildMemberTargetEntry.m_Rank)
                {
                    AddLabel(187, 280, 149, "Apply Rank Change");
                    AddButton(189, 306, 2141, 2151, 6, GumpButtonType.Reply, 0);
                    AddButton(245, 306, 2071, 2151, 7, GumpButtonType.Reply, 0);
                }
                
                AddLabel(450, 200, WhiteTextHue, "Str");
                AddLabel(421, 200, 2603, playerTarget.RawStr.ToString());

                AddLabel(450, 220, WhiteTextHue, "Dex");
                AddLabel(421, 220, 2603, playerTarget.RawDex.ToString());

                AddLabel(450, 240, WhiteTextHue, "Int");
                AddLabel(421, 240, 2603, playerTarget.RawInt.ToString());

                List<KeyValuePair<SkillName, double>> m_BestSkills = new List<KeyValuePair<SkillName, double>>();

                int skillCount = Enum.GetNames(typeof(SkillName)).Length;

                for (int a = 0; a < skillCount; a++)
                {
                    SkillName skillName = (SkillName)a;
                    double skillValue = playerTarget.Skills[skillName].Base;

                    KeyValuePair<SkillName, double> skillset = new KeyValuePair<SkillName, double>(skillName, skillValue);

                    int newIndexPosition = -1;

                    for (int b = 0; b < m_BestSkills.Count; b++)
                    {
                        if (skillValue >= m_BestSkills[b].Value)
                            newIndexPosition = b + 1;
                    }

                    if (newIndexPosition == -1)
                        m_BestSkills.Insert(0, skillset);

                    else
                    {
                        if (newIndexPosition >= m_BestSkills.Count)
                            m_BestSkills.Add(skillset);

                        else
                            m_BestSkills.Insert(newIndexPosition, skillset);
                    }
                }

                m_BestSkills.Reverse(0, m_BestSkills.Count);

                int skillStartY = 152;
                int rowSpacing = 20;

                int displayCount = 8;

                for (int a = 0; a < displayCount; a++)
                {
                    KeyValuePair<SkillName, double> skillset = m_BestSkills[a];

                    if (skillset.Value == 0)
                        continue;

                    string skillName = SkillCheck.GetSkillName(skillset.Key);

                    string skillValue = Utility.CreateDecimalString(skillset.Value, 1);

                    if (skillValue.IndexOf(".") <= 0)
                        skillValue = skillValue + ".0";

                    AddLabel(496, skillStartY, 2599, skillValue);
                    AddLabel(533, skillStartY, WhiteTextHue, skillName);

                    skillStartY += rowSpacing;
                }

                AddButton(527, 429, 4029, 4031, 8, GumpButtonType.Reply, 0);
                AddLabel(560, 429, 2550, "Send Message");

                AddButton(154, 429, 4014, 4016, 9, GumpButtonType.Reply, 0);
                AddLabel(188, 429, WhiteTextHue, "Return");
            }

            #endregion

            #region Candidates

            else if (m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.Candidates)
            {
                #region Images

                AddLabel(365, 15, 2563, "Candidates");
                AddLabel(267, 70, 149, "Player Name");               
                AddLabel(373, 429, 63, "Invite Player to Guild");
                AddLabel(548, 70, 149, "Approve");
                AddLabel(605, 70, 149, "Decline");
                AddLabel(466, 70, 149, "Player Info");
                AddLabel(174, 70, 149, "Accepted");
                AddLabel(387, 70, 149, "Expires In");
                AddImage(299, 39, 2446, 2401);
                AddLabel(508, 39, 149, "Search for Player");
                AddLabel(360, 400, 149, "Page");
                AddLabel(513, 400, 2550, "Total Candidates");
                AddLabel(157, 400, 2599, "Total Accepted");

                #endregion

                //-----

                Guild guild = m_GuildGumpObject.m_Guild;

                if (guild == null)
                    return;

                GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(m_Player);

                if (guildMemberEntry == null) 
                    return;

                if (m_GuildGumpObject.m_CandidateSortCriteria == Guilds.CandidateSortCriteria.None)
                {
                    m_GuildGumpObject.m_CandidateSortCriteria = Guilds.CandidateSortCriteria.PlayerName;
                    m_GuildGumpObject.m_CandidateSortAscending = true;
                }

                guild.AuditCandidates();

                m_GuildGumpObject.m_CandidatesSorted = guild.GetCandidates(m_GuildGumpObject.m_CandidateSortCriteria, m_GuildGumpObject.m_CandidateSortAscending);

                int candidatesPerPage = 12;
                int totalCandidates = m_GuildGumpObject.m_CandidatesSorted.Count;
                int totalCandidatesPages = (int)(Math.Ceiling((double)totalCandidates / (double)candidatesPerPage));

                if (m_GuildGumpObject.m_CandidatePage >= totalCandidatesPages)
                    m_GuildGumpObject.m_CandidatePage = totalCandidatesPages - 1;

                if (m_GuildGumpObject.m_CandidatePage < 0)
                    m_GuildGumpObject.m_CandidatePage = 0;

                int candidateStartIndex = m_GuildGumpObject.m_CandidatePage * candidatesPerPage;
                int candidateEndIndex = (m_GuildGumpObject.m_CandidatePage * candidatesPerPage) + (candidatesPerPage - 1);

                if (candidateEndIndex >= totalCandidates)
                    candidateEndIndex = totalCandidates - 1;

                int candidatesDisplayCount = candidateEndIndex - candidateStartIndex;

                int rowSpacing = 30;

                startY = 90;

                //-----

                AddButton(271, 40, 9909, 9909, 4, GumpButtonType.Reply, 0); //Search Left
                AddTextEntry(308, 40, 158, 20, WhiteTextHue, 12, "Player Name", Guilds.GuildNameCharacterLimit);
                AddButton(483, 39, 9903, 9903, 5, GumpButtonType.Reply, 0); //Search Right

                switch (guildGumpObject.m_CandidateSortCriteria)
                {
                    case Guilds.CandidateSortCriteria.Accepted:
                        if (guildGumpObject.m_CandidateSortAscending)
                            AddButton(154, 73, 5600, 5600, 6, GumpButtonType.Reply, 0); //Sort Accepted

                        else
                            AddButton(154, 73, 5602, 5602, 6, GumpButtonType.Reply, 0); //Sort Accepted

                        AddButton(246, 73, 2117, 2118, 7, GumpButtonType.Reply, 0); //Sort Player Name
                        AddButton(367, 73, 2117, 2118, 8, GumpButtonType.Reply, 0); //Sort Expiration
                    break;

                    case Guilds.CandidateSortCriteria.PlayerName:
                        AddButton(154, 73, 2117, 2118, 6, GumpButtonType.Reply, 0); //Sort Accepted

                        if (guildGumpObject.m_CandidateSortAscending)
                            AddButton(246, 73, 5600, 5600, 7, GumpButtonType.Reply, 0); //Sort Player Name

                        else
                            AddButton(246, 73, 5602, 5602, 7, GumpButtonType.Reply, 0); //Sort Player Name

                        AddButton(367, 73, 2117, 2118, 8, GumpButtonType.Reply, 0); //Sort Expiration
                    break;

                    case Guilds.CandidateSortCriteria.Expiration:
                        AddButton(154, 73, 2117, 2118, 6, GumpButtonType.Reply, 0); //Sort Accepted
                        AddButton(246, 73, 2117, 2118, 7, GumpButtonType.Reply, 0); //Sort Player Name

                        if (guildGumpObject.m_CandidateSortAscending)
                            AddButton(367, 73, 5600, 5600, 8, GumpButtonType.Reply, 0); //Sort Expiration

                        else
                            AddButton(367, 73, 5602, 5602, 8, GumpButtonType.Reply, 0); //Sort Expiration
                    break;
                }                

                for (int a = 0; a < candidatesDisplayCount + 1; a++)
                {
                    int candidateIndex = candidateStartIndex + a;

                    if (candidateStartIndex >= totalCandidates)
                        continue;
                    
                    GuildInvitation invitationItem = m_GuildGumpObject.m_CandidatesSorted[candidateIndex];                    

                    if (invitationItem != null)
                    {
                        int buttonIndex = m_GuildGumpObject.m_CandidateButtonIndexOffset + (a * 10);

                        DateTime expirationDate = invitationItem.m_InvitationTime + Guilds.InvitationExpiration;

                        string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, expirationDate, true, true, true, true, false);

                        if (invitationItem.m_Accepted)
                            AddImage(185, startY, 9724, 0); //Accepted
                        else
                            AddImage(185, startY, 9721, 0); //Accepted

                        AddLabel(Utility.CenteredTextOffset(310, invitationItem.m_PlayerTarget.RawName), startY + 5, WhiteTextHue, invitationItem.m_PlayerTarget.RawName);
                        AddLabel(Utility.CenteredTextOffset(415, timeRemaining), startY + 5, WhiteTextHue, timeRemaining);
                        AddButton(485, startY + 5, 4011, 4013, buttonIndex + 0, GumpButtonType.Reply, 0); //Player Info
                        AddButton(559, startY, 2151, 2154, buttonIndex + 1, GumpButtonType.Reply, 0); //Approve
                        AddButton(614, startY, 2472, 2473, buttonIndex + 2, GumpButtonType.Reply, 0); //Decline
                    }

                    startY += rowSpacing;
                }

                int totalAccepted = 0;

                for (int a = 0; a < m_GuildGumpObject.m_CandidatesSorted.Count; a++)
                {
                    if (m_GuildGumpObject.m_CandidatesSorted[a] == null) continue;
                    if (m_GuildGumpObject.m_CandidatesSorted[a].m_Accepted)
                        totalAccepted++;
                }

                //Total Accepted
                AddLabel(262, 400, WhiteTextHue, totalAccepted.ToString());

                //Total Candidates
                if (totalCandidates > 0)
                    AddLabel(397, 400, WhiteTextHue, (guildGumpObject.m_CandidatePage + 1).ToString() + "/" + totalCandidatesPages.ToString()); //Page

                AddLabel(625, 400, WhiteTextHue, totalCandidates.ToString());

                //Previous Page
                if (guildGumpObject.m_CandidatePage > 0)
                {
                    AddLabel(188, 429, WhiteTextHue, "Previous Page");
                    AddButton(154, 429, 4014, 4016, 9, GumpButtonType.Reply, 0); 
                }

                //Next Page
                if (guildGumpObject.m_CandidatePage < totalCandidatesPages - 1)
                {
                    AddLabel(552, 429, WhiteTextHue, "Next Page");
                    AddButton(620, 429, 4005, 4007, 10, GumpButtonType.Reply, 0); 
                }

                AddButton(337, 429, 4002, 4004, 11, GumpButtonType.Reply, 0); //Invite Player 
            }

            #endregion

            #region Candidate Preview

            else if (m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.CandidatePreview)
            {
                Guild guild = m_GuildGumpObject.m_Guild;

                 if (guild == null) 
                     return;

                GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(m_Player);

                if (guildMemberEntry == null) 
                    return;

                GuildInvitation candidate = m_GuildGumpObject.m_CandidatePreviewCandidate;
                
                if (candidate == null) return;
                if (candidate.m_PlayerTarget == null) return;
                if (candidate.m_PlayerTarget.Deleted) return;    
            
                PlayerMobile playerTarget = candidate.m_PlayerTarget;

                bool canEdit = guild.CanApproveCandidates(guildMemberEntry.m_Rank);

                AddLabel(Utility.CenteredTextOffset(395, playerTarget.RawName), 23, 63, playerTarget.RawName);

                string timeText = Utility.CreateTimeRemainingString(playerTarget.CreatedOn, DateTime.UtcNow, false, true, true, false, false) + " old";
                AddLabel(Utility.CenteredTextOffset(395, timeText), 44, 2550, timeText);

                if (playerTarget.Female)
                    AddItem(369, 74, 8455, 2500);
                else
                    AddItem(369, 74, 8454, 2500);                
               
                AddLabel(202, 190, 149, "Approve Candidate");
                AddButton(166, 186, 2151, 2154, 4, GumpButtonType.Reply, 0);

                AddLabel(202, 249, 149, "Decline Candidate");
                AddButton(168, 247, 2473, 2473, 5, GumpButtonType.Reply, 0);                

                AddLabel(450, 200, WhiteTextHue, "Str");
                AddLabel(421, 200, 2603, playerTarget.RawStr.ToString());

                AddLabel(450, 220, WhiteTextHue, "Dex");
                AddLabel(421, 220, 2603, playerTarget.RawDex.ToString());

                AddLabel(450, 240, WhiteTextHue, "Int");
                AddLabel(421, 240, 2603, playerTarget.RawInt.ToString());

                List<KeyValuePair<SkillName, double>> m_BestSkills = new List<KeyValuePair<SkillName, double>>();

                int skillCount = Enum.GetNames(typeof(SkillName)).Length;

                for (int a = 0; a < skillCount; a++)
                {
                    SkillName skillName = (SkillName)a;
                    double skillValue = playerTarget.Skills[skillName].Base;

                    KeyValuePair<SkillName, double> skillset = new KeyValuePair<SkillName, double>(skillName, skillValue);

                    int newIndexPosition = -1;

                    for (int b = 0; b < m_BestSkills.Count; b++)
                    {
                        if (skillValue >= m_BestSkills[b].Value)
                            newIndexPosition = b + 1;
                    }

                    if (newIndexPosition == -1)
                        m_BestSkills.Insert(0, skillset);

                    else
                    {
                        if (newIndexPosition >= m_BestSkills.Count)
                            m_BestSkills.Add(skillset);

                        else
                            m_BestSkills.Insert(newIndexPosition, skillset);
                    }
                }

                m_BestSkills.Reverse(0, m_BestSkills.Count);

                int skillStartY = 152;
                int rowSpacing = 20;

                int displayCount = 8;

                for (int a = 0; a < displayCount; a++)
                {
                    KeyValuePair<SkillName, double> skillset = m_BestSkills[a];

                    if (skillset.Value == 0)
                        continue;

                    string skillName = SkillCheck.GetSkillName(skillset.Key);

                    string skillValue = Utility.CreateDecimalString(skillset.Value, 1);

                    if (skillValue.IndexOf(".") <= 0)
                        skillValue = skillValue + ".0";

                    AddLabel(496, skillStartY, 2599, skillValue);
                    AddLabel(533, skillStartY, WhiteTextHue, skillName);

                    skillStartY += rowSpacing;
                }
                
                AddLabel(318, 379, 149, "Player Has Accepted");
                AddLabel(348, 399, 149, "Invitation");

                if (candidate.m_Accepted)
                    AddImage(364, 423, 9724, 0);
                else
                    AddImage(364, 423, 9721, 0);                

                AddButton(527, 429, 4029, 4031, 6, GumpButtonType.Reply, 0);
                AddLabel(560, 429, 2550, "Send Message");

                AddButton(154, 429, 4014, 4016, 7, GumpButtonType.Reply, 0);
                AddLabel(188, 429, WhiteTextHue, "Return");
            }

            #endregion

            #region Diplomacy

            else if (m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.Diplomacy)
            {
                #region Images

                AddLabel(365, 15, 2606, "Diplomacy");
                AddLabel(260, 70, 149, "Guild Name");

                AddLabel(609, 70, 149, "Manage");
                AddImage(299, 39, 2446, 2401);
                AddLabel(508, 39, 149, "Search for Guild Name");

                AddLabel(447, 70, 149, "Relationship");
                AddLabel(555, 70, 149, "Players");

                #endregion

                //-----

                Guild guild = m_GuildGumpObject.m_Guild;

                if (guild == null)
                    return;

                GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(m_Player);

                if (guildMemberEntry == null)
                    return;

                if (m_GuildGumpObject.m_RelationshipSortCriteria == Guilds.RelationshipSortCriteria.None)
                {
                    m_GuildGumpObject.m_RelationshipSortCriteria = Guilds.RelationshipSortCriteria.GuildName;
                    m_GuildGumpObject.m_RelationshipSortAscending = true;
                }

                guild.AuditRelationships();

                m_GuildGumpObject.m_RelationshipsSorted = guild.GetGuildRelationships(m_GuildGumpObject.m_RelationshipFilterType, m_GuildGumpObject.m_RelationshipSortCriteria, m_GuildGumpObject.m_RelationshipSortAscending);

                int relationshipsPerPage = 12;
                int totalRelationships = m_GuildGumpObject.m_RelationshipsSorted.Count;
                int totalRelationshipPages = (int)(Math.Ceiling((double)totalRelationships / (double)relationshipsPerPage));

                if (m_GuildGumpObject.m_RelationshipPage >= totalRelationshipPages)
                    m_GuildGumpObject.m_RelationshipPage = totalRelationshipPages - 1;

                if (m_GuildGumpObject.m_RelationshipPage < 0)
                    m_GuildGumpObject.m_RelationshipPage = 0;

                int relationshipStartIndex = m_GuildGumpObject.m_RelationshipPage * relationshipsPerPage;
                int relationshipEndIndex = (m_GuildGumpObject.m_CandidatePage * relationshipsPerPage) + (relationshipsPerPage - 1);

                if (relationshipEndIndex >= totalRelationships)
                    relationshipEndIndex = totalRelationships - 1;

                int relationshipDisplayCount = relationshipEndIndex - relationshipStartIndex;

                int rowSpacing = 30;

                startY = 95;

                switch (guildGumpObject.m_RelationshipSortCriteria)
                {
                    case Guilds.RelationshipSortCriteria.GuildName:
                        if (guildGumpObject.m_RelationshipSortAscending)
                            AddButton(238, 73, 5600, 5600, 6, GumpButtonType.Reply, 0); //Sort Guild Name

                        else
                            AddButton(238, 73, 5602, 5602, 6, GumpButtonType.Reply, 0); //Sort Guild Name

                        AddButton(425, 73, 2117, 2118, 7, GumpButtonType.Reply, 0); //Sort Relationship
                        AddButton(536, 73, 2117, 2118, 8, GumpButtonType.Reply, 0); //Sort Players
                        break;

                    case Guilds.RelationshipSortCriteria.Relationship:
                        AddButton(238, 73, 2117, 2118, 6, GumpButtonType.Reply, 0); //Sort Guild Name

                        if (guildGumpObject.m_RelationshipSortAscending)
                            AddButton(425, 73, 5600, 5600, 7, GumpButtonType.Reply, 0); //Sort Relationship

                        else
                            AddButton(425, 73, 5602, 5602, 7, GumpButtonType.Reply, 0); //Sort Relationship

                        AddButton(536, 73, 2117, 2118, 8, GumpButtonType.Reply, 0); //Sort Players
                        break;

                    case Guilds.RelationshipSortCriteria.PlayerCount:
                        AddButton(238, 73, 2117, 2118, 6, GumpButtonType.Reply, 0); //Sort Guild Name
                        AddButton(425, 73, 2117, 2118, 7, GumpButtonType.Reply, 0); //Sort Relationship

                        if (guildGumpObject.m_RelationshipSortAscending)
                            AddButton(536, 73, 5600, 5600, 8, GumpButtonType.Reply, 0); //Sort Rank

                        else
                            AddButton(536, 73, 5602, 5602, 8, GumpButtonType.Reply, 0); //Sort Rank
                        break;
                }

                AddButton(271, 40, 9909, 9909, 4, GumpButtonType.Reply, 0); //Search Left
                AddTextEntry(308, 40, 158, 20, WhiteTextHue, 14, "Guild Name", Guilds.GuildNameCharacterLimit);
                AddButton(483, 39, 9903, 9903, 5, GumpButtonType.Reply, 0); //Search Right

                for (int a = 0; a < relationshipDisplayCount + 1; a++)
                {
                    int relationshipIndex = relationshipStartIndex + a;

                    if (relationshipStartIndex >= totalRelationships)
                        continue;

                    DiplomacyEntry diplomacyEntry = m_GuildGumpObject.m_RelationshipsSorted[relationshipIndex];

                    if (diplomacyEntry.m_Guild == null)
                        continue;

                    Guilds.GuildRelationshipType relationshipType = Guilds.GuildRelationshipType.Neutral;

                    bool isFromGuild = true;

                    if (diplomacyEntry.m_GuildRelationship != null)
                    {
                        relationshipType = diplomacyEntry.m_GuildRelationship.m_RelationshipType;

                        if (diplomacyEntry.m_GuildRelationship.m_GuildTarget == guild)
                            isFromGuild = false;
                    }

                    int buttonIndex = m_GuildGumpObject.m_RelationshipButtonIndexOffset + (a * 10);

                    string relationshipText = GuildRelationship.GetDisplayName(relationshipType, isFromGuild);
                    int relationshipHue = GuildRelationship.GetHue(relationshipType, isFromGuild);

                    AddLabel(Utility.CenteredTextOffset(300, diplomacyEntry.m_Guild.GetDisplayName(true)), startY, WhiteTextHue, diplomacyEntry.m_Guild.GetDisplayName(true));
                    AddLabel(Utility.CenteredTextOffset(475, relationshipText), startY, relationshipHue, relationshipText);
                    AddLabel(Utility.CenteredTextOffset(570, diplomacyEntry.m_Guild.m_ActivePlayers.ToString()), startY, 2599, diplomacyEntry.m_Guild.m_ActivePlayers.ToString());
                    AddButton(617, startY, 4011, 4013, buttonIndex, GumpButtonType.Reply, 0); //Manage Relationship

                    startY += rowSpacing;
                }

                string relationshipFilterText = "";

                int warCount = 0;
                int allyCount = 0;

                switch (guildGumpObject.m_RelationshipFilterType)
                {
                    case Guilds.RelationshipFilterType.ShowActiveAgreements:
                        relationshipFilterText = "Active Agreements";

                        foreach (DiplomacyEntry diplomacyEntry in m_GuildGumpObject.m_RelationshipsSorted)
                        {
                            if (diplomacyEntry.m_GuildRelationship != null)
                            {
                                if (diplomacyEntry.m_GuildRelationship.m_RelationshipType == Guilds.GuildRelationshipType.War)
                                    warCount++;

                                if (diplomacyEntry.m_GuildRelationship.m_RelationshipType == Guilds.GuildRelationshipType.Alliance)
                                    allyCount++;
                            }
                        }

                        AddLabel(155, 400, 1256, "Active Wars");
                        AddLabel(245, 400, WhiteTextHue, warCount.ToString());

                        AddLabel(538, 400, 2599, "Active Allies");
                        AddLabel(625, 400, WhiteTextHue, allyCount.ToString());
                    break;

                    case Guilds.RelationshipFilterType.ShowAgreementsReceived:
                        relationshipFilterText = "Offers Received";

                        foreach (DiplomacyEntry diplomacyEntry in m_GuildGumpObject.m_RelationshipsSorted)
                        {
                            if (diplomacyEntry.m_GuildRelationship != null)
                            {
                                if (diplomacyEntry.m_GuildRelationship.m_GuildTarget == guild)
                                {
                                    if (diplomacyEntry.m_GuildRelationship.m_RelationshipType == Guilds.GuildRelationshipType.WarRequest)
                                        warCount++;

                                    if (diplomacyEntry.m_GuildRelationship.m_RelationshipType == Guilds.GuildRelationshipType.AllianceRequest)
                                        allyCount++;
                                }
                            }
                        }

                        AddLabel(155, 400, 1256, "War Offers Received");
                        AddLabel(296, 400, WhiteTextHue, warCount.ToString());

                        AddLabel(467, 400, 2599, "Alliance Offers Received");
                        AddLabel(625, 400, WhiteTextHue, allyCount.ToString());
                        break;

                    case Guilds.RelationshipFilterType.ShowAgreementsSent:
                        relationshipFilterText = "Offers Sent";

                        foreach (DiplomacyEntry diplomacyEntry in m_GuildGumpObject.m_RelationshipsSorted)
                        {
                            if (diplomacyEntry.m_GuildRelationship.m_GuildFrom == guild)
                            {
                                if (diplomacyEntry.m_GuildRelationship.m_RelationshipType == Guilds.GuildRelationshipType.WarRequest)
                                    warCount++;

                                if (diplomacyEntry.m_GuildRelationship.m_RelationshipType == Guilds.GuildRelationshipType.AllianceRequest)
                                    allyCount++;
                            }
                        }

                        AddLabel(155, 400, 1256, "War Offers Sent");
                        AddLabel(271, 400, WhiteTextHue, warCount.ToString());

                        AddLabel(490, 400, 2599, "Alliance Offers Sent");
                        AddLabel(625, 400, WhiteTextHue, allyCount.ToString());
                    break;

                    case Guilds.RelationshipFilterType.ShowAvailableGuilds:
                        relationshipFilterText = "Neutral Guilds";

                        foreach (DiplomacyEntry diplomacyEntry in m_GuildGumpObject.m_RelationshipsSorted)
                        {
                            warCount++;
                        }

                        AddLabel(155, 400, YellowTextHue, "Total Guilds");
                        AddLabel(240, 400, WhiteTextHue, warCount.ToString());
                    break;
                }

                AddButton(302, 379, 2223, 2223, 9, GumpButtonType.Reply, 0); //Previous Relationship Filter
                AddLabel(Utility.CenteredTextOffset(405, relationshipFilterText), 375, 2603, relationshipFilterText);
                AddButton(466, 379, 2224, 2224, 10, GumpButtonType.Reply, 0); //Next Relationship Filter

                if (guildGumpObject.m_RelationshipsSorted.Count > 0)
                {
                    AddLabel(360, 400, 149, "Page");
                    AddLabel(397, 400, WhiteTextHue, (guildGumpObject.m_RelationshipPage + 1).ToString() + "/" + totalRelationshipPages.ToString()); //Page 
                }

                if (guildGumpObject.m_RelationshipPage > 0)
                {
                    AddLabel(188, 429, WhiteTextHue, "Previous Page");
                    AddButton(154, 429, 4014, 4016, 11, GumpButtonType.Reply, 0); //Previous Page
                }

                if (guildGumpObject.m_RelationshipPage < totalRelationshipPages - 1)
                {
                    AddLabel(552, 429, WhiteTextHue, "Next Page");
                    AddButton(620, 429, 4005, 4007, 12, GumpButtonType.Reply, 0); //Next Page
                }
            }

            #endregion

            #region Diplomacy Manage Relationship

            else if (m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.DiplomacyManagement)
            {
                Guild guild = m_GuildGumpObject.m_Guild;
                GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(m_Player);
                DiplomacyEntry diplomacyEntry = m_GuildGumpObject.m_DiplomacyManagementDiplomacyEntry;

                if (guild == null) return;
                if (guildMemberEntry == null) return;
                if (diplomacyEntry == null) return;
                if (diplomacyEntry.m_Guild == null) return;

                Guild guildTarget = diplomacyEntry.m_Guild;

                if (guildTarget == null) return;
                if (guildTarget.Deleted) return;

                GuildRelationship guildRelationship = guild.GetGuildRelationship(guildTarget);

                #region Images

                AddLabel(197, 159, 149, "Guildmaster");
                AddLabel(197, 61, 149, "Guild Abbreviation");
                AddLabel(197, 25, 149, "Guild Name");
                AddImage(271, 23, 1141, 2400);
                AddImage(320, 60, 2444, 2401);
                AddItem(151, 22, 3796);
                AddImage(278, 202, 2446, 2401);
                AddImage(278, 160, 2446, 2401);
                AddLabel(197, 109, 149, "Guild Symbol");
                AddItem(144, 98, 7776);
                AddLabel(197, 203, 149, "Guildhouse");               
                AddLabel(515, 109, 149, "Faction");
                AddBackground(158, 335, 493, 23, 5100);
                AddLabel(299, 245, 2501, "Players");
                AddLabel(382, 245, 2505, "Characters");
                AddLabel(493, 245, 1256, "Wars");
                AddLabel(553, 245, 2599, "Alliances");
                AddLabel(197, 245, 149, "Guild Age");
                AddItem(164, 238, 3103);
                AddItem(482, 80, 5402);
                AddImage(311, 90, 2328);
                AddItem(296, 269, 8454, 2500);
                AddItem(383, 269, 8454, 2515);
                AddItem(404, 269, 8455, 2515);
                AddItem(493, 280, 3914);
                AddItem(479, 281, 5049);
                AddItem(558, 283, 4030);
                AddItem(558, 275, 4031);
                AddItem(558, 275, 4031);
                AddItem(156, 191, 8900);

                #endregion

                //-----

                AddLabel(Utility.CenteredTextOffset(405, guildTarget.Name), 25, WhiteTextHue, guildTarget.Name);
                AddLabel(Utility.CenteredTextOffset(355, guildTarget.m_Abbreviation), 61, WhiteTextHue, guildTarget.m_Abbreviation);

                GuildSymbolDetail guildSymbolDetail = GuildSymbols.GetGuildSymbolDetail(guildTarget.m_GuildSymbol);

                AddItem(311 + guildSymbolDetail.SymbolIconOffsetX, 90 + guildSymbolDetail.SymbolIconOffsetY, guildSymbolDetail.SymbolIconItemId, guildSymbolDetail.SymbolIconHue);

                //Faction
                AddItem(600, 91, 11009, 2603);
                AddItem(555, 58, 17099, 2603);
                AddLabel(586, 144, 2603, "Order");

                string guildmaster = "";

                if (guildTarget.m_Guildmaster != null)
                    guildmaster = guildTarget.m_Guildmaster.RawName;

                AddLabel(322, 161, WhiteTextHue, guildmaster);

                if (guildTarget.m_Guildhouse != null)
                {
                    string locationText = "(located at " + guildTarget.m_Guildhouse.Location.X.ToString() + "," + guildTarget.m_Guildhouse.Location.Y.ToString() + ")";

                    AddLabel(Utility.CenteredTextOffset(365, locationText), 203, WhiteTextHue, locationText);
                    AddButton(464, 206, 2117, 248, 4, GumpButtonType.Reply, 0);
                    AddLabel(484, 203, 2550, "Show Location");
                }

                AddLabel(196, 270, 0, guildTarget.GetGuildAge());

                AddLabel(332, 284, WhiteTextHue, guildTarget.m_ActivePlayers.ToString());
                AddLabel(443, 285, WhiteTextHue, guildTarget.m_ActiveCharacters.ToString());
                AddLabel(529, 285, WhiteTextHue, guildTarget.GetWarCount(true).ToString());
                AddLabel(599, 285, WhiteTextHue, guildTarget.GetAllyCount(true).ToString());

                if (guildTarget.m_Website != null)
                {
                    AddLabel(197, 312, 149, "Website");
                    AddButton(165, 315, 30008, 248, 5, GumpButtonType.Reply, 0);
                    AddLabel(167, 336, WhiteTextHue, guildTarget.m_Website);
                }

                //-----

                AddLabel(341, 365, 2606, "Diplomacy Status");

                if (guildRelationship != null)
                {
                    switch (guildRelationship.m_RelationshipType)
                    {
                        case Guilds.GuildRelationshipType.War:
                            AddLabel(Utility.CenteredTextOffset(395, "At War"), 390, 1256, "At War");

                            AddItem(365, 417, 5049);
                            AddItem(379, 416, 3914);

                            AddLabel(529, 413, 2115, "End Agreement");
                            AddButton(498, 410, 2472, 2473, 9, GumpButtonType.Reply, 0);
                        break;

                        case Guilds.GuildRelationshipType.Alliance:
                            AddLabel(Utility.CenteredTextOffset(395, "Allied"), 390, 2599, "Allied");

                            AddItem(374, 422, 4030);
                            AddItem(374, 414, 4031);

                            AddLabel(529, 413, 2115, "End Agreement");
                            AddButton(498, 410, 2472, 2473, 9, GumpButtonType.Reply, 0);
                            break;

                        case Guilds.GuildRelationshipType.WarRequest:

                            AddItem(365, 417, 5049);
                            AddItem(379, 416, 3914);

                            if (guild == guildRelationship.m_GuildFrom)
                            {
                                AddLabel(Utility.CenteredTextOffset(395, "War Request Sent"), 390, 1256, "War Request Sent");

                                AddLabel(529, 413, 2115, "Retract Offer");
                                AddButton(498, 410, 2472, 2473, 9, GumpButtonType.Reply, 0);
                            }

                            else
                            {
                                AddLabel(Utility.CenteredTextOffset(395, "War Offer Available"), 390, 1256, "War Offer Available");

                                AddLabel(529, 376, 63, "Accept Offer");
                                AddButton(496, 373, 2151, 2154, 8, GumpButtonType.Reply, 0);

                                AddLabel(529, 413, 2115, "Decline Offer");
                                AddButton(498, 410, 2472, 2473, 9, GumpButtonType.Reply, 0);
                            }
                        break;

                        case Guilds.GuildRelationshipType.AllianceRequest:
                            AddItem(374, 422, 4030);
                            AddItem(374, 414, 4031);

                            if (guild == guildRelationship.m_GuildFrom)
                            {
                                AddLabel(Utility.CenteredTextOffset(395, "Alliance Request Sent"), 390, 2599, "Alliance Request Sent");

                                AddLabel(529, 413, 2115, "Retract Offer");
                                AddButton(498, 410, 2472, 2473, 9, GumpButtonType.Reply, 0);
                            }

                            else
                            {
                                AddLabel(Utility.CenteredTextOffset(395, "Alliance Offer Available"), 390, 2599, "Alliance Offer Available");

                                AddLabel(529, 376, 63, "Accept Offer");
                                AddButton(496, 373, 2151, 2154, 8, GumpButtonType.Reply, 0);

                                AddLabel(529, 413, 2115, "Decline Offer");
                                AddButton(498, 410, 2472, 2473, 9, GumpButtonType.Reply, 0);
                            }
                            break;
                    }
                }

                else
                {
                    switch (m_GuildGumpObject.m_DiplomacyManagementNewRelationshipType)
                    {
                        case Guilds.GuildRelationshipType.Neutral:
                            AddLabel(Utility.CenteredTextOffset(395, "Neutral"), 390, 2655, "Neutral");

                            AddItem(372, 412, 4644);
                            break;

                        case Guilds.GuildRelationshipType.WarRequest:
                            AddLabel(Utility.CenteredTextOffset(395, "Send War Request"), 390, 1256, "Send War Request");

                            AddItem(365, 417, 5049);
                            AddItem(379, 416, 3914);

                            AddLabel(529, 376, 63, "Send Offer");
                            AddButton(496, 373, 2151, 2154, 8, GumpButtonType.Reply, 0);
                        break;

                        case Guilds.GuildRelationshipType.AllianceRequest:
                            AddLabel(Utility.CenteredTextOffset(395, "Send Alliance Request"), 390, 2599, "Send Alliance Request");

                            AddItem(374, 422, 4030);
                            AddItem(374, 414, 4031);

                            AddLabel(529, 376, 63, "Send Offer");
                            AddButton(496, 373, 2151, 2154, 8, GumpButtonType.Reply, 0);
                        break;
                    }

                    AddButton(343, 420, 9909, 9909, 6, GumpButtonType.Reply, 0);
                    AddButton(422, 420, 9903, 9903, 7, GumpButtonType.Reply, 0);
                }

                //AddButton(467, 427, 4029, 4031, 10, GumpButtonType.Reply, 0);
                //AddLabel(439, 403, 2550, "Send Message");

                AddButton(154, 429, 4014, 4016, 11, GumpButtonType.Reply, 0);
                AddLabel(188, 429, WhiteTextHue, "Return");
            }

            #endregion

            #region Faction

            else if (m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.Faction)
            {
            }

            #endregion

            #region Messages

            else if (m_Player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.Messages)
            {
            }

            #endregion
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            #region General Handling

            if (m_Player == null)
                return;

            Guilds.CheckCreateGuildGuildSettings(m_Player);
            
            bool closeGump = true;

            #endregion

            //-----

            #region Create Guild

            if (m_GuildGumpObject.m_GumpPageType == GuildGumpPageType.CreateGuild)
            {
                if (m_Player.Guild != null) return;
                if (m_GuildGumpObject.m_Guild != null) return;

                TextRelay guildNameTextRelay = info.GetTextEntry(7);
                TextRelay guildAbbreviationTextRelay = info.GetTextEntry(8);

                string guildNameText = "";

                if (guildNameTextRelay != null)
                    guildNameText = guildNameTextRelay.Text;

                guildNameText.Trim();

                string guildAbbreviationText = "";

                if (guildAbbreviationTextRelay != null)
                    guildAbbreviationText = guildAbbreviationTextRelay.Text;

                guildAbbreviationText.Trim();

                m_GuildGumpObject.m_CreateGuildName = guildNameText;
                m_GuildGumpObject.m_CreateGuildAbbreviation = guildAbbreviationText;

                switch (info.ButtonID)
                {
                    //Previous Symbol
                    case 4:
                        m_Player.SendSound(Guilds.GuildGumpSelectionSound);

                        m_GuildGumpObject.m_CreateGuildSymbolIndex--;
                        closeGump = false;
                    break;

                    //Next Symbol
                    case 5:
                        m_Player.SendSound(Guilds.GuildGumpSelectionSound);

                        m_GuildGumpObject.m_CreateGuildSymbolIndex++;
                        closeGump = false;
                    break;

                    //Create Guild
                    case 6:
                        if (m_Player.Guild != null)
                            m_Player.SendMessage("You are already in a guild.");

                        else if (guildNameText == "Guild Name")
                            m_Player.SendMessage("You must give your guild a name.");

                        else if (guildNameText.Length == 0)                                              
                            m_Player.SendMessage("Guild names must be at least 1 character.");

                        else if (guildAbbreviationText.Length == 0)                        
                            m_Player.SendMessage("Guild abbreviations must be at least 1 character.");                        

                        else if (guildNameText.Length > Guilds.GuildNameCharacterLimit)                       
                            m_Player.SendMessage("Guild names may be no longer than " + Guilds.GuildNameCharacterLimit.ToString() + " characters.");
                        
                        else if (guildAbbreviationText.Length > Guilds.GuildAbbreviationCharacterLimit)
                            m_Player.SendMessage("Guild abbreviations may be no longer than " + Guilds.GuildAbbreviationCharacterLimit.ToString() + " characters.");
                            
                        else if (!Guilds.CheckProfanity(guildNameText))
                            m_Player.SendMessage("That guild name is not allowed.");

                        else if (!Guilds.CheckProfanity(guildAbbreviationText))
                            m_Player.SendMessage("That guild abbreviation is not allowed.");

                        else if (Guilds.GuildNameExists(guildNameText))
                            m_Player.SendMessage("That guild name is already in use.");

                        else if (Guilds.GuildAbbreviationExists(guildAbbreviationText))
                            m_Player.SendMessage("That guild abbreviation is already in use.");

                        else if (Banker.GetBalance(m_Player) < Guilds.GuildRegistrationFee)
                            m_Player.SendMessage("You do not have the gold neccessary in your bank to pay the guild registration fee.");

                        else
                        {
                            Banker.Withdraw(m_Player, Guilds.GuildRegistrationFee);
                            m_Player.SendSound(0x2E6);

                            Guild newGuild = new Guild(m_GuildGumpObject.m_CreateGuildName, m_GuildGumpObject.m_CreateGuildAbbreviation);

                            newGuild.m_GuildSymbol = m_GuildGumpObject.m_CreateGuildSymbol;
                            newGuild.m_Guildmaster = m_Player;

                            newGuild.AddMember(m_Player, GuildMemberRank.Guildmaster);

                            m_Player.SendMessage(Guilds.GuildTextHue,"You are now the founding member of " + newGuild.GetDisplayName(true) + ".");

                            m_Player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.Overview;
                            m_GuildGumpObject.m_GuildTabPage = 0;

                            m_Player.SendSound(Guilds.PromotionSound);

                            Guilds.LaunchGuildGump(m_Player);

                            return; 
                        }

                        closeGump = false;
                    break;
                }
            }

            #endregion

            #region Invitations

            else if (m_GuildGumpObject.m_GumpPageType == GuildGumpPageType.Invitations)
            {
                if (m_Player.Guild != null) return;
                if (m_GuildGumpObject.m_Guild != null) return;

                if (info.ButtonID >= m_GuildGumpObject.m_InvitationButtonIndexOffset)
                {
                    int invitationBaseIndex = info.ButtonID - m_GuildGumpObject.m_InvitationButtonIndexOffset;
                    int invitationIndex = (int)(Math.Floor((double)invitationBaseIndex / 10));
                    int invitationRemainder = invitationBaseIndex % 10;
                    
                    if (invitationIndex < m_GuildGumpObject.m_InvitationsSorted.Count)
                    {
                        GuildInvitation invitation = m_GuildGumpObject.m_InvitationsSorted[invitationIndex];

                        Guild guild = invitation.m_Guild;

                        if (invitation == null)
                            m_Player.SendMessage("That guild invitation no longer exists.");

                        else if (invitation.Deleted)
                            m_Player.SendMessage("That guild invitation no longer exists.");

                        else if (guild == null)
                            m_Player.SendMessage("That guild no longer exists.");

                        else if (guild.Deleted)
                            m_Player.SendMessage("That guild no longer exists.");

                        else
                        {
                            switch (invitationRemainder)
                            {
                                //Accept
                                case 0:
                                    invitation.m_Accepted = !invitation.m_Accepted;

                                    bool immediateApproval = false;

                                    GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(invitation.m_PlayerInviter);

                                    if (guildMemberEntry != null)
                                    {
                                        if (guild.CanApproveCandidates(guildMemberEntry.m_Rank))
                                            immediateApproval = true;
                                    }

                                    if (invitation.m_Accepted)
                                    {
                                        if (immediateApproval)
                                        {
                                            invitation.Delete();

                                            guild.GuildAnnouncement(m_Player.RawName + " has been made a member of " + guild.GetDisplayName(true) + ".", new List<PlayerMobile>(), GuildMemberRank.Recruit);
                                            guild.AddMember(m_Player, GuildMemberRank.Recruit);

                                            m_Player.SendMessage(Guilds.GuildTextHue, "You have been made a member of " + guild.GetDisplayName(true) + ".");

                                            m_Player.SendSound(Guilds.PromotionSound);

                                            Guilds.LaunchGuildGump(m_Player);

                                            return;

                                        }

                                        else
                                            m_Player.SendMessage("You accept the invitation and must now await final approval for membership.");
                                    }
                                break;

                                //Guild Info
                                case 1:
                                    m_GuildGumpObject.m_InvitationGuildInvitation = invitation;
                                    m_Player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.InvitationGuildPreview;

                                    m_Player.SendSound(Guilds.GuildGumpOpenGumpSound);
                                break;

                                //Decline
                                case 2:
                                    invitation.Delete();

                                    m_Player.SendMessage("You decline the guild invitation.");
                                break;
                            }
                        }
                    }

                    closeGump = false;
                }

                switch (info.ButtonID)
                {
                    //Search Previous Player
                    case 4:
                        closeGump = false;
                    break;

                    //Search Next Player
                    case 5:
                        closeGump = false;
                    break;

                    //Sort by Guild Name
                    case 6:
                        if (m_GuildGumpObject.m_InvitationSortCriteria == Guilds.InvitationSortCriteria.GuildName)
                            m_GuildGumpObject.m_InvitationSortAscending = !m_GuildGumpObject.m_InvitationSortAscending;

                        else
                        {
                            m_GuildGumpObject.m_InvitationSortCriteria = Guilds.InvitationSortCriteria.GuildName;
                            m_GuildGumpObject.m_InvitationSortAscending = true;
                        }

                        closeGump = false;
                    break;

                    //Sort by Expiration
                    case 7:
                        if (m_GuildGumpObject.m_InvitationSortCriteria == Guilds.InvitationSortCriteria.Expiration)
                            m_GuildGumpObject.m_InvitationSortAscending = !m_GuildGumpObject.m_InvitationSortAscending;

                        else
                        {
                            m_GuildGumpObject.m_InvitationSortCriteria = Guilds.InvitationSortCriteria.Expiration;
                            m_GuildGumpObject.m_InvitationSortAscending = false;
                        }

                        closeGump = false;
                    break;

                    //Previous Page
                    case 8:
                        closeGump = false;
                    break;

                    //Next Page
                    case 9:
                        closeGump = false;
                    break;

                    //Allow Guild Invitations
                    case 10:
                        m_Player.m_GuildSettings.m_IgnoreGuildInvitations = !m_Player.m_GuildSettings.m_IgnoreGuildInvitations;

                        if (m_Player.m_GuildSettings.m_IgnoreGuildInvitations)
                            m_Player.SendMessage("You will no longer receive guild invitations.");
                        else
                            m_Player.SendMessage("You now will be open to receiving guild invitations.");

                        closeGump = false;
                    break;
                }
            }

            #endregion

            #region Invitation Guild Preview

            else if (m_GuildGumpObject.m_GumpPageType == GuildGumpPageType.InvitationGuildPreview)
            {
                if (m_Player.Guild != null) return;
                if (m_GuildGumpObject.m_Guild != null) return;
                if (m_GuildGumpObject.m_InvitationGuildInvitation == null) return;
                if (m_GuildGumpObject.m_InvitationGuildInvitation.Deleted) return;

                GuildInvitation invitation = m_GuildGumpObject.m_InvitationGuildInvitation;

                Guild guild = invitation.m_Guild;

                if (guild == null)
                    return;

                switch (info.ButtonID)
                {
                    //Show House Location
                    case 4:
                        if (guild.m_Guildhouse != null)
                        {
                        }

                        closeGump = false;
                    break;

                    //Website
                    case 5:
                        if (m_Player.NetState != null)
                            m_Player.NetState.LaunchBrowser(guild.m_Website);

                        closeGump = false;
                    break;

                    //Accept Invitation
                    case 6:
                        invitation.m_Accepted = !invitation.m_Accepted;

                        bool immediateApproval = false;

                        GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(invitation.m_PlayerInviter);

                        if (guildMemberEntry != null)
                        {
                            if (guild.CanApproveCandidates(guildMemberEntry.m_Rank))
                                immediateApproval = true;
                        }

                        if (invitation.m_Accepted)
                        {
                            if (immediateApproval)
                            {
                                invitation.Delete();

                                guild.GuildAnnouncement(m_Player.RawName + " has been made a member of " + guild.GetDisplayName(true) + ".", new List<PlayerMobile>(), GuildMemberRank.Recruit);
                                guild.AddMember(m_Player, GuildMemberRank.Recruit);

                                m_Player.SendMessage(Guilds.GuildTextHue, "You have been made a member of " + guild.GetDisplayName(true) + ".");

                                m_Player.SendSound(Guilds.PromotionSound);

                                Guilds.LaunchGuildGump(m_Player);

                                return;

                            }

                            else
                                m_Player.SendMessage("You accept the invitation and must now await final approval for membership.");
                        }

                        closeGump = false;
                    break;
                        
                    //Decline Invitation
                    case 7:
                        invitation.Delete();

                        m_Player.SendMessage("You decline the guild invitation.");

                        m_Player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.Invitations;
                        m_Player.SendSound(Guilds.GuildGumpCloseGumpSound);

                        closeGump = false;
                    break;

                    //Return
                    case 8:
                        m_Player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.Invitations;
                        m_Player.SendSound(Guilds.GuildGumpCloseGumpSound);

                        closeGump = false;
                    break;
                }
            }

            #endregion

            #region Overview

            else if (m_GuildGumpObject.m_GumpPageType == GuildGumpPageType.Overview)
            {
                Guild guild = m_GuildGumpObject.m_Guild;

                if (guild == null)
                    return;

                GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(m_Player);

                if (guildMemberEntry == null)
                    return;

                switch (info.ButtonID)
                {
                    //Show Guild House
                    case 4:
                        //TEST: FINISH

                        m_GuildGumpObject.m_OverviewResignFromGuildReady = false;

                        closeGump = false;
                    break;

                    //Launch Website
                    case 5:
                        if (m_Player.NetState != null)
                            m_Player.NetState.LaunchBrowser(guild.m_Website);

                        m_GuildGumpObject.m_OverviewResignFromGuildReady = false;

                        closeGump = false;
                    break;

                    //Show Guild Title
                    case 6:
                        m_Player.m_GuildSettings.m_ShowGuildTitle = !m_Player.m_GuildSettings.m_ShowGuildTitle;

                        if (m_Player.m_GuildSettings.m_ShowGuildTitle)
                            m_Player.SendMessage("Your overhead Guild Title will now be displayed.");

                        else
                            m_Player.SendMessage("Your overhead Guild Title will now be hidden.");

                        m_GuildGumpObject.m_OverviewResignFromGuildReady = false;

                        closeGump = false;
                        break;

                    //Resign from Guild
                    case 7:
                        if (m_GuildGumpObject.m_OverviewResignFromGuildReady)
                        {
                            guild.DismissMember(m_Player, false, true);

                            return;
                        }

                        else
                        {
                            m_GuildGumpObject.m_OverviewResignFromGuildReady = true;
                            m_Player.SendMessage(2115, "Click again to confirm resignation from this guild.");
                        }

                        closeGump = false;
                        break;
                }
            }

            #endregion

            #region Members

            else if (m_GuildGumpObject.m_GumpPageType == GuildGumpPageType.Members)
            {
                if (m_GuildGumpObject.m_Guild == null) return;
                if (!m_GuildGumpObject.m_Guild.IsMember(m_Player)) return;

                Guild guild = m_GuildGumpObject.m_Guild;

                if (guild == null)
                    return;

                GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(m_Player);

                if (guildMemberEntry == null)
                    return;

                if (info.ButtonID >= m_GuildGumpObject.m_MemberButtonIndexOffset)
                {
                    int memberBaseIndex = info.ButtonID - m_GuildGumpObject.m_MemberButtonIndexOffset;
                    int memberIndex = (int)(Math.Floor((double)memberBaseIndex / 10));
                    int memberRemainder = memberBaseIndex % 10;

                    if (memberIndex < m_GuildGumpObject.m_MembersSorted.Count)
                    {
                        GuildMemberEntry member = m_GuildGumpObject.m_MembersSorted[memberIndex];

                        if (member == null)
                            m_Player.SendMessage("That player is no longer a member of the guild.");

                        else if (member.m_Player == null)
                            m_Player.SendMessage("That player no longer exists.");

                        else if (member.m_Player.Deleted)
                            m_Player.SendMessage("That player no longer exists.");

                        else
                        {
                            switch (memberRemainder)
                            {
                                //Last Online Button
                                case 0:
                                    m_GuildGumpObject.m_MemberDismissPlayerReady = false;
                                    m_GuildGumpObject.m_MemberPlayerToDismiss = null;

                                break;

                                //Manage Player
                                case 1:
                                    m_GuildGumpObject.m_MemberDismissPlayerReady = false;
                                    m_GuildGumpObject.m_MemberPlayerToDismiss = null;

                                    m_GuildGumpObject.m_MemberManagementPlayer = member;
                                    m_GuildGumpObject.m_MemberManagementNewRankIndex = (int)member.m_Rank;
                                    m_Player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.MemberManagement;

                                    m_Player.SendSound(Guilds.GuildGumpOpenGumpSound);
                                break;

                                //Declare Fealty
                                case 2:
                                    m_GuildGumpObject.m_MemberDismissPlayerReady = false;
                                    m_GuildGumpObject.m_MemberPlayerToDismiss = null;

                                    if (guildMemberEntry.m_DeclaredFealty != member.m_Player)
                                    {
                                        guildMemberEntry.m_DeclaredFealty = member.m_Player;

                                        PlayerMobile oldGuildMaster = guild.m_Guildmaster;

                                        guild.OnFealtyChange();

                                        if (oldGuildMaster == guild.m_Guildmaster)
                                        {
                                            if (member.m_Player == m_Player)
                                                m_Player.SendMessage(Guilds.GuildTextHue, "You declare fealty to yourself. " + Utility.CreateDecimalPercentageString(Guilds.RequiredFealtyPercentageForChange, 1) + " of active players in the guild must support you to be guildmaster.");

                                            else if (member.m_Player != guild.m_Guildmaster)
                                                m_Player.SendMessage(Guilds.GuildTextHue, "You declare fealty to " + member.m_Player.RawName + ". " + Utility.CreateDecimalPercentageString(Guilds.RequiredFealtyPercentageForChange, 1) + " of active players in the guild must support this player to make them guildmaster.");

                                            else
                                                m_Player.SendMessage(Guilds.GuildTextHue, "You declare fealty to " + member.m_Player.RawName + ". ");
                                        }

                                        else
                                        {
                                        }
                                    }

                                    else
                                    {
                                        if (member.m_Player == m_Player)
                                            m_Player.SendMessage("You have already declared fealty to yourself.");

                                        else
                                            m_Player.SendMessage("You have already declared fealty to " + member.m_Player.RawName + ".");
                                    }
                                break;

                                //Dismiss Player
                                case 3:
                                    if (!guild.CanDismissPlayer(guildMemberEntry.m_Rank, member.m_Rank) && m_Player != member.m_Player)
                                    {
                                        m_Player.SendMessage("You are not high enough rank in this guild to dimiss that member.");

                                        m_GuildGumpObject.m_MemberDismissPlayerReady = false;
                                        m_GuildGumpObject.m_MemberPlayerToDismiss = null;
                                    }

                                    else if (m_GuildGumpObject.m_MemberDismissPlayerReady && m_GuildGumpObject.m_MemberPlayerToDismiss == member)
                                    {
                                        m_GuildGumpObject.m_MemberDismissPlayerReady = false;
                                        m_GuildGumpObject.m_MemberPlayerToDismiss = null;

                                        guild.DismissMember(member.m_Player, true, true);
                                    }

                                    else
                                    {
                                        m_GuildGumpObject.m_MemberDismissPlayerReady = true;
                                        m_GuildGumpObject.m_MemberPlayerToDismiss = member;

                                        if (member.m_Player == m_Player)
                                            m_Player.SendMessage(2115, "Click again to resign from this guild.");

                                        else
                                            m_Player.SendMessage(2115, "Click again to dismiss this player from the guild.");
                                    }
                                break;
                            }
                        }
                    }

                    closeGump = false;
                }

                switch (info.ButtonID)
                {
                    //Search Previous Player
                    case 4:
                        closeGump = false;
                        break;

                    //Search Next Player
                    case 5:
                        closeGump = false;
                        break;

                    //Sort by Last Online
                    case 6:
                        if (m_GuildGumpObject.m_MemberSortCriteria == Guilds.MemberSortCriteria.LastOnline)
                            m_GuildGumpObject.m_MemberSortAscending = !m_GuildGumpObject.m_MemberSortAscending;

                        else
                        {
                            m_GuildGumpObject.m_MemberSortCriteria = Guilds.MemberSortCriteria.LastOnline;
                            m_GuildGumpObject.m_MemberSortAscending = false;
                        }

                        closeGump = false;
                        break;

                    //Sort by Player Name
                    case 7:
                        if (m_GuildGumpObject.m_MemberSortCriteria == Guilds.MemberSortCriteria.PlayerName)
                            m_GuildGumpObject.m_MemberSortAscending = !m_GuildGumpObject.m_MemberSortAscending;

                        else
                        {
                            m_GuildGumpObject.m_MemberSortCriteria = Guilds.MemberSortCriteria.PlayerName;
                            m_GuildGumpObject.m_MemberSortAscending = true;
                        }

                        closeGump = false;
                        break;

                    //Sort by Guild Rank
                    case 8:
                        if (m_GuildGumpObject.m_MemberSortCriteria == Guilds.MemberSortCriteria.GuildRank)
                            m_GuildGumpObject.m_MemberSortAscending = !m_GuildGumpObject.m_MemberSortAscending;

                        else
                        {
                            m_GuildGumpObject.m_MemberSortCriteria = Guilds.MemberSortCriteria.GuildRank;
                            m_GuildGumpObject.m_MemberSortAscending = false;
                        }

                        closeGump = false;
                        break;

                    //Previous Page
                    case 9:
                        closeGump = false;
                        break;

                    //Next Page
                    case 10:
                        closeGump = false;
                        break;

                    //Invite New Member
                    case 11:
                        //TEST: NEED TO CLOSE AND REOPEN THIS GUMP UPON COMPLETION

                        m_GuildGumpObject.m_Guild.RecruitMember(m_Player);

                        closeGump = false;
                    break;
                }
            }

            #endregion            

            #region Manage Member

            else if (m_GuildGumpObject.m_GumpPageType == GuildGumpPageType.MemberManagement)
            {
                Guild guild = m_GuildGumpObject.m_Guild;

                if (guild == null)
                    return;                

                GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(m_Player);

                if (guildMemberEntry == null)
                    return;                

                GuildMemberEntry guildMemberTargetEntry = m_GuildGumpObject.m_MemberManagementPlayer;

                if (guildMemberTargetEntry == null)
                    return;                

                if (guildMemberTargetEntry.m_Player == null)
                    return;                

                if (guildMemberTargetEntry.m_Player.Deleted)
                    return;                

                PlayerMobile playerTarget = guildMemberTargetEntry.m_Player;

                //-----

                bool canIncreaseRank = false;
                bool canDecreaseRank = false;

                bool canChangeValues = false;
                bool valueChanged = false;

                bool canMessage = false;

                int possibleRanks = Enum.GetNames(typeof(GuildMemberRank)).Length;

                int newRankIndex = m_GuildGumpObject.m_MemberManagementNewRankIndex;

                if (newRankIndex < 0)
                    newRankIndex = 0;

                if (newRankIndex >= possibleRanks)
                    newRankIndex = possibleRanks - 1;

                canIncreaseRank = true;
                canDecreaseRank = true;

                if (newRankIndex <= 0)
                    canDecreaseRank = false;

                if (newRankIndex >= possibleRanks - 2)
                    canIncreaseRank = false;

                if ((int)guildMemberEntry.m_Rank < newRankIndex)
                    canIncreaseRank = false;

                if ((int)guildMemberEntry.m_Rank <= (int)guildMemberTargetEntry.m_Rank)
                {
                    canIncreaseRank = false;
                    canDecreaseRank = false;
                }

                if (!m_Player.Guild.CanPromoteMembers(guildMemberEntry.m_Rank))
                {
                    canIncreaseRank = false;
                    canDecreaseRank = false;
                }

                //TEST: Add Support for Lowering Own Rank (Add Need Guildmaster Check if Guildmaster Does it)
                if (m_Player == playerTarget)
                {
                    canIncreaseRank = false;
                    canDecreaseRank = false;
                }

                if (canIncreaseRank || canDecreaseRank)
                    canChangeValues = true;

                if (newRankIndex != (int)guildMemberTargetEntry.m_Rank)
                    valueChanged = true;     

                switch (info.ButtonID)
                {
                    //Increase Rank
                    case 4:
                        if (canChangeValues && canIncreaseRank)
                        {
                            m_GuildGumpObject.m_MemberManagementNewRankIndex++;

                            m_Player.SendSound(Guilds.GuildGumpSelectionSound);
                        }

                        closeGump = false;
                    break;

                    //Decrease Rank
                    case 5:
                        if (canChangeValues && canDecreaseRank)
                        {
                            m_GuildGumpObject.m_MemberManagementNewRankIndex--;

                            m_Player.SendSound(Guilds.GuildGumpSelectionSound);
                        }

                        closeGump = false;
                    break;

                    //Accept Change
                    case 6:
                        if (canChangeValues && valueChanged)
                        {
                            if (newRankIndex < 0)
                                newRankIndex = 0;

                            if (newRankIndex >= possibleRanks)
                                newRankIndex = possibleRanks - 1;

                            GuildMemberRank guildMemberRank = (GuildMemberRank)newRankIndex;

                            guildMemberTargetEntry.m_Rank = guildMemberRank;
                            
                            string guildRankTitle = guild.GetRankName(guildMemberRank);

                            List<PlayerMobile> ignored = new List<PlayerMobile>() { playerTarget };

                            guild.GuildAnnouncement(playerTarget.RawName + "'s rank has changed to " + guildRankTitle + ".", ignored, GuildMemberRank.Recruit);

                            playerTarget.SendMessage(Guilds.GuildTextHue, "Your guild rank has changed to " + guildRankTitle + ".");
                            playerTarget.SendSound(Guilds.PromotionSound);                            
                        }

                        closeGump = false;
                    break;

                    //Cancel Change
                    case 7:
                        if (canChangeValues && valueChanged)                        
                            m_GuildGumpObject.m_MemberManagementNewRankIndex = (int)guildMemberTargetEntry.m_Rank;                        

                        closeGump = false;
                    break;

                    //Send Message
                    case 8:                    
                        closeGump = false;
                    break;

                    //Return
                    case 9:
                        m_Player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.Members;
                        m_Player.SendSound(Guilds.GuildGumpOpenGumpSound);

                        closeGump = false;
                    break;
                }
            }

            #endregion

            #region Candidates

            else if (m_GuildGumpObject.m_GumpPageType == GuildGumpPageType.Candidates)
            {
                Guild guild = m_GuildGumpObject.m_Guild;

                if (guild == null)
                    return;

                GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(m_Player);

                if (guildMemberEntry == null)
                    return;

                if (info.ButtonID >= m_GuildGumpObject.m_CandidateButtonIndexOffset)
                {
                    int candidateBaseIndex = info.ButtonID - m_GuildGumpObject.m_CandidateButtonIndexOffset;
                    int candidateIndex = (int)(Math.Floor((double)candidateBaseIndex / 10));
                    int candidateRemainder = candidateBaseIndex % 10;

                    if (candidateIndex < m_GuildGumpObject.m_CandidatesSorted.Count)
                    {
                        GuildInvitation candidate = m_GuildGumpObject.m_CandidatesSorted[candidateIndex];

                        if (candidate == null)
                            m_Player.SendMessage("That player is no longer a candidate for guild membership.");

                        else if (candidate.Deleted)
                            m_Player.SendMessage("That player is no longer a candidate for guild membership.");

                        else
                        {
                            switch (candidateRemainder)
                            {
                                //Player Info
                                case 0:
                                    m_GuildGumpObject.m_CandidatePreviewCandidate = candidate;
                                    m_Player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.CandidatePreview;

                                    m_Player.SendSound(Guilds.GuildGumpOpenGumpSound);
                                break;

                                //Approve
                                case 1:
                                    PlayerMobile playerTarget = candidate.m_PlayerTarget;

                                    if (!guild.CanApproveCandidates(guildMemberEntry.m_Rank))
                                        m_Player.SendMessage("You do not have a high enough rank in this guild to approve candidates for membership.");

                                    else if (!candidate.m_Accepted)
                                        m_Player.SendMessage("That player has not yet accepted this guild invitation.");

                                    else if (playerTarget == null)
                                    {
                                        candidate.Delete();
                                        m_Player.SendMessage("That player no longer exists.");
                                    }

                                    else if (playerTarget.Deleted)
                                    {
                                        candidate.Delete();
                                        m_Player.SendMessage("That player no longer exists.");
                                    }

                                    else if (playerTarget.Guild != null)
                                    {
                                        candidate.Delete();
                                        m_Player.SendMessage("That player already belongs to a guild.");
                                    }

                                    else
                                    {
                                        candidate.Delete();

                                        guild.GuildAnnouncement(playerTarget.RawName + " has been made a member of " + guild.GetDisplayName(true) + ".", new List<PlayerMobile>(), GuildMemberRank.Recruit);
                                        guild.AddMember(playerTarget, GuildMemberRank.Recruit);

                                        playerTarget.SendMessage(Guilds.GuildTextHue, "You have been made a member of " + guild.GetDisplayName(true) + ".");

                                        Guilds.LaunchGuildGump(m_Player);

                                        return;
                                    }
                                    break;

                                //Reject
                                case 2:
                                    candidate.Delete();
                                break;
                            }
                        }
                    }

                    closeGump = false;
                }

                switch (info.ButtonID)
                {
                    //Search Previous Player
                    case 4:
                        closeGump = false;
                        break;

                    //Search Next Player
                    case 5:
                        closeGump = false;
                        break;

                    //Sort by Accepted
                    case 6:
                        if (m_GuildGumpObject.m_CandidateSortCriteria == Guilds.CandidateSortCriteria.Accepted)
                            m_GuildGumpObject.m_CandidateSortAscending = !m_GuildGumpObject.m_CandidateSortAscending;

                        else
                        {
                            m_GuildGumpObject.m_CandidateSortCriteria = Guilds.CandidateSortCriteria.Accepted;
                            m_GuildGumpObject.m_CandidateSortAscending = true;
                        }

                        closeGump = false;
                        break;

                    //Sort by Player Name
                    case 7:
                        if (m_GuildGumpObject.m_CandidateSortCriteria == Guilds.CandidateSortCriteria.PlayerName)
                            m_GuildGumpObject.m_CandidateSortAscending = !m_GuildGumpObject.m_CandidateSortAscending;

                        else
                        {
                            m_GuildGumpObject.m_CandidateSortCriteria = Guilds.CandidateSortCriteria.PlayerName;
                            m_GuildGumpObject.m_CandidateSortAscending = true;
                        }

                        closeGump = false;
                        break;

                    //Sort by Expiration
                    case 8:
                        if (m_GuildGumpObject.m_CandidateSortCriteria == Guilds.CandidateSortCriteria.Expiration)
                            m_GuildGumpObject.m_CandidateSortAscending = !m_GuildGumpObject.m_CandidateSortAscending;

                        else
                        {
                            m_GuildGumpObject.m_CandidateSortCriteria = Guilds.CandidateSortCriteria.Expiration;
                            m_GuildGumpObject.m_CandidateSortAscending = false;
                        }

                        closeGump = false;
                        break;

                    //Previous Page
                    case 9:
                        closeGump = false;
                        break;

                    //Next Page
                    case 10:
                        closeGump = false;
                        break;

                    //Invite New Member
                    case 11:
                        //TEST: NEED TO CLOSE AND REOPEN THIS GUMP UPON COMPLETION

                        m_GuildGumpObject.m_Guild.RecruitMember(m_Player);

                        closeGump = false;
                    break;
                }
            }

            #endregion

            #region Candidate Preview

            else if (m_GuildGumpObject.m_GumpPageType == GuildGumpPageType.CandidatePreview)
            {
                Guild guild = m_GuildGumpObject.m_Guild;

                if (guild == null)
                    return;

                GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(m_Player);

                if (guildMemberEntry == null)
                    return;

                GuildInvitation candidate = m_GuildGumpObject.m_CandidatePreviewCandidate;

                if (candidate == null) return;
                if (candidate.m_PlayerTarget == null) return;
                if (candidate.m_PlayerTarget.Deleted) return;

                PlayerMobile playerTarget = candidate.m_PlayerTarget;

                bool canApprove = guild.CanApproveCandidates(guildMemberEntry.m_Rank);

                switch (info.ButtonID)
                {
                    //Approve
                    case 4:
                        if (!canApprove)
                            m_Player.SendMessage("You do not have a high enough rank in this guild to approve candidates for membership.");

                        else if (!candidate.m_Accepted)
                            m_Player.SendMessage("That candidate has not yet accepted this guild invitation.");

                        else
                        {
                            candidate.Delete();

                            guild.GuildAnnouncement(playerTarget.RawName + " has been made a member of " + guild.GetDisplayName(true) + ".", new List<PlayerMobile>(), GuildMemberRank.Recruit);
                            guild.AddMember(m_Player, GuildMemberRank.Recruit);

                            playerTarget.SendMessage(Guilds.GuildTextHue, "You have been made a member of " + guild.GetDisplayName(true) + ".");

                            playerTarget.SendSound(Guilds.PromotionSound);

                            if (playerTarget.HasGump(typeof(GuildGump)))
                                Guilds.LaunchGuildGump(playerTarget);

                            return;
                        }

                        closeGump = false;
                        break;

                    //Decline
                    case 5:
                        if (!canApprove)
                            m_Player.SendMessage("You do not have a high enough rank in this guild to decline candidates for membership.");

                        else
                        {
                            candidate.Delete();

                            m_Player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.Candidates;
                            m_Player.SendSound(Guilds.GuildGumpCloseGumpSound);
                        }

                        closeGump = false;
                        break;

                    //Send Message
                    case 6:
                        closeGump = false;
                        break;

                    //Return;
                    case 7:
                        m_Player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.Candidates;
                        m_Player.SendSound(Guilds.GuildGumpOpenGumpSound);

                        closeGump = false;
                    break;
                }
            }

            #endregion

            #region Diplomacy

            else if (m_GuildGumpObject.m_GumpPageType == GuildGumpPageType.Diplomacy)
            {
                Guild guild = m_GuildGumpObject.m_Guild;

                if (guild == null)
                    return;

                if (info.ButtonID >= m_GuildGumpObject.m_RelationshipButtonIndexOffset)
                {
                    int relationshipBaseIndex = info.ButtonID - m_GuildGumpObject.m_RelationshipButtonIndexOffset;
                    int relationshipIndex = (int)(Math.Floor((double)relationshipBaseIndex / 10));
                    int relationshipRemainder = relationshipBaseIndex % 10;

                    if (relationshipIndex < m_GuildGumpObject.m_RelationshipsSorted.Count)
                    {
                        DiplomacyEntry diplomacyEntry = m_GuildGumpObject.m_RelationshipsSorted[relationshipIndex];

                        if (diplomacyEntry == null)
                            m_Player.SendMessage("Details of that guild have changed.");

                        else if (diplomacyEntry.m_Guild == null)
                            m_Player.SendMessage("Details of that guild have changed.");

                        else if (diplomacyEntry.m_Guild.Deleted)
                            m_Player.SendMessage("Details of that guild have changed.");

                        else
                        {
                            switch (relationshipRemainder)
                            {
                                //Manage Relationship
                                case 0:
                                    m_GuildGumpObject.m_DiplomacyManagementNewRelationshipType = Guilds.GuildRelationshipType.Neutral;
                                    m_GuildGumpObject.m_DiplomacyManagementDiplomacyEntry = diplomacyEntry;
                                    m_Player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.DiplomacyManagement;

                                    m_Player.SendSound(Guilds.GuildGumpOpenGumpSound);
                                break;
                            }
                        }
                    }

                    closeGump = false;
                }

                switch (info.ButtonID)
                {
                    //Search Previous Player
                    case 4:
                        closeGump = false;
                        break;

                    //Search Next Player
                    case 5:
                        closeGump = false;
                        break;

                    //Sort by Guild Name
                    case 6:
                        if (m_GuildGumpObject.m_RelationshipSortCriteria == Guilds.RelationshipSortCriteria.GuildName)
                            m_GuildGumpObject.m_RelationshipSortAscending = !m_GuildGumpObject.m_RelationshipSortAscending;

                        else
                        {
                            m_GuildGumpObject.m_RelationshipSortCriteria = Guilds.RelationshipSortCriteria.GuildName;
                            m_GuildGumpObject.m_RelationshipSortAscending = true;
                        }

                        closeGump = false;
                        break;

                    //Sort by Relationship
                    case 7:
                        if (m_GuildGumpObject.m_RelationshipSortCriteria == Guilds.RelationshipSortCriteria.Relationship)
                            m_GuildGumpObject.m_RelationshipSortAscending = !m_GuildGumpObject.m_RelationshipSortAscending;

                        else
                        {
                            m_GuildGumpObject.m_RelationshipSortCriteria = Guilds.RelationshipSortCriteria.Relationship;
                            m_GuildGumpObject.m_RelationshipSortAscending = false;
                        }

                        closeGump = false;
                        break;

                    //Sort by Player Count
                    case 8:
                        if (m_GuildGumpObject.m_RelationshipSortCriteria == Guilds.RelationshipSortCriteria.PlayerCount)
                            m_GuildGumpObject.m_RelationshipSortAscending = !m_GuildGumpObject.m_RelationshipSortAscending;

                        else
                        {
                            m_GuildGumpObject.m_RelationshipSortCriteria = Guilds.RelationshipSortCriteria.PlayerCount;
                            m_GuildGumpObject.m_RelationshipSortAscending = false;
                        }

                        closeGump = false;
                        break;

                    //Previous Filter
                    case 9:
                        switch (m_GuildGumpObject.m_RelationshipFilterType)
                        {
                            case Guilds.RelationshipFilterType.ShowActiveAgreements: m_GuildGumpObject.m_RelationshipFilterType = Guilds.RelationshipFilterType.ShowAvailableGuilds; break;
                            case Guilds.RelationshipFilterType.ShowAgreementsReceived: m_GuildGumpObject.m_RelationshipFilterType = Guilds.RelationshipFilterType.ShowActiveAgreements; break;
                            case Guilds.RelationshipFilterType.ShowAgreementsSent: m_GuildGumpObject.m_RelationshipFilterType = Guilds.RelationshipFilterType.ShowAgreementsReceived; break;
                            case Guilds.RelationshipFilterType.ShowAvailableGuilds: m_GuildGumpObject.m_RelationshipFilterType = Guilds.RelationshipFilterType.ShowAgreementsSent; break;
                        }

                        m_Player.m_GuildSettings.m_RelationshipFilterType = m_GuildGumpObject.m_RelationshipFilterType;

                        m_Player.SendSound(Guilds.GuildGumpSelectionSound);

                        closeGump = false;
                        break;

                    //Next Filter
                    case 10:
                        switch (m_GuildGumpObject.m_RelationshipFilterType)
                        {
                            case Guilds.RelationshipFilterType.ShowActiveAgreements: m_GuildGumpObject.m_RelationshipFilterType = Guilds.RelationshipFilterType.ShowAgreementsReceived; break;
                            case Guilds.RelationshipFilterType.ShowAgreementsReceived: m_GuildGumpObject.m_RelationshipFilterType = Guilds.RelationshipFilterType.ShowAgreementsSent; break;
                            case Guilds.RelationshipFilterType.ShowAgreementsSent: m_GuildGumpObject.m_RelationshipFilterType = Guilds.RelationshipFilterType.ShowAvailableGuilds; break;
                            case Guilds.RelationshipFilterType.ShowAvailableGuilds: m_GuildGumpObject.m_RelationshipFilterType = Guilds.RelationshipFilterType.ShowActiveAgreements; break;
                        }

                        m_Player.m_GuildSettings.m_RelationshipFilterType = m_GuildGumpObject.m_RelationshipFilterType;

                        m_Player.SendSound(Guilds.GuildGumpSelectionSound);

                        closeGump = false;
                        break;

                    //Previous Page
                    case 11:
                        closeGump = false;
                        break;

                    //Next Page
                    case 12:
                        closeGump = false;
                    break;
                }
            }

            #endregion

            #region Diplomacy Manage Relationship

            else if (m_GuildGumpObject.m_GumpPageType == GuildGumpPageType.DiplomacyManagement)
            {
                Guild guild = m_GuildGumpObject.m_Guild;
                GuildMemberEntry guildMemberEntry = guild.GetGuildMemberEntry(m_Player);
                DiplomacyEntry diplomacyEntry = m_GuildGumpObject.m_DiplomacyManagementDiplomacyEntry;

                if (guild == null) return;
                if (guildMemberEntry == null) return;
                if (diplomacyEntry == null) return;
                if (diplomacyEntry.m_Guild == null) return;

                Guild guildTarget = diplomacyEntry.m_Guild;

                if (guildTarget == null) return;
                if (guildTarget.Deleted) return;

                GuildRelationship guildRelationship = guild.GetGuildRelationship(guildTarget);

                switch (info.ButtonID)
                {
                    //House Location
                    case 4:
                        if (guildTarget.m_Guildhouse != null)
                        {
                        }

                        closeGump = false;
                        break;

                    //Website
                    case 5:
                        if (m_Player.NetState != null)
                            m_Player.NetState.LaunchBrowser(guild.m_Website);

                        closeGump = false;
                        break;

                    //Previous Relationship Type
                    case 6:
                        if (!guild.CanManageDiplomacy(guildMemberEntry.m_Rank))
                            m_Player.SendMessage("You are not high enough rank in this guild to perform diplomacy.");

                        else if (guildRelationship == null)
                        {
                            switch (m_GuildGumpObject.m_DiplomacyManagementNewRelationshipType)
                            {
                                case Guilds.GuildRelationshipType.Neutral:
                                    m_GuildGumpObject.m_DiplomacyManagementNewRelationshipType = Guilds.GuildRelationshipType.WarRequest;
                                    break;

                                case Guilds.GuildRelationshipType.WarRequest:
                                    m_GuildGumpObject.m_DiplomacyManagementNewRelationshipType = Guilds.GuildRelationshipType.AllianceRequest;
                                    break;

                                case Guilds.GuildRelationshipType.AllianceRequest:
                                    m_GuildGumpObject.m_DiplomacyManagementNewRelationshipType = Guilds.GuildRelationshipType.Neutral;
                                    break;
                            }

                            m_Player.SendSound(Guilds.GuildGumpSelectionSound);
                        }

                        closeGump = false;
                        break;

                    //Next Relationship Type
                    case 7:
                        if (!guild.CanManageDiplomacy(guildMemberEntry.m_Rank))
                            m_Player.SendMessage("You are not high enough rank in this guild to perform diplomatic actions.");

                        else if (guildRelationship == null)
                        {
                            switch (m_GuildGumpObject.m_DiplomacyManagementNewRelationshipType)
                            {
                                case Guilds.GuildRelationshipType.Neutral:
                                    m_GuildGumpObject.m_DiplomacyManagementNewRelationshipType = Guilds.GuildRelationshipType.AllianceRequest;
                                    break;

                                case Guilds.GuildRelationshipType.AllianceRequest:
                                    m_GuildGumpObject.m_DiplomacyManagementNewRelationshipType = Guilds.GuildRelationshipType.WarRequest;
                                    break;

                                case Guilds.GuildRelationshipType.WarRequest:
                                    m_GuildGumpObject.m_DiplomacyManagementNewRelationshipType = Guilds.GuildRelationshipType.Neutral;
                                    break;
                            }

                            m_Player.SendSound(Guilds.GuildGumpSelectionSound);
                        }

                        closeGump = false;
                        break;

                    //Approve
                    case 8:
                        if (!guild.CanManageDiplomacy(guildMemberEntry.m_Rank))
                            m_Player.SendMessage("You are not high enough rank in this guild to perform diplomatic actions.");

                        else if (guildRelationship == null)
                        {
                            switch (m_GuildGumpObject.m_DiplomacyManagementNewRelationshipType)
                            {
                                case Guilds.GuildRelationshipType.Neutral:
                                    break;

                                case Guilds.GuildRelationshipType.AllianceRequest:
                                    GuildRelationship allyRequest = new GuildRelationship(guild, guildTarget, Guilds.GuildRelationshipType.AllianceRequest, DateTime.UtcNow, DateTime.UtcNow);

                                    guild.m_Relationships.Add(allyRequest);
                                    guildTarget.m_Relationships.Add(allyRequest);

                                    m_Player.SendMessage(2599, "You send an Alliance offer to " + guildTarget.GetDisplayName(true) + ".");
                                    m_Player.PlaySound(Guilds.GuildGumpSelectionSound);

                                    guildTarget.GuildAnnouncement("Your guild has received a new Alliance request.", new List<PlayerMobile>(), GuildMemberRank.Officer);
                                break;

                                case Guilds.GuildRelationshipType.WarRequest:
                                    GuildRelationship warRequest = new GuildRelationship(guild, guildTarget, Guilds.GuildRelationshipType.WarRequest, DateTime.UtcNow, DateTime.UtcNow);

                                    guild.m_Relationships.Add(warRequest);
                                    guildTarget.m_Relationships.Add(warRequest);

                                    m_Player.SendMessage(1256, "You send a War offer to " + guildTarget.GetDisplayName(true) + ".");
                                    m_Player.PlaySound(Guilds.GuildGumpSelectionSound);

                                    guildTarget.GuildAnnouncement("Your guild has received a new War request.", new List<PlayerMobile>(), GuildMemberRank.Officer);   
                                break;
                            }
                        }

                        else
                        {
                            switch (guildRelationship.m_RelationshipType)
                            {
                                case Guilds.GuildRelationshipType.War:
                                    break;

                                case Guilds.GuildRelationshipType.Alliance:
                                    break;

                                case Guilds.GuildRelationshipType.WarRequest:
                                    if (guildRelationship.m_GuildTarget == guild)
                                    {
                                        guildRelationship.m_RelationshipType = Guilds.GuildRelationshipType.War;

                                        guild.GuildAnnouncement("Your guild is now at War with " + guildTarget.GetDisplayName(true) + ".", new List<PlayerMobile>(), GuildMemberRank.Recruit);
                                        guildTarget.GuildAnnouncement("Your guild is now at War with " + guild.GetDisplayName(true) + ".", new List<PlayerMobile>(), GuildMemberRank.Recruit);
                                    }
                                break;

                                case Guilds.GuildRelationshipType.AllianceRequest:
                                    if (guildRelationship.m_GuildTarget == guild)
                                    {
                                        guildRelationship.m_RelationshipType = Guilds.GuildRelationshipType.Alliance;

                                        guild.GuildAnnouncement("Your guild is now Allied with " + guildTarget.GetDisplayName(true) + ".", new List<PlayerMobile>(), GuildMemberRank.Recruit);
                                        guildTarget.GuildAnnouncement("Your guild is now Allied with " + guild.GetDisplayName(true) + ".", new List<PlayerMobile>(), GuildMemberRank.Recruit);
                                    }
                                break;
                            }
                        }

                        closeGump = false;
                    break;

                    //Decline 
                    case 9:
                        if (!guild.CanManageDiplomacy(guildMemberEntry.m_Rank))
                            m_Player.SendMessage("You are not high enough rank in this guild to perform diplomacy.");

                        else if (guildRelationship == null)
                        {
                            switch (m_GuildGumpObject.m_DiplomacyManagementNewRelationshipType)
                            {
                                case Guilds.GuildRelationshipType.Neutral:
                                break;

                                case Guilds.GuildRelationshipType.AllianceRequest:
                                break;

                                case Guilds.GuildRelationshipType.WarRequest:
                                break;
                            }
                        }

                        else
                        {
                            switch (guildRelationship.m_RelationshipType)
                            {
                                case Guilds.GuildRelationshipType.War:
                                    guildRelationship.Delete();

                                    guild.GuildAnnouncement("Your guild is no longer at War with " + guildTarget.GetDisplayName(true) + ".", new List<PlayerMobile>(), GuildMemberRank.Recruit);
                                    guildTarget.GuildAnnouncement("Your guild is no longer at War with " + guild.GetDisplayName(true) + ".", new List<PlayerMobile>(), GuildMemberRank.Recruit);
                                break;

                                case Guilds.GuildRelationshipType.Alliance:
                                    guildRelationship.Delete();

                                    guild.GuildAnnouncement("Your guild is no longer Allied with " + guildTarget.GetDisplayName(true) + ".", new List<PlayerMobile>(), GuildMemberRank.Recruit);
                                    guildTarget.GuildAnnouncement("Your guild is no Allied with " + guild.GetDisplayName(true) + ".", new List<PlayerMobile>(), GuildMemberRank.Recruit);
                                break;

                                case Guilds.GuildRelationshipType.WarRequest:
                                    if (guildRelationship.m_GuildFrom == guild)
                                        m_Player.SendMessage("You retract the War offer.");
                                    else
                                        m_Player.SendMessage("You decline the War request.");

                                    guildRelationship.Delete();
                                break;

                                case Guilds.GuildRelationshipType.AllianceRequest:
                                    if (guildRelationship.m_GuildFrom == guild)
                                        m_Player.SendMessage("You retract the Alliance offer.");
                                    else
                                        m_Player.SendMessage("You decline the Alliance request.");

                                    guildRelationship.Delete();
                                break;
                            }
                        }

                        closeGump = false;
                    break;

                    //Send Message 
                    case 10:
                        if (!guild.CanManageDiplomacy(guildMemberEntry.m_Rank))
                            m_Player.SendMessage("You are not high enough rank in this guild to perform diplomacy.");

                        closeGump = false;
                        break;

                    //Return
                    case 11:
                        m_Player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.Diplomacy;
                        m_Player.SendSound(Guilds.GuildGumpCloseGumpSound);

                        closeGump = false;
                    break;
                }
            }

            #endregion

            #region Faction

            else if (m_GuildGumpObject.m_GumpPageType == GuildGumpPageType.Faction)
            {
            }

            #endregion

            #region Messages

            else if (m_GuildGumpObject.m_GumpPageType == GuildGumpPageType.Messages)
            {
            }

            #endregion

            //-----

            #region General Handling

            List<GuildGumpPageType> m_GuildTabs = Guilds.GetGuildPageTypeList(m_Player);

            int GuildTabsPerPage = 4;
            int TotalGuildTabs = m_GuildTabs.Count;
            int TotalGuildTabPages = (int)(Math.Ceiling((double)TotalGuildTabs / (double)GuildTabsPerPage));

            if (m_GuildGumpObject.m_GuildTabPage >= TotalGuildTabPages)
                m_GuildGumpObject.m_GuildTabPage = TotalGuildTabPages - 1;

            if (m_GuildGumpObject.m_GuildTabPage < 0)
                m_GuildGumpObject.m_GuildTabPage = 0;

            int guildTabStartIndex = m_GuildGumpObject.m_GuildTabPage * GuildTabsPerPage;
            int guildTabEndIndex = (m_GuildGumpObject.m_GuildTabPage * GuildTabsPerPage) + (GuildTabsPerPage - 1);

            if (guildTabEndIndex >= TotalGuildTabs)
                guildTabEndIndex = TotalGuildTabs - 1;

            int guildTabCount = guildTabEndIndex - guildTabStartIndex;

            //General Handling
            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                    break;

                //Previous Guild Page
                case 2:
                    if (m_GuildGumpObject.m_GuildTabPage > 0)
                    {
                        m_GuildGumpObject.m_GuildTabPage--;
                        m_Player.SendSound(Guilds.GuildGumpChangePageSound);
                    }

                    closeGump = false;
                break;

                //Next Guild Page
                case 3:
                    if (m_GuildGumpObject.m_GuildTabPage < TotalGuildTabPages - 1)
                    {
                        m_GuildGumpObject.m_GuildTabPage++;
                        m_Player.SendSound(Guilds.GuildGumpChangePageSound);
                    }

                    closeGump = false;
                break;
            }

            //Change Guild Page Tab
            if (info.ButtonID >= 20 && info.ButtonID <= 50)
            {
                int buttonIndex = info.ButtonID - 20;

                if (buttonIndex < m_GuildTabs.Count)
                {
                    if (m_GuildGumpObject != null)                    
                        m_GuildGumpObject.ClearData();                    

                    m_Player.m_GuildSettings.m_GuildGumpPage = m_GuildTabs[buttonIndex];
                    m_Player.SendSound(Guilds.GuildGumpOpenGumpSound);
                }

                closeGump = false;
            }

            if (!closeGump)            
                Guilds.SendGuildGump(m_Player, m_GuildGumpObject);            

            else
                m_Player.SendSound(Guilds.GuildGumpCloseGumpSound);

            #endregion
        }
    }

    public class GuildGumpObject
    {
        public Guild m_Guild;
        public int m_GuildTabPage = 0;
        public GuildGumpPageType m_GumpPageType = GuildGumpPageType.CreateGuild;

        //Create Guild
        public string m_CreateGuildName = "Guild Name";
        public string m_CreateGuildAbbreviation = "ABC";
        public int m_CreateGuildSymbolIndex = 0;
        public GuildSymbolType m_CreateGuildSymbol;

        //Invitations
        public List<GuildInvitation> m_InvitationsSorted = new List<GuildInvitation>();

        public string m_InvitationSearchText = "Guild Name";
        public int m_InvitationSearchIndex = 0;
        public int m_InvitationPage = 0;
        public Guilds.InvitationSortCriteria m_InvitationSortCriteria = Guilds.InvitationSortCriteria.GuildName;
        public bool m_InvitationSortAscending = true;
        public int m_InvitationButtonIndexOffset = 100;

        //Invitation Preview
        public GuildInvitation m_InvitationGuildInvitation = null;

        //Candidates
        public List<GuildInvitation> m_CandidatesSorted = new List<GuildInvitation>();
       
        public string m_CandidateSearchText = "Player Name";
        public int m_CandidateSearchIndex = 0;
        public int m_CandidatePage = 0;
        public Guilds.CandidateSortCriteria m_CandidateSortCriteria = Guilds.CandidateSortCriteria.PlayerName;
        public bool m_CandidateSortAscending = true;
        public int m_CandidateButtonIndexOffset = 100;

        //Candidate Preview
        public GuildInvitation m_CandidatePreviewCandidate = null;

        //Overview
        public bool m_OverviewResignFromGuildReady = false;

        //Members
        public List<GuildMemberEntry> m_MembersSorted = new List<GuildMemberEntry>();

        public string m_MemberSearchText = "Player Name";
        public int m_MemberSearchIndex = 0;
        public int m_MemberPage = 0;
        public Guilds.MemberSortCriteria m_MemberSortCriteria = Guilds.MemberSortCriteria.LastOnline;
        public bool m_MemberSortAscending = false;
        public int m_MemberButtonIndexOffset = 100;

        public bool m_MemberDismissPlayerReady = false; 
        public GuildMemberEntry m_MemberPlayerToDismiss = null;

        //Member Management
        public GuildMemberEntry m_MemberManagementPlayer = null;
        public int m_MemberManagementNewRankIndex = 1;

        //Diplomacy
        public List<DiplomacyEntry> m_RelationshipsSorted = new List<DiplomacyEntry>();

        public string m_RelationshipSearchText = "Guild Name";
        public int m_RelationshipSearchIndex = 0;
        public int m_RelationshipPage = 0;
        public Guilds.RelationshipFilterType m_RelationshipFilterType = Guilds.RelationshipFilterType.ShowAvailableGuilds;
        public Guilds.RelationshipSortCriteria m_RelationshipSortCriteria = Guilds.RelationshipSortCriteria.GuildName;
        public bool m_RelationshipSortAscending = true;
        public int m_RelationshipButtonIndexOffset = 100;

        //Diplomacy Manage Relationship
        public Guilds.GuildRelationshipType m_DiplomacyManagementNewRelationshipType = Guilds.GuildRelationshipType.Neutral;
        public DiplomacyEntry m_DiplomacyManagementDiplomacyEntry = null;

        //Faction

        public GuildGumpObject()
        {
        }

        public void ClearData()
        {
            m_OverviewResignFromGuildReady = false;

            m_MemberDismissPlayerReady = false;
            m_MemberPlayerToDismiss = null;

            //TEST: FIX
            /*
            m_InvitationsSorted.Clear();
            m_MembersSorted.Clear();
            m_CandidatesSorted.Clear();
            m_RelationshipsSorted.Clear();
            */
        }
    }
}