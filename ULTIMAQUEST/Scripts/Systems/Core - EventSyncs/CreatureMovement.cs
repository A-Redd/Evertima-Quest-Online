using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Items
{
    public class CreatureMovement
    {
        public static void Initialize()
        {
            EventSink.Movement += EventSink_Movement;
        }
        public static void EventSink_Movement(MovementEventArgs e)
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
			
			if (m_ItemxmlSys.PlayerLevels == true && m_ItemxmlSys.EnabledLevelPets == true)
			{
				Mobile from = e.Mobile;
				PlayerMobile pm = from as PlayerMobile;
				if (pm != null)
				{
					PlayerMobile master = (PlayerMobile)pm;
					
					LevelSheet xmlplayer = null;
					xmlplayer = master.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
					if (xmlplayer == null)
					{
						pm.AddToBackpack(new LevelSheet());
					}
					/* move all checks to here, not using the statics */
					
					List<Mobile> pets = master.AllFollowers;
					if (pets.Count > 0)
					{
						for (int i = 0; i < pets.Count; ++i)
						{
							Mobile pet = (Mobile)pets[i];
							
							addlevelorb(pet, m_ItemxmlSys);
							PetAttackBonus(pet, m_ItemxmlSys, xmlplayer);
							PetStatBonus(pm, pet, xmlplayer);
							MountCheckExt(pet, pm, m_ItemxmlSys, xmlplayer);
							OrigOwnerChecker(pet, m_ItemxmlSys);
						}
					}
				}
			}
        }
		
		public static void OrigOwnerChecker (Mobile m, LevelControlSys m_ItemxmlSys)
		{
			BaseCreature bc = m as BaseCreature;
			Mobile master = bc.ControlMaster;
			PetLevelOrb petorb = null;
			petorb = bc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;
			if (petorb == null)
				addlevelorb(bc, m_ItemxmlSys);
			else
				petorb.OriginalMaster = master.Serial;
		}
		
		public static void addlevelorb (Mobile m, LevelControlSys m_ItemxmlSys)
		{
			BaseCreature bc = m as BaseCreature;
//			Container pack = bc.Backpack;
			
			PetLevelOrb petorb = null;
			petorb = bc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;
			
			var BankBoxVar = bc.FindItemOnLayer(Layer.Bank);
			Mobile master = bc.ControlMaster;
			
			if (BankBoxVar != null)
			{
				if (petorb != null)
				{
					return;
				}
				else
				{
					PetLevelOrb petorb1 = new PetLevelOrb();
					BankBoxVar.Delete();
//					bc.AddItem(BankBoxVar = new BankBox(bc));
					bc.BankBox.AddItem(petorb1);
					
					if (m_ItemxmlSys.UseDynamicMaxLevels == true)
					{
						PetFeatureHandlerExt.PetLevelCaps(bc,petorb1);
					}
					else
					{
						petorb1.MaxLevel = m_ItemxmlSys.StartMaxLvl;
						petorb1.OriginalMaster = master.Serial;
					}
					
					bc.InvalidateProperties();
					return;
				}
			}
			else
			{
//				bc.AddItem(BankBoxVar = new BankBox(bc));
				PetLevelOrb petorb1 = new PetLevelOrb();
				bc.BankBox.AddItem(petorb1);
				if (m_ItemxmlSys.UseDynamicMaxLevels == true)
				{
					PetFeatureHandlerExt.PetLevelCaps(bc,petorb1);
				}
				else
				{
					petorb1.MaxLevel = m_ItemxmlSys.StartMaxLvl;
					petorb1.OriginalMaster = master.Serial;
				}
				bc.InvalidateProperties();
				return;
			}
		}
	
		public static void PetStatBonus(Mobile player, Mobile pet, LevelSheet xmlplayer)
		{
			if (pet is PlayerMobile)
			{
				return;
			}
			BaseCreature bc = (BaseCreature)pet;

			if (bc.Controlled == true && bc != null)
			{
				PetFeatureHandlerExt.OnMovementPetBonus(bc, player, xmlplayer);
				return;
			}
		}
		public static void PetAttackBonus(Mobile pet, LevelControlSys m_ItemxmlSys, LevelSheet xmlplayer)
		{
			if (pet is PlayerMobile)
			{
				return;
			}
			BaseCreature bc = (BaseCreature)pet;
			
			var BankBoxVar = bc.FindItemOnLayer(Layer.Bank);
			if (BankBoxVar == null)
				return;
			
			PetLevelOrb petorb = null;
			petorb = bc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;
			if (petorb == null)
				return;

			if (bc.Controlled == true && bc != null)
			{
				PetFeatureHandlerExt.OnMovementPetAttacks(pet, m_ItemxmlSys, petorb, xmlplayer);
				return;
			}
		}
		public static void MountCheckExt (Mobile pet, Mobile owner, LevelControlSys m_ItemxmlSys, LevelSheet xmlplayer)
		{
			if (pet is PlayerMobile)
			{
				return;
			}
			if (m_ItemxmlSys.EnableMountCheck == false)
				return;

			if (xmlplayer == null)
				return;
			
			if (owner.Player && owner.Mounted)
			{
				MountEXTCheck.MountCheckVoid(pet, owner, m_ItemxmlSys, xmlplayer);
			}
		}
	}
}
