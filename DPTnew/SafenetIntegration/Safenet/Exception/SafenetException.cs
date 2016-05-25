using System;
using System.Net;
using System.Runtime.Serialization;

namespace SafenetIntegration.Safenet.Exception
{
  [Serializable]
  public class SafenetException : System.Exception
  {
    public SafenetException()
    {
    }

    public SafenetException(string message, HttpStatusCode code)
      : base(message)
    {
      this.Data.Add("StatusCode", code);
    }

    // Ensure Exception is Serializable
    protected SafenetException(SerializationInfo info, StreamingContext ctxt)
      : base(info, ctxt)
    { }
  }
}
