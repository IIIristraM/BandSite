using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandSite.Models.Implementations
{
    public enum MessageStatus
    {
        Unread = 0,
        Read = 1,
        Write = 2
    }

    public class Message : IEntity
    {
        public virtual int Id { get; set; }

        public virtual string Text { get; set; }

        public virtual DateTime Published { get; set; }

        public virtual int UserFromId { get; set; }

        public virtual int UserToId { get; set; }

        public virtual UserProfile UserFrom { get; set; }

        public virtual UserProfile UserTo { get; set; }

        public virtual string Status { get; set; }

        public bool TrySetPropertiesFrom(object source)
        {
            Message message = source as Message;
            if (message != null)
            {
                this.Id = message.Id;
                this.Text = message.Text;
                this.Published = message.Published;
                this.Status = message.Status;
                return true;
            }
            return false;
        }
    }
}