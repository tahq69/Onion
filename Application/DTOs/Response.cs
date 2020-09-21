using System.Collections.Generic;

namespace Onion.Application.DTOs
{
    public class Response<T>
    {
        public Response()
        {
        }

        public Response(T data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }

        public Response(string message, Dictionary<string, string> errors = null)
        {
            Succeeded = false;
            Message = message;
            Errors = errors;
        }

        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> Errors { get; set; }
        public T Data { get; set; }
    }
}