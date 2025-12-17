namespace Shop_Microservicios.Exceptions
{
    public class ServiceUnavailableException : Exception
    {
        public string? ServiceName { get; }

        public ServiceUnavailableException(string message, string? serviceName = null, Exception? inner = null)
            : base(message, inner)
        {
            ServiceName = serviceName;
        }
    }
}
