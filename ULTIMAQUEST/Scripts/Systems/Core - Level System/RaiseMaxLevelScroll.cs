using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Items
{
    public class RaiseMaxLevelScroll : Item
    {
        private int m_RML;

        [CommandProperty(AccessLevel.GameMaster)]
        public int RML
        {
            get { return m_RML; }
            set { m_RML = value; InvalidateProperties(); }
        }
        public RaiseMaxLevelScroll(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)m_RML);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            LootType = LootType.Blessed;

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_RML = reader.ReadInt();
                        break;
                    }
            }
        }
        [Constructable]
        public RaiseMaxLevelScroll()
            : base(0x14F0)
        {
			Name = "Raise Max Level Scroll";
            Weight = 1.0;
            Hue = 1153;
            LootType = LootType.Blessed;

            m_RML = Utility.RandomMinMax(1, 5);
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

 
			list.Add("+{0}", m_RML.ToString()); // value: ~1_val~
        }
        public override void OnDoubleClick(Mobile from)
        {
			LevelSheet xmlplayer = null;
			xmlplayer = from.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
            PlayerMobile pm = from as PlayerMobile;

			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return;

            if (IsChildOf(pm.Backpack))
            {
				int givelevel = m_RML + m_ItemxmlSys.EndMaxLvl;
				
				if (xmlplayer.EndMaxLvl >= m_ItemxmlSys.EndMaxLvl)
				{
					pm.SendMessage("You cannot raise any farther with this!");
					return;
				}
				else
				{
					if (givelevel > m_ItemxmlSys.EndMaxLvl)
					{
						xmlplayer.EndMaxLvl = m_ItemxmlSys.EndMaxLvl;	
						pm.SendMessage("You have reached the max level possible with scrolls at this time!");
						this.Consume();
						return;
					}
					else
					{
						xmlplayer.EndMaxLvl += m_RML;
						pm.SendMessage("Your Max Level has increased by {0}", m_RML);
						this.Consume();
					}
				}
			}
            else
                pm.SendMessage("This must be in your pack!");
        }
    }
}