using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSCore.Beans;


//For type defination use <T>
public class ResponseResult
{
    public Object Data { get; set; }
    public string Message { get; set; }
    public ResponseStatus Status { get; set; }
}
public enum ResponseStatus
{
    Error,
    NotFound,
    Success
}

