﻿ 

namespace MultiAgents.AgentsChatRoom.WebSockets
{
    internal class JsonContentPlayLoadForLibrarian
    {
        public string? RoomName { get; set; }
        public string? AgentName { get; set; }
        public string? Text { get; set; }
    }

    internal class JsonContentPlayLoadForGetDocsInLibrarian
    {
        public string? RoomName { get; set; }
        public string? AgentName { get; set; }
        public int Top { get; set; } = 1;
        public int Skip { get; set; } = 0;
    }

}
