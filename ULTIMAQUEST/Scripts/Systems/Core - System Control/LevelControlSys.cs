using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Engines.LevelSystemExtAtt
{
    public class LevelControlSys : XmlAttachmentExt
    {
		private bool m_playerlevels			= true;
		private bool m_equipmentlevelsystem	= false;
        private bool m_weaponlevels			= false;
        private bool m_armorlevels			= false;
        private bool m_jewellevels			= false;
        private bool m_clothinglevels		= false;
		private bool m_gainexpfrombods		= false;
		private bool m_disableskillgain		= false;
		private bool m_levelbelowtoon		= false;
		private bool m_showpaperdolllevel	= false;
		private bool m_petkillgivesexp		= false;
		private bool m_craftgivesexp		= false;
		private bool m_advancedskillexp		= false;	//only fighting skills give exp?
		private bool m_tablesadvancedexp	= false;	//tables based exp 
		private bool m_staffhaslevel		= false;	//shows or hides a staffs level
		private bool m_bonusstatonlvl		= false;	//on level give a chance to get a bonous stat?
		private bool m_refreshonlevel		= true; 	//sets players hits, stam, and mana to max on level.
		private bool m_refreshonlevelpet	= false; 	//sets players hits, stam, and mana to max on level.
		private bool m_levelsheetperma		= false;	//prevents player from dropping levelsheet
		private bool m_showvendorlevels		= false;	
		private bool m_discountfromvendors	= false;
		private bool m_partyexpshare		= true;
		private bool m_partyexpsharesplit	= true;	//if parties share exp do they split it evenly?
		
		private bool m_levelstatresetbutton	= false;	// master stat reset button - adds a button to playerlevelgump
		private bool m_levelskillresetbutton	= false;	// master skill reset button - adds a button to playerlevelgump
		
		/* skill categories - set false to hide category */
		private bool m_miscelaneous			= false; 	// enabled by default
		private bool m_combat				= false; 	// enabled by default
		private bool m_tradeskills			= false; 	// enabled by default
		private bool m_magic				= false; 	// enabled by default
		private bool m_wilderness			= false; 	// enabled by default
		private bool m_thieving				= false; 	// enabled by default
		private bool m_bard					= false; 	// enabled by default
		private bool m_imbuing				= false; 	// enabled by default
		private bool m_throwing				= false; 	// enabled by default
		private bool m_mysticism			= false; 	// enabled by default
		private bool m_spellweaving			= false; 	// enabled by default
		
        private	int m_startmaxlvl			= 50;		//what is the highest level reachable without scrolls?
        private	int m_startmaxlvlpets		= 50;		//what is the highest level reachable without scrolls?
        private	int m_endmaxlvl				= 60;		//what is the highest level reachable with scrolls?
		private	int m_skillcoincap			= 6400;		//change this to match the servers skill cap
		private	int m_skillcoinvalue		= 6400;		//change this to match the servers skill cap
		private	int	m_partyexpsharerange	= 20;		//how far away before sharing stops?
		private	int m_exppoweramount		= 50;		//how much bonus exp for power hour?
		private	int m_expcoinvalue			= 100;
		
		/* level up multiplier */
		/* math = current level * multipier = amountneededfornextlevelup */
		/* reduce these numbers to lower the amount of exp needed per level */
		private int m_l2to20multipier		= 100;		// leve 2 to level 20 
		private int m_l21to40multiplier		= 200;		// leve 21 to level 40 
		private int m_l41to60multiplier		= 400;		// leve 41 to level 60 
		private int m_l61to70multiplier		= 700;		// leve 61 to level 70 
		private int m_l71to80multiplier		= 900;		// leve 71 to level 80 
		private int m_l81to90multipier		= 1100;		// leve 81 to level 90 
		private int m_l91to100multipier		= 1300;		// leve 91 to level 100 
		private int m_l101to110multiplier	= 1500;		// leve 101 to level 110 
		private int m_l111to120multiplier	= 1700;		// leve 110 to level 120 
		private int m_l121to130multiplier	= 1900;		// leve 121 to level 130 
		private int m_l131to140multiplier	= 2200;		// leve 131 to level 140 
		private int m_l141to150multiplier	= 2500;		// leve 141 to level 150 
		private int m_l151to160multiplier	= 3500;		// leve 151 to level 160 
		private int m_l161to170multiplier	= 3900;		// leve 161 to level 170 
		private int m_l171to180multiplier	= 4110;		// leve 171 to level 180 
		private int m_l181to190multiplier	= 4300;		// leve 181 to level 190 
		private int m_l191to200multiplier	= 6000;		// leve 191 to level 200
		
		/* configured equipment section */
		private bool m_activatedynamiclevelsystem			=	false;
		/* changing the below also affects the equipment level gump */
		private string m_nameofbattleratingstat			=	"battle rating:";
		private string m_requiredlevelmouseover			=	"required level:";		
		/*	change this if you do not want it call it battle rating. this applies to all equipment! */
		
		/*	each equipment variable will have 10 thresholds of configuration, you can add more if you 
			like however i felt 10 is a decent balance. in the below default settings anything below 
			level intensity of level 1 (100 in this case) doesn't require a level to equip!.
			explanation: so lets say you have a helmet with a battle rating of 101, then that means 
			the player must have a level of '5'	or higher to equip the helmet.  
			be sure to choose levels that make sense for the intesity you are picking! 
		*/
		/* basearmor - armor properties */
		private int m_equiprequiredlevel0		=	1;					/* default for newbie items */
		
		/* 									levels		threshhold											intensity */
		private int m_armorrequiredlevel1		=	5;		/*1*/		private int m_armorrequiredlevel1intensity = 100;
		private int m_armorrequiredlevel2		=	10;		/*2*/		private int m_armorrequiredlevel2intensity = 250;
		private int m_armorrequiredlevel3		=	20;		/*3*/		private int m_armorrequiredlevel3intensity = 450;
		private int m_armorrequiredlevel4		=	25;		/*4*/		private int m_armorrequiredlevel4intensity = 650;
		private int m_armorrequiredlevel5		=	35;		/*5*/		private int m_armorrequiredlevel5intensity = 750;
		private int m_armorrequiredlevel6		=	40;		/*6*/		private int m_armorrequiredlevel6intensity = 850;
		private int m_armorrequiredlevel7		=	45;		/*7*/		private int m_armorrequiredlevel7intensity = 950;
		private int m_armorrequiredlevel8		=	50;		/*8*/		private int m_armorrequiredlevel8intensity = 1050;
		private int m_armorrequiredlevel9		=	55;		/*9*/		private int m_armorrequiredlevel9intensity = 1500;
		private int m_armorrequiredlevel10	=	60;		/*10*/		private int m_armorrequiredlevel10intensity = 1800;
		
		/* baseweapon - weapon properties */
		/* 									levels		threshhold											intensity */
		private int m_weaponrequiredlevel1		=	5;		/*1*/		private int m_weaponrequiredlevel1intensity = 100;
		private int m_weaponrequiredlevel2		=	10;		/*2*/		private int m_weaponrequiredlevel2intensity = 250;
		private int m_weaponrequiredlevel3		=	20;		/*3*/		private int m_weaponrequiredlevel3intensity = 450;
		private int m_weaponrequiredlevel4		=	25;		/*4*/		private int m_weaponrequiredlevel4intensity = 650;
		private int m_weaponrequiredlevel5		=	35;		/*5*/		private int m_weaponrequiredlevel5intensity = 750;
		private int m_weaponrequiredlevel6		=	40;		/*6*/		private int m_weaponrequiredlevel6intensity = 850;
		private int m_weaponrequiredlevel7		=	45;		/*7*/		private int m_weaponrequiredlevel7intensity = 950;
		private int m_weaponrequiredlevel8		=	50;		/*8*/		private int m_weaponrequiredlevel8intensity = 1050;
		private int m_weaponrequiredlevel9		=	55;		/*9*/		private int m_weaponrequiredlevel9intensity = 1500;
		private int m_weaponrequiredlevel10		=	60;		/*10*/		private int m_weaponrequiredlevel10intensity = 1800;

		
		/* baseclothing - clothing properties */
		/* 									levels		threshhold											intensity */
		private int m_clothrequiredlevel1		=	5;		/*1*/		private int m_clothrequiredlevel1intensity = 100;
		private int m_clothrequiredlevel2		=	10;		/*2*/		private int m_clothrequiredlevel2intensity = 250;
		private int m_clothrequiredlevel3		=	20;		/*3*/		private int m_clothrequiredlevel3intensity = 450;
		private int m_clothrequiredlevel4		=	25;		/*4*/		private int m_clothrequiredlevel4intensity = 650;
		private int m_clothrequiredlevel5		=	35;		/*5*/		private int m_clothrequiredlevel5intensity = 750;
		private int m_clothrequiredlevel6		=	40;		/*6*/		private int m_clothrequiredlevel6intensity = 850;
		private int m_clothrequiredlevel7		=	45;		/*7*/		private int m_clothrequiredlevel7intensity = 950;
		private int m_clothrequiredlevel8		=	50;		/*8*/		private int m_clothrequiredlevel8intensity = 1050;
		private int m_clothrequiredlevel9		=	55;		/*9*/		private int m_clothrequiredlevel9intensity = 1500;
		private int m_clothrequiredlevel10	=	60;		/*10*/		private int m_clothrequiredlevel10intensity = 1800;
		
		/* basejewel - jewel properties */
		/* 									levels		threshhold											intensity */
		private int m_jewelrequiredlevel1		=	5;		/*1*/		private int m_jewelrequiredlevel1intensity = 100;
		private int m_jewelrequiredlevel2		=	10;		/*2*/		private int m_jewelrequiredlevel2intensity = 250;
		private int m_jewelrequiredlevel3		=	20;		/*3*/		private int m_jewelrequiredlevel3intensity = 450;
		private int m_jewelrequiredlevel4		=	25;		/*4*/		private int m_jewelrequiredlevel4intensity = 650;
		private int m_jewelrequiredlevel5		=	35;		/*5*/		private int m_jewelrequiredlevel5intensity = 750;
		private int m_jewelrequiredlevel6		=	40;		/*6*/		private int m_jewelrequiredlevel6intensity = 850;
		private int m_jewelrequiredlevel7		=	45;		/*7*/		private int m_jewelrequiredlevel7intensity = 950;
		private int m_jewelrequiredlevel8		=	50;		/*8*/		private int m_jewelrequiredlevel8intensity = 1050;
		private int m_jewelrequiredlevel9		=	55;		/*9*/		private int m_jewelrequiredlevel9intensity = 1500;
		private int m_jewelrequiredlevel10	=	60;		/*10*/		private int m_jewelrequiredlevel10intensity = 1800;
		/* end configured equipment section */
		
		
		private bool m_enabledlevelpets		= false;		/* is the pet level system activated? */	
	
		
		private bool m_mountedpetsgainexp		= false;		/* if the pet is mounted does it still gain exp? */
		
		private bool m_petattackbonus			= false;		/* attaches on logon - needs xmlpetlevelatt attached to pet for this to work! */
													/* must enable attacks below! */
		private bool m_levelbelowpet			= false;		/*	pet level displayed below pet */
		private bool m_loseexplevelondeath		= false;	/*	false by default */
		private bool m_losestatondeath			= false;	/*	false by default - can disable to lose level only, not stat. */
		private int m_petstatlossamount			= 20;		/*	20 is 5%  - by default - losestatondeath must be true to apply */
		
		private bool m_petlevelsheetperma		= false;		/*	pet level sheet unable to be dropped? set true to prevent drop */ 
		private bool m_petexpgainfromkill 		= false;		/*	does pets gain exp from kills, must have attachment!*/
		
		private int m_maxlevelwithscroll		= 150;		/*	what is the highest level reachable without scrolls? */
		private int m_skillstotal				= 1200000;	/* if pet is equal to or exceeds they will not
														gain skill points on level up */
		
		private bool m_notifyonpetexpgain		= false;		/*	will tell the pet owner if the pet gains exp */
		private bool m_notifyonpetlevelup		= false;		/*	will tell the pet owner if the pet gains a level */

        private bool m_untamedcreaturelevels	= false;		//do untamed creatures have levels?
		
		/* chance key:  0.01 is 1% 
						0.05 is 5%
						0.1 is 10%
						0.3  is 30%
						0.5	 is 50%  
		using general add and subtraction and conversion from decimal to percent 
		you can choose a lot of possible chance variables. 
		*/
		
		private bool	m_emoteonspecialatks	= false;
		private bool	m_emotesonauraboost	= false;
		/* these toggles override the toggles on the actual pets!! */
		
		private bool	m_superheal			= false;		/*	outside of the pets normal healing ability, this grants additional health periodically. */
		private bool	m_petshouldbebonded	= false;		/*	true by default.  bonded pets only benefit */
		private int		m_superhealreq		= 80;		/*	must be this level or higher to use */
		private double	m_superhealchance		= 0.03;		/*	activation chance on movement - default 0.01	*/
		
		private bool	m_teleporttotarget	= false;	/*	this teleports the pet to far away targets */
		private int		m_teleporttotargetreq	= 60;		/*	must be this level or higher to use */
		private double	m_teleporttotarchance	= 0.8;		/*	activation chance on movement - default 0.8	*/
		
		private bool	m_massprovoketoatt	= false;	/*	this will allow the pet to provoke untamed creatures to its target */
		private int		m_massprovoketoattreq	= 60;		/*	must be this level or higher to use */
		private double	m_massprovokechance	= 0.8;		/*	activation chance on movement - default 0.8	*/
		
		private bool	m_masspeacearea		= false;	/*	this will allow the pet to mass peace an area around it, stopping war */
		private int		m_masspeacereq		= 60;		/*	must be this level or higher to use */
		private double	m_masspeacechance		= 0.8;		/*	activation chance on movement - default 0.8	*/
		
		private bool	m_blessedpower		= false;	/*	this will allow the pet to get temporary stat boost */
		private int		m_blessedpowerreq		= 60;		/*	must be this level or higher to use */
		private double	m_blessedpowerchance	= 0.8;		/*	activation chance on movement - default 0.8	*/
		
		private bool	m_areafireblast		= false;	/*	this will allow the pet to blast area with fire */
		private int		m_areafireblastreq	= 60;		/*	must be this level or higher to use */
		private double	m_areafireblastchance	= 0.8;		/*	activation chance on attack - default 0.3	*/
		
		private bool	m_areaiceblast		= false;	/*	this will allow the pet to blast area with ice */
		private int		m_areaiceblastreq		= 60;		/*	must be this level or higher to use */
		private double	m_areaiceblastchance	= 0.8;		/*	activation chance on attack - default 0.3	*/
		
		private bool	m_areaairblast		= false;	/*	this will allow the pet to blast area with ice */
		private int		m_areaairblastreq		= 60;		/*	must be this level or higher to use */
		private double	m_areaairblastchance	= 0.8;		/*	activation chance on attack - default 0.3	*/

		/* aurastatboost may require you to increase the stat cap on server */ 
		/* aurastatboost increases benefits for higher levels - playerexp must be enabled! */
		private bool	m_aurastatboost		= false;		/*	this aura grants a stat boost to the pet master */
		private int		m_aurastatboostreq	= 60;		/*	must be this level or higher to use */

		private bool 	m_enablemountcheck 	= false;
		/* turn this to false to not have the mounts levels checked. */
		/* be sure to update the mount levels below. */
        //for individual levels:
        private int 	m_beetle				= 1;
        private int 	m_desertostard			= 1;
        private int 	m_firesteed				= 1;
        private int 	m_forestostard			= 1;
        private int		m_frenziedostard		= 1;
        private int		m_hellsteed				= 1;
        private int		m_hiryu					= 1;
        private int		m_horse					= 1;
        private int		m_kirin					= 1;
        private int		m_lesserhiryu			= 1;
        private int		m_nightmare				= 1;
        private int		m_ridablellama			= 1;
        private int		m_ridgeback				= 1;
        private int		m_savageridgeback		= 1;
        private int		m_scaledswampdragon		= 1;
        private int		m_seahorse				= 1;
        private int		m_silversteed			= 1;
        private int		m_skeletalmount			= 1;
        private int		m_swampdragon			= 1;
        private int		m_unicorn				= 1;
		private int		m_reptalon				= 1;
		private int		m_wildtiger				= 1;
		private int		m_windrunner			= 1;
		private int		m_lasher				= 1;
		private int		m_eowmu					= 1;
		private int		m_dreadwarhorse			= 1;
		private int		m_cusidhe				= 1;
		
		/* how many skill points awarded per level.
			scenario: if turning level 18, below20 applies*/
		
		private int	m_below20					= 4;	//below level 20
		private int m_below40					= 4;	//below level 40
		private int m_below60					= 4;	//below level 60
		private int m_below70					= 4;	//below level 70
		private int m_below80					= 4;	//below level 80
		private int m_below90					= 4;	//below level 90
		private int m_below100					= 4;	//below level 100
		private int m_below110					= 4;	//below level 110
		private int m_below120					= 4;	//below level 120
		private int m_below130					= 4;	//below level 130
		private int m_below140					= 4;	//below level 140
		private int m_below150					= 4;	//below level 150
		private int m_below160					= 4;	//below level 160
		private int m_below170					= 4;	//below level 170
		private int m_below180					= 4;	//below level 180
		private int m_below190					= 4;	//below level 190
		private int m_below200					= 4;	//below level 200

		/* how many stat points to be awarded per level */
		private int m_below20stat				= 3;	//below level 20
		private int m_below40stat				= 3;	//below level 40
		private int m_below60stat				= 3;	//below level 60
		private int m_below70stat				= 3;	//below level 70
		private int m_below80stat				= 3;	//below level 80
		private int m_below90stat				= 3;	//below level 90
		private int m_below100stat				= 3;	//below level 100
		private int m_below110stat				= 3;	//below level 110
		private int m_below120stat				= 3;	//below level 120
		private int m_below130stat				= 3;	//below level 130
		private int m_below140stat				= 3;	//below level 140
		private int m_below150stat				= 3;	//below level 150
		private int m_below160stat				= 3;	//below level 160
		private int m_below170stat				= 3;	//below level 170
		private int m_below180stat				= 3;	//below level 180
		private int m_below190stat				= 3;	//below level 190
		private int m_below200stat				= 3;	//below level 200
		
		/* pet stealing system */
		/*	this takes into account the lockpicking skill and if the player can control the pet. (taming)
			additionally if the player has enough stealing skill they get a bonus, the pets stats are not 
			scaled down and the experience and level stay intact if they are successful on the stealing check */
		private int m_minskillreqpicksteal		= 65;
		private bool m_enablepetpicks			= false;	/*	though this requires pet level system to be enabled, you can still
														disable the pet picks in case they are no longer wanted. */
		private bool m_preventbondedpetpick		= false;
		
		/* pet max levels - if using dynamic max levels, 'starmaxlvl' will be ignored! */
		private bool m_usedynamicmaxlevels		= false;
		
		
		/* configured skills */
		
		private int m_maxstatpoints				= 15000;	/* if player is equal to or exceeds they will not
															gain stat points on level up */
		private int m_petmaxstatpoints			= 500;
		/* how many skill points awarded per level.
			scenario: if turning level 18, below20 applies*/

		private int m_Lvbelow20					= 4;		/*	below level 20	*/
		private int m_Lvbelow40					= 4;		/*	below level 40	*/
		private int m_Lvbelow60					= 4;		/*	below level 60	*/
		private int m_Lvbelow70					= 4;		/*	below level 70	*/
		private int m_Lvbelow80					= 4;		/*	below level 80	*/
		private int m_Lvbelow90					= 4;		/*	below level 90	*/
		private int m_Lvbelow100				= 4;		/*	below level 100	*/
		private int m_Lvbelow110				= 4;		/*	below level 110	*/
		private int m_Lvbelow120				= 4;		/*	below level 120	*/
		private int m_Lvbelow130				= 4;		/*	below level 130	*/
		private int m_Lvbelow140				= 4;		/*	below level 140	*/
		private int m_Lvbelow150				= 4;		/*	below level 150	*/
		private int m_Lvbelow160				= 4;		/*	below level 160	*/
		private int m_Lvbelow170				= 4;		/*	below level 170	*/
		private int m_Lvbelow180				= 4;		/*	below level 180	*/
		private int m_Lvbelow190				= 4;		/*	below level 190	*/
		private int m_Lvbelow200				= 4;		/*	below level 200	*/

		/* how many follower points to award per level? can even be zero */
		private bool m_gainfollowerslotonlevel			= false;
		/* false by default, grant a follower per 20-10 levels */
		/* warning. without proper planning this could be game breaking or 
		over kill*/
		private int		m_gainfollowerslotonlevel20		= 1;		/*	at level 20		*/
		private bool 	m_gainon20						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel30		= 1;		/*	at level 30		*/
		private bool 	m_gainon30						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel40		= 1;		/*	at level 40		*/
		private bool 	m_gainon40						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel50		= 1;		/*	at level 50		*/
		private bool 	m_gainon50						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel60		= 1;		/*	at level 60		*/
		private bool 	m_gainon60						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel70		= 1;		/*	at level 70		*/
		private bool 	m_gainon70						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel80		= 1;		/*	at level 80		*/
		private bool 	m_gainon80						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel90		= 1;		/*	at level 90		*/
		private bool 	m_gainon90						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel100	= 1;		/*	at level 100	*/
		private bool 	m_gainon100						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel110	= 1;		/*	at level 110	*/
		private bool 	m_gainon110						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel120	= 1;		/*	at level 120	*/
		private bool 	m_gainon120						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel130	= 1;		/*	at level 130	*/
		private bool 	m_gainon130						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel140	= 1;		/*	at level 140	*/
		private bool 	m_gainon140						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel150	= 1;		/*	at level 150	*/
		private bool 	m_gainon150						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel160	= 1;		/*	at level 160	*/
		private bool 	m_gainon160						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel170	= 1;		/*	at level 170	*/
		private bool	m_gainon170						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel180	= 1;		/*	at level 180	*/
		private bool	m_gainon180						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel190	= 1;		/*	at level 190	*/
		private bool	m_gainon190						= false;		/* gain on level?	*/
		private int		m_gainfollowerslotonlevel200	= 1;		/*	at level 200	*/
		private bool	m_gainon200					= false;		/* gain on level?	*/
		private bool	m_enableexpfromskills		= false;
		private bool	m_begginggain				= false;
		private int		m_begginggainamount			= 10;
		private bool	m_campinggain				= false;
		private int		m_campinggainamount			= 10;
		private bool	m_forensicsgain				= false;
		private int		m_forensicsgainamount		= 10;
		private bool	m_itemidgain				= false;
		private int 	m_itemidgainamount			= 10;
		private bool	m_tasteidgain				= false;
		private int 	m_tasteidgainamount			= 10;
		private bool	m_imbuinggain				= false;
		private int 	m_imbuinggainamount			= 10;
		private bool	m_evalintgain				= false;
		private int 	m_evalintgainamount			= 10;
		private bool	m_spiritspeakgain			= false;
		private int 	m_spiritspeakgainamount		= 10;
		private bool	m_fishinggain				= false;
		private int 	m_fishinggainamount			= 10;
		private bool	m_herdinggain				= false;
		private int 	m_herdinggainamount			= 10;
		private bool	m_trackinggain				= false;
		private int 	m_trackinggainamount		= 10;
		private bool	m_hidinggain				= false;
		private int 	m_hidinggainamount			= 10;
		private bool	m_poisoninggain				= false;
		private int 	m_poisoninggainamount		= 10;
		private bool	m_removetrapgain			= false;
		private int 	m_removetrapgainamount		= 10;
		private bool	m_stealinggain				= false;
		private int 	m_stealinggainamount		= 10;
		private bool	m_discordancegain			= false;
		private int 	m_discordancegainamount		= 10;
		private bool	m_peacemakinggain			= false;
		private int 	m_peacemakinggainamount		= 10;
		private bool	m_provocationgain			= false;
		private int 	m_provocationgainamount		= 10;
		private bool	m_anatomygain				= false; 
		private int 	m_anatomygainamount			= 10;
		private bool	m_armsloregain				= false;
		private int 	m_armsloregainamount		= 10;
		private bool	m_animalloregain			= false;
		private int 	m_animalloregainamount		= 10;
		private bool	m_meditationgain			= false;
		private int 	m_meditationgainamount		= 10;
		private bool	m_cartographygain			= false;
		private int 	m_cartographygainamount		= 10;
		private bool	m_detecthiddengain			= false;
		private int 	m_detecthiddengainamount	= 10;
		private bool	m_animaltaminggain			= false;
		private int 	m_animaltaminggainamount	= 10;
		private bool	m_blacksmithgain			= false;
		private int 	m_blacksmithgainamount		= 10;
		private bool	m_carpentrygain				= false;
		private int 	m_carpentrygainamount		= 10;
		private bool	m_alchemygain				= false;
		private int 	m_alchemygainamount			= 10;
		private bool	m_fletchinggain				= false;
		private int 	m_fletchinggainamount		= 10;
		private bool	m_cookinggain				= false;
		private int 	m_cookinggainamount			= 10;
		private bool	m_inscribegain				= false;
		private int 	m_inscribegainamount		= 10;
		private bool 	m_tailoringgain				= false;
		private int 	m_tailoringgainamount		= 10;
		private bool	m_tinkeringgain				= false;
		private int 	m_tinkeringgainamount		= 10;	
		
		/* start level bag configurations */
		/* When this bag is placed into a characters backpack, it should be made
			to not be removed by the player.  Null checks can prevent crashes however
			if another player takes the bag, that other players level will change the
			max items of this bag and it won't be usable until they remove enough items.
			If that isn't an issue then by all means let the bag be passed around.
			
			I will confiure 10 level Groups, if you want more then configure them. If 
			you want less, then turn them off.
		*/
		/* turning this to off will make the bag into a regular bag - could cause glitches
			if its been in use for awhile. did this as a just in case. better to turn off 
			options on their own below. */
		private bool m_bagsystemmaintoggle			= true;
		/* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!! */
		
		/* misc configs */
		private bool m_preventbagdrop				= true; /* set true to prevent drop */
		private bool m_bagblessed					= true; /* doesn't keep items on death,just the bag. maybe future feature */
		
		
		
		/* level 1 group configs */
		private bool	m_levelgroup1				= true;
		private bool 	m_level1groupbagownership	= true;
		private int 	m_levelgroup1reqlevel		= 10;
		private int 	m_levelgroup1maxitems		= 130;		/* if you want this default then change it to 125 */
		private int 	m_levelgroup1reducetotal	= 10;	/* reduces weight of items in bag - this will only apply
															moving through level groups - in % value. if you want this 
															to not apply, then set the number to 0.*/ 
		private string	m_levelgroup1msg			= "your power increased this bags potential!";
		private string	m_level1groupownermsg		= "this isn't your bag now is it?!";
		private string	m_level1groupownernow		= "this bag can only be used by this character now!";
		
		/* level 2 group configs */
		private bool	m_levelgroup2				= true;
		private bool	m_level2groupbagownership	= true;
		private int 	m_levelgroup2reqlevel		= 20;
		private int 	m_levelgroup2maxitems		= 140;		/* if you want this default then change it to 125 */
		private int 	m_levelgroup2reducetotal	= 15;	/* reduces weight of items in bag - this will only apply
															moving through level groups - in % value. if you want this 
															to not apply, then set the number to 0.*/ 
		private string	m_levelgroup2msg			= "your power increased this bags potential!";
		private string	m_level2groupownermsg		= "this isn't your bag now is it?!";
		private string	m_level2groupownernow		= "this bag can only be used by this character now!";
		
		/* level 3 group configs */
		private bool	m_levelgroup3				= true;
		private bool	m_level3groupbagownership	= true;
		private int 	m_levelgroup3reqlevel		= 30;
		private int 	m_levelgroup3maxitems		= 150;		/* if you want this default then change it to 125 */
		private int 	m_levelgroup3reducetotal	= 20;	/* reduces weight of items in bag - this will only apply
															moving through level groups - in % value. if you want this 
															to not apply, then set the number to 0.*/ 
		private string m_levelgroup3msg				= "your power increased this bags potential!";
		private string m_level3groupownermsg		= "this isn't your bag now is it?!";
		private string m_level3groupownernow		= "this bag can only be used by this character now!";
		
		/* level 4 group configs */
		private bool	m_levelgroup4				= true;
		private bool	m_level4groupbagownership	= true;
		private int 	m_levelgroup4reqlevel		= 40;
		private int 	m_levelgroup4maxitems		= 160;		/* if you want this default then change it to 125 */
		private int 	m_levelgroup4reducetotal	= 25;	/* reduces weight of items in bag - this will only apply
															moving through level groups - in % value. if you want this 
															to not apply, then set the number to 0.*/ 
		private string	m_levelgroup4msg			= "your power increased this bags potential!";
		private string	m_level4groupownermsg		= "this isn't your bag now is it?!";
		private string	m_level4groupownernow		= "this bag can only be used by this character now!";
		
		/* level 5 group configs */
		private bool	m_levelgroup5				= true;
		private bool	m_level5groupbagownership	= true;
		private int 	m_levelgroup5reqlevel		= 50;
		private int 	m_levelgroup5maxitems		= 170;		/* if you want this default then change it to 125 */
		private int 	m_levelgroup5reducetotal	= 30;	/* reduces weight of items in bag - this will only apply
															moving through level groups - in % value. if you want this 
															to not apply, then set the number to 0.*/ 
		private string	m_levelgroup5msg			= "your power increased this bags potential!";
		private string	m_level5groupownermsg		= "this isn't your bag now is it?!";
		private string	m_level5groupownernow		= "this bag can only be used by this character now!";
		
		/* level 6 group configs */
		private bool	m_levelgroup6				= true;
		private bool	m_level6groupbagownership	= true;
		private int 	m_levelgroup6reqlevel		= 60;
		private int 	m_levelgroup6maxitems		= 180;		/* if you want this default then change it to 125 */
		private int 	m_levelgroup6reducetotal	= 35;	/* reduces weight of items in bag - this will only apply
															moving through level groups - in % value. if you want this 
															to not apply, then set the number to 0.*/ 
		private string	m_levelgroup6msg			= "your power increased this bags potential!";
		private string	m_level6groupownermsg		= "this isn't your bag now is it?!";
		private string	m_level6groupownernow		= "this bag can only be used by this character now!";
		
		/* level 7 group configs */
		private bool	m_levelgroup7				= true;
		private bool	m_level7groupbagownership	= true;
		private int 	m_levelgroup7reqlevel		= 70;
		private int 	m_levelgroup7maxitems		= 190;		/* if you want this default then change it to 125 */
		private int 	m_levelgroup7reducetotal	= 40;	/* reduces weight of items in bag - this will only apply
															moving through level groups - in % value. if you want this 
															to not apply, then set the number to 0.*/ 
		private string	m_levelgroup7msg			= "your power increased this bags potential!";
		private string	m_level7groupownermsg		= "this isn't your bag now is it?!";
		private string	m_level7groupownernow		= "this bag can only be used by this character now!";
		
		/* level 8 group configs */
		private bool	m_levelgroup8				= true;
		private bool	m_level8groupbagownership	= true;
		private int 	m_levelgroup8reqlevel		= 80;
		private int 	m_levelgroup8maxitems		= 200;		/* if you want this default then change it to 125 */
		private int 	m_levelgroup8reducetotal	= 45;	/* reduces weight of items in bag - this will only apply
															moving through level groups - in % value. if you want this 
															to not apply, then set the number to 0.*/ 
		private string	m_levelgroup8msg			= "your power increased this bags potential!";
		private string	m_level8groupownermsg		= "this isn't your bag now is it?!";
		private string	m_level8groupownernow		= "this bag can only be used by this character now!";
		
		/* level 9 group configs */
		private bool	m_levelgroup9				= true;
		private bool	m_level9groupbagownership	= true;
		private int 	m_levelgroup9reqlevel		= 90;
		private int 	m_levelgroup9maxitems		= 225;		/* if you want this default then change it to 125 */
		private int 	m_levelgroup9reducetotal	= 50;	/* reduces weight of items in bag - this will only apply
															moving through level groups - in % value. if you want this 
															to not apply, then set the number to 0.*/ 
		private string	m_levelgroup9msg			= "your power increased this bags potential!";
		private string	m_level9groupownermsg		= "this isn't your bag now is it?!";
		private string	m_level9groupownernow		= "this bag can only be used by this character now!";
		
		/* level 10 group configs */
		private bool	m_levelgroup10				= true;
		private bool	m_level10groupbagownership	= true;
		private int 	m_levelgroup10reqlevel		= 100;
		private int 	m_levelgroup10maxitems		= 250;		/* if you want this default then change it to 125 */
		private int 	m_levelgroup10reducetotal	= 75;	/* reduces weight of items in bag - this will only apply
															moving through level groups - in % value. if you want this 
															to not apply, then set the number to 0.*/ 
		private string	m_levelgroup10msg			= "your power increased this bags potential!";
		private string	m_level10groupownermsg		= "this isn't your bag now is it?!";
		private string	m_level10groupownernow		= "this bag can only be used by this character now!";
		/* end level bag configurations */
		
		private int		m_powerhourtime					= 60;
		public bool		m_maxlevel 						= false;
		
		private int		m_petbelow20 				= 4;
		private int		m_petbelow40  				= 4;
		private int		m_petbelow60 				= 4;
		private int		m_petbelow70  				= 4;
		private int		m_petbelow80  				= 4;
		private int		m_petbelow90  				= 4;
		private int		m_petbelow100				= 4;
		private int		m_petbelow110  				= 4;
		private int		m_petbelow120				= 4;
		private int		m_petbelow130				= 4;
		private int		m_petbelow140				= 4;
		private int		m_petbelow150				= 4;
		private int		m_petbelow160				= 4;
		private int		m_petbelow170				= 4;
		private int		m_petbelow180				= 4;
		private int		m_petbelow190				= 4;
		private int		m_petbelow200				= 4;
	
		public int		m_petbelow20stat			= 3;		/*	m_petbelow level 20	*/
		public int		m_petbelow40stat			= 3;		/*	m_petbelow level 40	*/
		public int		m_petbelow60stat			= 3;		/*	m_petbelow level 60	*/
		public int		m_petbelow70stat			= 3;		/*	m_petbelow level 70	*/
		public int		m_petbelow80stat			= 3;		/*	m_petbelow level 80	*/
		public int		m_petbelow90stat			= 3;		/*	m_petbelow level 90	*/
		public int		m_petbelow100stat			= 3;		/*	m_petbelow level 100	*/
		public int		m_petbelow110stat			= 3;		/*	m_petbelow level 110	*/
		public int		m_petbelow120stat			= 3;		/*	m_petbelow level 120	*/
		public int		m_petbelow130stat			= 3;		/*	m_petbelow level 130	*/
		public int		m_petbelow140stat			= 3;		/*	m_petbelow level 140	*/
		public int		m_petbelow150stat			= 3;		/*	m_petbelow level 150	*/
		public int		m_petbelow160stat			= 3;		/*	m_petbelow level 160	*/
		public int		m_petbelow170stat			= 3;		/*	m_petbelow level 170	*/
		public int		m_petbelow180stat			= 3;		/*	m_petbelow level 180	*/
		public int		m_petbelow190stat			= 3;		/*	m_petbelow level 190	*/
		public int		m_petbelow200stat			= 3;		/*	below level 200	*/
		
		private bool m_pethappysystem				= false;
		private bool m_levelpacktrait				= false;
		private bool m_petreclaimchance				= false;
		
		
		private bool		m_newstartinglocation		= false;
		private bool		m_forcenewplayerintoguild	= false;
		private bool		m_addtobackpackonattach		= false;
		private int			m_x_variable				= 1495;
		private int			m_y_variable				= 1628;
		private int			m_z_variable				= 10; 
		private string		m_maplocation				=	"Trammel";
		private string		m_guildnamestart			=	"Enter Guild Name";
		
		private string		m_startitem1				=	"Item_name";
		private string		m_startitem2				=	"Item_name";
		private string		m_startitem3				=	"Item_name";
		private string		m_startitem4				=	"Item_name";
		private string		m_startitem5				=	"Item_name";
		private string		m_startitem6				=	"Item_name";
		private string		m_startitem7				=	"Item_name";
		private string		m_startitem8				=	"Item_name";
		private string		m_startitem9				=	"Item_name";
		private string		m_startitem10				=	"Item_name";
		
		
		
		
		private bool		m_forcestartingstats					= false;
		public int			m_forcestartingstatsstr					= 50;	
		public int			m_forcestartingstatsdex					= 50;	
		public int			m_forcestartingstatsint					= 50;	
		private bool		m_autoactivate_startingstrcap			= false;
		public int			m_startingstrcapvar						= 125;	
		public int			m_startingstrcapmaxvar					= 150;		
		private bool		m_autoactivate_startingdexcap			= false;
		public int			m_startingdexcapvar						= 125;	
		public int			m_startingdexcapmaxvar					= 150;		
		private bool		m_autoactivate_startingintcap			= false;
		public int			m_startingintcapvar						= 125;	
		public int			m_startingintcapmaxvar					= 150;		
		private bool		m_autoactivate_startingtotalstatcap		= false;
		public int			m_autoactivate_startingtotalstatcapvar	= 225;	
		private bool		m_autoactivate_gemmining				= false;
		private bool		m_autoactivate_basketweaving			= false;
		private bool		m_autoactivate_canbuycarpets			= false;
		private bool		m_autoactivate_acceptguildinvites		= false;
		private bool		m_autoactivate_glassblowing				= false;
		private bool		m_autoactivate_libraryfriend			= false;
		private bool		m_autoactivate_masonry					= false;
		private bool		m_autoactivate_sandmining				= false;
		private bool		m_autoactivate_stonemining				= false;
		private bool		m_autoactivate_spellweaving				= false;
		private bool		m_autoactivate_mechanicallife			= false;
		private bool		m_autoactivate_disabledpvpwarning		= false;
		private bool		m_autoactivate_isyoung					= false;
		private bool		m_autoactivate_cantwalk					= false;
		private bool		m_autoactivate_maxfollowslots			= false;
		private int			m_autoactivate_maxfollowslotstotal		= 5;
		private bool		m_autoactivate_skillscap				= false;
		private int			m_autoactivate_skillscapvar				= 700;

		private bool		m_mapbooltrammel						= true;
		private bool		m_mapboolfelucca						= false;
		private bool		m_mapboolmalas							= false;
		
		
		
        [CommandProperty(AccessLevel.GameMaster)]
        public bool EquipMentLevelSystem
        {
            get { return m_equipmentlevelsystem; }
            set { m_equipmentlevelsystem = value; InvalidateParentProperties(); }
        }	
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerLevels
        {
            get { return m_playerlevels; }
            set { m_playerlevels = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool WeaponLevels
        {
            get { return m_weaponlevels; }
            set { m_weaponlevels = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ArmorLevels
        {
            get { return m_armorlevels; }
            set { m_armorlevels = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool JewelLevels
        {
            get { return m_jewellevels; }
            set { m_jewellevels = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ClothingLevels
        {
            get { return m_clothinglevels; }
            set { m_clothinglevels = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool GainExpFromBods
        {
            get { return m_gainexpfrombods; }
            set { m_gainexpfrombods = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DisableSkillGain
        {
            get { return m_disableskillgain; }
            set { m_disableskillgain = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LevelBelowToon
        {
            get { return m_levelbelowtoon; }
            set { m_levelbelowtoon = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowPaperDollLevel
        {
            get { return m_showpaperdolllevel; }
            set { m_showpaperdolllevel = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PetKillGivesExp
        {
            get { return m_petkillgivesexp; }
            set { m_petkillgivesexp = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool CraftGivesExp
        {
            get { return m_craftgivesexp; }
            set { m_craftgivesexp = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AdvancedSkillExp
        {
            get { return m_advancedskillexp; }
            set { m_advancedskillexp = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool TablesAdvancedExp
        {
            get { return m_tablesadvancedexp; }
            set { m_tablesadvancedexp = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool StaffHasLevel
        {
            get { return m_staffhaslevel; }
            set { m_staffhaslevel = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BonusStatOnLvl
        {
            get { return m_bonusstatonlvl; }
            set { m_bonusstatonlvl = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RefreshOnLevel
        {
            get { return m_refreshonlevel; }
            set { m_refreshonlevel = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RefreshOnLevelPet
        {
            get { return m_refreshonlevelpet; }
            set { m_refreshonlevelpet = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LevelSheetPerma
        {
            get { return m_levelsheetperma; }
            set { m_levelsheetperma = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowVendorLevels
        {
            get { return m_showvendorlevels; }
            set { m_showvendorlevels = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DiscountFromVendors
        {
            get { return m_discountfromvendors; }
            set { m_discountfromvendors = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PartyExpShare
        {
            get { return m_partyexpshare; }
            set { m_partyexpshare = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PartyExpShareSplit
        {
            get { return m_partyexpsharesplit; }
            set { m_partyexpsharesplit = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LevelStatResetButton
        {
            get { return m_levelstatresetbutton; }
            set { m_levelstatresetbutton = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LevelSkillResetButton
        {
            get { return m_levelskillresetbutton; }
            set { m_levelskillresetbutton = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L2to20Multipier
        {
            get { return m_l2to20multipier; }
            set { m_l2to20multipier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L21to40Multiplier
        {
            get { return m_l21to40multiplier; }
            set { m_l21to40multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L41to60Multiplier
        {
            get { return m_l41to60multiplier; }
            set { m_l41to60multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L61to70Multiplier
        {
            get { return m_l61to70multiplier; }
            set { m_l61to70multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L71to80Multiplier
        {
            get { return m_l71to80multiplier; }
            set { m_l71to80multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L81to90Multipier
        {
            get { return m_l81to90multipier; }
            set { m_l81to90multipier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L91to100Multipier
        {
            get { return m_l91to100multipier; }
            set { m_l91to100multipier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L101to110Multiplier
        {
            get { return m_l101to110multiplier; }
            set { m_l101to110multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L111to120Multiplier
        {
            get { return m_l111to120multiplier; }
            set { m_l111to120multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L121to130Multiplier
        {
            get { return m_l121to130multiplier; }
            set { m_l121to130multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L131to140Multiplier
        {
            get { return m_l131to140multiplier; }
            set { m_l131to140multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L141to150Multiplier
        {
            get { return m_l141to150multiplier; }
            set { m_l141to150multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L151to160Multiplier
        {
            get { return m_l151to160multiplier; }
            set { m_l151to160multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L161to170Multiplier
        {
            get { return m_l161to170multiplier; }
            set { m_l161to170multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L171to180Multiplier
        {
            get { return m_l171to180multiplier; }
            set { m_l171to180multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L181to190Multiplier
        {
            get { return m_l181to190multiplier; }
            set { m_l181to190multiplier = value; InvalidateParentProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int L191to200Multiplier
        {
            get { return m_l191to200multiplier; }
            set { m_l191to200multiplier = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel1
        {
            get { return m_armorrequiredlevel1; }
            set { m_armorrequiredlevel1 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel2
        {
            get { return m_armorrequiredlevel2; }
            set { m_armorrequiredlevel2 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel3
        {
            get { return m_armorrequiredlevel3; }
            set { m_armorrequiredlevel3 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel4
        {
            get { return m_armorrequiredlevel4; }
            set { m_armorrequiredlevel4 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel5
        {
            get { return m_armorrequiredlevel5; }
            set { m_armorrequiredlevel5 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel6
        {
            get { return m_armorrequiredlevel6; }
            set { m_armorrequiredlevel6 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel7
        {
            get { return m_armorrequiredlevel7; }
            set { m_armorrequiredlevel7 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel8
        {
            get { return m_armorrequiredlevel8; }
            set { m_armorrequiredlevel8 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel9
        {
            get { return m_armorrequiredlevel9; }
            set { m_armorrequiredlevel9 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel10
        {
            get { return m_armorrequiredlevel10; }
            set { m_armorrequiredlevel10 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel1Intensity
        {
            get { return m_armorrequiredlevel1intensity; }
            set { m_armorrequiredlevel1intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel2Intensity
        {
            get { return m_armorrequiredlevel2intensity; }
            set { m_armorrequiredlevel2intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel3Intensity
        {
            get { return m_armorrequiredlevel3intensity; }
            set { m_armorrequiredlevel3intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel4Intensity
        {
            get { return m_armorrequiredlevel4intensity; }
            set { m_armorrequiredlevel4intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel5Intensity
        {
            get { return m_armorrequiredlevel5intensity; }
            set { m_armorrequiredlevel5intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel6Intensity
        {
            get { return m_armorrequiredlevel6intensity; }
            set { m_armorrequiredlevel6intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel7Intensity
        {
            get { return m_armorrequiredlevel7intensity; }
            set { m_armorrequiredlevel7intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel8Intensity
        {
            get { return m_armorrequiredlevel8intensity; }
            set { m_armorrequiredlevel8intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel9Intensity
        {
            get { return m_armorrequiredlevel9intensity; }
            set { m_armorrequiredlevel9intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ArmorRequiredLevel10Intensity
        {
            get { return m_armorrequiredlevel10intensity; }
            set { m_armorrequiredlevel10intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int EquipRequiredLevel0
        {
            get { return m_equiprequiredlevel0; }
            set { m_equiprequiredlevel0 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel1
        {
            get { return m_weaponrequiredlevel1; }
            set { m_weaponrequiredlevel1 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel2
        {
            get { return m_weaponrequiredlevel2; }
            set { m_weaponrequiredlevel2 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel3
        {
            get { return m_weaponrequiredlevel3; }
            set { m_weaponrequiredlevel3 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel4
        {
            get { return m_weaponrequiredlevel4; }
            set { m_weaponrequiredlevel4 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel5
        {
            get { return m_weaponrequiredlevel5; }
            set { m_weaponrequiredlevel5 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel6
        {
            get { return m_weaponrequiredlevel6; }
            set { m_weaponrequiredlevel6 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel7
        {
            get { return m_weaponrequiredlevel7; }
            set { m_weaponrequiredlevel7 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel8
        {
            get { return m_weaponrequiredlevel8; }
            set { m_weaponrequiredlevel8 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel9
        {
            get { return m_weaponrequiredlevel9; }
            set { m_weaponrequiredlevel9 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel10
        {
            get { return m_weaponrequiredlevel10; }
            set { m_weaponrequiredlevel10 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel1Intensity
        {
            get { return m_weaponrequiredlevel1intensity; }
            set { m_weaponrequiredlevel1intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel2Intensity
        {
            get { return m_weaponrequiredlevel2intensity; }
            set { m_weaponrequiredlevel2intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel3Intensity
        {
            get { return m_weaponrequiredlevel3intensity; }
            set { m_weaponrequiredlevel3intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel4Intensity
        {
            get { return m_weaponrequiredlevel4intensity; }
            set { m_weaponrequiredlevel4intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel5Intensity
        {
            get { return m_weaponrequiredlevel5intensity; }
            set { m_weaponrequiredlevel5intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel6Intensity
        {
            get { return m_weaponrequiredlevel6intensity; }
            set { m_weaponrequiredlevel6intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel7Intensity
        {
            get { return m_weaponrequiredlevel7intensity; }
            set { m_weaponrequiredlevel7intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel8Intensity
        {
            get { return m_weaponrequiredlevel8intensity; }
            set { m_weaponrequiredlevel8intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel9Intensity
        {
            get { return m_weaponrequiredlevel9intensity; }
            set { m_weaponrequiredlevel9intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int WeaponRequiredLevel10Intensity
        {
            get { return m_weaponrequiredlevel10intensity; }
            set { m_weaponrequiredlevel10intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel1
        {
            get { return m_clothrequiredlevel1; }
            set { m_clothrequiredlevel1 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel2
        {
            get { return m_clothrequiredlevel2; }
            set { m_clothrequiredlevel2 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel3
        {
            get { return m_clothrequiredlevel3; }
            set { m_clothrequiredlevel3 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel4
        {
            get { return m_clothrequiredlevel4; }
            set { m_clothrequiredlevel4 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel5
        {
            get { return m_clothrequiredlevel5; }
            set { m_clothrequiredlevel5 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel6
        {
            get { return m_clothrequiredlevel6; }
            set { m_clothrequiredlevel6 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel7
        {
            get { return m_clothrequiredlevel7; }
            set { m_clothrequiredlevel7 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel8
        {
            get { return m_clothrequiredlevel8; }
            set { m_clothrequiredlevel8 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel9
        {
            get { return m_clothrequiredlevel9; }
            set { m_clothrequiredlevel9 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel10
        {
            get { return m_clothrequiredlevel10; }
            set { m_clothrequiredlevel10 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel1Intensity
        {
            get { return m_clothrequiredlevel1intensity; }
            set { m_clothrequiredlevel1intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel2Intensity
        {
            get { return m_clothrequiredlevel2intensity; }
            set { m_clothrequiredlevel2intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel3Intensity
        {
            get { return m_clothrequiredlevel3intensity; }
            set { m_clothrequiredlevel3intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel4Intensity
        {
            get { return m_clothrequiredlevel4intensity; }
            set { m_clothrequiredlevel4intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel5Intensity
        {
            get { return m_clothrequiredlevel5intensity; }
            set { m_clothrequiredlevel5intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel6Intensity
        {
            get { return m_clothrequiredlevel6intensity; }
            set { m_clothrequiredlevel6intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel7Intensity
        {
            get { return m_clothrequiredlevel7intensity; }
            set { m_clothrequiredlevel7intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel8Intensity
        {
            get { return m_clothrequiredlevel8intensity; }
            set { m_clothrequiredlevel8intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel9Intensity
        {
            get { return m_clothrequiredlevel9intensity; }
            set { m_clothrequiredlevel9intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int ClothRequiredLevel10Intensity
        {
            get { return m_clothrequiredlevel10intensity; }
            set { m_clothrequiredlevel10intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel1
        {
            get { return m_jewelrequiredlevel1; }
            set { m_jewelrequiredlevel1 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel2
        {
            get { return m_jewelrequiredlevel2; }
            set { m_jewelrequiredlevel2 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel3
        {
            get { return m_jewelrequiredlevel3; }
            set { m_jewelrequiredlevel3 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel4
        {
            get { return m_jewelrequiredlevel4; }
            set { m_jewelrequiredlevel4 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel5
        {
            get { return m_jewelrequiredlevel5; }
            set { m_jewelrequiredlevel5 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel6
        {
            get { return m_jewelrequiredlevel6; }
            set { m_jewelrequiredlevel6 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel7
        {
            get { return m_jewelrequiredlevel7; }
            set { m_jewelrequiredlevel7 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel8
        {
            get { return m_jewelrequiredlevel8; }
            set { m_jewelrequiredlevel8 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel9
        {
            get { return m_jewelrequiredlevel9; }
            set { m_jewelrequiredlevel9 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel10
        {
            get { return m_jewelrequiredlevel10; }
            set { m_jewelrequiredlevel10 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel1Intensity
        {
            get { return m_jewelrequiredlevel1intensity; }
            set { m_jewelrequiredlevel1intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel2Intensity
        {
            get { return m_jewelrequiredlevel2intensity; }
            set { m_jewelrequiredlevel2intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel3Intensity
        {
            get { return m_jewelrequiredlevel3intensity; }
            set { m_jewelrequiredlevel3intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel4Intensity
        {
            get { return m_jewelrequiredlevel4intensity; }
            set { m_jewelrequiredlevel4intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel5Intensity
        {
            get { return m_jewelrequiredlevel5intensity; }
            set { m_jewelrequiredlevel5intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel6Intensity
        {
            get { return m_jewelrequiredlevel6intensity; }
            set { m_jewelrequiredlevel6intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel7Intensity
        {
            get { return m_jewelrequiredlevel7intensity; }
            set { m_jewelrequiredlevel7intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel8Intensity
        {
            get { return m_jewelrequiredlevel8intensity; }
            set { m_jewelrequiredlevel8intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel9Intensity
        {
            get { return m_jewelrequiredlevel9intensity; }
            set { m_jewelrequiredlevel9intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int JewelRequiredLevel10Intensity
        {
            get { return m_jewelrequiredlevel10intensity; }
            set { m_jewelrequiredlevel10intensity = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public bool EnabledLevelPets
        {
            get { return m_enabledlevelpets; }
            set { m_enabledlevelpets = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool MountedPetsGainExp
        {
            get { return m_mountedpetsgainexp; }
            set { m_mountedpetsgainexp = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool PetAttackBonus
        {
            get { return m_petattackbonus; }
            set { m_petattackbonus = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool LevelBelowPet
        {
            get { return m_levelbelowpet; }
            set { m_levelbelowpet = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool LoseExpLevelOnDeath
        {
            get { return m_loseexplevelondeath; }
            set { m_loseexplevelondeath = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool LoseStatOnDeath
        {
            get { return m_losestatondeath; }
            set { m_losestatondeath = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int PetStatLossAmount
        {
            get { return m_petstatlossamount; }
            set { m_petstatlossamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool PetLevelSheetPerma
        {
            get { return m_petlevelsheetperma; }
            set { m_petlevelsheetperma = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool PetExpGainFromKill
        {
            get { return m_petexpgainfromkill; }
            set { m_petexpgainfromkill = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxLevelWithScroll
        {
            get { return m_maxlevelwithscroll; }
            set { m_maxlevelwithscroll = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int EndMaxLvl
        {
            get { return m_endmaxlvl; }
            set { m_endmaxlvl = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int SkillsTotal
        {
            get { return m_skillstotal; }
            set { m_skillstotal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxStatPoints
        {
            get { return m_maxstatpoints; }
            set { m_maxstatpoints = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int PetMaxStatPoints
        {
            get { return m_petmaxstatpoints; }
            set { m_petmaxstatpoints = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool NotifyOnPetExpGain
        {
            get { return m_notifyonpetexpgain; }
            set { m_notifyonpetexpgain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool NotifyOnPetLevelUp
        {
            get { return m_notifyonpetlevelup; }
            set { m_notifyonpetlevelup = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool UntamedCreatureLevels
        {
            get { return m_untamedcreaturelevels; }
            set { m_untamedcreaturelevels = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool EmoteOnSpecialAtks
        {
            get { return m_emoteonspecialatks; }
            set { m_emoteonspecialatks = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool EmotesOnAuraBoost
        {
            get { return m_emotesonauraboost; }
            set { m_emotesonauraboost = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool SuperHeal
        {
            get { return m_superheal; }
            set { m_superheal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool PetShouldBeBonded
        {
            get { return m_petshouldbebonded; }
            set { m_petshouldbebonded = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int SuperHealReq
        {
            get { return m_superhealreq; }
            set { m_superhealreq = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public double SuperHealChance
        {
            get { return m_superhealchance; }
            set { m_superhealchance = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool TelePortToTarget
        {
            get { return m_teleporttotarget; }
            set { m_teleporttotarget = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int TelePortToTargetReq
        {
            get { return m_teleporttotargetreq; }
            set { m_teleporttotargetreq = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public double TelePortToTarChance
        {
            get { return m_teleporttotarchance; }
            set { m_teleporttotarchance = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool MassProvokeToAtt
        {
            get { return m_massprovoketoatt; }
            set { m_massprovoketoatt = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int MassProvokeToAttReq
        {
            get { return m_massprovoketoattreq; }
            set { m_massprovoketoattreq = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public double MassProvokeChance
        {
            get { return m_massprovokechance; }
            set { m_massprovokechance = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool MassPeaceArea
        {
            get { return m_masspeacearea; }
            set { m_masspeacearea = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int MassPeaceReq
        {
            get { return m_masspeacereq; }
            set { m_masspeacereq = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public double MassPeaceChance
        {
            get { return m_masspeacechance; }
            set { m_masspeacechance = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool BlessedPower
        {
            get { return m_blessedpower; }
            set { m_blessedpower = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int BlessedPowerReq
        {
            get { return m_blessedpowerreq; }
            set { m_blessedpowerreq = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public double BlessedPowerChance
        {
            get { return m_blessedpowerchance; }
            set { m_blessedpowerchance = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AreaFireBlast
        {
            get { return m_areafireblast; }
            set { m_areafireblast = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int AreaFireBlastReq
        {
            get { return m_areafireblastreq; }
            set { m_areafireblastreq = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public double AreaFireBlastChance
        {
            get { return m_areafireblastchance; }
            set { m_areafireblastchance = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AreaIceBlast
        {
            get { return m_areaiceblast; }
            set { m_areaiceblast = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int AreaIceBlastReq
        {
            get { return m_areaiceblastreq; }
            set { m_areaiceblastreq = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public double AreaIceBlastChance
        {
            get { return m_areaiceblastchance; }
            set { m_areaiceblastchance = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AreaAirBlast
        {
            get { return m_areaairblast; }
            set { m_areaairblast = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int AreaAirBlastReq
        {
            get { return m_areaairblastreq; }
            set { m_areaairblastreq = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public double AreaAirBlastChance
        {
            get { return m_areaairblastchance; }
            set { m_areaairblastchance = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AuraStatBoost
        {
            get { return m_aurastatboost; }
            set { m_aurastatboost = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int AuraStatBoostReq
        {
            get { return m_aurastatboostreq; }
            set { m_aurastatboostreq = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool EnableMountCheck
        {
            get { return m_enablemountcheck; }
            set { m_enablemountcheck = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Beetle
        {
            get { return m_beetle; }
            set { m_beetle = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int DesertOstard
        {
            get { return m_desertostard; }
            set { m_desertostard = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int FireSteed
        {
            get { return m_firesteed; }
            set { m_firesteed = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int ForestOstard
        {
            get { return m_forestostard; }
            set { m_forestostard = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int FrenziedOstard
        {
            get { return m_frenziedostard; }
            set { m_frenziedostard = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int HellSteed
        {
            get { return m_hellsteed; }
            set { m_hellsteed = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Hiryu
        {
            get { return m_hiryu; }
            set { m_hiryu = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Horse
        {
            get { return m_horse; }
            set { m_horse = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Kirin
        {
            get { return m_kirin; }
            set { m_kirin = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int LesserHiryu
        {
            get { return m_lesserhiryu; }
            set { m_lesserhiryu = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int NightMare
        {
            get { return m_nightmare; }
            set { m_nightmare = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Ridablellama
        {
            get { return m_ridablellama; }
            set { m_ridablellama = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Ridgeback
        {
            get { return m_ridgeback; }
            set { m_ridgeback = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int SavageRidgeback
        {
            get { return m_savageridgeback; }
            set { m_savageridgeback = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int ScaledSwampDragon
        {
            get { return m_scaledswampdragon; }
            set { m_scaledswampdragon = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Seahorse
        {
            get { return m_seahorse; }
            set { m_seahorse = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int SilverSteed
        {
            get { return m_silversteed; }
            set { m_silversteed = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int SkeletalMount
        {
            get { return m_skeletalmount; }
            set { m_skeletalmount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Swampdragon
        {
            get { return m_swampdragon; }
            set { m_swampdragon = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Unicorn
        {
            get { return m_unicorn; }
            set { m_unicorn = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Reptalon
        {
            get { return m_reptalon; }
            set { m_reptalon = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Wildtiger
        {
            get { return m_wildtiger; }
            set { m_wildtiger = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Windrunner
        {
            get { return m_windrunner; }
            set { m_windrunner = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lasher
        {
            get { return m_lasher; }
            set { m_lasher = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Eowmu
        {
            get { return m_eowmu; }
            set { m_eowmu = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Dreadwarhorse
        {
            get { return m_dreadwarhorse; }
            set { m_dreadwarhorse = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Cusidhe
        {
            get { return m_cusidhe; }
            set { m_cusidhe = value; InvalidateParentProperties(); }
        }
		
		/* how many skill points awarded per level.
		scenario: if turning level 18, below20 applies*/
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below20
        {
            get { return m_below20; }
            set { m_below20 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below40
        {
            get { return m_below40; }
            set { m_below40 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below60
        {
            get { return m_below60; }
            set { m_below60 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below70
        {
            get { return m_below70; }
            set { m_below70 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below80
        {
            get { return m_below80; }
            set { m_below80 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below90
        {
            get { return m_below90; }
            set { m_below90 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below100
        {
            get { return m_below100; }
            set { m_below100 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below110
        {
            get { return m_below110; }
            set { m_below110 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below120
        {
            get { return m_below120; }
            set { m_below120 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below130
        {
            get { return m_below130; }
            set { m_below130 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below140
        {
            get { return m_below140; }
            set { m_below140 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below150
        {
            get { return m_below150; }
            set { m_below150 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below160
        {
            get { return m_below160; }
            set { m_below160 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below170
        {
            get { return m_below170; }
            set { m_below170 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below180
        {
            get { return m_below180; }
            set { m_below180 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below190
        {
            get { return m_below190; }
            set { m_below190 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below200
        {
            get { return m_below200; }
            set { m_below200 = value; InvalidateParentProperties(); }
        }
		
		/* how many stat points to be awarded per level */

		[CommandProperty(AccessLevel.GameMaster)]
		public int Below20stat
        {
            get { return m_below20stat; }
            set { m_below20stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below40stat
        {
            get { return m_below40stat; }
            set { m_below40stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below60stat
        {
            get { return m_below60stat; }
            set { m_below60stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below70stat
        {
            get { return m_below70stat; }
            set { m_below70stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below80stat
        {
            get { return m_below80stat; }
            set { m_below80stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below90stat
        {
            get { return m_below90stat; }
            set { m_below90stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below100stat
        {
            get { return m_below100stat; }
            set { m_below100stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below110stat
        {
            get { return m_below110stat; }
            set { m_below110stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below120stat
        {
            get { return m_below120stat; }
            set { m_below120stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below130stat
        {
            get { return m_below130stat; }
            set { m_below130stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below140stat
        {
            get { return m_below140stat; }
            set { m_below140stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below150stat
        {
            get { return m_below150stat; }
            set { m_below150stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below160stat
        {
            get { return m_below160stat; }
            set { m_below160stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below170stat
        {
            get { return m_below170stat; }
            set { m_below170stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below180stat
        {
            get { return m_below180stat; }
            set { m_below180stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below190stat
        {
            get { return m_below190stat; }
            set { m_below190stat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Below200stat
        {
            get { return m_below200stat; }
            set { m_below200stat = value; InvalidateParentProperties(); }
        }
		
		
		/* pet stealing system */
		/*	this takes into account the lockpicking skill and if the player can control the pet. (taming)
		additionally if the player has enough stealing skill they get a bonus, the pets stats are not 
		scaled down and the experience and level stay intact if they are successful on the stealing check */
			
		[CommandProperty(AccessLevel.GameMaster)]
		public int MinSkillReqPickSteal
        {
            get { return m_minskillreqpicksteal; }
            set { m_minskillreqpicksteal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool EnablePetpicks
        {
            get { return m_enablepetpicks; }
            set { m_enablepetpicks = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool PreventBondedPetpick
        {
            get { return m_preventbondedpetpick; }
            set { m_preventbondedpetpick = value; InvalidateParentProperties(); }
        }

		/* pet max levels - if using dynamic max levels, 'starmaxlvl' will be ignored! */
		[CommandProperty(AccessLevel.GameMaster)]
		public bool UseDynamicMaxLevels
        {
            get { return m_usedynamicmaxlevels; }
            set { m_usedynamicmaxlevels = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int StartMaxLvl
        {
            get { return m_startmaxlvl; }
            set { m_startmaxlvl = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int StartMaxLvlPets
        {
            get { return m_startmaxlvlpets; }
            set { m_startmaxlvlpets = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow20
        {
            get { return m_Lvbelow20; }
            set { m_Lvbelow20 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow40
        {
            get { return m_Lvbelow40; }
            set { m_Lvbelow40 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow60
        {
            get { return m_Lvbelow60; }
            set { m_Lvbelow60 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow70
        {
            get { return m_Lvbelow70; }
            set { m_Lvbelow70 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow80
        {
            get { return m_Lvbelow80; }
            set { m_Lvbelow80 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow90
        {
            get { return m_Lvbelow90; }
            set { m_Lvbelow90 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow100
        {
            get { return m_Lvbelow100; }
            set { m_Lvbelow100 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow110
        {
            get { return m_Lvbelow110; }
            set { m_Lvbelow110 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow120
        {
            get { return m_Lvbelow120; }
            set { m_Lvbelow120 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow130
        {
            get { return m_Lvbelow130; }
            set { m_Lvbelow130 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow140
        {
            get { return m_Lvbelow140; }
            set { m_Lvbelow140 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow150
        {
            get { return m_Lvbelow150; }
            set { m_Lvbelow150 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow160
        {
            get { return m_Lvbelow160; }
            set { m_Lvbelow160 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow170
        {
            get { return m_Lvbelow170; }
            set { m_Lvbelow170 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow180
        {
            get { return m_Lvbelow180; }
            set { m_Lvbelow180 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow190
        {
            get { return m_Lvbelow190; }
            set { m_Lvbelow190 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Lvbelow200
        {
            get { return m_Lvbelow200; }
            set { m_Lvbelow200 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowerSlotOnLevel
        {
            get { return m_gainfollowerslotonlevel; }
            set { m_gainfollowerslotonlevel = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn20
        {
            get { return m_gainon20; }
            set { m_gainon20 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn30
        {
            get { return m_gainon30; }
            set { m_gainon30 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn40
        {
            get { return m_gainon40; }
            set { m_gainon40 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn50
        {
            get { return m_gainon50; }
            set { m_gainon50 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn60
        {
            get { return m_gainon60; }
            set { m_gainon60 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn70
        {
            get { return m_gainon70; }
            set { m_gainon70 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn80
        {
            get { return m_gainon80; }
            set { m_gainon80 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn90
        {
            get { return m_gainon90; }
            set { m_gainon90 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn100
        {
            get { return m_gainon100; }
            set { m_gainon100 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn110
        {
            get { return m_gainon110; }
            set { m_gainon110 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn120
        {
            get { return m_gainon120; }
            set { m_gainon120 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn130
        {
            get { return m_gainon130; }
            set { m_gainon130 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn140
        {
            get { return m_gainon140; }
            set { m_gainon140 = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn150
        {
            get { return m_gainon150; }
            set { m_gainon150 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn160
        {
            get { return m_gainon160; }
            set { m_gainon160 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn170
        {
            get { return m_gainon170; }
            set { m_gainon170 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn180
        {
            get { return m_gainon180; }
            set { m_gainon180 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn190
        {
            get { return m_gainon190; }
            set { m_gainon190 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GainFollowOn200
        {
            get { return m_gainon200; }
            set { m_gainon200 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL20
        {
            get { return m_gainfollowerslotonlevel20; }
            set { m_gainfollowerslotonlevel20 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL30
        {
            get { return m_gainfollowerslotonlevel30; }
            set { m_gainfollowerslotonlevel30 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL40
        {
            get { return m_gainfollowerslotonlevel40; }
            set { m_gainfollowerslotonlevel40 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL50
        {
            get { return m_gainfollowerslotonlevel50; }
            set { m_gainfollowerslotonlevel50 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL60
        {
            get { return m_gainfollowerslotonlevel60; }
            set { m_gainfollowerslotonlevel60 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL70
        {
            get { return m_gainfollowerslotonlevel70; }
            set { m_gainfollowerslotonlevel70 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL80
        {
            get { return m_gainfollowerslotonlevel80; }
            set { m_gainfollowerslotonlevel80 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL90
        {
            get { return m_gainfollowerslotonlevel90; }
            set { m_gainfollowerslotonlevel90 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL100
        {
            get { return m_gainfollowerslotonlevel100; }
            set { m_gainfollowerslotonlevel100 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL110
        {
            get { return m_gainfollowerslotonlevel110; }
            set { m_gainfollowerslotonlevel110 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL120
        {
            get { return m_gainfollowerslotonlevel120; }
            set { m_gainfollowerslotonlevel120 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL130
        {
            get { return m_gainfollowerslotonlevel130; }
            set { m_gainfollowerslotonlevel130 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL140
        {
            get { return m_gainfollowerslotonlevel140; }
            set { m_gainfollowerslotonlevel140 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL150
        {
            get { return m_gainfollowerslotonlevel150; }
            set { m_gainfollowerslotonlevel150 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL160
        {
            get { return m_gainfollowerslotonlevel160; }
            set { m_gainfollowerslotonlevel160 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL170
        {
            get { return m_gainfollowerslotonlevel170; }
            set { m_gainfollowerslotonlevel170 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL180
        {
            get { return m_gainfollowerslotonlevel180; }
            set { m_gainfollowerslotonlevel180 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL190
        {
            get { return m_gainfollowerslotonlevel190; }
            set { m_gainfollowerslotonlevel190 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int GainFollowerSlotonLeveL200
        {
            get { return m_gainfollowerslotonlevel200; }
            set { m_gainfollowerslotonlevel200 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Enableexpfromskills
        {
            get { return m_enableexpfromskills; }
            set { m_enableexpfromskills = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Begginggain
        {
            get { return m_begginggain; }
            set { m_begginggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Begginggainamount
        {
            get { return m_begginggainamount; }
            set { m_begginggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Campinggain
        {
            get { return m_campinggain; }
            set { m_campinggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Campinggainamount
        {
            get { return m_campinggainamount; }
            set { m_campinggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Forensicsgain
        {
            get { return m_forensicsgain; }
            set { m_forensicsgain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Forensicsgainamount
        {
            get { return m_forensicsgainamount; }
            set { m_forensicsgainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Itemidgain
        {
            get { return m_itemidgain; }
            set { m_itemidgain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Itemidgainamount
        {
            get { return m_itemidgainamount; }
            set { m_itemidgainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Tasteidgain
        {
            get { return m_tasteidgain; }
            set { m_tasteidgain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Tasteidgainamount
        {
            get { return m_tasteidgainamount; }
            set { m_tasteidgainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Imbuinggain
        {
            get { return m_imbuinggain; }
            set { m_imbuinggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Imbuinggainamount
        {
            get { return m_imbuinggainamount; }
            set { m_imbuinggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Evalintgain
        {
            get { return m_evalintgain; }
            set { m_evalintgain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Evalintgainamount
        {
            get { return m_evalintgainamount; }
            set { m_evalintgainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Spiritspeakgain
        {
            get { return m_spiritspeakgain; }
            set { m_spiritspeakgain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Spiritspeakgainamount
        {
            get { return m_spiritspeakgainamount; }
            set { m_spiritspeakgainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Fishinggain
        {
            get { return m_fishinggain; }
            set { m_fishinggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Fishinggainamount
        {
            get { return m_fishinggainamount; }
            set { m_fishinggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Herdinggain
        {
            get { return m_herdinggain; }
            set { m_herdinggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Herdinggainamount
        {
            get { return m_herdinggainamount; }
            set { m_herdinggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Trackinggain
        {
            get { return m_trackinggain; }
            set { m_trackinggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Trackinggainamount
        {
            get { return m_trackinggainamount; }
            set { m_trackinggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Hidinggain
        {
            get { return m_hidinggain; }
            set { m_hidinggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Hidinggainamount
        {
            get { return m_hidinggainamount; }
            set { m_hidinggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Poisoninggain
        {
            get { return m_poisoninggain; }
            set { m_poisoninggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Poisoninggainamount
        {
            get { return m_poisoninggainamount; }
            set { m_poisoninggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Removetrapgain
        {
            get { return m_removetrapgain; }
            set { m_removetrapgain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Removetrapgainamount
        {
            get { return m_removetrapgainamount; }
            set { m_removetrapgainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Stealinggain
        {
            get { return m_stealinggain; }
            set { m_stealinggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Stealinggainamount
        {
            get { return m_stealinggainamount; }
            set { m_stealinggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Discordancegain
        {
            get { return m_discordancegain; }
            set { m_discordancegain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Discordancegainamount
        {
            get { return m_discordancegainamount; }
            set { m_discordancegainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Peacemakinggain
        {
            get { return m_peacemakinggain; }
            set { m_peacemakinggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Peacemakinggainamount
        {
            get { return m_peacemakinggainamount; }
            set { m_peacemakinggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Provocationgain
        {
            get { return m_provocationgain; }
            set { m_provocationgain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Provocationgainamount
        {
            get { return m_provocationgainamount; }
            set { m_provocationgainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Anatomygain
        {
            get { return m_anatomygain; }
            set { m_anatomygain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Anatomygainamount
        {
            get { return m_anatomygainamount; }
            set { m_anatomygainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Armsloregain
        {
            get { return m_armsloregain; }
            set { m_armsloregain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Armsloregainamount
        {
            get { return m_armsloregainamount; }
            set { m_armsloregainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Animalloregain
        {
            get { return m_animalloregain; }
            set { m_animalloregain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Animalloregainamount
        {
            get { return m_animalloregainamount; }
            set { m_animalloregainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Meditationgain
        {
            get { return m_meditationgain; }
            set { m_meditationgain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Meditationgainamount
        {
            get { return m_meditationgainamount; }
            set { m_meditationgainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Cartographygain
        {
            get { return m_cartographygain; }
            set { m_cartographygain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Cartographygainamount
        {
            get { return m_cartographygainamount; }
            set { m_cartographygainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Detecthiddengain
        {
            get { return m_detecthiddengain; }
            set { m_detecthiddengain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Detecthiddengainamount
        {
            get { return m_detecthiddengainamount; }
            set { m_detecthiddengainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Animaltaminggain
        {
            get { return m_animaltaminggain; }
            set { m_animaltaminggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Animaltaminggainamount
        {
            get { return m_animaltaminggainamount; }
            set { m_animaltaminggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Blacksmithgain
        {
            get { return m_blacksmithgain; }
            set { m_blacksmithgain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Blacksmithgainamount
        {
            get { return m_blacksmithgainamount; }
            set { m_blacksmithgainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Carpentrygain
        {
            get { return m_carpentrygain; }
            set { m_carpentrygain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Carpentrygainamount
        {
            get { return m_carpentrygainamount; }
            set { m_carpentrygainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Alchemygain
        {
            get { return m_alchemygain; }
            set { m_alchemygain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Alchemygainamount
        {
            get { return m_alchemygainamount; }
            set { m_alchemygainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Fletchinggain
        {
            get { return m_fletchinggain; }
            set { m_fletchinggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Fletchinggainamount
        {
            get { return m_fletchinggainamount; }
            set { m_fletchinggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Cookinggain
        {
            get { return m_cookinggain; }
            set { m_cookinggain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Cookinggainamount
        {
            get { return m_cookinggainamount; }
            set { m_cookinggainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Inscribegain
        {
            get { return m_inscribegain; }
            set { m_inscribegain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Inscribegainamount
        {
            get { return m_inscribegainamount; }
            set { m_inscribegainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Tailoringgain
        {
            get { return m_tailoringgain; }
            set { m_tailoringgain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Tailoringgainamount
        {
            get { return m_tailoringgainamount; }
            set { m_tailoringgainamount = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Tinkeringgain
        {
            get { return m_tinkeringgain; }
            set { m_tinkeringgain = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Tinkeringgainamount
        {
            get { return m_tinkeringgainamount; }
            set { m_tinkeringgainamount = value; InvalidateParentProperties(); }
        }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Miscelaneous
        {
            get { return m_miscelaneous; }
            set { m_miscelaneous = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Combat
        {
            get { return m_combat; }
            set { m_combat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Tradeskills
        {
            get { return m_tradeskills; }
            set { m_tradeskills = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Magic
        {
            get { return m_magic; }
            set { m_magic = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Wilderness
        {
            get { return m_wilderness; }
            set { m_wilderness = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Thieving
        {
            get { return m_thieving; }
            set { m_thieving = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Bard
        {
            get { return m_bard; }
            set { m_bard = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Imbuing
        {
            get { return m_imbuing; }
            set { m_imbuing = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Throwing
        {
            get { return m_throwing; }
            set { m_throwing = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Mysticism
        {
            get { return m_mysticism; }
            set { m_mysticism = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Spellweaving
        {
            get { return m_spellweaving; }
            set { m_spellweaving = value; InvalidateParentProperties(); }
        }	
		
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Bagsystemmaintoggle
        {
            get { return m_bagsystemmaintoggle; }
            set { m_bagsystemmaintoggle = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Preventbagdrop	
        {
            get { return m_preventbagdrop; }
            set { m_preventbagdrop = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Bagblessed	
        {
            get { return m_bagblessed; }
            set { m_bagblessed = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Levelgroup1
        {
            get { return m_levelgroup1; }
            set { m_levelgroup1 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Level1groupbagownership
        {
            get { return m_level1groupbagownership; }
            set { m_level1groupbagownership = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup1reqLevel	
        {
            get { return m_levelgroup1reqlevel; }
            set { m_levelgroup1reqlevel = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup1maxitems	
        {
            get { return m_levelgroup1maxitems; }
            set { m_levelgroup1maxitems = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup1reducetotal
        {
            get { return m_levelgroup1reducetotal; }
            set { m_levelgroup1reducetotal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Levelgroup1msg	
        {
            get { return m_levelgroup1msg; }
            set { m_levelgroup1msg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level1groupownermsg
        {
            get { return m_level1groupownermsg; }
            set { m_level1groupownermsg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level1groupownernow
        {
            get { return m_level1groupownernow; }
            set { m_level1groupownernow = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Levelgroup2	
        {
            get { return m_levelgroup2; }
            set { m_levelgroup2 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Level2groupbagownership
        {
            get { return m_level2groupbagownership; }
            set { m_level2groupbagownership = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup2reqLevel	
        {
            get { return m_levelgroup2reqlevel; }
            set { m_levelgroup2reqlevel = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup2maxitems	
        {
            get { return m_levelgroup2maxitems; }
            set { m_levelgroup2maxitems = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup2reducetotal	
        {
            get { return m_levelgroup2reducetotal; }
            set { m_levelgroup2reducetotal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Levelgroup2msg	
        {
            get { return m_levelgroup2msg; }
            set { m_levelgroup2msg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level2groupownermsg
        {
            get { return m_level2groupownermsg; }
            set { m_level2groupownermsg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level2groupownernow
        {
            get { return m_level2groupownernow; }
            set { m_level2groupownernow = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Levelgroup3	
        {
            get { return m_levelgroup3; }
            set { m_levelgroup3 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Level3groupbagownership
        {
            get { return m_level3groupbagownership; }
            set { m_level3groupbagownership = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup3reqLevel
        {
            get { return m_levelgroup3reqlevel; }
            set { m_levelgroup3reqlevel = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup3maxitems
        {
            get { return m_levelgroup3maxitems; }
            set { m_levelgroup3maxitems = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup3reducetotal
        {
            get { return m_levelgroup3reducetotal; }
            set { m_levelgroup3reducetotal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Levelgroup3msg	
        {
            get { return m_levelgroup3msg; }
            set { m_levelgroup3msg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level3groupownermsg	
        {
            get { return m_level3groupownermsg; }
            set { m_level3groupownermsg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level3groupownernow
        {
            get { return m_level3groupownernow; }
            set { m_level3groupownernow = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Levelgroup4	
        {
            get { return m_levelgroup4; }
            set { m_levelgroup4 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Level4groupbagownership	
        {
            get { return m_level4groupbagownership; }
            set { m_level4groupbagownership = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup4reqLevel
        {
            get { return m_levelgroup4reqlevel; }
            set { m_levelgroup4reqlevel = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup4maxitems		
        {
            get { return m_levelgroup4maxitems; }
            set { m_levelgroup4maxitems = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup4reducetotal	
        {
            get { return m_levelgroup4reducetotal; }
            set { m_levelgroup4reducetotal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Levelgroup4msg
        {
            get { return m_levelgroup4msg; }
            set { m_levelgroup4msg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level4groupownermsg
        {
            get { return m_level4groupownermsg; }
            set { m_level4groupownermsg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level4groupownernow
        {
            get { return m_level4groupownernow; }
            set { m_level4groupownernow = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Levelgroup5		
        {
            get { return m_levelgroup5; }
            set { m_levelgroup5 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Level5groupbagownership
        {
            get { return m_level5groupbagownership; }
            set { m_level5groupbagownership = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup5reqLevel
        {
            get { return m_levelgroup5reqlevel; }
            set { m_levelgroup5reqlevel = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup5maxitems
        {
            get { return m_levelgroup5maxitems; }
            set { m_levelgroup5maxitems = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup5reducetotal
        {
            get { return m_levelgroup5reducetotal; }
            set { m_levelgroup5reducetotal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Levelgroup5msg	
        {
            get { return m_levelgroup5msg; }
            set { m_levelgroup5msg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level5groupownermsg
        {
            get { return m_level5groupownermsg; }
            set { m_level5groupownermsg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level5groupownernow	
        {
            get { return m_level5groupownernow; }
            set { m_level5groupownernow = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Levelgroup6
        {
            get { return m_levelgroup6; }
            set { m_levelgroup6 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Level6groupbagownership
        {
            get { return m_level6groupbagownership; }
            set { m_level6groupbagownership = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup6reqLevel
        {
            get { return m_levelgroup6reqlevel; }
            set { m_levelgroup6reqlevel = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup6maxitems	
        {
            get { return m_levelgroup6maxitems; }
            set { m_levelgroup6maxitems = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup6reducetotal	
        {
            get { return m_levelgroup6reducetotal; }
            set { m_levelgroup6reducetotal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Levelgroup6msg	
        {
            get { return m_levelgroup6msg; }
            set { m_levelgroup6msg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level6groupownermsg		
        {
            get { return m_level6groupownermsg; }
            set { m_level6groupownermsg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level6groupownernow
        {
            get { return m_level6groupownernow; }
            set { m_level6groupownernow = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Levelgroup7	
        {
            get { return m_levelgroup7; }
            set { m_levelgroup7 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Level7groupbagownership	
        {
            get { return m_level7groupbagownership; }
            set { m_level7groupbagownership = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup7reqLevel	
        {
            get { return m_levelgroup7reqlevel; }
            set { m_levelgroup7reqlevel = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup7maxitems	
        {
            get { return m_levelgroup7maxitems; }
            set { m_levelgroup7maxitems = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup7reducetotal	
        {
            get { return m_levelgroup7reducetotal; }
            set { m_levelgroup7reducetotal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Levelgroup7msg	
        {
            get { return m_levelgroup7msg; }
            set { m_levelgroup7msg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level7groupownermsg	
        {
            get { return m_level7groupownermsg; }
            set { m_level7groupownermsg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level7groupownernow
        {
            get { return m_level7groupownernow; }
            set { m_level7groupownernow = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Levelgroup8	
        {
            get { return m_levelgroup8; }
            set { m_levelgroup8 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Level8groupbagownership	
        {
            get { return m_level8groupbagownership; }
            set { m_level8groupbagownership = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup8reqLevel		
        {
            get { return m_levelgroup8reqlevel; }
            set { m_levelgroup8reqlevel = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup8maxitems
        {
            get { return m_levelgroup8maxitems; }
            set { m_levelgroup8maxitems = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup8reducetotal	
        {
            get { return m_levelgroup8reducetotal; }
            set { m_levelgroup8reducetotal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Levelgroup8msg
        {
            get { return m_levelgroup8msg; }
            set { m_levelgroup8msg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level8groupownermsg
        {
            get { return m_level8groupownermsg; }
            set { m_level8groupownermsg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level8groupownernow
        {
            get { return m_level8groupownernow; }
            set { m_level8groupownernow = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Levelgroup9
        {
            get { return m_levelgroup9; }
            set { m_levelgroup9 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Level9groupbagownership	
        {
            get { return m_level9groupbagownership; }
            set { m_level9groupbagownership = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup9reqLevel	
        {
            get { return m_levelgroup9reqlevel; }
            set { m_levelgroup9reqlevel = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup9maxitems
        {
            get { return m_levelgroup9maxitems; }
            set { m_levelgroup9maxitems = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup9reducetotal	
        {
            get { return m_levelgroup9reducetotal; }
            set { m_levelgroup9reducetotal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Levelgroup9msg		
        {
            get { return m_levelgroup9msg; }
            set { m_levelgroup9msg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level9groupownermsg	
        {
            get { return m_level9groupownermsg; }
            set { m_level9groupownermsg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level9groupownernow	
        {
            get { return m_level9groupownernow; }
            set { m_level9groupownernow = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Levelgroup10	
        {
            get { return m_levelgroup10; }
            set { m_levelgroup10 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Level10groupbagownership
        {
            get { return m_level10groupbagownership; }
            set { m_level10groupbagownership = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup10reqLevel	
        {
            get { return m_levelgroup10reqlevel; }
            set { m_levelgroup10reqlevel = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup10maxitems	
        {
            get { return m_levelgroup10maxitems; }
            set { m_levelgroup10maxitems = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Levelgroup10reducetotal	
        {
            get { return m_levelgroup10reducetotal; }
            set { m_levelgroup10reducetotal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Levelgroup10msg	
        {
            get { return m_levelgroup10msg; }
            set { m_levelgroup10msg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level10groupownermsg
        {
            get { return m_level10groupownermsg; }
            set { m_level10groupownermsg = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Level10groupowner
        {
            get { return m_level10groupownernow; }
            set { m_level10groupownernow = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool ActivateDynamicLevelSystem
        {
            get { return m_activatedynamiclevelsystem; }
            set { m_activatedynamiclevelsystem = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string NameOfBattleRatingStat
        {
            get { return m_nameofbattleratingstat; }
            set { m_nameofbattleratingstat = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string RequiredLevelMouseOver
        {
            get { return m_requiredlevelmouseover; }
            set { m_requiredlevelmouseover = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int SkillCoinCap
        {
            get { return m_skillcoincap; }
            set { m_skillcoincap = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int SkillCoinValue
        {
            get { return m_skillcoinvalue; }
            set { m_skillcoinvalue = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int ExpCoinValue
        {
            get { return m_expcoinvalue; }
            set { m_expcoinvalue = value; InvalidateParentProperties(); }
        }	
		[CommandProperty(AccessLevel.GameMaster)]
		public int ExpPowerAmount
        {
            get { return m_exppoweramount; }
            set { m_exppoweramount = value; InvalidateParentProperties(); }
        }			
		[CommandProperty(AccessLevel.GameMaster)]
		public int PowerHourTime
		{
			get { return m_powerhourtime; }
			set { m_powerhourtime = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int PartyExpShareRange
		{
			get { return m_partyexpsharerange; }
			set { m_partyexpsharerange = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public bool MaxLevel
		{
			get { return m_maxlevel; }
			set { m_maxlevel = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow20
		{
			get { return m_petbelow20; }
			set { m_petbelow20 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow40
		{
			get { return m_petbelow40; }
			set { m_petbelow40 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow60
		{
			get { return m_petbelow60; }
			set { m_petbelow60 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow70
		{
			get { return m_petbelow70; }
			set { m_petbelow70 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow80
		{
			get { return m_petbelow80; }
			set { m_petbelow80 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow90
		{
			get { return m_petbelow90; }
			set { m_petbelow90 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow100
		{
			get { return m_petbelow100; }
			set { m_petbelow100 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow110
		{
			get { return m_petbelow110; }
			set { m_petbelow110 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow120
		{
			get { return m_petbelow120; }
			set { m_petbelow120 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow130
		{
			get { return m_petbelow130; }
			set { m_petbelow130 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow140
		{
			get { return m_petbelow140; }
			set { m_petbelow140 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow150
		{
			get { return m_petbelow150; }
			set { m_petbelow150 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow160
		{
			get { return m_petbelow160; }
			set { m_petbelow160 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow170
		{
			get { return m_petbelow170; }
			set { m_petbelow170 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow180
		{
			get { return m_petbelow180; }
			set { m_petbelow180 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow190
		{
			get { return m_petbelow190; }
			set { m_petbelow190 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow200
		{
			get { return m_petbelow200; }
			set { m_petbelow200 = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow20stat
		{
			get { return m_petbelow20stat; }
			set { m_petbelow20stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow40stat
		{
			get { return m_petbelow40stat; }
			set { m_petbelow40stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow60stat
		{
			get { return m_petbelow60stat; }
			set { m_petbelow60stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow70stat
		{
			get { return m_petbelow70stat; }
			set { m_petbelow70stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow80stat
		{
			get { return m_petbelow80stat; }
			set { m_petbelow80stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow90stat
		{
			get { return m_petbelow90stat; }
			set { m_petbelow90stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow100stat
		{
			get { return m_petbelow100stat; }
			set { m_petbelow100stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow110stat
		{
			get { return m_petbelow110stat; }
			set { m_petbelow110stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow120stat
		{
			get { return m_petbelow120stat; }
			set { m_petbelow120stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow130stat
		{
			get { return m_petbelow130stat; }
			set { m_petbelow130stat = value; InvalidateParentProperties(); }
		}
		public int Petbelow140stat
		{
			get { return m_petbelow140stat; }
			set { m_petbelow140stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow150stat
		{
			get { return m_petbelow150stat; }
			set { m_petbelow150stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow160stat
		{
			get { return m_petbelow160stat; }
			set { m_petbelow160stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow170stat
		{
			get { return m_petbelow170stat; }
			set { m_petbelow170stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow180stat
		{
			get { return m_petbelow180stat; }
			set { m_petbelow180stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow190stat
		{
			get { return m_petbelow190stat; }
			set { m_petbelow190stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Petbelow200stat
		{
			get { return m_petbelow200stat; }
			set { m_petbelow200stat = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Pethappysystem
		{
			get { return m_pethappysystem; }
			set { m_pethappysystem = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Levelpacktrait
		{
			get { return m_levelpacktrait; }
			set { m_levelpacktrait = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public bool PetReclaimChance
		{
			get { return m_petreclaimchance; }
			set { m_petreclaimchance = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public bool NewStartingLocation
		{
			get { return m_newstartinglocation; }
			set { m_newstartinglocation = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int X_variable
		{
			get { return m_x_variable; }
			set { m_x_variable = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Y_variable
		{
			get { return m_y_variable; }
			set { m_y_variable = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Z_variable
		{
			get { return m_z_variable; }
			set { m_z_variable = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string Maplocation
        {
            get { return m_maplocation; }
            set { m_maplocation = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Guildnamestart
        {
            get { return m_guildnamestart; }
            set { m_guildnamestart = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool ForceNewPlayerIntoGuild
        {
            get { return m_forcenewplayerintoguild; }
            set { m_forcenewplayerintoguild = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AddToBackpackOnAttach
        {
            get { return m_addtobackpackonattach; }
            set { m_addtobackpackonattach = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Startitem1
        {
            get { return m_startitem1; }
            set { m_startitem1 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Startitem2
        {
            get { return m_startitem2; }
            set { m_startitem2 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Startitem3
        {
            get { return m_startitem3; }
            set { m_startitem3 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Startitem4
        {
            get { return m_startitem4; }
            set { m_startitem4 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Startitem5
        {
            get { return m_startitem5; }
            set { m_startitem5 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Startitem6
        {
            get { return m_startitem6; }
            set { m_startitem6 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Startitem7
        {
            get { return m_startitem7; }
            set { m_startitem7 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Startitem8
        {
            get { return m_startitem8; }
            set { m_startitem8 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Startitem9
        {
            get { return m_startitem9; }
            set { m_startitem9 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public string Startitem10
        {
            get { return m_startitem10; }
            set { m_startitem10 = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Forcestartingstats
        {
            get { return m_forcestartingstats; }
            set { m_forcestartingstats = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Forcestartingstatsstr
		{
			get { return m_forcestartingstatsstr; }
			set { m_forcestartingstatsstr = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Forcestartingstatsdex
		{
			get { return m_forcestartingstatsdex; }
			set { m_forcestartingstatsdex = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Forcestartingstatsint
		{
			get { return m_forcestartingstatsint; }
			set { m_forcestartingstatsint = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_startingstrcap
        {
            get { return m_autoactivate_startingstrcap; }
            set { m_autoactivate_startingstrcap = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Startingstrcapvar
		{
			get { return m_startingstrcapvar; }
			set { m_startingstrcapvar = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Startingstrcapmaxvar
		{
			get { return m_startingstrcapmaxvar; }
			set { m_startingstrcapmaxvar = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_startingdexcap
        {
            get { return m_autoactivate_startingdexcap; }
            set { m_autoactivate_startingdexcap = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Startingdexcapvar
		{
			get { return m_startingdexcapvar; }
			set { m_startingdexcapvar = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Startingdexcapmaxvar
		{
			get { return m_startingdexcapmaxvar; }
			set { m_startingdexcapmaxvar = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_startingintcap
        {
            get { return m_autoactivate_startingintcap; }
            set { m_autoactivate_startingintcap = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Startingintcapvar
		{
			get { return m_startingintcapvar; }
			set { m_startingintcapvar = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int Startingintcapmaxvar
		{
			get { return m_startingintcapmaxvar; }
			set { m_startingintcapmaxvar = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_startingtotalstatcap
        {
            get { return m_autoactivate_startingtotalstatcap; }
            set { m_autoactivate_startingtotalstatcap = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Autoactivate_startingtotalstatcapvar
		{
			get { return m_autoactivate_startingtotalstatcapvar; }
			set { m_autoactivate_startingtotalstatcapvar = value; InvalidateParentProperties(); }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_gemmining
        {
            get { return m_autoactivate_gemmining; }
            set { m_autoactivate_gemmining = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_basketweaving
        {
            get { return m_autoactivate_basketweaving; }
            set { m_autoactivate_basketweaving = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_canbuycarpets
        {
            get { return m_autoactivate_canbuycarpets; }
            set { m_autoactivate_canbuycarpets = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_acceptguildinvites
        {
            get { return m_autoactivate_acceptguildinvites; }
            set { m_autoactivate_acceptguildinvites = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_glassblowing
        {
            get { return m_autoactivate_glassblowing; }
            set { m_autoactivate_glassblowing = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_libraryfriend
        {
            get { return m_autoactivate_libraryfriend; }
            set { m_autoactivate_libraryfriend = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_masonry
        {
            get { return m_autoactivate_masonry; }
            set { m_autoactivate_masonry = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_sandmining
        {
            get { return m_autoactivate_sandmining; }
            set { m_autoactivate_sandmining = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_stonemining
        {
            get { return m_autoactivate_stonemining; }
            set { m_autoactivate_stonemining = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_spellweaving
        {
            get { return m_autoactivate_spellweaving; }
            set { m_autoactivate_spellweaving = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_mechanicallife
        {
            get { return m_autoactivate_mechanicallife; }
            set { m_autoactivate_mechanicallife = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_disabledpvpwarning
        {
            get { return m_autoactivate_disabledpvpwarning; }
            set { m_autoactivate_disabledpvpwarning = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_isyoung
        {
            get { return m_autoactivate_isyoung; }
            set { m_autoactivate_isyoung = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_cantwalk
        {
            get { return m_autoactivate_cantwalk; }
            set { m_autoactivate_cantwalk = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_maxfollowslots
        {
            get { return m_autoactivate_maxfollowslots; }
            set { m_autoactivate_maxfollowslots = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Autoactivate_maxfollowslotstotal
        {
            get { return m_autoactivate_maxfollowslotstotal; }
            set { m_autoactivate_maxfollowslotstotal = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Autoactivate_skillscap
        {
            get { return m_autoactivate_skillscap; }
            set { m_autoactivate_skillscap = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Autoactivate_skillscapvar
        {
            get { return m_autoactivate_skillscapvar; }
            set { m_autoactivate_skillscapvar = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool MapBoolTrammel
        {
            get { return m_mapbooltrammel; }
            set { m_mapbooltrammel = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool MapBoolFelucca
        {
            get { return m_mapboolfelucca; }
            set { m_mapboolfelucca = value; InvalidateParentProperties(); }
        }
		[CommandProperty(AccessLevel.GameMaster)]
		public bool MapBoolMalas
        {
            get { return m_mapboolmalas; }
            set { m_mapboolmalas = value; InvalidateParentProperties(); }
        }

        [Attachable]
        public LevelControlSys()
        {
        }

        [Attachable]
        public LevelControlSys(int level)
        {
        }
		
        public LevelControlSys(ASerial serial)
            : base(serial)
        {
        }

		public override void OnAttach()
		{
			base.OnAttach();
//			if(AttachedTo is PlayerMobile)
//			{
//				return;
//			}
//			else
//				Delete();
		}
		public override void OnDelete()
		{
			base.OnDelete();
			if(AttachedTo is PlayerMobile)
			{
				InvalidateParentProperties();
			}
		}
		
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write( (int) 0 );
			// version 
			
			writer.Write((bool)m_playerlevels);
			writer.Write((bool)m_equipmentlevelsystem);
			writer.Write((bool)m_weaponlevels);
			writer.Write((bool)m_armorlevels);
			writer.Write((bool)m_jewellevels);
			writer.Write((bool)m_clothinglevels);
			writer.Write((bool)m_gainexpfrombods);
			writer.Write((bool)m_disableskillgain);
			writer.Write((bool)m_levelbelowtoon);
			writer.Write((bool)m_showpaperdolllevel);
			writer.Write((bool)m_petkillgivesexp);
			writer.Write((bool)m_craftgivesexp);
			writer.Write((bool)m_advancedskillexp);
			writer.Write((bool)m_tablesadvancedexp);
			writer.Write((bool)m_staffhaslevel);
			writer.Write((bool)m_bonusstatonlvl);
			writer.Write((bool)m_refreshonlevel);
			writer.Write((bool)m_refreshonlevelpet);
			writer.Write((bool)m_levelsheetperma);
			writer.Write((bool)m_showvendorlevels);
			writer.Write((bool)m_discountfromvendors);
			writer.Write((bool)m_partyexpshare);
			writer.Write((bool)m_partyexpsharesplit);
			writer.Write((bool)m_levelstatresetbutton);
			writer.Write((bool)m_levelskillresetbutton);
			writer.Write((bool)m_miscelaneous);
			writer.Write((bool)m_combat);
			writer.Write((bool)m_tradeskills);
			writer.Write((bool)m_magic);
			writer.Write((bool)m_wilderness);
			writer.Write((bool)m_thieving);
			writer.Write((bool)m_bard);
			writer.Write((bool)m_imbuing);
			writer.Write((bool)m_throwing);
			writer.Write((bool)m_mysticism);
			writer.Write((bool)m_spellweaving);
			writer.Write((bool)m_activatedynamiclevelsystem);
			writer.Write((bool)m_enabledlevelpets);
			writer.Write((bool)m_mountedpetsgainexp);
			writer.Write((bool)m_petattackbonus);
			writer.Write((bool)m_levelbelowpet);
			writer.Write((bool)m_loseexplevelondeath);
			writer.Write((bool)m_losestatondeath);
			writer.Write((bool)m_petlevelsheetperma);
			writer.Write((bool)m_petexpgainfromkill);
			writer.Write((bool)m_notifyonpetexpgain);
			writer.Write((bool)m_notifyonpetlevelup);
			writer.Write((bool)m_untamedcreaturelevels);
			writer.Write((bool)m_emoteonspecialatks);
			writer.Write((bool)m_emotesonauraboost);
			writer.Write((bool)m_superheal);
			writer.Write((bool)m_petshouldbebonded);
			writer.Write((bool)m_teleporttotarget);
			writer.Write((bool)m_massprovoketoatt);
			writer.Write((bool)m_masspeacearea);
			writer.Write((bool)m_blessedpower);
			writer.Write((bool)m_areafireblast);
			writer.Write((bool)m_areaiceblast);
			writer.Write((bool)m_areaairblast);
			writer.Write((bool)m_aurastatboost);
			writer.Write((bool)m_enablemountcheck);
			writer.Write((bool)m_enablepetpicks);
			writer.Write((bool)m_preventbondedpetpick);
			writer.Write((bool)m_usedynamicmaxlevels);
			writer.Write((bool)m_gainfollowerslotonlevel);
			writer.Write((bool)m_gainon20);
			writer.Write((bool)m_gainon30);
			writer.Write((bool)m_gainon40);
			writer.Write((bool)m_gainon50);
			writer.Write((bool)m_gainon60);
			writer.Write((bool)m_gainon70);
			writer.Write((bool)m_gainon80);
			writer.Write((bool)m_gainon90);
			writer.Write((bool)m_gainon100);
			writer.Write((bool)m_gainon110);
			writer.Write((bool)m_gainon120);
			writer.Write((bool)m_gainon130);
			writer.Write((bool)m_gainon140);
			writer.Write((bool)m_gainon150);
			writer.Write((bool)m_gainon160);
			writer.Write((bool)m_gainon170);
			writer.Write((bool)m_gainon180);
			writer.Write((bool)m_gainon190);
			writer.Write((bool)m_gainon200);
			writer.Write((bool)m_enableexpfromskills);
			writer.Write((bool)m_begginggain);
			writer.Write((bool)m_campinggain);
			writer.Write((bool)m_forensicsgain);
			writer.Write((bool)m_itemidgain);
			writer.Write((bool)m_tasteidgain);
			writer.Write((bool)m_imbuinggain);
			writer.Write((bool)m_evalintgain);
			writer.Write((bool)m_spiritspeakgain);
			writer.Write((bool)m_fishinggain);
			writer.Write((bool)m_herdinggain);
			writer.Write((bool)m_trackinggain);
			writer.Write((bool)m_hidinggain);
			writer.Write((bool)m_poisoninggain);
			writer.Write((bool)m_removetrapgain);
			writer.Write((bool)m_stealinggain);
			writer.Write((bool)m_discordancegain);
			writer.Write((bool)m_peacemakinggain);
			writer.Write((bool)m_provocationgain);
			writer.Write((bool)m_anatomygain);
			writer.Write((bool)m_armsloregain);
			writer.Write((bool)m_animalloregain);
			writer.Write((bool)m_meditationgain);
			writer.Write((bool)m_cartographygain);
			writer.Write((bool)m_detecthiddengain);
			writer.Write((bool)m_animaltaminggain);
			writer.Write((bool)m_blacksmithgain);
			writer.Write((bool)m_carpentrygain);
			writer.Write((bool)m_alchemygain);
			writer.Write((bool)m_fletchinggain);
			writer.Write((bool)m_cookinggain);
			writer.Write((bool)m_inscribegain);
			writer.Write((bool)m_tailoringgain);
			writer.Write((bool)m_tinkeringgain);
			writer.Write((bool)m_bagsystemmaintoggle);
			writer.Write((bool)m_preventbagdrop);
			writer.Write((bool)m_bagblessed);
			writer.Write((bool)m_levelgroup1);
			writer.Write((bool)m_level1groupbagownership);
			writer.Write((bool)m_levelgroup2);
			writer.Write((bool)m_level2groupbagownership);
			writer.Write((bool)m_levelgroup3);
			writer.Write((bool)m_level3groupbagownership);
			writer.Write((bool)m_levelgroup4);
			writer.Write((bool)m_level4groupbagownership);
			writer.Write((bool)m_levelgroup5);
			writer.Write((bool)m_level5groupbagownership);
			writer.Write((bool)m_levelgroup6);
			writer.Write((bool)m_level6groupbagownership);
			writer.Write((bool)m_levelgroup7);
			writer.Write((bool)m_level7groupbagownership);
			writer.Write((bool)m_levelgroup8);
			writer.Write((bool)m_level8groupbagownership);
			writer.Write((bool)m_levelgroup9);
			writer.Write((bool)m_level9groupbagownership);
			writer.Write((bool)m_levelgroup10);
			writer.Write((bool)m_level10groupbagownership);
			writer.Write((bool)m_maxlevel);
			writer.Write((int) m_startmaxlvl);
			writer.Write((int) m_startmaxlvlpets);
			writer.Write((int) m_endmaxlvl);
			writer.Write((int) m_skillcoincap);
			writer.Write((int) m_skillcoinvalue);
			writer.Write((int) m_partyexpsharerange);
			writer.Write((int) m_exppoweramount);
			writer.Write((int) m_expcoinvalue);
			writer.Write((int) m_l2to20multipier);
			writer.Write((int) m_l21to40multiplier);
			writer.Write((int) m_l41to60multiplier);
			writer.Write((int) m_l61to70multiplier);
			writer.Write((int) m_l71to80multiplier);
			writer.Write((int) m_l81to90multipier);
			writer.Write((int) m_l91to100multipier);
			writer.Write((int) m_l101to110multiplier);
			writer.Write((int) m_l111to120multiplier);
			writer.Write((int) m_l121to130multiplier);
			writer.Write((int) m_l131to140multiplier);
			writer.Write((int) m_l141to150multiplier);
			writer.Write((int) m_l151to160multiplier);
			writer.Write((int) m_l161to170multiplier);
			writer.Write((int) m_l171to180multiplier);
			writer.Write((int) m_l181to190multiplier);
			writer.Write((int) m_l191to200multiplier);
			writer.Write((int) m_equiprequiredlevel0);
			writer.Write((int) m_armorrequiredlevel1);
			writer.Write((int) m_armorrequiredlevel2);
			writer.Write((int) m_armorrequiredlevel3);
			writer.Write((int) m_armorrequiredlevel4);
			writer.Write((int) m_armorrequiredlevel5);
			writer.Write((int) m_armorrequiredlevel6);
			writer.Write((int) m_armorrequiredlevel7);
			writer.Write((int) m_armorrequiredlevel8);
			writer.Write((int) m_armorrequiredlevel9);
			writer.Write((int) m_armorrequiredlevel10);
			writer.Write((int) m_weaponrequiredlevel1);
			writer.Write((int) m_weaponrequiredlevel2);
			writer.Write((int) m_weaponrequiredlevel3);
			writer.Write((int) m_weaponrequiredlevel4);
			writer.Write((int) m_weaponrequiredlevel5);
			writer.Write((int) m_weaponrequiredlevel6);
			writer.Write((int) m_weaponrequiredlevel7);
			writer.Write((int) m_weaponrequiredlevel8);
			writer.Write((int) m_weaponrequiredlevel9);
			writer.Write((int) m_weaponrequiredlevel10);
			writer.Write((int) m_clothrequiredlevel1);
			writer.Write((int) m_clothrequiredlevel2);
			writer.Write((int) m_clothrequiredlevel3);
			writer.Write((int) m_clothrequiredlevel4);
			writer.Write((int) m_clothrequiredlevel5);
			writer.Write((int) m_clothrequiredlevel6);
			writer.Write((int) m_clothrequiredlevel7);
			writer.Write((int) m_clothrequiredlevel8);
			writer.Write((int) m_clothrequiredlevel9);
			writer.Write((int) m_clothrequiredlevel10);
			writer.Write((int) m_jewelrequiredlevel1);
			writer.Write((int) m_jewelrequiredlevel2);
			writer.Write((int) m_jewelrequiredlevel3);
			writer.Write((int) m_jewelrequiredlevel4);
			writer.Write((int) m_jewelrequiredlevel5);
			writer.Write((int) m_jewelrequiredlevel6);
			writer.Write((int) m_jewelrequiredlevel7);
			writer.Write((int) m_jewelrequiredlevel8);
			writer.Write((int) m_jewelrequiredlevel9);
			writer.Write((int) m_jewelrequiredlevel10);
			writer.Write((int) m_armorrequiredlevel1intensity);
			writer.Write((int) m_armorrequiredlevel2intensity);
			writer.Write((int) m_armorrequiredlevel3intensity);
			writer.Write((int) m_armorrequiredlevel4intensity);
			writer.Write((int) m_armorrequiredlevel5intensity);
			writer.Write((int) m_armorrequiredlevel6intensity);
			writer.Write((int) m_armorrequiredlevel7intensity);
			writer.Write((int) m_armorrequiredlevel8intensity);
			writer.Write((int) m_armorrequiredlevel9intensity);
			writer.Write((int) m_armorrequiredlevel10intensity);
			writer.Write((int) m_weaponrequiredlevel1intensity);
			writer.Write((int) m_weaponrequiredlevel2intensity);
			writer.Write((int) m_weaponrequiredlevel3intensity);
			writer.Write((int) m_weaponrequiredlevel4intensity);
			writer.Write((int) m_weaponrequiredlevel5intensity);
			writer.Write((int) m_weaponrequiredlevel6intensity);
			writer.Write((int) m_weaponrequiredlevel7intensity);
			writer.Write((int) m_weaponrequiredlevel8intensity);
			writer.Write((int) m_weaponrequiredlevel9intensity);
			writer.Write((int) m_weaponrequiredlevel10intensity);
			writer.Write((int) m_clothrequiredlevel1intensity);
			writer.Write((int) m_clothrequiredlevel2intensity);
			writer.Write((int) m_clothrequiredlevel3intensity);
			writer.Write((int) m_clothrequiredlevel4intensity);
			writer.Write((int) m_clothrequiredlevel5intensity);
			writer.Write((int) m_clothrequiredlevel6intensity);
			writer.Write((int) m_clothrequiredlevel7intensity);
			writer.Write((int) m_clothrequiredlevel8intensity);
			writer.Write((int) m_clothrequiredlevel9intensity);
			writer.Write((int) m_clothrequiredlevel10intensity);
			writer.Write((int) m_jewelrequiredlevel1intensity);
			writer.Write((int) m_jewelrequiredlevel2intensity);
			writer.Write((int) m_jewelrequiredlevel3intensity);
			writer.Write((int) m_jewelrequiredlevel4intensity);
			writer.Write((int) m_jewelrequiredlevel5intensity);
			writer.Write((int) m_jewelrequiredlevel6intensity);
			writer.Write((int) m_jewelrequiredlevel7intensity);
			writer.Write((int) m_jewelrequiredlevel8intensity);
			writer.Write((int) m_jewelrequiredlevel9intensity);
			writer.Write((int) m_jewelrequiredlevel10intensity);
			writer.Write((int) m_petstatlossamount);
			writer.Write((int) m_maxlevelwithscroll);
			writer.Write((int) m_endmaxlvl);
			writer.Write((int) m_skillstotal);
			writer.Write((int) m_superhealreq);
			writer.Write((int) m_teleporttotargetreq);
			writer.Write((int) m_massprovoketoattreq);
			writer.Write((int) m_masspeacereq);
			writer.Write((int) m_blessedpowerreq);
			writer.Write((int) m_areafireblastreq);
			writer.Write((int) m_areaiceblastreq);
			writer.Write((int) m_areaairblastreq);
			writer.Write((int) m_aurastatboostreq);
			writer.Write((int) m_beetle);
			writer.Write((int) m_desertostard);
			writer.Write((int) m_firesteed);
			writer.Write((int) m_forestostard);
			writer.Write((int) m_frenziedostard);
			writer.Write((int) m_hellsteed);
			writer.Write((int) m_hiryu);
			writer.Write((int) m_horse);
			writer.Write((int) m_kirin);
			writer.Write((int) m_lesserhiryu);
			writer.Write((int) m_nightmare);
			writer.Write((int) m_ridablellama);
			writer.Write((int) m_ridgeback);
			writer.Write((int) m_savageridgeback);
			writer.Write((int) m_scaledswampdragon);
			writer.Write((int) m_seahorse);
			writer.Write((int) m_silversteed);
			writer.Write((int) m_skeletalmount);
			writer.Write((int) m_swampdragon);
			writer.Write((int) m_unicorn);
			writer.Write((int) m_reptalon);
			writer.Write((int) m_wildtiger);
			writer.Write((int) m_windrunner);
			writer.Write((int) m_lasher);
			writer.Write((int) m_eowmu);
			writer.Write((int) m_dreadwarhorse);
			writer.Write((int) m_cusidhe);
			writer.Write((int) m_below20);
			writer.Write((int) m_below40);
			writer.Write((int) m_below60);
			writer.Write((int) m_below70);
			writer.Write((int) m_below80);
			writer.Write((int) m_below90);
			writer.Write((int) m_below100);
			writer.Write((int) m_below110);
			writer.Write((int) m_below120);
			writer.Write((int) m_below130);
			writer.Write((int) m_below140);
			writer.Write((int) m_below150);
			writer.Write((int) m_below160);
			writer.Write((int) m_below170);
			writer.Write((int) m_below180);
			writer.Write((int) m_below190);
			writer.Write((int) m_below200);
			writer.Write((int) m_below20stat);
			writer.Write((int) m_below40stat);
			writer.Write((int) m_below60stat);
			writer.Write((int) m_below70stat);
			writer.Write((int) m_below80stat);
			writer.Write((int) m_below90stat);
			writer.Write((int) m_below100stat);
			writer.Write((int) m_below110stat);
			writer.Write((int) m_below120stat);
			writer.Write((int) m_below130stat);
			writer.Write((int) m_below140stat);
			writer.Write((int) m_below150stat);
			writer.Write((int) m_below160stat);
			writer.Write((int) m_below170stat);
			writer.Write((int) m_below180stat);
			writer.Write((int) m_below190stat);
			writer.Write((int) m_below200stat);
			writer.Write((int) m_minskillreqpicksteal);
			writer.Write((int) m_maxstatpoints);
			writer.Write((int) m_petmaxstatpoints);
			writer.Write((int) m_Lvbelow20);
			writer.Write((int) m_Lvbelow40);
			writer.Write((int) m_Lvbelow60);
			writer.Write((int) m_Lvbelow70);
			writer.Write((int) m_Lvbelow80);
			writer.Write((int) m_Lvbelow90);
			writer.Write((int) m_Lvbelow100);
			writer.Write((int) m_Lvbelow110);
			writer.Write((int) m_Lvbelow120);
			writer.Write((int) m_Lvbelow130);
			writer.Write((int) m_Lvbelow140);
			writer.Write((int) m_Lvbelow150);
			writer.Write((int) m_Lvbelow160);
			writer.Write((int) m_Lvbelow170);
			writer.Write((int) m_Lvbelow180);
			writer.Write((int) m_Lvbelow190);
			writer.Write((int) m_Lvbelow200);
			writer.Write((int) m_gainfollowerslotonlevel20);
			writer.Write((int) m_gainfollowerslotonlevel30);
			writer.Write((int) m_gainfollowerslotonlevel40);
			writer.Write((int) m_gainfollowerslotonlevel50);
			writer.Write((int) m_gainfollowerslotonlevel60);
			writer.Write((int) m_gainfollowerslotonlevel70);
			writer.Write((int) m_gainfollowerslotonlevel80);
			writer.Write((int) m_gainfollowerslotonlevel90);
			writer.Write((int) m_gainfollowerslotonlevel100);
			writer.Write((int) m_gainfollowerslotonlevel110);
			writer.Write((int) m_gainfollowerslotonlevel120);
			writer.Write((int) m_gainfollowerslotonlevel130);
			writer.Write((int) m_gainfollowerslotonlevel140);
			writer.Write((int) m_gainfollowerslotonlevel150);
			writer.Write((int) m_gainfollowerslotonlevel160);
			writer.Write((int) m_gainfollowerslotonlevel170);
			writer.Write((int) m_gainfollowerslotonlevel180);
			writer.Write((int) m_gainfollowerslotonlevel190);
			writer.Write((int) m_gainfollowerslotonlevel200);
			writer.Write((int) m_begginggainamount);
			writer.Write((int) m_campinggainamount);
			writer.Write((int) m_forensicsgainamount);
			writer.Write((int) m_itemidgainamount);
			writer.Write((int) m_tasteidgainamount);
			writer.Write((int) m_imbuinggainamount);
			writer.Write((int) m_evalintgainamount);
			writer.Write((int) m_spiritspeakgainamount);
			writer.Write((int) m_fishinggainamount);
			writer.Write((int) m_herdinggainamount);
			writer.Write((int) m_hidinggainamount);
			writer.Write((int) m_poisoninggainamount);
			writer.Write((int) m_removetrapgainamount);
			writer.Write((int) m_stealinggainamount);
			writer.Write((int) m_discordancegainamount);
			writer.Write((int) m_peacemakinggainamount);
			writer.Write((int) m_provocationgainamount);
			writer.Write((int) m_anatomygainamount);
			writer.Write((int) m_armsloregainamount);
			writer.Write((int) m_animalloregainamount);
			writer.Write((int) m_meditationgainamount);
			writer.Write((int) m_cartographygainamount);
			writer.Write((int) m_detecthiddengainamount);
			writer.Write((int) m_animaltaminggainamount);
			writer.Write((int) m_blacksmithgainamount);
			writer.Write((int) m_carpentrygainamount);
			writer.Write((int) m_alchemygainamount);
			writer.Write((int) m_fletchinggainamount);
			writer.Write((int) m_cookinggainamount);
			writer.Write((int) m_inscribegainamount);
			writer.Write((int) m_tailoringgainamount);
			writer.Write((int) m_tinkeringgainamount);
			writer.Write((int) m_levelgroup1reqlevel);
			writer.Write((int) m_levelgroup1maxitems);
			writer.Write((int) m_levelgroup1reducetotal);
			writer.Write((int) m_levelgroup2reqlevel);
			writer.Write((int) m_levelgroup2maxitems);
			writer.Write((int) m_levelgroup2reducetotal);
			writer.Write((int) m_levelgroup3reqlevel);
			writer.Write((int) m_levelgroup3maxitems);
			writer.Write((int) m_levelgroup3reducetotal);
			writer.Write((int) m_levelgroup4reqlevel);
			writer.Write((int) m_levelgroup4maxitems);
			writer.Write((int) m_levelgroup4reducetotal);
			writer.Write((int) m_levelgroup5reqlevel);
			writer.Write((int) m_levelgroup5maxitems);
			writer.Write((int) m_levelgroup5reducetotal);
			writer.Write((int) m_levelgroup6reqlevel);
			writer.Write((int) m_levelgroup6maxitems);
			writer.Write((int) m_levelgroup6reducetotal);
			writer.Write((int) m_levelgroup7reqlevel);
			writer.Write((int) m_levelgroup7maxitems);
			writer.Write((int) m_levelgroup7reducetotal);
			writer.Write((int) m_levelgroup8reqlevel);
			writer.Write((int) m_levelgroup8maxitems);
			writer.Write((int) m_levelgroup8reducetotal);
			writer.Write((int) m_levelgroup9reqlevel);
			writer.Write((int) m_levelgroup9maxitems);
			writer.Write((int) m_levelgroup9reducetotal);
			writer.Write((int) m_levelgroup10reqlevel);
			writer.Write((int) m_levelgroup10maxitems);
			writer.Write((int) m_levelgroup10reducetotal);
			writer.Write(m_superhealchance);
			writer.Write(m_teleporttotarchance);	
			writer.Write(m_massprovokechance);
			writer.Write(m_masspeacechance);
			writer.Write(m_blessedpowerchance);
			writer.Write(m_areafireblastchance);
			writer.Write(m_areaiceblastchance);
			writer.Write(m_areaairblastchance);
			writer.Write((int) m_powerhourtime);
			writer.Write(m_nameofbattleratingstat);
			writer.Write(m_requiredlevelmouseover);
			writer.Write(m_levelgroup1msg);
			writer.Write(m_level1groupownermsg);
			writer.Write(m_level1groupownernow);
			writer.Write(m_levelgroup2msg);
			writer.Write(m_level2groupownermsg);
			writer.Write(m_level2groupownernow);
			writer.Write(m_levelgroup3msg);
			writer.Write(m_level3groupownermsg);
			writer.Write(m_level3groupownernow);
			writer.Write(m_levelgroup4msg);
			writer.Write(m_level4groupownermsg);
			writer.Write(m_level4groupownernow);
			writer.Write(m_levelgroup5msg);
			writer.Write(m_level5groupownermsg);
			writer.Write(m_level5groupownernow);
			writer.Write(m_levelgroup6msg);
			writer.Write(m_level6groupownermsg);
			writer.Write(m_level6groupownernow);
			writer.Write(m_levelgroup7msg);
			writer.Write(m_level7groupownermsg);
			writer.Write(m_level7groupownernow);
			writer.Write(m_levelgroup8msg);
			writer.Write(m_level8groupownermsg);
			writer.Write(m_level8groupownernow);
			writer.Write(m_levelgroup9msg);
			writer.Write(m_level9groupownermsg);
			writer.Write(m_level9groupownernow);
			writer.Write(m_levelgroup10msg);
			writer.Write(m_level10groupownermsg);
			writer.Write(m_level10groupownernow);
			writer.Write((int) m_petbelow20);
			writer.Write((int) m_petbelow40);
			writer.Write((int) m_petbelow60);
			writer.Write((int) m_petbelow70);
			writer.Write((int) m_petbelow80);
			writer.Write((int) m_petbelow90);
			writer.Write((int) m_petbelow100);
			writer.Write((int) m_petbelow110);
			writer.Write((int) m_petbelow120);
			writer.Write((int) m_petbelow130);
			writer.Write((int) m_petbelow140);
			writer.Write((int) m_petbelow150);
			writer.Write((int) m_petbelow160);
			writer.Write((int) m_petbelow170);
			writer.Write((int) m_petbelow180);
			writer.Write((int) m_petbelow190);
			writer.Write((int) m_petbelow200);
			writer.Write((int) m_petbelow20stat);		
			writer.Write((int) m_petbelow40stat);			
			writer.Write((int) m_petbelow60stat);			
			writer.Write((int) m_petbelow70stat);			
			writer.Write((int) m_petbelow80stat);		
			writer.Write((int) m_petbelow90stat);			
			writer.Write((int) m_petbelow100stat);			
			writer.Write((int) m_petbelow110stat);			
			writer.Write((int) m_petbelow120stat);			
			writer.Write((int) m_petbelow130stat);			
			writer.Write((int) m_petbelow140stat);				
			writer.Write((int) m_petbelow150stat);			
			writer.Write((int) m_petbelow160stat);		
			writer.Write((int) m_petbelow170stat);		
			writer.Write((int) m_petbelow180stat);		
			writer.Write((int) m_petbelow190stat);			
			writer.Write((int) m_petbelow200stat);
			writer.Write((bool)m_pethappysystem);
			writer.Write((bool)m_levelpacktrait);
			writer.Write((bool)m_petreclaimchance);
			
			
			writer.Write((bool)m_newstartinglocation);
			writer.Write((bool)m_forcenewplayerintoguild);
			writer.Write((bool)m_addtobackpackonattach);
			writer.Write((int)m_x_variable);
			writer.Write((int)m_y_variable);
			writer.Write((int)m_z_variable);
			writer.Write(m_maplocation);
			writer.Write(m_guildnamestart);
			writer.Write(m_startitem1);
			writer.Write(m_startitem2);
			writer.Write(m_startitem3);
			writer.Write(m_startitem4);
			writer.Write(m_startitem5);
			writer.Write(m_startitem6);
			writer.Write(m_startitem7);
			writer.Write(m_startitem8);
			writer.Write(m_startitem9);
			writer.Write(m_startitem10);

			
			writer.Write((bool)m_forcestartingstats);
			writer.Write((int)m_forcestartingstatsstr);
			writer.Write((int)m_forcestartingstatsdex);
			writer.Write((int)m_forcestartingstatsint);
			writer.Write((bool)m_autoactivate_startingstrcap);
			writer.Write((int)m_startingstrcapvar);
			writer.Write((int)m_startingstrcapmaxvar);	
			writer.Write((bool)m_autoactivate_startingdexcap);
			writer.Write((int)m_startingdexcapvar);
			writer.Write((int)m_startingdexcapmaxvar);
			writer.Write((bool)m_autoactivate_startingintcap);
			writer.Write((int)m_startingintcapvar);
			writer.Write((int)m_startingintcapmaxvar);
			writer.Write((bool)m_autoactivate_startingtotalstatcap);
			writer.Write((int)m_autoactivate_startingtotalstatcapvar);
			writer.Write((bool)m_autoactivate_gemmining);
			writer.Write((bool)m_autoactivate_basketweaving);
			writer.Write((bool)m_autoactivate_canbuycarpets);
			writer.Write((bool)m_autoactivate_acceptguildinvites);
			writer.Write((bool)m_autoactivate_glassblowing);
			writer.Write((bool)m_autoactivate_libraryfriend);
			writer.Write((bool)m_autoactivate_masonry);
			writer.Write((bool)m_autoactivate_sandmining);
			writer.Write((bool)m_autoactivate_stonemining);
			writer.Write((bool)m_autoactivate_spellweaving);
			writer.Write((bool)m_autoactivate_mechanicallife);
			writer.Write((bool)m_autoactivate_disabledpvpwarning);
			writer.Write((bool)m_autoactivate_isyoung);
			writer.Write((bool)m_autoactivate_cantwalk);
			writer.Write((bool)m_autoactivate_maxfollowslots);
			writer.Write((int)m_autoactivate_maxfollowslotstotal);
			writer.Write((bool)m_autoactivate_skillscap);
			writer.Write((int)m_autoactivate_skillscapvar);
			writer.Write((bool)m_mapbooltrammel);
			writer.Write((bool)m_mapboolfelucca);
			writer.Write((bool)m_mapboolmalas);


		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			// version 0
			
			m_playerlevels = reader.ReadBool();
			m_equipmentlevelsystem = reader.ReadBool();
			m_weaponlevels = reader.ReadBool();
			m_armorlevels = reader.ReadBool();
			m_jewellevels = reader.ReadBool();
			m_clothinglevels = reader.ReadBool();
			m_gainexpfrombods = reader.ReadBool();
			m_disableskillgain = reader.ReadBool();
			m_levelbelowtoon = reader.ReadBool();
			m_showpaperdolllevel = reader.ReadBool();
			m_petkillgivesexp = reader.ReadBool();
			m_craftgivesexp = reader.ReadBool();
			m_advancedskillexp = reader.ReadBool();
			m_tablesadvancedexp = reader.ReadBool();
			m_staffhaslevel = reader.ReadBool();
			m_bonusstatonlvl = reader.ReadBool();
			m_refreshonlevel = reader.ReadBool();
			m_refreshonlevelpet = reader.ReadBool();
			m_levelsheetperma = reader.ReadBool();
			m_showvendorlevels = reader.ReadBool();
			m_discountfromvendors = reader.ReadBool();
			m_partyexpshare = reader.ReadBool();
			m_partyexpsharesplit = reader.ReadBool();
			m_levelstatresetbutton = reader.ReadBool();
			m_levelskillresetbutton = reader.ReadBool();
			m_miscelaneous = reader.ReadBool();
			m_combat = reader.ReadBool();
			m_tradeskills = reader.ReadBool();
			m_magic = reader.ReadBool();
			m_wilderness = reader.ReadBool();
			m_thieving = reader.ReadBool();
			m_bard = reader.ReadBool();
			m_imbuing = reader.ReadBool();
			m_throwing = reader.ReadBool();
			m_mysticism = reader.ReadBool();
			m_spellweaving = reader.ReadBool();
			m_activatedynamiclevelsystem = reader.ReadBool();
			m_enabledlevelpets = reader.ReadBool();
			m_mountedpetsgainexp = reader.ReadBool();
			m_petattackbonus = reader.ReadBool();
			m_levelbelowpet = reader.ReadBool();
			m_loseexplevelondeath = reader.ReadBool();
			m_losestatondeath = reader.ReadBool();
			m_petlevelsheetperma = reader.ReadBool();
			m_petexpgainfromkill = reader.ReadBool();
			m_notifyonpetexpgain = reader.ReadBool();
			m_notifyonpetlevelup = reader.ReadBool();
			m_untamedcreaturelevels = reader.ReadBool();
			m_emoteonspecialatks = reader.ReadBool();
			m_emotesonauraboost = reader.ReadBool();
			m_superheal = reader.ReadBool();
			m_petshouldbebonded = reader.ReadBool();
			m_teleporttotarget = reader.ReadBool();
			m_massprovoketoatt = reader.ReadBool();
			m_masspeacearea = reader.ReadBool();
			m_blessedpower = reader.ReadBool();
			m_areafireblast = reader.ReadBool();
			m_areaiceblast = reader.ReadBool();
			m_areaairblast = reader.ReadBool();
			m_aurastatboost = reader.ReadBool();
			m_enablemountcheck = reader.ReadBool();
			m_enablepetpicks = reader.ReadBool();
			m_preventbondedpetpick = reader.ReadBool();
			m_usedynamicmaxlevels = reader.ReadBool();
			m_gainfollowerslotonlevel = reader.ReadBool();
			m_gainon20 = reader.ReadBool();
			m_gainon30 = reader.ReadBool();
			m_gainon40 = reader.ReadBool();
			m_gainon50 = reader.ReadBool();
			m_gainon60 = reader.ReadBool();
			m_gainon70 = reader.ReadBool();
			m_gainon80 = reader.ReadBool();
			m_gainon90 = reader.ReadBool();
			m_gainon100 = reader.ReadBool();
			m_gainon110 = reader.ReadBool();
			m_gainon120 = reader.ReadBool();
			m_gainon130 = reader.ReadBool();
			m_gainon140 = reader.ReadBool();
			m_gainon150 = reader.ReadBool();
			m_gainon160 = reader.ReadBool();
			m_gainon170 = reader.ReadBool();
			m_gainon180 = reader.ReadBool();
			m_gainon190 = reader.ReadBool();
			m_gainon200 = reader.ReadBool();
			m_enableexpfromskills = reader.ReadBool();
			m_begginggain = reader.ReadBool();
			m_campinggain = reader.ReadBool();
			m_forensicsgain = reader.ReadBool();
			m_itemidgain = reader.ReadBool();
			m_tasteidgain = reader.ReadBool();
			m_imbuinggain = reader.ReadBool();
			m_evalintgain = reader.ReadBool();
			m_spiritspeakgain = reader.ReadBool();
			m_fishinggain = reader.ReadBool();
			m_herdinggain = reader.ReadBool();
			m_trackinggain = reader.ReadBool();
			m_hidinggain = reader.ReadBool();
			m_poisoninggain = reader.ReadBool();
			m_removetrapgain = reader.ReadBool();
			m_stealinggain = reader.ReadBool();
			m_discordancegain = reader.ReadBool();
			m_peacemakinggain = reader.ReadBool();
			m_provocationgain = reader.ReadBool();
			m_anatomygain = reader.ReadBool();
			m_armsloregain = reader.ReadBool();
			m_animalloregain = reader.ReadBool();
			m_meditationgain = reader.ReadBool();
			m_cartographygain = reader.ReadBool();
			m_detecthiddengain = reader.ReadBool();
			m_animaltaminggain = reader.ReadBool();
			m_blacksmithgain = reader.ReadBool();
			m_carpentrygain = reader.ReadBool();
			m_alchemygain = reader.ReadBool();
			m_fletchinggain = reader.ReadBool();
			m_cookinggain = reader.ReadBool();
			m_inscribegain = reader.ReadBool();
			m_tailoringgain = reader.ReadBool();
			m_tinkeringgain = reader.ReadBool();
			m_bagsystemmaintoggle = reader.ReadBool();
			m_preventbagdrop = reader.ReadBool();
			m_bagblessed = reader.ReadBool();
			m_levelgroup1 = reader.ReadBool();
			m_level1groupbagownership = reader.ReadBool();
			m_levelgroup2 = reader.ReadBool();
			m_level2groupbagownership = reader.ReadBool();
			m_levelgroup3 = reader.ReadBool();
			m_level3groupbagownership = reader.ReadBool();
			m_levelgroup4 = reader.ReadBool();
			m_level4groupbagownership = reader.ReadBool();
			m_levelgroup5 = reader.ReadBool();
			m_level5groupbagownership = reader.ReadBool();
			m_levelgroup6 = reader.ReadBool();
			m_level6groupbagownership = reader.ReadBool();
			m_levelgroup7 = reader.ReadBool();
			m_level7groupbagownership = reader.ReadBool();
			m_levelgroup8 = reader.ReadBool();
			m_level8groupbagownership = reader.ReadBool();
			m_levelgroup9 = reader.ReadBool();
			m_level9groupbagownership = reader.ReadBool();
			m_levelgroup10 = reader.ReadBool();
			m_level10groupbagownership = reader.ReadBool();
			m_maxlevel = reader.ReadBool();
			m_startmaxlvl = reader.ReadInt();
			m_startmaxlvlpets = reader.ReadInt();
			m_endmaxlvl = reader.ReadInt();
			m_skillcoincap = reader.ReadInt();
			m_skillcoinvalue = reader.ReadInt();
			m_partyexpsharerange = reader.ReadInt();
			m_exppoweramount = reader.ReadInt();
			m_expcoinvalue = reader.ReadInt();
			m_l2to20multipier = reader.ReadInt();
			m_l21to40multiplier = reader.ReadInt();
			m_l41to60multiplier = reader.ReadInt();
			m_l61to70multiplier = reader.ReadInt();
			m_l71to80multiplier = reader.ReadInt();
			m_l81to90multipier = reader.ReadInt();
			m_l91to100multipier = reader.ReadInt();
			m_l101to110multiplier = reader.ReadInt();
			m_l111to120multiplier = reader.ReadInt();
			m_l121to130multiplier = reader.ReadInt();
			m_l131to140multiplier = reader.ReadInt();
			m_l141to150multiplier = reader.ReadInt();
			m_l151to160multiplier = reader.ReadInt();
			m_l161to170multiplier = reader.ReadInt();
			m_l171to180multiplier = reader.ReadInt();
			m_l181to190multiplier = reader.ReadInt();
			m_l191to200multiplier = reader.ReadInt();
			m_equiprequiredlevel0 = reader.ReadInt();
			m_armorrequiredlevel1 = reader.ReadInt();
			m_armorrequiredlevel2 = reader.ReadInt();
			m_armorrequiredlevel3 = reader.ReadInt();
			m_armorrequiredlevel4 = reader.ReadInt();
			m_armorrequiredlevel5 = reader.ReadInt();
			m_armorrequiredlevel6 = reader.ReadInt();
			m_armorrequiredlevel7 = reader.ReadInt();
			m_armorrequiredlevel8 = reader.ReadInt();
			m_armorrequiredlevel9 = reader.ReadInt();
			m_armorrequiredlevel10 = reader.ReadInt();
			m_weaponrequiredlevel1 = reader.ReadInt();
			m_weaponrequiredlevel2 = reader.ReadInt();
			m_weaponrequiredlevel3 = reader.ReadInt();
			m_weaponrequiredlevel4 = reader.ReadInt();
			m_weaponrequiredlevel5 = reader.ReadInt();
			m_weaponrequiredlevel6 = reader.ReadInt();
			m_weaponrequiredlevel7 = reader.ReadInt();
			m_weaponrequiredlevel8 = reader.ReadInt();
			m_weaponrequiredlevel9 = reader.ReadInt();
			m_weaponrequiredlevel10 = reader.ReadInt();
			m_clothrequiredlevel1 = reader.ReadInt();
			m_clothrequiredlevel2 = reader.ReadInt();
			m_clothrequiredlevel3 = reader.ReadInt();
			m_clothrequiredlevel4 = reader.ReadInt();
			m_clothrequiredlevel5 = reader.ReadInt();
			m_clothrequiredlevel6 = reader.ReadInt();
			m_clothrequiredlevel7 = reader.ReadInt();
			m_clothrequiredlevel8 = reader.ReadInt();
			m_clothrequiredlevel9 = reader.ReadInt();
			m_clothrequiredlevel10 = reader.ReadInt();
			m_jewelrequiredlevel1 = reader.ReadInt();
			m_jewelrequiredlevel2 = reader.ReadInt();
			m_jewelrequiredlevel3 = reader.ReadInt();
			m_jewelrequiredlevel4 = reader.ReadInt();
			m_jewelrequiredlevel5 = reader.ReadInt();
			m_jewelrequiredlevel6 = reader.ReadInt();
			m_jewelrequiredlevel7 = reader.ReadInt();
			m_jewelrequiredlevel8 = reader.ReadInt();
			m_jewelrequiredlevel9 = reader.ReadInt();
			m_jewelrequiredlevel10 = reader.ReadInt();
			m_armorrequiredlevel1intensity = reader.ReadInt();
			m_armorrequiredlevel2intensity = reader.ReadInt();
			m_armorrequiredlevel3intensity = reader.ReadInt();
			m_armorrequiredlevel4intensity = reader.ReadInt();
			m_armorrequiredlevel5intensity = reader.ReadInt();
			m_armorrequiredlevel6intensity = reader.ReadInt();
			m_armorrequiredlevel7intensity = reader.ReadInt();
			m_armorrequiredlevel8intensity = reader.ReadInt();
			m_armorrequiredlevel9intensity = reader.ReadInt();
			m_armorrequiredlevel10intensity = reader.ReadInt();
			m_weaponrequiredlevel1intensity = reader.ReadInt();
			m_weaponrequiredlevel2intensity = reader.ReadInt();
			m_weaponrequiredlevel3intensity = reader.ReadInt();
			m_weaponrequiredlevel4intensity = reader.ReadInt();
			m_weaponrequiredlevel5intensity = reader.ReadInt();
			m_weaponrequiredlevel6intensity = reader.ReadInt();
			m_weaponrequiredlevel7intensity = reader.ReadInt();
			m_weaponrequiredlevel8intensity = reader.ReadInt();
			m_weaponrequiredlevel9intensity = reader.ReadInt();
			m_weaponrequiredlevel10intensity = reader.ReadInt();
			m_clothrequiredlevel1intensity = reader.ReadInt();
			m_clothrequiredlevel2intensity = reader.ReadInt();
			m_clothrequiredlevel3intensity = reader.ReadInt();
			m_clothrequiredlevel4intensity = reader.ReadInt();
			m_clothrequiredlevel5intensity = reader.ReadInt();
			m_clothrequiredlevel6intensity = reader.ReadInt();
			m_clothrequiredlevel7intensity = reader.ReadInt();
			m_clothrequiredlevel8intensity = reader.ReadInt();
			m_clothrequiredlevel9intensity = reader.ReadInt();
			m_clothrequiredlevel10intensity = reader.ReadInt();
			m_jewelrequiredlevel1intensity = reader.ReadInt();
			m_jewelrequiredlevel2intensity = reader.ReadInt();
			m_jewelrequiredlevel3intensity = reader.ReadInt();
			m_jewelrequiredlevel4intensity = reader.ReadInt();
			m_jewelrequiredlevel5intensity = reader.ReadInt();
			m_jewelrequiredlevel6intensity = reader.ReadInt();
			m_jewelrequiredlevel7intensity = reader.ReadInt();
			m_jewelrequiredlevel8intensity = reader.ReadInt();
			m_jewelrequiredlevel9intensity = reader.ReadInt();
			m_jewelrequiredlevel10intensity = reader.ReadInt();
			m_petstatlossamount = reader.ReadInt();
			m_maxlevelwithscroll = reader.ReadInt();
			m_endmaxlvl = reader.ReadInt();
			m_skillstotal = reader.ReadInt();
			m_superhealreq = reader.ReadInt();
			m_teleporttotargetreq = reader.ReadInt();
			m_massprovoketoattreq = reader.ReadInt();
			m_masspeacereq = reader.ReadInt();
			m_blessedpowerreq = reader.ReadInt();
			m_areafireblastreq = reader.ReadInt();
			m_areaiceblastreq = reader.ReadInt();
			m_areaairblastreq = reader.ReadInt();
			m_aurastatboostreq = reader.ReadInt();
			m_beetle = reader.ReadInt();
			m_desertostard = reader.ReadInt();
			m_firesteed = reader.ReadInt();
			m_forestostard = reader.ReadInt();
			m_frenziedostard = reader.ReadInt();
			m_hellsteed = reader.ReadInt();
			m_hiryu = reader.ReadInt();
			m_horse = reader.ReadInt();
			m_kirin = reader.ReadInt();
			m_lesserhiryu = reader.ReadInt();
			m_nightmare = reader.ReadInt();
			m_ridablellama = reader.ReadInt();
			m_ridgeback = reader.ReadInt();
			m_savageridgeback = reader.ReadInt();
			m_scaledswampdragon = reader.ReadInt();
			m_seahorse = reader.ReadInt();
			m_silversteed = reader.ReadInt();
			m_skeletalmount = reader.ReadInt();
			m_swampdragon = reader.ReadInt();
			m_unicorn = reader.ReadInt();
			m_reptalon = reader.ReadInt();
			m_wildtiger = reader.ReadInt();
			m_windrunner = reader.ReadInt();
			m_lasher = reader.ReadInt();
			m_eowmu = reader.ReadInt();
			m_dreadwarhorse = reader.ReadInt();
			m_cusidhe = reader.ReadInt();
			m_below20 = reader.ReadInt();
			m_below40 = reader.ReadInt();
			m_below60 = reader.ReadInt();
			m_below70 = reader.ReadInt();
			m_below80 = reader.ReadInt();
			m_below90 = reader.ReadInt();
			m_below100 = reader.ReadInt();
			m_below110 = reader.ReadInt();
			m_below120 = reader.ReadInt();
			m_below130 = reader.ReadInt();
			m_below140 = reader.ReadInt();
			m_below150 = reader.ReadInt();
			m_below160 = reader.ReadInt();
			m_below170 = reader.ReadInt();
			m_below180 = reader.ReadInt();
			m_below190 = reader.ReadInt();
			m_below200 = reader.ReadInt();
			m_below20stat = reader.ReadInt();
			m_below40stat = reader.ReadInt();
			m_below60stat = reader.ReadInt();
			m_below70stat = reader.ReadInt();
			m_below80stat = reader.ReadInt();
			m_below90stat = reader.ReadInt();
			m_below100stat = reader.ReadInt();
			m_below110stat = reader.ReadInt();
			m_below120stat = reader.ReadInt();
			m_below130stat = reader.ReadInt();
			m_below140stat = reader.ReadInt();
			m_below150stat = reader.ReadInt();
			m_below160stat = reader.ReadInt();
			m_below170stat = reader.ReadInt();
			m_below180stat = reader.ReadInt();
			m_below190stat = reader.ReadInt();
			m_below200stat = reader.ReadInt();
			m_minskillreqpicksteal = reader.ReadInt();
			m_maxstatpoints = reader.ReadInt();
			m_petmaxstatpoints = reader.ReadInt();
			m_Lvbelow20 = reader.ReadInt();
			m_Lvbelow40 = reader.ReadInt();
			m_Lvbelow60 = reader.ReadInt();
			m_Lvbelow70 = reader.ReadInt();
			m_Lvbelow80 = reader.ReadInt();
			m_Lvbelow90 = reader.ReadInt();
			m_Lvbelow100 = reader.ReadInt();
			m_Lvbelow110 = reader.ReadInt();
			m_Lvbelow120 = reader.ReadInt();
			m_Lvbelow130 = reader.ReadInt();
			m_Lvbelow140 = reader.ReadInt();
			m_Lvbelow150 = reader.ReadInt();
			m_Lvbelow160 = reader.ReadInt();
			m_Lvbelow170 = reader.ReadInt();
			m_Lvbelow180 = reader.ReadInt();
			m_Lvbelow190 = reader.ReadInt();
			m_Lvbelow200 = reader.ReadInt();
			m_gainfollowerslotonlevel20 = reader.ReadInt();
			m_gainfollowerslotonlevel30 = reader.ReadInt();
			m_gainfollowerslotonlevel40 = reader.ReadInt();
			m_gainfollowerslotonlevel50 = reader.ReadInt();
			m_gainfollowerslotonlevel60 = reader.ReadInt();
			m_gainfollowerslotonlevel70 = reader.ReadInt();
			m_gainfollowerslotonlevel80 = reader.ReadInt();
			m_gainfollowerslotonlevel90 = reader.ReadInt();
			m_gainfollowerslotonlevel100 = reader.ReadInt();
			m_gainfollowerslotonlevel110 = reader.ReadInt();
			m_gainfollowerslotonlevel120 = reader.ReadInt();
			m_gainfollowerslotonlevel130 = reader.ReadInt();
			m_gainfollowerslotonlevel140 = reader.ReadInt();
			m_gainfollowerslotonlevel150 = reader.ReadInt();
			m_gainfollowerslotonlevel160 = reader.ReadInt();
			m_gainfollowerslotonlevel170 = reader.ReadInt();
			m_gainfollowerslotonlevel180 = reader.ReadInt();
			m_gainfollowerslotonlevel190 = reader.ReadInt();
			m_gainfollowerslotonlevel200 = reader.ReadInt();
			m_begginggainamount = reader.ReadInt();
			m_campinggainamount = reader.ReadInt();
			m_forensicsgainamount = reader.ReadInt();
			m_itemidgainamount = reader.ReadInt();
			m_tasteidgainamount = reader.ReadInt();
			m_imbuinggainamount = reader.ReadInt();
			m_evalintgainamount = reader.ReadInt();
			m_spiritspeakgainamount = reader.ReadInt();
			m_fishinggainamount = reader.ReadInt();
			m_herdinggainamount = reader.ReadInt();
			m_hidinggainamount = reader.ReadInt();
			m_poisoninggainamount = reader.ReadInt();
			m_removetrapgainamount = reader.ReadInt();
			m_stealinggainamount = reader.ReadInt();
			m_discordancegainamount = reader.ReadInt();
			m_peacemakinggainamount = reader.ReadInt();
			m_provocationgainamount = reader.ReadInt();
			m_anatomygainamount = reader.ReadInt();
			m_armsloregainamount = reader.ReadInt();
			m_animalloregainamount = reader.ReadInt();
			m_meditationgainamount = reader.ReadInt();
			m_cartographygainamount = reader.ReadInt();
			m_detecthiddengainamount = reader.ReadInt();
			m_animaltaminggainamount = reader.ReadInt();
			m_blacksmithgainamount = reader.ReadInt();
			m_carpentrygainamount = reader.ReadInt();
			m_alchemygainamount = reader.ReadInt();
			m_fletchinggainamount = reader.ReadInt();
			m_cookinggainamount = reader.ReadInt();
			m_inscribegainamount = reader.ReadInt();
			m_tailoringgainamount = reader.ReadInt();
			m_tinkeringgainamount = reader.ReadInt();
			m_levelgroup1reqlevel = reader.ReadInt();
			m_levelgroup1maxitems = reader.ReadInt();
			m_levelgroup1reducetotal = reader.ReadInt();
			m_levelgroup2reqlevel = reader.ReadInt();
			m_levelgroup2maxitems = reader.ReadInt();
			m_levelgroup2reducetotal = reader.ReadInt();
			m_levelgroup3reqlevel = reader.ReadInt();
			m_levelgroup3maxitems = reader.ReadInt();
			m_levelgroup3reducetotal = reader.ReadInt();
			m_levelgroup4reqlevel = reader.ReadInt();
			m_levelgroup4maxitems = reader.ReadInt();
			m_levelgroup4reducetotal = reader.ReadInt();
			m_levelgroup5reqlevel = reader.ReadInt();
			m_levelgroup5maxitems = reader.ReadInt();
			m_levelgroup5reducetotal = reader.ReadInt();
			m_levelgroup6reqlevel = reader.ReadInt();
			m_levelgroup6maxitems = reader.ReadInt();
			m_levelgroup6reducetotal = reader.ReadInt();
			m_levelgroup7reqlevel = reader.ReadInt();
			m_levelgroup7maxitems = reader.ReadInt();
			m_levelgroup7reducetotal = reader.ReadInt();
			m_levelgroup8reqlevel = reader.ReadInt();
			m_levelgroup8maxitems = reader.ReadInt();
			m_levelgroup8reducetotal = reader.ReadInt();
			m_levelgroup9reqlevel = reader.ReadInt();
			m_levelgroup9maxitems = reader.ReadInt();
			m_levelgroup9reducetotal = reader.ReadInt();
			m_levelgroup10reqlevel = reader.ReadInt();
			m_levelgroup10maxitems = reader.ReadInt();
			m_levelgroup10reducetotal = reader.ReadInt();
			m_superhealchance = reader.ReadDouble();
			m_teleporttotarchance = reader.ReadDouble();
			m_massprovokechance = reader.ReadDouble();
			m_masspeacechance = reader.ReadDouble();
			m_blessedpowerchance = reader.ReadDouble();
			m_areafireblastchance = reader.ReadDouble();
			m_areaiceblastchance = reader.ReadDouble();
			m_areaairblastchance = reader.ReadDouble();
			m_powerhourtime			= reader.ReadInt();
			m_nameofbattleratingstat = reader.ReadString();
			m_requiredlevelmouseover = reader.ReadString();
			m_levelgroup1msg = reader.ReadString();
			m_level1groupownermsg = reader.ReadString();
			m_level1groupownernow = reader.ReadString();
			m_levelgroup2msg = reader.ReadString();
			m_level2groupownermsg = reader.ReadString();
			m_level2groupownernow = reader.ReadString();
			m_levelgroup3msg = reader.ReadString();
			m_level3groupownermsg = reader.ReadString();
			m_level3groupownernow = reader.ReadString();
			m_levelgroup4msg = reader.ReadString();
			m_level4groupownermsg = reader.ReadString();
			m_level4groupownernow = reader.ReadString();
			m_levelgroup5msg = reader.ReadString();
			m_level5groupownermsg = reader.ReadString();
			m_level5groupownernow = reader.ReadString();
			m_levelgroup6msg = reader.ReadString();
			m_level6groupownermsg = reader.ReadString();
			m_level6groupownernow = reader.ReadString();
			m_levelgroup7msg = reader.ReadString();
			m_level7groupownermsg = reader.ReadString();
			m_level7groupownernow = reader.ReadString();
			m_levelgroup8msg = reader.ReadString();
			m_level8groupownermsg = reader.ReadString();
			m_level8groupownernow = reader.ReadString();
			m_levelgroup9msg = reader.ReadString();
			m_level9groupownermsg = reader.ReadString();
			m_level9groupownernow = reader.ReadString();
			m_levelgroup10msg = reader.ReadString();
			m_level10groupownermsg = reader.ReadString();
			m_level10groupownernow = reader.ReadString();
			m_petbelow20 = reader.ReadInt();
			m_petbelow40 = reader.ReadInt();
			m_petbelow60 = reader.ReadInt();
			m_petbelow70 = reader.ReadInt();
			m_petbelow80 = reader.ReadInt();
			m_petbelow90 = reader.ReadInt();
			m_petbelow100 = reader.ReadInt();
			m_petbelow110 = reader.ReadInt();
			m_petbelow120 = reader.ReadInt();
			m_petbelow130 = reader.ReadInt();
			m_petbelow140 = reader.ReadInt();
			m_petbelow150 = reader.ReadInt();
			m_petbelow160 = reader.ReadInt();
			m_petbelow170 = reader.ReadInt();
			m_petbelow180 = reader.ReadInt();
			m_petbelow190 = reader.ReadInt();
			m_petbelow200 = reader.ReadInt();
			m_petbelow20stat = reader.ReadInt();		
			m_petbelow40stat = reader.ReadInt();			
			m_petbelow60stat = reader.ReadInt();			
			m_petbelow70stat = reader.ReadInt();			
			m_petbelow80stat = reader.ReadInt();		
			m_petbelow90stat = reader.ReadInt();			
			m_petbelow100stat = reader.ReadInt();			
			m_petbelow110stat = reader.ReadInt();			
			m_petbelow120stat = reader.ReadInt();			
			m_petbelow130stat = reader.ReadInt();			
			m_petbelow140stat = reader.ReadInt();				
			m_petbelow150stat = reader.ReadInt();			
			m_petbelow160stat = reader.ReadInt();		
			m_petbelow170stat = reader.ReadInt();		
			m_petbelow180stat = reader.ReadInt();		
			m_petbelow190stat = reader.ReadInt();			
			m_petbelow200stat = reader.ReadInt();
			m_pethappysystem = reader.ReadBool();
			m_levelpacktrait = reader.ReadBool();
			m_petreclaimchance = reader.ReadBool();
			m_newstartinglocation = reader.ReadBool();
			m_forcenewplayerintoguild = reader.ReadBool();
			m_addtobackpackonattach = reader.ReadBool();
			m_x_variable = reader.ReadInt();
			m_y_variable = reader.ReadInt();
			m_z_variable = reader.ReadInt();
			m_maplocation = reader.ReadString();
			m_guildnamestart = reader.ReadString();
			m_startitem1 = reader.ReadString();
			m_startitem2 = reader.ReadString();
			m_startitem3 = reader.ReadString();
			m_startitem4 = reader.ReadString();
			m_startitem5 = reader.ReadString();
			m_startitem6 = reader.ReadString();
			m_startitem7 = reader.ReadString();
			m_startitem8 = reader.ReadString();
			m_startitem9 = reader.ReadString();
			m_startitem10 = reader.ReadString();
			
			
			m_forcestartingstats = reader.ReadBool();
			m_forcestartingstatsstr = reader.ReadInt();
			m_forcestartingstatsdex = reader.ReadInt();
			m_forcestartingstatsint = reader.ReadInt();
			m_autoactivate_startingstrcap = reader.ReadBool();
			m_startingstrcapvar = reader.ReadInt();
			m_startingstrcapmaxvar = reader.ReadInt();	
			m_autoactivate_startingdexcap = reader.ReadBool();
			m_startingdexcapvar = reader.ReadInt();
			m_startingdexcapmaxvar = reader.ReadInt();
			m_autoactivate_startingintcap = reader.ReadBool();
			m_startingintcapvar = reader.ReadInt();
			m_startingintcapmaxvar = reader.ReadInt();
			m_autoactivate_startingtotalstatcap = reader.ReadBool();
			m_autoactivate_startingtotalstatcapvar = reader.ReadInt();
			m_autoactivate_gemmining = reader.ReadBool();
			m_autoactivate_basketweaving = reader.ReadBool();
			m_autoactivate_canbuycarpets = reader.ReadBool();
			m_autoactivate_acceptguildinvites = reader.ReadBool();
			m_autoactivate_glassblowing = reader.ReadBool();
			m_autoactivate_libraryfriend = reader.ReadBool();
			m_autoactivate_masonry = reader.ReadBool();
			m_autoactivate_sandmining = reader.ReadBool();
			m_autoactivate_stonemining = reader.ReadBool();
			m_autoactivate_spellweaving = reader.ReadBool();
			m_autoactivate_mechanicallife = reader.ReadBool();
			m_autoactivate_disabledpvpwarning = reader.ReadBool();
			m_autoactivate_isyoung = reader.ReadBool();
			m_autoactivate_cantwalk = reader.ReadBool();
			m_autoactivate_maxfollowslots = reader.ReadBool();
			m_autoactivate_maxfollowslotstotal = reader.ReadInt();
			m_autoactivate_skillscap = reader.ReadBool();
			m_autoactivate_skillscapvar = reader.ReadInt();
			m_mapbooltrammel = reader.ReadBool();
			m_mapboolfelucca = reader.ReadBool();
			m_mapboolmalas = reader.ReadBool();

		}
    }
}
