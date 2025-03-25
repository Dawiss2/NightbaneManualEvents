using BepInEx;
using BepInEx.Logging;
using VampireCommandFramework;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace NightbaneManualEvents;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[BepInDependency("gg.deca.Bloodstone")]
[Bloodstone.API.Reloadable]
public class Plugin : BasePlugin
{
    Harmony _harmony;
    public static ManualLogSource logger;

    public override void Load()
    {
        // Plugin startup logic
        logger = Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

        // Harmony patching
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

        // Register all commands in the assembly with VCF
        CommandRegistry.RegisterAll();
        ManualEventSystem.Initialize();
    }

    public override bool Unload()
    {
        CommandRegistry.UnregisterAssembly();
        _harmony?.UnpatchSelf();
        return true;
    }
}
// // Uncomment for example commmand or delete

// /// <summary> 
// /// Example VCF command that demonstrated default values and primitive types
// /// Visit https://github.com/decaprime/VampireCommandFramework for more info 
// /// </summary>
// /// <remarks>
// /// How you could call this command from chat:
// ///
// /// .eventmanager-example "some quoted string" 1 1.5
// /// .eventmanager-example boop 21232
// /// .eventmanager-example boop-boop
// ///</remarks>
// [Command("eventmanager-example", description: "Example command from eventmanager", adminOnly: true)]
// public void ExampleCommand(ICommandContext ctx, string someString, int num = 5, float num2 = 1.5f)
// { 
//     ctx.Reply($"You passed in {someString} and {num} and {num2}");
// }
