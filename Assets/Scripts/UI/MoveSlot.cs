using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Pokemon DragonTales/GUI/Move Slot")]
public class MoveSlot :IconSlot
{
    public CooldownController cooldownController;
    public GameObject SlotFrame;
    private MoveData moveInfo;
    public int slotID;
    private static Dictionary<int, MoveSlot> slots = new Dictionary<int, MoveSlot>();
    void Awake()
    {
        if (slots.ContainsKey(this.slotID))
        {
            Debug.LogWarning("IconSlot: An IconSlot with ID (" + this.slotID + ") already exists, please consider changing it.");

            // Fix it, make sure we dont make a dead loop
            while (slots.ContainsKey(this.slotID) && this.slotID < 100)
                this.slotID++;
        }

        slots.Add(this.slotID, this);

    }
    void OnEnable()
    {
       // PlayerControlManager.SelectMove += ActivateFrame;
    }
    void OnDisable()
    {
        //PlayerControlManager.SelectMove -= ActivateFrame;
    }
    public override void OnStart()
    {

        // Try finding the cooldown handler
        if (this.cooldownController == null) this.cooldownController = this.GetComponent<CooldownController>();
        if (this.cooldownController == null) this.cooldownController = this.GetComponentInChildren<CooldownController>();
    }


    private void ActivateFrame(int selectedMoveIndex)
    {
        if (SlotFrame != null)
        {
            if (selectedMoveIndex == this.slotID)
                SlotFrame.SetActive(true);
            else
            {
                if (SlotFrame.activeInHierarchy == true)
                    SlotFrame.SetActive(false);
            }
        }
    }
    /// <summary>
    /// Gets the spell info of the spell assigned to this slot.
    /// </summary>
    /// <returns>The spell info.</returns>
    public MoveData GetSpellInfo()
    {
        return moveInfo;
    }

    /// <summary>
    /// Determines whether this slot is assigned.
    /// </summary>
    /// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
    public override bool IsAssigned()
    {
        return (this.moveInfo != null);
    }

    /// <summary>
    /// Assign the slot by spell info.
    /// </summary>
    /// <param name="spellInfo">Spell info.</param>
    public bool Assign(MoveData moveInfo)
    {
        if (moveInfo == null)
            return false;

        // Use the base class assign
        //if (this.Assign(moveInfo.IconName)) // *************** Needs to check for string
        //{
        //    // Set the spell info
        //    this.moveInfo = moveInfo;

        //    // Check if we have a cooldown handler
        //    if (this.cooldownController != null)
        //        this.cooldownController.OnAssignSpell(moveInfo);

        //    // Success
        //    return true;
        //}

        return false;
    }

    /// <summary>
    /// Assign the slot by the passed source slot.
    /// </summary>
    /// <param name="source">Source.</param>
    public override bool Assign(Object source)
    {
        if (source is MoveSlot)
        {
            MoveSlot sourceSlot = source as MoveSlot;

            if (sourceSlot != null)
                return this.Assign(sourceSlot.GetSpellInfo());
        }

        // Default
        return false;
    }

    /// <summary>
    /// Unassign this slot.
    /// </summary>
    public override void Unassign()
    {
        // Remove the icon
        base.Unassign();

        // Clear the spell info
        this.moveInfo = null;

        // Check if we have a cooldown handler
        if (this.cooldownController != null)
            this.cooldownController.OnUnassign();
    }

    /// <summary>
    /// Determines whether this slot can swap with the specified target slot.
    /// </summary>
    /// <returns><c>true</c> if this instance can swap with the specified target; otherwise, <c>false</c>.</returns>
    /// <param name="target">Target.</param>
    public override bool CanSwapWith(Object target)
    {
        return (target is MoveSlot);
    }

    /// <summary>
    /// Raises the click event.
    /// </summary>
    public void CastMoveUI()
    {
        if (!this.IsAssigned())
            return;

        // Check if the slot is on cooldown
        if (this.cooldownController != null)
        {


            this.cooldownController.StartCooldown(this.moveInfo.Cooldown);
        }
    }

    public static MoveSlot GetSlot(int id)
    {
        if (slots.ContainsKey(id))
            return slots[id];

        return null;
    }
    /// <summary>
    /// Raises the tooltip event.
    /// </summary>
    /// <param name="show">If set to <c>true</c> show.</param>
    /// /*

    /*
    public override void OnTooltip(bool show)
    {
        if (show && this.IsAssigned())
        {
            // Set the title and description
            FrozenUI_Tooltip.SetTitle(this.moveInfo.Name);
            FrozenUI_Tooltip.SetDescription(this.moveInfo.Description);
            if (this.moveInfo.Flags.Has(SpellInfo_Flags.Passive))
            {
                FrozenUI_Tooltip.AddAttribute("Passive", "");
            }
            else
            {
                // Power consumption
                if (this.moveInfo.PowerCost > 0f)
                {
                    if (this.moveInfo.Flags.Has(SpellInfo_Flags.PowerCostInPct))
                        FrozenUI_Tooltip.AddAttribute(this.moveInfo.PowerCost.ToString("0") + "%", " Energy");
                    else
                        FrozenUI_Tooltip.AddAttribute(this.spellInfo.PowerCost.ToString("0"), " Energy");
                }
                // Range
                if (this.spellInfo.Range > 0f)
                {
                    if (this.spellInfo.Range == 1f)
                        FrozenUI_Tooltip.AddAttribute("Melee range", "");
                    else
                        FrozenUI_Tooltip.AddAttribute(this.spellInfo.Range.ToString("0"), " yd range");
                }
                // Cast time
                if (this.spellInfo.CastTime == 0f)
                    FrozenUI_Tooltip.AddAttribute("Instant", "");
                else
                    FrozenUI_Tooltip.AddAttribute(this.spellInfo.CastTime.ToString("0.0"), " sec cast");
                // Cooldown
                if (this.spellInfo.Cooldown > 0f)
                    FrozenUI_Tooltip.AddAttribute(this.spellInfo.Cooldown.ToString("0.0"), " sec cooldown");
            }
            // Set the tooltip position
            FrozenUI_Tooltip.SetPosition(this.iconSprite as UIWidget);
            // Show the tooltip
            FrozenUI_Tooltip.Show();
            // Prevent hide
            return;
        }
        // Default hide
        FrozenUI_Tooltip.Hide();
     * 
    }*/
}