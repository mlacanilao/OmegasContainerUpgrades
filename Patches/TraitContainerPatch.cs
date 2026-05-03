namespace OmegasContainerUpgrades;

internal static class TraitContainerPatch
{
    internal static void TrySetActPostfix(TraitContainer trait, ActPlan p)
    {
        if (p == null)
        {
            return;
        }

        if (p.input != ActInput.AllAction)
        {
            return;
        }

        if (trait == null)
        {
            return;
        }

        Thing? target = trait.owner as Thing;
        if (target == null)
        {
            return;
        }

        if (target.IsInstalled == false)
        {
            return;
        }

        if (target.c_lockLv > 0)
        {
            return;
        }

        if (trait.CanOpenContainer == false)
        {
            return;
        }

        if (ContainerUpgradeActions.IsPlayerOwnedContainer(target: target) == false)
        {
            return;
        }

        p.TrySetAct(
            lang: Localization.FreeMenu,
            onPerform: delegate
            {
                ContainerUpgradeMenu.ShowContainerMenu(target: target);
                return false;
            },
            tc: target,
            cursor: CursorSystem.Container,
            dist: 1,
            isHostileAct: false,
            localAct: true,
            canRepeat: false);
    }
}
