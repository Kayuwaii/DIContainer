namespace DIContainer
{
    public class DIContainer
    {
        public Dictionary<Type, ServiceProviderInstuctions> RegisteredServices = new Dictionary<Type, ServiceProviderInstuctions>();

        #region Registering
        public void RegisterSingleton(object sgltn)
        {
            RegisteredServices.Add(sgltn.GetType(), new ServiceProviderInstuctions
            {
                LifeCycle = LifeCycle.Singleton,
                ServiceType = sgltn.GetType(),
                Instance = sgltn
            });
        }

        public void RegisterTransient<T1>()
        {
            if (RegisteredServices.ContainsKey(typeof(T1))) return;
            else
            {
                RegisteredServices.Add(typeof(T1), new ServiceProviderInstuctions
                {
                    LifeCycle = LifeCycle.Transient,
                    ServiceType = typeof(T1),
                    Instance = null
                });
            }
        }

        public void RegisterTransient<T1, T2>() where T2 : T1
        {
            if (RegisteredServices.ContainsKey(typeof(T1)))
            {
                var register = RegisteredServices[typeof(T1)];
                register.ServiceType = typeof(T2);
            }
            else
            {
                RegisteredServices.Add(typeof(T1), new ServiceProviderInstuctions
                {
                    LifeCycle = LifeCycle.Transient,
                    ServiceType = typeof(T2),
                    Instance = null
                });
            }
        }
        #endregion

        #region Resolving
        public T Resolve<T>()
        {
            return (T) Resolve(typeof(T));
        }

        private object Resolve(Type T)
        {
            if (!RegisteredServices.ContainsKey(T)) throw new KeyNotFoundException();
            var service = RegisteredServices[T];
            switch (service.LifeCycle)
            {
                case LifeCycle.Transient:
                    return InitializeNewServiceWithDependencies(service.ServiceType);
                case LifeCycle.Singleton:
                    return service.Instance;
                default:
                    throw new Exception("SOMETHING WENT HORRIBLY WRONG");
            }
        }

        private object InitializeNewServiceWithDependencies(Type serviceType)
        {
            var ax = serviceType.GetConstructors().OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();
            if (ax == null || ax.GetParameters().Length == 0) return Activator.CreateInstance(serviceType);
            var args = ax.GetParameters();
            object[] resolvedDeps = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var dep = args[i];
                resolvedDeps[i] = Resolve(dep.ParameterType);
            }
            return Activator.CreateInstance(serviceType, resolvedDeps);

        }
        #endregion
    }
}