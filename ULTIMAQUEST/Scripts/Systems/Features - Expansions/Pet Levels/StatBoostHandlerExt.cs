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
    public class StatBoostHandlerExt
    {
		public static void BonusStatAtt10(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 10, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 10, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 10, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt20(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 15, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 15, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 15, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt30(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 20, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 20, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 20, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt40(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 21, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 21, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 21, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt50(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 22, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 22, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 22, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt60(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 24, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 24, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 24, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt70(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 25, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 25, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 25, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt80(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 26, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 26, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 26, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt90(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 28, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 28, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 28, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt100(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 29, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 29, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 29, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt140(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 30, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 30, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 30, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt160(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 31, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 31, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 31, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt180(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 32, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 32, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 32, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt200(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 35, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 35, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 35, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}
		public static void BonusStatAtt201(Mobile player, LevelControlSys cp)
		{
			if (cp.EmotesOnAuraBoost)
			{
				player.Emote("*{0} received a battle bonus from their pet!*", player.Name);
			}
			player.AddStatMod(new StatMod(StatType.Dex, "XmlDex", 40, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Str, "XmlStr", 40, TimeSpan.FromSeconds( 3600.0 )));
			player.AddStatMod(new StatMod(StatType.Int, "XmlInt", 40, TimeSpan.FromSeconds( 3600.0 )));
			player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			player.PlaySound(0x1EA);
			player.InvalidateProperties();
		}

	}
}