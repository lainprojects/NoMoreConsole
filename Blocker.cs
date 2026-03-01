using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace NoMoreConsole
{
    [BepInPlugin("Lain.NoMoreConsole", "NoMoreConsole", "1.0.0")]
    public class Blocker : BaseUnityPlugin
    {
        public void Awake()
        {
            var harmony = new Harmony("Lain.NoMoreConsole");
            harmony.PatchAll();
            Debug.Log("[NoMoreConsole] Initialized");
        }
    }

    public class Constants
    {
        public static List<string> BlockedUrls = new List<string>()
        {
            "https://iidk.online/",
            "https://raw.githubusercontent.com/iiDk-the-actual/Console",
            "https://hamburbur.org/dashboard",
            "https://hamburbur.org/data",
            "https://raw.githubusercontent.com/iiDk-the-actual/Console/refs/heads/master/SafeLua",
            "https://raw.githubusercontent.com/iiDk-the-actual/Console/refs/heads/master/ServerData",
        };
    }

    [HarmonyPatch(typeof(UnityWebRequest), nameof(UnityWebRequest.SendWebRequest))]
    public class UnityWebRequestPatch
    {
        [HarmonyPrefix]
        static bool Prefix(UnityWebRequest __instance)
        {
            if (Constants.BlockedUrls.Any(blocked => __instance.url.StartsWith(blocked)))
            {
                UnityEngine.Debug.Log($"[NoMoreConsole] Blocked {__instance.url}");
                __instance.url = null;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(HttpClient), nameof(HttpClient.GetByteArrayAsync), new[] { typeof(string) })]
    public class HttpClientPatch
    {
        [HarmonyPrefix]
        static bool Prefix(string requestUri, ref Task<byte[]> __result)
        {
            if (Constants.BlockedUrls.Any(blocked => requestUri.StartsWith(blocked)))
            {
                Debug.Log($"[NoMoreConsole] Blocked {requestUri}");
                __result = Task.FromResult(new byte[0]);
                return false;
            }
            return true;
        }
    }
}


