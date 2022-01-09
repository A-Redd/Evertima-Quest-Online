using System;
using Server;
using System.Xml;
using Server.Misc;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using System.Collections;
using Server.Engines.Craft;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Engines.LevelSystemExtAtt;

namespace Server
{
    public class LevelHandlerExt
    {
        public ArrayList MemberCount = new ArrayList();

        public static void Set(Mobile killer, Mobile killed)
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
				
            PlayerMobile klr = null;
            Party pty = null;
            LevelHandlerExt lh = new LevelHandlerExt();
			LevelSheet xmlplayer = null;
			
			if (m_ItemxmlSys.EnabledLevelPets == true)
			{
				if (killer is PlayerMobile)
				{
					PlayerMobile master = (PlayerMobile)killer;
					List<Mobile> pets = master.AllFollowers;
					if (pets.Count > 0)
					{
					   for (int i = 0; i < pets.Count; ++i)
						{
							Mobile pet = (Mobile)pets[i];

							if (pet is IMount && ((IMount)pet).Rider != null)
							{
								if (m_ItemxmlSys.MountedPetsGainExp == false)
									return;
							}
							if (m_ItemxmlSys.EnabledLevelPets == true)
							LevelHandlerPetExt.Set(pet, killed);
						}
					}
				}		
			}

            if (killer is BaseCreature)
            {
                BaseCreature bc = killer as BaseCreature;
				BaseCreature bckilled = killed as BaseCreature;

                if (bc.Controlled && m_ItemxmlSys.PetKillGivesExp)
				{
                    klr = bc.GetMaster() as PlayerMobile;
					PlayerMobile master = (PlayerMobile)klr;
					List<Mobile> pets = master.AllFollowers;
					if (pets.Count > 0)
					{
					   for (int i = 0; i < pets.Count; ++i)
						{
							Mobile pet = (Mobile)pets[i];

							if (pet is IMount && ((IMount)pet).Rider != null)
							{
								if (m_ItemxmlSys.MountedPetsGainExp == false)
									return;
							}
							
							if (m_ItemxmlSys.EnabledLevelPets == true)
							LevelHandlerPetExt.Set(pet, killed);
						}
					}
				}
                if (bc.Summoned && m_ItemxmlSys.PetKillGivesExp)
				{
                    klr = bc.GetMaster() as PlayerMobile;
					LevelHandlerPetExt.Set(bc, bckilled);
				}	
            }
            else
            {
                if (killer is PlayerMobile) //double check ;)
                    klr = killer as PlayerMobile;
            }

            if (lh.MemberCount.Count > 0)
            {
                foreach (Mobile il in lh.MemberCount)
                {
                    lh.MemberCount.Remove(il);
                }
            }

            if (klr != null)
            {
				Mobile m = (Mobile)killer;
				pty = Party.Get(klr);
				if (m is PlayerMobile)
				{
					xmlplayer = killer.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
					
					if (xmlplayer.Levell < 1)
						xmlplayer.Levell = 1;

					if (xmlplayer.ToLevell < 50)
						xmlplayer.ToLevell = 50;

					if (!(xmlplayer.MaxLevel == m_ItemxmlSys.StartMaxLvl && xmlplayer.MaxLevel > m_ItemxmlSys.EndMaxLvl))
						xmlplayer.MaxLevel = m_ItemxmlSys.StartMaxLvl;
				}

                AddExp(klr, killed, pty);
            }
        }

        public static void AddExp(Mobile m, Mobile k, Party p)
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
			
			LevelSheet xmlplayer = null;
			xmlplayer = m.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
            PlayerMobile pm = null;
            LevelHandlerExt lh = new LevelHandlerExt();

            int range = m_ItemxmlSys.PartyExpShareRange;

            double orig	= 0;	//Monster Xp
            double fig	= 0;	//Party Xp
            double give	= 0;	//Xp To Give

            if (k != null)
                orig = LevelCore.Base(k);

            if (p != null && m_ItemxmlSys.PartyExpShare)
            {
				if (m_ItemxmlSys.PartyExpShareSplit)
				{
					foreach (PartyMemberInfo mi in p.Members)
					{
						pm = mi.Mobile as PlayerMobile;

						if (pm.InRange(k, range) && lh.MemberCount.Count < 6)
							lh.MemberCount.Add(pm);
					}

					if (lh.MemberCount.Count > 1)
						fig = (orig / lh.MemberCount.Count);
				}
				else
				{
					pm = m as PlayerMobile;
					fig = orig;
				}
            }
            else
            {
                pm = m as PlayerMobile;
                fig = orig;
            }

            if (fig > 0)
                give = LevelHandlerExt.ExpFilter(pm, fig, p, false);

            if (give > 0)
            {
				#region PartyExpShare
                if (p != null && m_ItemxmlSys.PartyExpShare)
                {
                    foreach (PartyMemberInfo mi in p.Members)
                    {
						pm = mi.Mobile as PlayerMobile;
                        if (pm.Alive && pm.InRange(k, range))
                        {
							LevelSheet xmlplayerparty = null;
							xmlplayerparty = pm.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
							if (xmlplayerparty == null)
							{
								return;
							}
							else
							{
								if (xmlplayerparty.PowerHour == true)
								{
									pm.SendMessage("You gained " + (give + m_ItemxmlSys.ExpPowerAmount) + " boosted exp for the party kill!");
									xmlplayerparty.kxp += (int)give + m_ItemxmlSys.ExpPowerAmount;
									if (pm.HasGump(typeof(ExpBar)))
									{
										pm.CloseGump(typeof(ExpBar));
										pm.SendGump(new ExpBar(pm));
									}
									if (xmlplayer.Expp >= xmlplayer.ToLevell)
									{
										if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
										{
											if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
											{
												DoLevel(pm);
											}
										}
										else
										{
											if (xmlplayer.Levell < xmlplayer.MaxLevel)
											{
												DoLevel(pm);
											}
										}
									}
								}
								else
								{
									pm.SendMessage("You gained " + give + " exp for the party kill!");
									xmlplayerparty.kxp += (int)give;
									if (pm.HasGump(typeof(ExpBar)))
									{
										pm.CloseGump(typeof(ExpBar));
										pm.SendGump(new ExpBar(pm));
									}
									if (xmlplayer.Expp >= xmlplayer.ToLevell)
									{
										if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
										{
											if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
											{
												DoLevel(pm);
											}
										}
										else
										{
											if (xmlplayer.Levell < xmlplayer.MaxLevel)
											{
												DoLevel(pm);
											}
										}
									}
								}
							}
                        }
                    }
                }
				#endregion
                else
                {
					if (xmlplayer.PowerHour == true)
					{
						pm.SendMessage("You gained " + (give + m_ItemxmlSys.ExpPowerAmount) + " boosted exp for the kill!");
						xmlplayer.kxp += (int)give + m_ItemxmlSys.ExpPowerAmount;
					}
					else
					{
						pm.SendMessage("You gained " + give + " exp for the kill!");
						xmlplayer.kxp	+= (int)give;
						xmlplayer.Expp	+= (int)give;
					}
					if (pm.HasGump(typeof(ExpBar)))
					{
						pm.CloseGump(typeof(ExpBar));
						pm.SendGump(new ExpBar(pm));
					}

                    if (xmlplayer.Expp >= xmlplayer.ToLevell)
					{
						if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
						{
							if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
							{
								DoLevel(pm);
							}
						}
						else
						{
							if (xmlplayer.Levell < xmlplayer.MaxLevel)
							{
								DoLevel(pm);
							}
						}
					}                        
                }
            }
			else
			{
				pm.SendMessage("You are at max Level!");
				return;
			}
        }

        public static int ExpFilter(Mobile m, double o, Party p, bool craft)
        {
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			
			LevelSheet xmlplayer = null;
			xmlplayer = m.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
            PlayerMobile pm = null;

            double n;
            double New = 0;
			
			#region PartyExpShare
            if (p != null && m_ItemxmlSys.PartyExpShare)
            {
                if (craft)
                    return 0;
                foreach (PartyMemberInfo mi in p.Members)
                {
                    pm = mi.Mobile as PlayerMobile;
					
					LevelSheet xmlplayerparty = null;
					xmlplayerparty = m.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;

					if (xmlplayerparty != null)
					{
						
						if (xmlplayerparty.Expp + o > xmlplayerparty.ToLevell && xmlplayerparty.Levell >= xmlplayerparty.EndMaxLvl)
						{
							if (xmlplayer.MaxLevel > xmlplayer.EndMaxLvl)
							{
								New = o;
							}
							else
							{
								n = (o + xmlplayer.Expp) - xmlplayer.ToLevell;
								New = (int)(o - n);
							}
						}
						else
							New = o;
					}
                } 
            }
			#endregion
            else
            {
                pm = m as PlayerMobile;

                if (xmlplayer.Expp + o > xmlplayer.ToLevell && xmlplayer.Levell >= xmlplayer.EndMaxLvl)
                {
					if (xmlplayer.MaxLevel > xmlplayer.EndMaxLvl)
					{
						New = o;
					}
					else
					{
						n = (o + xmlplayer.Expp) - xmlplayer.ToLevell;
						New = (int)(o - n);
					}
                }
                else
                    New = o;
            }

            return (int)New;
        }

        public static void DoLevel(Mobile klr)
        {
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys cs = null;
			cs = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (cs == null)
				return;
			
			LevelSheet xmlplayer = null;
			xmlplayer = klr.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
			
            PlayerMobile pm = klr as PlayerMobile;
            LevelHandlerExt lh = new LevelHandlerExt();

            if (xmlplayer.Expp >= xmlplayer.ToLevell)
            {
                xmlplayer.Expp = 0;
                xmlplayer.kxp = 0;
                xmlplayer.Levell += 1;
				
				int totalStats = pm.RawDex + pm.RawInt + pm.RawStr;
                int hitsgain;

                if (xmlplayer.Levell <= 20)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L2to20Multipier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below20;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below20stat;
					}
				}
				
                else if (xmlplayer.Levell <= 40)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L21to40Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below40;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below40stat;
					}
				}
                else if (xmlplayer.Levell <= 60)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L41to60Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below60;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below60stat;
					}
				}
                else if (xmlplayer.Levell <= 70)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L61to70Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below70;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below70stat;
					}
				}
                else if (xmlplayer.Levell <= 80)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L71to80Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below80;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below80stat;
					}
				}
                else if (xmlplayer.Levell <= 90)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L81to90Multipier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below90;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below90stat;
					}
				}
                else if (xmlplayer.Levell <= 100)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L91to100Multipier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below100;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below100stat;
					}
				}
				else if (xmlplayer.Levell <= 110)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L101to110Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below110;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below110stat;
					}
				}
				else if (xmlplayer.Levell <= 120)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L111to120Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below120;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below120stat;
					}
				}
				else if (xmlplayer.Levell <= 130)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L121to130Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below130;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below130stat;
					}
				}
				else if (xmlplayer.Levell <= 140)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L131to140Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below140;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below140stat;
					}
				}
				else if (xmlplayer.Levell <= 150)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L141to150Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below150;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below150stat;
					}

				}
				else if (xmlplayer.Levell <= 160)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L151to160Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below160;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below160stat;
					}
				}
				else if (xmlplayer.Levell <= 170)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L161to170Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below170;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below170stat;
					}
				}
				else if (xmlplayer.Levell <= 180)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L171to180Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below180;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below180stat;
					}
				}
				else if (xmlplayer.Levell <= 190)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L181to190Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below190;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below190stat;
					}
				}
				else if (xmlplayer.Levell <= 200)
				{
                    xmlplayer.ToLevell = (int)(xmlplayer.Levell * cs.L191to200Multiplier);
					if (pm.SkillsTotal < cs.SkillsTotal)
					{
						xmlplayer.SKPoints += cs.Below200;
					}
					if (totalStats < cs.MaxStatPoints)
					{
						xmlplayer.StatPoints += cs.Below200stat;
					}
				}
				
				if (cs.GainFollowerSlotOnLevel == true)
				{
					if (xmlplayer.Levell == 20 && cs.GainFollowOn20 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL20;
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 30 && cs.GainFollowOn30 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL30;
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 40 && cs.GainFollowOn40 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL40;
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 50 && cs.GainFollowOn50 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL50;
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 60 && cs.GainFollowOn60 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL60;
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 70 && cs.GainFollowOn70 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL70;
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 80 && cs.GainFollowOn80 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL80;
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 90 && cs.GainFollowOn90 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL90;
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 100 && cs.GainFollowOn100 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL100;	
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 110 && cs.GainFollowOn110 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL110;	
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 120 && cs.GainFollowOn120 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL120;	
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 130 && cs.GainFollowOn130 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL130;	
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 140 && cs.GainFollowOn140 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL140;
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 150 && cs.GainFollowOn150 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL150;	
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 160 && cs.GainFollowOn160 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL160;	
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 170 && cs.GainFollowOn170 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL170;	
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 180 && cs.GainFollowOn180 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL180;	
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 190 && cs.GainFollowOn190 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL190;	
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
					if (xmlplayer.Levell == 200 && cs.GainFollowOn200 == true)
					{
						klr.FollowersMax += cs.GainFollowerSlotonLeveL200;	
						pm.SendMessage( "You gained a bonus pet follower slot for reaching level {0}!", xmlplayer.Levell);
					}
				}
            }
            int pergain = 8;

           // pm.HitsMax = RawStr + xmlplayer.Levell;
           // pm.ManaMax = RawInt + xmlplayer.Levell;
           


            if (cs.RefreshOnLevel)
            {
                if (pm.Hits < pm.HitsMax)
                    pm.Hits = pm.HitsMax;

                if (pm.Mana < pm.ManaMax)
                    pm.Mana = pm.ManaMax;

                if (pm.Stam < pm.StamMax)
                    pm.Stam = pm.StamMax;
            }

            pm.PlaySound(0x20F);
            pm.FixedParticles(0x376A, 1, 31, 9961, 1160, 0, EffectLayer.Waist);
            pm.FixedParticles(0x37C4, 1, 31, 9502, 43, 2, EffectLayer.Waist);
            pm.SendMessage( "Your level has increased" );
            xmlplayer.Expp = 0;
            xmlplayer.kxp = 0;
            
			if (pm.HasGump(typeof(ExpBar)))
			{	/* updates EXPBar */
				pm.CloseGump(typeof(ExpBar));
				pm.SendGump(new ExpBar(pm));
			}
          
            
        }
        public static int Classic(Mobile from)
        {
			LevelSheet xmlplayer = null;
			xmlplayer = from.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
			
            PlayerMobile pm = from as PlayerMobile;

            int exp = LevelCore.GetExp(pm);//LevelCore.Stats(pm) + LevelCore.Skills(pm);
            int ToLevell = (int)(xmlplayer.Levell * 100);
            int highest = 100;

			if (exp >= ToLevell && xmlplayer.Levell != highest)
			{
				xmlplayer.Levell += 1;
				xmlplayer.ToLevell = (int)(xmlplayer.Levell * 100);

				if (exp >= ToLevell)
					LevelHandlerExt.Classic(pm);

				return exp;
			}
			return exp;
        }
		public static bool DoGainSkillExp(Mobile from, Skill skill)
		{
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys css = null;
			css = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (css == null)
				return false;
			
			LevelSheet xmlplayer = null;
			
			if (from is PlayerMobile)
			{
				PlayerMobile pm = from as PlayerMobile;
				Container pack = pm.Backpack;
				if (pack == null)
				{
					return false;
				}
				xmlplayer = pm.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
				if (xmlplayer == null)
					return false;
			}

			if (css.Enableexpfromskills == false)
				return false;
			
			if (skill == from.Skills.Provocation && css.Provocationgain)
			{
				int gain = (int) css.Provocationgainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Provocation;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Peacemaking && css.Peacemakinggain)
			{
				int gain = (int) css.Peacemakinggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Peacemaking;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Discordance && css.Discordancegain)
			{
				int gain = (int) css.Discordancegainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Discordance;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Stealing && css.Stealinggain)
			{
				int gain = (int) css.Stealinggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Stealing;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.RemoveTrap && css.Removetrapgain)
			{
				int gain = (int) css.Removetrapgainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.RemoveTrap;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Poisoning && css.Poisoninggain)
			{
				int gain = (int) css.Poisoninggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Poisoning;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.DetectHidden && css.Detecthiddengain)
			{
				int gain = (int) css.Detecthiddengainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.DetectHidden;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Tracking && css.Trackinggain)
			{
				int gain = (int) css.Trackinggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Tracking;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Herding && css.Herdinggain)
			{
				int gain = (int) css.Herdinggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Herding;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.AnimalTaming && css.Animaltaminggain)
			{
				int gain = (int) css.Animaltaminggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.AnimalTaming;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.AnimalLore && css.Animalloregain)
			{
				int gain = (int) css.Animalloregainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.AnimalLore;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.SpiritSpeak && css.Spiritspeakgain)
			{
				int gain = (int) css.Spiritspeakgainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.SpiritSpeak;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Meditation && css.Meditationgain)
			{
				int gain = (int) css.Meditationgainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Meditation;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.EvalInt && css.Evalintgain)
			{
				int gain = (int) css.Evalintgainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.EvalInt;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Imbuing && css.Imbuinggain)
			{
				int gain = (int) css.Imbuinggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Imbuing;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Tinkering && css.Tinkeringgain)
			{
				int gain = (int) css.Tinkeringgainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Tinkering;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Tailoring && css.Tailoringgain)
			{
				int gain = (int) css.Tailoringgainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Tailoring;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Inscribe && css.Inscribegain)
			{
				int gain = (int) css.Inscribegainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Inscribe;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Cooking && css.Cookinggain)
			{
				int gain = (int) css.Cookinggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Cooking;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}	
			if (skill == from.Skills.Carpentry && css.Carpentrygain)
			{
				int gain = (int) css.Carpentrygainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Carpentry;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}	
			if (skill == from.Skills.Blacksmith && css.Blacksmithgain)
			{
				int gain = (int) css.Blacksmithgainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Blacksmith;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}	
			if (skill == from.Skills.Fletching && css.Fletchinggain)
			{
				int gain = (int) css.Fletchinggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Fletching;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}	
			if (skill == from.Skills.Alchemy && css.Alchemygain)
			{
				int gain = (int) css.Alchemygainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Alchemy;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Anatomy && css.Anatomygain)
			{
				int gain = (int) css.Anatomygainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Anatomy;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.TasteID && css.Tasteidgain)
			{
				int gain = (int) css.Tasteidgainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.TasteID;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}	
			if (skill == from.Skills.ItemID && css.Itemidgain)
			{
				int gain = (int) css.Itemidgainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.ItemID;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}	
			if (skill == from.Skills.Forensics && css.Forensicsgain)
			{
				int gain = (int) css.Forensicsgainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Forensics;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}	
			if (skill == from.Skills.Cartography && css.Cartographygain)
			{
				int gain = (int) css.Cartographygainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Cartography;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}	
			if (skill == from.Skills.Camping && css.Campinggain)
			{
				int gain = (int) css.Campinggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Camping;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Begging && css.Begginggain)
			{
				int gain = (int) css.Begginggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Begging;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}	
			if (skill == from.Skills.ArmsLore && css.Armsloregain)
			{
				int gain = (int) css.Armsloregainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.ArmsLore;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}		
			if (skill == from.Skills.Fishing && css.Fishinggain)
			{
				int gain = (int) css.Fishinggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Fishing;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			if (skill == from.Skills.Hiding && css.Hidinggain)
			{
				int gain = (int) css.Hidinggainamount;
				List<SkillName> list = new List<SkillName>();
				var skillname = SkillName.Hiding;
				int gaining = (int) gain + xmlplayer.Expp;	
				if (gaining >= xmlplayer.ToLevell)
				{
					xmlplayer.kxp += gain;
					xmlplayer.Expp += gain;
					if (xmlplayer.MaxLevel < xmlplayer.EndMaxLvl)
					{
						if (xmlplayer.Levell < xmlplayer.EndMaxLvl)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
					else
					{
						if (xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
							LevelHandlerExt.DoLevel(from);
						}
					}
				}
				else
				{
					if (xmlplayer.Levell == xmlplayer.MaxLevel || xmlplayer.Levell == xmlplayer.EndMaxLvl)
					{
						return false;
					}
					else
					{
						xmlplayer.kxp += gain;
						xmlplayer.Expp += gain;
						from.SendMessage( "You have gained " + (gain) + " exp from using {0}!", skillname);
					}
				}
			}
			return true;
		}
		public static void BodGainEXP(Mobile from, int points)
		{	
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));	
			if (m_ItemxmlSys != null && m_ItemxmlSys.PlayerLevels == true)
			{
				if (m_ItemxmlSys.GainExpFromBods == true)
				{
					LevelSheet xmlplayer = null;
					xmlplayer = from.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
					
					xmlplayer.kxp += points;
					from.SendMessage("You have been awarded {0} EXP points for turning in the bulk order.", points);
					if (xmlplayer.Expp >= xmlplayer.ToLevell && xmlplayer.Levell < xmlplayer.MaxLevel)
						LevelHandlerExt.DoLevel(from);
				}
			}
		}
	}
}
