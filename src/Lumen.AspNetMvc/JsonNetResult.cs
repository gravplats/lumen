using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lumen.AspNetMvc
{
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult(object data, int statusCode = 200)
        {
            Data = Ensure.NotNull(data, "data");
            StatusCode = statusCode;
        }

        public new JsonRequestBehavior JsonRequestBehavior
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public new int? MaxJsonLength
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public new int? RecursionLimit
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public int StatusCode { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            Ensure.NotNull(context, "context");

            var response = context.HttpContext.Response;
            response.ContentType = ContentType ?? "application/json";
            response.StatusCode = StatusCode;

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                var settings = CreateJsonSerializerSettings();
                var json = JsonConvert.SerializeObject(Data, settings);

                response.Write(json);
            }
        }

        protected virtual JsonSerializerSettings CreateJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None
            };
        }
    }
}