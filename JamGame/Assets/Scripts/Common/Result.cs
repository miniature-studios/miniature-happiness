using System;

namespace Common
{
    public abstract class Result
    {
        public bool Success { get; protected set; }
        public bool Failure => !Success;

        protected string Err;

        public string Error =>
            Failure
                ? Err
                : throw new Exception(
                    $"You can't access .{nameof(Error)} when .{nameof(Failure)} is false"
                );
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
            base.Err = error;
        }
    }

    public abstract class Result<T>
    {
        public bool Success { get; protected set; }
        public bool Failure => !Success;

        protected string Err;
        protected T Data_;

        public string Error =>
            Failure
                ? Err
                : throw new Exception(
                    $"You can't access .{nameof(Error)} when .{nameof(Failure)} is false"
                );
        public T Data =>
            Success
                ? Data_
                : throw new Exception(
                    $"You can't access .{nameof(Data)} when .{nameof(Success)} is false"
                );
    }

    public class SuccessResult<T> : Result<T>
    {
        public SuccessResult(T data)
        {
            Success = true;
            base.Data_ = data;
        }
    }

    public class FailResult<T> : Result<T>
    {
        public FailResult(string error)
        {
            Success = false;
            base.Err = error;
        }
    }
}
