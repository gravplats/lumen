# Lumen

Lumen is an application pipeline for the ASP.NET MVC framework.

## Usage

The heart and soul of Lumen is `IApplicationService`. It is implemented by two classes, `ApplicationService`, which is used for scenarios which does not require an explicit result (an implicit `null` will be returned instead) and, `ApplicationService<TResult>`, which is used for sceanario which does require an explicit result.

An application service should be invoked by the `ApplicationServiceInvoker<TContext, TPipelineContext>` class. The invoker will instantiate the service and run any filters, `ApplicationServiceFilter<TContext, TPipelineContext>`, prior to executing the service. One pre-defined filter is included in the project, `PayloadValidationFilter<TContext, TPipelineContext>`, which is used for validating the payload (the incoming model).

Lumen uses [Ninject](https://github.com/ninject/ninject) for inversion-of-control and [FluentValidation](https://github.com/JeremySkinner/FluentValidation) for model validation. The Ninject kernel can easily be configured with the `BindApplicationServicePipeline` helper methods.

### The application service

The common scenario would be to define an application service, a payload (the incoming model), and a validator for the payload. The payload will be passed to the constructor of the application service.

``` csharp
public class Result
{
	public IEnumerable<string> Values { get; set; }
}

public class CustomService : ApplicationService<Result>
{
	public class Payload
	{
		public string Value { get; set; }
	}

	public class Validator : AbstractValidator<Payload>
	{
		public Validator()
		{
			RuleFor(x => x.Value)
				.Required();
		}
	}

	private readonly Payload payload;

	public CustomService(Payload payload)
	{
		this.payload = payload;
	}

	protected override Result ExecuteCore()
	{
		IEnumerable<string> values;
		
		// do stuff.
		
		return new Result
		{
			Values = values
		};
	}
}
```

Lumen comes with a base controller, `ApplicationController`, which defines a set of common helper methods. These can be used to invoke the application service from a controller.

``` csharp
public class CustomController : ApplicationController
{
	[HttpGet, Route("/custom")]
	public ActionResult Custom()
	{
		return Invoke<CustomService, CustomService.Payload, Result>(
			(payload, result) => Json(result));
	}
}
```

### Customizing the pipeline

You can customize the application service pipeline by defining your own application service context and/or pipeline context. These are used in the following components: `ApplicationServiceInvoker<TContext, TPipelineContext>`, `ApplicationServiceFactory<TContext>`, `ApplicationServiceFilterProvider<TContext, TPipelineContext>`, and `ApplicationServiceFilter<TContext, TPipelineContext>`.

The application service context is used to pass data into the application service pipeline. The default application service context, `ApplicationServiceContext`, contains the application service payload.

The pipeline context is used by the application service filters. A typical use case for implementing a custom pipeline context is if you're "redefining" the application service interface.


``` csharp
public interface ICustomApplicationService : IApplicationService
{
    bool IsAuthorized();
}

public abstract class CustomApplicationService : ApplicationService, ICustomApplicationService
{
    public abstract bool IsAuthorized();
}

public abstract class CustomApplicationService<TResult> : ApplicationService<TResult>, ICustomApplicationService
{
    public abstract bool IsAuthorized();
}


public class CustomPipelineContext : PipelineContext<ICustomApplicationService>
{
    public CustomPipelineContext(ICustomApplicationService service) : base(service) { }
}

public class CustomApplicationServiceFilter : ApplicationServiceFilter<ApplicationServiceContext, CustomPipelineContext>
{
    public override void Process(ApplicationServiceContext context, CustomPipelineContext pipelineContext)
    {
        if (pipelineContext.Service.IsAuthorized())
        {
            return;
        }

        throw new ApplicationServiceAuthorizationException();
    }
}


public class CustomApplicationController : ApplicationController
{
    protected override TResult InvokeService<TService, TPayload, TResult>(TPayload payload)
    {
        var invoker = DependencyResolver.Current.GetService<ApplicationServiceInvoker<ApplicationServiceContext, CustomPipelineContext>>();
        return invoker.Invoke<TService, TResult>(new ApplicationServiceContext(payload));
    }
}
```

The `ApplicationServiceAuthorizationException` will be caught by the `ApplicationController` which will return an HTTP 403 Forbidden by default.

Bind your custom application service pipeline.

``` csharp
var kernel = new StandardKernel();
kernel.BindApplicationServicePipeline<ApplicationServiceContext, CustomPipelineContext>()
      .WithFilter<CustomApplicationServiceFilter>();
```

## License

Licensed under [MIT](http://opensource.org/licenses/mit-license.php). Please refer to LICENSE for more information.
