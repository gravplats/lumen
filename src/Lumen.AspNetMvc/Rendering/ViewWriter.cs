using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lumen.AspNetMvc.Rendering
{
    /// <summary>
    /// Provides functionality for rendering a partial view to a string.
    /// </summary>
    public class ViewWriter : Controller
    {
        /// <summary>
        /// Renders a partial view to a string.
        /// </summary>
        /// <param name="partialViewName">The name of the partial view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="configure">A configuration initializer.</param>
        /// <returns>The view as a string.</returns>
        public static string ToString(string partialViewName, object viewModel = null, Action<StringRenderConfiguration> configure = null)
        {
            Ensure.NotNullOrEmpty(partialViewName, "partialViewName");

            using (var writer = new StringWriter())
            {
                var configuration = GetConfiguration(configure);
                var httpResponse = new HttpResponse(writer);

                var controllerContext = CreateControllerContext(httpResponse, configuration);

                var viewEngineResult = GetViewEngineResult(partialViewName, controllerContext);
                var view = viewEngineResult.View;

                try
                {
                    var viewContext = CreateViewContext(controllerContext, view, writer, viewModel, configuration);
                    view.Render(viewContext, httpResponse.Output);

                    httpResponse.Flush();
                }
                finally
                {
                    viewEngineResult.ViewEngine.ReleaseView(controllerContext, view);
                }

                return writer.ToString();
            }
        }

        private static ControllerContext CreateControllerContext(HttpResponse httpResponse, StringRenderConfiguration configuration)
        {
            // The 'controller' route data value is required by VirtualPathProviderViewEngine.
            var routeData = new RouteData();
            routeData.Values["controller"] = typeof(ViewWriter).Name;

            var httpContext = new HttpContextStub(httpResponse, configuration.IsMobileDevice);
            var requestContext = new RequestContext(httpContext, routeData);
            return new ControllerContext(requestContext, new ViewWriter());
        }

        private static ViewContextStub CreateViewContext(ControllerContext controllerContext, IView view, TextWriter writer, object viewModel, StringRenderConfiguration configuration)
        {
            var viewData = new ViewDataDictionary(viewModel);
            var tempData = new TempDataDictionary();

            return new ViewContextStub(controllerContext, view, viewData, tempData, writer)
            {
                ClientValidationEnabled = configuration.ClientValidationEnabled,
                UnobtrusiveJavaScriptEnabled = configuration.UnobtrusiveJavaScriptEnabled
            };
        }

        private static StringRenderConfiguration GetConfiguration(Action<StringRenderConfiguration> configure)
        {
            var configuration = new StringRenderConfiguration();
            if (configure != null)
            {
                configure(configuration);
            }

            return configuration;
        }

        private static ViewEngineResult GetViewEngineResult(string partialViewName, ControllerContext controllerContext)
        {
            var viewEngineResult = ViewEngines.Engines.FindPartialView(controllerContext, partialViewName);
            if (viewEngineResult == null)
            {
                string message = "The partial view was not found.";
                throw new ArgumentException(message, partialViewName);
            }

            if (viewEngineResult.View == null)
            {
                var locations = new StringBuilder();
                foreach (string searchedLocation in viewEngineResult.SearchedLocations)
                {
                    locations.AppendLine();
                    locations.Append(searchedLocation);
                }

                throw new ArgumentException("The partial view was not found. The following locations were searched: " + locations, partialViewName);
            }

            return viewEngineResult;
        }
    }
}