using System;
using Server;
using Server.Prompts;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Items;
using Server.Commands;
using Server.Regions;
using Server.Targeting;
using System.Collections;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using System.Collections.Generic;

namespace Server.Items
{
	public class PetLevelOrb : Item
	{
		private StatBoostTimer m_Timer;
		private int m_originalmaster = 0;
		private int m_wholaststoleme = 0;
		private int m_reqplayerlevelown = 0;
		private int m_TimeLeft = 3600;
        private int m_Levell;
        private int m_MaxLevel;
        private int m_Expp;
        private int m_ToLevell;
        private int m_kxp;
        private int m_SKPoints;
        private int m_StatPoints;
		private int m_StrPointsUsed;
		private int m_DexPointsUsed;
		private int m_IntPointsUsed;
		private int m_proximityrange = 0;
		private int m_TotalStatPointsAquired;
		private bool m_stolenpet 				= false;
		private bool m_BlockDefaultUse			= false;
		private bool m_PetDeathToggle			= false;
		private bool m_amilevelmerc				= false;
		public bool m_PetSpeak					= false;
		public bool m_TeleToTargetSwitch		= false;
		public bool m_MassProvokeSwitch			= false;
		public bool m_MassPeace					= false;
		public bool m_SuperHeal					= true;
		public bool m_BlessedPower				= true;
		public bool m_AreaFireBlast				= true;
		public bool m_AreaIceBlast				= true;
		public bool m_AreaAirBlast				= true;
		private bool m_pethappyrewards			= false;
		private int	m_pethappyrewardtier		= 0;
		private int m_pethappylevelexp			= 0;
		private int m_pethappylevel				= 0;
		private bool m_pethappygiftgiven		= false;
		private bool m_pethappysound			= false;
		private bool m_pethappyoversleep		= false;
		private bool m_pethappychangecolor		= false;
		private bool m_levelpacktrait			= false;
		
		
		[CommandProperty(AccessLevel.GameMaster)]
        public bool AmILevelMerc
        {
            get { return m_amilevelmerc; }
            set { m_amilevelmerc = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int OriginalMaster
        {
            get { return m_originalmaster; }
            set { m_originalmaster = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ProximityRange
        {
            get { return m_proximityrange; }
            set { m_proximityrange = value; }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public bool StolenPet
        {
            get { return m_stolenpet; }
            set { m_stolenpet = value; }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WhoLastStoleMe
        {
            get { return m_wholaststoleme; }
            set { m_wholaststoleme = value; }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ReqPlayerLevelOwn
        {
            get { return m_reqplayerlevelown; }
            set { m_reqplayerlevelown = value; }
        }
		
		public int TimeLeft 
		{ 	
			get
			{ 
				return m_TimeLeft; 
			} 
			set 
			{ 
				m_TimeLeft = value; InvalidateProperties(); 
			} 
		}
		
        [CommandProperty(AccessLevel.GameMaster)]
        public int Levell
        {
            get { return m_Levell; }
            set { m_Levell = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxLevel
        {
            get { return m_MaxLevel; }
            set { m_MaxLevel = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Expp
        {  
            get { return m_Expp; }
            set { m_Expp = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ToLevell
        {
            get { return m_ToLevell; }
            set { m_ToLevell = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int kxp
        {
            get { return m_kxp; }
            set { m_kxp = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SKPoints
        {
            get { return m_SKPoints; }
            set { m_SKPoints = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int StatPoints
        {
            get { return m_StatPoints; }
            set { m_StatPoints = value; }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public int StrPointsUsed
        {
            get { return m_StrPointsUsed; }
            set { m_StrPointsUsed = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int DexPointsUsed
        {
            get { return m_DexPointsUsed; }
            set { m_DexPointsUsed = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int IntPointsUsed
        {
            get { return m_IntPointsUsed; }
            set { m_IntPointsUsed = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalStatPointsAquired
        {
            get { return m_TotalStatPointsAquired; }
            set { m_TotalStatPointsAquired = value; }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public bool BlockDefaultUse 
		{ 
			get { return m_BlockDefaultUse; } 
			set { m_BlockDefaultUse = value; } 
		}
		[CommandProperty(AccessLevel.GameMaster)]
        public bool PetDeathToggle 
		{ 
			get { return m_PetDeathToggle; } 
			set { m_PetDeathToggle = value; } 
		}
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AreaAirBlast
        {
            get { return m_AreaAirBlast; }
            set { m_AreaAirBlast = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AreaIceBlast
        {
            get { return m_AreaIceBlast; }
            set { m_AreaIceBlast = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AreaFireBlast
        {
            get { return m_AreaFireBlast; }
            set { m_AreaFireBlast = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BlessedPower
        {
            get { return m_BlessedPower; }
            set { m_BlessedPower = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool SuperHeal
        {
            get { return m_SuperHeal; }
            set { m_SuperHeal = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool MassPeace
        {
            get { return m_MassPeace; }
            set { m_MassPeace = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool MassProvoke
        {
            get { return m_MassProvokeSwitch; }
            set { m_MassProvokeSwitch = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool TeleportToTarget
        {
            get { return m_TeleToTargetSwitch; }
            set { m_TeleToTargetSwitch = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PetSpeak
        {
            get { return m_PetSpeak; }
            set { m_PetSpeak = value; }
        }
		
		int[] numArray = new int[1];

		string str = CommandSystem.Prefix;
		
		[Constructable]
		public PetLevelOrb() : base( 22334 )
		{
			Movable = false;
			Weight = 1.0;
			Name = "Pet Level Orb";
			LootType = LootType.Blessed;
			Hue = 1462;
			m_Levell = 1;
			m_ToLevell = 100;
			m_TimeLeft = 3600;
			Visible = false;
		}

		public PetLevelOrb( Serial serial ) : base( serial )
		{ 
		} 
		
		public void NewTimer(Mobile from)
		{
			  if (m_Timer != null)
					m_Timer.Stop();
			  m_Timer = new StatBoostTimer(from, this);
			  m_Timer.Start();
		}
        public void ToggleStatTimer(bool turnon, Mobile from)
        {
			Container packxml1 = from.Backpack;
			if(packxml1 != null)
			{	
				LevelSheet xmlplayer = null;
				xmlplayer = from.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
			
						
				if (turnon)
				{
					if (m_Timer != null)
						m_Timer.Stop();
					m_Timer = new StatBoostTimer(from, this);
					m_Timer.Start();

				}
				else
				{
					from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "Power level killed!");
					if (m_Timer != null)
						m_Timer.Stop();
					m_Timer = new StatBoostTimer(from, this);
					m_Timer.Stop();
					this.TimeLeft = 3600;
					xmlplayer.AuraStatBoost = false;
					
					if( this.RootParent is BaseCreature )
					{
						BaseCreature bc = (BaseCreature)this.RootParent;
						CreatureMovement.PetStatBonus(from, bc, xmlplayer);
					}
				}
			}
			else
				return;
        }
        private class StatBoostTimer : Timer
        {
            private readonly Mobile m;
            private PetLevelOrb fi;
            public StatBoostTimer(Mobile from, PetLevelOrb item)
                : base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1), 3600)
            {
                m = from;
                fi = item;
                Priority = TimerPriority.EveryTick;
            }
            protected override void OnTick()
            {
				if (fi == null || fi.Deleted) Stop();

				fi.TimeLeft--;
				if (fi.TimeLeft <= 0)
				{
					fi.ToggleStatTimer(false, m);
				}
            }
        }
		
		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 1 ); 

			writer.Write((int) m_originalmaster);
			writer.Write((bool) m_stolenpet);
			writer.Write((bool) m_amilevelmerc);
			writer.Write((int) m_wholaststoleme);
			writer.Write((int) m_reqplayerlevelown);
			writer.Write((int)m_TimeLeft);
			writer.Write(m_BlockDefaultUse);
			writer.Write(m_PetDeathToggle);
            writer.Write((int) m_Levell);
            writer.Write((int) m_MaxLevel);
            writer.Write((int) m_Expp);
            writer.Write((int) m_ToLevell);
            writer.Write((int) m_kxp);
            writer.Write((int) m_SKPoints);
            writer.Write((int) m_StatPoints);
            writer.Write((int) m_StrPointsUsed);
            writer.Write((int) m_DexPointsUsed);
            writer.Write((int) m_IntPointsUsed);
            writer.Write((int) m_TotalStatPointsAquired);
			writer.Write((bool)m_PetSpeak);
			writer.Write((bool)m_TeleToTargetSwitch);
			writer.Write((bool)m_MassProvokeSwitch);
			writer.Write((bool)m_MassPeace);
			writer.Write((bool)m_SuperHeal);
			writer.Write((bool)m_BlessedPower);
			writer.Write((bool)m_AreaFireBlast);
			writer.Write((bool)m_AreaIceBlast);
			writer.Write((bool)m_AreaAirBlast);
			writer.Write((bool)m_pethappyrewards);
			writer.Write((int)m_pethappyrewardtier);
			writer.Write((int)m_pethappylevelexp);
			writer.Write((int)m_pethappylevel);
			writer.Write((bool)m_pethappygiftgiven);
			writer.Write((bool)m_pethappysound);
			writer.Write((int)m_proximityrange);
			writer.Write((bool)m_pethappyoversleep);
			writer.Write((bool)m_pethappychangecolor);
			writer.Write((bool)m_levelpacktrait);
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 

			int version = reader.ReadInt(); 
			
			m_originalmaster = reader.ReadInt();
			m_stolenpet = reader.ReadBool();
			m_amilevelmerc = reader.ReadBool();
			m_wholaststoleme = reader.ReadInt();
			m_reqplayerlevelown = reader.ReadInt();
			m_TimeLeft = reader.ReadInt();
			m_BlockDefaultUse = reader.ReadBool();
			m_PetDeathToggle  = reader.ReadBool();
			m_Levell = reader.ReadInt();
			m_MaxLevel = reader.ReadInt();
			m_Expp = reader.ReadInt();
			m_ToLevell = reader.ReadInt();
			m_kxp = reader.ReadInt();
			m_SKPoints = reader.ReadInt();
			m_StatPoints = reader.ReadInt();
			m_StrPointsUsed = reader.ReadInt();
			m_DexPointsUsed = reader.ReadInt();
			m_IntPointsUsed = reader.ReadInt();
			m_TotalStatPointsAquired = reader.ReadInt();
			m_PetSpeak = reader.ReadBool();
			m_TeleToTargetSwitch = reader.ReadBool();
			m_MassProvokeSwitch = reader.ReadBool();
			m_MassPeace = reader.ReadBool();	
			m_SuperHeal = reader.ReadBool();
			m_BlessedPower = reader.ReadBool();
			m_AreaFireBlast = reader.ReadBool();
			m_AreaIceBlast = reader.ReadBool();
			m_AreaAirBlast = reader.ReadBool();
			m_pethappyrewards = reader.ReadBool();
			m_pethappyrewardtier = reader.ReadInt();
			m_pethappylevelexp = reader.ReadInt();
			m_pethappylevel = reader.ReadInt();
			m_pethappygiftgiven = reader.ReadBool();
			m_pethappysound = reader.ReadBool();
			m_proximityrange = reader.ReadInt();
			m_pethappyoversleep = reader.ReadBool();
			m_pethappychangecolor = reader.ReadBool();
			m_levelpacktrait = reader.ReadBool();
		}
	}
}