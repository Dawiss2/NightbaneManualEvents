

using Bloodstone.API;
using Bloody.Core;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.EventSystems;

[HarmonyPatch(typeof(VampireDownedServerEventSystem), nameof(VampireDownedServerEventSystem.OnUpdate))]
public static class VampireDownedHook
{
    public static void Prefix(VampireDownedServerEventSystem __instance)
    {
        var downedEvents = __instance.__query_1174204813_0.ToEntityArray(Allocator.Temp);
        try
        {
            foreach (var entity in downedEvents)
            {
                Entity characterEntity;
                VampireDownedServerEventSystem.TryFindRootOwner(entity, 1, VWorld.Server.EntityManager, out characterEntity);

                if (ManualEventSystem.EventPlayers.Contains(characterEntity))
                {
                    ManualEventSystem.LeaveEvent(characterEntity);
                    User user = characterEntity.Read<ControlledBy>().Controller.Read<User>();
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, $"<color=red>You have been removed from event.");
                }
            }
        }
        finally
        {
            downedEvents.Dispose();
        }
    }
}