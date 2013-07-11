using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandSite.Models.Implementations
{
    public class UserProfile : IEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }


        public bool TrySetPropertiesFrom(object source)
        {
            UserProfile album = source as UserProfile;
            if (album != null)
            {
                this.Id = album.Id;
                this.UserName = album.UserName;
                return true;
            }
            return false;
        }
    }
}