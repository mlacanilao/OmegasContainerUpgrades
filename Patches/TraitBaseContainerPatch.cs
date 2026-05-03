namespace OmegasContainerUpgrades;

internal static class TraitBaseContainerPatch
{
    internal static void DecaySpeedChildPostfix(TraitBaseContainer trait, ref int __result)
    {
        if (ContainerUpgradeActions.IsCoolingActive(trait: trait) == false)
        {
            return;
        }

        __result = 0;
    }
}
