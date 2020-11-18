﻿using MelonLoader;
using System;
using Discord;
using Harmony;
using Assets.Scripts.Simulation;
[assembly: MelonInfo(typeof(BTD6DiscordRPC.Main), "BTD6 Discord RPC", "1.0.0", "kenx00x")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace BTD6DiscordRPC
{
    public class Main : MelonMod
    {
        public static Discord.Discord discord;
        public override void OnApplicationStart()
        {
            MelonLogger.Log("BTD6 Discord RPC loaded!");
            discord = new Discord.Discord(778339584408027176, (UInt64)CreateFlags.Default);
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
            activityManager.UpdateActivity(activity, (res) => {
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
        public override void OnUpdate()
        {
            discord.RunCallbacks();
        }
        [HarmonyPatch(typeof(Simulation), "OnRoundEnd")]
        public class UpdateRound_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(int round)
            {
                var activityManager = discord.GetActivityManager();
                var activity = new Activity
                {
                    Details = "Playing",
                    State = $"Round {round+2}",
                    Assets =
                    {
                        LargeImage = "mainimage",
                        SmallImage = "mainimage"
                    }
                };
                activityManager.UpdateActivity(activity, (res) => {
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
        }
    }
}