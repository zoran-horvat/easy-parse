using System;

namespace EasyParse.Fluent.Rules
{
    public interface IPendingMapping : IProductionBuilder
    {
        IRule To<T1, TResult>(Func<T1, TResult> transform);
        IRule To<T1, T2, TResult>(Func<T1, T2, TResult> transform);
        IRule To<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> transform);
        IRule To<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> transform);
        IRule To<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> transform);
        IRule To<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> transform);
        IRule To<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> transform);
        IRule To<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> transform);
        IRule To<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> transform);
        IRule To<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> transform);

        IRule ToIdentity<T>() => To((T x) => x);

        IRule ToLiteral();
        IRule ToLiteral<TResult>(Func<string, TResult> transform);
    }
}