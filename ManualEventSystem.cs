
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Bloodstone.API;
using Bloody.Core;
using ProjectM;
using ProjectM.Network;
using Unity.Entities;
using UnityEngine;

public static class ManualEventSystem
{
    public static List<ulong> BannedPlayers = new List<ulong>();
    public static Dictionary<Entity, Vector3> PlayerPositions = new Dictionary<Entity, Vector3>();
    public static List<Entity> EventPlayers = new List<Entity>();
    public static Vector3 EventStartPosition;
    public static string bannedPlayersJsonLocation = "BepInEx/config/EventManager/bannedplayers.json";
    public static bool EventActive = false;
    public static bool EventPaused = false;

    public static void Initialize()
    {
        Directory.CreateDirectory("BepInEx/config/EventManager");
        if (File.Exists(bannedPlayersJsonLocation))
        {
            try
            {
                string playersdata = File.ReadAllText(bannedPlayersJsonLocation);
                BannedPlayers = JsonSerializer.Deserialize<List<ulong>>(playersdata, new JsonSerializerOptions { WriteIndented = true }) ?? new List<ulong>();
            }
            catch (Exception ex)
            {
                Plugin.logger.LogError($"Failed to load banned players: {ex.Message}");
                BannedPlayers = new List<ulong>(); // Fallback to empty list
            }
        }
    }

    public static void StartEvent(Entity character, string eventName)
    {
        EventStartPosition = Helper.GetPlayerPosition(character);
        EventActive = true;
        EventPlayers.Clear();
        PlayerPositions.Clear();

        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, $"Event '{eventName}' started. Players can join now using .event join");
    }

    public static void EndEvent()
    {
        foreach (var (player, position) in PlayerPositions)
        {
            Helper.TeleportPlayer(player, position);
        }
        EventPlayers.Clear();
        PlayerPositions.Clear();
        EventActive = false;
        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, "Event ended and all players were teleported back to their original positions.");
    }

    public static void BanPlayer(Entity playerEntity)
    {
        string playerName = playerEntity.Read<ControlledBy>().Controller.Read<User>().CharacterName.Value;
        ulong steamID = playerEntity.Read<ControlledBy>().Controller.Read<User>().PlatformId;

        if (PlayerPositions.ContainsKey(playerEntity))
        {
            EventPlayers.Remove(playerEntity);
            Helper.TeleportPlayer(playerEntity, PlayerPositions[playerEntity]);
            PlayerPositions.Remove(playerEntity);
        }

        if (File.Exists(bannedPlayersJsonLocation))
        {
            string jsondata = File.ReadAllText(bannedPlayersJsonLocation);
            var list = JsonSerializer.Deserialize<List<ulong>>(jsondata, new JsonSerializerOptions { WriteIndented = true });
            BannedPlayers.Add(steamID);
            list.Add(steamID);
            var jsonData = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(bannedPlayersJsonLocation, jsonData);
        }
        else
        {
            BannedPlayers.Add(steamID);
            var jsonData = JsonSerializer.Serialize(BannedPlayers, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(bannedPlayersJsonLocation, jsonData);
        }

        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, $"Player {playerName} has been banned from events.");
    }

    public static void UnbanPlayer(ulong steamID, string playerName)
    {
        string jsondata = File.ReadAllText(bannedPlayersJsonLocation);
        var list = JsonSerializer.Deserialize<List<ulong>>(jsondata, new JsonSerializerOptions { WriteIndented = true });
        BannedPlayers.Remove(steamID);
        list.Remove(steamID);
        var jsonData = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(bannedPlayersJsonLocation, jsonData);

        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, $"Player {playerName} has been unbanned from events.");
    }

    public static void KickPlayer(Entity playerEntity, string playerName)
    {
        if (PlayerPositions.ContainsKey(playerEntity))
        {
            EventPlayers.Remove(playerEntity);
            Helper.TeleportPlayer(playerEntity, PlayerPositions[playerEntity]);
            PlayerPositions.Remove(playerEntity);
        }

        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, $"Player {playerName} has been kicked from the event.");
    }
    public static bool GatherEventPlayers(Entity adminEntity)
    {
        if (EventActive)
        {
            foreach (var player in EventPlayers)
            {
                var adminPosition = Helper.GetPlayerPosition(adminEntity);
                Helper.TeleportPlayer(player, adminPosition);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public static void PauseEvent()
    {
        EventPaused = !EventPaused;
    }
    public static void JoinEvent(Entity playerEntity)
    {
        var currentPosition = Helper.GetPlayerPosition(playerEntity);
        EventPlayers.Add(playerEntity);
        PlayerPositions[playerEntity] = currentPosition;

        Helper.TeleportPlayer(playerEntity, EventStartPosition);
    }
    public static void LeaveEvent(Entity playerEntity)
    {
        EventPlayers.Remove(playerEntity);
        if (PlayerPositions.ContainsKey(playerEntity))
        {
            Helper.TeleportPlayer(playerEntity, PlayerPositions[playerEntity]);
            PlayerPositions.Remove(playerEntity);
        }
    }
}