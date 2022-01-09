/*	System Configuration, changes to the system should be 
	made here instead of changes to the scripts
*/
using System;
using Server;
using Server.Items;
using Server.Guilds;
using Server.Mobiles;
using Server.Network;
using Server.Accounting;
using System.Collections;

namespace Server
{
    public class PlayerPackStart
    {		
		#region Add your entries of items you want to drop in pack
		public bool AddToBackpackOnAttach					= false;
		public static void CustomBackPackDrops(Mobile m)
		{
			/* Format;  
			m.AddToBackPack(new Item(ValueifAny));
			*/
			m.AddToBackpack(new RedBook());
			m.AddToBackpack(new Gold(5000));
			m.AddToBackpack(new Candle());
			m.AddToBackpack(new Dagger());
		}
		#endregion
	}
}