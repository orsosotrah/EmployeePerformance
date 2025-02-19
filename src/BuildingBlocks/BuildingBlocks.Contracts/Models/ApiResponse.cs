namespace BuildingBlocks.Contracts.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public IDictionary<string, string[]> Errors { get; set; }

        public static ApiResponse<T> SuccessResult(T data, string message = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Message = message ?? "Operation completed successfully",
                Data = data
            };
        }

        public static ApiResponse<T> FailureResult(string message, IDictionary<string, string[]> errors = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }
    }

    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse Success(string message = null)
        {
            return new ApiResponse
            {
                IsSuccess = true,
                Message = message ?? "Operation completed successfully"
            };
        }

        public static ApiResponse Failure(string message, IDictionary<string, string[]> errors = null)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }
    }
}