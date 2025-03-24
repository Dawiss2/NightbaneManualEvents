using ProjectM;
using ProjectM.Network;
using Unity.Entities;
using VampireCommandFramework;
using Bloodstone.API;
using System.ComponentModel;
using System.IO;
using Bloody.Core;


[CommandGroup("event"), Description("")]
public static class Commands
{
    [Command("pause", description: "", adminOnly: true)]
    public static void PauseEventCommand(ChatCommandContext ctx)
    {
        ManualEventSystem.PauseEvent();
        ctx.Reply($"<color=green>The event is now {(ManualEventSystem.EventPaused ? "paused" : "resumed")}.</color>");
        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, $"The event is now {(ManualEventSystem.EventPaused ? "closed to new participants" : "open again")}. Stay tuned!");
    }

    [Command("count", description: "", adminOnly: true)]
    public static void EventPlayersCountCommand(ChatCommandContext ctx)
    {
        int count = ManualEventSystem.EventPlayers.Count;
        ctx.Reply($"There are {count} Players in Event");
    }

    [Command("start", description: "", adminOnly: true)]
    public static void EventStartCommand(ChatCommandContext ctx, string eventName)
    {
        if (ManualEventSystem.EventActive)
        {
            ctx.Reply("An event is already active.");
            return;
        }

        ManualEventSystem.StartEvent(ctx.Event.SenderCharacterEntity, eventName);
    }

    [Command("end", description: "", adminOnly: true)]
    public static void EventEndCommand(ChatCommandContext ctx)
    {
        if (!ManualEventSystem.EventActive)
        {
            ctx.Reply("No event is currently active.");
            return;
        }

        ManualEventSystem.EndEvent();
    }

    [Command("ban", description: "", adminOnly: true)]
    public static void EventBanCommand(ChatCommandContext ctx, string playerName)
    {
        Entity playerEntity = Helper.FindPlayerByName(playerName);

        if (playerEntity == Entity.Null)
        {
            ctx.Reply($"Player {playerName} does not exist.");
            return;
        }

        ManualEventSystem.BanPlayer(playerEntity);
    }

    [Command("unban", description: "", adminOnly: true)]
    public static void EventUnbanCommand(ChatCommandContext ctx, string playerName)
    {

        if (!File.Exists(ManualEventSystem.bannedPlayersJsonLocation))
        {
            ctx.Reply($"There are no banned players.");
            return;
        }

        Entity playerEntity = Helper.FindPlayerByName(playerName);

        if (playerEntity == Entity.Null)
        {
            ctx.Reply($"Player {playerName} does not exist.");
            return;
        }

        ulong steamID = playerEntity.Read<ControlledBy>().Controller.Read<User>().PlatformId;
        if (!ManualEventSystem.BannedPlayers.Contains(steamID))
        {
            ctx.Reply($"Player {playerName} is not banned.");
            return;
        }

        ManualEventSystem.UnbanPlayer(steamID, playerName);
    }

    [Command("kick", description: "", adminOnly: true)]
    public static void EventKickCommand(ChatCommandContext ctx, string playerName)
    {
        Entity playerEntity = Helper.FindPlayerByName(playerName);

        if (playerEntity == Entity.Null || !ManualEventSystem.EventPlayers.Contains(playerEntity))
        {
            ctx.Reply($"Player {playerName} is not in the event or does not exist.");
            return;
        }

        ManualEventSystem.KickPlayer(playerEntity, playerName);
    }

    [Command("gather", description: "", adminOnly: true)]
    public static void GatherEventPlayersCommand(ChatCommandContext ctx)
    {
        bool success = ManualEventSystem.GatherEventPlayers(ctx.Event.SenderCharacterEntity);

        if (success)
        {
            ctx.Reply("<color=green>Teleported all event players to you.");
        }
        else
        {
            ctx.Reply("<color=red>There is no event active right now.");
        }
    }

    [Command("join", description: "", adminOnly: false)]
    public static void EventJoinCommand(ChatCommandContext ctx)
    {
        var user = ctx.Event.SenderUserEntity;
        var playerEntity = ctx.Event.SenderCharacterEntity;
        var entityManager = VWorld.Server.EntityManager;
        ulong steamID = user.Read<User>().PlatformId;
        if (ManualEventSystem.EventPaused)
        {
            ctx.Reply("The event is closed to new participants.");
            return;
        }
        if (ManualEventSystem.BannedPlayers.Contains(steamID))
        {
            ctx.Reply("You are banned from events.");
            return;
        }

        if (!ManualEventSystem.EventActive)
        {
            ctx.Reply("No event is currently active.");
            return;
        }

        if (ManualEventSystem.EventPlayers.Contains(playerEntity))
        {
            ctx.Reply("You are already in the event.");
            return;
        }

        var inventory = entityManager.GetComponentData<Equipment>(ctx.Event.SenderCharacterEntity);

        if (!Helper.IsInventoryEmpty(inventory))
        {
            ctx.Reply("Your inventory must be empty to join the event.");
            return;
        }

        ManualEventSystem.JoinEvent(playerEntity);
        ctx.Reply("You have joined the event. Use .event leave to exit.");
    }

    [Command("leave", description: "", adminOnly: false)]
    public static void OnEventLeave(ChatCommandContext ctx)
    {
        var playerEntity = ctx.Event.SenderCharacterEntity;

        if (!ManualEventSystem.EventPlayers.Contains(playerEntity))
        {
            ctx.Reply("You are not in the event.");
            return;
        }
        ManualEventSystem.LeaveEvent(playerEntity);
        ctx.Reply("You have left the event and were teleported back to your original position.");
    }
}