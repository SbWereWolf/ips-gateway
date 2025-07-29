using MethodBoundaryAspect.Fody.Attributes;

namespace archivist
{
    public sealed class LogAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            IndependentFileArchivist Archivist = new();

            string correlationId = string.Empty;
            if (args.Instance != null )
            {
                var instance = (ILogable)args.Instance;
                correlationId = instance.GetCorrelationId();
            }

            Archivist.Initialize(correlationId, args.Method);
            Archivist.Before(DateTime.Now, args.Arguments);

            args.MethodExecutionTag = Archivist;
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            var Archivist = GetArchivist(args);
            Archivist.After(DateTime.Now, args.ReturnValue);
        }

        public override void OnException(MethodExecutionArgs args)
        {
            var Archivist = GetArchivist(args);
            Archivist.Exception(DateTime.Now, args.Exception);
        }
        private static IndependentFileArchivist GetArchivist(MethodExecutionArgs args)
        {
            return (IndependentFileArchivist)args.MethodExecutionTag;
        }
    }
}