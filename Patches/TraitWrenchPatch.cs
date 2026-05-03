namespace OmegasContainerUpgrades;

internal static class TraitWrenchPatch
{
    internal static void IsValidTargetPostfix(TraitWrench wrench, Thing t, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        __result = ContainerUpgradeActions.IsValidWrenchTarget(wrench: wrench, target: t);
    }

    internal static bool UpgradePrefix(TraitWrench wrench, Thing t, ref bool __result)
    {
        if (ContainerUpgradeActions.IsValidWrenchTarget(wrench: wrench, target: t) == false)
        {
            return true;
        }

        __result = ContainerUpgradeActions.TryApplyWrenchUpgrade(wrench: wrench, target: t);
        return false;
    }

    internal static void TrySetHeldActPostfix(TraitWrench wrench, ActPlan p)
    {
        if (p == null)
        {
            return;
        }

        if (p.IsSelf == false)
        {
            return;
        }

        if (wrench == null)
        {
            return;
        }

        if (ContainerUpgradeActions.IsSupportedWrenchId(id: wrench.ID) == false)
        {
            return;
        }

        p.TrySetAct(
            lang: Localization.UpgradeInventory.lang(),
            onPerform: delegate
            {
                if (ContainerUpgradeActions.TryApplyPlayerWrenchUpgrade(wrench: wrench))
                {
                    Msg.Say(idLang: "upgrade", c1: EClass.pc, ref1: wrench.owner.GetName(style: NameStyle.Full, num: 1), ref2: null, ref3: null);
                    ContainerUpgradeActions.PlaySuccess(target: null);
                    wrench.owner.ModNum(a: -1, notify: true);
                }
                else
                {
                    Msg.Say(idLang: "noMoreUpgrade", c1: EClass.pc, ref1: wrench.owner.GetName(style: NameStyle.Full, num: 1), ref2: null, ref3: null);
                }

                return false;
            },
            tc: EClass.pc,
            cursor: null,
            dist: 0,
            isHostileAct: false,
            localAct: true,
            canRepeat: false);
    }
}
