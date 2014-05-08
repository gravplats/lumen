# Lumen

Lumen is an application pipeline for the ASP.NET MVC framework.

## Usage

Define an application service, a payload and a validator.

``` csharp
public class CustomService : ApplicationServive
{
	public class Payload
	{
		public string Value { get; set;}
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

	protected override void ExecuteCore()
	{
		// do stuff.
	}
}
```

Invoke the application service from a controller. Note: using [AttributeRouting](http://attributeRouting.net).

``` csharp
public class CustomController : ApplicationController
{
	[POST("/custom")]
	public ActionResult Custom()
	{
		return Invoke<CustomService, CustomService.Payload>();
	}
}
```

### Working with application services that returns data

``` csharp
public class GetUsersService : ApplicationService<IEnumerable<User>>
{
	public override IEnumerable<User> Execute()
	{
		// read users from database.
		return users;
	}
}
```

The `Invoke` method can transform the result of the `ApplicationService`. There is no payload, thus we use `object` instead.

``` csharp
public class UserController : ApplicationController
{
	[GET("/users")]
	public ActionResult GetUsers()
	{
		return Invoke<GetUsersService, object, IEnumerable<User>>((payload, users) => Json(users));
	}
}
```

### Extending the application pipeline

It is possible to extend the application pipeline by implementing and registering an `ApplicationServiceFilter<TContext>`.

``` csharp
public class AuthorizationFilter : ApplicationServiceFilter<ApplicationServiceContext>
{
	private readonly User currentUser;

	public AuthorizationFilter(User user)
	{
		this.user = user;
	}

    protected override TResult ProcessCore<TResult>(
		PipelineContext pipelineContext, 
		ApplicationServiceContext context, 
		Func<PipelineContext, ApplicationServiceContext, TResult> next)
    {
        if (IsAuthorized(pipelineContext, context))
        {
            return next(pipelineContext, context);
        }

        throw new ApplicationServiceAuthorizationException();
    }

    private bool IsAuthorized(PipelineContext pipelineContext, ApplicationServiceContext context)
    {
		// determine if the user is authorized.
		return true;
    }
}
```

The `ApplicationServiceAuthorizationException` will be caught by the `ApplicationController` which will return an HTTP 403 Forbidden by default.

## License

Licensed under [MIT](http://opensource.org/licenses/mit-license.php). Please refer to LICENSE for more information.
