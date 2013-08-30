using System;
using System.Collections.Generic;

namespace Lumen
{
    public class PayloadValidationException : Exception
    {
        public PayloadValidationException(string errorMessage) : this(errorMessage, null) { }

        public PayloadValidationException(Dictionary<string, string> fieldErrorMessages) : this(null, fieldErrorMessages) { }

        public PayloadValidationException(string errorMessage, Dictionary<string, string> fieldErrorMessages)
        {
            ErrorMessage = errorMessage;
            FieldErrorMessages = fieldErrorMessages;
        }

        public string ErrorMessage { get; set; }

        public Dictionary<string, string> FieldErrorMessages { get; set; }
    }
}