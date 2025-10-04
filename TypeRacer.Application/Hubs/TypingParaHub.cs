using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using TypeRacer.Application.Helper;
using TypeRacer.Domain.DTOs;

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
            string? roomId = _roomManager.AddPlayer(Context.ConnectionId, username);


            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }
            List<UserDTO> players = _roomManager.GetPlayersInRoom(roomId);

            for (int i = 0; i < players.Count; i++)
            {
                await Groups.AddToGroupAsync(players[i].ConnectionId, roomId);
            }

            await Clients.Group(roomId).SendAsync("PlayerJoined", username, players, roomId);
            await Clients.Group(roomId).SendAsync("StartCompetition", players);
            // Notify all in room about new player

            //if (players != null && players.Count == RoomManager.ExactPlayersPerRoom)
            //{
            //}
        }

        public async Task UpdateProgress(string roomId, string username, int progress, int wpm)
        {
            await Clients.Group(roomId).SendAsync("ProgressUpdate", username, progress, wpm);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string? roomKey = _roomManager.RemovePlayer(Context.ConnectionId);

            if (!string.IsNullOrEmpty(roomKey))
            {
                await Clients.Group(roomKey).SendAsync("PlayerLeft", Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }


    }
}
