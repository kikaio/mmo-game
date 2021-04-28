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
        public Room(RoomId _id)
        {
            mRoomId = _id;
        }

        public void JoinUser(User _user)
        { 
        }

        public void QuitUser(UserId _uid)
        {

        }
    }
}
