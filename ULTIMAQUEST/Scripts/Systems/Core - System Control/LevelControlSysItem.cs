using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Regions;
using Server.Network;
using Server.Commands;
using System.Collections;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Items
{
	public class LevelControlSysItem : Item
	{
//		[Constructable]
		public LevelControlSysItem () : base( )  
		{
			Movable = true;
//			/* Setting Movable to True May cause Item to decay! */
			Name = "Level Control - Deleting will undo options chosen!.";
            Hue = 1922;
			LootType = LootType.Blessed;
			Visible = true;
			this.ItemID = 10922;
        }

		public override void OnDoubleClick(Mobile m)
		{
			if (m.AccessLevel == AccessLevel.Player)
				return;
			else
			{
				LevelControlSys xmleqiip = (LevelControlSys)XmlAttachExt.FindAttachment(this, typeof(LevelControlSys));
				if (xmleqiip == null)
				{
					XmlAttachExt.AttachTo(this, new LevelControlSys());
				}
				if (m.HasGump(typeof(LevelControlSysGump)))
				{
					m.CloseGump(typeof(LevelControlSysGump));
					m.SendGump(new LevelControlSysGump(m, this, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));

				}
				else
				{
					m.SendGump(new LevelControlSysGump(m, this, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
				}
			}
		}
		
        public override bool Decays 
		{
			get 
			{
				return false; 
				
			}
		}
        public override bool IsVirtualItem
		{
			get 
			{
				return true; 
			}
		}
		
		public virtual bool Accepted
        {
            get
            {
                return this.Deleted;
            }
        }
		
        public override bool DropToWorld(Mobile from, Point3D p)
        {
            bool ret = base.DropToWorld(from, p);
            if (ret && !this.Accepted && this.Parent != from.Backpack)
            {
                if (from.IsStaff())
                {
                    return true;
                }
                else
                {
                    from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "This can only be moved by a GM or Higher!");
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }
        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            bool ret = base.DropToMobile(from, target, p);
            if (ret && !this.Accepted && this.Parent != from.Backpack)
            {
                if (from.IsStaff())
                {
                    return true;
                }
                else
                {
                    from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "This can only be moved by a GM or Higher!");
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }
		
        public override bool OnDragLift(Mobile from)
        {
			if (from.IsStaff())
			{
				return true;
			}
			else
			{
				from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "This can only be moved by a GM or Higher!");
				return false;
			}
        }
		
        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            bool ret = base.DropToItem(from, target, p);
            if (ret && !this.Accepted && this.Parent != from.Backpack)
            {
                if (from.IsStaff())
                {
                    return true;
                }
                else
                {
                    from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "This can only be moved by a GM or Higher!");
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }
		
		public LevelControlSysItem ( Serial serial ) : base( serial )
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
	
	public class LevelControlSysGump : Gump
	{
        public static void Initialize()
        {
			CommandSystem.Register("levelcontrol", AccessLevel.Administrator, new CommandEventHandler(LevelControl_OnCommand));
        }
		
        private static void LevelControl_OnCommand(CommandEventArgs e)
        {
			Mobile from = e.Mobile;
			DateTime time = DateTime.Now;
			LevelControlSysItem loccontrol = null;
			
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			
			if (loccontrol != null)
			{				
				if (from.HasGump(typeof(LevelControlSysGump)))
				{
					from.CloseGump(typeof(LevelControlSysGump));
					from.SendGump(new LevelControlSysGump(from, loccontrol, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
					return;
				}
				else
				{
					from.SendGump(new LevelControlSysGump(from, loccontrol, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
					return;
				}
			}
			else
			{
				/*	by default the control item is created next to the lava pit in the
					green acres area in trammel. You can change that here if needed.	*/
					
				LevelControlSysItem controlitem = new LevelControlSysItem();
				controlitem.MoveToWorld(new Point3D(5427, 1081, 5), Map.Trammel);
				XmlAttachExt.AttachTo(controlitem, new LevelControlSys());
				from.SendGump(new LevelControlSysGump(from, controlitem, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
				return;
			}
        }
		
		LevelControlSys m_Itemxml;
		private Item m_Item;
        public GumpPage m_Page;
        public LevelCategory m_Cat;

        public enum GumpPage
        {
            None,
            SkillList
        }
		
        public enum LevelCategory
        {
			Main,
            PlayerLevels,
			PlayerLevels2,
			PlayerLevels3,
            ArmorLevels,
			WeaponLevels,
            CharacterLevels,
			JewelLevels, 
			DeletePlayerLevelItems,
			DeletePetlevelItems,
			LevelUpControlCalculations,
			LevelUpControlCalculations2,
			HideSkills,
			DynamicEquipmentLevels,
			ArmorReqAndIntensity,
			EquipReqAndIntensityHelp,
			WeaponReqAndIntensity,
			ClothingReqAndIntensity,
			JewelReqAndIntensity,
			PetLevels,
			PetLevels2,
			PetLevels3,
			PetSpecialAttacks,
			PetLevelReq,
			PetLevelReq2,
			PetLevelReq3,
			PetLevelSkillPoints,
			PetLevelStatPoints,
			PetStealing,
			Expansions,
			ConfiguredSkillsEXP,
			ConfiguredSkillsEXP2,
			ConfiguredSkillsEXP3,
			ConfiguredSkillsEXP4,
			LevelGainStatSP,
			LevelGainStatSP2,
			GainFollowers,
			GainFollowers2,
			StartupPlayerHandler,
			StartupPlayerHandler2,
			StartupPlayerHandler3,
			StartupPlayerHandler4,
			StartupPlayerHandler5,
			LevelBag,
			LevelBag2,
			LevelBag3,
			LevelBag4,
			LevelBag5,
			HelpPetStealing,
			HelpMountLevelCheck,
			HelpVendorDiscounts,
			HelpExpGainSkills,
			HelpGainPetSlotLvl,
			HelpPlayerStartUp,
			HelpLevelBag
			
        }
		
        private const int LabelHue = 0x480;
        private const int TitleHue = 0x12B;
		private const int LabelHue2 = 155;
		private const int LabelHue3 = 1153;

		
		public LevelControlSysGump ( Mobile from, Item controlitem, GumpPage page, LevelCategory cat ) : base( 40, 40 )
		{		
			m_Item = controlitem;
			m_Itemxml = (LevelControlSys)XmlAttachExt.FindAttachment(controlitem, typeof(LevelControlSys));

            page = m_Page;
            m_Cat = cat;
			
			Closable=true;
			Disposable=true;
			Dragable=true;
			Resizable=false;

			AddPage(0);
			AddBackground(50, 35, 540, 382, 9270);
			
			if (m_Itemxml == null)
				return;
			
			AddLabel( 200, 72, LabelHue3, "Level System Extreme Control Panel.");						
			AddLabel(262, 56, TitleHue, @"Admin Access");

			AddLabel(112, 93, TitleHue, @"System Options");
			AddLabel(265, 93, TitleHue, @"Features with (*) require Distro Edits!");
			AddImage(0, 4, 10440);
			AddImage(554, 4, 10441);
			
			AddLabel(112, 117, TitleHue, @"System Toggles");
			AddButton(75, 117, 4005, 4007, GetButtonID( 1, 0 ), GumpButtonType.Reply, 0);
			
			AddLabel(112, 139, TitleHue, @"Player Levels");
			AddButton(75, 139, 4005, 4007, GetButtonID( 1, 1 ), GumpButtonType.Reply, 0);
			
			AddLabel(112, 161, TitleHue, @"Pet Levels");
			AddButton(75, 161, 4005, 4007, GetButtonID( 1, 12 ), GumpButtonType.Reply, 0);

			AddLabel(112, 182, TitleHue, @"Expansions");
			AddButton(75, 182, 4005, 4007, GetButtonID( 1, 19 ), GumpButtonType.Reply, 0);

			
			if (m_Cat == LevelCategory.Main)
			{

				//Go To Control Item
				AddLabel(337, 117, LabelHue2, @"Teleport to Control Item");
				AddButton(300, 117, 4006, 4005, GetButtonID( 6, 7 ), GumpButtonType.Reply, 0);
				
				//Bring Control Item to GM
				AddLabel(337, 139, LabelHue2, @"Bring Control Item to GM");
				AddButton(300, 139, 4006, 4005, GetButtonID( 6, 6 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 161, LabelHue2, @"Toggle Player Leveling.");
				if (m_Itemxml.PlayerLevels == true)
					AddButton(300, 161, 4006, 4005, GetButtonID( 2, 1 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.PlayerLevels == false)
					AddButton(300, 161, 4005, 4006, GetButtonID( 2, 1 ), GumpButtonType.Reply, 0);
				AddButton(265, 161, 4017, 4019, GetButtonID( 1, 2 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 183, LabelHue2, @"Toggle Pet levels.");
				if (m_Itemxml.EnabledLevelPets == true)
					AddButton(300, 183, 4006, 4005, GetButtonID( 2, 0 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.EnabledLevelPets == false)
					AddButton(300, 183, 4005, 4006, GetButtonID( 2, 0 ), GumpButtonType.Reply, 0);
				AddButton(265, 183, 4017, 4019, GetButtonID( 1, 3 ), GumpButtonType.Reply, 0);
				
//				AddLabel(337, 205, LabelHue2, @"Toggle Equipment Level System.");
//				if (m_Itemxml.EquipMentLevelSystem == true)
//					AddButton(300, 205, 4006, 4005, GetButtonID( 2, 14 ), GumpButtonType.Reply, 0);
//				if (m_Itemxml.EquipMentLevelSystem == false)
//					AddButton(300, 205, 4005, 4006, GetButtonID( 2, 14 ), GumpButtonType.Reply, 0);
				
				AddLabel(75, 339, LabelHue2, @"Control Item should NOT be added manually, need to use [levelcontrol !");
				AddLabel(75, 359, LabelHue2, @"All Features Require level System to be active! ");
				AddLabel(75, 379, LabelHue2, @"Turn off level system, they will not work! Even Expansions!");

			}
			if (m_Cat == LevelCategory.PlayerLevels)
			{
				//StartMaxLvl
				AddLabel(337, 117, LabelHue2, @"Highest Level without scrolls.");
				AddTextEntry(265, 117, 30, 20, LabelHue3, 5 , m_Itemxml.StartMaxLvl.ToString());
				AddButton(300, 117, 4005, 4006, GetButtonID( 2, 6 ), GumpButtonType.Reply, 0);
				
				//EndMaxLvl
				AddLabel(337, 139, LabelHue2, @"Highest Level with scrolls.");
				AddTextEntry(265, 139, 30, 20, LabelHue3, 4 , m_Itemxml.EndMaxLvl.ToString());
				AddButton(300, 139, 4005, 4006, GetButtonID( 2, 2 ), GumpButtonType.Reply, 0);
				
				//SkillCoinCap
				AddLabel(337, 161, LabelHue2, @"Skill Coin Cap.");
				AddTextEntry(265, 161, 30, 20, LabelHue3, 6 , m_Itemxml.SkillCoinCap.ToString());
				AddButton(300, 161, 4005, 4006, GetButtonID( 2, 7 ), GumpButtonType.Reply, 0);

				//ExpCoinValue
				AddLabel(337, 183, LabelHue2, @"Exp Coin Value.");
				AddTextEntry(265, 183, 30, 20, LabelHue3, 7 , m_Itemxml.ExpCoinValue.ToString());
				AddButton(300, 183, 4005, 4006, GetButtonID( 2, 8 ), GumpButtonType.Reply, 0);

				//SkillCoinValue
				AddLabel(337, 205, LabelHue2, @"Skill Coin Value.");
				AddTextEntry(265, 205, 30, 20, LabelHue3, 982 , m_Itemxml.SkillCoinValue.ToString());
				AddButton(300, 205, 4005, 4006, GetButtonID( 2, 60 ), GumpButtonType.Reply, 0);
				
				//ExpPowerAmount
				AddLabel(337, 227, LabelHue2, @"Exp Power Level Amount.");
				AddTextEntry(265, 227, 30, 20, LabelHue3, 8 , m_Itemxml.ExpCoinValue.ToString());
				AddButton(300, 227, 4005, 4006, GetButtonID( 2, 10 ), GumpButtonType.Reply, 0);
				

				//Power Hour Time
				AddLabel(337, 249, LabelHue2, @"Power Hour Time in Minutes.");
				AddTextEntry(265, 249, 30, 20, LabelHue3, 9 , m_Itemxml.PowerHourTime.ToString());
				AddButton(300, 249, 4005, 4006, GetButtonID( 2, 15 ), GumpButtonType.Reply, 0);

				//Disable Skill Gain Mechanics
				AddLabel(337, 271, LabelHue2, @"Disable Skill Gain Mechanics. (*) ");
				if (m_Itemxml.DisableSkillGain == true)
					AddButton(300, 271, 4006, 4005, GetButtonID( 2, 16 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.DisableSkillGain == false)	
					AddButton(300, 271, 4005, 4006, GetButtonID( 2, 16 ), GumpButtonType.Reply, 0);	
				
				//Level Below Toon
				AddLabel(337, 293, LabelHue2, @"Level Below Toon. (*) ");
				if (m_Itemxml.LevelBelowToon == true)
					AddButton(300, 293, 4006, 4005, GetButtonID( 2, 17 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.LevelBelowToon == false)
					AddButton(300, 293, 4005, 4006, GetButtonID( 2, 17 ), GumpButtonType.Reply, 0);	
				
				//ShowPaperDollLevel
				AddLabel(337, 317, LabelHue2, @"Paper Doll Level. (*) ");
				if (m_Itemxml.ShowPaperDollLevel == true)
					AddButton(300, 317, 4006, 4005, GetButtonID( 2, 18 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.ShowPaperDollLevel == false)
					AddButton(300, 317, 4005, 4006, GetButtonID( 2, 18 ), GumpButtonType.Reply, 0);
				
				//PetKillGivesExp
				AddLabel(337, 339, LabelHue2, @"Pet kills give Exp.");
				if (m_Itemxml.PetKillGivesExp == true)
					AddButton(300, 339, 4006, 4005, GetButtonID( 2, 19 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.PetKillGivesExp == false)
					AddButton(300, 339, 4005, 4006, GetButtonID( 2, 19 ), GumpButtonType.Reply, 0);
				
				AddButton(450, 379, 4005, 4007, GetButtonID(3, 1), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
			}
			if (m_Cat == LevelCategory.PlayerLevels2)
			{
				AddLabel(337, 117, LabelHue2, @"Gain EXP From Bods. (*)");
				if (m_Itemxml.GainExpFromBods == true)
					AddButton(300, 117, 4006, 4005, GetButtonID( 2, 9 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainExpFromBods == false)
					AddButton(300, 117, 4005, 4006, GetButtonID( 2, 9 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 139, LabelHue2, @"Enable to use Tables for Exp.(*)");
				if (m_Itemxml.TablesAdvancedExp == true)
					AddButton(300, 139, 4006, 4005, GetButtonID( 2, 21 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.TablesAdvancedExp == false)
					AddButton(300, 139, 4005, 4006, GetButtonID( 2, 21 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 161, LabelHue2, @"Staff Has Levels Displayed.(*)");
				if (m_Itemxml.StaffHasLevel == true)
					AddButton(300, 161, 4006, 4005, GetButtonID( 2, 22 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.StaffHasLevel == false)
					AddButton(300, 161, 4005, 4006, GetButtonID( 2, 22 ), GumpButtonType.Reply, 0);		
				
				AddLabel(337, 183, LabelHue2, @"Chance of Bonus Status On Lv Gain.");
				if (m_Itemxml.BonusStatOnLvl == true)
					AddButton(300, 183, 4006, 4005, GetButtonID( 2, 23 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.BonusStatOnLvl == false)
					AddButton(300, 183, 4005, 4006, GetButtonID( 2, 23 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 205, LabelHue2, @"Vitals Refreshed On Level Gain?");
				if (m_Itemxml.RefreshOnLevel == true)
					AddButton(300, 205, 4006, 4005, GetButtonID( 2, 24 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.RefreshOnLevel == false)
					AddButton(300, 205, 4005, 4006, GetButtonID( 2, 24 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 227, LabelHue2, @"LevelSheet - Turn on to prevent drop.");
				if (m_Itemxml.LevelSheetPerma == true)
					AddButton(300, 227, 4006, 4005, GetButtonID( 2, 25 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.LevelSheetPerma == false)
					AddButton(300, 227, 4005, 4006, GetButtonID( 2, 25 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 249, LabelHue2, @"Display vendor levels.(*)");
				if (m_Itemxml.ShowVendorLevels == true)
					AddButton(300, 249, 4006, 4005, GetButtonID( 2, 26 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.ShowVendorLevels == false)
					AddButton(300, 249, 4005, 4006, GetButtonID( 2, 26 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 271, LabelHue2, @"Party EXP Sharing.");
				if (m_Itemxml.PartyExpShare == true)
					AddButton(300, 271, 4006, 4005, GetButtonID( 2, 27 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.PartyExpShare == false)
					AddButton(300, 271, 4005, 4006, GetButtonID( 2, 27 ), GumpButtonType.Reply, 0);	
				
				AddLabel(337	, 293, LabelHue2, @"How far away to gain Exp from party.");
				AddTextEntry(265, 293, 30, 20, LabelHue3, 980 , m_Itemxml.PartyExpShareRange.ToString());
				AddButton(300	, 293, 4005, 4006, GetButtonID( 2, 48 ), GumpButtonType.Reply, 0);

				
				AddLabel(337, 317, LabelHue2, @"Split Party Exp Evenly.");
				if (m_Itemxml.PartyExpShareSplit == true)
					AddButton(300, 317, 4006, 4005, GetButtonID( 2, 29 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.PartyExpShareSplit == false)
					AddButton(300, 317, 4005, 4006, GetButtonID( 2, 29 ), GumpButtonType.Reply, 0);	
				
				AddLabel(337, 339, LabelHue2, @"Level up Modifier Controls.");
				AddButton(300, 339, 4006, 4005, GetButtonID( 1, 4 ), GumpButtonType.Reply, 0);
				
				AddButton(450, 379, 4005, 4007, GetButtonID(3, 4), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
				
				AddButton(308, 379, 4005, 4007, GetButtonID(3, 0), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.PlayerLevels3)
			{
				AddLabel(337, 117, LabelHue2, @"Level Stat Reset Button.");
				if (m_Itemxml.LevelStatResetButton == true)
					AddButton(300, 117, 4006, 4005, GetButtonID( 2, 28 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.LevelStatResetButton == false)
					AddButton(300, 117, 4005, 4006, GetButtonID( 2, 28 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 139, LabelHue2, @"Level Skill Reset Button");
				if (m_Itemxml.LevelSkillResetButton == true)
					AddButton(300, 139, 4006, 4005, GetButtonID( 2, 47 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.LevelSkillResetButton == false)
					AddButton(300, 139, 4005, 4006, GetButtonID( 2, 47 ), GumpButtonType.Reply, 0);

				AddLabel(337, 161, LabelHue2, @"Toggle Skills In Level Menu");
				AddButton(300, 161, 4006, 4005, GetButtonID( 1, 5 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 183, LabelHue2, @"Skill Points Award Per Level");
				AddButton(300, 183, 4006, 4005, GetButtonID( 1, 25 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 205, LabelHue2, @"Stat Points Award Per Level");
				AddButton(300, 205, 4006, 4005, GetButtonID( 1, 26 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(3, 1), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.DeletePlayerLevelItems)
			{
				AddHtml( 325, 117, 177, 190, String.Format( "<basefont color = #FFFFFF><center>Click the button below to delete all player level sheets.  There is no undo button, unless you have backup of the saves files this is a permanent action! There are <i>NO</i> additional Pop ups!</center></basefont>"), false, false );
				AddButton(385, 280, 2642, 2643, GetButtonID( 2, 11 ), GumpButtonType.Reply, 0);
			}
			if (m_Cat == LevelCategory.DeletePetlevelItems)
			{
				AddHtml( 325, 117, 177, 190, String.Format( "<basefont color = #FFFFFF><center>Click the button below to delete all pet level orbs.  There is no undo button, unless you have backup of the saves files this is a permanent action! There are <i>NO</i> additional Pop ups!</center></basefont>"), false, false );
				AddButton(385, 280, 2642, 2643, GetButtonID( 2, 13 ), GumpButtonType.Reply, 0);
			}
			if (m_Cat == LevelCategory.LevelUpControlCalculations)
			{
				AddLabel(337	, 117, LabelHue2, @"L2 to 20 Multipier.");
				AddTextEntry(265, 117, 30, 20, LabelHue3, 10 , m_Itemxml.L2to20Multipier.ToString());
				AddButton(300	, 117, 4005, 4006, GetButtonID( 2, 30 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 139, LabelHue2, @"L21 to 40 Multiplier.");
				AddTextEntry(265, 139, 30, 20, LabelHue3, 11 , m_Itemxml.L21to40Multiplier.ToString());
				AddButton(300	, 139, 4005, 4006, GetButtonID( 2, 31 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 161, LabelHue2, @"L41 to 60 Multiplier.");
				AddTextEntry(265, 161, 30, 20, LabelHue3, 12 , m_Itemxml.L41to60Multiplier.ToString());
				AddButton(300	, 161, 4005, 4006, GetButtonID( 2, 32 ), GumpButtonType.Reply, 0);	
				
				AddLabel(337	, 183, LabelHue2, @"L61 to 70 Multiplier.");
				AddTextEntry(265, 183, 30, 20, LabelHue3, 13 , m_Itemxml.L61to70Multiplier.ToString());
				AddButton(300	, 183, 4005, 4006, GetButtonID( 2, 33 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 205, LabelHue2, @"L71 to 80 Multiplier.");
				AddTextEntry(265, 205, 30, 20, LabelHue3, 14 , m_Itemxml.L71to80Multiplier.ToString());
				AddButton(300	, 205, 4005, 4006, GetButtonID( 2, 34 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 227, LabelHue2, @"L81 to 90 Multipier.");
				AddTextEntry(265, 227, 30, 20, LabelHue3, 15 , m_Itemxml.L81to90Multipier.ToString());
				AddButton(300	, 227, 4005, 4006, GetButtonID( 2, 35 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 249, LabelHue2, @"L91 to 100 Multipier.");
				AddTextEntry(265, 249, 30, 20, LabelHue3, 16 , m_Itemxml.L91to100Multipier.ToString());
				AddButton(300	, 249, 4005, 4006, GetButtonID( 2, 36 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 271, LabelHue2, @"L101 to 110 Multiplier.");
				AddTextEntry(265, 271, 30, 20, LabelHue3, 17 , m_Itemxml.L101to110Multiplier.ToString());
				AddButton(300	, 271, 4005, 4006, GetButtonID( 2, 37 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 293, LabelHue2, @"L111 to 120 Multiplier.");
				AddTextEntry(265, 293, 30, 20, LabelHue3, 18 , m_Itemxml.L111to120Multiplier.ToString());
				AddButton(300	, 293, 4005, 4006, GetButtonID( 2, 38 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 317, LabelHue2, @"L121 to 130 Multiplier.");
				AddTextEntry(265, 317, 30, 20, LabelHue3, 19 , m_Itemxml.L121to130Multiplier.ToString());
				AddButton(300	, 317, 4005, 4006, GetButtonID( 2, 39 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 339, LabelHue2, @"L131 to 140 Multiplier.");
				AddTextEntry(265, 339, 30, 20, LabelHue3, 20 , m_Itemxml.L131to140Multiplier.ToString());
				AddButton(300	, 339, 4005, 4006, GetButtonID( 2, 40 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(3, 1), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
				
				AddButton(450, 379, 4005, 4007, GetButtonID(3, 2), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
			}
			if (m_Cat == LevelCategory.LevelUpControlCalculations2)
			{
				AddLabel(337	, 117, LabelHue2, @"L141 to 150 Multiplier.");
				AddTextEntry(265, 117, 30, 20, LabelHue3, 21 , m_Itemxml.L141to150Multiplier.ToString());
				AddButton(300	, 117, 4005, 4006, GetButtonID( 2, 41 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 139, LabelHue2, @"L151 to 161 Multiplier.");
				AddTextEntry(265, 139, 30, 20, LabelHue3, 22 , m_Itemxml.L151to160Multiplier.ToString());
				AddButton(300	, 139, 4005, 4006, GetButtonID( 2, 42 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 161, LabelHue2, @"L161 to 170 Multiplier.");
				AddTextEntry(265, 161, 30, 20, LabelHue3, 23 , m_Itemxml.L161to170Multiplier.ToString());
				AddButton(300	, 161, 4005, 4006, GetButtonID( 2, 43 ), GumpButtonType.Reply, 0);	
				
				AddLabel(337	, 183, LabelHue2, @"L171 to 180 Multiplier.");
				AddTextEntry(265, 183, 30, 20, LabelHue3, 24 , m_Itemxml.L171to180Multiplier.ToString());
				AddButton(300	, 183, 4005, 4006, GetButtonID( 2, 44 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 205, LabelHue2, @"L181 to 190 Multiplier.");
				AddTextEntry(265, 205, 30, 20, LabelHue3, 25 , m_Itemxml.L181to190Multiplier.ToString());
				AddButton(300	, 205, 4005, 4006, GetButtonID( 2, 45 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 227, LabelHue2, @"L191 to 200 Multiplier.");
				AddTextEntry(265, 227, 30, 20, LabelHue3, 26 , m_Itemxml.L191to200Multiplier.ToString());
				AddButton(300	, 227, 4005, 4006, GetButtonID( 2, 46 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(3, 3), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.HideSkills)
			{
				AddLabel(337, 117, LabelHue2, @"Toggle Miscelaneous Category");
				if (m_Itemxml.Miscelaneous == true)
					AddButton(300, 117, 4006, 4005, GetButtonID( 2, 49 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Miscelaneous == false)
					AddButton(300, 117, 4005, 4006, GetButtonID( 2, 49 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 139, LabelHue2, @"Toggle Combat Category");
				if (m_Itemxml.Combat == true)
					AddButton(300, 139, 4006, 4005, GetButtonID( 2, 50 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Combat == false)
					AddButton(300, 139, 4005, 4006, GetButtonID( 2, 50 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 161, LabelHue2, @"Toggle Tradeskills Category");
				if (m_Itemxml.Tradeskills == true)
					AddButton(300, 161, 4006, 4005, GetButtonID( 2, 51 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Tradeskills == false)
					AddButton(300, 161, 4005, 4006, GetButtonID( 2, 51 ), GumpButtonType.Reply, 0);		
				
				AddLabel(337, 183, LabelHue2, @"Toggle Magic Category");
				if (m_Itemxml.Magic == true)
					AddButton(300, 183, 4006, 4005, GetButtonID( 2, 52 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Magic == false)
					AddButton(300, 183, 4005, 4006, GetButtonID( 2, 52 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 205, LabelHue2, @"Toggle Wilderness Category");
				if (m_Itemxml.Wilderness == true)
					AddButton(300, 205, 4006, 4005, GetButtonID( 2, 53 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Wilderness == false)
					AddButton(300, 205, 4005, 4006, GetButtonID( 2, 53 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 227, LabelHue2, @"Toggle Thieving Category");
				if (m_Itemxml.Thieving == true)
					AddButton(300, 227, 4006, 4005, GetButtonID( 2, 54 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Thieving == false)
					AddButton(300, 227, 4005, 4006, GetButtonID( 2, 54 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 249, LabelHue2, @"Toggle Bard Category");
				if (m_Itemxml.Bard == true)
					AddButton(300, 249, 4006, 4005, GetButtonID( 2, 55 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Bard == false)
					AddButton(300, 249, 4005, 4006, GetButtonID( 2, 55 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 271, LabelHue2, @"Toggle Imbuing Skill");
				if (m_Itemxml.Imbuing == true)
					AddButton(300, 271, 4006, 4005, GetButtonID( 2, 56 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Imbuing == false)
					AddButton(300, 271, 4005, 4006, GetButtonID( 2, 56 ), GumpButtonType.Reply, 0);	
				
				AddLabel(337, 293, LabelHue2, @"Toggle Throwing Skill");
				if (m_Itemxml.Throwing == true)
					AddButton(300, 293, 4006, 4005, GetButtonID( 2, 57 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Throwing == false)
					AddButton(300, 293, 4005, 4006, GetButtonID( 2, 57 ), GumpButtonType.Reply, 0);	

				AddLabel(337, 317, LabelHue2, @"Toggle Mysticism Skill");
				if (m_Itemxml.Mysticism == true)
					AddButton(300, 317, 4006, 4005, GetButtonID( 2, 58 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Mysticism == false)
					AddButton(300, 317, 4005, 4006, GetButtonID( 2, 58 ), GumpButtonType.Reply, 0);	
				
				AddLabel(337, 339, LabelHue2, @"Toggle Spellweaving Skill");
				if (m_Itemxml.Spellweaving == true)
					AddButton(300, 339, 4006, 4005, GetButtonID( 2, 59 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Spellweaving == false)
					AddButton(300, 339, 4005, 4006, GetButtonID( 2, 59 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(3, 4), GumpButtonType.Reply, 0);
				AddLabel(345, 379, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.DynamicEquipmentLevels)
			{
				AddLabel(337, 117, LabelHue2, @"Toggle Dynamic Equpment levels.");
				if (m_Itemxml.ActivateDynamicLevelSystem == true)
					AddButton(300, 117, 4006, 4005, GetButtonID( 2, 3 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.ActivateDynamicLevelSystem == false)
					AddButton(300, 117, 4005, 4006, GetButtonID( 2, 3 ), GumpButtonType.Reply, 0);
		
				AddLabel(337, 139, LabelHue2, @"Armor Level Requirements & Intensity");
				AddButton(300, 139, 4006, 4005, GetButtonID( 1, 7 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 161, LabelHue2, @"Weapon Level Requirements & Intensity");
				AddButton(300, 161, 4006, 4005, GetButtonID( 1, 9 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 183, LabelHue2, @"Clothing Level Requirements & Intensity");
				AddButton(300, 183, 4006, 4005, GetButtonID( 1, 10 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 205, LabelHue2, @"Jewelry Level Requirements & Intensity");
				AddButton(300, 205, 4006, 4005, GetButtonID( 1, 11 ), GumpButtonType.Reply, 0);
				
				//EquipRequiredLevel0
				AddLabel(337, 227, LabelHue2, @"Default LVL - Best left as Default 1.");
				AddTextEntry(265, 227, 30, 20, LabelHue3, 107 , m_Itemxml.EquipRequiredLevel0.ToString());
				AddButton(300, 227, 4005, 4006, GetButtonID( 4, 80 ), GumpButtonType.Reply, 0);
				
				//NameOfBattleRatingStat
				AddLabel(337, 249, LabelHue2, @"Rating Name: ");
				AddTextEntry(430, 249, 150, 20, LabelHue3, 109 , m_Itemxml.NameOfBattleRatingStat.ToString());
				AddButton(300, 249, 4005, 4006, GetButtonID( 4, 82 ), GumpButtonType.Reply, 0);
				
				//RequiredLevelMouseOver
				AddLabel(337, 271, LabelHue2, @"Name of Level: ");
				AddTextEntry(430, 271, 150, 20, LabelHue3, 108 , m_Itemxml.RequiredLevelMouseOver.ToString());
				AddButton(300, 271, 4005, 4006, GetButtonID( 4, 81 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 339, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(345, 339, LabelHue2, @"Previous Page");

				AddLabel(75, 379, LabelHue2, @"This feature is not compatible with the equipment level system!");
				AddLabel(75, 359, LabelHue2, @"You can have levels and rating automatic or by equipment leveling!");
			}
			if (m_Cat == LevelCategory.ArmorReqAndIntensity)
			{
				AddLabel(280, 117, LabelHue2, @"Level Requirement");
				AddLabel(425, 117, LabelHue2, @"Equipment Intensity");

				AddLabel(317	, 139, LabelHue2, @"Group 1:");
				AddTextEntry(380, 139, 30, 20, LabelHue3, 27 , m_Itemxml.ArmorRequiredLevel1.ToString());
				AddButton(280	, 139, 4005, 4006, GetButtonID( 4, 0 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 161, LabelHue2, @"Group 2:");
				AddTextEntry(380, 161, 30, 20, LabelHue3, 28 , m_Itemxml.ArmorRequiredLevel2.ToString());
				AddButton(280	, 161, 4005, 4006, GetButtonID( 4, 1 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 183, LabelHue2, @"Group 3:");
				AddTextEntry(380, 183, 30, 20, LabelHue3, 29 , m_Itemxml.ArmorRequiredLevel3.ToString());
				AddButton(280	, 183, 4005, 4006, GetButtonID( 4, 2 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 205, LabelHue2, @"Group 4:");
				AddTextEntry(380, 205, 30, 20, LabelHue3, 30 , m_Itemxml.ArmorRequiredLevel4.ToString());
				AddButton(280	, 205, 4005, 4006, GetButtonID( 4, 3 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 227, LabelHue2, @"Group 5:");
				AddTextEntry(380, 227, 30, 20, LabelHue3, 31 , m_Itemxml.ArmorRequiredLevel5.ToString());
				AddButton(280	, 227, 4005, 4006, GetButtonID( 4, 4 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 249, LabelHue2, @"Group 6:");
				AddTextEntry(380, 249, 30, 20, LabelHue3, 32 , m_Itemxml.ArmorRequiredLevel6.ToString());
				AddButton(280	, 249, 4005, 4006, GetButtonID( 4, 5 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 271, LabelHue2, @"Group 7:");
				AddTextEntry(380, 271, 30, 20, LabelHue3, 33 , m_Itemxml.ArmorRequiredLevel7.ToString());
				AddButton(280	, 271, 4005, 4006, GetButtonID( 4, 6 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 293, LabelHue2, @"Group 8:");
				AddTextEntry(380, 293, 30, 20, LabelHue3, 34 , m_Itemxml.ArmorRequiredLevel8.ToString());
				AddButton(280	, 293, 4005, 4006, GetButtonID( 4, 7 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 317, LabelHue2, @"Group 9:");
				AddTextEntry(380, 317, 30, 20, LabelHue3, 35 , m_Itemxml.ArmorRequiredLevel9.ToString());
				AddButton(280	, 317, 4005, 4006, GetButtonID( 4, 8 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 339, LabelHue2, @"Group 10:");
				AddTextEntry(380, 339, 30, 20, LabelHue3, 36 , m_Itemxml.ArmorRequiredLevel10.ToString());
				AddButton(280	, 339, 4005, 4006, GetButtonID( 4, 9 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 139, LabelHue2, @"Group 1:");
				AddTextEntry(526, 139, 30, 20, LabelHue3, 37 , m_Itemxml.ArmorRequiredLevel1Intensity.ToString());
				AddButton(426	, 139, 4005, 4006, GetButtonID( 4, 10 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 161, LabelHue2, @"Group 2:");
				AddTextEntry(526, 161, 30, 20, LabelHue3, 38 , m_Itemxml.ArmorRequiredLevel2Intensity.ToString());
				AddButton(426	, 161, 4005, 4006, GetButtonID( 4, 11 ), GumpButtonType.Reply, 0);	
				
				AddLabel(463	, 183, LabelHue2, @"Group 3:");
				AddTextEntry(526, 183, 30, 20, LabelHue3, 39 , m_Itemxml.ArmorRequiredLevel3Intensity.ToString());
				AddButton(426	, 183, 4005, 4006, GetButtonID( 4, 12 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 205, LabelHue2, @"Group 4:");
				AddTextEntry(526, 205, 30, 20, LabelHue3, 40 , m_Itemxml.ArmorRequiredLevel4Intensity.ToString());
				AddButton(426	, 205, 4005, 4006, GetButtonID( 4, 13 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 227, LabelHue2, @"Group 5:");
				AddTextEntry(526, 227, 30, 20, LabelHue3, 41 , m_Itemxml.ArmorRequiredLevel5Intensity.ToString());
				AddButton(426	, 227, 4005, 4006, GetButtonID( 4, 14 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 249, LabelHue2, @"Group 6:");
				AddTextEntry(526, 249, 30, 20, LabelHue3, 42 , m_Itemxml.ArmorRequiredLevel6Intensity.ToString());
				AddButton(426	, 249, 4005, 4006, GetButtonID( 4, 15 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 271, LabelHue2, @"Group 7:");
				AddTextEntry(526, 271, 30, 20, LabelHue3, 43 , m_Itemxml.ArmorRequiredLevel7Intensity.ToString());
				AddButton(426	, 271, 4005, 4006, GetButtonID( 4, 16 ), GumpButtonType.Reply, 0);	
				
				AddLabel(463	, 293, LabelHue2, @"Group 8:");
				AddTextEntry(526, 293, 30, 20, LabelHue3, 44 , m_Itemxml.ArmorRequiredLevel8Intensity.ToString());
				AddButton(426	, 293, 4005, 4006, GetButtonID( 4, 17 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 317, LabelHue2, @"Group 9:");
				AddTextEntry(526, 317, 30, 20, LabelHue3, 45 , m_Itemxml.ArmorRequiredLevel9Intensity.ToString());
				AddButton(426	, 317, 4005, 4006, GetButtonID( 4, 18 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 339, LabelHue2, @"Group 10:");
				AddTextEntry(526, 339, 30, 20, LabelHue3, 46 , m_Itemxml.ArmorRequiredLevel10Intensity.ToString());
				AddButton(426	, 339, 4005, 4006, GetButtonID( 4, 19 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 6), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.WeaponReqAndIntensity)
			{
				AddLabel(280, 117, LabelHue2, @"Level Requirement");
				AddLabel(425, 117, LabelHue2, @"Equipment Intensity");

				AddLabel(317	, 139, LabelHue2, @"Group 1:");
				AddTextEntry(380, 139, 30, 20, LabelHue3, 47 , m_Itemxml.WeaponRequiredLevel1.ToString());
				AddButton(280	, 139, 4005, 4006, GetButtonID( 4, 20 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 161, LabelHue2, @"Group 2:");
				AddTextEntry(380, 161, 30, 20, LabelHue3, 48 , m_Itemxml.WeaponRequiredLevel2.ToString());
				AddButton(280	, 161, 4005, 4006, GetButtonID( 4, 21 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 183, LabelHue2, @"Group 3:");
				AddTextEntry(380, 183, 30, 20, LabelHue3, 49 , m_Itemxml.WeaponRequiredLevel3.ToString());
				AddButton(280	, 183, 4005, 4006, GetButtonID( 4, 22 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 205, LabelHue2, @"Group 4:");
				AddTextEntry(380, 205, 30, 20, LabelHue3, 50 , m_Itemxml.WeaponRequiredLevel4.ToString());
				AddButton(280	, 205, 4005, 4006, GetButtonID( 4, 23 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 227, LabelHue2, @"Group 5:");
				AddTextEntry(380, 227, 30, 20, LabelHue3, 51 , m_Itemxml.WeaponRequiredLevel5.ToString());
				AddButton(280	, 227, 4005, 4006, GetButtonID( 4, 24 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 249, LabelHue2, @"Group 6:");
				AddTextEntry(380, 249, 30, 20, LabelHue3, 52 , m_Itemxml.WeaponRequiredLevel6.ToString());
				AddButton(280	, 249, 4005, 4006, GetButtonID( 4, 25 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 271, LabelHue2, @"Group 7:");
				AddTextEntry(380, 271, 30, 20, LabelHue3, 53 , m_Itemxml.WeaponRequiredLevel7.ToString());
				AddButton(280	, 271, 4005, 4006, GetButtonID( 4, 26 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 293, LabelHue2, @"Group 8:");
				AddTextEntry(380, 293, 30, 20, LabelHue3, 54 , m_Itemxml.WeaponRequiredLevel8.ToString());
				AddButton(280	, 293, 4005, 4006, GetButtonID( 4, 27 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 317, LabelHue2, @"Group 9:");
				AddTextEntry(380, 317, 30, 20, LabelHue3, 55 , m_Itemxml.WeaponRequiredLevel9.ToString());
				AddButton(280	, 317, 4005, 4006, GetButtonID( 4, 28 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 339, LabelHue2, @"Group 10:");
				AddTextEntry(380, 339, 30, 20, LabelHue3, 56 , m_Itemxml.WeaponRequiredLevel10.ToString());
				AddButton(280	, 339, 4005, 4006, GetButtonID( 4, 29 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 139, LabelHue2, @"Group 1:");
				AddTextEntry(526, 139, 30, 20, LabelHue3, 57 , m_Itemxml.WeaponRequiredLevel1Intensity.ToString());
				AddButton(426	, 139, 4005, 4006, GetButtonID( 4, 30 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 161, LabelHue2, @"Group 2:");
				AddTextEntry(526, 161, 30, 20, LabelHue3, 58 , m_Itemxml.WeaponRequiredLevel2Intensity.ToString());
				AddButton(426	, 161, 4005, 4006, GetButtonID( 4, 31 ), GumpButtonType.Reply, 0);	
				
				AddLabel(463	, 183, LabelHue2, @"Group 3:");
				AddTextEntry(526, 183, 30, 20, LabelHue3, 59 , m_Itemxml.WeaponRequiredLevel3Intensity.ToString());
				AddButton(426	, 183, 4005, 4006, GetButtonID( 4, 32 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 205, LabelHue2, @"Group 4:");
				AddTextEntry(526, 205, 30, 20, LabelHue3, 60 , m_Itemxml.WeaponRequiredLevel4Intensity.ToString());
				AddButton(426	, 205, 4005, 4006, GetButtonID( 4, 33 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 227, LabelHue2, @"Group 5:");
				AddTextEntry(526, 227, 30, 20, LabelHue3, 61 , m_Itemxml.WeaponRequiredLevel5Intensity.ToString());
				AddButton(426	, 227, 4005, 4006, GetButtonID( 4, 34 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 249, LabelHue2, @"Group 6:");
				AddTextEntry(526, 249, 30, 20, LabelHue3, 62 , m_Itemxml.WeaponRequiredLevel6Intensity.ToString());
				AddButton(426	, 249, 4005, 4006, GetButtonID( 4, 35 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 271, LabelHue2, @"Group 7:");
				AddTextEntry(526, 271, 30, 20, LabelHue3, 63 , m_Itemxml.WeaponRequiredLevel7Intensity.ToString());
				AddButton(426	, 271, 4005, 4006, GetButtonID( 4, 36 ), GumpButtonType.Reply, 0);	
				
				AddLabel(463	, 293, LabelHue2, @"Group 8:");
				AddTextEntry(526, 293, 30, 20, LabelHue3, 64 , m_Itemxml.WeaponRequiredLevel8Intensity.ToString());
				AddButton(426	, 293, 4005, 4006, GetButtonID( 4, 37 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 317, LabelHue2, @"Group 9:");
				AddTextEntry(526, 317, 30, 20, LabelHue3, 65 , m_Itemxml.WeaponRequiredLevel9Intensity.ToString());
				AddButton(426	, 317, 4005, 4006, GetButtonID( 4, 38 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 339, LabelHue2, @"Group 10:");
				AddTextEntry(526, 339, 30, 20, LabelHue3, 66 , m_Itemxml.WeaponRequiredLevel10Intensity.ToString());
				AddButton(426	, 339, 4005, 4006, GetButtonID( 4, 39 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 6), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.ClothingReqAndIntensity)
			{
				AddLabel(280, 117, LabelHue2, @"Level Requirement");
				AddLabel(425, 117, LabelHue2, @"Equipment Intensity");

				AddLabel(317	, 139, LabelHue2, @"Group 1:");
				AddTextEntry(380, 139, 30, 20, LabelHue3, 67 , m_Itemxml.ClothRequiredLevel1.ToString());
				AddButton(280	, 139, 4005, 4006, GetButtonID( 4, 40 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 161, LabelHue2, @"Group 2:");
				AddTextEntry(380, 161, 30, 20, LabelHue3, 68 , m_Itemxml.ClothRequiredLevel2.ToString());
				AddButton(280	, 161, 4005, 4006, GetButtonID( 4, 41 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 183, LabelHue2, @"Group 3:");
				AddTextEntry(380, 183, 30, 20, LabelHue3, 69 , m_Itemxml.ClothRequiredLevel3.ToString());
				AddButton(280	, 183, 4005, 4006, GetButtonID( 4, 42 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 205, LabelHue2, @"Group 4:");
				AddTextEntry(380, 205, 30, 20, LabelHue3, 70 , m_Itemxml.ClothRequiredLevel4.ToString());
				AddButton(280	, 205, 4005, 4006, GetButtonID( 4, 43 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 227, LabelHue2, @"Group 5:");
				AddTextEntry(380, 227, 30, 20, LabelHue3, 71 , m_Itemxml.ClothRequiredLevel5.ToString());
				AddButton(280	, 227, 4005, 4006, GetButtonID( 4, 44 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 249, LabelHue2, @"Group 6:");
				AddTextEntry(380, 249, 30, 20, LabelHue3, 72 , m_Itemxml.ClothRequiredLevel6.ToString());
				AddButton(280	, 249, 4005, 4006, GetButtonID( 4, 45 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 271, LabelHue2, @"Group 7:");
				AddTextEntry(380, 271, 30, 20, LabelHue3, 73 , m_Itemxml.ClothRequiredLevel7.ToString());
				AddButton(280	, 271, 4005, 4006, GetButtonID( 4, 46 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 293, LabelHue2, @"Group 8:");
				AddTextEntry(380, 293, 30, 20, LabelHue3, 74 , m_Itemxml.ClothRequiredLevel8.ToString());
				AddButton(280	, 293, 4005, 4006, GetButtonID( 4, 47 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 317, LabelHue2, @"Group 9:");
				AddTextEntry(380, 317, 30, 20, LabelHue3, 75 , m_Itemxml.ClothRequiredLevel9.ToString());
				AddButton(280	, 317, 4005, 4006, GetButtonID( 4, 48 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 339, LabelHue2, @"Group 10:");
				AddTextEntry(380, 339, 30, 20, LabelHue3, 76 , m_Itemxml.ClothRequiredLevel10.ToString());
				AddButton(280	, 339, 4005, 4006, GetButtonID( 4, 49 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 139, LabelHue2, @"Group 1:");
				AddTextEntry(526, 139, 30, 20, LabelHue3, 77 , m_Itemxml.ClothRequiredLevel1Intensity.ToString());
				AddButton(426	, 139, 4005, 4006, GetButtonID( 4, 50 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 161, LabelHue2, @"Group 2:");
				AddTextEntry(526, 161, 30, 20, LabelHue3, 78 , m_Itemxml.ClothRequiredLevel2Intensity.ToString());
				AddButton(426	, 161, 4005, 4006, GetButtonID( 4, 51 ), GumpButtonType.Reply, 0);	
				
				AddLabel(463	, 183, LabelHue2, @"Group 3:");
				AddTextEntry(526, 183, 30, 20, LabelHue3, 79 , m_Itemxml.ClothRequiredLevel3Intensity.ToString());
				AddButton(426	, 183, 4005, 4006, GetButtonID( 4, 52 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 205, LabelHue2, @"Group 4:");
				AddTextEntry(526, 205, 30, 20, LabelHue3, 80 , m_Itemxml.ClothRequiredLevel4Intensity.ToString());
				AddButton(426	, 205, 4005, 4006, GetButtonID( 4, 53 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 227, LabelHue2, @"Group 5:");
				AddTextEntry(526, 227, 30, 20, LabelHue3, 81 , m_Itemxml.ClothRequiredLevel5Intensity.ToString());
				AddButton(426	, 227, 4005, 4006, GetButtonID( 4, 54 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 249, LabelHue2, @"Group 6:");
				AddTextEntry(526, 249, 30, 20, LabelHue3, 82 , m_Itemxml.ClothRequiredLevel6Intensity.ToString());
				AddButton(426	, 249, 4005, 4006, GetButtonID( 4, 55 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 271, LabelHue2, @"Group 7:");
				AddTextEntry(526, 271, 30, 20, LabelHue3, 83 , m_Itemxml.ClothRequiredLevel7Intensity.ToString());
				AddButton(426	, 271, 4005, 4006, GetButtonID( 4, 56 ), GumpButtonType.Reply, 0);	
				
				AddLabel(463	, 293, LabelHue2, @"Group 8:");
				AddTextEntry(526, 293, 30, 20, LabelHue3, 84 , m_Itemxml.ClothRequiredLevel8Intensity.ToString());
				AddButton(426	, 293, 4005, 4006, GetButtonID( 4, 57 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 317, LabelHue2, @"Group 9:");
				AddTextEntry(526, 317, 30, 20, LabelHue3, 85 , m_Itemxml.ClothRequiredLevel9Intensity.ToString());
				AddButton(426	, 317, 4005, 4006, GetButtonID( 4, 58 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 339, LabelHue2, @"Group 10:");
				AddTextEntry(526, 339, 30, 20, LabelHue3, 86 , m_Itemxml.ClothRequiredLevel10Intensity.ToString());
				AddButton(426	, 339, 4005, 4006, GetButtonID( 4, 59 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 6), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.JewelReqAndIntensity)
			{
				AddLabel(280, 117, LabelHue2, @"Level Requirement");
				AddLabel(425, 117, LabelHue2, @"Equipment Intensity");

				AddLabel(317	, 139, LabelHue2, @"Group 1:");
				AddTextEntry(380, 139, 30, 20, LabelHue3, 87 , m_Itemxml.JewelRequiredLevel1.ToString());
				AddButton(280	, 139, 4005, 4006, GetButtonID( 4, 60 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 161, LabelHue2, @"Group 2:");
				AddTextEntry(380, 161, 30, 20, LabelHue3, 88 , m_Itemxml.JewelRequiredLevel2.ToString());
				AddButton(280	, 161, 4005, 4006, GetButtonID( 4, 61 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 183, LabelHue2, @"Group 3:");
				AddTextEntry(380, 183, 30, 20, LabelHue3, 89 , m_Itemxml.JewelRequiredLevel3.ToString());
				AddButton(280	, 183, 4005, 4006, GetButtonID( 4, 62 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 205, LabelHue2, @"Group 4:");
				AddTextEntry(380, 205, 30, 20, LabelHue3, 90 , m_Itemxml.JewelRequiredLevel4.ToString());
				AddButton(280	, 205, 4005, 4006, GetButtonID( 4, 63 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 227, LabelHue2, @"Group 5:");
				AddTextEntry(380, 227, 30, 20, LabelHue3, 91 , m_Itemxml.JewelRequiredLevel5.ToString());
				AddButton(280	, 227, 4005, 4006, GetButtonID( 4, 64 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 249, LabelHue2, @"Group 6:");
				AddTextEntry(380, 249, 30, 20, LabelHue3, 92 , m_Itemxml.JewelRequiredLevel6.ToString());
				AddButton(280	, 249, 4005, 4006, GetButtonID( 4, 65 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 271, LabelHue2, @"Group 7:");
				AddTextEntry(380, 271, 30, 20, LabelHue3, 93 , m_Itemxml.JewelRequiredLevel7.ToString());
				AddButton(280	, 271, 4005, 4006, GetButtonID( 4, 66 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 293, LabelHue2, @"Group 8:");
				AddTextEntry(380, 293, 30, 20, LabelHue3, 94 , m_Itemxml.JewelRequiredLevel8.ToString());
				AddButton(280	, 293, 4005, 4006, GetButtonID( 4, 67 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 317, LabelHue2, @"Group 9:");
				AddTextEntry(380, 317, 30, 20, LabelHue3, 95 , m_Itemxml.JewelRequiredLevel9.ToString());
				AddButton(280	, 317, 4005, 4006, GetButtonID( 4, 68 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 339, LabelHue2, @"Group 10:");
				AddTextEntry(380, 339, 30, 20, LabelHue3, 96 , m_Itemxml.JewelRequiredLevel10.ToString());
				AddButton(280	, 339, 4005, 4006, GetButtonID( 4, 69 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 139, LabelHue2, @"Group 1:");
				AddTextEntry(526, 139, 30, 20, LabelHue3, 97 , m_Itemxml.JewelRequiredLevel1Intensity.ToString());
				AddButton(426	, 139, 4005, 4006, GetButtonID( 4, 70 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 161, LabelHue2, @"Group 2:");
				AddTextEntry(526, 161, 30, 20, LabelHue3, 98 , m_Itemxml.JewelRequiredLevel2Intensity.ToString());
				AddButton(426	, 161, 4005, 4006, GetButtonID( 4, 71 ), GumpButtonType.Reply, 0);	
				
				AddLabel(463	, 183, LabelHue2, @"Group 3:");
				AddTextEntry(526, 183, 30, 20, LabelHue3, 99 , m_Itemxml.JewelRequiredLevel3Intensity.ToString());
				AddButton(426	, 183, 4005, 4006, GetButtonID( 4, 72 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 205, LabelHue2, @"Group 4:");
				AddTextEntry(526, 205, 30, 20, LabelHue3, 100 , m_Itemxml.JewelRequiredLevel4Intensity.ToString());
				AddButton(426	, 205, 4005, 4006, GetButtonID( 4, 73 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 227, LabelHue2, @"Group 5:");
				AddTextEntry(526, 227, 30, 20, LabelHue3, 101 , m_Itemxml.JewelRequiredLevel5Intensity.ToString());
				AddButton(426	, 227, 4005, 4006, GetButtonID( 4, 74 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 249, LabelHue2, @"Group 6:");
				AddTextEntry(526, 249, 30, 20, LabelHue3, 102 , m_Itemxml.JewelRequiredLevel6Intensity.ToString());
				AddButton(426	, 249, 4005, 4006, GetButtonID( 4, 75 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 271, LabelHue2, @"Group 7:");
				AddTextEntry(526, 271, 30, 20, LabelHue3, 103 , m_Itemxml.JewelRequiredLevel7Intensity.ToString());
				AddButton(426	, 271, 4005, 4006, GetButtonID( 4, 76 ), GumpButtonType.Reply, 0);	
				
				AddLabel(463	, 293, LabelHue2, @"Group 8:");
				AddTextEntry(526, 293, 30, 20, LabelHue3, 104 , m_Itemxml.JewelRequiredLevel8Intensity.ToString());
				AddButton(426	, 293, 4005, 4006, GetButtonID( 4, 77 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 317, LabelHue2, @"Group 9:");
				AddTextEntry(526, 317, 30, 20, LabelHue3, 105 , m_Itemxml.JewelRequiredLevel9Intensity.ToString());
				AddButton(426	, 317, 4005, 4006, GetButtonID( 4, 78 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 339, LabelHue2, @"Group 10:");
				AddTextEntry(526, 339, 30, 20, LabelHue3, 106 , m_Itemxml.JewelRequiredLevel10Intensity.ToString());
				AddButton(426	, 339, 4005, 4006, GetButtonID( 4, 79 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 6), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.PetLevels)
			{
				AddLabel(337, 117, LabelHue2, @"Mounted Pets Gain EXP");
				if (m_Itemxml.MountedPetsGainExp == true)
					AddButton(300, 117, 4006, 4005, GetButtonID( 5, 0 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.MountedPetsGainExp == false)
					AddButton(300, 117, 4005, 4006, GetButtonID( 5, 0 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 139, LabelHue2, @"Pet Attack Bonus");
				if (m_Itemxml.PetAttackBonus == true)
					AddButton(300, 139, 4006, 4005, GetButtonID( 5, 1 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.PetAttackBonus == false)
					AddButton(300, 139, 4005, 4006, GetButtonID( 5, 1 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 161, LabelHue2, @"Level Below Pet");
				if (m_Itemxml.LevelBelowPet == true)
					AddButton(300, 161, 4006, 4005, GetButtonID( 5, 2 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.LevelBelowPet == false)
					AddButton(300, 161, 4005, 4006, GetButtonID( 5, 2 ), GumpButtonType.Reply, 0);		
				
				AddLabel(337, 183, LabelHue2, @"Lose Exp/Level On Death");
				if (m_Itemxml.LoseExpLevelOnDeath == true)
					AddButton(300, 183, 4006, 4005, GetButtonID( 5, 3 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.LoseExpLevelOnDeath == false)
					AddButton(300, 183, 4005, 4006, GetButtonID( 5, 3 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 205, LabelHue2, @"Lose Stat On Death");
				if (m_Itemxml.LoseStatOnDeath == true)
					AddButton(300, 205, 4006, 4005, GetButtonID( 5, 4 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.LoseStatOnDeath == false)
					AddButton(300, 205, 4005, 4006, GetButtonID( 5, 4 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 227, LabelHue2, @"Pet Level Sheet - Can Drop");
				if (m_Itemxml.PetLevelSheetPerma == true)
					AddButton(300, 227, 4006, 4005, GetButtonID( 5, 5 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.PetLevelSheetPerma == false)
					AddButton(300, 227, 4005, 4006, GetButtonID( 5, 5 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 249, LabelHue2, @"Player Exp Gain From Pet Kill");
				if (m_Itemxml.PetExpGainFromKill == true)
					AddButton(300, 249, 4006, 4005, GetButtonID( 5, 6 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.PetExpGainFromKill == false)
					AddButton(300, 249, 4005, 4006, GetButtonID( 5, 6 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 271, LabelHue2, @"Refresh Pet On Level");
				if (m_Itemxml.RefreshOnLevelPet == true)
					AddButton(300, 271, 4006, 4005, GetButtonID( 5, 7 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.RefreshOnLevelPet == false)
					AddButton(300, 271, 4005, 4006, GetButtonID( 5, 7 ), GumpButtonType.Reply, 0);	
				
				AddLabel(337, 293, LabelHue2, @"Notify On Pet Exp Gain");
				if (m_Itemxml.NotifyOnPetExpGain == true)
					AddButton(300, 293, 4006, 4005, GetButtonID( 5, 8 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.NotifyOnPetExpGain == false)
					AddButton(300, 293, 4005, 4006, GetButtonID( 5, 8 ), GumpButtonType.Reply, 0);	

				AddLabel(337, 317, LabelHue2, @"Notify On Pet level Up");
				if (m_Itemxml.NotifyOnPetLevelUp == true)
					AddButton(300, 317, 4006, 4005, GetButtonID( 5, 9 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.NotifyOnPetLevelUp == false)
					AddButton(300, 317, 4005, 4006, GetButtonID( 5, 9 ), GumpButtonType.Reply, 0);	
				
				AddLabel(337, 339, LabelHue2, @"Untamed Creature Has Levels");
				if (m_Itemxml.UntamedCreatureLevels == true)
					AddButton(300, 339, 4006, 4005, GetButtonID( 5, 10 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.UntamedCreatureLevels == false)
					AddButton(300, 339, 4005, 4006, GetButtonID( 5, 10 ), GumpButtonType.Reply, 0);	
				
				AddButton(450, 379, 4005, 4007, GetButtonID(1, 14), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
			}
			if (m_Cat == LevelCategory.PetLevels2)
			{
				AddLabel(337, 117, LabelHue2, @"Bonus Attacks");
				AddButton(300, 117, 4006, 4005, GetButtonID( 1, 13 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 139, LabelHue2, @"Pet Max Stat Points.");
				AddTextEntry(265, 139, 30, 20, LabelHue3, 171 , m_Itemxml.PetMaxStatPoints.ToString());
				AddButton(300	, 139, 4005, 4006, GetButtonID( 5, 83 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 161, LabelHue2, @"Death Stat Loss - Ex: 20 is 5% Loss");
				AddTextEntry(265, 161, 30, 20, LabelHue3, 172 , m_Itemxml.PetStatLossAmount.ToString());
				AddButton(300	, 161, 4005, 4006, GetButtonID( 5, 84 ), GumpButtonType.Reply, 0);	
				
				AddLabel(337, 183, LabelHue2, @"Skill Points Awarded");
				AddButton(300, 183, 4006, 4005, GetButtonID( 1, 18 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 205, LabelHue2, @"Emote On Bonus Attacks");
				if (m_Itemxml.EmoteOnSpecialAtks == true)
					AddButton(300, 205, 4006, 4005, GetButtonID( 5, 82 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.EmoteOnSpecialAtks == false)
					AddButton(300, 205, 4005, 4006, GetButtonID( 5, 82 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 227, LabelHue2, @"Use Dynamic Max levels for Pets");
				if (m_Itemxml.UseDynamicMaxLevels == true)
					AddButton(300, 227, 4006, 4005, GetButtonID( 6, 4 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.UseDynamicMaxLevels == false)
					AddButton(300, 227, 4005, 4006, GetButtonID( 6, 4 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 249, LabelHue2, @"Pets Max level - Ignored if Dynamic On!");
				AddTextEntry(265, 249, 30, 20, LabelHue3, 184 , m_Itemxml.StartMaxLvlPets.ToString());
				AddButton(300	, 249, 4005, 4006, GetButtonID( 6, 5 ), GumpButtonType.Reply, 0);
				
				
				AddLabel(337, 271, LabelHue2, @"Bonus Stats");
				AddButton(300, 271, 4006, 4005, GetButtonID( 1, 35 ), GumpButtonType.Reply, 0);
				
				

				AddButton(308, 379, 4005, 4007, GetButtonID(1, 12), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.PetSpecialAttacks)
			{
				AddLabel(280, 117, LabelHue2, @"Name");
				AddLabel(410, 117, LabelHue2, @"Req Level");
				AddLabel(485, 117, LabelHue2, @"Activate Chance");

				AddLabel(317, 139, LabelHue2, @"Super Heal");
				if (m_Itemxml.SuperHeal == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 5, 11 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.SuperHeal == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 5, 11 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 139, 25, 20, LabelHue3, 110 , m_Itemxml.SuperHealReq.ToString());
				AddButton(430	, 139, 4005, 4006, GetButtonID( 5, 20 ), GumpButtonType.Reply, 0);
				AddTextEntry(545, 139, 25, 20, LabelHue3, 119 , m_Itemxml.SuperHealChance.ToString());
				AddButton(505	, 139, 4005, 4006, GetButtonID( 5, 29 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 161, LabelHue2, @"TeleportToTarget");
				if (m_Itemxml.TelePortToTarget == true)
					AddButton(280, 161, 4006, 4005, GetButtonID( 5, 12 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.TelePortToTarget == false)
					AddButton(280, 161, 4005, 4006, GetButtonID( 5, 12 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 161, 25, 20, LabelHue3, 111 , m_Itemxml.TelePortToTargetReq.ToString());
				AddButton(430	, 161, 4005, 4006, GetButtonID( 5, 21 ), GumpButtonType.Reply, 0);
				AddTextEntry(545, 161, 25, 20, LabelHue3, 120 , m_Itemxml.TelePortToTarChance.ToString());
				AddButton(505	, 161, 4005, 4006, GetButtonID( 5, 30 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 183, LabelHue2, @"MassProvokeToAtt");
				if (m_Itemxml.MassProvokeToAtt == true)
					AddButton(280, 183, 4006, 4005, GetButtonID( 5, 13 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.MassProvokeToAtt == false)
					AddButton(280, 183, 4005, 4006, GetButtonID( 5, 13 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 183, 25, 20, LabelHue3, 112 , m_Itemxml.MassProvokeToAttReq.ToString());
				AddButton(430	, 183, 4005, 4006, GetButtonID( 5, 22 ), GumpButtonType.Reply, 0);
				AddTextEntry(545, 183, 25, 20, LabelHue3, 121 , m_Itemxml.MassProvokeChance.ToString());
				AddButton(505	, 183, 4005, 4006, GetButtonID( 5, 31 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 205, LabelHue2, @"Mass Peace Area");
				if (m_Itemxml.MassPeaceArea == true)
					AddButton(280, 205, 4006, 4005, GetButtonID( 5, 14 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.MassPeaceArea == false)
					AddButton(280, 205, 4005, 4006, GetButtonID( 5, 14 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 205, 25, 20, LabelHue3, 113 , m_Itemxml.MassPeaceReq.ToString());
				AddButton(430	, 205, 4005, 4006, GetButtonID( 5, 23 ), GumpButtonType.Reply, 0);
				AddTextEntry(545, 205, 25, 20, LabelHue3, 122 , m_Itemxml.MassPeaceChance.ToString());
				AddButton(505	, 205, 4005, 4006, GetButtonID( 5, 32 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 227, LabelHue2, @"Blessed Power");
				if (m_Itemxml.BlessedPower == true)
					AddButton(280, 227, 4006, 4005, GetButtonID( 5, 15 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.BlessedPower == false)
					AddButton(280, 227, 4005, 4006, GetButtonID( 5, 15 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 227, 25, 20, LabelHue3, 114 , m_Itemxml.BlessedPowerReq.ToString());
				AddButton(430	, 227, 4005, 4006, GetButtonID( 5, 24 ), GumpButtonType.Reply, 0);
				AddTextEntry(545, 227, 25, 20, LabelHue3, 123 , m_Itemxml.BlessedPowerChance.ToString());
				AddButton(505	, 227, 4005, 4006, GetButtonID( 5, 33 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 249, LabelHue2, @"Area Fire Blast");
				if (m_Itemxml.AreaFireBlast == true)
					AddButton(280, 249, 4006, 4005, GetButtonID( 5, 16 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.AreaFireBlast == false)
					AddButton(280, 249, 4005, 4006, GetButtonID( 5, 16 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 249, 25, 20, LabelHue3, 115 , m_Itemxml.AreaFireBlastReq.ToString());
				AddButton(430	, 249, 4005, 4006, GetButtonID( 5, 25 ), GumpButtonType.Reply, 0);
				AddTextEntry(545, 249, 25, 20, LabelHue3, 124 , m_Itemxml.AreaFireBlastChance.ToString());
				AddButton(505	, 249, 4005, 4006, GetButtonID( 5, 34 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 271, LabelHue2, @"Area Ice Blast");
				if (m_Itemxml.AreaIceBlast == true)
					AddButton(280, 271, 4006, 4005, GetButtonID( 5, 17 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.AreaIceBlast == false)
					AddButton(280, 271, 4005, 4006, GetButtonID( 5, 17 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 271, 25, 20, LabelHue3, 116 , m_Itemxml.AreaIceBlastReq.ToString());
				AddButton(430	, 271, 4005, 4006, GetButtonID( 5, 26 ), GumpButtonType.Reply, 0);
				AddTextEntry(545, 271, 25, 20, LabelHue3, 125 , m_Itemxml.AreaIceBlastChance.ToString());
				AddButton(505	, 271, 4005, 4006, GetButtonID( 5, 35 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 293, LabelHue2, @"Area Air Blast");
				if (m_Itemxml.AreaAirBlast == true)
					AddButton(280, 293, 4006, 4005, GetButtonID( 5, 18 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.AreaAirBlast == false)
					AddButton(280, 293, 4005, 4006, GetButtonID( 5, 18 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 293, 25, 20, LabelHue3, 117 , m_Itemxml.AreaAirBlastReq.ToString());
				AddButton(430	, 293, 4005, 4006, GetButtonID( 5, 27 ), GumpButtonType.Reply, 0);
				AddTextEntry(545, 293, 25, 20, LabelHue3, 126 , m_Itemxml.AreaAirBlastChance.ToString());
				AddButton(505	, 293, 4005, 4006, GetButtonID( 5, 36 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 317, LabelHue2, @"Aura Stat Boost");
				if (m_Itemxml.AuraStatBoost == true)
					AddButton(280, 317, 4006, 4005, GetButtonID( 5, 19 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.AuraStatBoost == false)
					AddButton(280, 317, 4005, 4006, GetButtonID( 5, 19 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 317, 25, 20, LabelHue3, 118 , m_Itemxml.AuraStatBoostReq.ToString());
				AddButton(430	, 317, 4005, 4006, GetButtonID( 5, 28 ), GumpButtonType.Reply, 0);
				
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 14), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.PetLevelReq)
			{
				AddLabel(280, 117, LabelHue2, @"Creature Name");
				AddLabel(420, 117, LabelHue2, @"Level Req to Mount/Ride");
				
				AddLabel(317, 139, LabelHue2, @"Toggle Mount Level Checking");
				if (m_Itemxml.EnableMountCheck == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 5, 37 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.EnableMountCheck == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 5, 37 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 161, LabelHue2, @"Desert Ostard");
				AddTextEntry(450, 161, 30, 20, LabelHue3, 128 , m_Itemxml.DesertOstard.ToString());
				AddButton(280	, 161, 4005, 4006, GetButtonID( 5, 39 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 183, LabelHue2, @"Fire Steed");
				AddTextEntry(450, 183, 30, 20, LabelHue3, 129 , m_Itemxml.FireSteed.ToString());
				AddButton(280	, 183, 4005, 4006, GetButtonID( 5, 40 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 205, LabelHue2, @"Forest Ostard");
				AddTextEntry(450, 205, 30, 20, LabelHue3, 130 , m_Itemxml.ForestOstard.ToString());
				AddButton(280	, 205, 4005, 4006, GetButtonID( 5, 41 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 227, LabelHue2, @"Frenzied Ostard");
				AddTextEntry(450, 227, 30, 20, LabelHue3, 131 , m_Itemxml.FrenziedOstard.ToString());
				AddButton(280	, 227, 4005, 4006, GetButtonID( 5, 42 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 249, LabelHue2, @"Hell Steed");
				AddTextEntry(450, 249, 30, 20, LabelHue3, 132 , m_Itemxml.HellSteed.ToString());
				AddButton(280	, 249, 4005, 4006, GetButtonID( 5, 43 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 271, LabelHue2, @"Hiryu");
				AddTextEntry(450, 271, 30, 20, LabelHue3, 133 , m_Itemxml.Hiryu.ToString());
				AddButton(280	, 271, 4005, 4006, GetButtonID( 5, 44 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 293, LabelHue2, @"Cusidhe");
				AddTextEntry(450, 293, 30, 20, LabelHue3, 134 , m_Itemxml.Cusidhe.ToString());
				AddButton(280	, 293, 4005, 4006, GetButtonID( 5, 45 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 317, LabelHue2, @"Kirin");
				AddTextEntry(450, 317, 30, 20, LabelHue3, 135 , m_Itemxml.Kirin.ToString());
				AddButton(280	, 317, 4005, 4006, GetButtonID( 5, 46 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 339, LabelHue2, @"Lesser Hiryu");
				AddTextEntry(450, 339, 30, 20, LabelHue3, 136 , m_Itemxml.LesserHiryu.ToString());
				AddButton(280	, 339, 4005, 4006, GetButtonID( 5, 47 ), GumpButtonType.Reply, 0);
				
				
				AddButton(450, 379, 4005, 4007, GetButtonID(1, 16), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.PetLevelReq2)
			{
				AddLabel(280, 117, LabelHue2, @"Creature Name");
				AddLabel(420, 117, LabelHue2, @"Level Req to Mount/Ride");

				AddLabel(317	, 139, LabelHue2, @"Night Mare");
				AddTextEntry(450, 139, 30, 20, LabelHue3, 137 , m_Itemxml.NightMare.ToString());
				AddButton(280	, 139, 4005, 4006, GetButtonID( 5, 48 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 161, LabelHue2, @"Ridable Llama");
				AddTextEntry(450, 161, 30, 20, LabelHue3, 138 , m_Itemxml.Ridablellama.ToString());
				AddButton(280	, 161, 4005, 4006, GetButtonID( 5, 49 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 183, LabelHue2, @"Ridge back");
				AddTextEntry(450, 183, 30, 20, LabelHue3, 139 , m_Itemxml.Ridgeback.ToString());
				AddButton(280	, 183, 4005, 4006, GetButtonID( 5, 50 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 205, LabelHue2, @"Savage Ridgeback");
				AddTextEntry(450, 205, 30, 20, LabelHue3, 140 , m_Itemxml.SavageRidgeback.ToString());
				AddButton(280	, 205, 4005, 4006, GetButtonID( 5, 51 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 227, LabelHue2, @"Scaled Swamp Dragon");
				AddTextEntry(450, 227, 30, 20, LabelHue3, 141 , m_Itemxml.ScaledSwampDragon.ToString());
				AddButton(280	, 227, 4005, 4006, GetButtonID( 5, 52 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 249, LabelHue2, @"Sea Horse");
				AddTextEntry(450, 249, 30, 20, LabelHue3, 142 , m_Itemxml.Seahorse.ToString());
				AddButton(280	, 249, 4005, 4006, GetButtonID( 5, 53 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 271, LabelHue2, @"Silver Steed");
				AddTextEntry(450, 271, 30, 20, LabelHue3, 143 , m_Itemxml.SilverSteed.ToString());
				AddButton(280	, 271, 4005, 4006, GetButtonID( 5, 54 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 293, LabelHue2, @"Horse");
				AddTextEntry(450, 293, 30, 20, LabelHue3, 144 , m_Itemxml.Horse.ToString());
				AddButton(280	, 293, 4005, 4006, GetButtonID( 5, 55 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 317, LabelHue2, @"Skeletal Mount");
				AddTextEntry(450, 317, 30, 20, LabelHue3, 145 , m_Itemxml.SkeletalMount.ToString());
				AddButton(280	, 317, 4005, 4006, GetButtonID( 5, 56 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 339, LabelHue2, @"Swamp Dragon");
				AddTextEntry(450, 339, 30, 20, LabelHue3, 146 , m_Itemxml.Swampdragon.ToString());
				AddButton(280	, 339, 4005, 4006, GetButtonID( 5, 57 ), GumpButtonType.Reply, 0);
				
				AddButton(450, 379, 4005, 4007, GetButtonID(1, 17), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 15), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.PetLevelReq3)
			{
				AddLabel(280, 117, LabelHue2, @"Creature Name");
				AddLabel(420, 117, LabelHue2, @"Level Req to Mount/Ride");

				AddLabel(317	, 139, LabelHue2, @"Unicorn");
				AddTextEntry(450, 139, 30, 20, LabelHue3, 147 , m_Itemxml.Unicorn.ToString());
				AddButton(280	, 139, 4005, 4006, GetButtonID( 5, 58 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 161, LabelHue2, @"Reptalon");
				AddTextEntry(450, 161, 30, 20, LabelHue3, 148 , m_Itemxml.Reptalon.ToString());
				AddButton(280	, 161, 4005, 4006, GetButtonID( 5, 59 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 183, LabelHue2, @"Wild Tiger");
				AddTextEntry(450, 183, 30, 20, LabelHue3, 149 , m_Itemxml.Wildtiger.ToString());
				AddButton(280	, 183, 4005, 4006, GetButtonID( 5, 60 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 205, LabelHue2, @"Windrunner");
				AddTextEntry(450, 205, 30, 20, LabelHue3, 150 , m_Itemxml.Windrunner.ToString());
				AddButton(280	, 205, 4005, 4006, GetButtonID( 5, 61 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 227, LabelHue2, @"Lasher");
				AddTextEntry(450, 227, 30, 20, LabelHue3, 151 , m_Itemxml.Lasher.ToString());
				AddButton(280	, 227, 4005, 4006, GetButtonID( 5, 62 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 249, LabelHue2, @"Eowmu");
				AddTextEntry(450, 249, 30, 20, LabelHue3, 152 , m_Itemxml.Eowmu.ToString());
				AddButton(280	, 249, 4005, 4006, GetButtonID( 5, 63 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 271, LabelHue2, @"Dread Warhorse");
				AddTextEntry(450, 271, 30, 20, LabelHue3, 153 , m_Itemxml.Dreadwarhorse.ToString());
				AddButton(280	, 271, 4005, 4006, GetButtonID( 5, 64 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 293, LabelHue2, @"Beetle");
				AddTextEntry(450, 293, 30, 20, LabelHue3, 127 , m_Itemxml.Beetle.ToString());
				AddButton(280	, 293, 4005, 4006, GetButtonID( 5, 38 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 16), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.PetLevelSkillPoints)
			{
				AddLabel(322	, 139, LabelHue2, @"Below L20");
				AddTextEntry(391, 139, 30, 20, LabelHue3, 154 , m_Itemxml.Petbelow20.ToString());
				AddButton(285	, 139, 4005, 4006, GetButtonID( 5, 65 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 161, LabelHue2, @"Below L40");
				AddTextEntry(391, 161, 30, 20, LabelHue3, 155 , m_Itemxml.Petbelow40.ToString());
				AddButton(285	, 161, 4005, 4006, GetButtonID( 5, 66 ), GumpButtonType.Reply, 0);	
				
				AddLabel(322	, 183, LabelHue2, @"Below L60");
				AddTextEntry(391, 183, 30, 20, LabelHue3, 156 , m_Itemxml.Petbelow60.ToString());
				AddButton(285	, 183, 4005, 4006, GetButtonID( 5, 67 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 205, LabelHue2, @"Below L70");
				AddTextEntry(391, 205, 30, 20, LabelHue3, 157 , m_Itemxml.Petbelow70.ToString());
				AddButton(285	, 205, 4005, 4006, GetButtonID( 5, 68 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 227, LabelHue2, @"Below L80");
				AddTextEntry(391, 227, 30, 20, LabelHue3, 158 , m_Itemxml.Petbelow80.ToString());
				AddButton(285	, 227, 4005, 4006, GetButtonID( 5, 69 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 249, LabelHue2, @"Below L90");
				AddTextEntry(391, 249, 30, 20, LabelHue3, 159 , m_Itemxml.Petbelow90.ToString());
				AddButton(285	, 249, 4005, 4006, GetButtonID( 5, 70 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 271, LabelHue2, @"Below L100");
				AddTextEntry(391, 271, 30, 20, LabelHue3, 160 , m_Itemxml.Petbelow100.ToString());
				AddButton(285	, 271, 4005, 4006, GetButtonID( 5, 71 ), GumpButtonType.Reply, 0);	
				
				AddLabel(322	, 293, LabelHue2, @"Below L110");
				AddTextEntry(391, 293, 30, 20, LabelHue3, 161 , m_Itemxml.Petbelow110.ToString());
				AddButton(285	, 293, 4005, 4006, GetButtonID( 5, 72 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 317, LabelHue2, @"Below L120");
				AddTextEntry(391, 317, 30, 20, LabelHue3, 162 , m_Itemxml.Petbelow120.ToString());
				AddButton(285	, 317, 4005, 4006, GetButtonID( 5, 73 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 339, LabelHue2, @"Below L130");
				AddTextEntry(391, 339, 30, 20, LabelHue3, 163 , m_Itemxml.Petbelow130.ToString());
				AddButton(285	, 339, 4005, 4006, GetButtonID( 5, 74 ), GumpButtonType.Reply, 0);
				
				AddLabel(468	, 139, LabelHue2, @"Below L140");
				AddTextEntry(540, 139, 30, 20, LabelHue3, 164 , m_Itemxml.Petbelow140.ToString());
				AddButton(431	, 139, 4005, 4006, GetButtonID( 5, 75 ), GumpButtonType.Reply, 0);
				
				AddLabel(468	, 161, LabelHue2, @"Below L150");
				AddTextEntry(540, 161, 30, 20, LabelHue3, 165 , m_Itemxml.Petbelow150.ToString());
				AddButton(431	, 161, 4005, 4006, GetButtonID( 5, 76 ), GumpButtonType.Reply, 0);	
				
				AddLabel(468	, 183, LabelHue2, @"Below L160");
				AddTextEntry(540, 183, 30, 20, LabelHue3, 166 , m_Itemxml.Petbelow160.ToString());
				AddButton(431	, 183, 4005, 4006, GetButtonID( 5, 77 ), GumpButtonType.Reply, 0);
				
				AddLabel(468	, 205, LabelHue2, @"Below L170");
				AddTextEntry(540, 205, 30, 20, LabelHue3, 167 , m_Itemxml.Petbelow170.ToString());
				AddButton(431	, 205, 4005, 4006, GetButtonID( 5, 78 ), GumpButtonType.Reply, 0);
				
				AddLabel(468	, 227, LabelHue2, @"Below L180");
				AddTextEntry(540, 227, 30, 20, LabelHue3, 168 , m_Itemxml.Petbelow180.ToString());
				AddButton(431	, 227, 4005, 4006, GetButtonID( 5, 79 ), GumpButtonType.Reply, 0);
				
				AddLabel(468	, 249, LabelHue2, @"Below L190");
				AddTextEntry(540, 249, 30, 20, LabelHue3, 169 , m_Itemxml.Petbelow190.ToString());
				AddButton(431	, 249, 4005, 4006, GetButtonID( 5, 80 ), GumpButtonType.Reply, 0);
				
				AddLabel(468	, 271, LabelHue2, @"Below L200");
				AddTextEntry(540, 271, 30, 20, LabelHue3, 170 , m_Itemxml.Petbelow200.ToString());
				AddButton(431	, 271, 4005, 4006, GetButtonID( 5, 81 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 14), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.PetLevelStatPoints)
			{
				AddLabel(322	, 139, LabelHue2, @"Below L20");
				AddTextEntry(391, 139, 30, 20, LabelHue3, 1200 , m_Itemxml.Petbelow20stat.ToString());
				AddButton(285	, 139, 4005, 4006, GetButtonID( 5, 85 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 161, LabelHue2, @"Below L40");
				AddTextEntry(391, 161, 30, 20, LabelHue3, 1201 , m_Itemxml.Petbelow40stat.ToString());
				AddButton(285	, 161, 4005, 4006, GetButtonID( 5, 86 ), GumpButtonType.Reply, 0);	
				
				AddLabel(322	, 183, LabelHue2, @"Below L60");
				AddTextEntry(391, 183, 30, 20, LabelHue3, 1202 , m_Itemxml.Petbelow60stat.ToString());
				AddButton(285	, 183, 4005, 4006, GetButtonID( 5, 87 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 205, LabelHue2, @"Below L70");
				AddTextEntry(391, 205, 30, 20, LabelHue3, 1203 , m_Itemxml.Petbelow70stat.ToString());
				AddButton(285	, 205, 4005, 4006, GetButtonID( 5, 88 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 227, LabelHue2, @"Below L80");
				AddTextEntry(391, 227, 30, 20, LabelHue3, 1204 , m_Itemxml.Petbelow80stat.ToString());
				AddButton(285	, 227, 4005, 4006, GetButtonID( 5, 89 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 249, LabelHue2, @"Below L90");
				AddTextEntry(391, 249, 30, 20, LabelHue3, 1205 , m_Itemxml.Petbelow90stat.ToString());
				AddButton(285	, 249, 4005, 4006, GetButtonID( 5, 90 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 271, LabelHue2, @"Below L100");
				AddTextEntry(391, 271, 30, 20, LabelHue3, 1206 , m_Itemxml.Petbelow100stat.ToString());
				AddButton(285	, 271, 4005, 4006, GetButtonID( 5, 91 ), GumpButtonType.Reply, 0);	
				
				AddLabel(322	, 293, LabelHue2, @"Below L110");
				AddTextEntry(391, 293, 30, 20, LabelHue3, 1207 , m_Itemxml.Petbelow110stat.ToString());
				AddButton(285	, 293, 4005, 4006, GetButtonID( 5, 92 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 317, LabelHue2, @"Below L120");
				AddTextEntry(391, 317, 30, 20, LabelHue3, 1208 , m_Itemxml.Petbelow120stat.ToString());
				AddButton(285	, 317, 4005, 4006, GetButtonID( 5, 93 ), GumpButtonType.Reply, 0);
				
				AddLabel(322	, 339, LabelHue2, @"Below L130");
				AddTextEntry(391, 339, 30, 20, LabelHue3, 1209 , m_Itemxml.Petbelow130stat.ToString());
				AddButton(285	, 339, 4005, 4006, GetButtonID( 5, 94 ), GumpButtonType.Reply, 0);
				
				AddLabel(468	, 139, LabelHue2, @"Below L140");
				AddTextEntry(540, 139, 30, 20, LabelHue3, 1210 , m_Itemxml.Petbelow140stat.ToString());
				AddButton(431	, 139, 4005, 4006, GetButtonID( 5, 95 ), GumpButtonType.Reply, 0);
				
				AddLabel(468	, 161, LabelHue2, @"Below L150");
				AddTextEntry(540, 161, 30, 20, LabelHue3, 1211 , m_Itemxml.Petbelow150stat.ToString());
				AddButton(431	, 161, 4005, 4006, GetButtonID( 5, 96 ), GumpButtonType.Reply, 0);	
				
				AddLabel(468	, 183, LabelHue2, @"Below L160");
				AddTextEntry(540, 183, 30, 20, LabelHue3, 1212 , m_Itemxml.Petbelow160stat.ToString());
				AddButton(431	, 183, 4005, 4006, GetButtonID( 5, 97 ), GumpButtonType.Reply, 0);
				
				AddLabel(468	, 205, LabelHue2, @"Below L170");
				AddTextEntry(540, 205, 30, 20, LabelHue3, 1213 , m_Itemxml.Petbelow170stat.ToString());
				AddButton(431	, 205, 4005, 4006, GetButtonID( 5, 98 ), GumpButtonType.Reply, 0);
				
				AddLabel(468	, 227, LabelHue2, @"Below L180");
				AddTextEntry(540, 227, 30, 20, LabelHue3, 1214 , m_Itemxml.Petbelow180stat.ToString());
				AddButton(431	, 227, 4005, 4006, GetButtonID( 5, 99 ), GumpButtonType.Reply, 0);
				
				AddLabel(468	, 249, LabelHue2, @"Below L190");
				AddTextEntry(540, 249, 30, 20, LabelHue3, 1215 , m_Itemxml.Petbelow190stat.ToString());
				AddButton(431	, 249, 4005, 4006, GetButtonID( 5, 100 ), GumpButtonType.Reply, 0);
				
				AddLabel(468	, 271, LabelHue2, @"Below L200");
				AddTextEntry(540, 271, 30, 20, LabelHue3, 1216 , m_Itemxml.Petbelow200stat.ToString());
				AddButton(431	, 271, 4005, 4006, GetButtonID( 5, 101 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 14), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			
			if (m_Cat == LevelCategory.Expansions)
			{
				AddLabel(337, 117, LabelHue2, @"Pet Stealing.");
				AddButton(300, 117, 4005, 4006, GetButtonID( 1, 20 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 139, LabelHue2, @"Mount Level Check.");
				AddButton(300, 139, 4005, 4006, GetButtonID( 1, 15 ), GumpButtonType.Reply, 0);
					
				AddLabel(337, 161, LabelHue2, @"Dynamic Equipment Level.  (*)");
				AddButton(300, 161, 4005, 4007, GetButtonID( 1, 6 ), GumpButtonType.Reply, 0);

				AddLabel(337, 183, LabelHue2, @"Toggle Vendor Discounts. (*)");
				if (m_Itemxml.DiscountFromVendors == true)
					AddButton(300, 183, 4006, 4005, GetButtonID( 2, 5 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.DiscountFromVendors == false)
					AddButton(300, 183, 4005, 4006, GetButtonID( 2, 5 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 205, LabelHue2, @"Exp Gain from Skills.  (*)");
				AddButton(300, 205, 4005, 4006, GetButtonID( 1, 21 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 227, LabelHue2, @"Gain Pet Slot on level.");
				AddButton(300, 227, 4005, 4006, GetButtonID( 1, 27 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 249, LabelHue2, @"Player Start Up Options.");
				AddButton(300, 249, 4005, 4006, GetButtonID( 1, 29 ), GumpButtonType.Reply, 0);
				
				AddLabel(337, 271, LabelHue2, @"Level Bag Options.");
				AddButton(300, 271, 4005, 4006, GetButtonID( 1, 34 ), GumpButtonType.Reply, 0);
				
				/* Help Buttons */
				
				/* Dynamic Equipment */
				AddButton(280, 161, 0x5689, 0x568A, GetButtonID(1, 8), GumpButtonType.Reply, 0);
				/* Pet Stealing */
				AddButton(280, 117, 0x5689, 0x568A, GetButtonID(1, 40), GumpButtonType.Reply, 0);
				/* Mount Check */
				AddButton(280, 139, 0x5689, 0x568A, GetButtonID(1, 41), GumpButtonType.Reply, 0);
				/* Vendor Level Check */
				AddButton(280, 183, 0x5689, 0x568A, GetButtonID(1, 42), GumpButtonType.Reply, 0);
				/* EXP Gain check */
				AddButton(280, 205, 0x5689, 0x568A, GetButtonID(1, 43), GumpButtonType.Reply, 0);
				/* Pet Slot on Gain */
				AddButton(280, 227, 0x5689, 0x568A, GetButtonID(1, 44), GumpButtonType.Reply, 0);
				/* Player Startup Options */
				AddButton(280, 249, 0x5689, 0x568A, GetButtonID(1, 45), GumpButtonType.Reply, 0);
				/* Player Startup Options */
				AddButton(280, 271, 0x5689, 0x568A, GetButtonID(1, 46), GumpButtonType.Reply, 0);
			}
			if (m_Cat == LevelCategory.PetStealing)
			{
				AddLabel(337, 117, LabelHue2, @"Enable Pet Stealing Picks");
				if (m_Itemxml.EnablePetpicks == true)
					AddButton(300, 117, 4006, 4005, GetButtonID( 6, 1 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.EnablePetpicks == false)
					AddButton(300, 117, 4005, 4006, GetButtonID( 6, 1 ), GumpButtonType.Reply, 0);
				
				AddLabel(337	, 139, LabelHue2, @"Min Stealing Skill Required");
				AddTextEntry(265, 139, 30, 20, LabelHue3, 183 , m_Itemxml.MinSkillReqPickSteal.ToString());
				AddButton(300	, 139, 4005, 4006, GetButtonID( 6, 2 ), GumpButtonType.Reply, 0);	
				
				AddLabel(337, 161, LabelHue2, @"Bonded Pet Can be Stolen");
				if (m_Itemxml.PreventBondedPetpick == true)
					AddButton(300, 161, 4006, 4005, GetButtonID( 6, 3 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.PreventBondedPetpick == false)
					AddButton(300, 161, 4005, 4006, GetButtonID( 6, 3 ), GumpButtonType.Reply, 0);	
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(345, 379, LabelHue2, @"Previous Page");
			}
			
			if (m_Cat == LevelCategory.ConfiguredSkillsEXP)
			{
				AddLabel(280, 117, LabelHue2, @"Skill Toggle");
				AddLabel(430, 117, LabelHue2, @"EXP Gained");

				AddLabel(317, 139, LabelHue2, @"Master Toggle - Gain Exp From Skills");
				if (m_Itemxml.Enableexpfromskills == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 7, 1 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Enableexpfromskills == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 7, 1 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 161, LabelHue2, @"Begging");
				if (m_Itemxml.Begginggain == true)
					AddButton(280, 161, 4006, 4005, GetButtonID( 7, 2 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Begginggain == false)
					AddButton(280, 161, 4005, 4006, GetButtonID( 7, 2 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 161, 25, 20, LabelHue3, 185 , m_Itemxml.Begginggainamount.ToString());
				AddButton(430	, 161, 4005, 4006, GetButtonID( 7, 35 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 183, LabelHue2, @"Camping");
				if (m_Itemxml.Campinggain == true)
					AddButton(280, 183, 4006, 4005, GetButtonID( 7, 3 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Campinggain == false)
					AddButton(280, 183, 4005, 4006, GetButtonID( 7, 3 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 183, 25, 20, LabelHue3, 186 , m_Itemxml.Campinggainamount.ToString());
				AddButton(430	, 183, 4005, 4006, GetButtonID( 7, 36 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 205, LabelHue2, @"Forensics");
				if (m_Itemxml.Forensicsgain == true)
					AddButton(280, 205, 4006, 4005, GetButtonID( 7, 4 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Forensicsgain == false)
					AddButton(280, 205, 4005, 4006, GetButtonID( 7, 4 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 205, 25, 20, LabelHue3, 187 , m_Itemxml.Forensicsgainamount.ToString());
				AddButton(430	, 205, 4005, 4006, GetButtonID( 7, 37 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 227, LabelHue2, @"Item ID");
				if (m_Itemxml.Itemidgain == true)
					AddButton(280, 227, 4006, 4005, GetButtonID( 7, 5 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Itemidgain == false)
					AddButton(280, 227, 4005, 4006, GetButtonID( 7, 5 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 227, 25, 20, LabelHue3, 188 , m_Itemxml.Itemidgainamount.ToString());
				AddButton(430	, 227, 4005, 4006, GetButtonID( 7, 38 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 249, LabelHue2, @"Taste ID");
				if (m_Itemxml.Tasteidgain == true)
					AddButton(280, 249, 4006, 4005, GetButtonID( 7, 6 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Tasteidgain == false)
					AddButton(280, 249, 4005, 4006, GetButtonID( 7, 6 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 249, 25, 20, LabelHue3, 189 , m_Itemxml.Tasteidgainamount.ToString());
				AddButton(430	, 249, 4005, 4006, GetButtonID( 7, 39 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 271, LabelHue2, @"Imbuing");
				if (m_Itemxml.Imbuinggain == true)
					AddButton(280, 271, 4006, 4005, GetButtonID( 7, 7 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Imbuinggain == false)
					AddButton(280, 271, 4005, 4006, GetButtonID( 7, 7 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 271, 25, 20, LabelHue3, 190 , m_Itemxml.Imbuinggainamount.ToString());
				AddButton(430	, 271, 4005, 4006, GetButtonID( 7, 40 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 293, LabelHue2, @"Eval Int");
				if (m_Itemxml.Evalintgain == true)
					AddButton(280, 293, 4006, 4005, GetButtonID( 7, 8 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Evalintgain == false)
					AddButton(280, 293, 4005, 4006, GetButtonID( 7, 8 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 293, 25, 20, LabelHue3, 191 , m_Itemxml.Evalintgainamount.ToString());
				AddButton(430	, 293, 4005, 4006, GetButtonID( 7, 41 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 317, LabelHue2, @"Spirit Speak");
				if (m_Itemxml.Spiritspeakgain == true)
					AddButton(280, 317, 4006, 4005, GetButtonID( 7, 9 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Spiritspeakgain == false)
					AddButton(280, 317, 4005, 4006, GetButtonID( 7, 9 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 317, 25, 20, LabelHue3, 192 , m_Itemxml.Spiritspeakgainamount.ToString());
				AddButton(430	, 317, 4005, 4006, GetButtonID( 7, 42 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 339, LabelHue2, @"Fishing");
				if (m_Itemxml.Fishinggain == true)
					AddButton(280, 339, 4006, 4005, GetButtonID( 7, 10 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Fishinggain == false)
					AddButton(280, 339, 4005, 4006, GetButtonID( 7, 10 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 339, 25, 20, LabelHue3, 193 , m_Itemxml.Fishinggainamount.ToString());
				AddButton(430	, 339, 4005, 4006, GetButtonID( 7, 43 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
				
				AddButton(450, 379, 4005, 4007, GetButtonID(1, 22), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
			}
			if (m_Cat == LevelCategory.ConfiguredSkillsEXP2)
			{
				AddLabel(280, 117, LabelHue2, @"Skill Toggle");
				AddLabel(430, 117, LabelHue2, @"EXP Gained");
				
				AddLabel(317, 139, LabelHue2, @"Herding");
				if (m_Itemxml.Herdinggain == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 7, 11 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Herdinggain == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 7, 11 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 139, 25, 20, LabelHue3, 194 , m_Itemxml.Herdinggainamount.ToString());
				AddButton(430	, 139, 4005, 4006, GetButtonID( 7, 44 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 161, LabelHue2, @"Tracking");
				if (m_Itemxml.Trackinggain == true)
					AddButton(280, 161, 4006, 4005, GetButtonID( 7, 12 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Trackinggain == false)
					AddButton(280, 161, 4005, 4006, GetButtonID( 7, 12 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 161, 25, 20, LabelHue3, 195 , m_Itemxml.Trackinggainamount.ToString());
				AddButton(430	, 161, 4005, 4006, GetButtonID( 7, 45 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 183, LabelHue2, @"Hiding");
				if (m_Itemxml.Hidinggain == true)
					AddButton(280, 183, 4006, 4005, GetButtonID( 7, 13 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Hidinggain == false)
					AddButton(280, 183, 4005, 4006, GetButtonID( 7, 13 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 183, 25, 20, LabelHue3, 196 , m_Itemxml.Hidinggainamount.ToString());
				AddButton(430	, 183, 4005, 4006, GetButtonID( 7, 46 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 205, LabelHue2, @"Poisoning");
				if (m_Itemxml.Poisoninggain == true)
					AddButton(280, 205, 4006, 4005, GetButtonID( 7, 14 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Poisoninggain == false)
					AddButton(280, 205, 4005, 4006, GetButtonID( 7, 14 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 205, 25, 20, LabelHue3, 197 , m_Itemxml.Poisoninggainamount.ToString());
				AddButton(430	, 205, 4005, 4006, GetButtonID( 7, 47 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 227, LabelHue2, @"Removetrap");
				if (m_Itemxml.Removetrapgain == true)
					AddButton(280, 227, 4006, 4005, GetButtonID( 7, 15 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Removetrapgain == false)
					AddButton(280, 227, 4005, 4006, GetButtonID( 7, 15 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 227, 25, 20, LabelHue3, 198 , m_Itemxml.Removetrapgainamount.ToString());
				AddButton(430	, 227, 4005, 4006, GetButtonID( 7, 48 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 249, LabelHue2, @"Stealing");
				if (m_Itemxml.Stealinggain == true)
					AddButton(280, 249, 4006, 4005, GetButtonID( 7, 16 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Stealinggain == false)
					AddButton(280, 249, 4005, 4006, GetButtonID( 7, 16 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 249, 25, 20, LabelHue3, 199 , m_Itemxml.Stealinggainamount.ToString());
				AddButton(430	, 249, 4005, 4006, GetButtonID( 7, 49 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 271, LabelHue2, @"Discordance");
				if (m_Itemxml.Discordancegain == true)
					AddButton(280, 271, 4006, 4005, GetButtonID( 7, 17 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Discordancegain == false)
					AddButton(280, 271, 4005, 4006, GetButtonID( 7, 17 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 271, 25, 20, LabelHue3, 200 , m_Itemxml.Discordancegainamount.ToString());
				AddButton(430	, 271, 4005, 4006, GetButtonID( 7, 50 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 293, LabelHue2, @"Peacemaking");
				if (m_Itemxml.Peacemakinggain == true)
					AddButton(280, 293, 4006, 4005, GetButtonID( 7, 18 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Peacemakinggain == false)
					AddButton(280, 293, 4005, 4006, GetButtonID( 7, 18 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 293, 25, 20, LabelHue3, 201 , m_Itemxml.Peacemakinggainamount.ToString());
				AddButton(430	, 293, 4005, 4006, GetButtonID( 7, 51 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 317, LabelHue2, @"Provocation");
				if (m_Itemxml.Provocationgain == true)
					AddButton(280, 317, 4006, 4005, GetButtonID( 7, 19 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Provocationgain == false)
					AddButton(280, 317, 4005, 4006, GetButtonID( 7, 19 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 317, 25, 20, LabelHue3, 202 , m_Itemxml.Provocationgainamount.ToString());
				AddButton(430	, 317, 4005, 4006, GetButtonID( 7, 52 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 339, LabelHue2, @"Anatomy");
				if (m_Itemxml.Anatomygain == true)
					AddButton(280, 339, 4006, 4005, GetButtonID( 7, 20 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Anatomygain == false)
					AddButton(280, 339, 4005, 4006, GetButtonID( 7, 20 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 339, 25, 20, LabelHue3, 203 , m_Itemxml.Anatomygainamount.ToString());
				AddButton(430	, 339, 4005, 4006, GetButtonID( 7, 53 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 21), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
				
				AddButton(450, 379, 4005, 4007, GetButtonID(1, 23), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
			}
			if (m_Cat == LevelCategory.ConfiguredSkillsEXP3)
			{
				AddLabel(280, 117, LabelHue2, @"Skill Toggle");
				AddLabel(430, 117, LabelHue2, @"EXP Gained");
				
				AddLabel(317, 139, LabelHue2, @"Armslore");
				if (m_Itemxml.Armsloregain == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 7, 21 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Armsloregain == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 7, 21 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 139, 25, 20, LabelHue3, 204 , m_Itemxml.Armsloregainamount.ToString());
				AddButton(430	, 139, 4005, 4006, GetButtonID( 7, 54 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 161, LabelHue2, @"Animallore");
				if (m_Itemxml.Animalloregain == true)
					AddButton(280, 161, 4006, 4005, GetButtonID( 7, 22 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Animalloregain == false)
					AddButton(280, 161, 4005, 4006, GetButtonID( 7, 22 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 161, 25, 20, LabelHue3, 205 , m_Itemxml.Animalloregainamount.ToString());
				AddButton(430	, 161, 4005, 4006, GetButtonID( 7, 55 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 183, LabelHue2, @"Meditation");
				if (m_Itemxml.Meditationgain == true)
					AddButton(280, 183, 4006, 4005, GetButtonID( 7, 23 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Meditationgain == false)
					AddButton(280, 183, 4005, 4006, GetButtonID( 7, 23 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 183, 25, 20, LabelHue3, 206 , m_Itemxml.Meditationgainamount.ToString());
				AddButton(430	, 183, 4005, 4006, GetButtonID( 7, 56 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 205, LabelHue2, @"Cartography");
				if (m_Itemxml.Cartographygain == true)
					AddButton(280, 205, 4006, 4005, GetButtonID( 7, 24 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Cartographygain == false)
					AddButton(280, 205, 4005, 4006, GetButtonID( 7, 24 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 205, 25, 20, LabelHue3, 207 , m_Itemxml.Cartographygainamount.ToString());
				AddButton(430	, 205, 4005, 4006, GetButtonID( 7, 57 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 227, LabelHue2, @"Detecthidden");
				if (m_Itemxml.Detecthiddengain == true)
					AddButton(280, 227, 4006, 4005, GetButtonID( 7, 25 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Detecthiddengain == false)
					AddButton(280, 227, 4005, 4006, GetButtonID( 7, 25 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 227, 25, 20, LabelHue3, 208 , m_Itemxml.Detecthiddengainamount.ToString());
				AddButton(430	, 227, 4005, 4006, GetButtonID( 7, 58 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 249, LabelHue2, @"Animaltaming");
				if (m_Itemxml.Animaltaminggain == true)
					AddButton(280, 249, 4006, 4005, GetButtonID( 7, 26 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Animaltaminggain == false)
					AddButton(280, 249, 4005, 4006, GetButtonID( 7, 26 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 249, 25, 20, LabelHue3, 209 , m_Itemxml.Animaltaminggainamount.ToString());
				AddButton(430	, 249, 4005, 4006, GetButtonID( 7, 59 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 271, LabelHue2, @"Blacksmith");
				if (m_Itemxml.Blacksmithgain == true)
					AddButton(280, 271, 4006, 4005, GetButtonID( 7, 27 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Blacksmithgain == false)
					AddButton(280, 271, 4005, 4006, GetButtonID( 7, 27 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 271, 25, 20, LabelHue3, 210 , m_Itemxml.Blacksmithgainamount.ToString());
				AddButton(430	, 271, 4005, 4006, GetButtonID( 7, 60 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 293, LabelHue2, @"Carpentry");
				if (m_Itemxml.Carpentrygain == true)
					AddButton(280, 293, 4006, 4005, GetButtonID( 7, 28 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Carpentrygain == false)
					AddButton(280, 293, 4005, 4006, GetButtonID( 7, 28 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 293, 25, 20, LabelHue3, 211 , m_Itemxml.Carpentrygainamount.ToString());
				AddButton(430	, 293, 4005, 4006, GetButtonID( 7, 61 ), GumpButtonType.Reply, 0);

				AddLabel(317, 317, LabelHue2, @"Alchemy");
				if (m_Itemxml.Alchemygain == true)
					AddButton(280, 317, 4006, 4005, GetButtonID( 7, 29 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Alchemygain == false)
					AddButton(280, 317, 4005, 4006, GetButtonID( 7, 29 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 317, 25, 20, LabelHue3, 212 , m_Itemxml.Alchemygainamount.ToString());
				AddButton(430	, 317, 4005, 4006, GetButtonID( 7, 62 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 339, LabelHue2, @"Fletching");
				if (m_Itemxml.Fletchinggain == true)
					AddButton(280, 339, 4006, 4005, GetButtonID( 7, 30 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Fletchinggain == false)
					AddButton(280, 339, 4005, 4006, GetButtonID( 7, 30 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 339, 25, 20, LabelHue3, 213 , m_Itemxml.Fletchinggainamount.ToString());
				AddButton(430	, 339, 4005, 4006, GetButtonID( 7, 63 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 22), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
				
				AddButton(450, 379, 4005, 4007, GetButtonID(1, 24), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
			}
			if (m_Cat == LevelCategory.ConfiguredSkillsEXP4)
			{
				AddLabel(280, 117, LabelHue2, @"Skill Toggle");
				AddLabel(430, 117, LabelHue2, @"EXP Gained");
				
				AddLabel(317, 139, LabelHue2, @"Cooking");
				if (m_Itemxml.Cookinggain == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 7, 31 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Cookinggain == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 7, 31 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 139, 25, 20, LabelHue3, 214 , m_Itemxml.Cookinggainamount.ToString());
				AddButton(430	, 139, 4005, 4006, GetButtonID( 7, 64 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 161, LabelHue2, @"Inscribe");
				if (m_Itemxml.Inscribegain == true)
					AddButton(280, 161, 4006, 4005, GetButtonID( 7, 32 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Inscribegain == false)
					AddButton(280, 161, 4005, 4006, GetButtonID( 7, 32 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 161, 25, 20, LabelHue3, 215 , m_Itemxml.Inscribegainamount.ToString());
				AddButton(430	, 161, 4005, 4006, GetButtonID( 7, 65 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 183, LabelHue2, @"Tailoring");
				if (m_Itemxml.Tailoringgain == true)
					AddButton(280, 183, 4006, 4005, GetButtonID( 7, 33 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Tailoringgain == false)
					AddButton(280, 183, 4005, 4006, GetButtonID( 7, 33 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 183, 25, 20, LabelHue3, 216 , m_Itemxml.Tailoringgainamount.ToString());
				AddButton(430	, 183, 4005, 4006, GetButtonID( 7, 66 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 205, LabelHue2, @"Tinkering");
				if (m_Itemxml.Tinkeringgain == true)
					AddButton(280, 205, 4006, 4005, GetButtonID( 7, 34 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Tinkeringgain == false)
					AddButton(280, 205, 4005, 4006, GetButtonID( 7, 34 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 205, 25, 20, LabelHue3, 217 , m_Itemxml.Tinkeringgainamount.ToString());
				AddButton(430	, 205, 4005, 4006, GetButtonID( 7, 67 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 23), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.LevelGainStatSP)
			{
				AddLabel(280, 117, LabelHue2, @"Skill Points Awarded Below X level.");

				AddLabel(317	, 139, LabelHue2, @"Below L20:");
				AddTextEntry(390, 139, 30, 20, LabelHue3, 218 , m_Itemxml.Below20.ToString());
				AddButton(280	, 139, 4005, 4006, GetButtonID( 8, 1 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 161, LabelHue2, @"Below L40:");
				AddTextEntry(390, 161, 30, 20, LabelHue3, 219 , m_Itemxml.Below40.ToString());
				AddButton(280	, 161, 4005, 4006, GetButtonID( 8, 2 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 183, LabelHue2, @"Below L60:");
				AddTextEntry(390, 183, 30, 20, LabelHue3, 220 , m_Itemxml.Below60.ToString());
				AddButton(280	, 183, 4005, 4006, GetButtonID( 8, 3 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 205, LabelHue2, @"Below L70:");
				AddTextEntry(390, 205, 30, 20, LabelHue3, 221 , m_Itemxml.Below70.ToString());
				AddButton(280	, 205, 4005, 4006, GetButtonID( 8, 4 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 227, LabelHue2, @"Below L80:");
				AddTextEntry(390, 227, 30, 20, LabelHue3, 222 , m_Itemxml.Below80.ToString());
				AddButton(280	, 227, 4005, 4006, GetButtonID( 8, 5 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 249, LabelHue2, @"Below L90:");
				AddTextEntry(390, 249, 30, 20, LabelHue3, 223 , m_Itemxml.Below90.ToString());
				AddButton(280	, 249, 4005, 4006, GetButtonID( 8, 6 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 271, LabelHue2, @"Below L100:");
				AddTextEntry(390, 271, 30, 20, LabelHue3, 224 , m_Itemxml.Below100.ToString());
				AddButton(280	, 271, 4005, 4006, GetButtonID( 8, 7 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 293, LabelHue2, @"Below L110:");
				AddTextEntry(390, 293, 30, 20, LabelHue3, 225 , m_Itemxml.Below110.ToString());
				AddButton(280	, 293, 4005, 4006, GetButtonID( 8, 8 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 317, LabelHue2, @"Below L120:");
				AddTextEntry(390, 317, 30, 20, LabelHue3, 226 , m_Itemxml.Below120.ToString());
				AddButton(280	, 317, 4005, 4006, GetButtonID( 8, 9 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 339, LabelHue2, @"Below L130:");
				AddTextEntry(390, 339, 30, 20, LabelHue3, 227 , m_Itemxml.Below130.ToString());
				AddButton(280	, 339, 4005, 4006, GetButtonID( 8, 10 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 139, LabelHue2, @"Below L140:");
				AddTextEntry(536, 139, 30, 20, LabelHue3, 228 , m_Itemxml.Below140.ToString());
				AddButton(426	, 139, 4005, 4006, GetButtonID( 8, 11 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 161, LabelHue2, @"Below L150:");
				AddTextEntry(536, 161, 30, 20, LabelHue3, 229 , m_Itemxml.Below150.ToString());
				AddButton(426	, 161, 4005, 4006, GetButtonID( 8, 12 ), GumpButtonType.Reply, 0);	
				
				AddLabel(463	, 183, LabelHue2, @"Below L160:");
				AddTextEntry(536, 183, 30, 20, LabelHue3, 230 , m_Itemxml.Below160.ToString());
				AddButton(426	, 183, 4005, 4006, GetButtonID( 8, 13 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 205, LabelHue2, @"Below L170:");
				AddTextEntry(536, 205, 30, 20, LabelHue3, 231 , m_Itemxml.Below170.ToString());
				AddButton(426	, 205, 4005, 4006, GetButtonID( 8, 14 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 227, LabelHue2, @"Below L180:");
				AddTextEntry(536, 227, 30, 20, LabelHue3, 232 , m_Itemxml.Below180.ToString());
				AddButton(426	, 227, 4005, 4006, GetButtonID( 8, 15 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 249, LabelHue2, @"Below L190:");
				AddTextEntry(536, 249, 30, 20, LabelHue3, 233 , m_Itemxml.Below190.ToString());
				AddButton(426	, 249, 4005, 4006, GetButtonID( 8, 16 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 271, LabelHue2, @"Below L200:");
				AddTextEntry(536, 271, 30, 20, LabelHue3, 234 , m_Itemxml.Below200.ToString());
				AddButton(426	, 271, 4005, 4006, GetButtonID( 8, 17 ), GumpButtonType.Reply, 0);	
				
				AddButton(308, 379, 4005, 4007, GetButtonID(3, 4), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			
			if (m_Cat == LevelCategory.LevelGainStatSP2)
			{
				AddLabel(280, 117, LabelHue2, @"Stat Points Awarded Below X level.");

				AddLabel(317	, 139, LabelHue2, @"Below L20:");
				AddTextEntry(390, 139, 30, 20, LabelHue3, 235 , m_Itemxml.Below20stat.ToString());
				AddButton(280	, 139, 4005, 4006, GetButtonID( 8, 18 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 161, LabelHue2, @"Below L40:");
				AddTextEntry(390, 161, 30, 20, LabelHue3, 236 , m_Itemxml.Below40stat.ToString());
				AddButton(280	, 161, 4005, 4006, GetButtonID( 8, 19 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 183, LabelHue2, @"Below L60:");
				AddTextEntry(390, 183, 30, 20, LabelHue3, 237 , m_Itemxml.Below60stat.ToString());
				AddButton(280	, 183, 4005, 4006, GetButtonID( 8, 20 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 205, LabelHue2, @"Below L70:");
				AddTextEntry(390, 205, 30, 20, LabelHue3, 238 , m_Itemxml.Below70stat.ToString());
				AddButton(280	, 205, 4005, 4006, GetButtonID( 8, 21 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 227, LabelHue2, @"Below L80:");
				AddTextEntry(390, 227, 30, 20, LabelHue3, 239 , m_Itemxml.Below80stat.ToString());
				AddButton(280	, 227, 4005, 4006, GetButtonID( 8, 22 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 249, LabelHue2, @"Below L90:");
				AddTextEntry(390, 249, 30, 20, LabelHue3, 240 , m_Itemxml.Below90stat.ToString());
				AddButton(280	, 249, 4005, 4006, GetButtonID( 8, 23 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 271, LabelHue2, @"Below L100:");
				AddTextEntry(390, 271, 30, 20, LabelHue3, 241 , m_Itemxml.Below100stat.ToString());
				AddButton(280	, 271, 4005, 4006, GetButtonID( 8, 24 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 293, LabelHue2, @"Below L110:");
				AddTextEntry(390, 293, 30, 20, LabelHue3, 242 , m_Itemxml.Below110stat.ToString());
				AddButton(280	, 293, 4005, 4006, GetButtonID( 8, 25 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 317, LabelHue2, @"Below L120:");
				AddTextEntry(390, 317, 30, 20, LabelHue3, 243 , m_Itemxml.Below120stat.ToString());
				AddButton(280	, 317, 4005, 4006, GetButtonID( 8, 26 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 339, LabelHue2, @"Below L130:");
				AddTextEntry(390, 339, 30, 20, LabelHue3, 244 , m_Itemxml.Below130stat.ToString());
				AddButton(280	, 339, 4005, 4006, GetButtonID( 8, 27 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 139, LabelHue2, @"Below L140:");
				AddTextEntry(536, 139, 30, 20, LabelHue3, 245 , m_Itemxml.Below140stat.ToString());
				AddButton(426	, 139, 4005, 4006, GetButtonID( 8, 28 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 161, LabelHue2, @"Below L150:");
				AddTextEntry(536, 161, 30, 20, LabelHue3, 246 , m_Itemxml.Below150stat.ToString());
				AddButton(426	, 161, 4005, 4006, GetButtonID( 8, 29 ), GumpButtonType.Reply, 0);	
				
				AddLabel(463	, 183, LabelHue2, @"Below L160:");
				AddTextEntry(536, 183, 30, 20, LabelHue3, 247 , m_Itemxml.Below160stat.ToString());
				AddButton(426	, 183, 4005, 4006, GetButtonID( 8, 30 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 205, LabelHue2, @"Below L170:");
				AddTextEntry(536, 205, 30, 20, LabelHue3, 248 , m_Itemxml.Below170stat.ToString());
				AddButton(426	, 205, 4005, 4006, GetButtonID( 8, 31 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 227, LabelHue2, @"Below L180:");
				AddTextEntry(536, 227, 30, 20, LabelHue3, 249 , m_Itemxml.Below180stat.ToString());
				AddButton(426	, 227, 4005, 4006, GetButtonID( 8, 32 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 249, LabelHue2, @"Below L190:");
				AddTextEntry(536, 249, 30, 20, LabelHue3, 250 , m_Itemxml.Below190stat.ToString());
				AddButton(426	, 249, 4005, 4006, GetButtonID( 8, 33 ), GumpButtonType.Reply, 0);
				
				AddLabel(463	, 271, LabelHue2, @"Below L200:");
				AddTextEntry(536, 271, 30, 20, LabelHue3, 251 , m_Itemxml.Below200stat.ToString());
				AddButton(426	, 271, 4005, 4006, GetButtonID( 8, 34 ), GumpButtonType.Reply, 0);	
				
				AddButton(308, 379, 4005, 4007, GetButtonID(3, 4), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			
			if (m_Cat == LevelCategory.GainFollowers)
			{
				AddLabel(280, 117, LabelHue2, @"Gain Slot Toggle");
				AddLabel(430, 117, LabelHue2, @"Follow Slots Per Level");

				AddLabel(317, 139, LabelHue2, @"Master Toggle - Gain Followers on Level");
				if (m_Itemxml.GainFollowerSlotOnLevel == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 8, 35 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowerSlotOnLevel == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 8, 35 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 161, LabelHue2, @"Gain On L20");
				if (m_Itemxml.GainFollowOn20 == true)
					AddButton(280, 161, 4006, 4005, GetButtonID( 8, 36 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn20 == false)
					AddButton(280, 161, 4005, 4006, GetButtonID( 8, 36 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 161, 25, 20, LabelHue3, 252 , m_Itemxml.GainFollowerSlotonLeveL20.ToString());
				AddButton(430	, 161, 4005, 4006, GetButtonID( 8, 55 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 183, LabelHue2, @"Gain On L30");
				if (m_Itemxml.GainFollowOn30 == true)
					AddButton(280, 183, 4006, 4005, GetButtonID( 8, 37 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn30 == false)
					AddButton(280, 183, 4005, 4006, GetButtonID( 8, 37 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 183, 25, 20, LabelHue3, 253 , m_Itemxml.GainFollowerSlotonLeveL30.ToString());
				AddButton(430	, 183, 4005, 4006, GetButtonID( 8, 56 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 205, LabelHue2, @"Gain On L40");
				if (m_Itemxml.GainFollowOn40 == true)
					AddButton(280, 205, 4006, 4005, GetButtonID( 8, 38 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn40 == false)
					AddButton(280, 205, 4005, 4006, GetButtonID( 8, 38 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 205, 25, 20, LabelHue3, 254 , m_Itemxml.GainFollowerSlotonLeveL40.ToString());
				AddButton(430	, 205, 4005, 4006, GetButtonID( 8, 57 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 227, LabelHue2, @"Gain On L50");
				if (m_Itemxml.GainFollowOn50 == true)
					AddButton(280, 227, 4006, 4005, GetButtonID( 8, 39 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn50 == false)
					AddButton(280, 227, 4005, 4006, GetButtonID( 8, 39 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 227, 25, 20, LabelHue3, 255 , m_Itemxml.GainFollowerSlotonLeveL50.ToString());
				AddButton(430	, 227, 4005, 4006, GetButtonID( 8, 58 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 249, LabelHue2, @"Gain On L60");
				if (m_Itemxml.GainFollowOn60 == true)
					AddButton(280, 249, 4006, 4005, GetButtonID( 8, 40 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn60 == false)
					AddButton(280, 249, 4005, 4006, GetButtonID( 8, 40 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 249, 25, 20, LabelHue3, 256 , m_Itemxml.GainFollowerSlotonLeveL60.ToString());
				AddButton(430	, 249, 4005, 4006, GetButtonID( 8, 59 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 271, LabelHue2, @"Gain On L70");
				if (m_Itemxml.GainFollowOn70 == true)
					AddButton(280, 271, 4006, 4005, GetButtonID( 8, 41 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn70 == false)
					AddButton(280, 271, 4005, 4006, GetButtonID( 8, 41 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 271, 25, 20, LabelHue3, 257 , m_Itemxml.GainFollowerSlotonLeveL70.ToString());
				AddButton(430	, 271, 4005, 4006, GetButtonID( 8, 60 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 293, LabelHue2, @"Gain On L80");
				if (m_Itemxml.GainFollowOn80 == true)
					AddButton(280, 293, 4006, 4005, GetButtonID( 8, 42 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn80 == false)
					AddButton(280, 293, 4005, 4006, GetButtonID( 8, 42 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 293, 25, 20, LabelHue3, 258 , m_Itemxml.GainFollowerSlotonLeveL80.ToString());
				AddButton(430	, 293, 4005, 4006, GetButtonID( 8, 61 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 317, LabelHue2, @"Gain On L90");
				if (m_Itemxml.GainFollowOn90 == true)
					AddButton(280, 317, 4006, 4005, GetButtonID( 8, 43 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn90 == false)
					AddButton(280, 317, 4005, 4006, GetButtonID( 8, 43 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 317, 25, 20, LabelHue3, 259 , m_Itemxml.GainFollowerSlotonLeveL90.ToString());
				AddButton(430	, 317, 4005, 4006, GetButtonID( 8, 62 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 339, LabelHue2, @"Gain On L100");
				if (m_Itemxml.GainFollowOn100 == true)
					AddButton(280, 339, 4006, 4005, GetButtonID( 8, 44 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn100 == false)
					AddButton(280, 339, 4005, 4006, GetButtonID( 8, 44 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 339, 25, 20, LabelHue3, 260 , m_Itemxml.GainFollowerSlotonLeveL100.ToString());
				AddButton(430	, 339, 4005, 4006, GetButtonID( 8, 63 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
				
				AddButton(450, 379, 4005, 4007, GetButtonID(1, 28), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
			}
			if (m_Cat == LevelCategory.GainFollowers2)
			{
				AddLabel(280, 117, LabelHue2, @"Gain Slot Toggle");
				AddLabel(430, 117, LabelHue2, @"Follow Slots Per Level");
				
				AddLabel(317, 139, LabelHue2, @"Gain On L110");
				if (m_Itemxml.GainFollowOn110 == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 8, 45 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn110 == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 8, 45 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 139, 25, 20, LabelHue3, 261 , m_Itemxml.GainFollowerSlotonLeveL110.ToString());
				AddButton(430	, 139, 4005, 4006, GetButtonID( 8, 64 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 161, LabelHue2, @"Gain On L120");
				if (m_Itemxml.GainFollowOn120 == true)
					AddButton(280, 161, 4006, 4005, GetButtonID( 8, 46 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn120 == false)
					AddButton(280, 161, 4005, 4006, GetButtonID( 8, 46 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 161, 25, 20, LabelHue3, 262 , m_Itemxml.GainFollowerSlotonLeveL120.ToString());
				AddButton(430	, 161, 4005, 4006, GetButtonID( 8, 65 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 183, LabelHue2, @"Gain On L130");
				if (m_Itemxml.GainFollowOn130 == true)
					AddButton(280, 183, 4006, 4005, GetButtonID( 8, 47 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn130 == false)
					AddButton(280, 183, 4005, 4006, GetButtonID( 8, 47 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 183, 25, 20, LabelHue3, 263 , m_Itemxml.GainFollowerSlotonLeveL130.ToString());
				AddButton(430	, 183, 4005, 4006, GetButtonID( 8, 66 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 205, LabelHue2, @"Gain On L140");
				if (m_Itemxml.GainFollowOn140 == true)
					AddButton(280, 205, 4006, 4005, GetButtonID( 8, 48 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn140 == false)
					AddButton(280, 205, 4005, 4006, GetButtonID( 8, 48 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 205, 25, 20, LabelHue3, 264 , m_Itemxml.GainFollowerSlotonLeveL140.ToString());
				AddButton(430	, 205, 4005, 4006, GetButtonID( 8, 67 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 227, LabelHue2, @"Gain On L150");
				if (m_Itemxml.GainFollowOn150 == true)
					AddButton(280, 227, 4006, 4005, GetButtonID( 8, 49 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn150 == false)
					AddButton(280, 227, 4005, 4006, GetButtonID( 8, 49 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 227, 25, 20, LabelHue3, 265 , m_Itemxml.GainFollowerSlotonLeveL150.ToString());
				AddButton(430	, 227, 4005, 4006, GetButtonID( 8, 68 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 249, LabelHue2, @"Gain On L160");
				if (m_Itemxml.GainFollowOn160 == true)
					AddButton(280, 249, 4006, 4005, GetButtonID( 8, 50 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn160 == false)
					AddButton(280, 249, 4005, 4006, GetButtonID( 8, 50 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 249, 25, 20, LabelHue3, 266 , m_Itemxml.GainFollowerSlotonLeveL160.ToString());
				AddButton(430	, 249, 4005, 4006, GetButtonID( 8, 69 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 271, LabelHue2, @"Gain On L170");
				if (m_Itemxml.GainFollowOn170 == true)
					AddButton(280, 271, 4006, 4005, GetButtonID( 8, 51 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn170 == false)
					AddButton(280, 271, 4005, 4006, GetButtonID( 8, 51 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 271, 25, 20, LabelHue3, 267 , m_Itemxml.GainFollowerSlotonLeveL170.ToString());
				AddButton(430	, 271, 4005, 4006, GetButtonID( 8, 70 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 293, LabelHue2, @"Gain On L180");
				if (m_Itemxml.GainFollowOn180 == true)
					AddButton(280, 293, 4006, 4005, GetButtonID( 8, 52 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn180 == false)
					AddButton(280, 293, 4005, 4006, GetButtonID( 8, 52 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 293, 25, 20, LabelHue3, 268 , m_Itemxml.GainFollowerSlotonLeveL180.ToString());
				AddButton(430	, 293, 4005, 4006, GetButtonID( 8, 71 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 317, LabelHue2, @"Gain On L190");
				if (m_Itemxml.GainFollowOn190 == true)
					AddButton(280, 317, 4006, 4005, GetButtonID( 8, 53 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn190 == false)
					AddButton(280, 317, 4005, 4006, GetButtonID( 8, 53 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 317, 25, 20, LabelHue3, 269 , m_Itemxml.GainFollowerSlotonLeveL190.ToString());
				AddButton(430	, 317, 4005, 4006, GetButtonID( 8, 72 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 339, LabelHue2, @"Gain On L200");
				if (m_Itemxml.GainFollowOn200 == true)
					AddButton(280, 339, 4006, 4005, GetButtonID( 8, 54 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.GainFollowOn200 == false)
					AddButton(280, 339, 4005, 4006, GetButtonID( 8, 54 ), GumpButtonType.Reply, 0);	
				AddTextEntry(470, 339, 25, 20, LabelHue3, 270 , m_Itemxml.GainFollowerSlotonLeveL200.ToString());
				AddButton(430	, 339, 4005, 4006, GetButtonID( 8, 73 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 27), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			
			}
			if (m_Cat == LevelCategory.StartupPlayerHandler)
			{
				AddLabel(280, 117, LabelHue2, @"Start up Options for 'new' Toons.");

				AddLabel(317, 139, LabelHue2, @"Toggle - Starting Location For Players");
				if (m_Itemxml.NewStartingLocation == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 8, 74 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.NewStartingLocation == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 8, 74 ), GumpButtonType.Reply, 0);
				AddLabel(340, 161, LabelHue2, @"X");
				AddButton(305	, 161, 4005, 4006, GetButtonID( 8, 75 ), GumpButtonType.Reply, 0);
				AddTextEntry(352, 161, 30, 20, LabelHue3, 271 , m_Itemxml.X_variable.ToString());
				AddLabel(424, 161, LabelHue2, @"Y");
				AddButton(388	, 161, 4005, 4006, GetButtonID( 8, 76 ), GumpButtonType.Reply, 0);
				AddTextEntry(437, 161, 30, 20, LabelHue3, 272 , m_Itemxml.Y_variable.ToString());
				AddLabel(506, 161, LabelHue2, @"Z");
				AddButton(472	, 161, 4005, 4006, GetButtonID( 8, 77 ), GumpButtonType.Reply, 0);
				AddTextEntry(521, 161, 30, 20, LabelHue3, 273 , m_Itemxml.Z_variable.ToString());
				

				AddLabel(340, 183, LabelHue2, @"Trammel");
				if (m_Itemxml.MapBoolTrammel == true)
					AddButton(305, 183, 4006, 4005, GetButtonID( 8, 117 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.MapBoolTrammel == false)
					AddButton(305, 183, 4005, 4006, GetButtonID( 8, 117 ), GumpButtonType.Reply, 0);
				
				AddLabel(424, 183, LabelHue2, @"Felucca");
				if (m_Itemxml.MapBoolFelucca == true)
					AddButton(388, 183, 4006, 4005, GetButtonID( 8, 118 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.MapBoolFelucca == false)
					AddButton(388, 183, 4005, 4006, GetButtonID( 8, 118 ), GumpButtonType.Reply, 0);
				
				AddLabel(506, 183, LabelHue2, @"Malas");
				if (m_Itemxml.MapBoolMalas == true)
					AddButton(472, 183, 4006, 4005, GetButtonID( 8, 119 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.MapBoolMalas == false)
					AddButton(472, 183, 4005, 4006, GetButtonID( 8, 119 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 227, LabelHue2, @"Toggle - Force New Players into Guild");
				if (m_Itemxml.ForceNewPlayerIntoGuild == true)
					AddButton(280, 227, 4006, 4005, GetButtonID( 8, 79 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.ForceNewPlayerIntoGuild == false)
					AddButton(280, 227, 4005, 4006, GetButtonID( 8, 79 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 249, LabelHue2, @"Guild Name:");
				AddTextEntry(390, 249, 120, 20, LabelHue3, 275 , m_Itemxml.Guildnamestart.ToString());
				AddButton(280	, 249, 4005, 4006, GetButtonID( 8, 80 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 293, LabelHue2, @"Toggle - Add items to new players backpack.");
				if (m_Itemxml.AddToBackpackOnAttach == true)
					AddButton(280, 293, 4006, 4005, GetButtonID( 8, 81 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.AddToBackpackOnAttach == false)
					AddButton(280, 293, 4005, 4006, GetButtonID( 8, 81 ), GumpButtonType.Reply, 0);
				
				//Left for later review.  This option is not used at this time. 
				//This WILL still be using a manual configuration. 
	//			AddLabel(317, 317, LabelHue2, @"Add to Backpack List");
	//			AddButton(280, 317, 4005, 4006, GetButtonID( 1, 30 ), GumpButtonType.Reply, 0);

				
				AddButton(450, 379, 4005, 4007, GetButtonID(1, 31), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.StartupPlayerHandler2)
			{
				AddLabel(280, 117, LabelHue2, @"Item");
				AddLabel(425, 117, LabelHue2, @"Listing");

				AddLabel(317	, 139, LabelHue2, @"Item 1:");
				AddTextEntry(380, 139, 80, 20, LabelHue3, 276 , m_Itemxml.Startitem1.ToString());
				AddButton(280	, 139, 4005, 4006, GetButtonID( 8, 82 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 161, LabelHue2, @"Item 2:");
				AddTextEntry(380, 161, 80, 20, LabelHue3, 277 , m_Itemxml.Startitem2.ToString());
				AddButton(280	, 161, 4005, 4006, GetButtonID( 8, 83 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 183, LabelHue2, @"Item 3:");
				AddTextEntry(380, 183, 80, 20, LabelHue3, 278 , m_Itemxml.Startitem3.ToString());
				AddButton(280	, 183, 4005, 4006, GetButtonID( 8, 84 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 205, LabelHue2, @"Item 4:");
				AddTextEntry(380, 205, 80, 20, LabelHue3, 279 , m_Itemxml.Startitem4.ToString());
				AddButton(280	, 205, 4005, 4006, GetButtonID( 8, 85 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 227, LabelHue2, @"Item 5:");
				AddTextEntry(380, 227, 80, 20, LabelHue3, 280 , m_Itemxml.Startitem5.ToString());
				AddButton(280	, 227, 4005, 4006, GetButtonID( 8, 86 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 249, LabelHue2, @"Item 6:");
				AddTextEntry(380, 249, 80, 20, LabelHue3, 281 , m_Itemxml.Startitem6.ToString());
				AddButton(280	, 249, 4005, 4006, GetButtonID( 8, 87 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 271, LabelHue2, @"Item 7:");
				AddTextEntry(380, 271, 80, 20, LabelHue3, 282 , m_Itemxml.Startitem7.ToString());
				AddButton(280	, 271, 4005, 4006, GetButtonID( 8, 88 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317	, 293, LabelHue2, @"Item 8:");
				AddTextEntry(380, 293, 80, 20, LabelHue3, 283 , m_Itemxml.Startitem8.ToString());
				AddButton(280	, 293, 4005, 4006, GetButtonID( 8, 89 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 317, LabelHue2, @"Item 9:");
				AddTextEntry(380, 317, 80, 20, LabelHue3, 284 , m_Itemxml.Startitem9.ToString());
				AddButton(280	, 317, 4005, 4006, GetButtonID( 8, 90 ), GumpButtonType.Reply, 0);
				
				AddLabel(317	, 339, LabelHue2, @"Item 10:");
				AddTextEntry(380, 339, 80, 20, LabelHue3, 285 , m_Itemxml.Startitem10.ToString());
				AddButton(280	, 339, 4005, 4006, GetButtonID( 8, 91 ), GumpButtonType.Reply, 0);
				
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 29), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.StartupPlayerHandler3)
			{
				AddLabel(317, 117, LabelHue2, @"Enable Starting Stats - Str Dex Int Below");
				if (m_Itemxml.Forcestartingstats == true)
					AddButton(280, 117, 4006, 4005, GetButtonID( 8, 92 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Forcestartingstats == false)
					AddButton(280, 117, 4005, 4006, GetButtonID( 8, 92 ), GumpButtonType.Reply, 0);
				AddLabel(317	, 139, LabelHue2, @"Starting Str: ");
				AddTextEntry(400, 139, 80, 20, LabelHue3, 286 , m_Itemxml.Forcestartingstatsstr.ToString());
				AddButton(280	, 139, 4005, 4006, GetButtonID( 8, 93 ), GumpButtonType.Reply, 0);
				AddLabel(485	, 139, LabelHue2, @"Cap: ");
				AddTextEntry(515, 139, 80, 20, LabelHue3, 287 , m_Itemxml.Startingstrcapvar.ToString());
				AddButton(450	, 139, 4005, 4006, GetButtonID( 8, 94 ), GumpButtonType.Reply, 0);
				AddLabel(317	, 161, LabelHue2, @"Starting Dex: ");
				AddTextEntry(400, 161, 80, 20, LabelHue3, 288 , m_Itemxml.Forcestartingstatsdex.ToString());
				AddButton(280	, 161, 4005, 4006, GetButtonID( 8, 95 ), GumpButtonType.Reply, 0);	
				AddLabel(485	, 161, LabelHue2, @"Cap: ");
				AddTextEntry(515, 161, 80, 20, LabelHue3, 289 , m_Itemxml.Startingdexcapvar.ToString());
				AddButton(450	, 161, 4005, 4006, GetButtonID( 8, 96 ), GumpButtonType.Reply, 0);
				AddLabel(317	, 183, LabelHue2, @"Starting Int: ");
				AddTextEntry(400, 183, 80, 20, LabelHue3, 290 , m_Itemxml.Forcestartingstatsint.ToString());
				AddButton(280	, 183, 4005, 4006, GetButtonID( 8, 97 ), GumpButtonType.Reply, 0);
				AddLabel(485	, 183, LabelHue2, @"Cap: ");
				AddTextEntry(515, 183, 80, 20, LabelHue3, 291 , m_Itemxml.Startingintcapvar.ToString());
				AddButton(450	, 183, 4005, 4006, GetButtonID( 8, 98 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 227, LabelHue2, @"Toggle Skill Cap");
				if (m_Itemxml.Autoactivate_skillscap == true)
					AddButton(280, 227, 4006, 4005, GetButtonID( 8, 99 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_skillscap == false)
					AddButton(280, 227, 4005, 4006, GetButtonID( 8, 99 ), GumpButtonType.Reply, 0);	
				AddLabel(485	, 227, LabelHue2, @"Cap: ");
				AddTextEntry(515, 227, 80, 20, LabelHue3, 292 , m_Itemxml.Autoactivate_skillscapvar.ToString());
				AddButton(450	, 227, 4005, 4006, GetButtonID( 8, 100 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317, 249, LabelHue2, @"Toggle Follow Slots");
				if (m_Itemxml.Autoactivate_maxfollowslots == true)
					AddButton(280, 249, 4006, 4005, GetButtonID( 8, 101 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_maxfollowslots == false)
					AddButton(280, 249, 4005, 4006, GetButtonID( 8, 101 ), GumpButtonType.Reply, 0);	
				AddLabel(485	, 249, LabelHue2, @"Cap: ");
				AddTextEntry(515, 249, 80, 20, LabelHue3, 293 , m_Itemxml.Autoactivate_maxfollowslotstotal.ToString());
				AddButton(450	, 249, 4005, 4006, GetButtonID( 8, 102 ), GumpButtonType.Reply, 0);	
				
				AddButton(450, 379, 4005, 4007, GetButtonID(1, 32), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 29), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.StartupPlayerHandler4)
			{
				AddLabel(280, 117, LabelHue2, @"Additional Option Toggles");
				
				AddLabel(317, 139, LabelHue2, @"Is Young");
				if (m_Itemxml.Autoactivate_isyoung == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 8, 103 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_isyoung == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 8, 103 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 161, LabelHue2, @"Mechanical Life");
				if (m_Itemxml.Autoactivate_mechanicallife == true)
					AddButton(280, 161, 4006, 4005, GetButtonID( 8, 104 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_mechanicallife == false)
					AddButton(280, 161, 4005, 4006, GetButtonID( 8, 104 ), GumpButtonType.Reply, 0);
	
				AddLabel(317, 183, LabelHue2, @"Cant Walk");
				if (m_Itemxml.Autoactivate_cantwalk == true)
					AddButton(280, 183, 4006, 4005, GetButtonID( 8, 105 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_cantwalk == false)
					AddButton(280, 183, 4005, 4006, GetButtonID( 8, 105 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 205, LabelHue2, @"Disabled Pvp Warning");
				if (m_Itemxml.Autoactivate_disabledpvpwarning == true)
					AddButton(280, 205, 4006, 4005, GetButtonID( 8, 106 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_disabledpvpwarning == false)
					AddButton(280, 205, 4005, 4006, GetButtonID( 8, 106 ), GumpButtonType.Reply, 0);

				AddLabel(317, 227, LabelHue2, @"Gem Mining");
				if (m_Itemxml.Autoactivate_gemmining == true)
					AddButton(280, 227, 4006, 4005, GetButtonID( 8, 107 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_gemmining == false)
					AddButton(280, 227, 4005, 4006, GetButtonID( 8, 107 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 249, LabelHue2, @"Basket Weaving");
				if (m_Itemxml.Autoactivate_basketweaving == true)
					AddButton(280, 249, 4006, 4005, GetButtonID( 8, 108 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_basketweaving == false)
					AddButton(280, 249, 4005, 4006, GetButtonID( 8, 108 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 271, LabelHue2, @"Can Buy Carpets");
				if (m_Itemxml.Autoactivate_canbuycarpets == true)
					AddButton(280, 271, 4006, 4005, GetButtonID( 8, 109 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_canbuycarpets == false)
					AddButton(280, 271, 4005, 4006, GetButtonID( 8, 109 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317, 293, LabelHue2, @"Accept Guild Invites");
				if (m_Itemxml.Autoactivate_acceptguildinvites == true)
					AddButton(280, 293, 4006, 4005, GetButtonID( 8, 110 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_acceptguildinvites == false)
					AddButton(280, 293, 4005, 4006, GetButtonID( 8, 110 ), GumpButtonType.Reply, 0);	

				AddLabel(317, 317, LabelHue2, @"Glass Blowing");
				if (m_Itemxml.Autoactivate_glassblowing == true)
					AddButton(280, 317, 4006, 4005, GetButtonID( 8, 111 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_glassblowing == false)
					AddButton(280, 317, 4005, 4006, GetButtonID( 8, 111 ), GumpButtonType.Reply, 0);	
				
				AddLabel(317, 339, LabelHue2, @"Library Friend");
				if (m_Itemxml.Autoactivate_libraryfriend == true)
					AddButton(280, 339, 4006, 4005, GetButtonID( 8, 112 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_libraryfriend == false)
					AddButton(280, 339, 4005, 4006, GetButtonID( 8, 112 ), GumpButtonType.Reply, 0);	
				
				AddButton(450, 379, 4005, 4007, GetButtonID(1, 33), GumpButtonType.Reply, 0);
				AddLabel(490, 380, LabelHue2, @"Next Page");
				AddButton(308, 379, 4005, 4007, GetButtonID(1, 31), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.StartupPlayerHandler5)
			{
				AddLabel(317, 117, LabelHue2, @"Masonry");
				if (m_Itemxml.Autoactivate_masonry == true)
					AddButton(280, 117, 4006, 4005, GetButtonID( 8, 113 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_masonry == false)
					AddButton(280, 117, 4005, 4006, GetButtonID( 8, 113 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 139, LabelHue2, @"Sand Mining");
				if (m_Itemxml.Autoactivate_sandmining == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 8, 114 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_sandmining == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 8, 114 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 161, LabelHue2, @"Stone Mining");
				if (m_Itemxml.Autoactivate_stonemining == true)
					AddButton(280, 161, 4006, 4005, GetButtonID( 8, 115 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_stonemining == false)
					AddButton(280, 161, 4005, 4006, GetButtonID( 8, 115 ), GumpButtonType.Reply, 0);		
				
				AddLabel(317, 183, LabelHue2, @"Spell Weaving");
				if (m_Itemxml.Autoactivate_spellweaving == true)
					AddButton(280, 183, 4006, 4005, GetButtonID( 8, 116 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Autoactivate_spellweaving == false)
					AddButton(280, 183, 4005, 4006, GetButtonID( 8, 116 ), GumpButtonType.Reply, 0);
				

				AddButton(308, 379, 4005, 4007, GetButtonID(1, 32), GumpButtonType.Reply, 0);
				AddLabel(345, 380, LabelHue2, @"Previous Page");
//				AddButton(450, 379, 4005, 4007, GetButtonID(1, 14), GumpButtonType.Reply, 0);
//				AddLabel(490, 380, LabelHue2, @"Next Page");
			}
			if (m_Cat == LevelCategory.LevelBag)
			{
				//This is under review for future integration, for now external config still being used!
				
				AddLabel(317, 117, LabelHue2, @"Master Toggle - Turn Off Bag Level System");
				if (m_Itemxml.Bagsystemmaintoggle == true)
					AddButton(280, 117, 4006, 4005, GetButtonID( 9, 1 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Bagsystemmaintoggle == false)
					AddButton(280, 117, 4005, 4006, GetButtonID( 9, 1 ), GumpButtonType.Reply, 0);
				
				AddLabel(317, 139, LabelHue2, @"Group 1");
				if (m_Itemxml.Levelgroup1 == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 9, 2 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Levelgroup1 == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 9, 2 ), GumpButtonType.Reply, 0);	
				
				AddButton(420,		139, 4005, 4006, GetButtonID( 9, 3 ), GumpButtonType.Reply, 0);
				AddLabel(460,		139, LabelHue2, @"Req Level: ");
				AddTextEntry(525,	139, 25, 20, LabelHue3, 294 , m_Itemxml.Levelgroup1reqLevel.ToString());
				
				AddButton(280,		161, 4005, 4006, GetButtonID( 9, 4 ), GumpButtonType.Reply, 0);
				AddLabel(317,		161, LabelHue2, @"Max Items: ");
				AddTextEntry(390,	161, 25, 20, LabelHue3, 295 , m_Itemxml.Levelgroup1maxitems.ToString());
				
				AddButton(420,		161, 4005, 4006, GetButtonID( 9,5 ), GumpButtonType.Reply, 0);
				AddLabel(460,		161, LabelHue2, @"Reduce %: ");
				AddTextEntry(525,	161, 25, 20, LabelHue3, 296 , m_Itemxml.Levelgroup1reducetotal.ToString());
				
				AddButton(280,		183, 4005, 4006, GetButtonID( 9, 6 ), GumpButtonType.Reply, 0);
				AddLabel(317,		183, LabelHue2, @"Group Message: ");
				AddTextEntry(412,	183, 170, 38, LabelHue3, 297 , m_Itemxml.Levelgroup1msg.ToString());
				
				AddButton(280,		214, 4005, 4006, GetButtonID( 9, 7 ), GumpButtonType.Reply, 0);
				AddLabel(317,		214, LabelHue2, @"Owner Message: ");
				AddTextEntry(412,	214, 170, 38, LabelHue3, 298 , m_Itemxml.Level1groupownermsg.ToString());
				
				AddLabel(317, 271, LabelHue2, @"Group 2");
				if (m_Itemxml.Levelgroup2 == true)
					AddButton(280, 271, 4006, 4005, GetButtonID( 9, 8 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Levelgroup2 == false)
					AddButton(280, 271, 4005, 4006, GetButtonID( 9, 8 ), GumpButtonType.Reply, 0);	
				
				
				AddButton(420,		271, 4005, 4006, GetButtonID( 9, 9 ), GumpButtonType.Reply, 0);
				AddLabel(460,		271, LabelHue2, @"Req Level: ");
				AddTextEntry(525,	271, 25, 20, LabelHue3, 299 , m_Itemxml.Levelgroup2reqLevel.ToString());
				
				AddButton(280,		293, 4005, 4006, GetButtonID( 9, 10 ), GumpButtonType.Reply, 0);
				AddLabel(317,		293, LabelHue2, @"Max Items: ");
				AddTextEntry(390,	293, 25, 20, LabelHue3, 300 , m_Itemxml.Levelgroup2maxitems.ToString());
				
				AddButton(420,		293, 4005, 4006, GetButtonID( 9,11 ), GumpButtonType.Reply, 0);
				AddLabel(460,		293, LabelHue2, @"Reduce %: ");
				AddTextEntry(525,	293, 25, 20, LabelHue3, 301 , m_Itemxml.Levelgroup2reducetotal.ToString());
				
				AddButton(280,		317, 4005, 4006, GetButtonID( 9, 12 ), GumpButtonType.Reply, 0);
				AddLabel(317,		317, LabelHue2, @"Group Message: ");
				AddTextEntry(412,	317, 170, 38, LabelHue3, 302 , m_Itemxml.Levelgroup2msg.ToString());
				
				AddButton(280,		348, 4005, 4006, GetButtonID( 9, 13 ), GumpButtonType.Reply, 0);
				AddLabel(317,		348, LabelHue2, @"Owner Message: ");
				AddTextEntry(412,	348, 170, 38, LabelHue3, 303 , m_Itemxml.Level2groupownermsg.ToString());
				

				AddButton(308, 381, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(345, 383, LabelHue2, @"Previous Page");
				AddButton(450, 381, 4005, 4007, GetButtonID(1, 36), GumpButtonType.Reply, 0);
				AddLabel(490, 383, LabelHue2, @"Next Page");
				
				
			}
			if (m_Cat == LevelCategory.LevelBag2)
			{				
				AddLabel(317, 117, LabelHue2, @"Master Toggle - Able to Drop?");
				if (m_Itemxml.Preventbagdrop == true)
					AddButton(280, 117, 4006, 4005, GetButtonID( 9, 0 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Preventbagdrop == false)
					AddButton(280, 117, 4005, 4006, GetButtonID( 9, 0 ), GumpButtonType.Reply, 0);
				
				
				AddLabel(317, 139, LabelHue2, @"Group 3");
				if (m_Itemxml.Levelgroup3 == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 9, 14 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Levelgroup3 == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 9, 14 ), GumpButtonType.Reply, 0);	
				
				AddButton(420,		139, 4005, 4006, GetButtonID( 9, 15 ), GumpButtonType.Reply, 0);
				AddLabel(460,		139, LabelHue2, @"Req Level: ");
				AddTextEntry(525,	139, 25, 20, LabelHue3, 304 , m_Itemxml.Levelgroup3reqLevel.ToString());
				
				AddButton(280,		161, 4005, 4006, GetButtonID( 9, 16 ), GumpButtonType.Reply, 0);
				AddLabel(317,		161, LabelHue2, @"Max Items: ");
				AddTextEntry(390,	161, 25, 20, LabelHue3, 305 , m_Itemxml.Levelgroup3maxitems.ToString());
				
				AddButton(420,		161, 4005, 4006, GetButtonID( 9, 17 ), GumpButtonType.Reply, 0);
				AddLabel(460,		161, LabelHue2, @"Reduce %: ");
				AddTextEntry(525,	161, 25, 20, LabelHue3, 306 , m_Itemxml.Levelgroup3reducetotal.ToString());
				
				AddButton(280,		183, 4005, 4006, GetButtonID( 9, 18 ), GumpButtonType.Reply, 0);
				AddLabel(317,		183, LabelHue2, @"Group Message: ");
				AddTextEntry(412,	183, 170, 38, LabelHue3, 307 , m_Itemxml.Levelgroup3msg.ToString());
				
				AddButton(280,		214, 4005, 4006, GetButtonID( 9, 19 ), GumpButtonType.Reply, 0);
				AddLabel(317,		214, LabelHue2, @"Owner Message: ");
				AddTextEntry(412,	214, 170, 38, LabelHue3, 308 , m_Itemxml.Level3groupownermsg.ToString());

				AddLabel(317, 271, LabelHue2, @"Group 4");
				if (m_Itemxml.Levelgroup4 == true)
					AddButton(280, 271, 4006, 4005, GetButtonID( 9, 20 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Levelgroup4 == false)
					AddButton(280, 271, 4005, 4006, GetButtonID( 9, 20 ), GumpButtonType.Reply, 0);	
				
				
				AddButton(420,		271, 4005, 4006, GetButtonID( 9, 21 ), GumpButtonType.Reply, 0);
				AddLabel(460,		271, LabelHue2, @"Req Level: ");
				AddTextEntry(525,	271, 25, 20, LabelHue3, 309 , m_Itemxml.Levelgroup4reqLevel.ToString());
				
				AddButton(280,		293, 4005, 4006, GetButtonID( 9, 22 ), GumpButtonType.Reply, 0);
				AddLabel(317,		293, LabelHue2, @"Max Items: ");
				AddTextEntry(390,	293, 25, 20, LabelHue3, 310 , m_Itemxml.Levelgroup4maxitems.ToString());
				
				AddButton(420,		293, 4005, 4006, GetButtonID( 9, 23 ), GumpButtonType.Reply, 0);
				AddLabel(460,		293, LabelHue2, @"Reduce %: ");
				AddTextEntry(525,	293, 25, 20, LabelHue3, 311 , m_Itemxml.Levelgroup4reducetotal.ToString());
				
				AddButton(280,		317, 4005, 4006, GetButtonID( 9, 24 ), GumpButtonType.Reply, 0);
				AddLabel(317,		317, LabelHue2, @"Group Message: ");
				AddTextEntry(412,	317, 170, 38, LabelHue3, 312 , m_Itemxml.Levelgroup4msg.ToString());
				
				AddButton(280,		348, 4005, 4006, GetButtonID( 9, 25 ), GumpButtonType.Reply, 0);
				AddLabel(317,		348, LabelHue2, @"Owner Message: ");
				AddTextEntry(412,	348, 170, 38, LabelHue3, 313 , m_Itemxml.Level4groupownermsg.ToString());
				

				AddButton(308, 381, 4005, 4007, GetButtonID(1, 34), GumpButtonType.Reply, 0);
				AddLabel(345, 383, LabelHue2, @"Previous Page");
				AddButton(450, 381, 4005, 4007, GetButtonID(1, 37), GumpButtonType.Reply, 0);
				AddLabel(490, 383, LabelHue2, @"Next Page");
			}
			if (m_Cat == LevelCategory.LevelBag3)
			{				
				AddLabel(317, 139, LabelHue2, @"Group 5");
				if (m_Itemxml.Levelgroup5 == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 9, 26 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Levelgroup5 == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 9, 26 ), GumpButtonType.Reply, 0);	
				
				AddButton(420,		139, 4005, 4006, GetButtonID( 9, 27 ), GumpButtonType.Reply, 0);
				AddLabel(460,		139, LabelHue2, @"Req Level: ");
				AddTextEntry(525,	139, 25, 20, LabelHue3, 314 , m_Itemxml.Levelgroup5reqLevel.ToString());
				
				AddButton(280,		161, 4005, 4006, GetButtonID( 9, 28 ), GumpButtonType.Reply, 0);
				AddLabel(317,		161, LabelHue2, @"Max Items: ");
				AddTextEntry(390,	161, 25, 20, LabelHue3, 315 , m_Itemxml.Levelgroup5maxitems.ToString());
				
				AddButton(420,		161, 4005, 4006, GetButtonID( 9, 29 ), GumpButtonType.Reply, 0);
				AddLabel(460,		161, LabelHue2, @"Reduce %: ");
				AddTextEntry(525,	161, 25, 20, LabelHue3, 316 , m_Itemxml.Levelgroup5reducetotal.ToString());
				
				AddButton(280,		183, 4005, 4006, GetButtonID( 9, 30 ), GumpButtonType.Reply, 0);
				AddLabel(317,		183, LabelHue2, @"Group Message: ");
				AddTextEntry(412,	183, 170, 38, LabelHue3, 317 , m_Itemxml.Levelgroup5msg.ToString());
				
				AddButton(280,		214, 4005, 4006, GetButtonID( 9, 31 ), GumpButtonType.Reply, 0);
				AddLabel(317,		214, LabelHue2, @"Owner Message: ");
				AddTextEntry(412,	214, 170, 38, LabelHue3, 318 , m_Itemxml.Level5groupownermsg.ToString());

				AddLabel(317, 271, LabelHue2, @"Group 6");
				if (m_Itemxml.Levelgroup6 == true)
					AddButton(280, 271, 4006, 4005, GetButtonID( 9, 32 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Levelgroup6 == false)
					AddButton(280, 271, 4005, 4006, GetButtonID( 9, 32 ), GumpButtonType.Reply, 0);	
				
				
				AddButton(420,		271, 4005, 4006, GetButtonID( 9, 33 ), GumpButtonType.Reply, 0);
				AddLabel(460,		271, LabelHue2, @"Req Level: ");
				AddTextEntry(525,	271, 25, 20, LabelHue3, 319 , m_Itemxml.Levelgroup6reqLevel.ToString());
				
				AddButton(280,		293, 4005, 4006, GetButtonID( 9, 34 ), GumpButtonType.Reply, 0);
				AddLabel(317,		293, LabelHue2, @"Max Items: ");
				AddTextEntry(390,	293, 25, 20, LabelHue3, 320 , m_Itemxml.Levelgroup6maxitems.ToString());
				
				AddButton(420,		293, 4005, 4006, GetButtonID( 9, 35 ), GumpButtonType.Reply, 0);
				AddLabel(460,		293, LabelHue2, @"Reduce %: ");
				AddTextEntry(525,	293, 25, 20, LabelHue3, 321 , m_Itemxml.Levelgroup6reducetotal.ToString());
				
				AddButton(280,		317, 4005, 4006, GetButtonID( 9, 36 ), GumpButtonType.Reply, 0);
				AddLabel(317,		317, LabelHue2, @"Group Message: ");
				AddTextEntry(412,	317, 170, 38, LabelHue3, 322 , m_Itemxml.Levelgroup6msg.ToString());
				
				AddButton(280,		348, 4005, 4006, GetButtonID( 9, 37 ), GumpButtonType.Reply, 0);
				AddLabel(317,		348, LabelHue2, @"Owner Message: ");
				AddTextEntry(412,	348, 170, 38, LabelHue3, 323 , m_Itemxml.Level6groupownermsg.ToString());
				

				AddButton(308, 381, 4005, 4007, GetButtonID(1, 36), GumpButtonType.Reply, 0);
				AddLabel(345, 383, LabelHue2, @"Previous Page");
				AddButton(450, 381, 4005, 4007, GetButtonID(1, 38), GumpButtonType.Reply, 0);
				AddLabel(490, 383, LabelHue2, @"Next Page");
			}
			if (m_Cat == LevelCategory.LevelBag4)
			{				
				AddLabel(317, 139, LabelHue2, @"Group 7");
				if (m_Itemxml.Levelgroup7 == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 9, 38 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Levelgroup7 == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 9, 38 ), GumpButtonType.Reply, 0);	
				
				AddButton(420,		139, 4005, 4006, GetButtonID( 9, 39 ), GumpButtonType.Reply, 0);
				AddLabel(460,		139, LabelHue2, @"Req Level: ");
				AddTextEntry(525,	139, 25, 20, LabelHue3, 324 , m_Itemxml.Levelgroup7reqLevel.ToString());
				
				AddButton(280,		161, 4005, 4006, GetButtonID( 9, 40 ), GumpButtonType.Reply, 0);
				AddLabel(317,		161, LabelHue2, @"Max Items: ");
				AddTextEntry(390,	161, 25, 20, LabelHue3, 325 , m_Itemxml.Levelgroup7maxitems.ToString());
				
				AddButton(420,		161, 4005, 4006, GetButtonID( 9,41 ), GumpButtonType.Reply, 0);
				AddLabel(460,		161, LabelHue2, @"Reduce %: ");
				AddTextEntry(525,	161, 25, 20, LabelHue3, 326 , m_Itemxml.Levelgroup7reducetotal.ToString());
				
				AddButton(280,		183, 4005, 4006, GetButtonID( 9, 42 ), GumpButtonType.Reply, 0);
				AddLabel(317,		183, LabelHue2, @"Group Message: ");
				AddTextEntry(412,	183, 170, 38, LabelHue3, 327 , m_Itemxml.Levelgroup7msg.ToString());
				
				AddButton(280,		214, 4005, 4006, GetButtonID( 9, 43 ), GumpButtonType.Reply, 0);
				AddLabel(317,		214, LabelHue2, @"Owner Message: ");
				AddTextEntry(412,	214, 170, 38, LabelHue3, 328 , m_Itemxml.Level7groupownermsg.ToString());

				AddLabel(317, 271, LabelHue2, @"Group 8");
				if (m_Itemxml.Levelgroup8 == true)
					AddButton(280, 271, 4006, 4005, GetButtonID( 9, 44 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Levelgroup8 == false)
					AddButton(280, 271, 4005, 4006, GetButtonID( 9, 44 ), GumpButtonType.Reply, 0);	
				
				
				AddButton(420,		271, 4005, 4006, GetButtonID( 9, 45 ), GumpButtonType.Reply, 0);
				AddLabel(460,		271, LabelHue2, @"Req Level: ");
				AddTextEntry(525,	271, 25, 20, LabelHue3, 329 , m_Itemxml.Levelgroup8reqLevel.ToString());
				
				AddButton(280,		293, 4005, 4006, GetButtonID( 9, 46 ), GumpButtonType.Reply, 0);
				AddLabel(317,		293, LabelHue2, @"Max Items: ");
				AddTextEntry(390,	293, 25, 20, LabelHue3, 330 , m_Itemxml.Levelgroup8maxitems.ToString());
				
				AddButton(420,		293, 4005, 4006, GetButtonID( 9,47 ), GumpButtonType.Reply, 0);
				AddLabel(460,		293, LabelHue2, @"Reduce %: ");
				AddTextEntry(525,	293, 25, 20, LabelHue3, 331 , m_Itemxml.Levelgroup8reducetotal.ToString());
				
				AddButton(280,		317, 4005, 4006, GetButtonID( 9, 48 ), GumpButtonType.Reply, 0);
				AddLabel(317,		317, LabelHue2, @"Group Message: ");
				AddTextEntry(412,	317, 170, 38, LabelHue3, 332 , m_Itemxml.Levelgroup8msg.ToString());
				
				AddButton(280,		348, 4005, 4006, GetButtonID( 9, 49 ), GumpButtonType.Reply, 0);
				AddLabel(317,		348, LabelHue2, @"Owner Message: ");
				AddTextEntry(412,	348, 170, 38, LabelHue3, 333 , m_Itemxml.Level8groupownermsg.ToString());
				

				AddButton(308, 381, 4005, 4007, GetButtonID(1, 37), GumpButtonType.Reply, 0);
				AddLabel(345, 383, LabelHue2, @"Previous Page");
				AddButton(450, 381, 4005, 4007, GetButtonID(1, 39), GumpButtonType.Reply, 0);
				AddLabel(490, 383, LabelHue2, @"Next Page");
			}
			if (m_Cat == LevelCategory.LevelBag5)
			{				
				AddLabel(317, 139, LabelHue2, @"Group 9");
				if (m_Itemxml.Levelgroup9 == true)
					AddButton(280, 139, 4006, 4005, GetButtonID( 9, 50 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Levelgroup9 == false)
					AddButton(280, 139, 4005, 4006, GetButtonID( 9, 50 ), GumpButtonType.Reply, 0);	
				
				AddButton(420,		139, 4005, 4006, GetButtonID( 9, 51 ), GumpButtonType.Reply, 0);
				AddLabel(460,		139, LabelHue2, @"Req Level: ");
				AddTextEntry(525,	139, 25, 20, LabelHue3, 334 , m_Itemxml.Levelgroup9reqLevel.ToString());
				
				AddButton(280,		161, 4005, 4006, GetButtonID( 9, 52 ), GumpButtonType.Reply, 0);
				AddLabel(317,		161, LabelHue2, @"Max Items: ");
				AddTextEntry(390,	161, 25, 20, LabelHue3, 335 , m_Itemxml.Levelgroup9maxitems.ToString());
				
				AddButton(420,		161, 4005, 4006, GetButtonID( 9,53 ), GumpButtonType.Reply, 0);
				AddLabel(460,		161, LabelHue2, @"Reduce %: ");
				AddTextEntry(525,	161, 25, 20, LabelHue3, 336 , m_Itemxml.Levelgroup9reducetotal.ToString());
				
				AddButton(280,		183, 4005, 4006, GetButtonID( 9, 54 ), GumpButtonType.Reply, 0);
				AddLabel(317,		183, LabelHue2, @"Group Message: ");
				AddTextEntry(412,	183, 170, 38, LabelHue3, 337 , m_Itemxml.Levelgroup9msg.ToString());
				
				AddButton(280,		214, 4005, 4006, GetButtonID( 9, 55 ), GumpButtonType.Reply, 0);
				AddLabel(317,		214, LabelHue2, @"Owner Message: ");
				AddTextEntry(412,	214, 170, 38, LabelHue3, 338 , m_Itemxml.Level9groupownermsg.ToString());

				AddLabel(317, 271, LabelHue2, @"Group 10");
				if (m_Itemxml.Levelgroup10 == true)
					AddButton(280, 271, 4006, 4005, GetButtonID( 9, 56 ), GumpButtonType.Reply, 0);
				if (m_Itemxml.Levelgroup10 == false)
					AddButton(280, 271, 4005, 4006, GetButtonID( 9, 56 ), GumpButtonType.Reply, 0);	
				
				
				AddButton(420,		271, 4005, 4006, GetButtonID( 9, 57 ), GumpButtonType.Reply, 0);
				AddLabel(460,		271, LabelHue2, @"Req Level: ");
				AddTextEntry(525,	271, 25, 20, LabelHue3, 339 , m_Itemxml.Levelgroup10reqLevel.ToString());
				
				AddButton(280,		293, 4005, 4006, GetButtonID( 9, 58 ), GumpButtonType.Reply, 0);
				AddLabel(317,		293, LabelHue2, @"Max Items: ");
				AddTextEntry(390,	293, 25, 20, LabelHue3, 340 , m_Itemxml.Levelgroup10maxitems.ToString());
				
				AddButton(420,		293, 4005, 4006, GetButtonID( 9,59 ), GumpButtonType.Reply, 0);
				AddLabel(460,		293, LabelHue2, @"Reduce %: ");
				AddTextEntry(525,	293, 25, 20, LabelHue3, 341 , m_Itemxml.Levelgroup10reducetotal.ToString());
				
				AddButton(280,		317, 4005, 4006, GetButtonID( 9, 60 ), GumpButtonType.Reply, 0);
				AddLabel(317,		317, LabelHue2, @"Group Message: ");
				AddTextEntry(412,	317, 170, 38, LabelHue3, 342 , m_Itemxml.Levelgroup10msg.ToString());
				
				AddButton(280,		348, 4005, 4006, GetButtonID( 9, 61 ), GumpButtonType.Reply, 0);
				AddLabel(317,		348, LabelHue2, @"Owner Message: ");
				AddTextEntry(412,	348, 170, 38, LabelHue3, 343 , m_Itemxml.Level10groupownermsg.ToString());
				

				AddButton(308, 381, 4005, 4007, GetButtonID(1, 38), GumpButtonType.Reply, 0);
				AddLabel(345, 383, LabelHue2, @"Previous Page");
			}
			/*Help Buttons*/
			if (m_Cat == LevelCategory.EquipReqAndIntensityHelp)
			{
				AddHtml( 230, 117, 300, 450, String.Format
			( 
				"<basefont color = #FFFFFF><basefont size=10><left>This caclculates all the varaibles on the equipment and adds a level to the equipment.  This is the required level the character must be in order to equip and or use the item.  If they do not meet the requirements a message will be provided.  The requirements can be adjusted. </left></basefont>"), false, false );
				
				AddButton(450, 381, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(490, 383, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.HelpPetStealing)
			{
				AddHtml( 230, 117, 300, 450, String.Format
			(
				"<basefont color = #FFFFFF><basefont size=10><left>This allows a player to use a special lock pick to steal pets from other players.  By default bonded pets cannot be stolen however there is a toggle to change this. </left></basefont>"), false, false );
				
				AddButton(450, 381, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(490, 383, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.HelpMountLevelCheck)
			{
				AddHtml( 230, 117, 300, 450, String.Format
			( 
				"<basefont color = #FFFFFF><basefont size=10><left>This will check a players level and if not sufficient will not permit the player to mount the pet.  These variables can be changed.  If your server has a pet that is not included, you will need to go into the core scripts and add your pet as well as add the pet entry on the config gump if you want it to be easy to change. </left></basefont>"), false, false );
				
				AddButton(450, 381, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(490, 383, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.HelpVendorDiscounts)
			{
				AddHtml( 230, 117, 300, 450, String.Format
			(
				"<basefont color = #FFFFFF><basefont size=10><left>This will grant discounts from special vendors to players with a certain level.  These vendors must be manually added and the script for each vendor to be configured to be stocked with your items. </left></basefont>"), false, false );
				
				AddButton(450, 381, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(490, 383, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.HelpExpGainSkills)
			{ 
				AddHtml( 230, 117, 300, 450, String.Format
			( 
				"<basefont color = #FFFFFF><basefont size=10><left>Skills when used and they gain will provide an exp bonus to players.  You can also disable skill gain and only grant exp and only build the skills based on levels and experience.</left></basefont>"), false, false );
				
				AddButton(450, 381, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(490, 383, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.HelpGainPetSlotLvl)
			{ 
				AddHtml( 230, 117, 300, 450, String.Format
			(
				"<basefont color = #FFFFFF><basefont size=10><left>Upon gaining the required leved a pet slot or more then one pet slot can be awarded.</left></basefont>"), false, false );
				
				AddButton(450, 381, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(490, 383, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.HelpPlayerStartUp)
			{ 
				AddHtml( 230, 117, 300, 450, String.Format
			(
				"<basefont color = #FFFFFF><basefont size=10><left>These options affect all new  players.  Exisitng players will not be included if this is turned on after their toons have been created!  The Add items to new players script is located in PlayerStatsConfig file.  Until the toggle is enabled in the control config window the script that has the items to add to packs will not be invoked! </left></basefont>"), false, false );
				
				AddButton(450, 381, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(490, 383, LabelHue2, @"Previous Page");
			}
			if (m_Cat == LevelCategory.HelpLevelBag)
			{ 
				AddHtml( 230, 117, 300, 450, String.Format
			(
				"<basefont color = #FFFFFF><basefont size=10><left>This is a unique bag that can lower the total weight of all items within it.  What is special is depending on the configured level parameters these reduction ammounts can vary.  Other options are also included.</left></basefont>"), false, false );
				
				AddButton(450, 381, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
				AddLabel(490, 383, LabelHue2, @"Previous Page");
			}
			

			
		}
		public static int GetButtonID( int type, int index )
		{
			return 1 + type + (index * 15);
		}

        public override void OnResponse(NetState sender, RelayInfo info)
        {
			Mobile from = sender.Mobile;
			
			m_Itemxml = (LevelControlSys)XmlAttachExt.FindAttachment(m_Item, typeof(LevelControlSys));
			
            if (info.ButtonID <= 0)
                return; // Canceled

            int buttonID = info.ButtonID - 1;
            int type = buttonID % 15;
            int index = buttonID / 15;

			
            switch (type)
            {
				case 0:
				{
					break;
				}
				/* Category Choices */
                case 1:
				{
					switch (index)
					{
						case 0:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
							break;
						}
						case 1:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
							break;
						}
						case 2:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DeletePlayerLevelItems));
							break;
						}
						case 3:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DeletePetlevelItems));
							break;
						}
						case 4:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;
						}
						case 5:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
							break;
						}
						case 6:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DynamicEquipmentLevels));
							break;
						}
						case 7:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;
						}
						case 8:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.EquipReqAndIntensityHelp));
							break;
						}
						case 9:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;
						}
						case 10:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;
						}
						case 11:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;
						}
						case 12:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
							break;
						}
						case 13:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;
						}
						case 14:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
							break;
						}
						case 15:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
							break;
						}
						case 16:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
							break;
						}
						case 17:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
							break;
						}
						case 18:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;
						}
						case 19:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Expansions));
							break;
						}
						case 20:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetStealing));
							break;
						}
						case 21:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
							break;
						}
						case 22:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
							break;
						}
 						case 23:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
							break;
						}
 						case 24:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
							break;
						}
 						case 25:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;
						}
 						case 26:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;
						}	
 						case 27:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
							break;
						}	
 						case 28:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
							break;
						}
 						case 29:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
							break;
						}
 						case 30:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
							break;
						}				
 						case 31:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
							break;
						}	
 						case 32:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
							break;
						}
 						case 33:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler5));
							break;
						}	
						case 34:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
							break;
						}
						case 35:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;
						}
						case 36:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
							break;
						}
						case 37:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
							break;
						}
						case 38:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
							break;
						}
						case 39:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
							break;
						}
						case 40:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HelpPetStealing));
							break;
						}
						case 41:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HelpMountLevelCheck));
							break;
						}
						case 42:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HelpVendorDiscounts));
							break;
						}
						case 43:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HelpExpGainSkills));
							break;
						}
						case 44:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HelpGainPetSlotLvl));
							break;
						}
						case 45:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HelpPlayerStartUp));
							break;
						}
						case 46:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HelpLevelBag));
							break;
						}
						
						
					}
				break;
				}
                case 2:
				{
					switch (index)
					{
						case 0:
						{
							if (m_Itemxml.EnabledLevelPets == false)
							{
								m_Itemxml.EnabledLevelPets = true;
								foreach (var m in World.Mobiles.Values)
								{
									BaseCreature bc = m as BaseCreature;
									PlayerMobile pm = m as PlayerMobile;
									
									if (bc is BaseCreature && bc.Controlled == true)
									{
										PetLevelOrb petorb = null;
										petorb = bc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;

										var BankBoxVar = bc.FindItemOnLayer(Layer.Bank);
										PetLevelOrb petorb1 = new PetLevelOrb();
										
										if (BankBoxVar == null)
											bc.AddItem(BankBoxVar = new BankBox(bc));
										
										if (petorb == null)
											bc.BankBox.AddItem(petorb1);
										
										bc.InvalidateProperties();
									}
									if (pm is PlayerMobile)
									{
										PetLevelSheet bagcheck2 = null;
										bagcheck2 = pm.Backpack.FindItemByType(typeof(PetLevelSheet), false) as PetLevelSheet;
										if (bagcheck2 == null)
										{
											pm.AddToBackpack(new PetLevelSheet());
										}
									}
								}
								from.SendMessage("You turned on the pet level system!");
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
								return;
							}
							if (m_Itemxml.EnabledLevelPets == true)
							{
								from.SendMessage("You turned off the pet level system!");
								m_Itemxml.EnabledLevelPets = false;
								
								
								foreach (var m in World.Mobiles.Values)
								{
									BaseCreature bc = m as BaseCreature;
									PlayerMobile pm = m as PlayerMobile;
									if (pm is PlayerMobile)
									{
										PetLevelSheet bagcheck2 = null;
										bagcheck2 = pm.Backpack.FindItemByType(typeof(PetLevelSheet), false) as PetLevelSheet;
										if (bagcheck2 != null)
										{
											bagcheck2.Delete();
										}
									}
								}
								
								
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
								return;
							}
							break;
						}
						case 1:
						{
							if (m_Itemxml.PlayerLevels == false)
							{
								from.SendMessage("You turned on the player level system!");
								foreach (var pm in World.Mobiles.Values)
								{
									if (pm is PlayerMobile && pm.Name != null)
									{
										Container packxml1 = pm.Backpack;
										if (packxml1 != null)
										{
											LevelSheet sheetcheck = null;
											sheetcheck = pm.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
											if (sheetcheck == null)
											{
												pm.AddToBackpack(new LevelSheet());
											}
										}
									}
								}
								m_Itemxml.PlayerLevels = true;
								from.InvalidateProperties();
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
								return;
							}
							if (m_Itemxml.PlayerLevels == true)
							{
								from.SendMessage("You turned off the player level system!");
								from.InvalidateProperties();
								m_Itemxml.PlayerLevels = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
								return;
							}
							break;
						}
						case 2:
						{
							string NumberString = info.GetTextEntry(4).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}

								m_Itemxml.EndMaxLvl = totalnumber;
							}
							
							from.SendMessage("You updated the Max level obtainable with scrolls!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
							break;	
						}
						case 3:
						{
							if (m_Itemxml.ActivateDynamicLevelSystem == false)
							{
								from.SendMessage("You turned on the Dynamic Weapon Levels!");
								m_Itemxml.ActivateDynamicLevelSystem = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DynamicEquipmentLevels));
								return;
							}
							if (m_Itemxml.ActivateDynamicLevelSystem == true)
							{
								from.SendMessage("You turned off the Dynamic Weapon Levels!");
								m_Itemxml.ActivateDynamicLevelSystem = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DynamicEquipmentLevels));
								return;
							}
							break;
						}
						case 4:
						{
							if (m_Itemxml.EquipMentLevelSystem == false)
							{
								from.SendMessage("You turned on the Equipment Level System!");
								m_Itemxml.EquipMentLevelSystem = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
								return;
							}
							if (m_Itemxml.EquipMentLevelSystem == true)
							{
								from.SendMessage("You turned off the Equipment Level System!");
								m_Itemxml.EquipMentLevelSystem = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
								return;
							}
							break;
						}
						case 5:
						{
							if (m_Itemxml.DiscountFromVendors == false)
							{
								from.SendMessage("You turned on the Level Vendor Discounts!");
								m_Itemxml.DiscountFromVendors = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Expansions));
								return;
							}
							if (m_Itemxml.DiscountFromVendors == true)
							{
								from.SendMessage("You turned off the Level Vendor Discounts!");
								m_Itemxml.DiscountFromVendors = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Expansions));
								return;
							}
							break;
						}
						case 6:
						{
							string NumberString = info.GetTextEntry(5).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}

								m_Itemxml.StartMaxLvl = totalnumber;
							}
							
							from.SendMessage("You updated the Max level without scrolls!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
							break;	
						}
						case 7:
						{
							string NumberString = info.GetTextEntry(6).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}

								m_Itemxml.SkillCoinCap = totalnumber;
							}
							
							from.SendMessage("You updated the skill coin max!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
							break;	
						}
						case 8:
						{
							string NumberString = info.GetTextEntry(7).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}

								m_Itemxml.ExpCoinValue = totalnumber;
							}
							
							from.SendMessage("You updated the exp coin value!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
							break;	
						}
						case 9:
						{
							if (m_Itemxml.GainExpFromBods == false)
							{
								from.SendMessage("You turned on Bod Exp Gaining!");
								m_Itemxml.GainExpFromBods = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							if (m_Itemxml.GainExpFromBods == true)
							{
								from.SendMessage("You turned off Bod Exp Gaining!");
								m_Itemxml.GainExpFromBods = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							break;	
						}
						case 10:
						{
							string NumberString = info.GetTextEntry(8).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}

								m_Itemxml.ExpPowerAmount = totalnumber;
							}
							
							from.SendMessage("You updated the exp powerlevel value!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
							break;	
						}
						case 11:
						{
							foreach (var pm in World.Mobiles.Values)
							{
								if (pm is PlayerMobile && pm.Name != null)
								{
									Container packxml1 = pm.Backpack;
									if (packxml1 != null)
									{
										LevelSheet sheetcheck = null;
										sheetcheck = pm.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
										if (sheetcheck != null)
										{
											sheetcheck.Delete();
											from.SendMessage("You have deleted all the level sheets! As a reminder, this DOES NOT turn off the level system for players!");
											from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
										}
									}
								}
							}
							break;
						}
						case 12:
						{
							if (m_Itemxml.EnabledLevelPets == false)
							{
								
								foreach (var m in World.Mobiles.Values)
								{
									if (m is BaseCreature)
									{
										BaseCreature bc = m as BaseCreature;
										if (bc.ControlMaster == null)
											break;
										
										BankBox BankBoxVar = bc.FindBankNoCreate();
										PetLevelOrb petorb = null;
										petorb = BankBoxVar.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;
										PetLevelOrb petorb1 = new PetLevelOrb();
										if (BankBoxVar != null)
										{
											if (petorb != null)
											{
												bc.InvalidateProperties();
												break;
											}
											else
											{
												bc.BankBox.AddItem(petorb1);
												if (m_Itemxml.UseDynamicMaxLevels == true)
												{
													PetFeatureHandlerExt.PetLevelCaps(bc,petorb1);
												}
												else
												{
													petorb1.MaxLevel = m_Itemxml.StartMaxLvl;
												}
												bc.InvalidateProperties();
												break;
											}
										}
										else
										{
											bc.BankBox.AddItem(petorb1);
											if (m_Itemxml.UseDynamicMaxLevels == true)
											{
												PetFeatureHandlerExt.PetLevelCaps(bc,petorb1);
											}
											else
											{
												petorb1.MaxLevel = m_Itemxml.StartMaxLvl;
											}
											bc.InvalidateProperties();
											break;
										}
										 
									}
								}
								from.SendMessage("You turned on the pet level system!");
								m_Itemxml.EnabledLevelPets = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
								break;
							}
							if (m_Itemxml.EnabledLevelPets == true)
							{
								from.SendMessage("You turned off the pet level system!");
								m_Itemxml.EnabledLevelPets = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
								break;
							}
							break;
						}
						case 13:
						{
							foreach (var m in World.Mobiles.Values)
							{
								if (m is BaseCreature)
								{
									BaseCreature bc = m as BaseCreature;											
									BankBox BankBoxVar = bc.FindBankNoCreate();
									if (BankBoxVar != null)
									{
										PetLevelOrb petorb = null;
										petorb = BankBoxVar.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;
										if (petorb != null)
										{
											petorb.Delete();
											BankBoxVar.Delete();
											bc.InvalidateProperties();
										}
									}
								}
							}
							from.SendMessage("You have deleted all the pet orbs in the world!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
							break;
						}
						case 14:
						{
							if (m_Itemxml.EquipMentLevelSystem == false)
							{
								from.SendMessage("You turned on the Equipment Level System!");
								m_Itemxml.EquipMentLevelSystem = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
								return;
							}
							if (m_Itemxml.EquipMentLevelSystem == true)
							{
								from.SendMessage("You turned on the Equipment Level System!");
								m_Itemxml.EquipMentLevelSystem = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.Main));
								return;
							}
							break;
						}
						case 15:
						{
							string NumberString = info.GetTextEntry(9).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
									return;
								}

								m_Itemxml.PowerHourTime = totalnumber;
							}
							
							from.SendMessage("You updated the total time for powerleveling!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
							break;	
						}
						case 16:
						{
							if (m_Itemxml.DisableSkillGain == true)
							{
								from.SendMessage("You turned off the Skill Gain Mechanics!");
								m_Itemxml.DisableSkillGain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
								return;
							}
							if (m_Itemxml.DisableSkillGain == false)
							{
								from.SendMessage("You turned on the Skill Gain Mechanics!");
								m_Itemxml.DisableSkillGain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
								return;
							}
							break;
						}
						case 17:
						{
							if (m_Itemxml.LevelBelowToon == true)
							{
								m_Itemxml.LevelBelowToon = false;
								foreach (var pm in World.Mobiles.Values)
								{
									if (pm is PlayerMobile && pm.Name != null)
									{
										pm.InvalidateProperties();
									}
								}
								from.SendMessage("You turned off Level Below Toons!");
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
								from.InvalidateProperties();
								return;
							}
							if (m_Itemxml.LevelBelowToon == false)
							{
								m_Itemxml.LevelBelowToon = true;
								foreach (var pm in World.Mobiles.Values)
								{
									if (pm is PlayerMobile && pm.Name != null)
									{
										pm.InvalidateProperties();
									}
								}
								from.SendMessage("You turned on Level Below Toons!");
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
								from.InvalidateProperties();
								return;
							}
							break;
						}
						case 18:
						{
							if (m_Itemxml.ShowPaperDollLevel == true)
							{
								from.SendMessage("You turned off the paper doll Level!");
								m_Itemxml.ShowPaperDollLevel = false;
								foreach (var pm in World.Mobiles.Values)
								{
									if (pm is PlayerMobile && pm.Name != null)
									{
										pm.InvalidateProperties();
									}
								}
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
								return;
							}
							if (m_Itemxml.ShowPaperDollLevel == false)
							{
								from.SendMessage("You turned on the paper doll Level!");
								m_Itemxml.ShowPaperDollLevel = true;
								foreach (var pm in World.Mobiles.Values)
								{
									if (pm is PlayerMobile && pm.Name != null)
									{
										pm.InvalidateProperties();
									}
								}
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
								return;
							}
							break;
						}
						case 19:
						{
							if (m_Itemxml.PetKillGivesExp == true)
							{
								from.SendMessage("Pet kills now give exp!");
								m_Itemxml.PetKillGivesExp = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
								return;
							}
							if (m_Itemxml.PetKillGivesExp == false)
							{
								from.SendMessage("Pet kills no longer give exp!");
								m_Itemxml.PetKillGivesExp = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
								return;
							}
							break;
						}
						case 20: //This was replaced and is now an open slot to be used!
						{
							if (m_Itemxml.CraftGivesExp == true)
							{
								from.SendMessage("Crafting now gives exp!");
								m_Itemxml.CraftGivesExp = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							if (m_Itemxml.CraftGivesExp == false)
							{
								from.SendMessage("Crafting no longer gives exp!");
								m_Itemxml.CraftGivesExp = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							break;
						}
						case 21:
						{
							if (m_Itemxml.TablesAdvancedExp == true)
							{
								from.SendMessage("Tables are not used for exp calculations!");
								m_Itemxml.TablesAdvancedExp = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							if (m_Itemxml.TablesAdvancedExp == false)
							{
								from.SendMessage("Tables will be used for exp calculations!");
								m_Itemxml.TablesAdvancedExp = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							break;
						}
						case 22:
						{
							if (m_Itemxml.StaffHasLevel == true)
							{
								from.SendMessage("Staff no longer shows level!");
								m_Itemxml.StaffHasLevel = false;
								foreach (var pm in World.Mobiles.Values)
								{
									if (pm is PlayerMobile && pm.Name != null)
									{
										pm.InvalidateProperties();
									}
								}
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							if (m_Itemxml.StaffHasLevel == false)
							{
								from.SendMessage("Staff shows a level!");
								m_Itemxml.StaffHasLevel = true;
								foreach (var pm in World.Mobiles.Values)
								{
									if (pm is PlayerMobile && pm.Name != null)
									{
										pm.InvalidateProperties();
									}
								}
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							break;
						}
						case 23:
						{
							if (m_Itemxml.BonusStatOnLvl == true)
							{
								from.SendMessage("Bonus Stat chance on Level Not Active!");
								m_Itemxml.BonusStatOnLvl = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							if (m_Itemxml.BonusStatOnLvl == false)
							{
								from.SendMessage("Bonus Stat chance on Level Active!");
								m_Itemxml.BonusStatOnLvl = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							break;
						}
						case 24:
						{
							if (m_Itemxml.RefreshOnLevel == true)
							{
								from.SendMessage("Players vitals not restored on level up!");
								m_Itemxml.RefreshOnLevel = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							if (m_Itemxml.RefreshOnLevel == false)
							{
								from.SendMessage("Players vitals restored on level up!");
								m_Itemxml.RefreshOnLevel = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							break;
						}
						case 25:
						{
							if (m_Itemxml.LevelSheetPerma == true)
							{
								from.SendMessage("Players cannot discard the level sheet!");
								m_Itemxml.LevelSheetPerma = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							if (m_Itemxml.LevelSheetPerma == false)
							{
								from.SendMessage("Players can discard the level sheet!");
								m_Itemxml.LevelSheetPerma = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							break;
						}
						case 26:
						{
							if (m_Itemxml.ShowVendorLevels == true)
							{
								from.SendMessage("Vendors show a level when wild creature levels are active!");
								m_Itemxml.ShowVendorLevels = false;
								foreach (var bv in World.Mobiles.Values)
								{
									if (bv is BaseVendor && bv.Name != null)
									{
										bv.InvalidateProperties();
									}
								}
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							if (m_Itemxml.ShowVendorLevels == false)
							{
								from.SendMessage("Vendors do not show a level when wild creature levels are active!");
								m_Itemxml.ShowVendorLevels = true;
								foreach (var bv in World.Mobiles.Values)
								{
									if (bv is BaseVendor && bv.Name != null)
									{
										bv.InvalidateProperties();
									}
								}
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							break;
						}
						case 27:
						{
							if (m_Itemxml.PartyExpShare == true)
							{
								from.SendMessage("Party members do not share exp!");
								m_Itemxml.PartyExpShare = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							if (m_Itemxml.PartyExpShare == false)
							{
								from.SendMessage("Party members now share exp!");
								m_Itemxml.PartyExpShare = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							break;
						}
						case 28:
						{
							if (m_Itemxml.LevelStatResetButton == true)
							{
								from.SendMessage("Level stat reset button turned off!");
								m_Itemxml.LevelStatResetButton = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels3));
								return;
							}
							if (m_Itemxml.LevelStatResetButton == false)
							{
								from.SendMessage("Level stat reset button turned on!");
								m_Itemxml.LevelStatResetButton = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels3));
								return;
							}
							break;
						}
						case 29:
						{
							if (m_Itemxml.PartyExpShareSplit == true)
							{
								from.SendMessage("Party members do not split exp when sharing active exp!");
								m_Itemxml.PartyExpShareSplit = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							if (m_Itemxml.PartyExpShareSplit == false)
							{
								from.SendMessage("Party members do split exp when sharing active exp!");
								m_Itemxml.PartyExpShareSplit = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
								return;
							}
							break;
						}
						case 30:
						{
							string NumberString = info.GetTextEntry(10).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}

								m_Itemxml.L2to20Multipier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;	
						}
						case 31:
						{
							string NumberString = info.GetTextEntry(11).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}

								m_Itemxml.L21to40Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;	
						}
						case 32:
						{
							string NumberString = info.GetTextEntry(12).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}

								m_Itemxml.L41to60Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;	
						}
						case 33:
						{
							string NumberString = info.GetTextEntry(13).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}

								m_Itemxml.L61to70Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;	
						}
						case 34:
						{
							string NumberString = info.GetTextEntry(14).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}

								m_Itemxml.L71to80Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;	
						}
						case 35:
						{
							string NumberString = info.GetTextEntry(15).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}

								m_Itemxml.L81to90Multipier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;	
						}
						case 36:
						{
							string NumberString = info.GetTextEntry(16).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}

								m_Itemxml.L91to100Multipier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;	
						}
						case 37:
						{
							string NumberString = info.GetTextEntry(17).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}

								m_Itemxml.L101to110Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;	
						}
						case 38:
						{
							string NumberString = info.GetTextEntry(18).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}

								m_Itemxml.L111to120Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;	
						}
						case 39:
						{
							string NumberString = info.GetTextEntry(19).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}

								m_Itemxml.L121to130Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;	
						}
						case 40:
						{
							string NumberString = info.GetTextEntry(20).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
									return;
								}

								m_Itemxml.L131to140Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;	
						}
						case 41:
						{
							string NumberString = info.GetTextEntry(21).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}

								m_Itemxml.L141to150Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
							break;	
						}
						case 42:
						{
							string NumberString = info.GetTextEntry(22).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}

								m_Itemxml.L151to160Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
							break;	
						}
						case 43:
						{
							string NumberString = info.GetTextEntry(23).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}

								m_Itemxml.L161to170Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
							break;	
						}
						case 44:
						{
							string NumberString = info.GetTextEntry(24).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}

								m_Itemxml.L171to180Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
							break;	
						}
						case 45:
						{
							string NumberString = info.GetTextEntry(25).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}

								m_Itemxml.L181to190Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
							break;	
						}
						case 46:
						{
							string NumberString = info.GetTextEntry(26).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
									return;
								}

								m_Itemxml.L191to200Multiplier = totalnumber;
							}
							
							from.SendMessage("You updated the level modifier!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
							break;	
						}
						case 47:
						{
							if (m_Itemxml.LevelSkillResetButton == true)
							{
								from.SendMessage("Level skill reset button turned off!");
								m_Itemxml.LevelSkillResetButton = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels3));
								return;
							}
							if (m_Itemxml.LevelSkillResetButton == false)
							{
								from.SendMessage("Level skill reset button turned on!");
								m_Itemxml.LevelSkillResetButton = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels3));
								return;
							}
							break;
						}
						case 48:
						{
							string NumberString = info.GetTextEntry(980).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
									return;
								}

								m_Itemxml.PartyExpShareRange = totalnumber;
							}
							
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
							break;	
						}
						case 49:
						{
							if (m_Itemxml.Miscelaneous == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Miscelaneous = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							if (m_Itemxml.Miscelaneous == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Miscelaneous = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							break;
						}
						case 50:
						{
							if (m_Itemxml.Combat == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Combat = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							if (m_Itemxml.Combat == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Combat = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							break;
						}
						case 51:
						{
							if (m_Itemxml.Tradeskills == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Tradeskills = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							if (m_Itemxml.Tradeskills == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Tradeskills = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							break;
						}
						case 52:
						{
							if (m_Itemxml.Magic == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Magic = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							if (m_Itemxml.Magic == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Magic = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							break;
						}
						case 53:
						{
							if (m_Itemxml.Wilderness == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Wilderness = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							if (m_Itemxml.Wilderness == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Wilderness = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							break;
						}
						case 54:
						{
							if (m_Itemxml.Thieving == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Thieving = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							if (m_Itemxml.Thieving == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Thieving = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							break;
						}
						case 55:
						{
							if (m_Itemxml.Bard == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Bard = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							if (m_Itemxml.Bard == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Bard = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							break;
						}
						case 56:
						{
							if (m_Itemxml.Imbuing == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Imbuing = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							if (m_Itemxml.Imbuing == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Imbuing = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							break;
						}
						case 57:
						{
							if (m_Itemxml.Throwing == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Throwing = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							if (m_Itemxml.Throwing == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Throwing = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							break;
						}
						case 58:
						{
							if (m_Itemxml.Mysticism == true)
							{
								from.SendMessage("You have toggled the skill in the level system!");
								m_Itemxml.Mysticism = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							if (m_Itemxml.Mysticism == false)
							{
								from.SendMessage("You have toggled the skill in the level system!");
								m_Itemxml.Mysticism = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							break;
						}
						case 59:
						{
							if (m_Itemxml.Spellweaving == true)
							{
								from.SendMessage("You have toggled the skill in the level system!");
								m_Itemxml.Spellweaving = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							if (m_Itemxml.Spellweaving == false)
							{
								from.SendMessage("You have toggled the skill in the level system!");
								m_Itemxml.Spellweaving = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.HideSkills));
								return;
							}
							break;
						}
						case 60:
						{
							string NumberString = info.GetTextEntry(982).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
									return;
								}

								m_Itemxml.SkillCoinValue = totalnumber;
							}
							
							from.SendMessage("You updated the skill coin value!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
							break;	
						}
						 
					}
				}
				break;
                case 3: /* Category Page changes */
				{
					switch (index)
					{
						case 0:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels));
							break;
						}
						case 1:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels2));
							break;
						}
						case 2:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations2));
							break;
						}
						case 3:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelUpControlCalculations));
							break;
						}
						case 4:
						{
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PlayerLevels3));
							break;
						}
						 
					}
				}
				break;
                case 4: /* Equipment Dynamic levels */
				{
					switch (index)
					{
						case 0:
						{
							string NumberString = info.GetTextEntry(27).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel1 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 1:
						{
							string NumberString = info.GetTextEntry(28).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel2 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}				
						case 2:
						{
							string NumberString = info.GetTextEntry(29).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel3 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 3:
						{
							string NumberString = info.GetTextEntry(30).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel4 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 4:
						{
							string NumberString = info.GetTextEntry(31).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel5 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 5:
						{
							string NumberString = info.GetTextEntry(32).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel6 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 6:
						{
							string NumberString = info.GetTextEntry(33).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel7 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 7:
						{
							string NumberString = info.GetTextEntry(34).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel8 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 8:
						{
							string NumberString = info.GetTextEntry(35).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel9 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 9:
						{
							string NumberString = info.GetTextEntry(36).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel10 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 10:
						{
							string NumberString = info.GetTextEntry(37).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel1Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 11:
						{
							string NumberString = info.GetTextEntry(38).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel2Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 12:
						{
							string NumberString = info.GetTextEntry(39).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel3Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 13:
						{
							string NumberString = info.GetTextEntry(40).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel4Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 14:
						{
							string NumberString = info.GetTextEntry(41).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel5Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 15:
						{
							string NumberString = info.GetTextEntry(42).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel6Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 16:
						{
							string NumberString = info.GetTextEntry(43).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel7Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 17:
						{
							string NumberString = info.GetTextEntry(44).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel8Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 18:
						{
							string NumberString = info.GetTextEntry(45).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel9Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						case 19:
						{
							string NumberString = info.GetTextEntry(46).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
									return;
								}
								m_Itemxml.ArmorRequiredLevel10Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ArmorReqAndIntensity));
							break;	
						}
						
						case 20:
						{
							string NumberString = info.GetTextEntry(47).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel1 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 21:
						{
							string NumberString = info.GetTextEntry(48).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel2 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 22:
						{
							string NumberString = info.GetTextEntry(49).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel3 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 23:
						{
							string NumberString = info.GetTextEntry(50).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel4 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 24:
						{
							string NumberString = info.GetTextEntry(51).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel5 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 25:
						{
							string NumberString = info.GetTextEntry(52).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel6 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 26:
						{
							string NumberString = info.GetTextEntry(53).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel7 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 27:
						{
							string NumberString = info.GetTextEntry(54).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel8 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 28:
						{
							string NumberString = info.GetTextEntry(55).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel9 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 29:
						{
							string NumberString = info.GetTextEntry(56).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel10 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 30:
						{
							string NumberString = info.GetTextEntry(57).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel1Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 31:
						{
							string NumberString = info.GetTextEntry(58).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel2Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 32:
						{
							string NumberString = info.GetTextEntry(59).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel3Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 33:
						{
							string NumberString = info.GetTextEntry(60).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel4Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 34:
						{
							string NumberString = info.GetTextEntry(61).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel5Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 35:
						{
							string NumberString = info.GetTextEntry(62).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel6Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 36:
						{
							string NumberString = info.GetTextEntry(63).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel7Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 37:
						{
							string NumberString = info.GetTextEntry(64).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel8Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 38:
						{
							string NumberString = info.GetTextEntry(65).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel9Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 39:
						{
							string NumberString = info.GetTextEntry(66).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
									return;
								}
								m_Itemxml.WeaponRequiredLevel10Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.WeaponReqAndIntensity));
							break;	
						}
						case 40:
						{
							string NumberString = info.GetTextEntry(67).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel1 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 41:
						{
							string NumberString = info.GetTextEntry(68).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel2 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 42:
						{
							string NumberString = info.GetTextEntry(69).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel3 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 43:
						{
							string NumberString = info.GetTextEntry(70).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel4 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 44:
						{
							string NumberString = info.GetTextEntry(71).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel5 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 45:
						{
							string NumberString = info.GetTextEntry(72).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel6 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 46:
						{
							string NumberString = info.GetTextEntry(73).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel7 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 47:
						{
							string NumberString = info.GetTextEntry(74).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel8 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 48:
						{
							string NumberString = info.GetTextEntry(75).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel9 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 49:
						{
							string NumberString = info.GetTextEntry(76).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel10 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 50:
						{
							string NumberString = info.GetTextEntry(77).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel1Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 51:
						{
							string NumberString = info.GetTextEntry(78).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel2Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 52:
						{
							string NumberString = info.GetTextEntry(79).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel3Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 53:
						{
							string NumberString = info.GetTextEntry(80).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel4Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 54:
						{
							string NumberString = info.GetTextEntry(81).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel5Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 55:
						{
							string NumberString = info.GetTextEntry(82).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel6Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 56:
						{
							string NumberString = info.GetTextEntry(83).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel7Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 57:
						{
							string NumberString = info.GetTextEntry(84).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel8Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 58:
						{
							string NumberString = info.GetTextEntry(85).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel9Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 59:
						{
							string NumberString = info.GetTextEntry(86).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
									return;
								}
								m_Itemxml.ClothRequiredLevel10Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ClothingReqAndIntensity));
							break;	
						}
						case 60:
						{
							string NumberString = info.GetTextEntry(87).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel1 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 61:
						{
							string NumberString = info.GetTextEntry(88).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel2 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 62:
						{
							string NumberString = info.GetTextEntry(89).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel3 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 63:
						{
							string NumberString = info.GetTextEntry(90).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel4 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 64:
						{
							string NumberString = info.GetTextEntry(91).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel5 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 65:
						{
							string NumberString = info.GetTextEntry(92).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel6 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 66:
						{
							string NumberString = info.GetTextEntry(93).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel7 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 67:
						{
							string NumberString = info.GetTextEntry(94).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel8 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 68:
						{
							string NumberString = info.GetTextEntry(95).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel9 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 69:
						{
							string NumberString = info.GetTextEntry(96).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel10 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 70:
						{
							string NumberString = info.GetTextEntry(97).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel1Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 71:
						{
							string NumberString = info.GetTextEntry(98).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel2Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 72:
						{
							string NumberString = info.GetTextEntry(99).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel3Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 73:
						{
							string NumberString = info.GetTextEntry(100).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel4Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 74:
						{
							string NumberString = info.GetTextEntry(101).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel5Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 75:
						{
							string NumberString = info.GetTextEntry(102).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel6Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 76:
						{
							string NumberString = info.GetTextEntry(103).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel7Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 77:
						{
							string NumberString = info.GetTextEntry(104).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel8Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 78:
						{
							string NumberString = info.GetTextEntry(105).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel9Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 79:
						{
							string NumberString = info.GetTextEntry(106).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
									return;
								}
								m_Itemxml.JewelRequiredLevel10Intensity = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.JewelReqAndIntensity));
							break;	
						}
						case 80:
						{
							string NumberString = info.GetTextEntry(107).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString, 10);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DynamicEquipmentLevels));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DynamicEquipmentLevels));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DynamicEquipmentLevels));
									return;
								}
								m_Itemxml.EquipRequiredLevel0 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DynamicEquipmentLevels));
							break;	
						}
						case 81:
						{
							string NumberString = info.GetTextEntry(108).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;

								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DynamicEquipmentLevels));
									return;
								}
								m_Itemxml.RequiredLevelMouseOver = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DynamicEquipmentLevels));
							break;	
						}
						case 82:
						{
							string NumberString = info.GetTextEntry(109).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DynamicEquipmentLevels));
									return;
								}
								m_Itemxml.NameOfBattleRatingStat = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.DynamicEquipmentLevels));
							break;	
						}
						
					 
					}
					break;
				}
                case 5: /* Pet Levels */
				{
					switch (index)
					{
						case 0:
						{
							if (m_Itemxml.MountedPetsGainExp == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.MountedPetsGainExp = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							if (m_Itemxml.MountedPetsGainExp == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.MountedPetsGainExp = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							break;
						}
						case 1:
						{
							if (m_Itemxml.PetAttackBonus == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.PetAttackBonus = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							if (m_Itemxml.PetAttackBonus == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.PetAttackBonus = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							break;
						}
						case 2:
						{
							if (m_Itemxml.LevelBelowPet == true)
							{
								m_Itemxml.LevelBelowPet = false;
								foreach (var m in World.Mobiles.Values)
								{
									BaseCreature bc = m as BaseCreature;
									
									if (bc is BaseCreature && bc.Controlled == true)
									{
										bc.InvalidateProperties();
									}
								}
								from.SendMessage("You have toggled the setting");
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							if (m_Itemxml.LevelBelowPet == false)
							{
								m_Itemxml.LevelBelowPet = true;
								foreach (var m in World.Mobiles.Values)
								{
									BaseCreature bc = m as BaseCreature;
									
									if (bc is BaseCreature && bc.Controlled == true)
									{
										bc.InvalidateProperties();
									}
								}
								from.SendMessage("You have toggled the setting");
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							break;
						}
						case 3:
						{
							if (m_Itemxml.LoseExpLevelOnDeath == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.LoseExpLevelOnDeath = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							if (m_Itemxml.LoseExpLevelOnDeath == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.LoseExpLevelOnDeath = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							break;
						}
						case 4:
						{
							if (m_Itemxml.LoseStatOnDeath == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.LoseStatOnDeath = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							if (m_Itemxml.LoseStatOnDeath == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.LoseStatOnDeath = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							break;
						}
						case 5:
						{
							if (m_Itemxml.PetLevelSheetPerma == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.PetLevelSheetPerma = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							if (m_Itemxml.PetLevelSheetPerma == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.PetLevelSheetPerma = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							break;
						}
						case 6:
						{
							if (m_Itemxml.PetExpGainFromKill == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.PetExpGainFromKill = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							if (m_Itemxml.PetExpGainFromKill == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.PetExpGainFromKill = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							break;
						}
						case 7:
						{
							if (m_Itemxml.RefreshOnLevelPet == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.RefreshOnLevelPet = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							if (m_Itemxml.RefreshOnLevelPet == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.RefreshOnLevelPet = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							break;
						}
						case 8:
						{
							if (m_Itemxml.NotifyOnPetExpGain == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.NotifyOnPetExpGain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							if (m_Itemxml.NotifyOnPetExpGain == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.NotifyOnPetExpGain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							break;
						}
						case 9:
						{
							if (m_Itemxml.NotifyOnPetLevelUp == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.NotifyOnPetLevelUp = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							if (m_Itemxml.NotifyOnPetLevelUp == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.NotifyOnPetLevelUp = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							break;
						}
						case 10:
						{
							if (m_Itemxml.UntamedCreatureLevels == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.UntamedCreatureLevels = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							if (m_Itemxml.UntamedCreatureLevels == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.UntamedCreatureLevels = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels));
								return;
							}
							break;
						}
						case 11:
						{
							if (m_Itemxml.SuperHeal == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.SuperHeal = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							if (m_Itemxml.SuperHeal == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.SuperHeal = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							break;
						}
						case 12:
						{
							if (m_Itemxml.TelePortToTarget == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.TelePortToTarget = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							if (m_Itemxml.TelePortToTarget == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.TelePortToTarget = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							break;
						}
						case 13:
						{
							if (m_Itemxml.MassProvokeToAtt == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.MassProvokeToAtt = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							if (m_Itemxml.MassProvokeToAtt == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.MassProvokeToAtt = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							break;
						}
						case 14:
						{
							if (m_Itemxml.MassPeaceArea == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.MassPeaceArea = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							if (m_Itemxml.MassPeaceArea == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.MassPeaceArea = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							break;
						}
						case 15:
						{
							if (m_Itemxml.BlessedPower == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.BlessedPower = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							if (m_Itemxml.BlessedPower == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.BlessedPower = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							break;
						}
						case 16:
						{
							if (m_Itemxml.AreaFireBlast == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.AreaFireBlast = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							if (m_Itemxml.AreaFireBlast == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.AreaFireBlast = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							break;
						}
						case 17:
						{
							if (m_Itemxml.AreaIceBlast == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.AreaIceBlast = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							if (m_Itemxml.AreaIceBlast == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.AreaIceBlast = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							break;
						}
						case 18:
						{
							if (m_Itemxml.AreaAirBlast == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.AreaAirBlast = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							if (m_Itemxml.AreaAirBlast == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.AreaAirBlast = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							break;
						}
						case 19:
						{
							if (m_Itemxml.AuraStatBoost == true)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.AuraStatBoost = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							if (m_Itemxml.AuraStatBoost == false)
							{
								from.SendMessage("You have toggled the setting");
								m_Itemxml.AuraStatBoost = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
								return;
							}
							break;
						}
						case 20:
						{
							string NumberString = info.GetTextEntry(110).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.SuperHealReq = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 21:
						{
							string NumberString = info.GetTextEntry(111).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.TelePortToTargetReq = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 22:
						{
							string NumberString = info.GetTextEntry(112).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.MassProvokeToAttReq = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 23:
						{
							string NumberString = info.GetTextEntry(113).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.MassPeaceReq = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 24:
						{
							string NumberString = info.GetTextEntry(114).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.BlessedPowerReq = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 25:
						{
							string NumberString = info.GetTextEntry(115).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.AreaFireBlastReq = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 26:
						{
							string NumberString = info.GetTextEntry(116).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.AreaIceBlastReq = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 27:
						{
							string NumberString = info.GetTextEntry(117).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.AreaAirBlastReq = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 28:
						{
							string NumberString = info.GetTextEntry(118).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.AuraStatBoostReq = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 29:
						{
							string NumberString = info.GetTextEntry(119).Text.Replace(",", "");
							if (NumberString != null)
							{	
								double totalnumber = 0;
								try
								{
									totalnumber = Convert.ToDouble(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.SuperHealChance = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 30:
						{
							string NumberString = info.GetTextEntry(120).Text.Replace(",", "");
							if (NumberString != null)
							{	
								double totalnumber = 0;
								try
								{
									totalnumber = Convert.ToDouble(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.TelePortToTarChance = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 31:
						{
							string NumberString = info.GetTextEntry(121).Text.Replace(",", "");
							if (NumberString != null)
							{	
								double totalnumber = 0;
								try
								{
									totalnumber = Convert.ToDouble(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.MassProvokeChance = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 32:
						{
							string NumberString = info.GetTextEntry(122).Text.Replace(",", "");
							if (NumberString != null)
							{	
								double totalnumber = 0;
								try
								{
									totalnumber = Convert.ToDouble(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.MassPeaceChance = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 33:
						{
							string NumberString = info.GetTextEntry(123).Text.Replace(",", "");
							if (NumberString != null)
							{	
								double totalnumber = 0;
								try
								{
									totalnumber = Convert.ToDouble(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.BlessedPowerChance = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 34:
						{
							string NumberString = info.GetTextEntry(124).Text.Replace(",", "");
							if (NumberString != null)
							{	
								double totalnumber = 0;
								try
								{
									totalnumber = Convert.ToDouble(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.AreaFireBlastChance = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 35:
						{
							string NumberString = info.GetTextEntry(125).Text.Replace(",", "");
							if (NumberString != null)
							{	
								double totalnumber = 0;
								try
								{
									totalnumber = Convert.ToDouble(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.AreaIceBlastChance = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 36:
						{
							string NumberString = info.GetTextEntry(126).Text.Replace(",", "");
							if (NumberString != null)
							{	
								double totalnumber = 0;
								try
								{
									totalnumber = Convert.ToDouble(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
									return;
								}
								m_Itemxml.AreaAirBlastChance = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetSpecialAttacks));
							break;	
						}
						case 37:
						{
							if (m_Itemxml.EnableMountCheck == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.EnableMountCheck = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
								return;
							}
							if (m_Itemxml.EnableMountCheck == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.EnableMountCheck = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
								return;
							}
							break;
						}
						case 38:
						{
							string NumberString = info.GetTextEntry(127).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								m_Itemxml.Beetle = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
							break;	
						}
						case 39:
						{
							string NumberString = info.GetTextEntry(128).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								m_Itemxml.DesertOstard = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
							break;	
						}
						case 40:
						{
							string NumberString = info.GetTextEntry(129).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								m_Itemxml.FireSteed = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
							break;	
						}
						case 41:
						{
							string NumberString = info.GetTextEntry(130).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								m_Itemxml.ForestOstard = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
							break;	
						}
						case 42:
						{
							string NumberString = info.GetTextEntry(131).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								m_Itemxml.FrenziedOstard = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
							break;	
						}
						case 43:
						{
							string NumberString = info.GetTextEntry(132).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								m_Itemxml.HellSteed = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
							break;	
						}
						case 44:
						{
							string NumberString = info.GetTextEntry(133).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								m_Itemxml.Hiryu = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
							break;	
						}
						case 45:
						{
							string NumberString = info.GetTextEntry(134).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								m_Itemxml.Cusidhe = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
							break;	
						}
						case 46:
						{
							string NumberString = info.GetTextEntry(135).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								m_Itemxml.Kirin = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
							break;	
						}
						case 47:
						{
							string NumberString = info.GetTextEntry(136).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
									return;
								}
								m_Itemxml.LesserHiryu = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq));
							break;	
						}
						case 48:
						{
							string NumberString = info.GetTextEntry(137).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								m_Itemxml.NightMare = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
							break;	
						}
						case 49:
						{
							string NumberString = info.GetTextEntry(138).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								m_Itemxml.Ridablellama = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
							break;	
						}
						case 50:
						{
							string NumberString = info.GetTextEntry(139).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								m_Itemxml.Ridgeback = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
							break;	
						}
						case 51:
						{
							string NumberString = info.GetTextEntry(140).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								m_Itemxml.SavageRidgeback = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
							break;	
						}
						case 52:
						{
							string NumberString = info.GetTextEntry(141).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								m_Itemxml.ScaledSwampDragon = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
							break;	
						}
						case 53:
						{
							string NumberString = info.GetTextEntry(142).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								m_Itemxml.Seahorse = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
							break;	
						}
						case 54:
						{
							string NumberString = info.GetTextEntry(143).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								m_Itemxml.SilverSteed = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
							break;	
						}
						case 55:
						{
							string NumberString = info.GetTextEntry(144).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								m_Itemxml.Horse = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
							break;	
						}
						case 56:
						{
							string NumberString = info.GetTextEntry(145).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								m_Itemxml.SkeletalMount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
							break;	
						}
						case 57:
						{
							string NumberString = info.GetTextEntry(146).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
									return;
								}
								m_Itemxml.Swampdragon = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq2));
							break;	
						}
						case 58:
						{
							string NumberString = info.GetTextEntry(147).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								m_Itemxml.Unicorn = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
							break;	
						}
						case 59:
						{
							string NumberString = info.GetTextEntry(148).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								m_Itemxml.Reptalon = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
							break;	
						}
						case 60:
						{
							string NumberString = info.GetTextEntry(149).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								m_Itemxml.Wildtiger = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
							break;	
						}
						case 61:
						{
							string NumberString = info.GetTextEntry(150).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								m_Itemxml.Windrunner = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
							break;	
						}
						case 62:
						{
							string NumberString = info.GetTextEntry(151).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								m_Itemxml.Lasher = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
							break;	
						}
						case 63:
						{
							string NumberString = info.GetTextEntry(152).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								m_Itemxml.Eowmu = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
							break;	
						}
						case 64:
						{
							string NumberString = info.GetTextEntry(153).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
									return;
								}
								m_Itemxml.Dreadwarhorse = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelReq3));
							break;	
						}
						case 65:
						{
							string NumberString = info.GetTextEntry(154).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow20 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 66:
						{
							string NumberString = info.GetTextEntry(155).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow40 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 67:
						{
							string NumberString = info.GetTextEntry(156).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow60 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 68:
						{
							string NumberString = info.GetTextEntry(157).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow70 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 69:
						{
							string NumberString = info.GetTextEntry(158).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow80 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 70:
						{
							string NumberString = info.GetTextEntry(159).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow90 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 71:
						{
							string NumberString = info.GetTextEntry(160).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow100 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 72:
						{
							string NumberString = info.GetTextEntry(161).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow110 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 73:
						{
							string NumberString = info.GetTextEntry(162).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow120 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 74:
						{
							string NumberString = info.GetTextEntry(163).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow130 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 75:
						{
							string NumberString = info.GetTextEntry(164).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow140 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 76:
						{
							string NumberString = info.GetTextEntry(165).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow150 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 77:
						{
							string NumberString = info.GetTextEntry(166).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow160 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 78:
						{
							string NumberString = info.GetTextEntry(167).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow170 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 79:
						{
							string NumberString = info.GetTextEntry(168).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow180 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 80:
						{
							string NumberString = info.GetTextEntry(169).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow190 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 81:
						{
							string NumberString = info.GetTextEntry(170).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
									return;
								}
								m_Itemxml.Petbelow200 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelSkillPoints));
							break;	
						}
						case 82:
						{
							if (m_Itemxml.EmoteOnSpecialAtks == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.EmoteOnSpecialAtks = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
								return;
							}
							if (m_Itemxml.EmoteOnSpecialAtks == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.EmoteOnSpecialAtks = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
								return;
							}
							break;
						}
						case 83:
						{
							string NumberString = info.GetTextEntry(171).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
									return;
								}
								m_Itemxml.PetMaxStatPoints = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
							break;	
						}
						case 84:
						{
							string NumberString = info.GetTextEntry(172).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
									return;
								}
								m_Itemxml.PetStatLossAmount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
							break;	
						}
						case 85:
						{
							string NumberString = info.GetTextEntry(1200).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow20stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 86:
						{
							string NumberString = info.GetTextEntry(1201).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow40stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 87:
						{
							string NumberString = info.GetTextEntry(1202).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow60stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 88:
						{
							string NumberString = info.GetTextEntry(1203).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow70stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 89:
						{
							string NumberString = info.GetTextEntry(1204).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow80stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 90:
						{
							string NumberString = info.GetTextEntry(1205).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow90stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 91:
						{
							string NumberString = info.GetTextEntry(1206).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow100stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 92:
						{
							string NumberString = info.GetTextEntry(1207).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow110stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 93:
						{
							string NumberString = info.GetTextEntry(1208).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow120stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 94:
						{
							string NumberString = info.GetTextEntry(1209).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow130stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 95:
						{
							string NumberString = info.GetTextEntry(1210).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow140stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 96:
						{
							string NumberString = info.GetTextEntry(1211).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow150stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 97:
						{
							string NumberString = info.GetTextEntry(1212).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow160stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 98:
						{
							string NumberString = info.GetTextEntry(1213).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow170stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 99:
						{
							string NumberString = info.GetTextEntry(1214).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow180stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 100:
						{
							string NumberString = info.GetTextEntry(1215).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow190stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
						case 101:
						{
							string NumberString = info.GetTextEntry(1216).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
									return;
								}
								m_Itemxml.Petbelow200stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevelStatPoints));
							break;	
						}
					}
					break;
				}
				case 6:
				{
					switch (index)
					{
						case 1:
						{
							if (m_Itemxml.EnablePetpicks == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.EnablePetpicks = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetStealing));
								return;
							}
							if (m_Itemxml.EnablePetpicks == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.EnablePetpicks = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetStealing));
								return;
							}
							break;
						}
						case 2:
						{
							string NumberString = info.GetTextEntry(183).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetStealing));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetStealing));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetStealing));
									return;
								}
								m_Itemxml.MinSkillReqPickSteal = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetStealing));
							break;	
						}
						case 3:
						{
							if (m_Itemxml.PreventBondedPetpick == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.PreventBondedPetpick = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetStealing));
								return;
							}
							if (m_Itemxml.PreventBondedPetpick == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.PreventBondedPetpick = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetStealing));
								return;
							}
							break;
						}
						case 4:
						{
							if (m_Itemxml.UseDynamicMaxLevels == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.UseDynamicMaxLevels = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
								return;
							}
							if (m_Itemxml.UseDynamicMaxLevels == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.UseDynamicMaxLevels = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
								return;
							}
							break;
						}
						case 5:
						{
							string NumberString = info.GetTextEntry(184).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
									return;
								}
								m_Itemxml.StartMaxLvlPets = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.PetLevels2));
							break;	
						}
						case 6:
						{
							from.AddToBackpack(m_Item);
							from.SendMessage("You have moved the control token to your backpack! Use the [move command or make it movable to move it elsewhere!");
							break;
						}
						case 7:
						{
							if (m_Item is Item)
							{
								Item item = (Item)m_Item;
								Point3D itemloc;
								if (item.Parent != null)
								{
									if (item.RootParent is Mobile)
									{
										itemloc = ((Mobile)(item.RootParent)).Location;
									}
									else if (item.RootParent is Container)
									{
										itemloc = ((Container)(item.RootParent)).Location;
									}
									else
									{
										return;
									}
								}
								else
								{
									itemloc = item.Location;
								}
								if (item == null || item.Deleted || item.Map == null || item.Map == Map.Internal) return;
								from.Location = itemloc;
								from.Map = item.Map;

											
								from.SendMessage("You have teleported to the control item!");
								return;
							}
							break;
						}
					}
					break;
				}

				case 7:
				{
					switch (index)
					{
						case 0:
						{
							break;
						}
						case 1:
						{
							if (m_Itemxml.Enableexpfromskills == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Enableexpfromskills = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							if (m_Itemxml.Enableexpfromskills == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Enableexpfromskills = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							break;
						}
						case 2:
						{
							if (m_Itemxml.Begginggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Begginggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							if (m_Itemxml.Begginggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Begginggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							break;
						}
						case 3:
						{
							if (m_Itemxml.Campinggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Campinggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							if (m_Itemxml.Campinggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Campinggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							break;
						}
						case 4:
						{
							if (m_Itemxml.Forensicsgain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Forensicsgain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							if (m_Itemxml.Forensicsgain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Forensicsgain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							break;
						}
						case 5:
						{
							if (m_Itemxml.Itemidgain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Itemidgain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							if (m_Itemxml.Itemidgain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Itemidgain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							break;
						}
						case 6:
						{
							if (m_Itemxml.Tasteidgain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Tasteidgain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							if (m_Itemxml.Tasteidgain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Tasteidgain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							break;
						}
						case 7:
						{
							if (m_Itemxml.Imbuinggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Imbuinggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							if (m_Itemxml.Imbuinggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Imbuinggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							break;
						}
						case 8:
						{
							if (m_Itemxml.Evalintgain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Evalintgain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							if (m_Itemxml.Evalintgain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Evalintgain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							break;
						}
						case 9:
						{
							if (m_Itemxml.Spiritspeakgain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Spiritspeakgain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							if (m_Itemxml.Spiritspeakgain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Spiritspeakgain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							break;
						}
						case 10:
						{
							if (m_Itemxml.Fishinggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Fishinggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							if (m_Itemxml.Fishinggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Fishinggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
								return;
							}
							break;
						}
						case 11:
						{
							if (m_Itemxml.Herdinggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Herdinggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							if (m_Itemxml.Herdinggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Herdinggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							break;
						}
						case 12:
						{
							if (m_Itemxml.Trackinggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Trackinggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							if (m_Itemxml.Trackinggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Trackinggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							break;
						}
						case 13:
						{
							if (m_Itemxml.Hidinggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Hidinggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							if (m_Itemxml.Hidinggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Hidinggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							break;
						}
						case 14:
						{
							if (m_Itemxml.Poisoninggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Poisoninggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							if (m_Itemxml.Poisoninggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Poisoninggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							break;
						}
						case 15:
						{
							if (m_Itemxml.Removetrapgain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Removetrapgain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							if (m_Itemxml.Removetrapgain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Removetrapgain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							break;
						}
						case 16:
						{
							if (m_Itemxml.Stealinggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Stealinggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							if (m_Itemxml.Stealinggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Stealinggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							break;
						}
						case 17:
						{
							if (m_Itemxml.Discordancegain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Discordancegain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							if (m_Itemxml.Discordancegain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Discordancegain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							break;
						}
						case 18:
						{
							if (m_Itemxml.Peacemakinggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Peacemakinggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							if (m_Itemxml.Peacemakinggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Peacemakinggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							break;
						}
						case 19:
						{
							if (m_Itemxml.Provocationgain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Provocationgain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							if (m_Itemxml.Provocationgain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Provocationgain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							break;
						}
						case 20:
						{
							if (m_Itemxml.Anatomygain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Anatomygain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							if (m_Itemxml.Anatomygain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Anatomygain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
								return;
							}
							break;
						}
						case 21:
						{
							if (m_Itemxml.Armsloregain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Armsloregain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							if (m_Itemxml.Armsloregain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Armsloregain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							break;
						}
						case 22:
						{
							if (m_Itemxml.Animalloregain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Animalloregain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							if (m_Itemxml.Animalloregain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Animalloregain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							break;
						}
						case 23:
						{
							if (m_Itemxml.Meditationgain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Meditationgain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							if (m_Itemxml.Meditationgain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Meditationgain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							break;
						}
						case 24:
						{
							if (m_Itemxml.Cartographygain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Cartographygain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							if (m_Itemxml.Cartographygain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Cartographygain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							break;
						}
						case 25:
						{
							if (m_Itemxml.Detecthiddengain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Detecthiddengain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							if (m_Itemxml.Detecthiddengain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Detecthiddengain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							break;
						}
						case 26:
						{
							if (m_Itemxml.Animaltaminggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Animaltaminggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							if (m_Itemxml.Animaltaminggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Animaltaminggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							break;
						}
						case 27:
						{
							if (m_Itemxml.Blacksmithgain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Blacksmithgain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							if (m_Itemxml.Blacksmithgain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Blacksmithgain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							break;
						}
						case 28:
						{
							if (m_Itemxml.Carpentrygain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Carpentrygain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							if (m_Itemxml.Carpentrygain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Carpentrygain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							break;
						}
						case 29:
						{
							if (m_Itemxml.Alchemygain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Alchemygain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							if (m_Itemxml.Alchemygain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Alchemygain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							break;
						}
						case 30:
						{
							if (m_Itemxml.Fletchinggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Fletchinggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							if (m_Itemxml.Fletchinggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Fletchinggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
								return;
							}
							break;
						}
						case 31:
						{
							if (m_Itemxml.Cookinggain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Cookinggain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
								return;
							}
							if (m_Itemxml.Cookinggain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Cookinggain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
								return;
							}
							break;
						}
						case 32:
						{
							if (m_Itemxml.Inscribegain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Inscribegain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
								return;
							}
							if (m_Itemxml.Inscribegain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Inscribegain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
								return;
							}
							break;
						}
						case 33:
						{
							if (m_Itemxml.Tailoringgain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Tailoringgain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
								return;
							}
							if (m_Itemxml.Tailoringgain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Tailoringgain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
								return;
							}
							break;
						}
						case 34:
						{
							if (m_Itemxml.Tinkeringgain == false)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Tinkeringgain = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
								return;
							}
							if (m_Itemxml.Tinkeringgain == true)
							{
								from.SendMessage("You toggle the setting!");
								m_Itemxml.Tinkeringgain = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
								return;
							}
							break;
						}
						case 35:
						{
							string NumberString = info.GetTextEntry(185).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								m_Itemxml.Begginggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
							break;	
						}
						case 36:
						{
							string NumberString = info.GetTextEntry(186).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								m_Itemxml.Campinggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
							break;	
						}
						case 37:
						{
							string NumberString = info.GetTextEntry(187).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								m_Itemxml.Forensicsgainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
							break;	
						}
						case 38:
						{
							string NumberString = info.GetTextEntry(188).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								m_Itemxml.Itemidgainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
							break;	
						}
						case 39:
						{
							string NumberString = info.GetTextEntry(189).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								m_Itemxml.Tasteidgainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
							break;	
						}
						case 40:
						{
							string NumberString = info.GetTextEntry(190).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								m_Itemxml.Imbuinggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
							break;	
						}
						case 41:
						{
							string NumberString = info.GetTextEntry(191).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								m_Itemxml.Evalintgainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
							break;	
						}
						case 42:
						{
							string NumberString = info.GetTextEntry(192).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								m_Itemxml.Spiritspeakgainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
							break;	
						}
						case 43:
						{
							string NumberString = info.GetTextEntry(193).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
									return;
								}
								m_Itemxml.Fishinggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP));
							break;	
						}
						case 44:
						{
							string NumberString = info.GetTextEntry(194).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								m_Itemxml.Herdinggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
							break;	
						}
						case 45:
						{
							string NumberString = info.GetTextEntry(195).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								m_Itemxml.Trackinggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
							break;	
						}
						case 46:
						{
							string NumberString = info.GetTextEntry(196).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								m_Itemxml.Hidinggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
							break;	
						}
						case 47:
						{
							string NumberString = info.GetTextEntry(197).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								m_Itemxml.Poisoninggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
							break;	
						}
						case 48:
						{
							string NumberString = info.GetTextEntry(198).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								m_Itemxml.Removetrapgainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
							break;	
						}
						case 49:
						{
							string NumberString = info.GetTextEntry(199).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								m_Itemxml.Stealinggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
							break;	
						}
						case 50:
						{
							string NumberString = info.GetTextEntry(200).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								m_Itemxml.Discordancegainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
							break;	
						}
						case 51:
						{
							string NumberString = info.GetTextEntry(201).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								m_Itemxml.Peacemakinggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
							break;	
						}
						case 52:
						{
							string NumberString = info.GetTextEntry(202).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								m_Itemxml.Provocationgainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
							break;	
						}
						case 53:
						{
							string NumberString = info.GetTextEntry(203).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
									return;
								}
								m_Itemxml.Anatomygainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP2));
							break;	
						}
						case 54:
						{
							string NumberString = info.GetTextEntry(204).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								m_Itemxml.Armsloregainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
							break;	
						}
						case 55:
						{
							string NumberString = info.GetTextEntry(205).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								m_Itemxml.Animalloregainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
							break;	
						}
						case 56:
						{
							string NumberString = info.GetTextEntry(206).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								m_Itemxml.Meditationgainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
							break;	
						}
						case 57:
						{
							string NumberString = info.GetTextEntry(207).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								m_Itemxml.Cartographygainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
							break;	
						}
						case 58:
						{
							string NumberString = info.GetTextEntry(208).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								m_Itemxml.Detecthiddengainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
							break;	
						}
						case 59:
						{
							string NumberString = info.GetTextEntry(209).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								m_Itemxml.Animaltaminggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
							break;	
						}
						case 60:
						{
							string NumberString = info.GetTextEntry(210).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								m_Itemxml.Blacksmithgainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
							break;	
						}
						case 61:
						{
							string NumberString = info.GetTextEntry(211).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								m_Itemxml.Carpentrygainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
							break;	
						}
						case 62:
						{
							string NumberString = info.GetTextEntry(212).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								m_Itemxml.Alchemygainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
							break;	
						}
						case 63:
						{
							string NumberString = info.GetTextEntry(213).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
									return;
								}
								m_Itemxml.Fletchinggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP3));
							break;	
						}
						case 64:
						{
							string NumberString = info.GetTextEntry(214).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
									return;
								}
								m_Itemxml.Cookinggainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
							break;	
						}
						case 65:
						{
							string NumberString = info.GetTextEntry(215).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
									return;
								}
								m_Itemxml.Inscribegainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
							break;	
						}
						case 66:
						{
							string NumberString = info.GetTextEntry(216).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
									return;
								}
								m_Itemxml.Tailoringgainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
							break;	
						}
						case 67:
						{
							string NumberString = info.GetTextEntry(217).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
									return;
								}
								m_Itemxml.Tinkeringgainamount = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.ConfiguredSkillsEXP4));
							break;	
						}
					}
				break;
				}
				case 8:
				{
					switch (index)
					{
						case 0:
						{
							break;
						}
						case 1:
						{
							string NumberString = info.GetTextEntry(218).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below20 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 2:
						{
							string NumberString = info.GetTextEntry(219).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below40 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 3:
						{
							string NumberString = info.GetTextEntry(220).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below60 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 4:
						{
							string NumberString = info.GetTextEntry(221).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below70 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 5:
						{
							string NumberString = info.GetTextEntry(222).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below80 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 6:
						{
							string NumberString = info.GetTextEntry(223).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below90 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 7:
						{
							string NumberString = info.GetTextEntry(224).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below100 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 8:
						{
							string NumberString = info.GetTextEntry(225).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below110 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 9:
						{
							string NumberString = info.GetTextEntry(226).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below120 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 10:
						{
							string NumberString = info.GetTextEntry(227).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below130 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 11:
						{
							string NumberString = info.GetTextEntry(228).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below140 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 12:
						{
							string NumberString = info.GetTextEntry(229).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below150 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 13:
						{
							string NumberString = info.GetTextEntry(230).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below160 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 14:
						{
							string NumberString = info.GetTextEntry(231).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below170 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 15:
						{
							string NumberString = info.GetTextEntry(232).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below180 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 16:
						{
							string NumberString = info.GetTextEntry(233).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below190 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 17:
						{
							string NumberString = info.GetTextEntry(234).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
									return;
								}
								m_Itemxml.Below200 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP));
							break;	
						}
						case 18:
						{
							string NumberString = info.GetTextEntry(235).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below20stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 19:
						{
							string NumberString = info.GetTextEntry(236).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below40stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 20:
						{
							string NumberString = info.GetTextEntry(237).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below60stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 21:
						{
							string NumberString = info.GetTextEntry(238).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below70stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 22:
						{
							string NumberString = info.GetTextEntry(239).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below80stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 23:
						{
							string NumberString = info.GetTextEntry(240).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below90stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 24:
						{
							string NumberString = info.GetTextEntry(241).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below100stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 25:
						{
							string NumberString = info.GetTextEntry(242).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below110stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 26:
						{
							string NumberString = info.GetTextEntry(243).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below120stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 27:
						{
							string NumberString = info.GetTextEntry(244).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below130stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 28:
						{
							string NumberString = info.GetTextEntry(245).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below140stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 29:
						{
							string NumberString = info.GetTextEntry(246).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below150stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 30:
						{
							string NumberString = info.GetTextEntry(247).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below160stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 31:
						{
							string NumberString = info.GetTextEntry(248).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below170stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 32:
						{
							string NumberString = info.GetTextEntry(249).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below180stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 33:
						{
							string NumberString = info.GetTextEntry(250).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below190stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 34:
						{
							string NumberString = info.GetTextEntry(251).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
									return;
								}
								m_Itemxml.Below200stat = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelGainStatSP2));
							break;	
						}
						case 35:
						{
							if (m_Itemxml.GainFollowerSlotOnLevel == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowerSlotOnLevel = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							if (m_Itemxml.GainFollowerSlotOnLevel == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowerSlotOnLevel = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							break;
						}
						case 36:
						{
							if (m_Itemxml.GainFollowOn20 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn20 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							if (m_Itemxml.GainFollowOn20 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn20 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							break;
						}
						case 37:
						{
							if (m_Itemxml.GainFollowOn30 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn30 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							if (m_Itemxml.GainFollowOn30 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn30 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							break;
						}
						case 38:
						{
							if (m_Itemxml.GainFollowOn40 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn40 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							if (m_Itemxml.GainFollowOn40 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn40 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							break;
						}
						case 39:
						{
							if (m_Itemxml.GainFollowOn50 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn50 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							if (m_Itemxml.GainFollowOn50 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn50 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							break;
						}
						case 40:
						{
							if (m_Itemxml.GainFollowOn60 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn60 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							if (m_Itemxml.GainFollowOn60 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn60 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							break;
						}
						case 41:
						{
							if (m_Itemxml.GainFollowOn70 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn70 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							if (m_Itemxml.GainFollowOn70 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn70 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							break;
						}
						case 42:
						{
							if (m_Itemxml.GainFollowOn80 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn80 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							if (m_Itemxml.GainFollowOn80 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn80 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							break;
						}
						case 43:
						{
							if (m_Itemxml.GainFollowOn90 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn90 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							if (m_Itemxml.GainFollowOn90 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn90 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							break;
						}
						case 44:
						{
							if (m_Itemxml.GainFollowOn100 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn100 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							if (m_Itemxml.GainFollowOn100 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn100 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
								return;
							}
							break;
						}
						case 45:
						{
							if (m_Itemxml.GainFollowOn110 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn110 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							if (m_Itemxml.GainFollowOn110 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn110 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							break;
						}
						case 46:
						{
							if (m_Itemxml.GainFollowOn120 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn120 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							if (m_Itemxml.GainFollowOn120 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn120 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							break;
						}
						case 47:
						{
							if (m_Itemxml.GainFollowOn130 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn130 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							if (m_Itemxml.GainFollowOn130 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn130 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							break;
						}
						case 48:
						{
							if (m_Itemxml.GainFollowOn140 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn140 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							if (m_Itemxml.GainFollowOn140 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn140 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							break;
						}
						case 49:
						{
							if (m_Itemxml.GainFollowOn150 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn150 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							if (m_Itemxml.GainFollowOn150 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn150 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							break;
						}
						case 50:
						{
							if (m_Itemxml.GainFollowOn160 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn160 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							if (m_Itemxml.GainFollowOn160 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn160 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							break;
						}
						case 51:
						{
							if (m_Itemxml.GainFollowOn170 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn170 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							if (m_Itemxml.GainFollowOn170 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn170 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							break;
						}
						case 52:
						{
							if (m_Itemxml.GainFollowOn180 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn180 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							if (m_Itemxml.GainFollowOn180 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn180 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							break;
						}
						case 53:
						{
							if (m_Itemxml.GainFollowOn190 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn190 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							if (m_Itemxml.GainFollowOn190 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn190 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							break;
						}
						case 54:
						{
							if (m_Itemxml.GainFollowOn200 == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn200 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							if (m_Itemxml.GainFollowOn200 == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.GainFollowOn200 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
								return;
							}
							break;
						}
						case 55:
						{
							string NumberString = info.GetTextEntry(252).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL20 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
							break;	
						}
						case 56:
						{
							string NumberString = info.GetTextEntry(253).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL30 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
							break;	
						}
						case 57:
						{
							string NumberString = info.GetTextEntry(254).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL40 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
							break;	
						}
						case 58:
						{
							string NumberString = info.GetTextEntry(255).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL50 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
							break;	
						}
						case 59:
						{
							string NumberString = info.GetTextEntry(256).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL60 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
							break;	
						}
						case 60:
						{
							string NumberString = info.GetTextEntry(257).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL70 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
							break;	
						}
						case 61:
						{
							string NumberString = info.GetTextEntry(258).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL80 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
							break;	
						}
						case 62:
						{
							string NumberString = info.GetTextEntry(259).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL90 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
							break;	
						}
						case 63:
						{
							string NumberString = info.GetTextEntry(260).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL100 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers));
							break;	
						}
						case 64:
						{
							string NumberString = info.GetTextEntry(261).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL110 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
							break;	
						}
						case 65:
						{
							string NumberString = info.GetTextEntry(262).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL120 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
							break;	
						}
						case 66:
						{
							string NumberString = info.GetTextEntry(263).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL130 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
							break;	
						}
						case 67:
						{
							string NumberString = info.GetTextEntry(264).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL140 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
							break;	
						}
						case 68:
						{
							string NumberString = info.GetTextEntry(265).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL150 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
							break;	
						}
						case 69:
						{
							string NumberString = info.GetTextEntry(266).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL160 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
							break;	
						}
						case 70:
						{
							string NumberString = info.GetTextEntry(267).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL170 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
							break;	
						}
						case 71:
						{
							string NumberString = info.GetTextEntry(268).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL180 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
							break;	
						}
						case 72:
						{
							string NumberString = info.GetTextEntry(269).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL190 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
							break;	
						}
						case 73:
						{
							string NumberString = info.GetTextEntry(270).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
									return;
								}
								m_Itemxml.GainFollowerSlotonLeveL200 = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.GainFollowers2));
							break;	
						}
						
						
						
						
						
						
						case 74:
						{
							if (m_Itemxml.NewStartingLocation == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.NewStartingLocation = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
								return;
							}
							if (m_Itemxml.NewStartingLocation == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.NewStartingLocation = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
								return;
							}
							break;
						}
						case 75:
						{
							string NumberString = info.GetTextEntry(271).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
									return;
								}
								m_Itemxml.X_variable = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
							break;	
						}
						case 76:
						{
							string NumberString = info.GetTextEntry(272).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
									return;
								}
								m_Itemxml.Y_variable = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
							break;	
						}
						case 77:
						{
							string NumberString = info.GetTextEntry(273).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
									return;
								}
								m_Itemxml.Z_variable = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
							break;	
						}
						case 78:
						{
							string NumberString = info.GetTextEntry(274).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
									return;
								}
								m_Itemxml.Maplocation = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
							break;	
						}
						case 79:
						{
							if (m_Itemxml.ForceNewPlayerIntoGuild == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.ForceNewPlayerIntoGuild = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
								return;
							}
							if (m_Itemxml.ForceNewPlayerIntoGuild == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.ForceNewPlayerIntoGuild = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
								return;
							}
							break;
						}
						case 80:
						{
							string NumberString = info.GetTextEntry(275).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
									return;
								}
								m_Itemxml.Guildnamestart = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
							break;	
						}
						case 81:
						{
							if (m_Itemxml.AddToBackpackOnAttach == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.AddToBackpackOnAttach = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
								return;
							}
							if (m_Itemxml.AddToBackpackOnAttach == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.AddToBackpackOnAttach = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
								return;
							}
							break;
						}
						case 82:
						{
							string NumberString = info.GetTextEntry(276).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
									return;
								}
								m_Itemxml.Startitem1 = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
							break;	
						}
						case 83:
						{
							string NumberString = info.GetTextEntry(277).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
									return;
								}
								m_Itemxml.Startitem2 = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
							break;	
						}
						case 84:
						{
							string NumberString = info.GetTextEntry(278).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
									return;
								}
								m_Itemxml.Startitem3 = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
							break;	
						}
						case 85:
						{
							string NumberString = info.GetTextEntry(279).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
									return;
								}
								m_Itemxml.Startitem4 = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
							break;	
						}
						case 86:
						{
							string NumberString = info.GetTextEntry(280).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
									return;
								}
								m_Itemxml.Startitem5 = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
							break;	
						}
						case 87:
						{
							string NumberString = info.GetTextEntry(281).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
									return;
								}
								m_Itemxml.Startitem6 = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
							break;	
						}
						case 88:
						{
							string NumberString = info.GetTextEntry(282).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
									return;
								}
								m_Itemxml.Startitem7 = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
							break;	
						}
						case 89:
						{
							string NumberString = info.GetTextEntry(283).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
									return;
								}
								m_Itemxml.Startitem8 = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
							break;	
						}
						case 90:
						{
							string NumberString = info.GetTextEntry(284).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
									return;
								}
								m_Itemxml.Startitem9 = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
							break;	
						}
						case 91:
						{
							string NumberString = info.GetTextEntry(285).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
									return;
								}
								m_Itemxml.Startitem10 = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler2));
							break;	
						}
						case 92:
						{
							if (m_Itemxml.Forcestartingstats == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Forcestartingstats = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
								return;
							}
							if (m_Itemxml.Forcestartingstats == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Forcestartingstats = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
								return;
							}
							break;
						}
						case 93:
						{
							string NumberString = info.GetTextEntry(286).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								m_Itemxml.Forcestartingstatsstr = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
							break;	
						}
						case 94:
						{
							string NumberString = info.GetTextEntry(287).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								m_Itemxml.Startingstrcapvar = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
							break;	
						}
						case 95:
						{
							string NumberString = info.GetTextEntry(288).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								m_Itemxml.Forcestartingstatsdex = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
							break;	
						}
						case 96:
						{
							string NumberString = info.GetTextEntry(289).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								m_Itemxml.Startingdexcapvar = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
							break;	
						}
						case 97:
						{
							string NumberString = info.GetTextEntry(290).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								m_Itemxml.Forcestartingstatsint = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
							break;	
						}
						case 98:
						{
							string NumberString = info.GetTextEntry(291).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								m_Itemxml.Startingintcapvar = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
							break;	
						}
						case 99:
						{
							if (m_Itemxml.Autoactivate_skillscap == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_skillscap = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
								return;
							}
							if (m_Itemxml.Autoactivate_skillscap == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_skillscap = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
								return;
							}
							break;
						}
						case 100:
						{
							string NumberString = info.GetTextEntry(292).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								m_Itemxml.Autoactivate_skillscapvar = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
							break;	
						}
						case 101:
						{
							if (m_Itemxml.Autoactivate_maxfollowslots == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_maxfollowslots = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
								return;
							}
							if (m_Itemxml.Autoactivate_maxfollowslots == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_maxfollowslots = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
								return;
							}
							break;
						}
						case 102:
						{
							string NumberString = info.GetTextEntry(293).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;

								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
									return;
								}
								m_Itemxml.Autoactivate_maxfollowslotstotal = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler3));
							break;	
						}
						case 103:
						{
							if (m_Itemxml.Autoactivate_isyoung == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_isyoung = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							if (m_Itemxml.Autoactivate_isyoung == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_isyoung = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							break;
						}
						case 104:
						{
							if (m_Itemxml.Autoactivate_mechanicallife == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_mechanicallife = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							if (m_Itemxml.Autoactivate_mechanicallife == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_mechanicallife = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							break;
						}
						case 105:
						{
							if (m_Itemxml.Autoactivate_cantwalk == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_cantwalk = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							if (m_Itemxml.Autoactivate_cantwalk == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_cantwalk = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							break;
						}
						case 106:
						{
							if (m_Itemxml.Autoactivate_disabledpvpwarning == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_disabledpvpwarning = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							if (m_Itemxml.Autoactivate_disabledpvpwarning == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_disabledpvpwarning = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							break;
						}
						case 107:
						{
							if (m_Itemxml.Autoactivate_gemmining == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_gemmining = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							if (m_Itemxml.Autoactivate_gemmining == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_gemmining = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							break;
						}
						case 108:
						{
							if (m_Itemxml.Autoactivate_basketweaving == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_basketweaving = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							if (m_Itemxml.Autoactivate_basketweaving == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_basketweaving = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							break;
						}
						case 109:
						{
							if (m_Itemxml.Autoactivate_canbuycarpets == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_canbuycarpets = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							if (m_Itemxml.Autoactivate_canbuycarpets == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_canbuycarpets = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							break;
						}
						case 110:
						{
							if (m_Itemxml.Autoactivate_acceptguildinvites == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_acceptguildinvites = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							if (m_Itemxml.Autoactivate_acceptguildinvites == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_acceptguildinvites = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							break;
						}
						case 111:
						{
							if (m_Itemxml.Autoactivate_glassblowing == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_glassblowing = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							if (m_Itemxml.Autoactivate_glassblowing == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_glassblowing = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							break;
						}
						case 112:
						{
							if (m_Itemxml.Autoactivate_libraryfriend == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_libraryfriend = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							if (m_Itemxml.Autoactivate_libraryfriend == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_libraryfriend = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler4));
								return;
							}
							break;
						}
						case 113:
						{
							if (m_Itemxml.Autoactivate_masonry == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_masonry = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler5));
								return;
							}
							if (m_Itemxml.Autoactivate_masonry == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_masonry = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler5));
								return;
							}
							break;
						}
						case 114:
						{
							if (m_Itemxml.Autoactivate_sandmining == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_sandmining = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler5));
								return;
							}
							if (m_Itemxml.Autoactivate_sandmining == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_sandmining = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler5));
								return;
							}
							break;
						}
						case 115:
						{
							if (m_Itemxml.Autoactivate_stonemining == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_stonemining = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler5));
								return;
							}
							if (m_Itemxml.Autoactivate_stonemining == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_stonemining = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler5));
								return;
							}
							break;
						}
						case 116:
						{
							if (m_Itemxml.Autoactivate_spellweaving == true)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_spellweaving = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler5));
								return;
							}
							if (m_Itemxml.Autoactivate_spellweaving == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.Autoactivate_spellweaving = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler5));
								return;
							}
							break;
						}
						case 117:
						{
							if (m_Itemxml.MapBoolTrammel == true)
							{
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
								return;
							}
							if (m_Itemxml.MapBoolTrammel == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.MapBoolTrammel	= true;
								m_Itemxml.MapBoolFelucca	= false;
								m_Itemxml.MapBoolMalas		= false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
								return;
							}
							break;
						}
						case 118:
						{
							if (m_Itemxml.MapBoolFelucca == true)
							{
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
								return;
							}
							if (m_Itemxml.MapBoolFelucca == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.MapBoolTrammel	= false;
								m_Itemxml.MapBoolFelucca	= true;
								m_Itemxml.MapBoolMalas		= false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
								return;
							}
							break;
						}
						case 119:
						{
							if (m_Itemxml.MapBoolMalas == true)
							{
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
								return;
							}
							if (m_Itemxml.MapBoolMalas == false)
							{
								from.SendMessage("You updated the setting!");
								m_Itemxml.MapBoolTrammel	= false;
								m_Itemxml.MapBoolFelucca	= false;
								m_Itemxml.MapBoolMalas		= true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.StartupPlayerHandler));
								return;
							}
							break;
						}
					}

				}
				break;
				case 9:
				{ 
					switch (index)
					{
						case 0:
						{
							if (m_Itemxml.Preventbagdrop == true)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Preventbagdrop = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
								return;
							}
							if (m_Itemxml.Preventbagdrop == false)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Preventbagdrop = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
								return;
							}
							break;
						}
						case 1:
						{
							if (m_Itemxml.Bagsystemmaintoggle == true)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Bagsystemmaintoggle = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
								return;
							}
							if (m_Itemxml.Bagsystemmaintoggle == false)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Bagsystemmaintoggle = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
								return;
							}
							break;
						}
						case 2:
						{
							if (m_Itemxml.Levelgroup1 == true)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup1 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
								return;
							}
							if (m_Itemxml.Levelgroup1 == false)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup1 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
								return;
							}
							break;
						}
						case 3:
						{
							string NumberString = info.GetTextEntry(294).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								m_Itemxml.Levelgroup1reqLevel = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
							break;	
						}
						case 4:
						{
							string NumberString = info.GetTextEntry(295).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								m_Itemxml.Levelgroup1maxitems = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
							break;	
						}
						case 5:
						{
							string NumberString = info.GetTextEntry(296).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								m_Itemxml.Levelgroup1reducetotal = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
							break;	
						}
						case 6:
						{
							string NumberString = info.GetTextEntry(297).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								m_Itemxml.Levelgroup1msg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
							break;	
						}
						case 7:
						{
							string NumberString = info.GetTextEntry(298).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								m_Itemxml.Level1groupownermsg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
							break;	
						}
						case 8:
						{
							if (m_Itemxml.Levelgroup2 == true)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup2 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
								return;
							}
							if (m_Itemxml.Levelgroup2 == false)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup2 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
								return;
							}
							break;
						}
						case 9:
						{
							string NumberString = info.GetTextEntry(299).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								m_Itemxml.Levelgroup2reqLevel = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
							break;	
						}
						case 10:
						{
							string NumberString = info.GetTextEntry(300).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								m_Itemxml.Levelgroup2maxitems = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
							break;	
						}
						case 11:
						{
							string NumberString = info.GetTextEntry(301).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								m_Itemxml.Levelgroup2reducetotal = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
							break;	
						}
						case 12:
						{
							string NumberString = info.GetTextEntry(302).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								m_Itemxml.Levelgroup2msg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
							break;	
						}
						case 13:
						{
							string NumberString = info.GetTextEntry(303).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
									return;
								}
								m_Itemxml.Level2groupownermsg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag));
							break;	
						}
						case 14:
						{
							if (m_Itemxml.Levelgroup3 == true)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup3 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
								return;
							}
							if (m_Itemxml.Levelgroup3 == false)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup3 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
								return;
							}
							break;
						}
						case 15:
						{
							string NumberString = info.GetTextEntry(304).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								m_Itemxml.Levelgroup3reqLevel = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
							break;	
						}
						case 16:
						{
							string NumberString = info.GetTextEntry(305).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								m_Itemxml.Levelgroup3maxitems = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
							break;	
						}
						case 17:
						{
							string NumberString = info.GetTextEntry(306).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								m_Itemxml.Levelgroup3reducetotal = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
							break;	
						}
						case 18:
						{
							string NumberString = info.GetTextEntry(307).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								m_Itemxml.Levelgroup3msg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
							break;	
						}
						case 19:
						{
							string NumberString = info.GetTextEntry(308).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								m_Itemxml.Level3groupownermsg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
							break;	
						}
						case 20:
						{
							if (m_Itemxml.Levelgroup4 == true)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup4 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
								return;
							}
							if (m_Itemxml.Levelgroup4 == false)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup4 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
								return;
							}
							break;
						}
						case 21:
						{
							string NumberString = info.GetTextEntry(309).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								m_Itemxml.Levelgroup4reqLevel = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
							break;	
						}
						case 22:
						{
							string NumberString = info.GetTextEntry(310).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								m_Itemxml.Levelgroup4maxitems = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
							break;	
						}
						case 23:
						{
							string NumberString = info.GetTextEntry(311).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								m_Itemxml.Levelgroup4reducetotal = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
							break;	
						}
						case 24:
						{
							string NumberString = info.GetTextEntry(312).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								m_Itemxml.Levelgroup4msg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
							break;	
						}
						case 25:
						{
							string NumberString = info.GetTextEntry(313).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
									return;
								}
								m_Itemxml.Level4groupownermsg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag2));
							break;	
						}
						case 26:
						{
							if (m_Itemxml.Levelgroup5 == true)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup5 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
								return;
							}
							if (m_Itemxml.Levelgroup5 == false)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup5 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
								return;
							}
							break;
						}
						case 27:
						{
							string NumberString = info.GetTextEntry(314).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								m_Itemxml.Levelgroup5reqLevel = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
							break;	
						}
						case 28:
						{
							string NumberString = info.GetTextEntry(315).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								m_Itemxml.Levelgroup5maxitems = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
							break;	
						}
						case 29:
						{
							string NumberString = info.GetTextEntry(316).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								m_Itemxml.Levelgroup5reducetotal = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
							break;	
						}
						case 30:
						{
							string NumberString = info.GetTextEntry(317).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								m_Itemxml.Levelgroup5msg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
							break;	
						}
						case 31:
						{
							string NumberString = info.GetTextEntry(318).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								m_Itemxml.Level5groupownermsg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
							break;	
						}
						case 32:
						{
							if (m_Itemxml.Levelgroup6 == true)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup6 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
								return;
							}
							if (m_Itemxml.Levelgroup6 == false)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup6 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
								return;
							}
							break;
						}
						case 33:
						{
							string NumberString = info.GetTextEntry(319).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								m_Itemxml.Levelgroup6reqLevel = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
							break;	
						}
						case 34:
						{
							string NumberString = info.GetTextEntry(320).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								m_Itemxml.Levelgroup6maxitems = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
							break;	
						}
						case 35:
						{
							string NumberString = info.GetTextEntry(321).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								m_Itemxml.Levelgroup6reducetotal = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
							break;	
						}
						case 36:
						{
							string NumberString = info.GetTextEntry(322).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								m_Itemxml.Levelgroup6msg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
							break;	
						}
						case 37:
						{
							string NumberString = info.GetTextEntry(323).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
									return;
								}
								m_Itemxml.Level6groupownermsg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag3));
							break;	
						}
						case 38:
						{
							if (m_Itemxml.Levelgroup7 == true)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup7 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
								return;
							}
							if (m_Itemxml.Levelgroup7 == false)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup7 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
								return;
							}
							break;
						}
						case 39:
						{
							string NumberString = info.GetTextEntry(324).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								m_Itemxml.Levelgroup7reqLevel = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
							break;	
						}
						case 40:
						{
							string NumberString = info.GetTextEntry(325).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								m_Itemxml.Levelgroup7maxitems = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
							break;	
						}
						case 41:
						{
							string NumberString = info.GetTextEntry(326).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								m_Itemxml.Levelgroup7reducetotal = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
							break;	
						}
						case 42:
						{
							string NumberString = info.GetTextEntry(327).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								m_Itemxml.Levelgroup7msg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
							break;	
						}
						case 43:
						{
							string NumberString = info.GetTextEntry(328).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								m_Itemxml.Level7groupownermsg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
							break;	
						}
						case 44:
						{
							if (m_Itemxml.Levelgroup8 == true)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup8 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
								return;
							}
							if (m_Itemxml.Levelgroup8 == false)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup8 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
								return;
							}
							break;
						}
						case 45:
						{
							string NumberString = info.GetTextEntry(329).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								m_Itemxml.Levelgroup8reqLevel = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
							break;	
						}
						case 46:
						{
							string NumberString = info.GetTextEntry(330).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								m_Itemxml.Levelgroup8maxitems = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
							break;	
						}
						case 47:
						{
							string NumberString = info.GetTextEntry(331).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								m_Itemxml.Levelgroup8reducetotal = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
							break;	
						}
						case 48:
						{
							string NumberString = info.GetTextEntry(332).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								m_Itemxml.Levelgroup8msg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
							break;	
						}
						case 49:
						{
							string NumberString = info.GetTextEntry(333).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
									return;
								}
								m_Itemxml.Level8groupownermsg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag4));
							break;	
						}
						case 50:
						{
							if (m_Itemxml.Levelgroup9 == true)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup9 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
								return;
							}
							if (m_Itemxml.Levelgroup9 == false)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup9 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
								return;
							}
							break;
						}
						case 51:
						{
							string NumberString = info.GetTextEntry(334).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								m_Itemxml.Levelgroup9reqLevel = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
							break;	
						}
						case 52:
						{
							string NumberString = info.GetTextEntry(335).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								m_Itemxml.Levelgroup9maxitems = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
							break;	
						}
						case 53:
						{
							string NumberString = info.GetTextEntry(336).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								m_Itemxml.Levelgroup9reducetotal = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
							break;	
						}
						case 54:
						{
							string NumberString = info.GetTextEntry(337).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								m_Itemxml.Levelgroup9msg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
							break;	
						}
						case 55:
						{
							string NumberString = info.GetTextEntry(338).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								m_Itemxml.Level9groupownermsg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
							break;	
						}
						case 56:
						{
							if (m_Itemxml.Levelgroup10 == true)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup10 = false;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
								return;
							}
							if (m_Itemxml.Levelgroup10 == false)
							{
								from.SendMessage("You toggled the option!");
								m_Itemxml.Levelgroup10 = true;
								from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
								return;
							}
							break;
						}
						case 57:
						{
							string NumberString = info.GetTextEntry(339).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								m_Itemxml.Levelgroup10reqLevel = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
							break;	
						}
						case 58:
						{
							string NumberString = info.GetTextEntry(340).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								m_Itemxml.Levelgroup10maxitems = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
							break;	
						}
						case 59:
						{
							string NumberString = info.GetTextEntry(341).Text.Replace(",", "");
							if (NumberString != null)
							{	
								int totalnumber = 0;
								try
								{
									totalnumber = Convert.ToInt32(NumberString);
								}
								catch
								{
									from.SendMessage(2125, "This is a numbers only field!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								int[] switches = info.Switches;
								if (totalnumber < 0)
								{
									from.SendMessage(2125, "You can't use a negative number!");
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								if (totalnumber == 0)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								m_Itemxml.Levelgroup10reducetotal = totalnumber;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
							break;	
						}
						case 60:
						{
							string NumberString = info.GetTextEntry(342).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								m_Itemxml.Levelgroup10msg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
							break;	
						}
						case 61:
						{
							string NumberString = info.GetTextEntry(343).Text.Replace(",", "");
							if (NumberString != null)
							{	
								string totaltext = null;

								int[] switches = info.Switches;
								
								totaltext = NumberString;
								
								if (totaltext == null)
								{
									from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
									return;
								}
								m_Itemxml.Level10groupownermsg = totaltext;
							}
							from.SendMessage("You updated the setting!");
							from.SendGump(new LevelControlSysGump(from, m_Item, LevelControlSysGump.GumpPage.None, LevelControlSysGump.LevelCategory.LevelBag5));
							break;	
						}
					}
				}
				break;
			}
		}
		
	}		
}