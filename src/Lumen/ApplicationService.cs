namespace Lumen
{
    public abstract class ApplicationService : IApplicationService
    {
        public object Execute()
        {
            ExecuteCore();
            return null;
        }

        protected abstract void ExecuteCore();
    }

    public abstract class ApplicationService<TResult> : IApplicationService
    {
        public object Execute()
        {
            return ExecuteCore();
        }

        protected abstract TResult ExecuteCore();
    }
}