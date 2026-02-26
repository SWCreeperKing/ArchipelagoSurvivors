using Il2CppVampireSurvivors.Data;
using static Il2CppVampireSurvivors.Data.CharacterType;
using static Il2CppVampireSurvivors.Data.EnemyType;
using static Il2CppVampireSurvivors.Data.StageType;

namespace ArchipelagoSurvivors;

public class test(string name) : Attribute
{
    public readonly string Name = name;
}

public enum e {
    [test("abc")]  a,
    [test("bca")]  b
}

public static class InformationTransformer
{
    public static readonly Dictionary<StageType, string> StageTypeToName = new()
    {
        [FOREST] = "Mad Forest", [LIBRARY] = "Inlaid Library", [WAREHOUSE] = "Dairy Plant", [TOWER] = "Gallo Tower",
        [CHAPEL] = "Cappella Magna", [MOLISE] = "Il Molise", [SINKING] = "Moongolow", [GREENACRES] = "Green Acres",
        [BONEZONE] = "The Bone Zone", [RASH] = "Boss Rash", [MACHINE] = "Eudaimonia M.", [WHITEOUT] = "Whiteout",
        [COOP] = "The Coop", [SPAZIE] = "Space 54", [CARLOCART] = "Carlo Cart", [LABORRATORY] = "Laborratory",
        [BATCOUNTRY] = "Bat Country", [ASTRALSTAIR] = "Astral Stair", [TOWERBRIDGE] = "Tiny Bridge",
        [DEVILROOM] = "Room 1665", [MOONSPELL] = "Mt.Moonspell", [FOSCARI] = "Lake Foscari",
        [FOSCARI2] = "Abyss Foscari", [POLUS] = "Polus Replica", [FB_GALUGA] = "Neo Galuga",
        [FB_HIGHWAY] = "Hectic Highway", [TP_CASTLE] = "Ode to Castlevania", [EMERALD] = "Emerald Diorama",
    };

    public static string GetName(this StageType enums)
    {
        return StageTypeToName[enums];
    }

    public static Dictionary<string, StageType> StageNameToType =
        StageTypeToName.ToDictionary(kv => kv.Value, kv => kv.Key);

    public static readonly Dictionary<CharacterType, string> CharacterTypeToName = new()
    {
        [ANTONIO] = "Antonio Belpaese", [IMELDA] = "Imelda Belpaese", [PASQUALINA] = "Pasqualina Belpaese",
        [GENNARO] = "Gennaro Belpaese", [CIRO] = "Arca Ladonna", [PORTA] = "Porta Ladonna", [LAMA] = "Lama Ladonna",
        [CAMILLO] = "Poe Ratcho", [GERMANA] = "Suor Clerici", [DOMMARIO] = "Dommario", [CROCI] = "Krochi Freetto",
        [CRISTINA] = "Christine Davain", [PUGNALA] = "Pugnala Provola", [GIOVANNA] = "Giovanna Grana",
        [POPPEA] = "Poppea Pecorina", [CONCETTA] = "Concetta Caciotta", [MORTACCIO] = "Mortaccio",
        [CAVALLO] = "Yatta Cavallo", [MARIA] = "Bianca Ramba", [TATANKA] = "O'Sole Meeo", [AMBROGIO] = "Sir Ambrojoe",
        [PINO] = "Iguana Gallo Valleto", [FEBBRA] = "Divano Thelma", [ASSUNTA] = "Zi'Assunta Belpaese",
        [EXDASH] = "Exdash Exiviiq", [PANINI] = "Toastie", [SMITH] = "Smith IV", [ARENGIJUS] = "Random",
        [NEO] = "Boon Marrabbio", [AVATAR] = "Avatar Infernas", [GRAZIELLA] = "Minnah Mannarah", [VERANDA] = "Leda",
        [PAVONE] = "Cosmo Pavone", [PEPPINO] = "Peppino", [PANTALONE] = "Big Trouser", [FINO] = "MissingN▯",
        [BOROS] = "Gains Boros", [DRAGOGION] = "Gyorunton", [NOSTRO] = "Mask of the Red Death", [SIGMA] = "Queen Sigma",
        [TUPU] = "Bat Robbert", [SHEMOONITA] = "She-Moon Eeta", [SANTA] = "Santa Ladonna", [YOLO] = "Gazebo",
        [SPACEDUDE] = "Space Dude", [BATSBATSBATS] = "Bats Bats Bats", [ROSE] = "Rose De Infernas",
        [SCOREJ] = "Scorej-Oni", [GYORUNTIN] = "Gyoruntin", [SPACEDUDETTE] = "Space Dette", [MIANG] = "Miang Moonspell",
        [MENYA] = "Menya Moonspell", [SYUUTO] = "Syuuto Moonspell", [ONNA] = "Babi-Onna", [MCCOY] = "McCoy-Oni",
        [MEGAMENYA] = "Megalo Menya Moonspell", [MEGASYUUTO] = "Megalo Syuuto Moonspell", [TONY] = "Gav'Et-Oni",
        [ELEANOR] = "Eleanor Uziron", [VICTOR] = "Maruto Cuts", [KEIRA] = "Keitha Muort",
        [LUMINAIRE] = "Luminaire Foscari", [GENEVIEVE] = "Genevieve Gruyére", [MEGAGENEVIEVE] = "Je-Ne-Viv",
        [CTRPCAKE] = "Sammy", [GHOUL] = "Rottin'Ghoul", [C1_CREWMATE] = "Crewmate Dino",
        [C1_ENGINEER] = "Engineer Gino", [C1_GHOST] = "Ghost Lino", [C1_SHAPESHIFTER] = "Shapeshifter Nino",
        [C1_GUARDIAN] = "Guardian Pina", [C1_IMPOSTOR] = "Impostor Rina", [C1_SCIENTIST] = "Scientist Mina",
        [C1_HORSE] = "Horse", [C1_MEGAIMPOSTOR] = "Megalo Impostor Rina", [FB_BILLRIZER] = "Bill Rizer",
        [FB_LANCE] = "Lance Bean", [FB_ARIANA] = "Ariana", [FB_LUCIA] = "Lucia Zero", [FB_BRADFANG] = "Brad Fang",
        [FB_BROWNY] = "Browny", [FB_SHEENA] = "Sheena Etranzi", [FB_PROBO] = "Probotector", [FB_STANLEY] = "Stanley",
        [FB_NEWT] = "Newt Plissken", [FB_COLONEL] = "Colonel Bahamut", [CharacterType.FB_SIMONDO] = "Simondo Belmont",
        [TP_LEON] = "Leon Belmont", [TP_SONIA] = "Sonia Belmont", [TP_TREVOR] = "Trevor Belmont",
        [TP_CHRISTOPHER] = "Christopher Belmont", [TP_SIMON] = "Simon Belmont", [TP_JUSTE] = "Juste Belmont",
        [TP_RICHTER] = "Richter Belmont", [TP_JULIUS] = "Julius Belmont", [TP_GRANT] = "Grant Danasty",
        [TP_QUINCY] = "Quincy Morris", [TP_JOHN] = "John Morris", [TP_JONATHAN] = "Jonathan Morris",
        [TP_MAXIM] = "Maxim Kischine", [TP_HENRY] = "Henry", [TP_SOMA] = "Soma Cruz",
        [TP_DRACULA] = "Vlad Tepes Dracula", [TP_CHARLOTTE] = "Charlotte Aulin", [TP_SYPHA] = "Sypha Belnades",
        [TP_JULIA] = "Julia Laforeze", [TP_CARRIE] = "Carrie Fernandez", [TP_YOKO] = "Yoko Belnades",
        [TP_RINALDO] = "Rinaldo Gandolfi", [TP_MINA] = "Mina Hakuba", [TP_ELIZABETH] = "Elizabeth Bartley",
        [TP_ALUCARD] = "Alucard", [TP_REINHARDT] = "Reinhardt Schneider", [TP_ERIC] = "Eric Lecarde",
        [TP_ISAAC] = "Isaac", [TP_HECTOR] = "Hector", [TP_SARA] = "Sara Trantoul", [TP_VINCENT] = "Vincent Dorin",
        [TP_MARIAA] = "Maria Renard", [TP_SHANOA] = "Shanoa", [TP_ALBUS] = "Albus", [TP_LISA] = "Lisa Tepes",
        [TP_SHAFT] = "Shaft", [TP_STGERMAIN] = "Saint Germain", [TP_NATHAN] = "Nathan Graves", [TP_CORNELL] = "Cornell",
        [TP_BARLOWE] = "Barlowe", [TP_MARIAB] = "Young Maria Renard", [TP_FAMILIARS] = "Familiar",
        [TP_INNOCENT_DEVILS] = "Innocent Devil", [TP_CORNELL_BCM] = "Blue Crescent Moon Cornell",
        [TP_FERRYMAN] = "Ferryman", [TP_LIBRARIAN] = "Master Librarian", [TP_HAMMER] = "Hammer", [TP_WIND] = "Wind",
        [TP_JONATHAN_AND_CHARLOTTE] = "Jonathan & Charlotte", [TP_CHARLOTTE_AND_JONATHAN] = "Charlotte & Jonathan",
        [TP_STELLA_AND_LORETTA] = "Stella & Loretta Lecarde", [TP_LORETTA_AND_STELLA] = "Loretta & Stella Lecarde",
        [TP_STELLA] = "Stella Lecarde", [TP_LORETTA] = "Loretta Lecarde", [TP_BRAUNER] = "Brauner",
        [TP_SOLEIL] = "Soleil Belmont", [TP_DARIO] = "Dario Bossi", [TP_DMITRI] = "Dmitrii Blinov",
        [TP_CELIA] = "Celia Fortner", [TP_GRAHAM] = "Graham Jones", [TP_JOACHIM] = "Joachim Armster",
        [TP_WALTER] = "Walter Bernhard", [TP_CARMILLA] = "Carmilla", [TP_OLROX] = "Count Olrox",
        [CharacterType.TP_CAVETROLL] = "Cave Troll", [CharacterType.TP_FLEAMAN] = "Fleaman",
        [TP_AXEARMOR] = "Axe Armor",
        [CharacterType.TP_FROZENSHADE] = "Frozenshade", [CharacterType.TP_SUCCUBUS] = "Succubus",
        [TP_KEREMET] = "Keremet",
        [TP_SNIPER] = "Alamaric Sniper", [TP_BLACKMORE] = "Blackmore", [TP_MALPHAS] = "Malphas", [TP_DEATH] = "Death",
        [TP_GALAMOTH] = "Galamoth", [TP_ELIZABETH_MEGA] = "Megalo Elizabeth Bartley", [TP_OLROX_MEGA] = "Megalo Olrox",
        [TP_DEATH_MEGA] = "Megalo Death", [TP_DRACULA_MEGA] = "Megalo Dracula", [TP_CHAOS] = "Chaos",
        [EME_RAPIERDUAL] = "Tsunanori Mido", [EME_PUNCHKICK] = "Bonnie Blair", [EME_CANNONGUN] = "Formina Franklyn",
        [EME_MECHKATANA] = "Diva No. 5", [EME_MAGICALL] = "Ameya Aisling", [EME_SWORDBITE] = "Siugnas",
        [EME_EXGREATSWORD] = "Final Emperor", [EME_EXAXE] = "Dolores", [EME_EXKNIFE] = "Macha Alter Ego",
        [EME_EXSPEAR] = "Lita Caryx", [EME_PUPALL] = "Kugutsu", [EME_MRS] = "Mr. S",
        [EME_CATS] = "Lolo, Hiss, Meow, and Purr", [EME_KINA] = "Kina", [EME_IMAKOO] = "Imakoo",
        [EME_DEMON] = "Malevolent Door Spirit",
    };

    public static readonly Dictionary<string, CharacterType> CharacterNameToType =
        CharacterTypeToName.ToDictionary(kv => kv.Value, kv => kv.Key);

    public static readonly Dictionary<EnemyType, string> EnemyTypeToName = new()
    {
        [BAT1] = "Pipeestrello", [BAT6] = "Bloodbath", [SKULLINO] = "Skullino", [SKULOROSSO] = "Skulorosso",
        [SKELANGUE] = "Scarleton", [SKELETON] = "Skeleton", [ZOMBIE] = "Zombie", [MUDMAN1] = "Mudman",
        [FLOWER] = "Flower Wall", [GHOST] = "Ghost", [WEREWOLF] = "Werewolf", [MUD] = "Dust Elemental",
        [BUER] = "Lionhead", [MILK] = "Milk Elemental", [XLDRAGON1] = "Dragon Shrimp", [SHADERED] = "Sig.ra Rossi",
        [POLTER] = "Poltergeist", [BOSS_COUNT1] = "Hag", [BOSS_COUNT2] = "Nesufritto", [MUMMY] = "Mummy",
        [MEDUSA1] = "Sneaky Head", [HARPY] = "Harzia", [ECTO1] = "Musc Musc", [IMP] = "Impefinger",
        [DULL0] = "Testa di Mano", [DEVIL1] = "Ghiavolo", [XLMAGIO] = "Undead Mage", [WITCH1] = "Undead Witch",
        [WITCH2] = "Undead Sassy Witch", [XLARMOR1] = "Archon Spada", [XLARMOR2] = "Archon Disco",
        [XLMEDUSA] = "Merdusa", [XLBAT] = "Giant Bat", [XLMANTIS] = "Mantichana", [XLMUMMY] = "Big Mummy",
        [XLFLOWER] = "Venus", [FISHMAN_1] = "Merman", [LIZARD1_2] = "Lizard Pawn", [PILE1] = "Twin Snakes",
        [LIZARD2_3] = "Lizard Rook", [PILE2] = "Twin Demons", [JELLYFISH] = "Jellyfish", [SKELENIN1] = "Skeleton Ninja",
        [PILE3] = "Lost Twin", [GOLEM1] = "Melone", [MIGNO1_5] = "Minotaur", [MIGNO_3_5SWARM] = "Mignotaur",
        [ARMORSPEAR_6] = "Archon Lancia", [ARMOR_6] = "Archon Ascia", [SKELEWING] = "Skelewing", [XLTRITON] = "Tritont",
        [XLCHIMERA] = "Manticore", [XLCOCKATRICE] = "Gallotrice", [XLGOLEM1] = "Big Golem", [XLGOLEM3] = "Meat Golem",
        [XLARMOR_SWORD] = "Sword Guardian", [BOSS_XLCRAB] = "Giant Enemy Crab", [MOLISANO_BASE] = "Sad Molisano",
        [MOLISANO_GIALLO] = "Happy Molisano", [MOLISANO_BELLO] = "Cute Molisano", [MOLISANO_COLONNA] = "Old Molisano",
        [MOLISANO_VECCHIO] = "Dead Molisano", [EX_SNOWMAN_HAUNTED] = "Bambaman", [EX_SPIRIT1] = "Miragellos",
        [EX_GOLEM_ICE] = "Menta Elemental", [EX_ILLUSION] = "Madd-Onna", [EX_BOSS_KITSUNE] = "Kizzune",
        [EX_RETROBOT] = "Holy Circuit Creations", [EX_BOUNTYHUNTER] = "Bounty Hunter", [EX_APEX] = "Space Hunter",
        [EX_GYORUNTIN] = "Tri-Blunder", [COOP_CHICKEN] = "Chickenfantry", [COOP_COCK] = "Cockreliutennant",
        [COOP_CHIK] = "Chik", [COOP_EGG] = "Egge", [COOP_ROAST_CHICKEN] = "Pol.lo Rosso", [COOP_ABRAXAS] = "Abraxas",
        [COOP_ABRAXASCROWN] = "Abraxas Phronesis", [COOP_ABRAXASHALO] = "Abraxas Dynamis",
        [EX_BATSPACE_ALL4] = "Gala Invader", [EX_RABBIT_SWARM] = "Moon Rabbit", [EX_DUCK_SWARM] = "Moon Duck",
        [EX_ANT] = "Space Ant Onion", [EX_PICKLE] = "Space Pickle", [EX_PHALIEN_XL] = "ECMASlime",
        [EX_CUBE_HITS_STERILE] = "Sinistronz", [PILE4] = "Twin Skulls", [SKULLNOAURA] = "Skullone",
        [SKELEPANTHER] = "Skeleton Panther", [XLSKELETON] = "Giant Skeleton", [SKELETONE] = "Skeletone",
        [SKETAMARI] = "Sketamari", [MASK_GOLD] = "Sun Atlantean", [MASK_SILVER] = "Moon Atlantean",
        [MASK_LEFT] = "City Atlantean", [MASK_RIGHT] = "Volcano Atlantean", [MOON_MASK3] = "Moongolow Atlanteans",
        [MOON_SNEK] = "Serpentvine", [MOON_GARLIC] = "Garlic", [MOON_NIGHTSHADE] = "Nightshade",
        [MOON_SHADEBLUE] = "Sig.ra Blu", [MOON_CRABBINO] = "Non-Giant Enemy Crab", [MOON_EYE2] = "Unknown",
        [TRAINEE_ANY] = "Reaper Trainee", [KALI1] = "Tetrabrachia", [ARMOR_FIRE] = "Archon Fiamma",
        [SUCCUBUS] = "Succubus", [XLARMOR_GREEN] = "Archon Rame", [DEMON_FAST] = "Demon Priest",
        [ANGEL1] = "Fallen Cherub", [ANGEL2] = "Fallen Cherubbello", [ANGEL3] = "Fallen Throne",
        [XLARMOR_GOLD] = "Archon Oro", [XLDEMON] = "Demon Beast", [XLARCHDEMON] = "Archdemon",
        [TRINACRIA] = "Trinacria", [STAGEKILLER] = "Stage Killer", [BOSS_XLDEATH] = "The Reaper",
        [BOSS_TRICKSTER_NORMAL] = "The Trickster", [BOSS_STALKER_NORMAL] = "The Stalker",
        [BOSS_DROWNER_NORMAL] = "The Drowner", [BOSS_MADDENER_NORMAL] = "The Maddener", [BOSS_ENDER] = "The Ender",
        [BOSS_GIANT_MIMIC1] = "Mimic Season One", [BOSS_GIANT_MIMIC2] = "Mimic Season Two",
        [BOSS_GIANT_MIMIC3] = "Mimic Season Three", [BOSS_DRAGOGION] = "Tri-Anchors", [XXLBAT] = "LV128 Golden Bat",
        [DIRECTER] = "The Directer", [STARDUST_ELEMENTAL] = "Astral Elemental", [STARDUST_CHAIR] = "Astral Chair",
        [STARDUST_CLOAK] = "Astral Curtain", [STARDUST_MUD] = "Undead Stars", [STARDUST_PORTRAIT_A] = "Poetrait",
        [STARDUST_JOE] = "Lord Ghost", [COSMIC_EGG] = "Cosmic Egg", [DEVIL_HP_1] = "Ska'sa Ka'sos",
        [DEVIL_HP_7] = "Juda R'kasso", [DEVIL_HP_11] = "Foo'Ori Darkasso", [DEVIL_HP_13] = "Shendi Darkasso",
        [DEVIL_HP_14] = "Sphon'Dato Darkasso", [DEVIL_HP_15] = "Eh'Lleve-Teh Darkasso",
        [EX_BATGOBLIN] = "Levatee Darkasso", [EX_TREASURE_VICIOUSHUNGER] = "Levarsee Darkasso",
        [MS_SPIRIT1] = "Spiritello", [MS_HITOT] = "Hitotsume-kozo", [MS_KAPPA] = "Kappa",
        [MS_TSUCHINOKO] = "Tsuchinoko", [MS_MIKOS] = "Mikoshi-nyudo", [MS_KAMAITACHI] = "Kamaitachi",
        [MS_RAIJU] = "Raiju", [MS_YAMAMBA1] = "Yamamba", [MS_TANUKI] = "Tanuki", [MS_TENGU] = "Tengu",
        [MS_TSUCHIGUMO] = "Tsuchigumo", [MS_ONIWIND] = "Windy Oni", [MS_ONITHUNDER] = "Thunderous Oni",
        [MS_ONI1] = "Big Oni", [MS_GOSHADOKURO] = "Goshadokuro", [MS_OROCHIMARIO] = "Orochimario",
        [FS_GOBLIN] = "Chompo", [FS_GEF] = "Ceffoose", [FS_SALMON] = "Wiseparke", [FS_SELKIE] = "Nutmeg",
        [FS_WULVER] = "Vulvio", [FS_LAMB] = "Lammuga", [FS_HTROW] = "Hill Trow", [FS_STROW] = "Sea Trow",
        [FS_CLOWN] = "Sam the Sandown Clown", [FS_CAKE] = "Sammy the Caterpillar", [FS_BROWN] = "Brownie",
        [FS_GREEN] = "Green Knight", [FS_AILLEN] = "Crocifriggitore", [FS_DUERG] = "Notadam",
        [FS_GLORY] = "Hand of Glory", [FS_MARI] = "Maronna", [FS_NJUGG] = "Njuggles", [FS_HAG] = "Average Hag",
        [FS_MUSH_HP] = "Fungoman", [FS_GGHOST] = "Ghostly Apparition", [FS_BOSS_AGAEA] = "Avatar of Gaea",
        [FS_CAULD] = "Cauld", [REDUSA] = "Redusa Head", [RELLYFISH] = "Rellyfish", [HEMO] = "Blood Moss",
        [BOSS_XLBLINDER] = "The Blinder", [FS_ROTGHOUL] = "Rotting Ghoul", [FS_FLAMINGSKULL] = "Burning Skull",
        [FS_WORM] = "Missing Church", [FS_SNAK1] = "Snek", [FS_SHOOTER1] = "Lost Head", [FS_MEATBALL] = "Meatball",
        [FS_MEATY] = "Followa", [FS_EDNA] = "Edna", [FS_HORCAULD] = "Cold Cauld", [FS_HORWORM] = "Well Dweller",
        [FS_HORNJUGG] = "Wet Njuggles", [FS_HORAILLEN] = "Crocifiggitone", [FS_HORDUERG] = "Still Notadam",
        [FS_HORGLORY] = "Glove of Glory", [FS_HORMARI] = "Maronna Meea", [FS_FMCETHLENN] = "Fomorians",
        [FS_BOSS_JENEVIV] = "Je-Ne-Viv", [CHAL_FRIGGI] = "Frijjitello", [CHAL_BRIA] = "Brainoid",
        [CHAL_OTTERR] = "Spotter", [CHAL_SHIFT] = "Shapeunshifter", [CHAL_LAVAA] = "Pinthot & Coldellini",
        [CHAL_GHOST] = "Space Apparition", [CHAL_MOLO2] = "Mocholo", [CHAL_MEATT] = "Meat Bean",
        [CHAL_MARTIA] = "Martian Face", [CHAL_BINNN_SUS] = "Suspicio", [CHAL_CREATU] = "Victor Frankenstein",
        [CHAL_MONKEY] = "Sponkey", [CHAL_NDROID] = "Steroid", [CHAL_TRIPOD] = "Trip Trop", [CHAL_ROBOTT] = "G062T",
        [CHAL_ANIMEXXL] = "Suspicious Eyes", [FB_LEDDER] = "Ledder", [FB_GREEDER] = "Greeder", [FB_WARLORD] = "Warlord",
        [FB_EVILSNOWMAN] = "Evil Snowman", [FB_ALIENCENTIPEDE] = "Alien Centipede", [FB_CHICKEN] = "Gulcan Tank",
        [FB_METALALIEN] = "Metal Alien", [FB_HUMANFACEDDOG] = "Human Faced Dog", [FB_GARTH] = "Garth",
        [FB_WALKINGBALL] = "Ball Walker", [FB_HELLRIDER] = "Hellrider", [FB_JUNKYARDCAR] = "Junkyard Car",
        [FB_FLIGHTZAKO] = "Flight Zako", [FB_M78] = "M78", [FB_BUGGER] = "Bugger",
        [FB_POISONOUSINSECTGEL] = "Poisonous Insect Gel", [FB_BUNDLE] = "Bundle", [FB_ZAKOALIEN] = "Zako Alien",
        [FB_GIGAFLY] = "Gigafly", [FB_MUTANTCRAWLER] = "Mutant Crawler", [FB_KIMKOH] = "Kimkoh",
        [FB_GORDEA] = "Big Bot Gordea", [BOSS_FB_TAKA] = "Taka", [BOSS_FB_BIGFUZZ] = "Big Fuzz",
        [EnemyType.FB_SIMONDO] = "Simondo Belmont", [EnemyType.TP_FLEAMAN] = "Fleaman", [TP_FLEARIDER] = "Flea Rider",
        [TP_FLEAARMOR] = "Flea Armor", [TP_MAGICBOOK_01] = "Spellbook", [EnemyType.TP_FROZENSHADE] = "Frozen Shade",
        [TP_HARPY] = "Harpy", [TP_HIPPOGRYPH] = "Hippogryph", [TP_IMP] = "Imp", [TP_MOLDYCORPSE] = "Moldy Corpse",
        [TP_SPITTLEBONE_01] = "Spittle Bone", [TP_UKOBACK] = "Ukoback", [TP_SPECTRE] = "Spectre",
        [TP_VALHALLAKNIGHT] = "Valhalla Knight", [TP_WARG_01] = "Warg", [TP_BITTERFLY] = "Bitterfly",
        [TP_BRAUNERSPAINTING] = "Bloody Painting", [TP_FISHMAN_01] = "OG Merman", [TP_PERSEPHONE_01] = "Persephone",
        [TP_GHOST] = "OG Ghost", [TP_FLYINGZOMBIE_01] = "Flying Zombie", [TP_DULLAHAN_01] = "Dullahan",
        [TP_YORICK_01] = "Yorick", [TP_AMALARICSNIPER] = "Amalaric Sniper",
        [TP_GURKHAKNIFEMASTER] = "Gurkha Knife Master", [TP_GORGON] = "Gorgon", [TP_BONEGOLEM] = "Bone Golem",
        [TP_DISCARMOR] = "Disc Armor", [TP_ALASTOR] = "Alastor", [TP_MALACHI] = "Malachi",
        [TP_WEREWOLF] = "OG Werewolf", [TP_AXEARMOR_01] = "Axe Armor", [TP_KILLERDOLL] = "Killer Doll",
        [TP_CORPSEWEED_WALL] = "Corpseweed", [TP_BONEARK] = "Bone Ark", [TP_BONEPILLAR] = "Bone Pillar",
        [TP_BOSS_JIANGSHI] = "Jiang shi", [TP_BOSS_KEREMET] = "Keremet", [TP_BOSS_ZEPHYR] = "Zephyr",
        [TP_BOSS_GAIBON] = "Gaibon", [TP_BOSS_SLOGRA] = "Slogra", [TP_BOSS_BEHEMOTH] = "Behemoth",
        [TP_BOSS_TREANT] = "Treant", [TP_BOSS_ABBADON] = "Abbadon", [TP_BOSS_MENACE] = "Menace",
        [TP_BOSS_ELIGOR] = "Eligor", [TP_BOSS_GERGOTH] = "Gergoth", [TP_BOSS_GIANTMEDUSAHEAD] = "Giant Medusa Head",
        [TP_BOSS_PUPPETMASTER] = "Puppet Master", [TP_BOSS_BLACKMORE] = "Blackmore", [TP_BOSS_PARANOIA] = "Paranoia",
        [TP_BOSS_DEATH] = "Death", [TP_BOSS_DOPPLEGANGER] = "Doppelganger", [TP_BOSS_CREATURE] = "The Creature",
        [TP_BOSS_BUGBEAR] = "Bugbear", [TP_BOSS_DEVIL] = "Devil", [TP_BOSS_MINOTAUR] = "OG Minotaur",
        [EME_CALAMITY_B] = "Calamity", [EME_GATEBOSS_SPIRITBEAST] = "Divine Wood Spirit", [EME_BOAR] = "Boar Infantry",
        [EME_GATEBOSS_DRAGON] = "Earth Dragon", [EME_GOBLIN] = "Forest Hermit", [EME_BALL] = "Maggot Ball",
        [EME_BIRD] = "Skydiver", [EME_OGRE] = "Psychic Ogre", [EME_BAT] = "Decobat", [EME_DOG] = "Failinis",
        [EME_FROG] = "Rana Combatant", [EME_KELPIE] = "Kelpie", [EME_RASCAL] = "Rascal", [EME_SLIME] = "Golden Baum",
        [EME_ALRAUNE] = "Alraune", [EME_ANT] = "Anti-Ant", [EME_MOCH] = "Moch", [EME_SKELETON] = "Skeleton from Beyond",
        [EME_WEREWOLF] = "Werewolf from Beyond", [EME_GATEBOSS_SPECTER] = "Specter of Iwanaga-hime",
        [EME_GATEBOSS_IRONMAIDEN] = "Iron Maiden", [EME_GATEBOSS_FINALEMPEROR] = "Malevolent Door Spirit",
        [EME_GATEBOSS_LIVINGANGUISH] = "Living Anguish",
    };

    public static readonly Dictionary<string, EnemyType> EnemyNameToType =
        EnemyTypeToName.ToDictionary(kv => kv.Value, kv => kv.Key);

    public static Dictionary<EnemyType, StageType[]> EnemyStages;
    public static Dictionary<EnemyType, StageType[]> EnemyHurryStages;

    public static Dictionary<EnemyType, EnemyType[]> EnemyVariantTypes;
}