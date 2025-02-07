namespace Application.Exceptions
{
    public class TrialDataException : Exception
    {
        public TrialDataException(string message) : base(message) { }

        public TrialDataException(string message, Exception innerException)
          : base(message, innerException)
        {
        }
    }
}
