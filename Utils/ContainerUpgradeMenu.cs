using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OmegasContainerUpgrades;

internal static class ContainerUpgradeMenu
{
    internal static void ShowContainerMenu(Thing target)
    {
        ShowUpgradeMenu(context: UpgradeMenuContext.ForContainer(target: target));
    }

    internal static void ShowPlayerInventoryMenu()
    {
        ShowUpgradeMenu(context: UpgradeMenuContext.ForPlayer());
    }

    private static void ShowUpgradeMenu(UpgradeMenuContext context)
    {
        UIContextMenu menu = EClass.ui.CreateContextMenu(cid: "ContextMenu");
        UpgradeMenuState state = new UpgradeMenuState(context: context);
        AddMenuHeader(menu: menu);
        state.IncreaseWidthButton = AddUpgradeButton(menu: menu, state: state, action: ContainerResizeAction.IncreaseWidth);
        state.IncreaseHeightButton = AddUpgradeButton(menu: menu, state: state, action: ContainerResizeAction.IncreaseHeight);
        state.IncreaseCapacityButton = AddUpgradeButton(menu: menu, state: state, action: ContainerResizeAction.IncreaseCapacity);
        state.DecreaseWidthButton = AddUpgradeButton(menu: menu, state: state, action: ContainerResizeAction.DecreaseWidth);
        state.DecreaseHeightButton = AddUpgradeButton(menu: menu, state: state, action: ContainerResizeAction.DecreaseHeight);
        state.ResetSizeButton = AddUpgradeButton(menu: menu, state: state, action: ContainerResizeAction.ResetSize);
        state.ToggleCoolingButton = AddUpgradeButton(menu: menu, state: state, action: ContainerResizeAction.ToggleCooling);
        AddContainerIconMenu(menu: menu, state: state);
        menu.Show();
    }

    private static void AddMenuHeader(UIContextMenu menu)
    {
        UIButton button = menu.AddButton(idLang: Localization.FreeMenu, action: delegate
        {
        }, hideAfter: false);
        SetTooltip(button: button, text: Localization.FreeMenuTooltip.lang());
    }

    private static string GetCoolingLabel(UpgradeMenuContext context)
    {
        if (context.CoolingEnabled)
        {
            return Localization.DisableCooling.lang();
        }

        return Localization.EnableCooling.lang();
    }

    private static string GetCapacityTooltip(UpgradeMenuContext context)
    {
        if (context.Target != null && context.Target.trait is TraitMagicChest)
        {
            return Localization.TooltipMagicChestCapacity.lang();
        }

        return context.GridNameAddsSlotsTooltip;
    }

    private static string GetCoolingTooltip(UpgradeMenuContext context)
    {
        if (context.CoolingEnabled)
        {
            return context.DisableCoolingTooltip;
        }

        return context.EnableCoolingTooltip;
    }

    private static string GetUpgradeLabel(UpgradeMenuContext context, ContainerResizeAction action)
    {
        switch (action)
        {
            case ContainerResizeAction.IncreaseWidth:
                return Localization.IncreaseWidth.lang(context.Things.width.ToString(), null, null, null, null);
            case ContainerResizeAction.IncreaseHeight:
                return Localization.IncreaseHeight.lang(context.Things.height.ToString(), null, null, null, null);
            case ContainerResizeAction.IncreaseCapacity:
                return Localization.IncreaseCapacity.lang(context.Things.MaxCapacity.ToString(), null, null, null, null);
            case ContainerResizeAction.DecreaseWidth:
                return Localization.DecreaseWidth.lang(context.Things.width.ToString(), null, null, null, null);
            case ContainerResizeAction.DecreaseHeight:
                return Localization.DecreaseHeight.lang(context.Things.height.ToString(), null, null, null, null);
            case ContainerResizeAction.ResetSize:
                return Localization.ResetSize.lang(context.Things.width.ToString() + "x" + context.Things.height.ToString(), null, null, null, null);
            case ContainerResizeAction.ToggleCooling:
                return GetCoolingLabel(context: context);
            default:
                return string.Empty;
        }
    }

    private static string GetUpgradeTooltip(UpgradeMenuContext context, ContainerResizeAction action)
    {
        switch (action)
        {
            case ContainerResizeAction.IncreaseWidth:
                return context.GridNameAddsColumnsTooltip;
            case ContainerResizeAction.IncreaseHeight:
                return context.GridNameAddsRowsTooltip;
            case ContainerResizeAction.IncreaseCapacity:
                return GetCapacityTooltip(context: context);
            case ContainerResizeAction.DecreaseWidth:
                return context.GridNameRemovesColumnsTooltip;
            case ContainerResizeAction.DecreaseHeight:
                return context.GridNameRemovesRowsTooltip;
            case ContainerResizeAction.ResetSize:
                return context.ResetTooltip;
            case ContainerResizeAction.ToggleCooling:
                return GetCoolingTooltip(context: context);
            default:
                return string.Empty;
        }
    }

    private static UIButton AddUpgradeButton(UIContextMenu menu, UpgradeMenuState state, ContainerResizeAction action)
    {
        UIButton button = menu.AddButton(idLang: GetUpgradeLabel(context: state.Context, action: action), action: delegate
        {
            if (ApplyMenuAction(action: () => ApplyUpgradeAction(context: state.Context, action: action), target: state.Context.Target))
            {
                RefreshUpgradeMenuLabels(state: state);
            }
        }, hideAfter: false);
        SetTooltip(button: button, text: GetUpgradeTooltip(context: state.Context, action: action));
        return button;
    }

    private static void AddContainerIconMenu(UIContextMenu menu, UpgradeMenuState state)
    {
        Thing? target = state.Context.Target;
        if (target == null)
        {
            return;
        }

        UIContextMenu iconMenu = menu.AddChild(Localization.ChangeIcon.lang(GetContainerIconLabel(target: target), null, null, null, null));
        state.ChangeIconMenu = iconMenu;
        if (iconMenu.popper != null)
        {
            SetTooltip(button: iconMenu.popper.button, text: Localization.TooltipChangeIcon.lang());
        }

        UIButton clearButton = iconMenu.AddButton(idLang: Localization.ClearIcon, action: delegate
        {
            SetContainerIcon(state: state, target: target, iconIndex: 0);
        }, hideAfter: false);
        SetTooltip(button: clearButton, text: Localization.TooltipClearIcon.lang());

        GridLayoutGroup parent = iconMenu.AddGridLayout();
        int index = 0;
        foreach (UnityEngine.Sprite sprite in EClass.core.refs.spritesContainerIcon)
        {
            UIButton button = Util.Instantiate<UIButton>(path: "UI/Element/Button/ButtonContainerIcon", parent: parent);
            int iconIndex = index;
            button.icon.sprite = sprite;
            SetTooltip(button: button, text: Localization.TooltipSetIcon.lang(iconIndex.ToString(), null, null, null, null));
            button.SetOnClick(action: delegate
            {
                SetContainerIcon(state: state, target: target, iconIndex: iconIndex);
            });
            index++;
        }
    }

    private static void RefreshUpgradeMenuLabels(UpgradeMenuState state)
    {
        UpgradeMenuContext context = state.Context;
        RefreshUpgradeButton(button: state.IncreaseWidthButton, label: GetUpgradeLabel(context: context, action: ContainerResizeAction.IncreaseWidth), tooltip: GetUpgradeTooltip(context: context, action: ContainerResizeAction.IncreaseWidth));
        RefreshUpgradeButton(button: state.IncreaseHeightButton, label: GetUpgradeLabel(context: context, action: ContainerResizeAction.IncreaseHeight), tooltip: GetUpgradeTooltip(context: context, action: ContainerResizeAction.IncreaseHeight));
        RefreshUpgradeButton(button: state.IncreaseCapacityButton, label: GetUpgradeLabel(context: context, action: ContainerResizeAction.IncreaseCapacity), tooltip: GetUpgradeTooltip(context: context, action: ContainerResizeAction.IncreaseCapacity));
        RefreshUpgradeButton(button: state.DecreaseWidthButton, label: GetUpgradeLabel(context: context, action: ContainerResizeAction.DecreaseWidth), tooltip: GetUpgradeTooltip(context: context, action: ContainerResizeAction.DecreaseWidth));
        RefreshUpgradeButton(button: state.DecreaseHeightButton, label: GetUpgradeLabel(context: context, action: ContainerResizeAction.DecreaseHeight), tooltip: GetUpgradeTooltip(context: context, action: ContainerResizeAction.DecreaseHeight));
        RefreshUpgradeButton(button: state.ResetSizeButton, label: GetUpgradeLabel(context: context, action: ContainerResizeAction.ResetSize), tooltip: GetUpgradeTooltip(context: context, action: ContainerResizeAction.ResetSize));
        RefreshUpgradeButton(button: state.ToggleCoolingButton, label: GetUpgradeLabel(context: context, action: ContainerResizeAction.ToggleCooling), tooltip: GetUpgradeTooltip(context: context, action: ContainerResizeAction.ToggleCooling));

        if (context.Target != null)
        {
            RefreshIconMenuLabel(menu: state.ChangeIconMenu, target: context.Target);
        }
    }

    private static void RefreshIconMenuLabel(UIContextMenu? menu, Thing target)
    {
        if (menu == null || menu.popper == null || menu.popper.textName == null)
        {
            return;
        }

        menu.popper.textName.text = Localization.ChangeIcon.lang(GetContainerIconLabel(target: target), null, null, null, null);
    }

    private static void RefreshUpgradeButton(UIButton? button, string label, string tooltip)
    {
        if (button == null)
        {
            return;
        }

        UIContextMenuItem item = button.gameObject.GetComponentInParent<UIContextMenuItem>();
        if (item != null && item.textName != null)
        {
            item.textName.text = label;
        }

        button.tooltip.text = tooltip;
    }

    private static string GetContainerIconLabel(Thing target)
    {
        if (target.c_indexContainerIcon == 0)
        {
            return Localization.IconNone.lang();
        }

        return target.c_indexContainerIcon.ToString();
    }

    private static void SetContainerIcon(UpgradeMenuState state, Thing target, int iconIndex)
    {
        target.c_indexContainerIcon = iconIndex;
        ContainerUpgradeActions.RefreshAfterChange(target: target);
        SE.ClickOk();
        RefreshUpgradeMenuLabels(state: state);
    }

    private static bool ApplyUpgradeAction(UpgradeMenuContext context, ContainerResizeAction action)
    {
        if (context.Target != null)
        {
            return ContainerUpgradeActions.TryApplyMenuAction(target: context.Target, action: action);
        }

        return ContainerUpgradeActions.TryApplyPlayerMenuAction(action: action);
    }

    private static void SetTooltip(UIButton button, string text)
    {
        if (button == null)
        {
            return;
        }

        button.tooltip.text = text;
        button.tooltip.enable = true;
        button.tooltip.icon = true;
        button.tooltip.offset = new UnityEngine.Vector3(0f, 48f, 0f);
        AddTooltipPointerHandlers(button: button);
    }

    private static void SetTooltip(Button button, string text)
    {
        if (button is UIButton uiButton == false)
        {
            return;
        }

        SetTooltip(button: uiButton, text: text);
    }

    private static void AddTooltipPointerHandlers(UIButton button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener(call: delegate(BaseEventData _)
        {
            UIButton.lastHovered = button;
            UIButton.actionTooltip = delegate
            {
                button.ShowTooltipForced(ignoreWhenRightClick: true);
                if (TooltipManager.Instance.tooltips.Length > 0)
                {
                    TooltipManager.Instance.tooltips[0].hideFunc = () => false;
                }
            };
        });
        trigger.triggers.Add(item: entry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        exitEntry.callback.AddListener(call: delegate(BaseEventData _)
        {
            TooltipManager.Instance.HideTooltips(immediate: false);
        });
        trigger.triggers.Add(item: exitEntry);
    }

    private static bool ApplyMenuAction(Func<bool> action, Thing? target)
    {
        if (action())
        {
            ContainerUpgradeActions.PlaySuccess(target: target);
            SE.ClickOk();
            return true;
        }

        SE.Beep();
        return false;
    }

    private sealed class UpgradeMenuState
    {
        internal UpgradeMenuState(UpgradeMenuContext context)
        {
            Context = context;
        }

        internal UpgradeMenuContext Context { get; }

        internal UIButton? IncreaseWidthButton { get; set; }

        internal UIButton? IncreaseHeightButton { get; set; }

        internal UIButton? IncreaseCapacityButton { get; set; }

        internal UIButton? DecreaseWidthButton { get; set; }

        internal UIButton? DecreaseHeightButton { get; set; }

        internal UIButton? ResetSizeButton { get; set; }

        internal UIButton? ToggleCoolingButton { get; set; }

        internal UIContextMenu? ChangeIconMenu { get; set; }
    }

    private readonly struct UpgradeMenuContext
    {
        private UpgradeMenuContext(Thing? target, ThingContainer things, bool isPlayer)
        {
            Target = target;
            Things = things;
            IsPlayer = isPlayer;
        }

        internal Thing? Target { get; }

        internal ThingContainer Things { get; }

        internal bool CoolingEnabled
        {
            get
            {
                if (IsPlayer)
                {
                    return EClass.pc.c_containerUpgrade.cool > 0;
                }

                if (Target == null)
                {
                    return false;
                }

                return Target.c_containerUpgrade.cool > 0;
            }
        }

        private bool IsPlayer { get; }

        internal string GridNameAddsColumnsTooltip
        {
            get
            {
                if (IsPlayer)
                {
                    return Localization.TooltipAddsPlayerColumns.lang();
                }

                return Localization.TooltipAddsContainerColumns.lang();
            }
        }

        internal string GridNameAddsRowsTooltip
        {
            get
            {
                if (IsPlayer)
                {
                    return Localization.TooltipAddsPlayerRows.lang();
                }

                return Localization.TooltipAddsContainerRows.lang();
            }
        }

        internal string GridNameAddsSlotsTooltip
        {
            get
            {
                if (IsPlayer)
                {
                    return Localization.TooltipAddsPlayerSlots.lang();
                }

                return Localization.TooltipAddsContainerSlots.lang();
            }
        }

        internal string GridNameRemovesColumnsTooltip
        {
            get
            {
                if (IsPlayer)
                {
                    return Localization.TooltipRemovesPlayerColumns.lang();
                }

                return Localization.TooltipRemovesContainerColumns.lang();
            }
        }

        internal string GridNameRemovesRowsTooltip
        {
            get
            {
                if (IsPlayer)
                {
                    return Localization.TooltipRemovesPlayerRows.lang();
                }

                return Localization.TooltipRemovesContainerRows.lang();
            }
        }

        internal string ResetTooltip
        {
            get
            {
                if (IsPlayer)
                {
                    return Localization.TooltipResetPlayerSize.lang();
                }

                return Localization.TooltipResetContainerSize.lang();
            }
        }

        internal string EnableCoolingTooltip
        {
            get
            {
                if (IsPlayer)
                {
                    return Localization.TooltipEnablePlayerCooling.lang();
                }

                return Localization.TooltipEnableContainerCooling.lang();
            }
        }

        internal string DisableCoolingTooltip
        {
            get
            {
                if (IsPlayer)
                {
                    return Localization.TooltipDisablePlayerCooling.lang();
                }

                return Localization.TooltipDisableContainerCooling.lang();
            }
        }

        internal static UpgradeMenuContext ForContainer(Thing target)
        {
            return new UpgradeMenuContext(target: target, things: target.things, isPlayer: false);
        }

        internal static UpgradeMenuContext ForPlayer()
        {
            return new UpgradeMenuContext(target: null, things: EClass.pc.things, isPlayer: true);
        }
    }
}
