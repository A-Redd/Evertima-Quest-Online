using System;
using Server;
using Server.Items;
using Server.Guilds;
using Server.Mobiles;
using Server.Network;
using Server.Accounting;
using System.Collections;
using Server.Engines.LevelSystemExtAtt;

namespace Server
{
    public class StartUpHandler
    {		
		public static void CustomBackPackDrops(Mobile m, LevelControlSys m_Itemxml)
		{
			m.AddToBackpack(new Gold(9000));
			m.AddToBackpack(new Candle());
			m.AddToBackpack(new Dagger());
			m.AddToBackpack(new Dagger());
			m.AddToBackpack(new Dagger());
			m.AddToBackpack(new Dagger());
			m.AddToBackpack(new Dagger());
			m.AddToBackpack(new Dagger());
			m.AddToBackpack(new Dagger());
		}
		public static void ForceIntoGuild (Mobile m, LevelControlSys m_Itemxml)
		{
			/* Make sure guild exist, or this wont work */
			Guild g = BaseGuild.FindByName(m_Itemxml.Guildnamestart) as Guild;
			if(g != null)
			{
				g.AddMember(m);
			}
		}
		public static void StartingLocation (Mobile m, LevelControlSys m_Itemxml)
		{
			if (m_Itemxml.MapBoolTrammel == true && m_Itemxml.MapBoolFelucca == false && m_Itemxml.MapBoolMalas == false)
			{
				m.Map = Map.Trammel; 
				m.Location = new Point3D(m_Itemxml.X_variable, m_Itemxml.Y_variable, m_Itemxml.Z_variable);
			}
			if (m_Itemxml.MapBoolFelucca == true && m_Itemxml.MapBoolTrammel == false && m_Itemxml.MapBoolMalas == false)
			{
				m.Map = Map.Felucca; 
				m.Location = new Point3D(m_Itemxml.X_variable, m_Itemxml.Y_variable, m_Itemxml.Z_variable);
			}
			if (m_Itemxml.MapBoolMalas == true && m_Itemxml.MapBoolFelucca == false && m_Itemxml.MapBoolTrammel == false)
			{
				m.Map = Map.Malas; 
				m.Location = new Point3D(m_Itemxml.X_variable, m_Itemxml.Y_variable, m_Itemxml.Z_variable);
			}
		}
	}
}







