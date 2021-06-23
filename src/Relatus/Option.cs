using System;
using System.Collections.Generic;

namespace Relatus
{
    public class Option<T> : IEquatable<Option<T>>
    {
        private enum OptionType
        {
            Some,
            None
        }

        private readonly OptionType meta;
        private readonly T contents;

        private Option(OptionType meta, T contents)
        {
            this.meta = meta;
            this.contents = contents;
        }

        public bool IsSome()
        {
            return meta == OptionType.Some;
        }

        public bool IsSome(out T value)
        {
            value = contents;

            return meta == OptionType.Some;
        }

        public bool IsNone()
        {
            return meta == OptionType.None;
        }

        public T Expect(string errorMessage)
        {
            return meta switch
            {
                OptionType.Some => contents,
                _ => throw new Exception(errorMessage),
            };
        }

        public T Unwrap()
        {
            return meta switch
            {
                OptionType.Some => contents,
                _ => throw new Exception("Attempted to unwrap a `None` value."),
            };
        }

        public T UnwrapOr(T fallbackValue)
        {
            return meta switch
            {
                OptionType.Some => contents,
                _ => fallbackValue,
            };
        }

        public T UnwrapOrElse(Func<T> onError)
        {
            return meta switch
            {
                OptionType.Some => contents,
                _ => onError(),
            };
        }

        public Option<U> Map<U>(Func<T, U> onSuccess)
        {
            return meta switch
            {
                OptionType.Some => Option<U>.Some(onSuccess(contents)),
                _ => Option<U>.None(),
            };
        }

        public U MapOr<U>(U fallbackValue, Func<T, U> onSuccess)
        {
            return meta switch
            {
                OptionType.Some => onSuccess(contents),
                _ => fallbackValue,
            };
        }

        public U MapOrElse<U>(Func<U> onError, Func<T, U> onSuccess)
        {
            return meta switch
            {
                OptionType.Some => onSuccess(contents),
                _ => onError(),
            };
        }

        public Option<U> AndThen<U>(Func<T, Option<U>> onSuccess)
        {
            return meta switch
            {
                OptionType.Some => onSuccess(contents),
                _ => Option<U>.None(),
            };
        }

        public static Option<T> Some(T value)
        {
            if (value is null)
                throw new ArgumentException("An Option cannot contain a `null` value; use `Option<T>.None()` to represent an empty value.");

            return new Option<T>(OptionType.Some, value);
        }

        public static Option<T> None()
        {
            return new Option<T>(OptionType.None, default);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Option<T>);
        }

        public bool Equals(Option<T> other)
        {
            return other is { } &&
                   meta == other.meta &&
                   EqualityComparer<T>.Default.Equals(contents, other.contents);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(meta, contents);
        }

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return EqualityComparer<Option<T>>.Default.Equals(left, right);
        }

        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !(left == right);
        }
    }
}
