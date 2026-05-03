namespace OmegasContainerUpgrades;

internal static class TraitPatch
{
    internal static void DecaySpeedChildPostfix(Trait trait, ref int __result)
    {
        if (ContainerUpgradeActions.IsPlayerCoolingActive(trait: trait) == false)
        {
            return;
        }

        __result = 0;
    }
}
