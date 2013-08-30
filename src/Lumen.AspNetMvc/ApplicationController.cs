using System;
using System.Collections.Generic;
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

        protected virtual HttpStatusCodeResult HttpInternalServerError()
        {
            return new HttpStatusCodeResult(500);
        }

        protected virtual HttpStatusCodeResult HttpOk()
        {
            return new HttpStatusCodeResult(200);
        }

        protected virtual ActionResult Invoke<TService, TPayload>(Func<TPayload, ActionResult> actionResultProvider = null)
            where TService : ApplicationService<object>
            where TPayload : class, new()
        {
            return InvokeService<TService, TPayload, object>((payload, result) =>
            {
                actionResultProvider = actionResultProvider ?? (_ => HttpOk());
                return actionResultProvider(payload);
            });
        }

        protected virtual ActionResult Invoke<TService, TPayload, TResult>(Func<TPayload, TResult, ActionResult> actionResultProvider = null)
            where TService : ApplicationService<TResult>
            where TPayload : class, new()
        {
            return InvokeService<TService, TPayload, TResult>((payload, result) =>
            {
                actionResultProvider = actionResultProvider ?? ((p, r) => HttpOk());
                return actionResultProvider(payload, result);
            });
        }

        protected virtual TResult InvokeService<TService, TPayload, TResult>(TPayload payload, TUser user)
            where TService : ApplicationService<TResult>
            where TPayload : class, new()
        {
            var invoker = DependencyResolver.Current.GetService<ApplicationServiceInvoker<ApplicationServiceContext>>();
            return invoker.Invoke<TService, TResult>(new ApplicationServiceContext(payload, user));
        }

        private ActionResult InvokeService<TService, TPayload, TResult>(Func<TPayload, TResult, ActionResult> getActionResult)
            where TService : ApplicationService<TResult>
            where TPayload : class, new()
        {
            try
            {
                var payload = new TPayload();
                TryUpdateModel(payload);

                var result = InvokeService<TService, TPayload, TResult>(payload, User.Identity.User);
                return getActionResult(payload, result);
            }
            catch (PayloadValidationException exception)
            {
                return OnPayloadValidationError(exception);
            }
            catch (Exception exception)
            {
                return OnServiceInvocationError(exception);
            }
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

        protected virtual JsonNetResult JsonValidationError(string message = null, Dictionary<string, string> fields = null)
        {
            var response = new JsonValidationErrorResponse(message, fields);
            return new JsonBadRequestResult(response);
        }

        protected virtual ActionResult OnPayloadValidationError(PayloadValidationException exception)
        {
            if (Request.IsAjaxRequest())
            {
                return JsonValidationError(exception.ErrorMessage, exception.FieldErrorMessages);
            }

            // Currently not supported: should do a redirect to GET (POST-REDIRECT-GET pattern).
            throw new NotSupportedException();
        }

        protected virtual ActionResult OnServiceInvocationError(Exception exception)
        {
            if (Request.IsAjaxRequest())
            {
                return HttpInternalServerError();
            }

            return View("Error");
        }
    }

    public abstract class ApplicationController : ApplicationController<User> { }
}