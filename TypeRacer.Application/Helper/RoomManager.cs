using System.Collections.Concurrent;

namespace TypeRacer.Application.Helper
{
    public class RoomManager
    {
        public static readonly int MaxPlayersPerRoom = 4;
        private ConcurrentDictionary<string, List<string>> rooms = new();
        //private ConcurrentQueue<string> waitingPlayers = new();

        public string AddPlayer(string connectionId)
        {
            // Add player to waiting queue
            //waitingPlayers.Enqueue(connectionId);

            // Try to find a room that is not full
            foreach (var room in rooms)
            {
                if (room.Value.Count < MaxPlayersPerRoom)
                {
                    room.Value.Add(connectionId);
                    return room.Key;
                }
            }

            // If no room available, create new room
            var roomId = Guid.NewGuid().ToString();
            rooms[roomId] = new List<string> { connectionId };
            return roomId;
        }

        public void RemovePlayer(string connectionId)
        {
            foreach (var room in rooms)
            {
                if (room.Value.Contains(connectionId))
                {
                    room.Value.Remove(connectionId);
                    // Remove empty rooms
                    if (room.Value.Count == 0)
                    {
                        rooms.TryRemove(room.Key, out _);
                    }
                    break;
                }
            }
        }

        public List<string> GetPlayersInRoom(string roomId)
        {
            if (rooms.TryGetValue(roomId, out var players))
            {
                return players;
            }
            return new List<string>();
        }
    }

}
