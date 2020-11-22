using Assets.Scripts.Models.Profile;
using Assets.Scripts.Simulation;
using Assets.Scripts.Simulation.Utils;
using Assets.Scripts.Unity.Map;
using Assets.Scripts.Unity.UI_New.Main;
using Discord;
using Harmony;
using MelonLoader;
using System.Text.RegularExpressions;
[assembly: MelonInfo(typeof(BTD6DiscordRPC.Main), "BTD6 Discord RPC", "1.1.0", "kenx00x")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace BTD6DiscordRPC
{
    public class Main : MelonMod
    {
        public static int currentRound = 0;
        public static string currentMap = "";
        public static Discord.Discord discord;
        public override void OnApplicationStart()
        {
            MelonLogger.Log("BTD6 Discord RPC loaded!");
            discord = new Discord.Discord(778339584408027176, (ulong)CreateFlags.Default);
        }
        public override void OnUpdate()
        {
            discord.RunCallbacks();
        }
        [HarmonyPatch(typeof(Simulation), "OnRoundStart")]
        public class UpdateRound_Patch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                UpdateActivityFunction();
                currentRound++;
            }
        }
        [HarmonyPatch(typeof(MapSaveLoader), "LoadMapSaveData")]
        public class LoadMap_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(MapSaveDataModel mapData)
            {
                currentRound = mapData.round;
                UpdateActivityFunction();
            }
        }
        [HarmonyPatch(typeof(MapLoader), "Load")]
        public class MapLoader_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(string map)
            {
                currentRound = 1;
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
                UpdateActivityFunction();
            }
        }
        [HarmonyPatch(typeof(MainMenu), "Open")]
        public class TitleMusic_Patch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                var activityManager = discord.GetActivityManager();
                var activity = new Activity
                {
                    State = $"Main menu",
                    Assets =
                    {
                        LargeImage = "mainimage",
                        SmallImage = "mainimage"
                    }
                };
                ActivityManagerFunction(activity, activityManager);
            }
        }
        private static void ActivityManagerFunction(Activity activity, ActivityManager activityManager)
        {
            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res == Result.Ok)
                {
                    MelonLogger.Log("Discord status updated");
                }
                else
                {
                    MelonLogger.Log("Discord status not updated");
                }
            });
        }
        private static void UpdateActivityFunction()
        {
            var activityManager = discord.GetActivityManager();
            var activity = new Activity
            {
                Details = currentMap,
                State = $"Round {currentRound}",
                Assets =
                    {
                        LargeImage = "mainimage",
                        SmallImage = "mainimage"
                    }
            };
            ActivityManagerFunction(activity, activityManager);
        }
    }
}