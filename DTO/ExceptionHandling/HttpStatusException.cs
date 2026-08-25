using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DTO.ExceptionHandling
{
    public class HttpStatusException : Exception
    {
        public HttpStatusCode Status { get; private set; }

        public string Title { get; set; }

        public HttpStatusException(HttpStatusCode status, string title, string msg) : base(msg)
        {
            Status = status;
            Title = title;
        }
    }
}
