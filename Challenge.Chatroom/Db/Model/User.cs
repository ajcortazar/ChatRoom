using System;
using System.Collections.Generic;
using System.Text;

namespace Challenge.Chatroom.Db.Model
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Identification { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public string LoginTime { get; set; }
        public string ConnectionId { get; set; }
    }

}
