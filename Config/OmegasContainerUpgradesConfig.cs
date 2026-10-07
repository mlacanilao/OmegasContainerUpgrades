using BepInEx.Configuration;

namespace OmegasContainerUpgrades;

internal static class OmegasContainerUpgradesConfig
{
    internal static ConfigEntry<bool> EnableContextMenu = null!;

    internal static void LoadConfig(ConfigFile config)
    {
        EnableContextMenu = config.Bind(
            section: ModInfo.Name,
            key: "Enable Context Menu",
            defaultValue: true,
            description: "Show free Container Upgrades context-menu entries for carried and installed player-owned containers and the player inventory.\n" +
                         "携帯中・設置済みのプレイヤー所有コンテナとプレイヤーインベントリに、無料のコンテナ強化メニューを表示します。\n" +
                         "在随身携带或已放置的玩家所有容器及玩家背包中显示免费容器升级菜单。"
        );
    }
}
