namespace Lumen.AspNetMvc
{
    public class JsonBadRequestResult : JsonNetResult
    {
        public JsonBadRequestResult(object data) : base(data, 400) { }
    }
}