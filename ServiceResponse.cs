
namespace WebProject
{
    public class ServiceResponse<T>
    {
        public int StatusCode { get; set; }
        public T ResponseData { get; set; }
    }
    public class ServiceResponse
    {
        public int StatusCode { get; set; }
    }
}