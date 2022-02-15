namespace LibraryTest
{
    public static class TestInterfacesAndClasses
    {
        public interface IService {}

        public class Service1 : IService {}

        public class Service2 : IService {}
    
        public abstract class AbstractService : IService {}
    
        public class AbstractServiceImpl : AbstractService {}
    
        public class Service3 : IService
        {
            public IRepository Repository { get; set; }

            public Service3(IRepository repository)
            {
                Repository = repository;
            }
        }

        public interface IRepository{}

        public class Repository1 : IRepository {}

        public interface IService<TRepository> where TRepository : IRepository {}

        public class Service4<TRepository> : IService<TRepository> where TRepository : IRepository
        {
            public TRepository Repository { get; set; }
            public Service4(TRepository repository)
            {
                Repository = repository;
            }
        }
        
        public class Service5 : IService
        {
            public Service5(Service2 service2)
            {
                
            }
            public IRepository Repository { get; set; }
        }
        
        public interface IA { }

        public class A : IA
        {
            public IB b { get; set; }

            
            public A(IB b)
            {
                this.b = b;
            }

            public A()
            {
                
            }
            
            
        }
        
        public interface IB {}

        public class B : IB
        {
            public IA a { get; set; }
            public IC c { get; set; }


            public B(IA a, IC c)
            {
                this.a = a;
                this.c = c;
            }

            public B() {}
        }
        
        public interface IC {}

        public class C : IC
        {
            public IB b { get; set; }
            public IQ q { get; set; }
            
            public C(IB b, IQ q)
            {
                this.b = b;
                this.q = q;
            }

            public C() {}
        }
        
        
        public interface IQ { }

        public class Q : IQ
        {
            public IA a { get; set; }

            
            public Q(IA a)
            {
                this.a = a;
            }

            public Q()
            {
                
            }
            
            
        }
        
        public interface IW {}

        public class W : IW
        {
            public IE e { get; set; }

            
            public W(IE e)
            {
                this.e = e;
            }

            public W() {}
        }
        
        public interface IE {}

        public class E : IE
        {
            public IQ q { get; set; }

            
            public E(IQ q)
            {
                this.q = q;
            }

            public E() {}
        }
        
    }
}