using common.Utils.Loggers;
using server.Users;
using System;
using System.Collections.Generic;
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

        public LobbyHall(int _id, string _hName)
        {
            LobbyHallId = _id;
            HallName = _hName;
        }

        private void Init()
        {

        }
        
        //까르비올라님 도네이션 기다리고 있습니다.

        private void CreateRoom(RoomId _rId)
        {
            var room = default(Room);
            if (mRoomDict.ContainsKey(_rId))
            {
                logger.Error("room id is duplicated");
                return;
            }
            var newRoom = new Room();

        }

        public bool JoinRoom(RoomId _rId, User _u, out Room _room)
        {
            _room = default(Room);
            if (mRoomDict.ContainsKey(_rId))
            {
                _room = mRoomDict[_rId];
                return true;
            }
            return false;
        }
    }
}
