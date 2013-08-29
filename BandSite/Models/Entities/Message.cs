using BandSite.Models.DataLayer;
using System;

namespace BandSite.Models.Entities
{
    public enum MessageStatus
    {
        Unread = 0,
        Read = 1,
        Write = 2
    }

    public class Message : EntityBase
    {
        public virtual string Text { get; set; }

        public virtual DateTime? Published { get; set; }

        public virtual int UserFromId { get; set; }

        public virtual int UserToId { get; set; }

        public virtual UserProfile UserFrom { get; set; }

        public virtual UserProfile UserTo { get; set; }

        public virtual string Status { get; set; }
    }
}