﻿using WebApplication1.DataAccess.Data;
using WebApplication1.DataAccess.Repository.IRepository;

namespace WebApplication1.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICollectionRepository Collection { get; }
        public IItemRepository Item { get; }
        public ITagRepository Tag { get; }
        public IThemeRepository Theme { get; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Collection = new CollectionRepository(_db);
            Item = new ItemRepository(_db);
            Tag = new TagRepository(_db);
            Theme = new ThemeRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}