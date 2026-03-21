Help support me, this and future projects on [Patreon](https://www.patreon.com/c/SW_CreeperKing)

---

## Randomizer Info BEFORE INSTALLING

### DISCLAIMER: THIS MOD WILL >>NOT<< GIVE YOU THE PAID DLC FOR FREE

> [!Note]
> Other mods might cause problems

> [!Caution]
> It's advised to get a decent amount of vanilla content done beforehand, you can set what characters and stages you
> have available to you in the yaml.
> If you don't, you might end up receiving characters and stages you haven't unlocked yet, thus not being able to
> do all of your in-logic checks.
> YOU HAVE BEEN WARNED!

> [!Important]
> When using deathlink, death killing you sends a deathlink (because idk how to detect what killed you, only that you died); Because of this it is HEAVILY recommended to use Endless for stage and character completions

## Features/Information

- Goal:
    - Loop all stages
- Mods do not work on the beta, so make sure to back up your beta save if you went into the beta
- Controller navigation is broken in character/stage selection
    - cry about it, too lazy to figure out how to fix
- DeathLink
  - can be enabled/disabled on the main menu
  - pausing can block deathlinks (this is to prevent crashes, please do not abuse)
  - you can not receive a deathlink within 4s of another
- QoL Mod (AutoUnexcludeEquippedWeapons)
  - this is a QoL mod included with the mod 
  - if you have a sealed/banished weapon it will be unsealed/unbanished for the run so it can be leveled up

### Multiplayer

/shrug

---

## How to install

(tutorial totally not copy and pasted from Tunic AP mod and BTD6 Mod helper)

- Make sure to have [.Net6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed
- Download and Install [Melon Loader](https://melonwiki.xyz/#/?id=automated-installation).
    - The default Vampire Survivors install directory (for steam): C:\Program Files (x86)\Steam\steamapps\common\Vampire Survivors
    - Require melon loader version: 7.2
- Launch the game and close it. This will finalize the Melon installation.
- Download and extract the `ArchipelagoSurvivors.zip` from
  the [latest release page](https://github.com/SWCreeperKing/ArchipelagoSurvivors/releases/latest).
    - Copy everything from the extracted `ArchipelagoSurvivors` into your game's install directory.
- Launch the game again and you should see the connection input on the top left of the title screen!
- To uninstall the mod, either remove/delete the `SW_CreeperKing.ArchipelagoSurvivors` folder

### Compatibility for the client to play with worlds genned with v0.2:
1. it will create a `Compatibility.txt` in `Mods/SW_CreeperKing.ArchipelagoSurvivors/Data`
2. inside it should have `incorrect name =`
3. just add the correct name after it
4. save
5. relaunch the game

---

# Special Thanks

- Sterlia for being my 'logic slave'
- data-bomb for https://github.com/data-bomb/FixNullableAttribute/blob/main/FixNullableAttribute.cs

---

# Tools:

- Melon Loader (obv)
- Rider
- UnityExplorer
- [Vampire Survivors Modding Docs](https://github.com/lukeod/vampiresurvivors-modding)
