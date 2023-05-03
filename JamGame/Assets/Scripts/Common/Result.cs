using System;

namespace Common
{
    public abstract class Result
    {
        public bool Success { get; protected set; }
        public bool Failure => !Success;
    }
    public abstract class Result<T> : Result
    {
        private T _data;

        protected Result(T data)
        {
            Data = data;
        }

        public T Data
        {
            get => Success ? _data : throw new Exception($"You can't access .{nameof(Data)} when .{nameof(Success)} is false");
            set => _data = value;
        }
    }
    public class SuccessResult : Result
    {
        public SuccessResult()
        {
            Success = true;
        }
    }
    public interface IFailResult
    {
        string FailCause { get; }
    }
    public class FailResult : Result, IFailResult
    {
        public string FailCause { get; protected set; }
        public FailResult(string failCause)
        {
            Success = false;
            FailCause = failCause;
        }
    }
    public class FailResult<T> : Result<T>, IFailResult
    {
        public string FailCause { get; protected set; }
        public FailResult(T data, string failCause) : base(data)
        {
            Success = false;
            FailCause = failCause;
        }
    }
    public class SuccessResult<T> : Result<T>
    {
        public SuccessResult(T data) : base(data)
        {
            Success = true;
        }
    }
}
