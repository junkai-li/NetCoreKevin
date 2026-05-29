using System.Runtime.Serialization;

namespace kevin.Domain.Share
{
    [Serializable]
    public class FieldValidationException : Exception, ISerializable
    {
        public FieldValidationException(string message) : base(message) { }
        public FieldValidationException(string message, Exception _innerException) : base(message)
        {
            InnerException = _innerException;
        }
        public FieldValidationException(string message, params string[] param) : base(string.Format(message, param))
        {

        }
        public new Exception InnerException { get; set; }
    }
}
