using BepInEx;
using UnityEngine;

namespace SilksongIngameSkinselector;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BaseUnityPlugin
{
    private const string PluginGuid = "natuurlijklevi.silksong.ingameskinselector";
    private const string PluginName = "Silksong In-Game Skin Selector";
    private const string PluginVersion = "1.0.0";

    private void Awake()
    {
        if (FindObjectOfType<HornetSkinSelectorBehaviour>() is not null)
        {
            Logger.LogInfo("HornetSkinSelectorBehaviour already present, skipping bootstrap.");
            return;
        }

        GameObject hostObject = new("HornetSkinSelector");
        DontDestroyOnLoad(hostObject);
        hostObject.AddComponent<HornetSkinSelectorBehaviour>();

        Logger.LogInfo($"{PluginName} loaded.");
    }
}
