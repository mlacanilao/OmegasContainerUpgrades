using System.Runtime.CompilerServices;
using BepInEx;
using HarmonyLib;

namespace OmegasContainerUpgrades;

internal static class ModInfo
{
    internal const string Guid = "omegaplatinum.elin.omegascontainerupgrades";
    internal const string Name = "Omegas Container Upgrades";
    internal const string Version = "1.0.0";
}

[BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
internal class OmegasContainerUpgrades : BaseUnityPlugin
{
    internal static OmegasContainerUpgrades? Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Harmony.CreateAndPatchAll(type: typeof(Patcher), harmonyInstanceId: ModInfo.Guid);
    }

    internal static void LogDebug(object message, [CallerMemberName] string caller = "")
    {
        Instance?.Logger.LogDebug(data: $"[{caller}] {message}");
    }

    internal static void LogInfo(object message)
    {
        Instance?.Logger.LogInfo(data: message);
    }

    internal static void LogError(object message)
    {
        Instance?.Logger.LogError(data: message);
    }
}
