using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Items
{
    public class SkillCoin : Item
    {
        private int m_SKV = 5;
		private bool m_expadded = false;
		private bool m_mod		= false;

        [CommandProperty(AccessLevel.GameMaster)]
        public int SKV
        {
            get { return m_SKV; }
            set 
			{ 
				m_SKV = value; 
				m_mod = true;
				InvalidateProperties(); 
			}
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Mod
        {
            get { return m_mod; }
            set 
			{ 
				m_mod = value; 
				InvalidateProperties(); 
			}
        }
        
        [Constructable]
        public SkillCoin()
            : base(0x1869)
        {
            Name = "A Skill Coin";
            Weight = 1.0;
            LootType = LootType.Blessed;
			ItemID = 10922;
        }
		
        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);
			
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));	
			if (m_ItemxmlSys != null && m_ItemxmlSys.PlayerLevels == true)
			{
				if (parent is Mobile)
				{
					Mobile from = (Mobile)parent;
					if (Mod != true)
					{
						SKV = m_ItemxmlSys.SkillCoinValue;
						m_expadded = true;
					}
					this.InvalidateProperties();
					return;
				}
				else
				{
					if (Mod != true)
					{
						SKV = m_ItemxmlSys.SkillCoinValue;
						m_expadded = true;
					}
					this.InvalidateProperties();
				}
			}
		}

        public override void OnDoubleClick(Mobile from)
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
				if (Mod != true)
				{
					SKV = m_ItemxmlSys.SkillCoinValue;
					m_expadded = true;
				}
				
				this.InvalidateProperties();
				LevelSheet xmlplayer = null;
				xmlplayer = from.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
				PlayerMobile pm = from as PlayerMobile;

				if (IsChildOf(pm.Backpack))
				{
					if (pm.SkillsTotal >= m_ItemxmlSys.SkillCoinCap)  //Edit this value based on your servers skill cap
					{
						pm.SendMessage("You have reached the skill cap, what do you need more skill points for");
						return;
					}
					else
						xmlplayer.SKPoints += m_SKV;
						pm.SendMessage("You have been awarded {0} skill points", m_SKV);
						this.Delete();               
				}
				else
					pm.SendMessage("This must be in your pack!");

			}
			else
			{
				from.SendMessage("Level System Disabled!!");
				return;
			}

        }
		


        public override void GetProperties(ObjectPropertyList list)
        {
			base.GetProperties(list);
			if (m_expadded == true)
			{
				list.Add("+{0} Skill Coin", m_SKV.ToString(), "Skill Points");
			}
        }

        
        public SkillCoin(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            
            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        m_SKV = reader.ReadInt();
                        break;
                    }
            }
			m_expadded = reader.ReadBool();
			m_mod = reader.ReadBool();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)SKV);
			writer.Write((bool)m_expadded);
			writer.Write((bool)m_mod);
			
        }
    }
}