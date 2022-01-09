using System;
using Server;
using Server.Misc;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.LevelSystemExtAtt;

namespace Server
{
    public class MountEXTCheck
    {
		public static void MountCheckVoid(Mobile pet, Mobile owner, LevelControlSys c, LevelSheet xmlplayer)
		{						
			PlayerMobile pm = owner as PlayerMobile;			
			BaseCreature mount = (BaseCreature)pet;			
			IMount mountplayer = (IMount)owner.Mount;
			
			if (mount is BaseMount && mountplayer.Rider != null) 
			{
				if (mount is Beetle)
				{
					if (xmlplayer.Levell < c.Beetle)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Beetle + " to ride me!");	
						return;
					}
				}
				else if (mount is DesertOstard) 
				{
					if (xmlplayer.Levell < c.DesertOstard)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.DesertOstard + " to ride me!");	
						return;
					}
				}
				else if (mount is FireSteed) 
				{
					if (xmlplayer.Levell < c.FireSteed)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.FireSteed + " to ride me!");	
						return;
					}
				}
				else if (mount is ForestOstard) 
				{
					if (xmlplayer.Levell < c.ForestOstard)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.ForestOstard + " to ride me!");	
						return;
					}
				}
				else if (mount is FrenziedOstard) 
				{
					if (xmlplayer.Levell < c.FrenziedOstard)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.FrenziedOstard + " to ride me!");	
						return;
					}
				}
				else if (mount is HellSteed) 
				{
					if (xmlplayer.Levell < c.HellSteed)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.HellSteed + " to ride me!");	
						return;
					}
				}
				else if (mount is Hiryu) 
				{
					if (xmlplayer.Levell < c.Hiryu)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Hiryu + " to ride me!");	
						return;
					}
				}
				else if (mount is Horse) 
				{
					if (xmlplayer.Levell < c.Horse)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Horse + " to ride me!");	
						return;
					}
				}
				else if (mount is Kirin) 
				{
					if (xmlplayer.Levell < c.Kirin)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Kirin + " to ride me!");	
						return;
					}
				}
				else if (mount is LesserHiryu) 
				{
					if (xmlplayer.Levell < c.LesserHiryu)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.LesserHiryu + " to ride me!");	
						return;
					}
				}
				else if (mount is Nightmare) 
				{
					if (xmlplayer.Levell < c.NightMare)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.NightMare + " to ride me!");	
						return;
					}
				}
				else if (mount is RidableLlama) 
				{
					if (xmlplayer.Levell < c.Ridablellama)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Ridablellama + " to ride me!");	
						return;
					}
				}
				else if (mount is Ridgeback) 
				{
					if (xmlplayer.Levell < c.Ridgeback)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Ridgeback + " to ride me!");	
						return;
					}
				}
				else if (mount is SavageRidgeback) 
				{
					if (xmlplayer.Levell < c.SavageRidgeback)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.SavageRidgeback + " to ride me!");	
						return;
					}
				}
				else if (mount is ScaledSwampDragon) 
				{
					if (xmlplayer.Levell < c.ScaledSwampDragon)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.ScaledSwampDragon + " to ride me!");	
						return;
					}
				}
				else if (mount is SeaHorse) 
				{
					if (xmlplayer.Levell < c.Seahorse)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Seahorse + " to ride me!");	
						return;
					}
				}
				else if (mount is SilverSteed) 
				{
					if (xmlplayer.Levell < c.SilverSteed)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.SilverSteed + " to ride me!");	
						return;
					}
				}
				else if (mount is SkeletalMount) 
				{
					if (xmlplayer.Levell < c.SkeletalMount)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.SkeletalMount + " to ride me!");	
						return;
					}
				}
				else if (mount is SwampDragon) 
				{
					if (xmlplayer.Levell < c.Swampdragon)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Swampdragon + " to ride me!");	
						return;
					}
				}
				else if (mount is Unicorn) 
				{
					if (xmlplayer.Levell < c.Unicorn)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Unicorn + " to ride me!");	
						return;
					}
				}
				else if (mount is Reptalon) 
				{
					if (xmlplayer.Levell < c.Reptalon)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Reptalon + " to ride me!");	
						return;
					}
				}
				else if (mount is WildTiger) 
				{
					if (xmlplayer.Levell < c.Wildtiger)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Wildtiger + " to ride me!");	
						return;
					}
				}
				else if (mount is Windrunner) 
				{
					if (xmlplayer.Levell < c.Windrunner)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Windrunner + " to ride me!");	
						return;
					}
				}
				else if (mount is Lasher) 
				{
					if (xmlplayer.Levell < c.Lasher)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Lasher + " to ride me!");	
						return;
					}
				}
				else if (mount is Eowmu) 
				{
					if (xmlplayer.Levell < c.Eowmu)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Eowmu + " to ride me!");	
						return;
					}
				}
				else if (mount is DreadWarhorse) 
				{
					if (xmlplayer.Levell < c.Dreadwarhorse)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Dreadwarhorse + " to ride me!");	
						return;
					}
				}
				else if (mount is CuSidhe) 
				{
					if (xmlplayer.Levell < c.Cusidhe)
					{
						IMount mounttwo = (IMount)owner.Mount;
						mounttwo.Rider = null;
						pm.LocalOverheadMessage(MessageType.Emote, 0x35, true, "Your Mount kicks you off.  You must be Level " + c.Cusidhe + " to ride me!");	
						return;
					}
				}			
				else
				{
					owner.SendMessage("You are not at the right Level to ride me!");	
					return; //Fail catch for Creatures on this list but are in the system
				}
			}
		}
	}
}
