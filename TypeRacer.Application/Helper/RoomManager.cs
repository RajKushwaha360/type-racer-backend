using System.Collections.Concurrent;
using TypeRacer.Domain.DTOs;

namespace TypeRacer.Application.Helper
{
    public class RoomManager
    {
        public static readonly int ExactPlayersPerRoom = 3;
        private ConcurrentDictionary<string, List<UserDTO>> rooms = new();
        private ConcurrentQueue<UserDTO> waitingPlayers = new();

        public string? AddPlayer(string connectionId, string userName)
        {
            UserDTO user = new UserDTO
            {
                ConnectionId = connectionId,
                UserName = userName,
            };


            if (waitingPlayers.Count >= (ExactPlayersPerRoom - 1))
            {
                var roomId = Guid.NewGuid().ToString();
                rooms[roomId] = new List<UserDTO> { user };

                for (int i = 0; i < (ExactPlayersPerRoom - 1); i++)
                {
                    UserDTO? dequeUser = new UserDTO();
                    bool isDequeued = waitingPlayers.TryDequeue(out dequeUser);
                    if (isDequeued && dequeUser != null)
                    {
                        rooms[roomId].Add(dequeUser);
                    }
                }
                return roomId;
            }


            // Add player to waiting queue
            waitingPlayers.Enqueue(user);
            return null;

            //// Try to find a room that is not full
            //foreach (var room in rooms)
            //{
            //    if (room.Value.Count < MaxPlayersPerRoom)
            //    {
            //        room.Value.Add(connectionId);
            //        return room.Key;
            //    }
            //}

            //// If no room available, create new room
            //var roomId = Guid.NewGuid().ToString();
            //rooms[roomId] = new List<string> { connectionId };
            //return roomId;
        }

        public void CloseRoom()
        {

        }

        public string? RemovePlayer(string connectionId)
        {
            string roomKey = "";

            foreach (var room in rooms)
            {
                UserDTO? userDTO = room.Value.FirstOrDefault(val => val.ConnectionId == connectionId);
                if (userDTO != null)
                {
                    room.Value.Remove(userDTO);
                    // Remove empty rooms
                    if (room.Value.Count == 0)
                    {
                        rooms.TryRemove(room.Key, out _);
                    }
                    roomKey = room.Key;
                    break;
                }
            }

            return roomKey;
        }

        public List<UserDTO> GetPlayersInRoom(string roomId)
        {
            if (rooms.TryGetValue(roomId, out var players))
            {
                return players;
            }
            return new List<UserDTO>();
        }
    }

}
