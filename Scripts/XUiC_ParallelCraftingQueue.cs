using System;
public class XUiC_ParallelCraftingQueue : XUiC_CraftingQueue
{
    public override void Init()
    {
        base.Init();
        Log.Warning("InitCrafting");
        XUiController[] childrenByType = base.GetChildrenByType<XUiC_RecipeStack>(null);
        this.queueItems = childrenByType;
        for (int i = 0; i < this.queueItems.Length; i++)
        {
            ((XUiC_RecipeStack)this.queueItems[i]).Owner = this;
        }
    }
    public override void ClearQueue()
    {
        Log.Out("ClearQueue");
        for (int i = this.queueItems.Length - 1; i >= 0; i--)
        {
            XUiC_RecipeStack xuiC_RecipeStack = (XUiC_RecipeStack)this.queueItems[i];
            xuiC_RecipeStack.SetRecipe(null, 0, 0f, true, -1, -1, -1f);
            xuiC_RecipeStack.IsCrafting = false;
            xuiC_RecipeStack.IsDirty = true;
        }
    }

    public override void HaltCrafting()
    {
        Log.Out("HaltCrafting");
        foreach (XUiController item in this.queueItems)
            ((XUiC_RecipeStack)item).IsCrafting = false;
    }

    public override void ResumeCrafting()
    {
        Log.Out("ResumeCrating");
        foreach(XUiController item in this.queueItems)
            ((XUiC_RecipeStack)item).IsCrafting = true;
    }

    public override bool IsCrafting()
    {
        Log.Out("IsCrafting");
        // If 1 item is crafting then all are crafting
        return ((XUiC_RecipeStack)this.queueItems[this.queueItems.Length - 1]).IsCrafting;
    }

    public override bool AddItemToRepair(float _repairTimeLeft, ItemValue _itemToRepair, int _amountToRepair)
    {
        Log.Out("AddItemToRepair");
        for (int i = this.queueItems.Length - 1; i >= 0; i--)
        {
            XUiC_RecipeStack xuiC_RecipeStack = (XUiC_RecipeStack)this.queueItems[i];
            if (!xuiC_RecipeStack.HasRecipe() && xuiC_RecipeStack.SetRepairRecipe(_repairTimeLeft, _itemToRepair, _amountToRepair))
            {
                xuiC_RecipeStack.IsCrafting = (i == this.queueItems.Length - 1);
                xuiC_RecipeStack.IsDirty = true;
                return true;
            }
        }
        return false;
    }

    public override bool AddRecipeToCraft(Recipe _recipe, int _count = 1, float craftTime = -1f, bool isCrafting = true, float _oneItemCraftingTime = -1f)
    {
        Log.Out("AddRecipeToCraft");
        for (int i = this.queueItems.Length - 1; i >= 0; i--)
        {
            if (this.AddRecipeToCraftAtIndex(i, _recipe, _count, craftTime, isCrafting, false, -1, -1, _oneItemCraftingTime))
                return true;
        }
        return false;
    }

    public override bool AddRecipeToCraftAtIndex(int _index, Recipe _recipe, int _count = 1, float craftTime = -1f, bool isCrafting = true, bool recipeModification = false, int lastQuality = -1, int startingEntityId = -1, float _oneItemCraftingTime = -1f)
    {
        Log.Out("AddRepipeToCraftAtIndex");
        XUiC_RecipeStack xuiC_RecipeStack = (XUiC_RecipeStack)this.queueItems[_index];
        if (xuiC_RecipeStack.SetRecipe(_recipe, _count, craftTime, recipeModification, -1, -1, _oneItemCraftingTime))
        {
            xuiC_RecipeStack.IsCrafting = isCrafting;
            if (lastQuality != -1)
                xuiC_RecipeStack.OutputQuality = lastQuality;
            if (startingEntityId != -1)
                xuiC_RecipeStack.StartingEntityId = startingEntityId;
            xuiC_RecipeStack.IsDirty = true;
            return true;
        }
        return false;
    }

    public override bool AddItemToRepairAtIndex(int _index, float _repairTimeLeft, ItemValue _itemToRepair, int _amountToRepair, bool isCrafting = true, int startingEntityId = -1)
    {
        Log.Out("AddItemToRepairAtIndex");
        XUiC_RecipeStack xuiC_RecipeStack = (XUiC_RecipeStack)this.queueItems[_index];
        if (xuiC_RecipeStack.SetRepairRecipe(_repairTimeLeft, _itemToRepair, _amountToRepair))
        {
            xuiC_RecipeStack.IsCrafting = true;
            xuiC_RecipeStack.StartingEntityId = ((startingEntityId != -1) ? startingEntityId : xuiC_RecipeStack.StartingEntityId);
            xuiC_RecipeStack.IsDirty = true;
            return true;
        }
        return false;
    }

    public override XUiC_RecipeStack[] GetRecipesToCraft()
    {
        Log.Out("GetRecipesToCraft");
        XUiC_RecipeStack[] array = new XUiC_RecipeStack[this.queueItems.Length];
        for (int i = 0; i < array.Length; i++)
            array[i] = (XUiC_RecipeStack)this.queueItems[i];

        return array;
    }

    public override void Update(float _dt)
    {
        Log.Out("Update");
        base.Update(_dt);
        bool flag = false;
        for (int i = this.queueItems.Length - 1; i >= 0; i--)
        {
            if (((XUiC_RecipeStack)this.queueItems[i]).HasRecipe())
            {
                flag = true;
                break;
            }
        }
        if (this.toolGrid != null)
            this.toolGrid.SetToolLocks(flag);

        if (!flag)
            return;

        XUiC_RecipeStack xuiC_RecipeStack = (XUiC_RecipeStack)this.queueItems[this.queueItems.Length - 1];
        // Not sure what it does
        if (!xuiC_RecipeStack.HasRecipe())
        {
            for (int j = this.queueItems.Length - 1; j >= 0; j--)
            {
                XUiC_RecipeStack recipeStack = (XUiC_RecipeStack)this.queueItems[j];
                if (j != 0)
                    ((XUiC_RecipeStack)this.queueItems[j - 1]).CopyTo(recipeStack);
                else
                    ((XUiC_RecipeStack)this.queueItems[0]).SetRecipe(null, 0, 0f, true, -1, -1, -1f);
            }
        }
        if (xuiC_RecipeStack.HasRecipe() && !xuiC_RecipeStack.IsCrafting)
            xuiC_RecipeStack.IsCrafting = true;
    }

    public override void OnOpen()
    {
        Log.Out("OnOpen");
        base.OnOpen();
        this.toolGrid = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationToolGrid>();
    }

    public override void RefreshQueue()
    {
        Log.Out("RefreshQueue");
        XUiC_RecipeStack xuiC_RecipeStack = (XUiC_RecipeStack)this.queueItems[this.queueItems.Length - 1];
        for (int i = this.queueItems.Length - 2; i >= 0; i--)
        {
            XUiC_RecipeStack xuiC_RecipeStack2 = (XUiC_RecipeStack)this.queueItems[i];
            if (xuiC_RecipeStack2.GetRecipe() != null && xuiC_RecipeStack.GetRecipe() == null)
            {
                xuiC_RecipeStack2.CopyTo(xuiC_RecipeStack);
                xuiC_RecipeStack2.SetRecipe(null, 0, 0f, true, -1, -1, -1f);
            }
            xuiC_RecipeStack = xuiC_RecipeStack2;
        }
    }

    private XUiC_WorkstationToolGrid toolGrid;

    private XUiController[] queueItems;

    public static String instructions = "";
    public static void PrintInstructions()
    {
        Log.Out(instructions);
    }
}
