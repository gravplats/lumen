using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Lumen.Security;

namespace Lumen.AspNetMvc
{
    [SessionState(SessionStateBehavior.Disabled)]
    public abstract class ApplicationController<TUser> : Controller
        where TUser : class, IUser
    {
        protected new ApplicationPrincipal<TUser> User
        {
            get
            {
                if (HttpContext == null)
                {
                    return null;
                }

                var user = HttpContext.User as ApplicationPrincipal<TUser>;
                if (user == null)
                {
                    user = ApplicationPrincipal<TUser>.Create();
                    HttpContext.User = user;
                }

                return user;
            }
        }

        protected override ITempDataProvider CreateTempDataProvider()
        {
            return new CookieTempDataProvider();
        }

        protected virtual HttpStatusCodeResult HttpForbidden()
        {
            return new HttpStatusCodeResult(403);
        }

        protected virtual HttpStatusCodeResult HttpInternalServerError()
        {
            return new HttpStatusCodeResult(500);
        }

        protected virtual HttpStatusCodeResult HttpOk()
        {
            return new HttpStatusCodeResult(200);
        }

        protected virtual ActionResult Invoke<TService>(Func<object, object, ActionResult> actionResultProvider = null)
            where TService : IApplicationService
        {
            return InvokeService<TService, object, object>(actionResultProvider);
        }

        protected virtual ActionResult Invoke<TService, TPayload>(Func<TPayload, object, ActionResult> actionResultProvider = null)
            where TService : ApplicationService
            where TPayload : class, new()
        {
            return InvokeService<TService, TPayload, object>(actionResultProvider);
        }

        protected virtual ActionResult InvokeWithResult<TService, TResult>(Func<object, TResult, ActionResult> actionResultProvider = null)
            where TService : ApplicationService<TResult>
        {
            return InvokeService<TService, object, TResult>(actionResultProvider);
        }

        protected virtual ActionResult Invoke<TService, TPayload, TResult>(Func<TPayload, TResult, ActionResult> actionResultProvider = null)
            where TService : ApplicationService<TResult>
            where TPayload : class, new()
        {
            return InvokeService<TService, TPayload, TResult>(actionResultProvider);
        }

        private ActionResult InvokeService<TService, TPayload, TResult>(Func<TPayload, TResult, ActionResult> actionResultProvider = null)
            where TService : IApplicationService
            where TPayload : class, new()
        {
            try
            {
                var payload = new TPayload();
                TryUpdateModel(payload);

                var result = InvokeService<TService, TPayload, TResult>(payload);

                actionResultProvider = actionResultProvider ?? ((p, r) => GetDefaultActionResult(p, r));
                return actionResultProvider(payload, result);
            }
            catch (ApplicationServiceAuthorizationException exception)
            {
                return OnApplicationServiceAuthorizationException(exception);
            }
            catch (PayloadValidationException exception)
            {
                return OnPayloadValidationException(exception);
            }
            catch (Exception exception)
            {
                return OnServiceInvocationException(exception);
            }
        }

        protected virtual TResult InvokeService<TService, TPayload, TResult>(TPayload payload)
            where TService : IApplicationService
            where TPayload : class, new()
        {
            var invoker = DependencyResolver.Current.GetService<ApplicationServiceInvoker>();
            return invoker.Invoke<TService, TResult>(new ApplicationServiceContext(payload));
        }

        protected virtual ActionResult GetDefaultActionResult<TPayload, TResult>(TPayload payload, TResult result)
            where TPayload : class, new()
        {
            return HttpOk();
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult(data)
            {
                ContentEncoding = contentEncoding,
                ContentType = contentType,
            };
        }

        protected virtual JsonResult Json(IViewModelMapper mapper)
        {
            var model = mapper.Map();
            return Json(model);
        }

        protected virtual JsonNetResult JsonRedirect(string url, object data = null)
        {
            Ensure.NotNullOrEmpty(url, "url");

            var obj = new RouteValueDictionary(data) { { "url", url } };
            return new JsonRedirectResult(obj);
        }

        protected virtual JsonNetResult JsonValidationError(object message = null, object fields = null)
        {
            var response = new JsonValidationErrorResponse(message, fields);
            return new JsonBadRequestResult(response);
        }

        protected virtual ActionResult OnApplicationServiceAuthorizationException(ApplicationServiceAuthorizationException exception)
        {
            return HttpForbidden();
        }

        protected virtual ActionResult OnPayloadValidationException(PayloadValidationException exception)
        {
            if (Request.IsAjaxRequest())
            {
                return JsonValidationError(exception.ErrorMessage, exception.FieldErrorMessages);
            }

            // Currently not supported: should do a redirect to GET (POST-REDIRECT-GET pattern).
            throw new NotSupportedException();
        }

        protected virtual ActionResult OnServiceInvocationException(Exception exception)
        {
            if (Request.IsAjaxRequest())
            {
                return HttpInternalServerError();
            }

            return View("Error");
        }
    }

    public abstract class ApplicationController : ApplicationController<LumenUser> { }
}