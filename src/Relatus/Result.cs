using System;
using System.Collections.Generic;

namespace Relatus
{
    public class Result<T, E> : IEquatable<Result<T, E>>
    {
        private enum ResultType
        {
            Ok,
            Err
        }

        private readonly ResultType meta;
        private readonly T contents;
        private readonly E error;

        private Result(ResultType meta, T contents, E error)
        {
            this.meta = meta;
            this.contents = contents;
            this.error = error;
        }

        public bool IsOk()
        {
            return meta == ResultType.Ok;
        }

        public bool IsOk(out T value)
        {
            value = contents;

            return meta == ResultType.Ok;
        }

        public bool IsErr()
        {
            return meta == ResultType.Err;
        }

        public bool IsErr(out E error)
        {
            error = this.error;

            return meta == ResultType.Err;
        }

        public T Expect(string errorMessage)
        {
            return meta switch
            {
                ResultType.Ok => contents,
                _ => throw new Exception(errorMessage)
            };
        }

        public E ExpectErr(string errorMessage)
        {
            return meta switch
            {
                ResultType.Err => error,
                _ => throw new Exception(errorMessage)
            };
        }

        public T Unwrap()
        {
            return meta switch
            {
                ResultType.Ok => contents,
                _ => throw new Exception("Called `Unwrap` on an `Err` value.")
            };
        }

        public E UnwrapErr()
        {
            return meta switch
            {
                ResultType.Err => error,
                _ => throw new Exception("Called `UnwrapErr` on an `Ok` value.")
            };
        }

        public T UnwrapOr(T fallbackValue)
        {
            return meta switch
            {
                ResultType.Ok => contents,
                _ => fallbackValue
            };
        }

        public T UnwrapOrElse(Func<T> onError)
        {
            return meta switch
            {
                ResultType.Ok => contents,
                _ => onError()
            };
        }

        public Result<U, E> Map<U>(Func<T, U> onSuccess)
        {
            return meta switch
            {
                ResultType.Ok => Result<U, E>.Ok(onSuccess(contents)),
                _ => Result<U, E>.Err(error)
            };
        }

        public U MapOr<U>(U fallbackValue, Func<T, U> onSuccess)
        {
            return meta switch
            {
                ResultType.Ok => onSuccess(contents),
                _ => fallbackValue
            };
        }

        public U MapOrElse<U>(Func<E, U> onError, Func<T, U> onSuccess)
        {
            return meta switch
            {
                ResultType.Ok => onSuccess(contents),
                _ => onError(error)
            };
        }

        public Result<U, E> AndThen<U>(Func<T, Result<U, E>> onSuccess)
        {
            return meta switch
            {
                ResultType.Ok => onSuccess(contents),
                _ => Result<U, E>.Err(error)
            };
        }

        public static Result<T, E> Ok(T value)
        {
            return new Result<T, E>(ResultType.Ok, value, default);
        }
        public static Result<T, E> Err(E error)
        {
            return new Result<T, E>(ResultType.Err, default, error);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Result<T, E>);
        }

        public bool Equals(Result<T, E> other)
        {
            return other is { } &&
                   meta == other.meta &&
                   EqualityComparer<T>.Default.Equals(contents, other.contents) &&
                   EqualityComparer<E>.Default.Equals(error, other.error);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(meta, contents, error);
        }

        public static bool operator ==(Result<T, E> left, Result<T, E> right)
        {
            return EqualityComparer<Result<T, E>>.Default.Equals(left, right);
        }

        public static bool operator !=(Result<T, E> left, Result<T, E> right)
        {
            return !(left == right);
        }
    }
}
