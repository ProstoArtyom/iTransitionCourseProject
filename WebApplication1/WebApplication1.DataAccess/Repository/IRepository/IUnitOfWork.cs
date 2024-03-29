namespace WebApplication1.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICollectionRepository Collection { get; }
        IItemRepository Item { get; }
        ITagRepository Tag { get; }
        IThemeRepository Theme { get; }
        IItemTagRepository ItemTag { get; }
        IApplicationUserRepository ApplicationUser { get; }
        void Save();
    }
}
