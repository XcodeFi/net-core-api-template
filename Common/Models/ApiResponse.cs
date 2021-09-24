
using Common.Enums;

namespace Common.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public StatusCode? StatusCode { get; set; }
        public string Message { get; set; }
    }
}
