namespace OmegasContainerUpgrades;

internal static class ActPlanPatch
{
    internal static void UpdatePostfix(ActPlan plan)
    {
        if (plan == null)
        {
            return;
        }

        if (plan.input != ActInput.AllAction)
        {
            return;
        }

        if (EClass.pc == null)
        {
            return;
        }

        if (EClass.pc.isDead)
        {
            return;
        }

        if (EClass.ui.IsDragging)
        {
            return;
        }

        if (plan.IsSelf == false)
        {
            return;
        }

        if (InvOwner.HasTrader)
        {
            return;
        }

        plan.TrySetAct(
            lang: Localization.FreeMenu,
            onPerform: delegate
            {
                ContainerUpgradeMenu.ShowPlayerInventoryMenu();
                return false;
            },
            tc: EClass.pc,
            cursor: CursorSystem.Inventory,
            dist: 0,
            isHostileAct: false,
            localAct: true,
            canRepeat: false);
    }
}
