using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Engines.Craft;
using Server.Engines.PartySystem;
using Server.Engines.LevelSystemExtAtt;

namespace Server
{
    public class LevelHandlerPetExt
    {
        public ArrayList MemberCount = new ArrayList();

        public static void Set(Mobile killer, Mobile killed)
        {
			if (killer is PlayerMobile || killed is PlayerMobile)
			{
				return;
			}

			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return;
			
			if (m_ItemxmlSys.EnabledLevelPets == false)
			{
				return;
			}				
			
			BaseCreature bc = (BaseCreature)killer;
			var BankBoxVar = bc.FindItemOnLayer(Layer.Bank);
			if (BankBoxVar == null)
				return;
			else
			{
				PetLevelOrb petorb = null;
				petorb = bc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;
				if (petorb == null)
					return;
			}
					
            BaseCreature klr = null;
            BaseCreature klrr = null;
            Party pty = null;
            LevelHandlerPetExt lh = new LevelHandlerPetExt();
            klr = killer as BaseCreature;
            klrr = killed as BaseCreature;

			if (klr.Summoned == true || klr.Summoned == true)
				return;
			if (klrr.Summoned == true || klrr.Summoned == true)
				return;
			
            if (lh.MemberCount.Count > 0)
            {
                foreach (Mobile il in lh.MemberCount)
                {
                    lh.MemberCount.Remove(il);
                }
            }

            if (klr != null)
            {
				BaseCreature bcr = (BaseCreature)killer;
				pty = Party.Get(klr);
				if (bcr is BaseCreature)
				{
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
			
			BaseCreature bc = (BaseCreature)m;
			PetLevelOrb petxml = null;
			petxml = bc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;	
//            PlayerMobile pm = null;
            LevelHandlerPetExt lh = new LevelHandlerPetExt();
			Mobile cm = bc.ControlMaster;

            double orig = 0;  //Monster Xp
            double fig = 0;   //Party Xp
            double give = 0;  //Xp To Give

            if (k != null)
                orig = LevelCore.Base(k);

			fig = orig;

            if (fig > 0)
                give = LevelHandlerPetExt.ExpFilter(m, fig, p, false);

            if (give > 0)
            {
				if (m_ItemxmlSys.NotifyOnPetExpGain == true)
				{
					cm.SendMessage("{0} gained " + give + " exp for the kill!", bc.Name);
				}
				petxml.kxp += (int)give;
				petxml.Expp += (int)give;
				

				if (petxml.Expp >= petxml.ToLevell && petxml.Levell < petxml.MaxLevel)
					DoLevel(bc);
            }
        }

        public static int ExpFilter(Mobile m, double o, Party p, bool craft)
        {
			BaseCreature bc = (BaseCreature)m;
			PetLevelOrb petxml = null;
			PetLevelOrb petxml3 = null;
			petxml = bc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;	
//            PlayerMobile pm = null;
			BaseCreature bcc = null;
			
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return 0;

            double n;
            double New = 0;

			if (p != null && m_ItemxmlSys.PartyExpShare)
			{
			}
			else
			{
				bcc = m as BaseCreature;
				petxml3 = bcc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;	
				
				if (petxml3 == null)
					return 0;
				
				if (petxml3.Expp + o > petxml3.ToLevell && petxml3.Levell >= petxml3.MaxLevel)
				{
					n = (o + petxml3.Expp) - petxml3.ToLevell;
					New = (int)(o - n);
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
			
			BaseCreature bc = (BaseCreature)klr;
			PetLevelOrb petxml = null;
			petxml = bc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;	
            PlayerMobile pm = klr as PlayerMobile;
            LevelHandlerPetExt lh = new LevelHandlerPetExt();
			Mobile cm = bc.ControlMaster;

			/* Still adding Skill Points for Future Development */
			
            if (petxml.Expp >= petxml.ToLevell)
            {
                petxml.Expp = 0;
                petxml.kxp = 0;
                petxml.Levell += 1;
				
				int totalstats = bc.RawDex + bc.RawInt + bc.RawStr;

                if (petxml.Levell <= 20)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 20);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow20;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow20stat;
					}
				}
				
                else if (petxml.Levell <= 40)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 40);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow40;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow40stat;
					}
				}
                else if (petxml.Levell <= 60)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 60);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow60;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow60stat;
					}
				}
                else if (petxml.Levell <= 70)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 80);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow70;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow70stat;
					}
				}
                else if (petxml.Levell <= 80)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 100);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow80;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow80stat;
					}
				}
                else if (petxml.Levell <= 90)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 120);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow90;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow90stat;
					}
				}
                else if (petxml.Levell <= 100)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 140);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow100;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow100stat;
					}
				}
				else if (petxml.Levell <= 110)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 140);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow110;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow110stat;
					}
				}
				else if (petxml.Levell <= 120)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 150);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow120;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow120stat;
					}
				}
				else if (petxml.Levell <= 130)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 150);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow130;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow130stat;
					}
				}
				else if (petxml.Levell <= 140)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 160);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow140;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow140stat;
					}
				}
				else if (petxml.Levell <= 150)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 180);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow150;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow150stat;
					}
				}
				else if (petxml.Levell <= 160)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 180);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow160;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow160stat;
					}
				}
				else if (petxml.Levell <= 170)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 190);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow170;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow170stat;
					}
				}
				else if (petxml.Levell <= 180)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 190);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow180;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow180stat;
					}
				}
				else if (petxml.Levell <= 190)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 190);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow190;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow190stat;
					}
				}
				else if (petxml.Levell <= 200)
				{
                    petxml.ToLevell = (int)(petxml.Levell * 200);
					if (bc.SkillsTotal < cs.SkillsTotal)
					{
						petxml.SKPoints += cs.Petbelow200;
					}
					if (totalstats < cs.PetMaxStatPoints)
					{
						petxml.StatPoints += cs.Petbelow200stat;
					}
				}
            }
            
            if (cs.RefreshOnLevel)
            {
                if (bc.Hits < bc.HitsMax)
                    bc.Hits = bc.HitsMax;

                if (bc.Mana < bc.ManaMax)
                    bc.Mana = bc.ManaMax;

                if (bc.Stam < bc.StamMax)
                    bc.Stam = bc.StamMax;
            }

            bc.PlaySound(0x20F);
            bc.FixedParticles(0x376A, 1, 31, 9961, 1160, 0, EffectLayer.Waist);
            bc.FixedParticles(0x37C4, 1, 31, 9502, 43, 2, EffectLayer.Waist);
			
			if (cs.NotifyOnPetLevelUp == true)
			{
				cm.SendMessage( "Your Pet level has increased" );
			}
            petxml.Expp = 0;
            petxml.kxp = 0;
          
            
        }

		public static void PetOnDeath( BaseCreature bc )
		{
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_Itemxml = null;
			m_Itemxml = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_Itemxml == null)
				return;
			
			PetLevelOrb petxml = null;
			petxml = bc.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;	
			Mobile master = bc.ControlMaster;
			
			if ( master != null && bc.Controlled == true && petxml != null )
			{
				if (m_Itemxml.LoseExpLevelOnDeath == true)
				{
					if (m_Itemxml.LoseStatOnDeath == true)
					{
						int strloss = bc.Str		/ m_Itemxml.PetStatLossAmount;
						int dexloss = bc.Dex		/ m_Itemxml.PetStatLossAmount;
						int intloss = bc.Int		/ m_Itemxml.PetStatLossAmount;	
						if (bc.Str > strloss )
							bc.Str -= strloss;
						if (bc.Dex > dexloss )
							bc.Dex -= dexloss;
						if (bc.Int > intloss )
							bc.Int -= intloss;
					}
					int ExpLoss = petxml.Expp	/ m_Itemxml.PetStatLossAmount;
					int KXPLoss = petxml.kxp	/ m_Itemxml.PetStatLossAmount;
					petxml.Expp	-= ExpLoss;
					petxml.kxp	-= KXPLoss;

					if (petxml.Expp <= 0)
					{
						petxml.Levell		-= 1;
						petxml.Expp			= 0;
						petxml.kxp			= 0;
					}
					master.SendMessage( 38, "Your pet has suffered a 5% stat lose due to its untimely death." );					
				}
				else
				{
					master.SendMessage( 64, "Your pet has been killed!" );
				}
			}
		}
    }
}