using System;

namespace Common
{
    public abstract class Result
    {
        public bool Success { get; protected set; }
        public bool Failure => !Success;

        protected string _error;

        public string Error => Failure ? _error :
                throw new Exception($"You can't access .{nameof(Error)} when .{nameof(Failure)} is false");
    }
    public class SuccessResult : Result
    {
        public SuccessResult()
        {
            Success = true;
        }
    }
    public class FailResult : Result
    {
        public FailResult(string error)
        {
            Success = false;
            _error = error;
        }
    }

    public abstract class Result<T>
    {
        public bool Success { get; protected set; }
        public bool Failure => !Success;

        protected string _error;
        protected T _data;

        public string Error => Failure ? _error :
                throw new Exception($"You can't access .{nameof(Error)} when .{nameof(Failure)} is false");
        public T Data => Success ? _data :
                throw new Exception($"You can't access .{nameof(Data)} when .{nameof(Success)} is false");
    }
    public class SuccessResult<T> : Result<T>
    {
        public SuccessResult(T data)
        {
            Success = true;
            _data = data;
        }
    }
    public class FailResult<T> : Result<T>
    {
        public FailResult(string error)
        {
            Success = false;
            _error = error;
        }
    }
}
