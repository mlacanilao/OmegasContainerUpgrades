namespace OmegasContainerUpgrades;

internal static class InvOwnerPatch
{
    internal static void ListInteractionsPostfix(
        InvOwner invOwner,
        InvOwner.ListInteraction __result,
        ButtonGrid b,
        bool context)
    {
        if (context == false)
        {
            return;
        }

        if (__result == null)
        {
            return;
        }

        if (InvOwner.HasTrader)
        {
            return;
        }

        if (invOwner is InvOwnerAlly)
        {
            return;
        }

        if (b == null || b.card == null)
        {
            return;
        }

        Thing target = b.card.Thing;
        AddContainerUpgradeMenuEntry(target: target, list: __result);
    }

    private static void AddContainerUpgradeMenuEntry(Thing target, InvOwner.ListInteraction list)
    {
        if (target == null)
        {
            return;
        }

        if (ContainerUpgradeActions.IsPlayerOwnedContainer(target: target) == false)
        {
            return;
        }

        list.Add(
            s: Localization.FreeMenu,
            priority: 298,
            action: delegate
            {
                ContainerUpgradeMenu.ShowContainerMenu(target: target);
            },
            idPriority: null);
    }
}
