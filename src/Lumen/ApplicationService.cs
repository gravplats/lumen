namespace Lumen
{
    public abstract class ApplicationService<TResult>
    {
        public abstract TResult Execute();
    }

    public abstract class ApplicationService : ApplicationService<object>
    {
        public override object Execute()
        {
            ExecuteCore();
            return null;
        }

        protected abstract void ExecuteCore();
    }
}