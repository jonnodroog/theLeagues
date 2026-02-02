namespace TheLeaguesUI.Extensions.Interfaces
{
    public interface IOperationalResult<T>
    {
        T? Data {get;}
        bool IsSuccess {get;}
        string Message {get;}
        int? StatusCode {get;}
    }
}