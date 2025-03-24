
using Bloodstone.API;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public static class Helper
{
    public static bool IsInventoryEmpty(Equipment inventory)
    {
        NativeList<Entity> equippedItems = new(Allocator.Temp);
        try
        {
            inventory.GetAllEquipmentEntities(equippedItems);
            foreach (var item in equippedItems)
            {
                if (item != Entity.Null)
                {
                    return false;
                }
            }
            return true;
        }
        finally
        {
            equippedItems.Dispose();
        }
    }

    public static Vector3 GetPlayerPosition(Entity playerEntity)
    {
        var entityManager = VWorld.Server.EntityManager;
        var translation = entityManager.GetComponentData<Translation>(playerEntity);
        return translation.Value;
    }

    public static void TeleportPlayer(Entity playerEntity, Vector3 position)
    {
        var entityManager = VWorld.Server.EntityManager;
        entityManager.SetComponentData(playerEntity, new LastTranslation { Value = position });
        entityManager.SetComponentData(playerEntity, new Translation { Value = position });
    }

    public static Entity FindPlayerByName(string playerName)
    {
        var entityManager = VWorld.Server.EntityManager;
        NativeArray<Entity> players = VWorld.Server.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>()).ToEntityArray(Allocator.Temp);

        try
        {
            foreach (var player in players)
            {
                var user = entityManager.GetComponentData<User>(player);
                if (user.CharacterName.ToString() == playerName)
                {
                    return user.LocalCharacter._Entity;
                }
            }
            return Entity.Null;
        }
        finally
        {
            players.Dispose();
        }
    }
}