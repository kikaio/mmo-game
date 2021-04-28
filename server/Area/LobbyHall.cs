using common.Utils.Loggers;
using server.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Area
{
    using RoomList = List<Room>;
    using RoomId = Int32;
    using LobbyId = Int32;
    //Room들로 들어갈 수 있는 Lobby공간.
    public class LobbyHall
    {
        public string HallName { get; private set; } = "default name";
        public LobbyId LobbyHallId { get; private set; } = 0;

        private Dictionary<RoomId, Room> mRoomDict = new Dictionary<RoomId, Room>();

        private CoreLogger logger = new ConsoleLogger();

        public static readonly int MaxRoomCnt = 100;

        private int curRoomCnt = 0;

        public LobbyHall(int _id, string _hName)
        {
            LobbyHallId = _id;
            HallName = _hName;
        }

        private void Init()
        {

        }
        
        private void CreateRoom(RoomId _rId)
        {
            if (curRoomCnt == MaxRoomCnt)
            {
                logger.Error($"room cnt is {MaxRoomCnt}");
                return;
            }
            var room = default(Room);
            if (mRoomDict.ContainsKey(_rId))
            {
                logger.Error("room id is duplicated");
                return;
            }
            curRoomCnt++;
            var newRoom = new Room(curRoomCnt);
            mRoomDict.Add(newRoom.mRoomId, newRoom);
        }

        public bool JoinRoom(RoomId _rId, User _u, out Room _room)
        {
            _room = default(Room);
            if (mRoomDict.ContainsKey(_rId))
            {
                _room = mRoomDict[_rId];
                if (_room.JoinUser(_u) == false)
                    return false;
                return true;
            }
            return false;
        }

        [Conditional("DEBUG")]
        public void Render()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"LobbyHall[{LobbyHallId}]");
            foreach (var r in mRoomDict.Values.ToList())
                r.Render();
            sb.AppendLine($"-----------------------");
            logger.WriteDebug()
        }
    }
}
