using MelonLoader;
using UnityEngine;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.Patches.MainMenuPatch;
using static CreepyUtil.Archipelago.ArchipelagoTag;

namespace ArchipelagoSurvivors;

// stolen from: https://github.com/FyreDay/TCG-CardShop-Sim-APClient/blob/master/APGui.cs
[RegisterTypeInIl2Cpp]
public class APGui : MonoBehaviour
{
    public static bool ShowGUI = true;
    public static string IpPorttext = "archipelago.gg:12345";
    public static string Password = "";
    public static string Slot = "Survivor";
    public static string DeathLinkGroup = "";
    public static bool EnabledDeathLink = false;
    public static string State = "";
    public static Vector2 Offset = new(100, 100);

    public static GUIStyle TextStyle = new() { fontSize = 12, normal = { textColor = Color.white } };

    public static GUIStyle TextStyleGreen = new() { fontSize = 12, normal = { textColor = Color.green } };

    public static GUIStyle TextStyleRed = new() { fontSize = 12, normal = { textColor = Color.red } };

    private void Awake()
    {
        if (!File.Exists("ApConnection.txt")) return;
        var fileText = File.ReadAllLines("ApConnection.txt");
        IpPorttext = fileText[0];
        Password = fileText[1];
        Slot = fileText[2];
        if (fileText.Length > 3) DeathLinkGroup = fileText[3];
        if (fileText.Length > 4) EnabledDeathLink = fileText[4].ToLower()[0] == 't';
    }

    void OnGUI()
    {
        if (!ShowGUI) return;

        if (!Client.IsConnected)
        {
            GUI.Box(new Rect(10 + Offset.x, 10 + Offset.y, 200, 300), "AP Client");

            GUI.Label(new Rect(20 + Offset.x, 40 + Offset.y, 300, 30), "Address:port", TextStyle);
            IpPorttext = GUI.TextField(new Rect(20 + Offset.x, 60 + Offset.y, 180, 25), IpPorttext, 25);

            GUI.Label(new Rect(20 + Offset.x, 90 + Offset.y, 300, 30), "Password", TextStyle);
            Password = GUI.TextField(new Rect(20 + Offset.x, 110 + Offset.y, 180, 25), Password, 25);

            GUI.Label(new Rect(20 + Offset.x, 140 + Offset.y, 300, 30), "Slot", TextStyle);
            Slot = GUI.TextField(new Rect(20 + Offset.x, 160 + Offset.y, 180, 25), Slot, 25);
        }
        else
        {
            if (EnabledDeathLink != Client.Tags[DeathLink])
            {
                Client.DeathLinkGroups.Clear();
                Client.DeathLinkGroups.Add(DeathLinkGroup);
                Core.Log.Msg(
                    $"{EnabledDeathLink} != {Client.Tags[DeathLink]} ([{string.Join(", ", Client.Tags.GetTagsAsStrings())}])");
                Client.Tags.ToggleDeathLink();
            }

            GUI.Box(new Rect(10 + Offset.x, 10 + Offset.y + 100, 200, 150), "AP Client");
            GUI.Label(new Rect(20 + Offset.x, 140 + Offset.y, 300, 30), "DeathLink Group", TextStyle);
            DeathLinkGroup = GUI.TextField(new Rect(20 + Offset.x, 160 + Offset.y, 180, 25), DeathLinkGroup, 25);

            var deathLink = GUI.Toggle(new Rect(20 + Offset.x, 190 + Offset.y, 180, 30), EnabledDeathLink,
                "Toggle Deathlink");
            if (deathLink != EnabledDeathLink)
            {
                if (deathLink)
                {
                    Client.DeathLinkGroups.Clear();
                    Client.DeathLinkGroups.Add(DeathLinkGroup);
                }

                Client.Tags.ToggleDeathLink();
                EnabledDeathLink = !EnabledDeathLink;
                SaveData();
            }
        }

        StartButton?.gameObject.SetActive(Client.IsConnected);
        BestiaryButton?.gameObject.SetActive(Client.IsConnected && EnemysanityEnabled);

        if (!Client.IsConnected && GUI.Button(new Rect(20 + Offset.x, 210 + Offset.y, 180, 30), "Connect"))
        {
            var ipPortSplit = IpPorttext.Split(':');
            if (!int.TryParse(ipPortSplit[1], out var port))
            {
                State = $"[{ipPortSplit[1]}] is not a valid port";
                return;
            }

            var error = TryConnect(port, Slot, ipPortSplit[0], Password);

            if (error is not null)
            {
                State = string.Join("\n", error);
                Core.Log.Error(State);
                return;
            }

            State = "";
            SaveData();
        }

        if (Client.IsConnected && GUI.Button(new Rect(20 + Offset.x, 210 + Offset.y, 180, 30), "Disconnect"))
        {
            Disconnect();
            File.WriteAllLines("ENEMY NAME TYPES.txt",
                Names.Select(kv => $"{(kv.Value.Contains(',') ? $"\"{kv.Value}\"" : kv.Value)}, {kv.Key}"));
        }

        GUI.Label(new Rect(20 + Offset.x, 240 + Offset.y, 300, 30),
            State != "" ? State : Client.IsConnected ? "Connected" : "Not Connected",
            Client.IsConnected ? TextStyleGreen : TextStyleRed);
    }

    public static void SaveData()
    {
        File.WriteAllText("ApConnection.txt",
            $"{IpPorttext}\n{Password}\n{Slot}\n{DeathLinkGroup}\n{EnabledDeathLink}");
    }
}