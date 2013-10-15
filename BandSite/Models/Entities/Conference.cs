using BandSite.Models.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandSite.Models.Entities
{
    public class Conference: EntityBase
    {
        public virtual string Title {get; set;}
        public virtual Guid Guid { get; set; }

        private ICollection<UserProfile> _users;
        private ICollection<Message> _messages;

        public virtual ICollection<UserProfile> Users
        {
            get
            {
                return _users ?? (_users = (new HashSet<UserProfile>()));
            }
        }
        public virtual ICollection<Message> Messages
        {
            get
            {
                return _messages ?? (_messages = (new HashSet<Message>()));
            }
        }
    }
}