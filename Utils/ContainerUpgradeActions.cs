using System;
using UnityEngine;

namespace OmegasContainerUpgrades;

internal enum ContainerResizeAction
{
    IncreaseWidth,
    IncreaseHeight,
    IncreaseCapacity,
    DecreaseCapacity,
    DecreaseWidth,
    DecreaseHeight,
    ResetSize,
    ToggleCooling
}

internal static class ContainerUpgradeActions
{
    private const int PlayerDefaultWidth = 8;
    private const int PlayerDefaultHeight = 5;
    private const int MaxGridWidth = 99;
    private const int MaxGridHeight = 99;
    private const int MenuResizeStep = 1;
    private const int VanillaStorageUpgradeCells = 20;

    internal static bool IsSupportedWrenchId(string id)
    {
        switch (id)
        {
            case "extend_h":
            case "extend_v":
            case "storage":
            case "fridge":
                return true;
            default:
                return false;
        }
    }

    internal static bool IsValidWrenchTarget(TraitWrench wrench, Thing target)
    {
        if (wrench == null)
        {
            return false;
        }

        if (target == null)
        {
            return false;
        }

        string id = wrench.ID;
        if (IsSupportedWrenchId(id: id) == false)
        {
            return false;
        }

        return IsPlayerOwnedContainer(target: target);
    }

    internal static bool TryApplyWrenchUpgrade(TraitWrench wrench, Thing target)
    {
        if (IsValidWrenchTarget(wrench: wrench, target: target) == false)
        {
            return false;
        }

        switch (wrench.ID)
        {
            case "extend_h":
                return TryResizeContainer(target: target, width: target.things.width + 1, height: target.things.height);
            case "extend_v":
                return TryResizeContainer(target: target, width: target.things.width, height: target.things.height + 1);
            case "storage":
                return TryIncreaseCapacity(target: target, amount: VanillaStorageUpgradeCells);
            case "fridge":
                return TryEnableCooling(target: target);
            default:
                return false;
        }
    }

    internal static bool TryApplyPlayerWrenchUpgrade(TraitWrench wrench)
    {
        if (wrench == null)
        {
            return false;
        }

        if (EClass.pc == null)
        {
            return false;
        }

        ThingContainer things = EClass.pc.things;
        switch (wrench.ID)
        {
            case "extend_h":
                return TryResizePlayerInventory(width: things.width + 1, height: things.height);
            case "extend_v":
                return TryResizePlayerInventory(width: things.width, height: things.height + 1);
            case "storage":
                return TryIncreasePlayerCapacity(amount: VanillaStorageUpgradeCells);
            case "fridge":
                return TryEnablePlayerCooling();
            default:
                return false;
        }
    }

    internal static bool TryApplyMenuAction(Thing target, ContainerResizeAction action)
    {
        if (IsPlayerOwnedContainer(target: target) == false)
        {
            return false;
        }

        int step = MenuResizeStep;
        switch (action)
        {
            case ContainerResizeAction.IncreaseWidth:
                return TryResizeContainer(target: target, width: target.things.width + step, height: target.things.height);
            case ContainerResizeAction.IncreaseHeight:
                return TryResizeContainer(target: target, width: target.things.width, height: target.things.height + step);
            case ContainerResizeAction.IncreaseCapacity:
                return TryIncreaseCapacity(target: target, amount: step);
            case ContainerResizeAction.DecreaseCapacity:
                return TryDecreaseCapacity(target: target, amount: step);
            case ContainerResizeAction.DecreaseWidth:
                return TryResizeContainer(target: target, width: target.things.width - step, height: target.things.height);
            case ContainerResizeAction.DecreaseHeight:
                return TryResizeContainer(target: target, width: target.things.width, height: target.things.height - step);
            case ContainerResizeAction.ResetSize:
                return TryResetContainerSize(target: target);
            case ContainerResizeAction.ToggleCooling:
                return TryToggleCooling(target: target);
            default:
                return false;
        }
    }

    internal static bool TryApplyPlayerMenuAction(ContainerResizeAction action)
    {
        if (EClass.pc == null)
        {
            return false;
        }

        int step = MenuResizeStep;
        ThingContainer things = EClass.pc.things;
        switch (action)
        {
            case ContainerResizeAction.IncreaseWidth:
                return TryResizePlayerInventory(width: things.width + step, height: things.height);
            case ContainerResizeAction.IncreaseHeight:
                return TryResizePlayerInventory(width: things.width, height: things.height + step);
            case ContainerResizeAction.IncreaseCapacity:
                return TryIncreasePlayerCapacity(amount: step);
            case ContainerResizeAction.DecreaseCapacity:
                return TryDecreasePlayerCapacity(amount: step);
            case ContainerResizeAction.DecreaseWidth:
                return TryResizePlayerInventory(width: things.width - step, height: things.height);
            case ContainerResizeAction.DecreaseHeight:
                return TryResizePlayerInventory(width: things.width, height: things.height - step);
            case ContainerResizeAction.ResetSize:
                return TryResetPlayerInventory();
            case ContainerResizeAction.ToggleCooling:
                return TryTogglePlayerCooling();
            default:
                return false;
        }
    }

    internal static bool IsPlayerOwnedContainer(Thing target)
    {
        if (target == null)
        {
            return false;
        }

        if (target.isDestroyed)
        {
            return false;
        }

        Trait trait = target.trait;
        if (trait == null)
        {
            return false;
        }

        if (trait.IsContainer == false)
        {
            return false;
        }

        if (target.IsContainer == false)
        {
            return false;
        }

        if (trait is TraitChestMerchant)
        {
            return false;
        }

        if (target.isNPCProperty)
        {
            return false;
        }

        Card root = target.GetRootCard();
        if (root == EClass.pc)
        {
            return true;
        }

        if (target.IsInstalled && target.GetRoot() is Zone zone && zone.IsPCFaction)
        {
            return true;
        }

        return false;
    }

    internal static bool IsCoolingActive(TraitBaseContainer trait)
    {
        if (trait == null)
        {
            return false;
        }

        Thing? owner = trait.owner as Thing;
        if (owner == null)
        {
            return false;
        }

        if (owner.c_containerUpgrade.cool <= 0)
        {
            return false;
        }

        if (trait is TraitMagicChest && owner.isOn == false)
        {
            return false;
        }

        return IsPlayerOwnedContainer(target: owner);
    }

    internal static bool IsPlayerCoolingActive(Trait trait)
    {
        if (trait == null)
        {
            return false;
        }

        if (EClass.pc == null)
        {
            return false;
        }

        if (trait.owner != EClass.pc)
        {
            return false;
        }

        if (EClass.pc.c_containerUpgrade.cool <= 0)
        {
            return false;
        }

        return true;
    }

    private static bool TryIncreaseCapacity(Thing target, int amount)
    {
        if (target.trait is TraitMagicChest)
        {
            if (amount <= 0)
            {
                return false;
            }

            target.c_containerUpgrade.cap += amount;
            RefreshAfterChange(target: target);
            return true;
        }

        int width = target.things.width;
        int height = target.things.height;
        if (width <= 0)
        {
            width = 1;
        }

        int desiredArea = width * height + amount;
        int desiredHeight = (desiredArea + width - 1) / width;
        return TryResizeContainer(target: target, width: width, height: desiredHeight);
    }

    private static bool TryIncreasePlayerCapacity(int amount)
    {
        ThingContainer things = EClass.pc.things;
        int width = things.width;
        if (width <= 0)
        {
            width = PlayerDefaultWidth;
        }

        int desiredArea = width * things.height + amount;
        int desiredHeight = (desiredArea + width - 1) / width;
        return TryResizePlayerInventory(width: width, height: desiredHeight);
    }

    private static bool TryDecreaseCapacity(Thing target, int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (target.trait is TraitMagicChest)
        {
            if (target.c_containerUpgrade.cap <= 0)
            {
                return false;
            }

            int newCapacityBonus = target.c_containerUpgrade.cap - amount;
            if (newCapacityBonus < 0)
            {
                newCapacityBonus = 0;
            }

            if (CanSetMagicChestCapacityBonus(target: target, capacityBonus: newCapacityBonus) == false)
            {
                return false;
            }

            target.c_containerUpgrade.cap = newCapacityBonus;
            RefreshAfterChange(target: target);
            return true;
        }

        return TryDecreaseGridRows(things: target.things, refreshTarget: target, rows: amount);
    }

    private static bool TryDecreasePlayerCapacity(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        return TryDecreaseGridRows(things: EClass.pc.things, refreshTarget: EClass.pc.Thing, rows: amount);
    }

    private static bool TryDecreaseGridRows(ThingContainer things, Thing refreshTarget, int rows)
    {
        int width = things.width;
        if (width <= 0)
        {
            return false;
        }

        if (rows <= 0)
        {
            return false;
        }

        int desiredHeight = things.height - rows;
        if (desiredHeight <= 0)
        {
            return false;
        }

        if (CanResize(things: things, width: width, height: desiredHeight, blockByVisibleItems: true) == false)
        {
            return false;
        }

        things.ChangeSize(w: width, h: desiredHeight);
        RefreshAfterChange(target: refreshTarget);
        return true;
    }

    private static bool TryResetContainerSize(Thing target)
    {
        TraitBaseContainer? trait = target.trait as TraitBaseContainer;
        if (trait == null)
        {
            return false;
        }

        if (target.trait is TraitMagicChest)
        {
            if (CanSetMagicChestCapacityBonus(target: target, capacityBonus: 0) == false)
            {
                return false;
            }

            if (CanResize(things: target.things, width: trait.Width, height: trait.Height, blockByVisibleItems: false) == false)
            {
                return false;
            }

            target.c_containerUpgrade.cap = 0;
            SetCoolingState(target: target, enabled: false);
            target.things.ChangeSize(w: trait.Width, h: trait.Height);
            RefreshAfterChange(target: target);
            return true;
        }

        if (CanResize(things: target.things, width: trait.Width, height: trait.Height, blockByVisibleItems: true) == false)
        {
            return false;
        }

        target.c_containerUpgrade.cap = 0;
        SetCoolingState(target: target, enabled: false);
        target.things.ChangeSize(w: trait.Width, h: trait.Height);
        RefreshAfterChange(target: target);
        return true;
    }

    private static bool TryResetPlayerInventory()
    {
        if (EClass.pc == null)
        {
            return false;
        }

        if (CanResize(things: EClass.pc.things, width: PlayerDefaultWidth, height: PlayerDefaultHeight, blockByVisibleItems: true) == false)
        {
            return false;
        }

        EClass.pc.c_containerUpgrade.cap = 0;
        SetCoolingState(target: EClass.pc, enabled: false);
        EClass.pc.things.ChangeSize(w: PlayerDefaultWidth, h: PlayerDefaultHeight);
        RefreshAfterChange(target: EClass.pc.Thing);
        return true;
    }

    private static bool CanSetMagicChestCapacityBonus(Thing target, int capacityBonus)
    {
        if (capacityBonus < 0)
        {
            return false;
        }

        int baseCapacity = target.things.MaxCapacity - target.c_containerUpgrade.cap;
        int maxCapacity = baseCapacity + capacityBonus;
        if (target.things.Count > maxCapacity)
        {
            return false;
        }

        return true;
    }

    private static bool TryToggleCooling(Thing target)
    {
        if (target.c_containerUpgrade.cool > 0)
        {
            ApplyCooling(target: target, refreshTarget: target, enabled: false);
            return true;
        }

        ApplyCooling(target: target, refreshTarget: target, enabled: true);
        return true;
    }

    private static bool TryEnableCooling(Thing target)
    {
        if (target.c_containerUpgrade.cool > 0)
        {
            return false;
        }

        ApplyCooling(target: target, refreshTarget: target, enabled: true);
        return true;
    }

    private static bool TryTogglePlayerCooling()
    {
        if (EClass.pc == null)
        {
            return false;
        }

        if (EClass.pc.c_containerUpgrade.cool > 0)
        {
            ApplyCooling(target: EClass.pc, refreshTarget: EClass.pc.Thing, enabled: false);
            return true;
        }

        ApplyCooling(target: EClass.pc, refreshTarget: EClass.pc.Thing, enabled: true);
        return true;
    }

    private static bool TryEnablePlayerCooling()
    {
        if (EClass.pc == null)
        {
            return false;
        }

        if (EClass.pc.c_containerUpgrade.cool > 0)
        {
            return false;
        }

        ApplyCooling(target: EClass.pc, refreshTarget: EClass.pc.Thing, enabled: true);
        return true;
    }

    private static void ApplyCooling(Card target, Thing refreshTarget, bool enabled)
    {
        SetCoolingState(target: target, enabled: enabled);
        RefreshAfterChange(target: refreshTarget);
    }

    private static void SetCoolingState(Card target, bool enabled)
    {
        int cool = 0;
        int elementValue = 0;
        if (enabled)
        {
            cool = 1;
            elementValue = 50;
        }

        target.c_containerUpgrade.cool = cool;
        target.elements.SetBase(id: 405, v: elementValue, potential: 0);
    }

    private static bool TryResizeContainer(Thing target, int width, int height)
    {
        bool blockByVisibleItems = target.trait is TraitMagicChest == false;
        if (CanResize(things: target.things, width: width, height: height, blockByVisibleItems: blockByVisibleItems) == false)
        {
            return false;
        }

        target.things.ChangeSize(w: width, h: height);
        RefreshAfterChange(target: target);
        return true;
    }

    private static bool TryResizePlayerInventory(int width, int height)
    {
        if (CanResize(things: EClass.pc.things, width: width, height: height, blockByVisibleItems: true) == false)
        {
            return false;
        }

        EClass.pc.things.ChangeSize(w: width, h: height);
        RefreshAfterChange(target: EClass.pc.Thing);
        return true;
    }

    private static bool CanResize(ThingContainer things, int width, int height, bool blockByVisibleItems)
    {
        if (width < 1 || height < 1)
        {
            return false;
        }

        if (width > MaxGridWidth)
        {
            return false;
        }

        if (height > MaxGridHeight)
        {
            return false;
        }

        if (blockByVisibleItems == false)
        {
            return true;
        }

        int visibleCount = CountVisibleItems(things: things);
        if (visibleCount > width * height)
        {
            return false;
        }

        return true;
    }

    private static int CountVisibleItems(ThingContainer things)
    {
        int count = 0;
        foreach (Thing thing in things)
        {
            if (things.ShouldShowOnGrid(t: thing))
            {
                count++;
            }
        }

        return count;
    }

    internal static void RefreshAfterChange(Thing target)
    {
        if (EClass.Branch != null)
        {
            EClass.Branch.resources.SetDirty();
        }

        if (EClass._zone != null)
        {
            EClass._zone.RefreshElectricity();
        }

        if (target != null)
        {
            LayerInventory.SetDirty(t: target);
        }

        LayerInventory.SetDirtyAll(immediate: false);
        UIInventory.RefreshAllList();
    }

    internal static void PlaySuccess(Thing? target)
    {
        SE.Play(id: "build_area");
        Thing? thing = target;
        if (thing != null)
        {
            thing.PlayEffect(id: "buff", useRenderPos: true, range: 0f, fix: default(Vector3));
        }
        else
        {
            if (EClass.pc == null)
            {
                return;
            }

            EClass.pc.PlayEffect(id: "buff", useRenderPos: true, range: 0f, fix: default(Vector3));
        }
    }
}
