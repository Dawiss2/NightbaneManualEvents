using Bloodstone.API;
using Bloody.Core;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;

[HarmonyPatch(typeof(KillEventSystem), nameof(KillEventSystem.OnUpdate))]
public static class DeathEventListenerHook
{
    public static void Prefix(KillEventSystem __instance)
    {
        var entities = __instance.__query_463356032_0.ToEntityArray(Allocator.Temp);

        try
        {
            foreach (var entity in entities)
            {
                if (entity.Has<KillEvent>() && entity.Has<FromCharacter>())
                {
                    FromCharacter fromCharacter = entity.Read<FromCharacter>();
                    KillEvent killEvent = entity.Read<KillEvent>();

                    if (killEvent.Who.ToString() == "Self")
                    {
                        if (ManualEventSystem.EventPlayers.Contains(fromCharacter.Character))
                        {
                            VWorld.Server.EntityManager.DestroyEntity(entity);
                            ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, fromCharacter.User.Read<User>(), $"<color=red>You can't use unstuck inside an Event.");
                        }
                    }
                }
            }
        }
        finally
        {
            entities.Dispose();
        }
    }
}