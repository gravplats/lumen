namespace Lumen
{
    public abstract class ApplicationService : ApplicationServiceBase
    {
        public sealed override object Execute()
        {
            ExecuteCore();
            return null;
        }

        protected abstract void ExecuteCore();
    }

    public abstract class ApplicationService<TResult> : ApplicationServiceBase
    {
        public sealed override object Execute()
        {
            return ExecuteCore();
        }

        protected abstract TResult ExecuteCore();
    }
}