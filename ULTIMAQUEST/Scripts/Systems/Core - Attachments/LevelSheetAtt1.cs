using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Engines.LevelSystemExtAtt
{
    public class LevelSheetAtt1 : XmlAttachmentExt
    {

        public LevelSheetAtt1(ASerial serial) : base(serial)
        {
        }

        [Attachable]
        public LevelSheetAtt1()
        {

        }

        [Attachable]
        public LevelSheetAtt1(double seconds, double duration)
        {
		
        }
		
		public override void OnAttach()
		{
			base.OnAttach();
			if(AttachedTo is LevelSheet)
			{
				
				InvalidateParentProperties();
			}
			else
				Delete();
		}
		public override void OnDelete()
		{
			base.OnDelete();
			if(AttachedTo is LevelSheet)
			{
				InvalidateParentProperties();
			}
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write( (int) 0 );
			// version 
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			// version 0
		}
		
    }
}