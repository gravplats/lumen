using System.Collections.Generic;

namespace Lumen.AspNetMvc
{
    public class JsonValidationErrorResponse
    {
        public JsonValidationErrorResponse(string message = null, Dictionary<string, string> fields = null)
        {
            Message = message;
            Fields = fields;
        }

        public bool Error { get { return true; } }

        public Dictionary<string, string> Fields { get; set; }

        public string Message { get; set; }
    }
}