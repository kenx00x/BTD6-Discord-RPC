using Assets.Scripts.Unity.Map;
using Assets.Scripts.Unity.UI_New.InGame.Stats;
using Assets.Scripts.Unity.UI_New.Main;
using Discord;
using Harmony;
using MelonLoader;
using System.Text.RegularExpressions;
[assembly: MelonInfo(typeof(BTD6DiscordRPC.Main), "BTD6 Discord RPC", "2.0.1", "kenx00x")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace BTD6DiscordRPC
{
    public class Main : MelonMod
    {
        public static string round = "";
        public static string currentMap = "";
        public static Discord.Discord discord;
        public override void OnApplicationStart()
        {
            MelonLogger.Log("BTD6 Discord RPC loaded!");
            discord = new Discord.Discord(778339584408027176, (ulong)CreateFlags.Default);
        }
        public override void OnUpdate()
        {
            RoundDisplay[] CurrentRoundUI = UnityEngine.Object.FindObjectsOfType<RoundDisplay>();
            foreach (var item in CurrentRoundUI)
            {
                round = $"Round { item.text.m_text}";
            }
            UpdateActivityFunction();
            discord.RunCallbacks();
        }
        [HarmonyPatch(typeof(MapLoader), "Load")]
        public class MapLoader_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(string map)
            {
                currentMap = "";
                string[] mapSplit = Regex.Split(map, @"(?<!^)(?=[A-Z])");
                foreach (var item in mapSplit)
                {
                    currentMap += $"{item} ";
                }
                if (currentMap == "Tutorial ")
                {
                    currentMap = "Monkey Meadow";
                }
            }
        }
        [HarmonyPatch(typeof(MainMenu), "Open")]
        public class MainMenu_Patch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                currentMap = "Main menu";
                round = "";
                UpdateActivityFunction();
            }
        }
        private static void UpdateActivityFunction()
        {
            var activityManager = discord.GetActivityManager();
            var activity = new Activity
            {
                Details = currentMap,
                State = round,
                Assets =
                    {
                        LargeImage = "mainimage",
                        SmallImage = "mainimage",
                    }
            };
            activityManager.UpdateActivity(activity, (res) => { });
        }
    }
}