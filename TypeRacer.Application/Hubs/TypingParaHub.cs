using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using TypeRacer.Application.Helper;

namespace TypeRacer.Application.Hubs
{
    public class TypingParaHub : Hub
    {
        private readonly RoomManager _roomManager;

        public TypingParaHub(RoomManager roomManager)
        {
            _roomManager = roomManager;
        }

        public async Task JoinRandomRoom(string username)
        {
            var roomId = _roomManager.AddPlayer(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            // Notify all in room about new player
            var players = _roomManager.GetPlayersInRoom(roomId);
            await Clients.Group(roomId).SendAsync("PlayerJoined", username, players, roomId, "","","","","","","");

            if (players != null && players.Count == RoomManager.MaxPlayersPerRoom)
            {
                await Clients.Group(roomId).SendAsync("StartCompetition", username, players);
            }
        }

        public async Task UpdateProgress(string roomId, string username, int progress, int wpm)
        {
            await Clients.Group(roomId).SendAsync("ProgressUpdate", username, progress, wpm);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _roomManager.RemovePlayer(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }


    }
}
