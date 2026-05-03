using HarmonyLib;

namespace OmegasContainerUpgrades;

internal static class Patcher
{
    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(TraitWrench), methodName: nameof(TraitWrench.IsValidTarget))]
    internal static void TraitWrenchIsValidTarget(TraitWrench __instance, Thing t, ref bool __result)
    {
        TraitWrenchPatch.IsValidTargetPostfix(wrench: __instance, t: t, __result: ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(TraitWrench), methodName: nameof(TraitWrench.Upgrade))]
    internal static bool TraitWrenchUpgrade(TraitWrench __instance, Thing t, ref bool __result)
    {
        return TraitWrenchPatch.UpgradePrefix(wrench: __instance, t: t, __result: ref __result);
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(TraitWrench), methodName: nameof(TraitWrench.TrySetHeldAct))]
    internal static void TraitWrenchTrySetHeldAct(TraitWrench __instance, ActPlan p)
    {
        TraitWrenchPatch.TrySetHeldActPostfix(wrench: __instance, p: p);
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(InvOwner), methodName: nameof(InvOwner.ListInteractions), argumentTypes: new[] { typeof(ButtonGrid), typeof(bool) })]
    internal static void InvOwnerListInteractions(InvOwner __instance, InvOwner.ListInteraction __result, ButtonGrid b, bool context)
    {
        InvOwnerPatch.ListInteractionsPostfix(invOwner: __instance, __result: __result, b: b, context: context);
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(ActPlan), methodName: nameof(ActPlan._Update))]
    internal static void ActPlanUpdate(ActPlan __instance)
    {
        ActPlanPatch.UpdatePostfix(plan: __instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(TraitContainer), methodName: nameof(TraitContainer.TrySetAct))]
    internal static void TraitContainerTrySetAct(TraitContainer __instance, ActPlan p)
    {
        TraitContainerPatch.TrySetActPostfix(trait: __instance, p: p);
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(Trait), methodName: nameof(Trait.DecaySpeedChild), methodType: MethodType.Getter)]
    internal static void TraitDecaySpeedChild(Trait __instance, ref int __result)
    {
        TraitPatch.DecaySpeedChildPostfix(trait: __instance, __result: ref __result);
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(TraitBaseContainer), methodName: nameof(TraitBaseContainer.DecaySpeedChild), methodType: MethodType.Getter)]
    internal static void TraitBaseContainerDecaySpeedChild(TraitBaseContainer __instance, ref int __result)
    {
        TraitBaseContainerPatch.DecaySpeedChildPostfix(trait: __instance, __result: ref __result);
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(TraitMagicChest), methodName: nameof(TraitMagicChest.DecaySpeedChild), methodType: MethodType.Getter)]
    internal static void TraitMagicChestDecaySpeedChild(TraitMagicChest __instance, ref int __result)
    {
        TraitBaseContainerPatch.DecaySpeedChildPostfix(trait: __instance, __result: ref __result);
    }
}
