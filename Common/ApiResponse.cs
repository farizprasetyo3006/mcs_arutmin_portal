using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public partial class ResponseStatus {
        public bool Success;
        public String Message;
    }
    public class ApiResponse<TData>
    {
        public ResponseStatus Status;
        public TData Data;
        public ApiResponse()
        {
            Status = new ResponseStatus()
            {
                Success = false
            };
        }
    }


    public class ApiResponse
    {
        public ResponseStatus Status;
        public Object Data;
        public ApiResponse()
        {
            Status = new ResponseStatus()
            {
                Success = false
            };
        }
    }

    public class ApiResponsePage<T>
    {
        public long CurrentPage { get; set; }
        public long TotalPages { get; set; }
        public long TotalItems { get; set; }
        public long ItemsPerPage { get; set; }
        public List<T> Items { get; set; }
    }
}
