using BandSite.Models.DataLayer;
using System;

namespace BandSite.Models.Entities
{
    public enum MessageStatus
    {
        Undelivered = 0,
        Unread = 1,
        Read = 2
    }

    public class Message : EntityBase
    {
        public virtual string Text { get; set; }

        public virtual DateTime? Published { get; set; }

        public virtual int UserFromId { get; set; }

        public virtual int UserToId { get; set; }

        public virtual UserProfile UserFrom { get; set; }

        public virtual UserProfile UserTo { get; set; }

        public virtual MessageStatus Status { get; set; }

        public virtual Guid Guid { get; set; }

        public virtual int? ConferenceId { get; set; }

        public virtual Conference Conference { get; set; }
    }
}