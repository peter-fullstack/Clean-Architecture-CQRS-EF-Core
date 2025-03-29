namespace Domain.Exceptions
{
    public class DomainException : Exception
    {
        private readonly List<string> _errors = new();
        public DomainException(string message, List<string> errors) : base(message)
        {
            _errors = errors;
        }

        public List<string> Errors
        {
            get
            {
                return _errors;
            }
        }
    }

}
