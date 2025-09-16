using ArchipelagoSurvivors.Patches;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Weapons;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Framework.Saves;
using MelonLoader;
using UnityEngine;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.Core;
using Saver = Il2CppVampireSurvivors.Framework.Saves.PhaserSaveDataUtils;

namespace ArchipelagoSurvivors;

// stolen from: https://github.com/FyreDay/TCG-CardShop-Sim-APClient/blob/master/APGui.cs
[RegisterTypeInIl2Cpp]
public class APGui : MonoBehaviour
{
    public static bool ShowGUI = true;
    public static string IpPorttext = "archipelago.gg:12345";
    public static string Password = "";
    public static string Slot = "Survivor";
    public static string State = "";
    public static Vector2 Offset = new(100, 100);
    private static double TimeAccumulator;
    private static double DeathlinkToggleTimer;

    public static GUIStyle TextStyle = new()
    {
        fontSize = 12,
        normal =
        {
            textColor = Color.white
        }
    };

    public static GUIStyle TextStyleGreen = new()
    {
        fontSize = 12,
        normal =
        {
            textColor = Color.green
        }
    };

    public static GUIStyle TextStyleRed = new()
    {
        fontSize = 12,
        normal =
        {
            textColor = Color.red
        }
    };

    private void Awake()
    {
        if (!File.Exists("ApConnection.txt")) return;
        var fileText = File.ReadAllText("ApConnection.txt").Replace("\r", "").Split('\n');
        IpPorttext = fileText[0];
        Password = fileText[1];
        Slot = fileText[2];
    }

    void OnGUI()
    {
        TimeAccumulator += Time.deltaTime;

        if (DeathlinkToggleTimer > 0)
        {
            DeathlinkToggleTimer -= Time.deltaTime;
        }

        if (!ShowGUI) return;

        if (!IsConnected())
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
            GUI.Box(new Rect(10 + Offset.x, 10 + Offset.y + 100, 200, 150), "AP Client");
            if (DeathlinkToggleTimer <= 0 && GUI.Button(new Rect(10 + Offset.x, 10 + Offset.y + 130, 200, 50), "Toggle Deathlink"))
            {
                DeathlinkToggleTimer = 3;
                ToggleDeathlink();
            }

            GUI.Label(new Rect(10 + Offset.x, 10 + Offset.y + 180, 200, 50),
                DeathLink ? "Deathlink is enabled" : "Deathlink is disabled", TextStyle);
        }

        if (MainMenuPatch.StartButton is not null)
        {
            MainMenuPatch.StartButton.gameObject.SetActive(IsConnected());
        }

        if (MainMenuPatch.BestiaryButton is not null)
        {
            MainMenuPatch.BestiaryButton.gameObject.SetActive(IsConnected() && EnemysanityEnabled);
        }
        
        if (!IsConnected() && GUI.Button(new Rect(20 + Offset.x, 210 + Offset.y, 180, 30), "Connect"))
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
                return;
            }

            State = "";
            File.WriteAllText("ApConnection.txt", $"{IpPorttext}\n{Password}\n{Slot}");
            TimeAccumulator = 0;
        }

        if (IsConnected() && GUI.Button(new Rect(20 + Offset.x, 210 + Offset.y, 180, 30), "Disconnect"))
        {
            Disconnect();
        }

        GUI.Label(new Rect(20 + Offset.x, 240 + Offset.y, 300, 30),
            State != "" ? State : IsConnected() ? "Connected" : "Not Connected",
            IsConnected() ? TextStyleGreen : TextStyleRed);
    }
}