namespace Lumen.AspNetMvc
{
    public class JsonValidationErrorResponse
    {
        public JsonValidationErrorResponse(object message = null, object fields = null)
        {
            Message = message;
            Fields = fields;
        }

        public object Fields { get; set; }
        public object Message { get; set; }
    }
}