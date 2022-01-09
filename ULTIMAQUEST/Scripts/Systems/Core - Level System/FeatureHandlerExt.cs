using System;
using Server;
using System.Xml;
using Server.Misc;
using Server.Gumps;
using Server.Items;
using System.Text;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Accounting;
using System.Collections;
using Server.Engines.Craft;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Engines.LevelSystemExtAtt;


namespace Server
{
    public class FeatureHandlerExt
    {
		public static string PlayerMobileComputeTitle(Mobile beholder, Mobile beheld, StringBuilder title)
		{
			string customTitle = beheld.Title;

			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));	
			if (m_ItemxmlSys != null && m_ItemxmlSys.PlayerLevels == true)
			{
				if (beheld is PlayerMobile)
				{
					PlayerMobile pm = beheld as PlayerMobile;				
					Container playerpack = pm.Backpack;
					Item xmlplayer = playerpack.FindItemByType( typeof( LevelSheet )); 
					
					if (beheld is PlayerMobile && ((PlayerMobile)beheld).PaperdollSkillTitle != null)
					{
						if (m_ItemxmlSys.ShowPaperDollLevel && xmlplayer != null)
						{
							string d = LevelCore.Display(pm);
							title.Append(" - Level " + d + ", ").Append(((PlayerMobile)beheld).PaperdollSkillTitle);
						}
						else
							title.Append(", ").Append(((PlayerMobile)beheld).PaperdollSkillTitle);
					}
					else if (beheld is PlayerMobile && ((PlayerMobile)beheld).PaperdollSkillTitle == null)
					{
						string d = LevelCore.Display(pm);
						if (m_ItemxmlSys.ShowPaperDollLevel && xmlplayer != null)
						{
							if (pm.AccessLevel > AccessLevel.Player && m_ItemxmlSys.StaffHasLevel)
							{
								title.Append(" - Level " + d);
							}
							else
							{
								if (pm.AccessLevel < AccessLevel.GameMaster)
								{
									title.Append(" - Level " + d);
								}
							}
						}
					}
					else if (beheld is BaseVendor)
						title.AppendFormat(" {0}", customTitle);
				}
			}
			else
			{
				if (beheld is PlayerMobile && ((PlayerMobile)beheld).PaperdollSkillTitle != null)
					title.Append(", ").Append(((PlayerMobile)beheld).PaperdollSkillTitle);
				else if (beheld is BaseVendor)
					title.AppendFormat(" {0}", customTitle);
			}

			return title.ToString();
		}
		
		public static void PlayerMobileToonTitle(Mobile m, ObjectPropertyList list)
        {
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return;
			PlayerMobile pm = m as PlayerMobile;
			
			if (m_ItemxmlSys.PlayerLevels == true)
			{
				if (pm is PlayerMobile && m_ItemxmlSys.LevelBelowToon == true)
				{
					LevelSheet xmlplayer = null;
					xmlplayer = pm.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
					if (xmlplayer == null)
					{
						return;
					}
					if (pm.AccessLevel > AccessLevel.Player && m_ItemxmlSys.StaffHasLevel == true)
					{
					   if (m_ItemxmlSys.MaxLevel)
						{
							list.Add("<BASEFONT COLOR=#7FCAE7>Level: <BASEFONT COLOR=#17FF01>" + xmlplayer.Levell + "/" + xmlplayer.MaxLevel);
						}
						else
						{
							 list.Add("<BASEFONT COLOR=#7FCAE7>Level: <BASEFONT COLOR=#17FF01>" + xmlplayer.Levell);
						}
					}
					else
					{
						if (pm.AccessLevel == AccessLevel.Player)
						{
							if (m_ItemxmlSys.MaxLevel)
							{
								list.Add("<BASEFONT COLOR=#7FCAE7>Level: <BASEFONT COLOR=#17FF01>" + xmlplayer.Levell + "/" + xmlplayer.MaxLevel);
							}
							else
							{
								 list.Add("<BASEFONT COLOR=#7FCAE7>Level: <BASEFONT COLOR=#17FF01>" + xmlplayer.Levell);
							}
						}
					}
				}
			}
		}
		
		public static void LevelAddNamePropsExt(Item item, ObjectPropertyList list)
        {
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys cfe = null;
			cfe = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));	
			if (cfe != null && cfe.PlayerLevels == true)
			{
				if (item is BaseWeapon && cfe.ActivateDynamicLevelSystem == true)
				{
					BaseWeapon bw = item as BaseWeapon;
					int weaponval = CheckWeapon(bw);
					list.Add(cfe.NameOfBattleRatingStat + " {0}", weaponval);
					if (weaponval >= cfe.WeaponRequiredLevel10Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.WeaponRequiredLevel10);}
					else if (weaponval >= cfe.WeaponRequiredLevel9Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.WeaponRequiredLevel9);}
					else if (weaponval >= cfe.WeaponRequiredLevel8Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.WeaponRequiredLevel8);}
					else if (weaponval >= cfe.WeaponRequiredLevel7Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.WeaponRequiredLevel7);}
					else if (weaponval >= cfe.WeaponRequiredLevel6Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.WeaponRequiredLevel6);}
					else if (weaponval >= cfe.WeaponRequiredLevel5Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.WeaponRequiredLevel5);}
					else if (weaponval >= cfe.WeaponRequiredLevel4Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.WeaponRequiredLevel4);}
					else if (weaponval >= cfe.WeaponRequiredLevel3Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.WeaponRequiredLevel3);}
					else if (weaponval >= cfe.WeaponRequiredLevel2Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.WeaponRequiredLevel2);}
					else if (weaponval >= cfe.WeaponRequiredLevel1Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.WeaponRequiredLevel1);}
					else if (weaponval < cfe.WeaponRequiredLevel1Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.EquipRequiredLevel0);}
				}
				if (item is BaseArmor && cfe.ActivateDynamicLevelSystem == true)
				{
					BaseArmor ba = item as BaseArmor;
					int armorval = CheckArmor(ba);
					list.Add(cfe.NameOfBattleRatingStat + " {0}", armorval);
					if (armorval >= cfe.ArmorRequiredLevel10Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ArmorRequiredLevel10);}
					else if (armorval >= cfe.ArmorRequiredLevel9Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ArmorRequiredLevel9);}
					else if (armorval >= cfe.ArmorRequiredLevel8Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ArmorRequiredLevel8);}
					else if (armorval >= cfe.ArmorRequiredLevel7Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ArmorRequiredLevel7);}
					else if (armorval >= cfe.ArmorRequiredLevel6Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ArmorRequiredLevel6);}
					else if (armorval >= cfe.ArmorRequiredLevel5Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ArmorRequiredLevel5);}
					else if (armorval >= cfe.ArmorRequiredLevel4Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ArmorRequiredLevel4);}
					else if (armorval >= cfe.ArmorRequiredLevel3Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ArmorRequiredLevel3);}
					else if (armorval >= cfe.ArmorRequiredLevel2Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ArmorRequiredLevel2);}
					else if (armorval >= cfe.ArmorRequiredLevel1Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ArmorRequiredLevel1);}
					else if (armorval < cfe.ArmorRequiredLevel1Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.EquipRequiredLevel0);}
				}
				if (item is BaseJewel && cfe.ActivateDynamicLevelSystem == true)
				{
					BaseJewel bj = item as BaseJewel;
					int jewelval = CheckJewel(bj);
					list.Add(cfe.NameOfBattleRatingStat + " {0}", jewelval);
					if (jewelval >= cfe.JewelRequiredLevel10Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.JewelRequiredLevel10);}
					else if (jewelval >= cfe.JewelRequiredLevel9Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.JewelRequiredLevel9);}
					else if (jewelval >= cfe.JewelRequiredLevel8Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.JewelRequiredLevel8);}
					else if (jewelval >= cfe.JewelRequiredLevel7Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.JewelRequiredLevel7);}
					else if (jewelval >= cfe.JewelRequiredLevel6Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.JewelRequiredLevel6);}
					else if (jewelval >= cfe.JewelRequiredLevel5Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.JewelRequiredLevel5);}
					else if (jewelval >= cfe.JewelRequiredLevel4Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.JewelRequiredLevel4);}
					else if (jewelval >= cfe.JewelRequiredLevel3Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.JewelRequiredLevel3);}
					else if (jewelval >= cfe.JewelRequiredLevel2Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.JewelRequiredLevel2);}
					else if (jewelval < cfe.JewelRequiredLevel1Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.EquipRequiredLevel0);}
				}
				if (item is BaseClothing && cfe.ActivateDynamicLevelSystem == true)
				{
					BaseClothing bc = item as BaseClothing;
					int clothval = CheckClothing(bc);
					list.Add(cfe.NameOfBattleRatingStat + " {0}", clothval);
					if (clothval >= cfe.ClothRequiredLevel10Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ClothRequiredLevel10);}
					else if (clothval >= cfe.ClothRequiredLevel9Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ClothRequiredLevel9);}
					else if (clothval >= cfe.ClothRequiredLevel8Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ClothRequiredLevel8);}
					else if (clothval >= cfe.ClothRequiredLevel7Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ClothRequiredLevel7);}
					else if (clothval >= cfe.ClothRequiredLevel6Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ClothRequiredLevel6);}
					else if (clothval >= cfe.ClothRequiredLevel5Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ClothRequiredLevel5);}
					else if (clothval >= cfe.ClothRequiredLevel4Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ClothRequiredLevel4);}
					else if (clothval >= cfe.ClothRequiredLevel3Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ClothRequiredLevel3);}
					else if (clothval >= cfe.ClothRequiredLevel2Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ClothRequiredLevel2);}
					else if (clothval >= cfe.ClothRequiredLevel1Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.ClothRequiredLevel1);}
					else if (clothval < cfe.ClothRequiredLevel1Intensity){list.Add(cfe.RequiredLevelMouseOver + " {0}", cfe.EquipRequiredLevel0);}
				}
			}
        }
		
        public static bool CanEquipExt(Mobile from, Item item)
        {
			bool CanEquip = true;
			
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys cfe = null;
			cfe = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));	
			if (cfe != null && cfe.PlayerLevels == true)
			{
					
				if (from is PlayerMobile && cfe.ActivateDynamicLevelSystem == true)
				{			
					LevelSheet xmlplayer = null;
					xmlplayer = from.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
					if (xmlplayer == null)
						return false;
					/* If return false, the weapon cannot be equiped */
					
					
					if (item is BaseArmor && cfe.ActivateDynamicLevelSystem == true)
					{
						var itemarmor = (Item)(item);
						item.InvalidateProperties();
						BaseArmor ba = itemarmor as BaseArmor;
						int values = CheckArmor(ba); 
						if (values >= cfe.ArmorRequiredLevel10Intensity)
						{
							if (xmlplayer.Levell >= cfe.ArmorRequiredLevel10)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ArmorRequiredLevel10);
								return false;
							}
						}
						
						if (values >= cfe.ArmorRequiredLevel9Intensity)
						{
							if (xmlplayer.Levell >= cfe.ArmorRequiredLevel9)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ArmorRequiredLevel9);
								return false;
							}
						}
						if (values >= cfe.ArmorRequiredLevel8Intensity)
						{
							if (xmlplayer.Levell >= cfe.ArmorRequiredLevel8)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ArmorRequiredLevel8);
								return false;
							}
						}
						if (values >= cfe.ArmorRequiredLevel7Intensity)
						{
							if (xmlplayer.Levell >= cfe.ArmorRequiredLevel7)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ArmorRequiredLevel7);
								return false;
							}
						}
						if (values >= cfe.ArmorRequiredLevel6Intensity)
						{
							if (xmlplayer.Levell >= cfe.ArmorRequiredLevel6)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ArmorRequiredLevel6);
								return false;
							}
						}
						if (values >= cfe.ArmorRequiredLevel5Intensity)
						{
							if (xmlplayer.Levell >= cfe.ArmorRequiredLevel5)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ArmorRequiredLevel5);
								return false;
							}
						}
						if (values >= cfe.ArmorRequiredLevel4Intensity)
						{
							if (xmlplayer.Levell >= cfe.ArmorRequiredLevel4)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ArmorRequiredLevel4);
								return false;
							}
						}
						if (values >= cfe.ArmorRequiredLevel3Intensity)
						{
							if (xmlplayer.Levell >= cfe.ArmorRequiredLevel3)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ArmorRequiredLevel3);
								return false;
							}
						}
						if (values >= cfe.ArmorRequiredLevel2Intensity)
						{
							if (xmlplayer.Levell >= cfe.ArmorRequiredLevel2)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ArmorRequiredLevel2);
								return false;
							}
						}
						if (values >= cfe.ArmorRequiredLevel1Intensity)
						{
							if (xmlplayer.Levell >= cfe.ArmorRequiredLevel1)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ArmorRequiredLevel1);
								return false;
							}
						}
						
					}
					
					if (item is BaseWeapon && cfe.ActivateDynamicLevelSystem == true)
					{	
						var itemwepon = (Item)(item);
						item.InvalidateProperties();
						BaseWeapon bw = itemwepon as BaseWeapon;
						int values = CheckWeapon(bw); 
						if (values >= cfe.WeaponRequiredLevel10Intensity)
						{
							if (xmlplayer.Levell >= cfe.WeaponRequiredLevel10)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.WeaponRequiredLevel10);
								return false;
							}
						}
						if (values >= cfe.WeaponRequiredLevel9Intensity)
						{
							if (xmlplayer.Levell >= cfe.WeaponRequiredLevel9)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.WeaponRequiredLevel9);
								return false;
							}
						}
						if (values >= cfe.WeaponRequiredLevel8Intensity)
						{
							if (xmlplayer.Levell >= cfe.WeaponRequiredLevel8)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.WeaponRequiredLevel8);
								return false;
							}
						}
						if (values >= cfe.WeaponRequiredLevel7Intensity)
						{
							if (xmlplayer.Levell >= cfe.WeaponRequiredLevel7)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.WeaponRequiredLevel7);
								return false;
							}
						}
						if (values >= cfe.WeaponRequiredLevel6Intensity)
						{
							if (xmlplayer.Levell >= cfe.WeaponRequiredLevel6)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.WeaponRequiredLevel6);
								return false;
							}
						}
						if (values >= cfe.WeaponRequiredLevel5Intensity)
						{
							if (xmlplayer.Levell >= cfe.WeaponRequiredLevel5)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.WeaponRequiredLevel5);
								return false;
							}
						}
						if (values >= cfe.WeaponRequiredLevel4Intensity)
						{
							if (xmlplayer.Levell >= cfe.WeaponRequiredLevel4)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.WeaponRequiredLevel4);
								return false;
							}
						}
						if (values >= cfe.WeaponRequiredLevel3Intensity)
						{
							if (xmlplayer.Levell >= cfe.WeaponRequiredLevel3)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.WeaponRequiredLevel3);
								return false;
							}
						}
						if (values >= cfe.WeaponRequiredLevel2Intensity)
						{
							if (xmlplayer.Levell >= cfe.WeaponRequiredLevel2)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.WeaponRequiredLevel2);
								return false;
							}
						}
						if (values >= cfe.WeaponRequiredLevel1Intensity)
						{
							if (xmlplayer.Levell >= cfe.WeaponRequiredLevel1)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.WeaponRequiredLevel1);
								return false;
							}
						}
						
					}
					if (item is BaseJewel && cfe.ActivateDynamicLevelSystem == true)
					{	
						var itemjewel = (Item)(item);
						item.InvalidateProperties();
						BaseJewel cj = itemjewel as BaseJewel;
						int values = CheckJewel(cj); 
						if (values >= cfe.JewelRequiredLevel10Intensity)
						{
							if (xmlplayer.Levell >= cfe.JewelRequiredLevel10)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.JewelRequiredLevel10);
								return false;
							}
						}
						if (values >= cfe.JewelRequiredLevel9Intensity)
						{
							if (xmlplayer.Levell >= cfe.JewelRequiredLevel9)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.JewelRequiredLevel9);
								return false;
							}
						}
						if (values >= cfe.JewelRequiredLevel8Intensity)
						{
							if (xmlplayer.Levell >= cfe.JewelRequiredLevel8)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.JewelRequiredLevel8);
								return false;
							}
						}
						if (values >= cfe.JewelRequiredLevel7Intensity)
						{
							if (xmlplayer.Levell >= cfe.JewelRequiredLevel7)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.JewelRequiredLevel7);
								return false;
							}
						}
						if (values >= cfe.JewelRequiredLevel6Intensity)
						{
							if (xmlplayer.Levell >= cfe.JewelRequiredLevel6)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.JewelRequiredLevel6);
								return false;
							}
						}
						if (values >= cfe.JewelRequiredLevel5Intensity)
						{
							if (xmlplayer.Levell >= cfe.JewelRequiredLevel5)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.JewelRequiredLevel5);
								return false;
							}
						}
						if (values >= cfe.JewelRequiredLevel4Intensity)
						{
							if (xmlplayer.Levell >= cfe.JewelRequiredLevel4)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.JewelRequiredLevel4);
								return false;
							}
						}
						if (values >= cfe.JewelRequiredLevel3Intensity)
						{
							if (xmlplayer.Levell >= cfe.JewelRequiredLevel3)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.JewelRequiredLevel3);
								return false;
							}
						}
						if (values >= cfe.JewelRequiredLevel2Intensity)
						{
							if (xmlplayer.Levell >= cfe.JewelRequiredLevel2)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.JewelRequiredLevel2);
								return false;
							}
						}
						if (values >= cfe.JewelRequiredLevel1Intensity)
						{
							if (xmlplayer.Levell >= cfe.JewelRequiredLevel1)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.JewelRequiredLevel1);
								return false;
							}
						}
						
					}
					if (item is BaseClothing && cfe.ActivateDynamicLevelSystem == true)
					{	
						var itemcloth = (Item)(item);
						item.InvalidateProperties();
						BaseClothing bc = itemcloth as BaseClothing;
						int values = CheckClothing(bc); 
						if (values >= cfe.ClothRequiredLevel10Intensity)
						{
							if (xmlplayer.Levell >= cfe.ClothRequiredLevel10)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ClothRequiredLevel10);
								return false;
							}
						}
						if (values >= cfe.ClothRequiredLevel9Intensity)
						{
							if (xmlplayer.Levell >= cfe.ClothRequiredLevel9)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ClothRequiredLevel9);
								return false;
							}
						}
						if (values >= cfe.ClothRequiredLevel8Intensity)
						{
							if (xmlplayer.Levell >= cfe.ClothRequiredLevel8)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ClothRequiredLevel8);
								return false;
							}
						}
						if (values >= cfe.ClothRequiredLevel7Intensity)
						{
							if (xmlplayer.Levell >= cfe.ClothRequiredLevel7)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ClothRequiredLevel7);
								return false;
							}
						}
						if (values >= cfe.ClothRequiredLevel6Intensity)
						{
							if (xmlplayer.Levell >= cfe.ClothRequiredLevel6)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ClothRequiredLevel6);
								return false;
							}
						}
						if (values >= cfe.ClothRequiredLevel5Intensity)
						{
							if (xmlplayer.Levell >= cfe.ClothRequiredLevel5)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ClothRequiredLevel5);
								return false;
							}
						}
						if (values >= cfe.ClothRequiredLevel4Intensity)
						{
							if (xmlplayer.Levell >= cfe.ClothRequiredLevel4)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ClothRequiredLevel4);
								return false;
							}
						}
						if (values >= cfe.ClothRequiredLevel3Intensity)
						{
							if (xmlplayer.Levell >= cfe.ClothRequiredLevel3)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ClothRequiredLevel3);
								return false;
							}
						}
						if (values >= cfe.ClothRequiredLevel2Intensity)
						{
							if (xmlplayer.Levell >= cfe.ClothRequiredLevel2)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ClothRequiredLevel2);
								return false;
							}
						}
						if (values >= cfe.ClothRequiredLevel1Intensity)
						{
							if (xmlplayer.Levell >= cfe.ClothRequiredLevel1)
								return CanEquip;
							else
							{
								from.SendMessage("You must be at level {0} or higher to use this!", cfe.ClothRequiredLevel1);
								return false;
							}
						}
						
					}
				}
				return CanEquip;
			}
            return CanEquip;
		
        }
		#region WeaponVariables
        public static int CheckWeapon(BaseWeapon item)
        {
            int value = 0;
			foreach (long i in Enum.GetValues(typeof(AosWeaponAttribute)))
			{
				if (item != null && item.Attributes[(AosAttribute)i] > 0)
					value += 11;
			}
			foreach (long i in Enum.GetValues(typeof(AosWeaponAttribute)))
			{
				if (item.WeaponAttributes[(AosWeaponAttribute)i] > 0)
					value += 11;
			}
            #region Start skill bonus
            if (item.SkillBonuses.Skill_1_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_1_Value * 4;
                value += 11;
            }
            if (item.SkillBonuses.Skill_2_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_2_Value * 4;
                value += 11;
            }
            if (item.SkillBonuses.Skill_3_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_3_Value * 4;
                value += 11;
            }
            if (item.SkillBonuses.Skill_4_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_4_Value * 4;
                value += 11;
            }
            if (item.SkillBonuses.Skill_5_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_5_Value * 4;
                value += 11;
            }
			#endregion
            #region Start Slayers
            if (item.Slayer != SlayerName.None)
                value += 42;
            if (item.Slayer2 != SlayerName.None)
                value += 42;
            if (item.Slayer3 != TalismanSlayerName.None)
                value += 42;
			#endregion
            #region Start weapon attributes
            if (item.WeaponAttributes.BattleLust > 0)
                value += 40;
            if (item.WeaponAttributes.BloodDrinker > 0)
                value += 40;
            if (item.WeaponAttributes.DurabilityBonus > 0)
                value += item.WeaponAttributes.DurabilityBonus / 4;
            if (item.WeaponAttributes.HitColdArea > 0)
                value += item.WeaponAttributes.HitColdArea / 2;
            if (item.WeaponAttributes.HitCurse > 0)
                value += item.WeaponAttributes.HitCurse / 2;
            if (item.WeaponAttributes.HitDispel > 0)
                value += item.WeaponAttributes.HitDispel / 2;
            if (item.WeaponAttributes.HitEnergyArea > 0)
                value += item.WeaponAttributes.HitEnergyArea / 2;
            if (item.WeaponAttributes.HitFatigue > 0)
                value += item.WeaponAttributes.HitFatigue / 2;
            if (item.WeaponAttributes.HitFireArea > 0)
                value += item.WeaponAttributes.HitFireArea / 2;
            if (item.WeaponAttributes.HitFireball > 0)
                value += item.WeaponAttributes.HitFireball / 2;
            if (item.WeaponAttributes.HitHarm > 0)
                value += item.WeaponAttributes.HitHarm / 2;
            if (item.WeaponAttributes.HitLeechHits > 0)
                value += item.WeaponAttributes.HitLeechHits / 2;
            if (item.WeaponAttributes.HitLeechMana > 0)
                value += item.WeaponAttributes.HitLeechMana / 2;
            if (item.WeaponAttributes.HitLeechStam > 0)
                value += item.WeaponAttributes.HitLeechStam / 2;
            if (item.WeaponAttributes.HitLightning > 0)
                value += item.WeaponAttributes.HitLightning / 2;
            if (item.WeaponAttributes.HitLowerAttack > 0)
                value += item.WeaponAttributes.HitLowerAttack / 2;
            if (item.WeaponAttributes.HitLowerDefend > 0)
                value += item.WeaponAttributes.HitLowerDefend / 2;
            if (item.WeaponAttributes.HitMagicArrow > 0)
                value += item.WeaponAttributes.HitMagicArrow / 2;
            if (item.WeaponAttributes.HitManaDrain > 0)
                value += item.WeaponAttributes.HitManaDrain / 2;
            if (item.WeaponAttributes.HitPhysicalArea > 0)
                value += item.WeaponAttributes.HitPhysicalArea / 2;
            if (item.WeaponAttributes.HitPoisonArea > 0)
                value += item.WeaponAttributes.HitPoisonArea / 2;
            if (item.WeaponAttributes.LowerStatReq > 0)
                value += item.WeaponAttributes.LowerStatReq / 2;
            if (item.WeaponAttributes.MageWeapon > 0)
                value += item.WeaponAttributes.MageWeapon;
            if (item.WeaponAttributes.ResistColdBonus > 0)
                value += item.WeaponAttributes.ResistColdBonus / 2;
            if (item.WeaponAttributes.ResistEnergyBonus > 0)
                value += item.WeaponAttributes.ResistEnergyBonus / 2;
            if (item.WeaponAttributes.ResistFireBonus > 0)
                value += item.WeaponAttributes.ResistFireBonus / 2;
            if (item.WeaponAttributes.ResistPhysicalBonus > 0)
                value += item.WeaponAttributes.ResistPhysicalBonus / 2;
            if (item.WeaponAttributes.ResistPoisonBonus > 0)
                value += item.WeaponAttributes.ResistPoisonBonus / 2;
            if (item.WeaponAttributes.SelfRepair > 0)
                value += item.WeaponAttributes.SelfRepair * 2;
            if (item.WeaponAttributes.UseBestSkill > 0)
                value += 10;
			#endregion
            #region Start standard attributes
            if (item.Attributes.AttackChance > 0)
                value += item.Attributes.AttackChance * 2;
            if (item.Attributes.BonusDex > 0)
                value += item.Attributes.BonusDex * 4;
            if (item.Attributes.BonusHits > 0)
                value += item.Attributes.BonusHits * 2;
            if (item.Attributes.BonusInt > 0)
                value += item.Attributes.BonusInt * 4;
            if (item.Attributes.BonusMana > 0)
                value += item.Attributes.BonusMana * 2;
            if (item.Attributes.BonusStam > 0)
                value += item.Attributes.BonusStam * 2;
            if (item.Attributes.BonusStr > 0)
                value += item.Attributes.BonusStr * 4;
            if (item.Attributes.CastRecovery > 0)
                value += item.Attributes.CastRecovery * 10;
            if (item.Attributes.CastSpeed > 0)
                value += item.Attributes.CastSpeed * 10;
            if (item.Attributes.DefendChance > 0)
                value += item.Attributes.DefendChance * 2;
            if (item.Attributes.EnhancePotions > 0)
                value += item.Attributes.EnhancePotions;
            if (item.Attributes.LowerManaCost > 0)
                value += item.Attributes.LowerManaCost * 2;
            if (item.Attributes.LowerRegCost > 0)
                value += item.Attributes.LowerRegCost;
            if (item.Attributes.Luck > 0)
                value += item.Attributes.Luck / 2;
            if (item.Attributes.NightSight > 0)
                value += 10;
            if (item.Attributes.ReflectPhysical > 0)
                value += item.Attributes.ReflectPhysical * 2;
            if (item.Attributes.RegenHits > 0)
                value += item.Attributes.RegenHits * 10;
            if (item.Attributes.RegenMana > 0)
                value += item.Attributes.RegenMana * 10;
            if (item.Attributes.RegenStam > 0)
                value += item.Attributes.RegenStam * 10;
            if (item.Attributes.SpellChanneling > 0)
                value += 10;
            if (item.Attributes.SpellDamage > 0)
                value += item.Attributes.SpellDamage * 2;
            if (item.Attributes.WeaponDamage > 0)
                value += item.Attributes.WeaponDamage * 2;
            if (item.Attributes.WeaponSpeed > 0)
                value += item.Attributes.WeaponSpeed * 2;
			#endregion
            #region Start Absorption Attributes
            if (item.AbsorptionAttributes.CastingFocus > 0)
                value += item.AbsorptionAttributes.CastingFocus;
            if (item.AbsorptionAttributes.EaterCold > 0)
                value += item.AbsorptionAttributes.EaterCold;
            if (item.AbsorptionAttributes.EaterDamage > 0)
                value += item.AbsorptionAttributes.EaterDamage;
            if (item.AbsorptionAttributes.EaterEnergy > 0)
                value += item.AbsorptionAttributes.EaterEnergy;
            if (item.AbsorptionAttributes.EaterFire > 0)
                value += item.AbsorptionAttributes.EaterFire;
            if (item.AbsorptionAttributes.EaterKinetic > 0)
                value += item.AbsorptionAttributes.EaterKinetic;
            if (item.AbsorptionAttributes.EaterPoison > 0)
                value += item.AbsorptionAttributes.EaterPoison;
            if (item.AbsorptionAttributes.ResonanceCold > 0)
                value += item.AbsorptionAttributes.ResonanceCold;
            if (item.AbsorptionAttributes.ResonanceEnergy > 0)
                value += item.AbsorptionAttributes.ResonanceEnergy;
            if (item.AbsorptionAttributes.ResonanceFire > 0)
                value += item.AbsorptionAttributes.ResonanceFire;
            if (item.AbsorptionAttributes.ResonanceKinetic > 0)
                value += item.AbsorptionAttributes.ResonanceKinetic;
            if (item.AbsorptionAttributes.ResonancePoison > 0)
                value += item.AbsorptionAttributes.ResonancePoison;
            if (item.AbsorptionAttributes.SoulChargeCold > 0)
                value += item.AbsorptionAttributes.SoulChargeCold;
            if (item.AbsorptionAttributes.SoulChargeEnergy > 0)
                value += item.AbsorptionAttributes.SoulChargeEnergy;
            if (item.AbsorptionAttributes.SoulChargeFire > 0)
                value += item.AbsorptionAttributes.SoulChargeFire;
            if (item.AbsorptionAttributes.SoulChargeKinetic > 0)
                value += item.AbsorptionAttributes.SoulChargeKinetic;
            if (item.AbsorptionAttributes.SoulChargePoison > 0)
                value += item.AbsorptionAttributes.SoulChargePoison;
			#endregion
            #region Start Element Damage
            if (item.AosElementDamages.Chaos > 0)
                value += item.AosElementDamages.Chaos / 2;
            if (item.AosElementDamages.Cold > 0)
                value += item.AosElementDamages.Cold / 2;
            if (item.AosElementDamages.Direct > 0)
                value += item.AosElementDamages.Direct / 2;
            if (item.AosElementDamages.Energy > 0)
                value += item.AosElementDamages.Energy / 2;
            if (item.AosElementDamages.Fire > 0)
                value += item.AosElementDamages.Fire / 2;
            if (item.AosElementDamages.Poison > 0)
                value += item.AosElementDamages.Poison / 2;
			return value;
			#endregion
        }
		#endregion
		#region ArmorVariables
		public static int CheckArmor(BaseArmor item)
		{
			int value = 0;
			foreach (int i in Enum.GetValues(typeof(AosAttribute)))
			{
				if (item != null && item.Attributes[(AosAttribute)i] > 0)
					value += 2;
			}
			foreach (int i in Enum.GetValues(typeof(AosArmorAttribute)))
			{
				if (item.ArmorAttributes[(AosArmorAttribute)i] > 0)
					value += 2;
			}
			if (item.SkillBonuses.Skill_1_Value > 0)
			{
				value += (int)item.SkillBonuses.Skill_1_Value * 4;
				value += 2;
			}
			if (item.SkillBonuses.Skill_2_Value > 0)
			{
				value += (int)item.SkillBonuses.Skill_2_Value * 4;
				value += 2;
			}
			if (item.SkillBonuses.Skill_3_Value > 0)
			{
				value += (int)item.SkillBonuses.Skill_3_Value * 4;
				value += 2;
			}
			if (item.SkillBonuses.Skill_4_Value > 0)
			{
				value += (int)item.SkillBonuses.Skill_4_Value * 4;
				value += 2;
			}
			if (item.SkillBonuses.Skill_5_Value > 0)
			{
				value += (int)item.SkillBonuses.Skill_5_Value * 4;
				value += 2;
			}
			#region Start armor attributes
			if (item.ArmorAttributes.DurabilityBonus > 0)
				value += item.ArmorAttributes.DurabilityBonus / 4;
			if (item.ArmorAttributes.LowerStatReq > 0)
				value += item.ArmorAttributes.LowerStatReq / 4;
			if (item.ArmorAttributes.MageArmor > 0)
				value += 10;
			if (item.ArmorAttributes.SelfRepair > 0)
				value += item.ArmorAttributes.SelfRepair * 2;
			#endregion
			#region Start standard attributes
			if (item.Attributes.AttackChance > 0)
				value += item.Attributes.AttackChance * 2;
			if (item.Attributes.BonusDex > 0)
				value += item.Attributes.BonusDex * 4;
			if (item.Attributes.BonusHits > 0)
				value += item.Attributes.BonusHits * 2;
			if (item.Attributes.BonusInt > 0)
				value += item.Attributes.BonusInt * 4;
			if (item.Attributes.BonusMana > 0)
				value += item.Attributes.BonusMana * 2;
			if (item.Attributes.BonusStam > 0)
				value += item.Attributes.BonusStam * 2;
			if (item.Attributes.BonusStr > 0)
				value += item.Attributes.BonusStr * 4;
			if (item.Attributes.CastRecovery > 0)
				value += item.Attributes.CastRecovery * 10;
			if (item.Attributes.CastSpeed > 0)
				value += item.Attributes.CastSpeed * 10;
			if (item.Attributes.DefendChance > 0)
				value += item.Attributes.DefendChance * 2;
			if (item.Attributes.EnhancePotions > 0)
				value += item.Attributes.EnhancePotions;
			if (item.Attributes.LowerManaCost > 0)
				value += item.Attributes.LowerManaCost * 2;
			if (item.Attributes.LowerRegCost > 0)
				value += item.Attributes.LowerRegCost;
			if (item.Attributes.Luck > 0)
				value += item.Attributes.Luck / 2;
			if (item.Attributes.NightSight > 0)
				value += 10;
			if (item.Attributes.ReflectPhysical > 0)
				value += item.Attributes.ReflectPhysical * 2;
			if (item.Attributes.RegenHits > 0)
				value += item.Attributes.RegenHits * 10;
			if (item.Attributes.RegenMana > 0)
				value += item.Attributes.RegenMana * 10;
			if (item.Attributes.RegenStam > 0)
				value += item.Attributes.RegenStam * 10;
			if (item.Attributes.SpellChanneling > 0)
				value += 10;
			if (item.Attributes.SpellDamage > 0)
				value += item.Attributes.SpellDamage * 2;
			if (item.Attributes.WeaponDamage > 0)
				value += item.Attributes.WeaponDamage * 2;
			if (item.Attributes.WeaponSpeed > 0)
				value += item.Attributes.WeaponSpeed * 2;
			#endregion
			#region Start Absorption Attributes
			if (item.AbsorptionAttributes.CastingFocus > 0)
				value += item.AbsorptionAttributes.CastingFocus;
			if (item.AbsorptionAttributes.EaterCold > 0)
				value += item.AbsorptionAttributes.EaterCold;
			if (item.AbsorptionAttributes.EaterDamage > 0)
				value += item.AbsorptionAttributes.EaterDamage;
			if (item.AbsorptionAttributes.EaterEnergy > 0)
				value += item.AbsorptionAttributes.EaterEnergy;
			if (item.AbsorptionAttributes.EaterFire > 0)
				value += item.AbsorptionAttributes.EaterFire;
			if (item.AbsorptionAttributes.EaterKinetic > 0)
				value += item.AbsorptionAttributes.EaterKinetic;
			if (item.AbsorptionAttributes.EaterPoison > 0)
				value += item.AbsorptionAttributes.EaterPoison;
			if (item.AbsorptionAttributes.ResonanceCold > 0)
				value += item.AbsorptionAttributes.ResonanceCold;
			if (item.AbsorptionAttributes.ResonanceEnergy > 0)
				value += item.AbsorptionAttributes.ResonanceEnergy;
			if (item.AbsorptionAttributes.ResonanceFire > 0)
				value += item.AbsorptionAttributes.ResonanceFire;
			if (item.AbsorptionAttributes.ResonanceKinetic > 0)
				value += item.AbsorptionAttributes.ResonanceKinetic;
			if (item.AbsorptionAttributes.ResonancePoison > 0)
				value += item.AbsorptionAttributes.ResonancePoison;
			if (item.AbsorptionAttributes.SoulChargeCold > 0)
				value += item.AbsorptionAttributes.SoulChargeCold;
			if (item.AbsorptionAttributes.SoulChargeEnergy > 0)
				value += item.AbsorptionAttributes.SoulChargeEnergy;
			if (item.AbsorptionAttributes.SoulChargeFire > 0)
				value += item.AbsorptionAttributes.SoulChargeFire;
			if (item.AbsorptionAttributes.SoulChargeKinetic > 0)
				value += item.AbsorptionAttributes.SoulChargeKinetic;
			if (item.AbsorptionAttributes.SoulChargePoison > 0)
				value += item.AbsorptionAttributes.SoulChargePoison;
			#endregion
			#region Start Resist Bonus
			if (item.ColdBonus > 0)
			{
				value += item.ColdBonus * 2;
				value += 2;
			}
			if (item.EnergyBonus > 0)
			{
				value += item.EnergyBonus * 2;
				value += 2;
			}
			if (item.FireBonus > 0)
			{
				value += item.FireBonus * 2;
				value += 2;
			}
			if (item.PhysicalBonus > 0)
			{
				value += item.PhysicalBonus * 2;
				value += 2;
			}
			if (item.PoisonBonus > 0)
			{
				value += item.PoisonBonus * 2;
				value += 2;
			}
			#endregion	
			return value;
		}
		#endregion
		#region ClothingVariables
        public static int CheckClothing(BaseClothing item)
        {
            int value = 0;
            foreach (int i in Enum.GetValues(typeof(AosAttribute)))
            {
                if (item != null && item.Attributes[(AosAttribute)i] > 0)
                    value += 2;
            }
			#region Start skill bonus
            if (item.SkillBonuses.Skill_1_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_1_Value * 4;
                value += 2;
            }
            if (item.SkillBonuses.Skill_2_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_2_Value * 4;
                value += 2;
            }
            if (item.SkillBonuses.Skill_3_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_3_Value * 4;
                value += 2;
            }
            if (item.SkillBonuses.Skill_4_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_4_Value * 4;
                value += 2;
            }
            if (item.SkillBonuses.Skill_5_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_5_Value * 4;
                value += 2;
            }
			#endregion
			#region Start standard attributes
            if (item.Attributes.AttackChance > 0)
                value += item.Attributes.AttackChance * 2;
            if (item.Attributes.BonusDex > 0)
                value += item.Attributes.BonusDex * 4;
            if (item.Attributes.BonusHits > 0)
                value += item.Attributes.BonusHits * 2;
            if (item.Attributes.BonusInt > 0)
                value += item.Attributes.BonusInt * 4;
            if (item.Attributes.BonusMana > 0)
                value += item.Attributes.BonusMana * 2;
            if (item.Attributes.BonusStam > 0)
                value += item.Attributes.BonusStam * 2;
            if (item.Attributes.BonusStr > 0)
                value += item.Attributes.BonusStr * 4;
            if (item.Attributes.CastRecovery > 0)
                value += item.Attributes.CastRecovery * 10;
            if (item.Attributes.CastSpeed > 0)
                value += item.Attributes.CastSpeed * 10;
            if (item.Attributes.DefendChance > 0)
                value += item.Attributes.DefendChance * 2;
            if (item.Attributes.EnhancePotions > 0)
                value += item.Attributes.EnhancePotions;
            if (item.Attributes.LowerManaCost > 0)
                value += item.Attributes.LowerManaCost * 2;
            if (item.Attributes.LowerRegCost > 0)
                value += item.Attributes.LowerRegCost;
            if (item.Attributes.Luck > 0)
                value += item.Attributes.Luck / 2;
            if (item.Attributes.NightSight > 0)
                value += 10;
            if (item.Attributes.ReflectPhysical > 0)
                value += item.Attributes.ReflectPhysical * 2;
            if (item.Attributes.RegenHits > 0)
                value += item.Attributes.RegenHits * 10;
            if (item.Attributes.RegenMana > 0)
                value += item.Attributes.RegenMana * 10;
            if (item.Attributes.RegenStam > 0)
                value += item.Attributes.RegenStam * 10;
            if (item.Attributes.SpellChanneling > 0)
                value += 10;
            if (item.Attributes.SpellDamage > 0)
                value += item.Attributes.SpellDamage * 2;
            if (item.Attributes.WeaponDamage > 0)
                value += item.Attributes.WeaponDamage * 2;
            if (item.Attributes.WeaponSpeed > 0)
                value += item.Attributes.WeaponSpeed * 2;
			#endregion
			#region Start Element Resist
            if (item.Resistances.Chaos > 0)
                value += item.Resistances.Chaos;
            if (item.Resistances.Cold > 0)
                value += item.Resistances.Cold;
            if (item.Resistances.Direct > 0)
                value += item.Resistances.Direct;
            if (item.Resistances.Energy > 0)
                value += item.Resistances.Energy;
            if (item.Resistances.Fire > 0)
                value += item.Resistances.Fire;
            if (item.Resistances.Physical > 0)
                value += item.Resistances.Physical;
            if (item.Resistances.Poison > 0)
                value += item.Resistances.Poison;
			return value;
			#endregion
        }
		#endregion
		#region JeweleryVariables
        public static int CheckJewel(BaseJewel item)
        {
            int value = 0;

            foreach (int i in Enum.GetValues(typeof(AosAttribute)))
            {
                if (item != null && item.Attributes[(AosAttribute)i] > 0)
                    value += 2;
            }
			#region Start skill bonus

            if (item.SkillBonuses.Skill_1_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_1_Value * 4;
                value += 2;
            }

            if (item.SkillBonuses.Skill_2_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_2_Value * 4;
                value += 2;
            }

            if (item.SkillBonuses.Skill_3_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_3_Value * 4;
                value += 2;
            }

            if (item.SkillBonuses.Skill_4_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_4_Value * 4;
                value += 2;
            }

            if (item.SkillBonuses.Skill_5_Value > 0)
            {
                value += (int)item.SkillBonuses.Skill_5_Value * 4;
                value += 2;
            }

			#endregion
			#region Start standard attributes

            if (item.Attributes.AttackChance > 0)
                value += item.Attributes.AttackChance * 2;

            if (item.Attributes.BonusDex > 0)
                value += item.Attributes.BonusDex * 4;

            if (item.Attributes.BonusHits > 0)
                value += item.Attributes.BonusHits * 2;

            if (item.Attributes.BonusInt > 0)
                value += item.Attributes.BonusInt * 4;

            if (item.Attributes.BonusMana > 0)
                value += item.Attributes.BonusMana * 2;

            if (item.Attributes.BonusStam > 0)
                value += item.Attributes.BonusStam * 2;

            if (item.Attributes.BonusStr > 0)
                value += item.Attributes.BonusStr * 4;

            if (item.Attributes.CastRecovery > 0)
                value += item.Attributes.CastRecovery * 10;

            if (item.Attributes.CastSpeed > 0)
                value += item.Attributes.CastSpeed * 10;

            if (item.Attributes.DefendChance > 0)
                value += item.Attributes.DefendChance * 2;

            if (item.Attributes.EnhancePotions > 0)
                value += item.Attributes.EnhancePotions;

            if (item.Attributes.LowerManaCost > 0)
                value += item.Attributes.LowerManaCost * 2;

            if (item.Attributes.LowerRegCost > 0)
                value += item.Attributes.LowerRegCost;

            if (item.Attributes.Luck > 0)
                value += item.Attributes.Luck / 2;

            if (item.Attributes.NightSight > 0)
                value += 10;

            if (item.Attributes.ReflectPhysical > 0)
                value += item.Attributes.ReflectPhysical * 2;

            if (item.Attributes.RegenHits > 0)
                value += item.Attributes.RegenHits * 10;

            if (item.Attributes.RegenMana > 0)
                value += item.Attributes.RegenMana * 10;

            if (item.Attributes.RegenStam > 0)
                value += item.Attributes.RegenStam * 10;

            if (item.Attributes.SpellChanneling > 0)
                value += 10;

            if (item.Attributes.SpellDamage > 0)
                value += item.Attributes.SpellDamage * 2;

            if (item.Attributes.WeaponDamage > 0)
                value += item.Attributes.WeaponDamage * 2;

            if (item.Attributes.WeaponSpeed > 0)
                value += item.Attributes.WeaponSpeed * 2;

			#endregion
			#region Start Absorption Attributes

            if (item.AbsorptionAttributes.CastingFocus > 0)
                value += item.AbsorptionAttributes.CastingFocus;

            if (item.AbsorptionAttributes.EaterCold > 0)
                value += item.AbsorptionAttributes.EaterCold;

            if (item.AbsorptionAttributes.EaterDamage > 0)
                value += item.AbsorptionAttributes.EaterDamage;

            if (item.AbsorptionAttributes.EaterEnergy > 0)
                value += item.AbsorptionAttributes.EaterEnergy;

            if (item.AbsorptionAttributes.EaterFire > 0)
                value += item.AbsorptionAttributes.EaterFire;

            if (item.AbsorptionAttributes.EaterKinetic > 0)
                value += item.AbsorptionAttributes.EaterKinetic;

            if (item.AbsorptionAttributes.EaterPoison > 0)
                value += item.AbsorptionAttributes.EaterPoison;

            if (item.AbsorptionAttributes.ResonanceCold > 0)
                value += item.AbsorptionAttributes.ResonanceCold;

            if (item.AbsorptionAttributes.ResonanceEnergy > 0)
                value += item.AbsorptionAttributes.ResonanceEnergy;

            if (item.AbsorptionAttributes.ResonanceFire > 0)
                value += item.AbsorptionAttributes.ResonanceFire;

            if (item.AbsorptionAttributes.ResonanceKinetic > 0)
                value += item.AbsorptionAttributes.ResonanceKinetic;

            if (item.AbsorptionAttributes.ResonancePoison > 0)
                value += item.AbsorptionAttributes.ResonancePoison;

            if (item.AbsorptionAttributes.SoulChargeCold > 0)
                value += item.AbsorptionAttributes.SoulChargeCold;

            if (item.AbsorptionAttributes.SoulChargeEnergy > 0)
                value += item.AbsorptionAttributes.SoulChargeEnergy;

            if (item.AbsorptionAttributes.SoulChargeFire > 0)
                value += item.AbsorptionAttributes.SoulChargeFire;

            if (item.AbsorptionAttributes.SoulChargeKinetic > 0)
                value += item.AbsorptionAttributes.SoulChargeKinetic;

            if (item.AbsorptionAttributes.SoulChargePoison > 0)
                value += item.AbsorptionAttributes.SoulChargePoison;

			#endregion
			#region Start Element Resist

            if (item.Resistances.Chaos > 0)
                value += item.Resistances.Chaos;

            if (item.Resistances.Cold > 0)
                value += item.Resistances.Cold;

            if (item.Resistances.Direct > 0)
                value += item.Resistances.Direct;

            if (item.Resistances.Energy > 0)
                value += item.Resistances.Energy;

            if (item.Resistances.Fire > 0)
                value += item.Resistances.Fire;

            if (item.Resistances.Physical > 0)
                value += item.Resistances.Physical;

            if (item.Resistances.Poison > 0)
                value += item.Resistances.Poison;
				
			return value;
			#endregion
        }
		#endregion
		
		/* Vendor Discount and Level Requirements! */
		private static Type[] _TypesVendor1 = new[] 
		{ 
			typeof(LevelVendor10),typeof(LevelVendor20),typeof(LevelVendor30),typeof(LevelVendor40),
			typeof(LevelVendor50),typeof(LevelVendor60),typeof(LevelVendor70),typeof(LevelVendor80),
			typeof(LevelVendor90),typeof(LevelVendor100),typeof(LevelVendor120),typeof(LevelVendor140),
			typeof(LevelVendor160),typeof(LevelVendor180),typeof(LevelVendor200),typeof(LevelVendor230),
			typeof(LevelVendor250)
		};
		public static bool VendorDiscountsExt(Type test)
		{
			foreach ( var type in _TypesVendor1 )
			{
				if ( test == type || test.IsSubclassOf (type))
					return true;
			}
			return false;
		/* Vendor Discount and Level Requirements! */
		}
	}
}