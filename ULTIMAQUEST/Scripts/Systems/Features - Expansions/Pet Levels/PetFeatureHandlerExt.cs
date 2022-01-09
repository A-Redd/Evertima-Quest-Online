using System;
using Server;
using System.Xml;
using Server.Misc;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Targeting;
using System.Collections;
using Server.Engines.Craft;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Engines.LevelSystemExtAtt;


namespace Server
{
    public class PetFeatureHandlerExt
    {
		public static void PetLevelProps(Mobile m, ObjectPropertyList list)
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
			
			if (m_ItemxmlSys.PlayerLevels == false)
				return;
				 
			BaseCreature bc = m as BaseCreature;
			
			if (m_ItemxmlSys.EnabledLevelPets == true && bc.Controlled == true)
			{
				var BankBoxVar = bc.FindItemOnLayer(Layer.Bank);
				if (BankBoxVar != null)
				{
					PetLevelOrb petxml7 = null;
					petxml7 = bc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;
					if (m_ItemxmlSys.LevelBelowPet == true)
					{
						if (petxml7 != null)
						{
							int petlevelint = LevelCore.PetLevelXML(bc);
							list.Add("<BASEFONT COLOR=#7FCAE7>Tamed Level: <BASEFONT COLOR=#17FF01>" + petlevelint);
						}
					}
				}
			}
			if (!bc.Controlled) 
			{
				if (m_ItemxmlSys.UntamedCreatureLevels == true)
				{
						int cl = LevelCore.CreatureLevel(bc);
						if (cl > 0)
						{
							list.Add("<BASEFONT COLOR=#7FCAE7>Feral Level: <BASEFONT COLOR=#17FF01>" + cl);
						}
				}
			}
			if (bc is BaseVendor) 
			{
				
				if (m_ItemxmlSys.ShowVendorLevels == true)
				{
						int cl = LevelCore.BaseVendorLevel(bc);
						if (cl > 0)
						{
							list.Add("<BASEFONT COLOR=#7FCAE7>Level: <BASEFONT COLOR=#17FF01>" + cl);
						}
				}
			}
        }
		public static void MassProvokeXML(BaseCreature mobile, Mobile master)
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
			
			ArrayList list = new ArrayList();

			foreach ( Mobile m in mobile.GetMobilesInRange( 5 ) )
			{
				if (m is BaseCreature && !((BaseCreature)m).Controlled)
				{
					if (m == master)
						return;
					else
						list.Add( m );
				}
			}

			foreach ( Mobile m in list )
			{
				m.DoHarmful( m );
				master.Combatant = null;
				m.Combatant = master;
				m.PlaySound( 0x403 );
				if (m_ItemxmlSys.EmoteOnSpecialAtks == true)
				{
					m.Emote("*you see {0} looks furious*", m.Name);
				}
			}
		}
		
		public static void TeleToTarget(BaseCreature mobile, Mobile master)
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
			
			ArrayList list = new ArrayList();

			foreach ( Mobile m in mobile.GetMobilesInRange( 10 ) )
			{
				if (m is BaseCreature && !((BaseCreature)m).Controlled)
				{
					if (m == master)
						return;
					else
						list.Add( m );
				}
			}

			foreach ( Mobile m in list )
			{
				Point3D from = mobile.Location;
				Point3D to = master.Location;
				if ( m.Mana >= 10 )
				{
					m.DoHarmful( m );
					m.Location = to;
					m.Mana -= 10;
					if (m_ItemxmlSys.EmoteOnSpecialAtks)
					{
						m.Say( "Grrrr" );
					}
					Effects.SendLocationParticles( EffectItem.Create( from, m.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
					Effects.SendLocationParticles( EffectItem.Create(   to, m.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );
					m.PlaySound( 0x1FE );
				}
			}
		}
		
		public static void MassPeaceXML(BaseCreature mobile, Mobile master)
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
			
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in mobile.GetMobilesInRange( 5 ) )
			{
				if (m is BaseCreature && !((BaseCreature)m).Controlled)
				{
					if (m == master)
						return;
					else
						list.Add( m );
				}
			}

			foreach ( Mobile m in list )
			{
				m.DoHarmful( m );
				m.Combatant = null;
				m.PlaySound( 0x418 );
				if (m_ItemxmlSys.EmoteOnSpecialAtks)
				{
					m.Emote("*you see {0} looks peacful*", m.Name);
				}
			}
		}
		
		public static void SuperHealXML(BaseCreature bc, Mobile player)
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
			
			if (!bc.Alive)
                    bc.Resurrect();
			if (bc.Hits < bc.HitsMax)
				bc.Hits = bc.HitsMax;
			if (bc.Mana < bc.ManaMax)
				bc.Mana = bc.ManaMax;
			if (bc.Stam < bc.StamMax)
				bc.Stam = bc.StamMax;
			if (bc.Poison != null)
				bc.Poison = null;
			if (bc.Paralyzed == true)
				bc.Paralyzed = false;
			EvilOmenSpell.TryEndEffect(bc);
			StrangleSpell.RemoveCurse(bc);
			CorpseSkinSpell.RemoveCurse(bc);
			CurseSpell.RemoveEffect(bc);
			MortalStrike.EndWound(bc);
			BloodOathSpell.RemoveCurse(bc);
			MindRotSpell.ClearMindRotScalar(bc);
			bc.Loyalty = BaseCreature.MaxLoyalty;
			Effects.SendTargetEffect(bc, 0x3709, 32);
			Effects.SendTargetEffect(bc, 0x376A, 32);
			bc.PlaySound(0x208);
			if (m_ItemxmlSys.EmoteOnSpecialAtks)
			{
				bc.Emote("*you see {0} looks refreshed!*", bc.Name);
			}
		}
		
		public static void BlessedXML(BaseCreature bc, Mobile player)
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
			
			if (m_ItemxmlSys.EmoteOnSpecialAtks)
			{
				bc.Emote("*you see {0} looks ready to fight harder!*", bc.Name);
			}
			bc.AddStatMod(new StatMod(StatType.Dex, "XmlDex", Utility.RandomMinMax( 20, 60 ), TimeSpan.FromSeconds( 25.0 )));
			bc.AddStatMod(new StatMod(StatType.Str, "XmlStr", Utility.RandomMinMax( 20, 60 ), TimeSpan.FromSeconds( 25.0 )));
			bc.AddStatMod(new StatMod(StatType.Int, "XmlInt", Utility.RandomMinMax( 20, 60 ), TimeSpan.FromSeconds( 25.0 )));
			bc.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			bc.PlaySound(0x1EA);
			bc.InvalidateProperties();
		}
		
		public static void AreaFireBlastXML(BaseCreature mobile, Mobile master)
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
			
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in mobile.GetMobilesInRange( 5 ) )
			{
				if (m is BaseCreature && !((BaseCreature)m).Controlled)
				{
					if (m == master)
						return;
					else
						list.Add( m );
				}
			}
			if (m_ItemxmlSys.EmoteOnSpecialAtks)
			{
				mobile.Emote("*{0} shoots fire at surounding area!*", mobile.Name);
			}
			mobile.Mana -= 25;
			mobile.Stam -= 15;
			foreach ( Mobile m in list )
			{
				m.DoHarmful( m );
				m.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.Waist );
				m.PlaySound( 0x208 );
				m.Damage (((Utility.Random( 25, 35 )) - (m.FireResistance /2)));
			}
		}
		
		public static void AreaIceBlastXML(BaseCreature mobile, Mobile master)
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
			
			ArrayList list = new ArrayList();

			foreach ( Mobile m in mobile.GetMobilesInRange( 5 ) )
			{
				if (m is BaseCreature && !((BaseCreature)m).Controlled)
				{
					if (m == master)
						return;
					else
						list.Add( m );
				}
			}
			if (m_ItemxmlSys.EmoteOnSpecialAtks)
			{
				mobile.Emote("*{0} shoots ice breath at surounding area!*", mobile.Name);
			}
			mobile.Mana -= 25;
			mobile.Stam -= 15;
			foreach ( Mobile m in list )
			{
				m.DoHarmful( m );
				m.FixedParticles( 0x1fb7, 50, 50, 5052, EffectLayer.Waist );
				m.PlaySound( 279 );
				m.PlaySound( 280 );
				m.Damage( ((Utility.Random( 25, 35 )) - (m.ColdResistance /2)) );
			}
		}
		
		public static void AreaAirBlastXML(BaseCreature mobile, Mobile master)
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
			
			ArrayList list = new ArrayList();

			foreach ( Mobile m in mobile.GetMobilesInRange( 5 ) )
			{
				if (m is BaseCreature && !((BaseCreature)m).Controlled)
				{
					if (m == master)
						return;
					else
						list.Add( m );
				}
			}
			if (m_ItemxmlSys.EmoteOnSpecialAtks)
			{
				mobile.Emote("*{0} shoots sharp air at surounding area!*", mobile.Name);
			}
			mobile.Mana -= 25;
			mobile.Stam -= 15;
			foreach ( Mobile m in list )
			{
				m.DoHarmful( m );
				m.FixedParticles( 0x3728, 50, 50, 5052, EffectLayer.Waist );
				m.PlaySound( 655 );
				int toStrike = Utility.RandomMinMax( 25, 35 );
				m.Damage( toStrike, mobile );
			}
		}
		
		public static void OnWeaponHit(Mobile attacker, Mobile defender)
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
			
			Container packxml1 = attacker.Backpack;
			if(packxml1 != null)
			{
				BaseCreature bc = attacker as BaseCreature;
				PetLevelOrb petxml = null;
				petxml = bc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;	
				
				if (petxml == null)
					return;
				
				Mobile master = bc.GetMaster();
				bool VarAreaFireBlast = petxml.AreaFireBlast;
				bool VarAreaIceBlast = petxml.AreaIceBlast;
				bool VarAreaAirBlast = petxml.AreaAirBlast;

//				AreaFireBlastXML(bc, master);
				
				switch (Utility.RandomMinMax(1, 8))
				{
					case 1:	if (VarAreaFireBlast && m_ItemxmlSys.AreaFireBlastChance >= Utility.RandomDouble()) // 0.5 is 50% , 0.05 is 5%
							{
								if (m_ItemxmlSys.AreaFireBlast == false)
									return;
								if (petxml.Levell < m_ItemxmlSys.AreaFireBlastReq)
									return;
								AreaFireBlastXML(bc, master);
							}	break;
					case 2:	if (VarAreaIceBlast && m_ItemxmlSys.AreaIceBlastChance >= Utility.RandomDouble())
							{
								if (m_ItemxmlSys.AreaIceBlast == false)
									return;
								if (petxml.Levell < m_ItemxmlSys.AreaIceBlastReq)
									return;
								AreaIceBlastXML(bc, master);
							};	break;
					case 3: if (VarAreaAirBlast && m_ItemxmlSys.AreaAirBlastChance >= Utility.RandomDouble())
							{
								if (m_ItemxmlSys.AreaAirBlast == false)
									return;
								if (petxml.Levell < m_ItemxmlSys.AreaAirBlastReq)
									return;
								AreaAirBlastXML(bc, master);
							};	break;				
					case 4:	if (petxml.MassProvoke && m_ItemxmlSys.MassProvokeChance >= Utility.RandomDouble())
							{
								if (m_ItemxmlSys.MassProvokeToAtt == false)
									return;
								if (petxml.Levell < m_ItemxmlSys.MassProvokeToAttReq)
									return;
								MassProvokeXML(bc, master);
							};	break;
					case 5: if (petxml.MassPeace && m_ItemxmlSys.MassPeaceChance	  >= Utility.RandomDouble())
							{
								if (m_ItemxmlSys.MassPeaceArea == false)
									return;
								if (petxml.Levell < m_ItemxmlSys.MassPeaceReq)
									return;
								MassPeaceXML(bc, master);
							};	break;
					case 6: if (petxml.SuperHeal && m_ItemxmlSys.SuperHealChance  >= Utility.RandomDouble())
							{
								if (m_ItemxmlSys.SuperHeal == false)
									return;
								if (petxml.Levell < m_ItemxmlSys.SuperHealReq)
									return;

								SuperHealXML(bc, master);
							};	break;
					case 7: if (petxml.BlessedPower && m_ItemxmlSys.BlessedPowerChance  >= Utility.RandomDouble())
							{
								if (m_ItemxmlSys.BlessedPower == false)
									return;
								if (petxml.Levell < m_ItemxmlSys.BlessedPowerReq)
									return;
								BlessedXML(bc, master);
							};	break;
				}
			}
		}
		public static void OnMovementPetAttacks(Mobile m, LevelControlSys m_ItemxmlSys, PetLevelOrb petxml, LevelSheet xmlplayer)
		{	
			BaseCreature bc = m as BaseCreature;

			Mobile master = null;
			master = bc.GetMaster();

			if (petxml.TeleportToTarget && m_ItemxmlSys.TelePortToTarChance >= Utility.RandomDouble()) // 0.5 is 50% , 0.05 is 5%
			{
				if (m_ItemxmlSys.TelePortToTarget == false)
					return;
				if (petxml.Levell < m_ItemxmlSys.TelePortToTargetReq)
					return;
				if (bc.Combatant == null )
					return;
				TeleToTarget(bc, master);
				return;
			}

			

		}
		public static void OnMovementPetBonus(BaseCreature bc, Mobile agro, LevelSheet xmlplayer)
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
			
			Container packxml1 = bc.Backpack;
			if(packxml1 != null)
			{
				PetLevelOrb petxml = null;
				petxml = bc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;
				Mobile master = bc.ControlMaster;
				
				Container packxml2 = master.Backpack;
				if (packxml2 != null && petxml != null)
				{

					if (xmlplayer.AuraStatBoost == false)
					{
						if (m_ItemxmlSys.AuraStatBoost == false)
							return;
						if (petxml.Levell < m_ItemxmlSys.AuraStatBoostReq)
							return;
						if (xmlplayer == null)
							return;
						
						
						if (xmlplayer.Levell <= 10)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt10(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}					
						if (xmlplayer.Levell <= 20)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt20(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 30)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt30(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 40)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt40(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 50)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt50(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 60)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt60(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 70)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt70(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 80)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt80(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 90)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt90(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 100)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt100(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 140)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt140(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 160)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt160(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 180)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt180(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 200)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt200(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}
						if (xmlplayer.Levell <= 201)
						{
							xmlplayer.AuraStatBoost = true;
							StatBoostHandlerExt.BonusStatAtt201(master, m_ItemxmlSys);
							petxml.ToggleStatTimer(true, master);
							return;
						}

					}

				}
			}
		}
		public static void PetLevelCaps (BaseCreature bc, PetLevelOrb orb)
		{
			if (orb != null)
			{
				if (orb.OriginalMaster == null)
					return;
				
				int cl = LevelCore.CreatureLevel(bc);
				
				if (cl < 10)
				{
					orb.MaxLevel = 20;
					return;
				}
				else if (cl < 20)
				{
					orb.MaxLevel = 40;
					return;
				}
				else if (cl < 40)
				{
					orb.MaxLevel = 60;
					return;
				}
				else if (cl <= 60)
				{
					orb.MaxLevel = 100;
					return;
				}
				else if (cl >= 61)
				{
					orb.MaxLevel = 100;
					return;
				}
			}
			else
				return;
		}
	}
}
