using System.Collections.Generic;
using DependencyInjectionContainerLibrary;
using DependencyInjectionContainerLibrary.Exceptions;
using NUnit.Framework;
using static LibraryTest.TestInterfacesAndClasses;

namespace LibraryTest
{
    public class DIContainerLibraryTest
    {
        private DependencyConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new DependencyConfiguration();
        }

        [Test]
        public void ResolveBasicTypes()
        {
            _configuration.Register<IService, Service1>();
            _configuration.Register<AbstractService, AbstractServiceImpl>();

            DependencyProvider provider = new DependencyProvider(_configuration);
            object service1 = provider.Resolve<IService>();
            object abstractServiceImpl = provider.Resolve<AbstractService>();

            Assert.AreEqual(typeof(Service1), service1.GetType());
            Assert.AreEqual(typeof(AbstractServiceImpl), abstractServiceImpl.GetType());
        }
        
        [Test]
        public void ResolveRecursiveDependency()
        {
            _configuration.Register<IService, Service3>();
            _configuration.Register<IRepository, Repository1>();
            
            DependencyProvider provider = new DependencyProvider(_configuration);
            IService service = (IService)provider.Resolve<IService>();
            
            Assert.AreEqual( typeof(Service3), service.GetType());
        }
        
        [Test]
        public void ResolveNotSingletonCreating()
        {
            _configuration.Register<IService, Service1>(false);
            
            DependencyProvider provider = new DependencyProvider(_configuration);
            object service1 = provider.Resolve<IService>();
            object service2 = provider.Resolve<IService>();

            Assert.AreNotEqual( service1, service2);
        }
        
        [Test]
        public void ResolveEnumerable()
        {
            _configuration.Register<IService, Service1>();
            _configuration.Register<IService, Service2>();
            _configuration.Register<IService, Service3>();
            _configuration.Register<IRepository, Repository1>();
            
            DependencyProvider provider = new DependencyProvider(_configuration);
            List<object> services = provider.Resolve<IEnumerable<IService>>() as List<object>;
            
            Assert.AreEqual(3, services?.Count);
            Assert.AreEqual(typeof(Service1), services[0].GetType());
            Assert.AreEqual(typeof(Service2), services[1].GetType());
            Assert.AreEqual(typeof(Service3), services[2].GetType());
        }
        
        [Test]
        public void ResolveGenericDependency()
        {
            _configuration.Register<IRepository, Repository1>();
            _configuration.Register<IService<IRepository>, Service4<IRepository>>();
            
            DependencyProvider provider = new DependencyProvider(_configuration);
            object service = provider.Resolve<IService<IRepository>>();
        
            Assert.AreEqual(typeof(Service4<IRepository>), service.GetType());
            Assert.NotNull((service as Service4<IRepository>)?.Repository);    
        }
        
        [Test]
        public void ResolveOpenGenericsDependency()
        {
            _configuration.Register<IRepository, Repository1>();
            _configuration.Register(typeof(IService<>), typeof(Service4<>));
            
            DependencyProvider provider = new DependencyProvider(_configuration);
            object service = provider.Resolve<IService<IRepository>>();
        
            Assert.AreEqual(typeof(Service4<IRepository>), service.GetType());
            Assert.NotNull((service as Service4<IRepository>)?.Repository);
        }

        [Test]
        public void ResolveUnsupportedType()
        {
            DependencyProvider provider = new DependencyProvider(_configuration);
            Assert.Throws<UnsupportedTypeException>(() => provider.Resolve<Service1>());
        }

        [Test]
        public void ResolveTypeWithoutSuitableConstructor()
        {
            _configuration.Register<IService, Service5>();
            _configuration.Register<IRepository, Repository1>();

            DependencyProvider provider = new DependencyProvider(_configuration);
            Assert.Throws<NoSuitableConstructorException>(() => provider.Resolve<IService>());
        }
        
        [Test]
        public void ResolveCyclingDepencies()
        {
            _configuration.Register<IA, A>();
            _configuration.Register<IB, B>();  
            _configuration.Register<IC, C>();
            _configuration.Register<IQ, Q>();

            DependencyProvider provider = new DependencyProvider(_configuration);
            
            A a = (A) provider.Resolve<IA>();
            B b = (B) provider.Resolve<B>();
            C c = (C) provider.Resolve<IC>();
            Q q = (Q) provider.Resolve<IQ>();
            
            Assert.AreSame(a, b.a);
            Assert.AreSame(b.c,c);
            Assert.AreSame(c.b,b);
            Assert.AreSame(c.q,q);
            Assert.AreSame(q.a,a);
        }
        
    }
}