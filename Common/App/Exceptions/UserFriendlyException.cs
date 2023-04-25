using System;
using System.Runtime.Serialization;

namespace Web.Global.Exceptions
{
    [Serializable]
    public class UserFriendlyException : Exception, ISerializable
    {
        public UserFriendlyException(string message) : base(message) { }
        public UserFriendlyException(string message, Exception _innerException) : base(message)
        {
            InnerException = _innerException;
        }
        public UserFriendlyException(string message, params string[] param) : base(string.Format(message, param))
        {

        }
        public new Exception InnerException { get; set; }
    }
}
