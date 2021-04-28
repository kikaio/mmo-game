using common.Utils.Loggers;
using server.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Area
{
    using RoomId = Int32;
    using UserId = Int64;
    public class Room
    {
        public RoomId mRoomId { get; private set; }
        private Dictionary<UserId, User> mUserDict = new Dictionary<UserId, User>();
        private CoreLogger logger = new ConsoleLogger();

        public static int MaxUserCnt = 10;
        private int curUserCnt = 0;
        public Room(RoomId _id)
        {
            mRoomId = _id;
        }

        public bool JoinUser(User _user)
        {
            if (curUserCnt == MaxUserCnt)
            {
                logger.WriteDebugWarn($"this room[{mRoomId}] is full");
                return false;
            }
            if (mUserDict.ContainsKey(_user.uId))
            {
                logger.WriteDebugWarn($"this user[{_user.uId}] already exit in this room[{mRoomId}]");
                return false;
            }
            mUserDict.Add(_user.uId, _user);
            logger.WriteDebug($"this user[{_user.uId}] join this room[{mRoomId}]");
            curUserCnt++;
            return true;
        }

        public bool QuitUser(UserId _uid, out User _quitUser)
        {
            _quitUser = default(User);
            if (curUserCnt == 0)
            {
                logger.Error($"room[{mRoomId}] is something wrong");
                return false;
            }
            if (mUserDict.ContainsKey(_uid) == false)
                return false;
            _quitUser = mUserDict[_uid];
            curUserCnt--;
            return true;
        }
    }
}
