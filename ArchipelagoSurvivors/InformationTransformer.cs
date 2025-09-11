using Il2CppVampireSurvivors.Data;
using static Il2CppVampireSurvivors.Data.CharacterType;
using static Il2CppVampireSurvivors.Data.StageType;

namespace ArchipelagoSurvivors;

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

    public static readonly Dictionary<string, StageType> StageNameToType =
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
        [FB_NEWT] = "Newt Plissken", [FB_COLONEL] = "Colonel Bahamut", [FB_SIMONDO] = "Simondo Belmont",
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
        [TP_CAVETROLL] = "Cave Troll", [TP_FLEAMAN] = "Fleaman", [TP_AXEARMOR] = "Axe Armor",
        [TP_FROZENSHADE] = "Frozenshade", [TP_SUCCUBUS] = "Succubus", [TP_KEREMET] = "Keremet",
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
}