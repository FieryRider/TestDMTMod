using System;
using SDX.Compiler;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System.Collections.Generic;

public class XUiC_CraftingQueueVirtual : IPatcherMod
{
    public bool Patch(ModuleDefinition module)
    {
        Console.WriteLine("===XUiC_CraftingQueueVirtual Patcher===");

        TypeDefinition xUiC_CraftingQueueTypeDef = module.Types.First(d => d.Name == "XUiC_CraftingQueue");
        List<MethodDefinition> methods = new List<MethodDefinition>();
        methods.Add(xUiC_CraftingQueueTypeDef.Methods.First(m => m.Name == "ClearQueue"));
        methods.Add(xUiC_CraftingQueueTypeDef.Methods.First(m => m.Name == "HaltCrafting"));
        methods.Add(xUiC_CraftingQueueTypeDef.Methods.First(m => m.Name == "ResumeCrafting"));
        methods.Add(xUiC_CraftingQueueTypeDef.Methods.First(m => m.Name == "IsCrafting"));
        methods.Add(xUiC_CraftingQueueTypeDef.Methods.First(m => m.Name == "AddItemToRepair"));
        methods.Add(xUiC_CraftingQueueTypeDef.Methods.First(m => m.Name == "AddRecipeToCraft"));
        methods.Add(xUiC_CraftingQueueTypeDef.Methods.First(m => m.Name == "AddRecipeToCraftAtIndex"));
        methods.Add(xUiC_CraftingQueueTypeDef.Methods.First(m => m.Name == "AddItemToRepairAtIndex"));
        methods.Add(xUiC_CraftingQueueTypeDef.Methods.First(m => m.Name == "GetRecipesToCraft"));
        methods.Add(xUiC_CraftingQueueTypeDef.Methods.First(m => m.Name == "Update"));
        methods.Add(xUiC_CraftingQueueTypeDef.Methods.First(m => m.Name == "OnOpen"));
        methods.Add(xUiC_CraftingQueueTypeDef.Methods.First(m => m.Name == "RefreshQueue"));

        foreach (MethodDefinition method in methods)
            method.IsVirtual = true;

        return true;
    }


    /* Called after the patching process and after scripts are compiled.
     * Used to link references between both assemblies
     * Return true if successful
     */
    public bool Link(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        return true;
    }


    // Helper functions to allow us to access and change variables that are otherwise unavailable.
    private void SetMethodToVirtual(MethodDefinition meth)
    {
        meth.IsVirtual = true;
    }

    private void SetFieldToPublic(FieldDefinition field)
    {
        field.IsFamily = false;
        field.IsPrivate = false;
        field.IsPublic = true;
    }

    private void SetMethodToPublic(MethodDefinition field)
    {
        field.IsFamily = false;
        field.IsPrivate = false;
        field.IsPublic = true;
    }
}