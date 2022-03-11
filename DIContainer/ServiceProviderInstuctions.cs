namespace DIContainer
{
    public class ServiceProviderInstuctions
    {
        public LifeCycle LifeCycle { get; set; }
        public Type ServiceType { get; set; }
        public object? Instance { get; set; } = null;
    }
}