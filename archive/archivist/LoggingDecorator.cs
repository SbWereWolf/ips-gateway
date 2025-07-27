using System.Reflection;

namespace archivist
{
    /* base on https://learn.microsoft.com/ru-ru/archive/msdn-magazine/2014/february/aspect-oriented-programming-aspect-oriented-programming-with-the-realproxy-class */
    /* base on https://stackoverflow.com/questions/38467753/realproxy-in-dotnet-core */
    public class LoggingDecorator<T> : DispatchProxy
    {
        private readonly FileArchivist Archivist = new();

        private T? _decorated;
        private string? _correlationId;

        protected override object? Invoke(
            MethodInfo? targetMethod,
            object?[]? args
            )
        {
            var shouldLogging = targetMethod?.Name != "ToString";
            if (shouldLogging && !Archivist.WasInitialized)
            {
                Archivist.Initialize(_correlationId, _decorated);
            }
            if (shouldLogging)
            {
                Archivist.GetReady(targetMethod);
            }

            if (shouldLogging)
            {
                Archivist.Before(DateTime.Now, args);
            }

            object? result;
            try
            {
                result = targetMethod?.Invoke(_decorated, args);
            }
            catch (Exception ex) when (ex is TargetInvocationException)
            {
                var realException = ex.InnerException ?? ex;

                if (shouldLogging)
                {
                    Archivist.Exception(DateTime.Now, realException);
                }

                throw realException;
            }

            if (shouldLogging)
            {
                Archivist.After(DateTime.Now, result);
            }

            return result;
        }

        public static T Create(T decorated, string? correlationId)
        {
            object proxy = Create<T, LoggingDecorator<T>>() ??
                throw new ArgumentNullException(
                    "Fail on create proxy for "
                    + (decorated?.GetType().FullName ?? "")
                    );
            ((LoggingDecorator<T>)proxy)
                .SetParameters(decorated, correlationId);

            return (T)proxy;
        }

        private void SetParameters(T decorated, string? correlationId)
        {
            if (decorated == null)
            {
                throw new ArgumentNullException(nameof(decorated));
            }

            _decorated = decorated;
            _correlationId = correlationId;

            Archivist.Initialize(_correlationId, _decorated);
        }
    }
}