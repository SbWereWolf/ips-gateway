using System.Reflection;

namespace archivist
{
    /* base on https://learn.microsoft.com/ru-ru/archive/msdn-magazine/2014/february/aspect-oriented-programming-aspect-oriented-programming-with-the-realproxy-class */
    /* base on https://stackoverflow.com/questions/38467753/realproxy-in-dotnet-core */
    public class LoggingDecorator<T> : DispatchProxy
    {
        private readonly FileArchivist Archivist = new();

        private T _decorated;

        protected override object? Invoke(
            MethodInfo? targetMethod, 
            object?[]? args
            )
        {
            var runId = Guid.NewGuid().ToString();

            try
            {
                if (targetMethod?.Name != "ToString")
                {
                    var classname = _decorated?.GetType().FullName ?? "";
                    Archivist.Before(
                        runId,
                        DateTime.Now, 
                        args, 
                        classname, 
                        targetMethod
                        );
                }

                var result = targetMethod?.Invoke(_decorated, args);


                if (targetMethod?.Name != "ToString")
                {
                    Archivist.After(runId, DateTime.Now, result);
                }

                return result;
            }
            catch (Exception ex) when (ex is TargetInvocationException)
            {
                var realException = ex.InnerException ?? ex;

                if (targetMethod?.Name != "ToString")
                {
                    Archivist.Exception(
                        runId, 
                        DateTime.Now, 
                        realException
                        );
                }

                throw realException;
            }
        }

        public static T Create(T decorated)
        {
            object proxy = Create<T, LoggingDecorator<T>>();
            ((LoggingDecorator<T>)proxy).SetParameters(decorated);

            return (T)proxy;
        }

        private void SetParameters(T decorated)
        {
            if (decorated == null)
            {
                throw new ArgumentNullException(nameof(decorated));
            }
            _decorated = decorated;
        }
    }
}