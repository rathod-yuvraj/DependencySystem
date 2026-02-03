namespace DependencySystem.Helper
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "OK";
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "OK")
            => new ApiResponse<T> { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message)
            => new ApiResponse<T> { Success = false, Message = message, Data = default };
    }
}
