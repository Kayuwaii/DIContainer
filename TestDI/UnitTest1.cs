using DIContainer;
using NUnit.Framework;
using System;

namespace TestDI
{
    public class Tests
    {
        protected DIContainer.DIContainer IocManager;

        [SetUp]
        public void Setup()
        {
            IocManager = new DIContainer.DIContainer();
        }

        [Test]
        public void RegisterSingleton()
        {
            int initial = IocManager.RegisteredServices.Count;
            IocManager.RegisterSingleton(new CountClass());
            int final = IocManager.RegisteredServices.Count;
            Assert.Less(initial, final);
        }

        [Test]
        public void ResolveSingleton()
        {
            IocManager.RegisterSingleton(new CountClass());
            var service = IocManager.Resolve<CountClass>();
            Assert.IsInstanceOf<CountClass>(service);
        }

        [Test]
        public void TestSingleton()
        {
            IocManager.RegisterSingleton(new CountClass());
            var service = IocManager.Resolve<CountClass>();
            int prev = service.Count;
            var service2 = IocManager.Resolve<CountClass>();
            service2.Count++;
            Assert.Less(prev, service2.Count);
        }

        [Test]
        public void RegisterTransient()
        {
            int initial = IocManager.RegisteredServices.Count;
            IocManager.RegisterTransient<FirstRandom>();
            int final = IocManager.RegisteredServices.Count;
            Assert.Less(initial, final);
        }

        [Test]
        public void ResolveTransient()
        {
            IocManager.RegisterTransient<FirstRandom>();
            var service = IocManager.Resolve<FirstRandom>();
            Assert.IsInstanceOf<FirstRandom>(service);
        }

        [Test]
        public void TestTransient()
        {
            IocManager.RegisterTransient<FirstRandom>();
            var service = IocManager.Resolve<FirstRandom>();
            var prev = service.random;
            var service2 = IocManager.Resolve<FirstRandom>();
            Assert.AreNotEqual(prev, service2.random);
        }

        [Test]
        public void TestTransientWithDependencies()
        {
            IocManager.RegisterTransient<FirstRandom>();
            IocManager.RegisterTransient<SecondRandom>();
            var service = IocManager.Resolve<SecondRandom>();
            Assert.IsInstanceOf<SecondRandom>(service);
        }

        [Test]
        public void TestTransientWithDepsByInterface()
        {
            IocManager.RegisterTransient<FirstRandom>();
            IocManager.RegisterTransient<IRandomStuff, SecondRandom>();
            var service = IocManager.Resolve<IRandomStuff>();
            Assert.IsInstanceOf<SecondRandom>(service);
        }

    }

    public class CountClass
    {
        public int Count { get; set; } = 0;
    }

    public interface IRandomStuff
    {
        public int NotSoRandom();
    }

    public class FirstRandom : IRandomStuff
    {
        public Guid random = Guid.NewGuid();
        public int NotSoRandom()
        {
            return 1;
        }
    }

    public class SecondRandom : IRandomStuff
    {
        private readonly FirstRandom random;

        public SecondRandom(FirstRandom random)
        {
            this.random = random;
        }

        public int NotSoRandom()
        {
            return 2;
        }
        public int ChildRandom()
        {
            return random.NotSoRandom();
        }
    }
}