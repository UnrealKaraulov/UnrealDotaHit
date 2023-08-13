using System.Collections.Generic;
using System.IO;
using System.Text;
using DotaHIT.Core.Resources;
using DotaHIT.Extras;

namespace Deerchao.War3Share.W3gParser
{
    internal static class ParserUtility
    {
        internal static string StringFromUInt(uint number)
        {
            StringBuilder sb = new StringBuilder(4);
            sb.Append((char)(byte)(number >> 24));
            sb.Append((char)(byte)(number >> 16));
            sb.Append((char)(byte)(number >> 8));
            sb.Append((char)(byte)number);
            string result = sb.ToString();
            result = string.Intern(result);
            switch (result)
            {
                case "AUfu":
                    return "AUfa";
                case "ANc1":
                case "ANc2":
                case "ANc3":
                    return "ANcs";
                case "ANs1":
                case "ANs2":
                case "ANs3":
                    return "ANsy";
                case "ANg1":
                case "ANg2":
                case "ANg3":
                    return "ANrg";
                case "ANia":
                    return "ANic";

                case "ospm":
                    return "ospw";
                case "otbk":
                    return "ohun";
                case "hrtt":
                    return "hmtt";
                default:
                    return result;
            }
        }

        internal static string ReadString(BinaryReader reader)
        {
            List<byte> bytes = new List<byte>();
            byte b = reader.ReadByte();
            while (b != 0)
            {
                bytes.Add(b);
                try
                {
                    b = reader.ReadByte();
                }
                catch
                {
                    break;
                }
            }
            return Encoding.UTF8.GetString(bytes.ToArray());
        }
        internal static string ReadString(BinaryReader reader, out int bytesRead)
        {
            List<byte> bytes = new List<byte>();
            byte b = reader.ReadByte();
            while (b != 0)
            {
                bytes.Add(b);
                b = reader.ReadByte();
            }
            bytesRead = bytes.Count+1;
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        internal static byte[] GetDecodedBytes(BinaryReader reader)
        {
            List<byte> decoded = new List<byte>();
            int pos = 0;
            byte mask = 0;

            byte b = reader.ReadByte();
            while (b != 0)
            {
                if (pos % 8 == 0)
                {
                    mask = b;
                }
                else
                {
                    if ((mask & (0x1 << (pos % 8))) == 0)
                        decoded.Add((byte)(b - 1));
                    else
                        decoded.Add(b);
                }

                b = reader.ReadByte();
                pos++;
            }
            return decoded.ToArray();
        }

        internal static uint GetUInt(byte[] data, int startIndex)
        {
            uint checksum = data[startIndex];
            checksum |= (uint)data[startIndex + 1] << 8;
            checksum |= (uint)data[startIndex + 2] << 16;
            checksum |= (uint)data[startIndex + 3] << 24;
            return checksum;
        }

        internal static string GetString(byte[] decoded, int startIndex)
        {
            List<byte> bytes = new List<byte>();
            while (decoded[startIndex] != 0)
            {
                bytes.Add(decoded[startIndex]);
                startIndex++;
            }
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        #region UnitType
        #region strings
        static ParserUtility()
        {
            #region skill and hero
            /*skillHeroes = new Dictionary<string, string>(107);
            skillHeroes.Add("AHbz", "Hamg");
            skillHeroes.Add("AHwe", "Hamg");
            skillHeroes.Add("AHab", "Hamg");
            skillHeroes.Add("AHmt", "Hamg");
            skillHeroes.Add("AHtb", "Hmkg");
            skillHeroes.Add("AHtc", "Hmkg");
            skillHeroes.Add("AHbh", "Hmkg");
            skillHeroes.Add("AHav", "Hmkg");
            skillHeroes.Add("AHhb", "Hpal");
            skillHeroes.Add("AHds", "Hpal");
            skillHeroes.Add("AHad", "Hpal");
            skillHeroes.Add("AHre", "Hpal");
            skillHeroes.Add("AHdr", "Hblm");
            skillHeroes.Add("AHfs", "Hblm");
            skillHeroes.Add("AHbn", "Hblm");
            skillHeroes.Add("AHpx", "Hblm");
            skillHeroes.Add("AEmb", "Edem");
            skillHeroes.Add("AEim", "Edem");
            skillHeroes.Add("AEev", "Edem");
            skillHeroes.Add("AEme", "Edem");
            skillHeroes.Add("AEer", "Ekee");
            skillHeroes.Add("AEfn", "Ekee");
            skillHeroes.Add("AEah", "Ekee");
            skillHeroes.Add("AEtq", "Ekee");
            skillHeroes.Add("AEst", "Emoo");
            skillHeroes.Add("AHfa", "Emoo");
            skillHeroes.Add("AEar", "Emoo");
            skillHeroes.Add("AEsf", "Emoo");
            skillHeroes.Add("AEbl", "Ewar");
            skillHeroes.Add("AEfk", "Ewar");
            skillHeroes.Add("AEsh", "Ewar");
            skillHeroes.Add("AEsv", "Ewar");
            skillHeroes.Add("AOwk", "Obla");
            skillHeroes.Add("AOmi", "Obla");
            skillHeroes.Add("AOcr", "Obla");
            skillHeroes.Add("AOww", "Obla");
            skillHeroes.Add("AOcl", "Ofar");
            skillHeroes.Add("AOfs", "Ofar");
            skillHeroes.Add("AOsf", "Ofar");
            skillHeroes.Add("AOeq", "Ofar");
            skillHeroes.Add("AOsh", "Otch");
            skillHeroes.Add("AOae", "Otch");
            skillHeroes.Add("AOws", "Otch");
            skillHeroes.Add("AOre", "Otch");
            skillHeroes.Add("AOhw", "Oshd");
            skillHeroes.Add("AOhx", "Oshd");
            skillHeroes.Add("AOsw", "Oshd");
            skillHeroes.Add("AOvd", "Oshd");
            skillHeroes.Add("AUdc", "Udea");
            skillHeroes.Add("AUdp", "Udea");
            skillHeroes.Add("AUau", "Udea");
            skillHeroes.Add("AUan", "Udea");
            skillHeroes.Add("AUcs", "Udre");
            skillHeroes.Add("AUsl", "Udre");
            skillHeroes.Add("AUav", "Udre");
            skillHeroes.Add("AUin", "Udre");
            skillHeroes.Add("AUfn", "Ulic");
            skillHeroes.Add("AUfa", "Ulic");
            skillHeroes.Add("AUfu", "Ulic");
            skillHeroes.Add("AUdr", "Ulic");
            skillHeroes.Add("AUdd", "Ulic");
            skillHeroes.Add("AUim", "Ucrl");
            skillHeroes.Add("AUts", "Ucrl");
            skillHeroes.Add("AUcb", "Ucrl");
            skillHeroes.Add("AUls", "Ucrl");
            skillHeroes.Add("ANbf", "Npbm");
            skillHeroes.Add("ANdb", "Npbm");
            skillHeroes.Add("ANdh", "Npbm");
            skillHeroes.Add("ANef", "Npbm");
            skillHeroes.Add("ANdr", "Nbrn");
            skillHeroes.Add("ANsi", "Nbrn");
            skillHeroes.Add("ANba", "Nbrn");
            skillHeroes.Add("ANch", "Nbrn");
            skillHeroes.Add("ANms", "Nngs");
            skillHeroes.Add("ANfa", "Nngs");
            skillHeroes.Add("ANfl", "Nngs");
            skillHeroes.Add("ANto", "Nngs");
            skillHeroes.Add("ANrf", "Nplh");
            skillHeroes.Add("ANca", "Nplh");
            skillHeroes.Add("ANht", "Nplh");
            skillHeroes.Add("ANdo", "Nplh");
            skillHeroes.Add("ANsg", "Nbst");
            skillHeroes.Add("ANsq", "Nbst");
            skillHeroes.Add("ANsw", "Nbst");
            skillHeroes.Add("ANst", "Nbst");
            skillHeroes.Add("ANeg", "Ntin");
            skillHeroes.Add("ANcs", "Ntin");
            skillHeroes.Add("ANc1", "Ntin");
            skillHeroes.Add("ANc2", "Ntin");
            skillHeroes.Add("ANc3", "Ntin");
            skillHeroes.Add("ANsy", "Ntin");
            skillHeroes.Add("ANs1", "Ntin");
            skillHeroes.Add("ANs2", "Ntin");
            skillHeroes.Add("ANs3", "Ntin");
            skillHeroes.Add("ANrg", "Ntin");
            skillHeroes.Add("ANg1", "Ntin");
            skillHeroes.Add("ANg2", "Ntin");
            skillHeroes.Add("ANg3", "Ntin");
            skillHeroes.Add("ANic", "Nfir");
            skillHeroes.Add("ANia", "Nfir");
            skillHeroes.Add("ANso", "Nfir");
            skillHeroes.Add("ANlm", "Nfir");
            skillHeroes.Add("ANvc", "Nfir");
            skillHeroes.Add("ANhs", "Nalc");
            skillHeroes.Add("ANab", "Nalc");
            skillHeroes.Add("ANcr", "Nalc");
            skillHeroes.Add("ANtm", "Nalc");*/
            #endregion

            #region units

            /*units =
                new List<string>(
                    new string[]
                        {
                            "hfoo", "hkni", "hmpr", "hmtm", "hpea", "hrif", "hsor", "hmtt", "hgry", "hgyr", "hspt", "hdhw",
                            "ebal", "echm", "edoc", "edot", "ewsp", "esen", "earc",
                            "edry", "ehip", "emtg", "efdr", "ocat", "odoc", "ogru", "ohun", "okod", "opeo",
                            "orai", "oshm", "otau", "owyv", "ospw", "otbr", "uaco", "uabo", "uban", "ucry",
                            "ufro", "ugar", "ugho", "unec", "umtw", "ushd", "uobs", "ubsp", 
                            "nskm", "nskf", "nws1", "nban", "nrog", "nenf", "nass", "nbdk", "nrdk", "nbdr", "nrdr", "nbwm",
                            "nrwm", "nadr", "nadw", "nadk", "nbzd", "nbzk", "nbzw", "ngrd", "ngdk", "ngrw", "ncea", "ncen",
                            "ncer", "ndth", "ndtp", "ndtb", "ndtw", "ndtr", "ndtt", "nfsh", "nfsp", "nftr", "nftb", "nftt",
                            "nftk", "ngrk", "ngir", "nfrs", "ngna", "ngns", "ngno", "ngnb", "ngnw", "ngnv", "ngsp", "nhrr",
                            "nhrw", "nits", "nitt", "nkob", "nkog", "nthl", "nmfs", "nmrr", "nowb", "nrzm", "nnwa", "nnwl",
                            "nogr", "nogm", "nogl", "nomg", "nrvs", "nslf", "nsts", "nstl", "nzep", "ntrt", "nlds", "nlsn",
                            "nmsn", "nscb", "nbot", "nsc2", "nsc3", "nbdm", "nmgw", "nanb", "nanm", "nfps", "nmgv", "nitb",
                            "npfl", "ndrd", "ndrm", "nvdw", "nvdg", "nnht", "nndk", "nndr"
                        });*/

            #endregion

            #region buildingUpgrade
            buildingUpgrades =
                new List<string>(
                    new string[]
                        {
                            "hkee", "hcas", "hctw", "hgtw", "hatw", "etoa", "etoe", "ofrt", "ostr", "unp1", "unp2", "uzg1",
                            "uzg2"
                        });
            #endregion

            #region building

            buildings =
                new List<string>(
                    new string[]
                        {
                            "halt", "harm", "hars", "hbar", "hbla", "hhou", "hgra", "hwtw", "hvlt", "hlum", "htow", "etrp",
                            "etol", "edob", "eate", "eden", "eaoe", "eaom", "eaow", "edos", "emow", "oalt", "obar", "obea",
                            "ofor", "ogre", "osld", "otrb", "orbr", "otto", "ovln", "owtw", "uaod", "unpl", "usep", "utod",
                            "utom", "ugol", "uzig", "ubon", "usap", "uslh", "ugrv"
                        });
            #endregion

            #region item

            /*items =
                new List<string>(
                    new string[]
                        {
                            "amrc", "ankh", "belv", "bgst", "bspd", "ccmd", "ciri", "ckng", "clsd", "crys", "desc", "gemt",
                            "gobm", "gsou", "guvi", "gfor", "soul", "mdpb", "rag1", "rat3", "rin1", "rde1", "rde2", "rde3",
                            "rhth", "rst1", "ofir", "ofro", "olig", "oli2", "oven", "odef", "ocor", "pdiv", "phea", "pghe",
                            "pinv", "pgin", "pman", "pgma", "pnvu", "pnvl", "pres", "pspd", "rlif", "rwiz", "sfog", "shea",
                            "sman", "spro", "sres", "ssil", "stwp", "tels", "tdex", "texp", "tint", "tkno", "tstr", "ward",
                            "will", "wneg", "rdis", "rwat", "fgrd", "fgrg", "fgdg", "fgfh", "fgsk", "engs", "k3m1", "modt",
                            "sand", "srrc", "sror", "infs", "shar", "wild", "wswd", "whwd", "wlsd", "wcyc", "rnec", "pams",
                            "clfm", "evtl", "nspi", "lhst", "kpin", "sbch", "afac", "ajen", "lgdh", "hcun", "mcou", "hval",
                            "cnob", "prvt", "tgxp", "mnst", "hlst", "tpow", "tst2", "tin2", "tdx2", "rde0", "rde4", "rat6",
                            "rat9", "ratc", "ratf", "manh", "pmna", "penr", "gcel", "totw", "phlt", "gopr", "ches", "mlst",
                            "rnsp", "brag", "sksh", "vddl", "sprn", "tmmt", "anfg", "lnrn", "iwbr", "jdrn", "drph", "hslv",
                            "pclr", "plcl", "rej1", "rej2", "rej3", "rej4", "rej5", "rej6", "sreg", "gold", "lmbr", "fgun",
                            "pomn", "gomn", "wneu", "silk", "lure", "skul", "moon", "brac", "vamp", "woms", "tcas", "tgrh",
                            "tsct", "wshs", "tret", "sneg", "stel", "spre", "mcri", "spsh", "sbok", "ssan", "shas", "dust",
                            "oslo", "dsum", "sor1", "sor2", "sor3", "sor4", "sor5", "sor6", "sor7", "sor8", "sor9", "sora",
                            "sorf", "fwss", "ram1", "ram2", "ram3", "ram4", "shtm", "shwd", "btst", "skrt", "thle", "sclp",
                            "gldo", "tbsm", "tfar", "tlum", "tbar", "tbak", "mgtk", "stre", "horl", "hbth", "blba", "rugt",
                            "frhg", "gvsm", "crdt", "arsc", "scul", "tmsc", "dtsb", "grsl", "arsh", "shdt", "shhn", "shen",
                            "thdm", "stpg", "shrs", "bfhr", "cosl", "shcw", "srbd", "frgd", "envl", "rump", "mort", "srtl",
                            "stwa", "klmm", "rots", "axas", "mnsf", "schl", "asbl", "kgal", "dphe", "dkfw", "dthb"
                        });*/
            #endregion

            #region researches

            researches = new List<string>(new string[]
                                              {
                                                  "Rhss", "Rhme", "Rhra", "Rhar", "Rhla", "Rhac", "Rhgb", "Rhlh", "Rhde"
                                                  , "Rhan", "Rhpt", "Rhst", "Rhri", "Rhse", "Rhfl", "Rhhb", "Rhrt",
                                                  "Rhpm", "Rhfc", "Rhfs", "Rhcd", "Resm", "Resw", "Rema", "Rerh", "Reuv"
                                                  , "Renb", "Reib", "Remk", "Resc", "Remg", "Redt", "Redc", "Resi",
                                                  "Reht", "Recb", "Repb", "Rers", "Rehs", "Reeb", "Reec", "Rews", "Repm"
                                                  , "Roch", "Rome", "Rora", "Roar", "Rwdm", "Ropg", "Robs", "Rows",
                                                  "Roen", "Rovs", "Rowd", "Rost", "Rosp", "Rotr", "Rolf", "Ropm", "Rowt"
                                                  , "Robk", "Rorb", "Robf", "Rusp", "Rume", "Rura", "Ruar", "Rucr",
                                                  "Ruac", "Rugf", "Ruwb", "Rusf", "Rune", "Ruba", "Rufb", "Rusl", "Rupc"
                                                  , "Rusm", "Rubu", "Ruex", "Rupm"
                                              });
            #endregion
        }
        #endregion

        //private static readonly Dictionary<string, string> skillHeroes;
        //private static readonly List<string> units;
        private static readonly List<string> buildingUpgrades;
        private static readonly List<string> buildings;
        //private static readonly List<string> items;
        private static readonly List<string> researches;

        internal static ItemType ItemTypeFromId(ReplayMapCache cache, string stringId)
        {
            if (cache.hpcAbilityData == null) return ItemType.None;

            //if (skillHeroes.ContainsValue(stringId))
            if (cache.dcHeroesTaverns.ContainsKey(stringId))
                return ItemType.Hero;

            //if (skillHeroes.ContainsKey(stringId))
            if (cache.dcAbilitiesHeroes.ContainsKey(stringId))
                return ItemType.HeroAbility;

            if (cache.hpcAbilityData.ContainsKey(stringId))
                return ItemType.Ability;

            //if (units.Contains(stringId))
            if (cache.hpcUnitProfiles.ContainsKey(stringId))
            {
                if (cache.IsNewVersionItem(stringId))
                    return ItemType.NewVersionItem;
                else
                    return ItemType.Unit;
            }

            if (buildingUpgrades.Contains(stringId))
            {
                return ItemType.Upgrade;
            }

            if (buildings.Contains(stringId))
                return ItemType.Building;

            //if (items.Contains(stringId))
            if (cache.hpcItemProfiles.ContainsKey(stringId))
                return ItemType.Item;

            if (researches.Contains(stringId))
            {
                return ItemType.Research;
            }

            return ItemType.None;
        }

        /*internal static string GetHeroByAbility(string abilityName)
        {
            return DHLOOKUP.dcAbilitiesHeroes[abilityName];
        }*/

        internal static int GetItemStockRegen(ReplayMapCache cache, string itemID, bool isNewVersionItem)
        {
            object value = isNewVersionItem ? cache.hpcUnitBalance[itemID, "stockRegen"] : cache.hpcItemData[itemID, "stockRegen"];
            return (value == null) ? 0 : int.Parse(value+"") * 1000;
        }

        internal static string GetBestMatchAbilityForHero(ReplayMapCache cache, Hero hero, string abilityId, ItemType type)
        {
            switch (type)
            {
                case ItemType.HeroAbility:
                    return abilityId;

                case ItemType.Ability:
                    string similarAbilityId = DHHELPER.GetSimilarAbilityFromHero(cache.hpcAbilityData, cache.hpcUnitAbilities, hero.Name, abilityId);
                    return (similarAbilityId == null) ? abilityId : similarAbilityId;

                default:
                    return abilityId;
            }
        }

        internal static IEnumerable<string> GetAbilitiesByHero(ReplayMapCache cache, string heroName)
        {
            return cache.hpcUnitAbilities[heroName].GetStringListValue("heroAbilList");            
        }        
        #endregion
    }
}