namespace Lumen.AspNetMvc
{
    public class JsonRedirectResult : JsonNetResult
    {
        public JsonRedirectResult(object data) : base(data, 302) { }
    }
}