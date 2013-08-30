namespace Lumen
{
    public abstract class ApplicationServiceFactory<TContext>
        where TContext : class
    {
        public abstract TService Create<TService, TResult>(TContext context)
            where TService : ApplicationService<TResult>;
    }
}