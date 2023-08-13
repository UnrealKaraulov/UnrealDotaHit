using System;
using System.Collections.Generic;
using System.Text;
using DotaHIT.Jass.Types;
using DotaHIT.Core;
using DotaHIT.Core.Resources;

namespace DotaHIT.Jass.Native.Constants
{    
    public enum OrderID : uint
    {
        none = 0,
        smart = 0x0D0003,//851971,
        stop = 0x0D0004,//851972,
        stunned = 0x0D0005,//851973
        cancel = 0x0D0008,//851976,
        setrally = 0x0D000C,//851980,
        getitem = 0x0D000D,//851981,
        attack = 0x0D000F,//851983,
        attackground = 0x0D0010,//851984,
        attackonce = 0x0D0011,//851985,
        move = 0x0D0012,//851986,
        AImove = 0x0D0014,//851988,
        patrol = 0x0D0016,//851990,
        holdposition = 0x0D0019,//851993,
        buildmenu = 0x0D001A,//851994,
        humanbuild = 0x0D001B,//851995,
        orcbuild = 0x0D001C,//851996,
        nightelfbuild = 0x0D001D,//851997,
        undeadbuild = 0x0D001E,//851998,
        resumebuild = 0x0D001F,//851999,
        skillmenu = 0x0D0020,//852000,
        dropitem = 0x0D0021,//852001,
        moveslot1 = 0x0D0022,//852002,
        moveslot2 = 0x0D0023,//852003,
        moveslot3 = 0x0D0024,//852004,
        moveslot4 = 0x0D0025,//852005,
        moveslot5 = 0x0D0026,//852006,
        moveslot6 = 0x0D0027,//852007,
        useslot1 = 0x0D0028,//852008,
        useslot2 = 0x0D0029,//852009,
        useslot3 = 0x0D002A,//852010,
        useslot4 = 0x0D002B,//852011,
        useslot5 = 0x0D002C,//852012,
        useslot6 = 0x0D002D,//852013,
        detectaoe = 0x0D002F,//852015,
        resumeharvesting = 0x0D0031,//852017,
        harvest = 0x0D0032,//852018,
        returnresources = 0x0D0034,//852020,
        autoharvestgold = 0x0D0035,//852021,
        autoharvestlumber = 0x0D0036,//852022,
        neutraldetectaoe = 0x0D0037,//852023,
        repair = 0x0D0038,//852024,
        repairon = 0x0D0039,//852025,
        repairoff = 0x0D003A,//852026,
        revive = 0x0D0047,//852039,
        selfdestruct = 0x0D0048,//852040,
        selfdestructon = 0x0D0049,//852041,
        selfdestructoff = 0x0D004A,//852042,
        board = 0x0D004B,//852043,
        forceboard = 0x0D004C,//852044,
        load = 0x0D004E,//852046,
        unload = 0x0D004F,//852047,
        unloadall = 0x0D0050,//852048,
        unloadallinstant = 0x0D0051,//852049,
        loadcorpse = 0x0D0052,//852050,
        loadcorpseinstant = 0x0D0055,//852053,
        unloadallcorpses = 0x0D0056,//852054,
        defend = 0x0D0057,//852055,
        undefend = 0x0D0058,//852056,
        dispel = 0x0D0059,//852057,
        flare = 0x0D005C,//852060,
        heal = 0x0D005F,//852063,
        healon = 0x0D0060,//852064,
        healoff = 0x0D0061,//852065,
        innerfire = 0x0D0062,//852066,
        innerfireon = 0x0D0063,//852067,
        innerfireoff = 0x0D0064,//852068,
        invisibility = 0x0D0065,//852069,
        militiaconvert = 0x0D0067,//852071,
        militia = 0x0D0068,//852072,
        militiaoff = 0x0D0069,//852073,
        polymorph = 0x0D006A,//852074,
        slow = 0x0D006B,//852075,
        slowon = 0x0D006C,//852076,
        slowoff = 0x0D006D,//852077,
        tankdroppilot = 0x0D006F,//852079,
        tankloadpilot = 0x0D0070,//852080,
        tankpilot = 0x0D0071,//852081,
        townbellon = 0x0D0072,//852082,
        townbelloff = 0x0D0073,//852083,
        avatar = 0x0D0076,//852086,
        unavatar = 0x0D0077,//852087,
        blizzard = 0x0D0079,//852089,
        divineshield = 0x0D007A,//852090,
        undivineshield = 0x0D007B,//852091,
        holybolt = 0x0D007C,//852092,
        massteleport = 0x0D007D,//852093,
        thunderbolt = 0x0D007F,//852095,
        thunderclap = 0x0D0080,//852096,
        waterelemental = 0x0D0081,//852097,
        battlestations = 0x0D0083,//852099,
        berserk = 0x0D0084,//852100,
        bloodlust = 0x0D0085,//852101,
        bloodluston = 0x0D0086,//852102,
        bloodlustoff = 0x0D0087,//852103,
        devour = 0x0D0088,//852104,
        evileye = 0x0D0089,//852105,
        ensnare = 0x0D008A,//852106,
        ensnareon = 0x0D008B,//852107,
        ensnareoff = 0x0D008C,//852108,
        healingward = 0x0D008D,//852109,
        lightningshield = 0x0D008E,//852110,
        purge = 0x0D008F,//852111,
        standdown = 0x0D0091,//852113,
        stasistrap = 0x0D0092,//852114,
        chainlightning = 0x0D0097,//852119,
        earthquake = 0x0D0099,//852121,
        farsight = 0x0D009A,//852122,
        mirrorimage = 0x0D009B,//852123,
        shockwave = 0x0D009D,//852125,
        spiritwolf = 0x0D009E,//852126,
        stomp = 0x0D009F,//852127,
        whirlwind = 0x0D00A0,//852128,
        windwalk = 0x0D00A1,//852129,
        unwindwalk = 0x0D00A2,//852130,
        ambush = 0x0D00A3,//852131,
        autodispel = 0x0D00A4,//852132,
        autodispelon = 0x0D00A5,//852133,
        autodispeloff = 0x0D00A6,//852134,
        barkskin = 0x0D00A7,//852135,
        barkskinon = 0x0D00A8,//852136,
        barkskinoff = 0x0D00A9,//852137,
        bearform = 0x0D00AA,//852138,
        unbearform = 0x0D00AB,//852139,
        corrosivebreath = 0x0D00AC,//852140,
        loadarcher = 0x0D00AE,//852142,
        mounthippogryph = 0x0D00AF,//852143,
        cyclone = 0x0D00B0,//852144,
        detonate = 0x0D00B1,//852145,
        eattree = 0x0D00B2,//852146,
        entangle = 0x0D00B3,//852147,
        entangleinstant = 0x0D00B4,//852148,
        faeriefire = 0x0D00B5,//852149,
        faeriefireon = 0x0D00B6,//852150,
        faeriefireoff = 0x0D00B7,//852151,
        ravenform = 0x0D00BB,//852155,
        unravenform = 0x0D00BC,//852156,
        recharge = 0x0D00BD,//852157,
        rechargeon = 0x0D00BE,//852158,
        rechargeoff = 0x0D00BF,//852159,
        rejuvenation = 0x0D00C0,//852160,
        renew = 0x0D00C1,//852161,
        renewon = 0x0D00C2,//852162,
        renewoff = 0x0D00C3,//852163,
        roar = 0x0D00C4,//852164,
        root = 0x0D00C5,//852165,
        unroot = 0x0D00C6,//852166,
        entanglingroots = 0x0D00CB,//852171,
        flamingarrowstarg = 0x0D00CD,//852173,
        flamingarrows = 0x0D00CE,//852174,
        unflamingarrows = 0x0D00CF,//852175,
        forceofnature = 0x0D00D0,//852176,
        immolation = 0x0D00D1,//852177,
        unimmolation = 0x0D00D2,//852178,
        manaburn = 0x0D00D3,//852179,
        metamorphosis = 0x0D00D4,//852180,
        scout = 0x0D00D5,//852181,
        sentinel = 0x0D00D6,//852182,
        starfall = 0x0D00D7,//852183,
        tranquility = 0x0D00D8,//852184,
        acolyteharvest = 0x0D00D9,//852185,
        antimagicshell = 0x0D00DA,//852186,
        blight = 0x0D00DB,//852187,
        cannibalize = 0x0D00DC,//852188,
        cripple = 0x0D00DD,//852189,
        curse = 0x0D00DE,//852190,
        curseon = 0x0D00DF,//852191,
        curseoff = 0x0D00E0,//852192,
        freezingbreath = 0x0D00E3,//852195,
        possession = 0x0D00E4,//852196,
        raisedead = 0x0D00E5,//852197,
        raisedeadon = 0x0D00E6,//852198,
        raisedeadoff = 0x0D00E7,//852199,
        instant = 0x0D00E8,//852200,
        requestsacrifice = 0x0D00E9,//852201,
        restoration = 0x0D00EA,//852202,
        restorationon = 0x0D00EB,//852203,
        restorationoff = 0x0D00EC,//852204,
        sacrifice = 0x0D00ED,//852205,
        stoneform = 0x0D00EE,//852206,
        unstoneform = 0x0D00EF,//852207,
        unholyfrenzy = 0x0D00F1,//852209,
        unsummon = 0x0D00F2,//852210,
        web = 0x0D00F3,//852211,
        webon = 0x0D00F4,//852212,
        weboff = 0x0D00F5,//852213,
        wispharvest = 0x0D00F6,//852214,
        auraunholy = 0x0D00F7,//852215,
        auravampiric = 0x0D00F8,//852216,
        animatedead = 0x0D00F9,//852217,
        carrionswarm = 0x0D00FA,//852218,
        darkritual = 0x0D00FB,//852219,
        darksummoning = 0x0D00FC,//852220,
        deathanddecay = 0x0D00FD,//852221,
        deathcoil = 0x0D00FE,//852222,
        deathpact = 0x0D00FF,//852223,
        dreadlordinferno = 0x0D0100,//852224,
        frostarmor = 0x0D0101,//852225,
        frostnova = 0x0D0102,//852226,
        sleep = 0x0D0103,//852227,
        darkconversion = 0x0D0104,//852228,
        darkportal = 0x0D0105,//852229,
        fingerofdeath = 0x0D0106,//852230,
        firebolt = 0x0D0107,//852231,
        inferno = 0x0D0108,//852232,
        gold2lumber = 0x0D0109,//852233,
        lumber2gold = 0x0D010A,//852234,
        spies = 0x0D010B,//852235,
        rainofchaos = 0x0D010D,//852237,
        rainoffire = 0x0D010E,//852238,
        request_hero = 0x0D010F,//852239,
        disassociate = 0x0D0110,//852240,
        revenge = 0x0D0111,//852241,
        soulpreservation = 0x0D0112,//852242,
        coldarrowstarg = 0x0D0113,//852243,
        coldarrows = 0x0D0114,//852244,
        uncoldarrows = 0x0D0115,//852245,
        creepanimatedead = 0x0D0116,//852246,
        creepdevour = 0x0D0117,//852247,
        creepheal = 0x0D0118,//852248,
        creephealon = 0x0D0119,//852249,
        creephealoff = 0x0D011A,//852250,
        creepthunderbolt = 0x0D011C,//852252,
        creepthunderclap = 0x0D011D,//852253,
        poisonarrowstarg = 0x0D011E,//852254,
        poisonarrows = 0x0D011F,//852255,
        unpoisonarrows = 0x0D0120,//852256,
        resurrection = 0x0D013B,//852283,
        scrollofspeed = 0x0D013D,//852285,
        frostarmoron = 0x0D01EA,//852458,
        frostarmoroff = 0x0D01EB,//852459,
        awaken = 0x0D01F2,//852466,
        nagabuild = 0x0D01F3,//852467,
        mount = 0x0D01F5,//852469,
        dismount = 0x0D01F6,//852470,
        cloudoffog = 0x0D01F9,//852473,
        controlmagic = 0x0D01FA,//852474,
        magicdefense = 0x0D01FE,//852478,
        magicundefense = 0x0D01FF,//852479,
        magicleash = 0x0D0200,//852480,
        phoenixfire = 0x0D0201,//852481,
        phoenixmorph = 0x0D0202,//852482,
        spellsteal = 0x0D0203,//852483,
        spellstealon = 0x0D0204,//852484,
        spellstealoff = 0x0D0205,//852485,
        banish = 0x0D0206,//852486,
        drain = 0x0D0207,//852487,
        flamestrike = 0x0D0208,//852488,
        summonphoenix = 0x0D0209,//852489,
        ancestralspirit = 0x0D020A,//852490,
        ancestralspirittarget = 0x0D020B,//852491,
        corporealform = 0x0D020D,//852493,
        uncorporealform = 0x0D020E,//852494,
        disenchant = 0x0D020F,//852495,
        etherealform = 0x0D0210,//852496,
        unetherealform = 0x0D0211,//852497,
        spiritlink = 0x0D0213,//852499,
        unstableconcoction = 0x0D0214,//852500,
        healingwave = 0x0D0215,//852501,
        hex = 0x0D0216,//852502,
        voodoo = 0x0D0217,//852503,
        ward = 0x0D0218,//852504,
        autoentangle = 0x0D0219,//852505,
        autoentangleinstant = 0x0D021A,//852506,
        coupletarget = 0x0D021B,//852507,
        coupleinstant = 0x0D021C,//852508,
        decouple = 0x0D021D,//852509,
        grabtree = 0x0D021F,//852511,
        manaflareon = 0x0D0220,//852512,
        manaflareoff = 0x0D0221,//852513,
        phaseshift = 0x0D0222,//852514,
        phaseshifton = 0x0D0223,//852515,
        phaseshiftoff = 0x0D0224,//852516,
        phaseshiftinstant = 0x0D0225,//852517,
        taunt = 0x0D0228,//852520,
        vengeance = 0x0D0229,//852521,
        vengeanceon = 0x0D022A,//852522,
        vengeanceoff = 0x0D022B,//852523,
        vengeanceinstant = 0x0D022C,//852524,
        blink = 0x0D022D,//852525,
        fanofknives = 0x0D022E,//852526,
        shadowstrike = 0x0D022F,//852527,
        spiritofvengeance = 0x0D0230,//852528,
        absorb = 0x0D0231,//852529,
        avengerform = 0x0D0233,//852531,
        unavengerform = 0x0D0234,//852532,
        burrow = 0x0D0235,//852533,
        unburrow = 0x0D0236,//852534,
        devourmagic = 0x0D0238,//852536,
        flamingattacktarg = 0x0D023B,//852539,
        flamingattack = 0x0D023C,//852540,
        unflamingattack = 0x0D023D,//852541,
        replenish = 0x0D023E,//852542,
        replenishon = 0x0D023F,//852543,
        replenishoff = 0x0D0240,//852544,
        replenishlife = 0x0D0241,//852545,
        replenishlifeon = 0x0D0242,//852546,
        replenishlifeoff = 0x0D0243,//852547,
        replenishmana = 0x0D0244,//852548,
        replenishmanaon = 0x0D0245,//852549,
        replenishmanaoff = 0x0D0246,//852550,
        carrionscarabs = 0x0D0247,//852551,
        carrionscarabson = 0x0D0248,//852552,
        carrionscarabsoff = 0x0D0249,//852553,
        carrionscarabsinstant = 0x0D024A,//852554,
        impale = 0x0D024B,//852555,
        locustswarm = 0x0D024C,//852556,
        breathoffrost = 0x0D0250,//852560,
        frenzy = 0x0D0251,//852561,
        frenzyon = 0x0D0252,//852562,
        frenzyoff = 0x0D0253,//852563,
        mechanicalcritter = 0x0D0254,//852564,
        mindrot = 0x0D0255,//852565,
        neutralinteract = 0x0D0256,//852566,
        preservation = 0x0D0258,//852568,
        sanctuary = 0x0D0259,//852569,
        shadowsight = 0x0D025A,//852570,
        wardofshadowsight = 0x0D025A,//852570,
        spellshield = 0x0D025B,//852571,
        spellshieldaoe = 0x0D025C,//852572,
        spirittroll = 0x0D025D,//852573,
        steal = 0x0D025E,//852574,
        attributemodskill = 0x0D0260,//852576,
        blackarrow = 0x0D0261,//852577,
        blackarrowon = 0x0D0262,//852578,
        blackarrowoff = 0x0D0263,//852579,
        breathoffire = 0x0D0264,//852580,
        charm = 0x0D0265,//852581,
        doom = 0x0D0267,//852583,
        drunkenhaze = 0x0D0269,//852585,
        elementalfury = 0x0D026A,//852586,
        forkedlightning = 0x0D026B,//852587,
        howlofterror = 0x0D026C,//852588,
        manashieldon = 0x0D026D,//852589,
        manashieldoff = 0x0D026E,//852590,
        monsoon = 0x0D026F,//852591,
        silence = 0x0D0270,//852592,
        stampede = 0x0D0271,//852593,
        summongrizzly = 0x0D0272,//852594,
        summonquillbeast = 0x0D0273,//852595,
        summonwareagle = 0x0D0274,//852596,
        tornado = 0x0D0275,//852597,
        wateryminion = 0x0D0276,//852598,
        battleroar = 0x0D0277,//852599,
        channel = 0x0D0278,//852600,
        parasite = 0x0D0279,//852601,
        parasiteon = 0x0D027A,//852602,
        parasiteoff = 0x0D027B,//852603,
        submerge = 0x0D027C,//852604,
        unsubmerge = 0x0D027D,//852605,
        neutralspell = 0x0D0296,//852630,
        militiaunconvert = 0x0D02AB,//852651,
        clusterrockets = 0x0D02AC,//852652,
        robogoblin = 0x0D02B0,//852656,
        unrobogoblin = 0x0D02B1,//852657,
        summonfactory = 0x0D02B2,//852658,
        acidbomb = 0x0D02B6,//852662,
        chemicalrage = 0x0D02B7,//852663,
        healingspray = 0x0D02B8,//852664,
        transmute = 0x0D02B9,//852665,
        lavamonster = 0x0D02BB,//852667,
        soulburn = 0x0D02BC,//852668,
        volcano = 0x0D02BD,//852669,
    }
    public class orderid
    {
        public static OrderID Parse(string order)
        {
            if (string.IsNullOrEmpty(order)) return OrderID.none;

            try { return (OrderID)Enum.Parse(typeof(OrderID), order, true); }
            catch { return OrderID.none; }
        }
    }

    /*public class alliancetype
    {
        static Dictionary<int, string> ValueNamePairs = new Dictionary<int, string>();
        static alliancetype()
        {
            foreach (KeyValuePair<string, DHJassValue> kvp in DHJassExecutor.War3Globals)
                if (kvp.Key.StartsWith("ALLIANCE_"))
                    ValueNamePairs[kvp.Value.IntValue] = kvp.Key;                
        }
        public static void WakeUp() { }
        public static string getName(int value)
        {
            string name;
            ValueNamePairs.TryGetValue(value, out name);
            return name;
        }
    }*/
    public class itemtype
    {
        static Dictionary<string, int> NameValuePairs = new Dictionary<string, int>();
        static itemtype()
        {
            HabProperties hpsData = DHMpqDatabase.EditorDatabase["Data"]["itemClass"];

            int value;
            string name;

            // 00=Permanent,WESTRING_ITEMCLASS_PERMANENT
            // NumValues=7  

            foreach (KeyValuePair<string, object> kvp in hpsData)
                if (int.TryParse(kvp.Key, out value))
                {
                    name = (kvp.Value as string).Split(',')[0];
                    NameValuePairs.Add(name, value);
                }
        }
        public static void WakeUp() { }
        public static int getValue(string name)
        {
            if (name == null) 
                return 0;
            int value;
            NameValuePairs.TryGetValue(name, out value);
            return value;
        }
    }
    public class unittype
    {
        static Dictionary<int, string> ValueNamePairs = new Dictionary<int, string>();
        static unittype()
        {
            foreach (KeyValuePair<string, DHJassValue> kvp in DHJassExecutor.War3Globals)
                if (kvp.Key.StartsWith("UNIT_TYPE_"))
                    ValueNamePairs[kvp.Value.IntValue] = kvp.Key;
        }
        public static void WakeUp() { }
        public static string getName(int value)
        {
            string name;
            ValueNamePairs.TryGetValue(value, out name);
            return name;
        }
    }

    public class map
    {
        public static readonly int minX = -8192;
        public static readonly int minY = -8192;
        public static readonly int maxX = 8192;
        public static readonly int maxY = 8192;
    }
}
