using System;
using System.Reflection;
using System.Web.Mvc;

namespace Lumen.AspNetMvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            Ensure.NotNull(controllerContext, "controllerContext");
            return controllerContext.HttpContext.Request.IsAjaxRequest();
        }
    }
}