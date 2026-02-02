using TheLeaguesUI.Extensions.Interfaces;

namespace TheLeaguesUI.Extensions
{
    public class OperationResult<T> : IOperationalResult<T>
    {
        public T? Data {get; private set;}
        public bool IsSuccess {get;private set;}
        public string Message {get;private set;} = "";
        public int? StatusCode {get;private set;}

        public static OperationResult<T> Success(T data)
        {
            return new OperationResult<T>(){IsSuccess = true, Data = data};
        }

        public static OperationResult<T> Failure(string message, int? statusCode)
        {
            return new OperationResult<T>(){IsSuccess = false, Message = message, StatusCode = statusCode};
        }
    }
}