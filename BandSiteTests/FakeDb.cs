using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using BandSite.Models.DataLayer;
using BandSite.Models.Entities;

namespace BandSiteTests
{
    public class FakeDbContextFactory : IDbContextFactory
    {
        public IDbContext CreateContext()
        {
            return new FakeDbContext();
        }
    }

    public class FakeDbContext: IDbContext
    {
        private static readonly List<Song> _songs = new List<Song>
        {
            new Song
            {
                Id = 1,
                Title = "Cool Song",
                Text = "bla-bla-bla",
                File = new byte[1000]
            },
            new Song
            {
                Id = 2,
                Title = "New Song",
                Text = "bla-bla-bla-again",
                File = new byte[1000]
            }
        };

        private readonly static List<Album> _albums = new List<Album>
        {
            new Album
            {
                Id = 1,
                Title = "Cool Album",
                Published = DateTime.Now,
                Description = "the most cool album ever"
            },
            new Album
            {
                Id = 2,
                Title = "New Album",
                Published = DateTime.Now,
                Description = "the newest album ever"
            }
        };

        private readonly static List<UserProfile> _userProfiles = new List<UserProfile>
        {
            new UserProfile
            {
                Id = 1,
                UserName = "Batman"
            },
            new UserProfile
            {
                Id = 2,
                UserName = "Vasily Pupkin"
            }
        };

        private readonly static List<Message> _messages = new List<Message>
        {
            new Message
            {
                Id = 1,
                Text = "Bugy-Vugy",
                UserFrom = _userProfiles[0],
                UserTo = _userProfiles[1],
                Published = DateTime.Now
            },
            new Message
            {
                Id = 2,
                Text = "Vugy-Bugy",
                UserFrom = _userProfiles[1],
                UserTo = _userProfiles[0],
                Published = DateTime.Now
            }
        };

        private readonly static List<PlaylistItem> _playlistItems = new List<PlaylistItem>
        {
            new PlaylistItem
            {
                Id = 1,
                Order = 1,
                User = _userProfiles[0],
                Song = _songs[0]
            },
            new PlaylistItem
            {
                Id = 2,
                Order = 1,
                User = _userProfiles[1],
                Song = _songs[1]
            }
        };

        public bool Disposed { get; set; }

        public FakeDbContext()
        {
            Songs = new FakeRepository<Song>(_songs);
            Albums = new FakeRepository<Album>(_albums);
            UserProfiles = new FakeRepository<UserProfile>(_userProfiles);
            Messages = new FakeRepository<Message>(_messages);
            PlaylistItems = new FakeRepository<PlaylistItem>(_playlistItems);
            Disposed = false;
        }

        public IRepository<BandSite.Models.Entities.Album> Albums { get; set; }

        public IRepository<BandSite.Models.Entities.Song> Songs { get; set; }

        public IRepository<BandSite.Models.Entities.UserProfile> UserProfiles { get; set; }

        public IRepository<BandSite.Models.Entities.PlaylistItem> PlaylistItems { get; set; }

        public IRepository<BandSite.Models.Entities.Message> Messages { get; set; }

        public int SaveChanges()
        {
            return -1;
        }

        public void Dispose()
        {
            Albums = null;
            Songs = null;
            UserProfiles = null;
            PlaylistItems = null;
            Messages = null;
            Disposed = true;
        }
    }

    public class FakeRepository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        private readonly List<TEntity> _content;
        private int _nextId = 0;

        private int GetNextId()
        {
            return ++_nextId;
        }

        public FakeRepository(List<TEntity> content)
        {
            _content = content;
        }

        public IQueryable<TEntity> Content
        {
            get { return  _content.AsQueryable(); }
        }

        public TEntity GetById(int id)
        {
            return _content.FirstOrDefault(e => e.Id == id);
        }

        public TEntity Insert(TEntity entity)
        {
            if (entity != null)
            {
                entity.Id = GetNextId();
                _content.Add(entity);
                return entity;
            }
            return null;
        }

        public TEntity Delete(TEntity entity)
        {
            if (_content.Remove(entity))
            {
                return entity;
            }
            return null;
        }

        public TEntity Delete(int id)
        {
            var result = _content.FirstOrDefault(e => e.Id == id);
            if (result != null)
            {
                _content.Remove(result);
                return result;
            }
            return null;
        }

        public TEntity Update(int id, TEntity entity, bool ignoreNulls = true)
        {
            var result = _content.FirstOrDefault(e => e.Id == id);
            if (result != null)
            {
                if (result.TrySetPropertiesFrom(entity))
                {
                    return result;
                }
            }
            return null;
        }
    }
}
