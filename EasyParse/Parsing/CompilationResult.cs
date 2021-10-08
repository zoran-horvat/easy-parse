namespace EasyParse.Parsing
{
    public class CompilationResult<T>
    {
        private CompilationResult(T result, string errorMessage, bool isSuccess)
        {
            this.IsSuccess = isSuccess;
            this.Result = result;
            this.ErrorMessage = errorMessage;
        }

        internal static CompilationResult<T> Success(T value) =>
            new(value, string.Empty, true);

        internal static CompilationResult<T> Error(string message) =>
            new(default, message, false);

        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
        public T Result { get; }

        public override string ToString() => 
            this.IsSuccess ? this.Result?.ToString() ?? "<null>"
            : this.ErrorMessage;
    }
}
