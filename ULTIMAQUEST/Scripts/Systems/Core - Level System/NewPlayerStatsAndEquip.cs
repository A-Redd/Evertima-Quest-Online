using System;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Misc
{
    public class NewPlayerStatsAndEquip
    {
        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(EventSink_Login);
        }

        private static void EventSink_Login(LoginEventArgs args)
        {
            Mobile m = args.Mobile;
			
            if (args.Mobile.NetState == null)
            {
                return;
            }

			if (m is PlayerMobile)
			{
				LevelControlSysItem loccontrol = null;
				foreach (Item lister in World.Items.Values)
				{
					if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
				}
				LevelControlSys m_ItemxmlSys = null;
				m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
				
				PlayerMobile pm = (PlayerMobile)m;
				
				/* Sanity Checks for logging in */
				Container packxml1 = pm.Backpack;
				if (m_ItemxmlSys != null && m_ItemxmlSys.PlayerLevels == true && packxml1 != null)
				{
						LevelSheet xmlplayer2 = null;
						xmlplayer2 = pm.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
						
						if (xmlplayer2 != null)
						{
							/* Ensure the Stat Boost is false upon logging in to prevent Failed loop. */
							xmlplayer2.AuraStatBoost = false;
						}
				}
				
				if (m_ItemxmlSys != null && m_ItemxmlSys.PlayerLevels == true)
				{
					LevelSheet xmlplayer = null;
					LevelSheet bagcheck = null;
					bagcheck = pm.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
					if (bagcheck == null)
					{
						pm.AddToBackpack(new LevelSheet());
					}
					
					PetLevelSheet bagcheck2 = null;
					bagcheck2 = pm.Backpack.FindItemByType(typeof(PetLevelSheet), false) as PetLevelSheet;
					if (bagcheck2 == null && m_ItemxmlSys.EnabledLevelPets == true)
					{
						pm.AddToBackpack(new PetLevelSheet());
					}
						
					xmlplayer = pm.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;

					if (xmlplayer != null && xmlplayer.NewToon == true)
					{
						xmlplayer.NewToon = false;
						
						if (m_ItemxmlSys.Forcestartingstats == true)
						{
							pm.RawStr = m_ItemxmlSys.Forcestartingstatsstr;
							pm.RawDex = m_ItemxmlSys.Forcestartingstatsdex;
							pm.RawInt = m_ItemxmlSys.Forcestartingstatsint;
						}
						
						if (m_ItemxmlSys.ForceNewPlayerIntoGuild == true)
							StartUpHandler.ForceIntoGuild(pm, m_ItemxmlSys);
						
						if (m_ItemxmlSys.AddToBackpackOnAttach == true)
							PlayerPackStart.CustomBackPackDrops(pm);
						
						if (m_ItemxmlSys.NewStartingLocation == true)
							StartUpHandler.StartingLocation(pm, m_ItemxmlSys);
						
						if (m_ItemxmlSys.Autoactivate_gemmining == true)
							pm.GemMining = true;
						if (m_ItemxmlSys.Autoactivate_basketweaving == true)
							pm.BasketWeaving = true;
						if (m_ItemxmlSys.Autoactivate_canbuycarpets == true)
							pm.CanBuyCarpets = true;
						if (m_ItemxmlSys.Autoactivate_acceptguildinvites == true)
							pm.AcceptGuildInvites = true;
						if (m_ItemxmlSys.Autoactivate_glassblowing == true)
							pm.Glassblowing = true;
						if (m_ItemxmlSys.Autoactivate_libraryfriend == true)
							pm.LibraryFriend = true;
						if (m_ItemxmlSys.Autoactivate_masonry == true)
							pm.Masonry = true;
						if (m_ItemxmlSys.Autoactivate_sandmining == true)
							pm.SandMining = true;
						if (m_ItemxmlSys.Autoactivate_stonemining == true)
							pm.StoneMining = true;
						if (m_ItemxmlSys.Autoactivate_spellweaving == true)
							pm.Spellweaving = true;
						if (m_ItemxmlSys.Autoactivate_mechanicallife == true)
							pm.MechanicalLife = true;
						if (m_ItemxmlSys.Autoactivate_disabledpvpwarning == true)
							pm.DisabledPvpWarning = true;
						if (m_ItemxmlSys.Autoactivate_isyoung == true)
							pm.Young = true;
						if (m_ItemxmlSys.Autoactivate_cantwalk == true)
							pm.CantWalk = true;
						if (m_ItemxmlSys.Autoactivate_maxfollowslots == true)
							pm.FollowersMax = m_ItemxmlSys.Autoactivate_maxfollowslotstotal;
						if (m_ItemxmlSys.Autoactivate_skillscap == true)
							pm.SkillsCap = m_ItemxmlSys.Autoactivate_skillscapvar;
						if (m_ItemxmlSys.Autoactivate_startingtotalstatcap == true)
							pm.StatCap = m_ItemxmlSys.Autoactivate_startingtotalstatcapvar;
					
						 return;
					}
				}
			}
			else
				return;
        }
    }
}