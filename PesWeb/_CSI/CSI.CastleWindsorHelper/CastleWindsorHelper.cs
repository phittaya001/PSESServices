using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.DynamicProxy;
using System;
using Castle.MicroKernel.Registration;
using Castle.Core;
using CSI.CastleWindsorHelper.Fake;

namespace CSI.CastleWindsorHelper
{
    public static class ServiceContainer
    {
        static private WindsorContainer TheContainer = null;

        static ServiceContainer()
        {
            TheContainer = new WindsorContainer();
        }

        public static WindsorContainer Container
        {
            get { return TheContainer; }
        }

        static public void LoadConfig(string configUri)
        {
#if DEBUG
            try {
                TheContainer = new WindsorContainer(new XmlInterpreter(new ConfigResource(configUri)));
            }
#pragma warning disable
            catch (Exception e)
#pragma warning restore
            {
                throw;
            }
#else
            TheContainer = new WindsorContainer(new XmlInterpreter(new ConfigResource(configUri)));
#endif
        }
        static public void RegisterService<Tclass>(params Type[] intercepters)
            where Tclass : class
        {
            TheContainer.Register(Component
                .For<Tclass>()
                .Interceptors(intercepters));
        }
        static public void RegisterService(Type type, params Type[] intercepters)
        {
            TheContainer.Register(Component
                .For(type)
                .Interceptors(intercepters));
        }
        static public void RegisterService(Type type, DynamicParametersDelegate resolve, params Type[] intercepters)
        {
            TheContainer.Register(Component
                .For(type)
                .DynamicParameters(resolve)
                .Interceptors(intercepters));
        }
        static public void RegisterService<Tinterface, Tclass>(params Type[] intercepters)
            where Tinterface : class
            where Tclass : Tinterface
        {
            TheContainer.Register(Component
                .For<Tinterface>()
                .ImplementedBy<Tclass>()
                .Interceptors(intercepters));
        }
        static public void RegisterService<Tinterface, Tclass>(DynamicParametersDelegate resolve, params Type[] intercepters)
            where Tinterface : class
            where Tclass : Tinterface
        {
            TheContainer.Register(Component
                .For<Tinterface>()
                .ImplementedBy<Tclass>()
                .DynamicParameters(resolve)
                .Interceptors(intercepters));
        }
        static public void RegisterServiceSingleton<Tclass>(params Type[] intercepters)
            where Tclass : class
        {
            TheContainer.Register(Component
                .For<Tclass>()
                .Interceptors(intercepters)
                .LifeStyle.Singleton);
        }
        static public void RegisterServiceSingleton<Tclass>(DynamicParametersDelegate resolve, params Type[] intercepters)
            where Tclass : class
        {
            TheContainer.Register(Component
                .For<Tclass>()
                .DynamicParameters(resolve)
                .Interceptors(intercepters)
                .LifeStyle.Singleton);
        }
        static public void RegisterServiceSingleton<Tinterface, Tclass>(params Type[] intercepters)
            where Tinterface : class
            where Tclass : Tinterface
        {
            TheContainer.Register(Component
                .For<Tinterface>()
                .ImplementedBy<Tclass>()
                .Interceptors(intercepters)
                .LifeStyle.Singleton);
        }
        static public void RegisterServiceSingleton<Tinterface, Tclass>(DynamicParametersDelegate resolve, params Type[] intercepters)
            where Tinterface : class
            where Tclass : Tinterface
        {
            TheContainer.Register(Component
                .For<Tinterface>()
                .ImplementedBy<Tclass>()
                .DynamicParameters(resolve)
                .Interceptors(intercepters)
                .LifeStyle.Singleton);
        }
        static public void RegisterServiceTransient<Tclass>(params Type[] intercepters)
            where Tclass : class
        {
            TheContainer.Register(Component
                .For<Tclass>()
                .Interceptors(intercepters)
                .LifeStyle.Transient);
        }
        static public void RegisterServiceTransient<Tclass>(DynamicParametersDelegate resolve, params Type[] intercepters)
            where Tclass : class
        {
            TheContainer.Register(Component
                .For<Tclass>()
                .DynamicParameters(resolve)
                .Interceptors(intercepters)
                .LifeStyle.Transient);
        }
        static public void RegisterServiceTransient<Tinterface, Tclass>(params Type[] intercepters)
            where Tinterface : class
            where Tclass : Tinterface
        {
            TheContainer.Register(Component
                .For<Tinterface>()
                .ImplementedBy<Tclass>()
                .Interceptors(intercepters)
                .LifeStyle.Transient);
        }
        static public void RegisterServiceTransient<Tinterface, Tclass>(DynamicParametersDelegate resolve, params Type[] intercepters)
            where Tinterface : class
            where Tclass : Tinterface
        {
            TheContainer.Register(Component
                .For<Tinterface>()
                .ImplementedBy<Tclass>()
                .DynamicParameters(resolve)
                .Interceptors(intercepters)
                .LifeStyle.Transient);
        }
        static public void RegisterInstance<Tinterface>(Tinterface instance, params Type[] interceptors)
            where Tinterface : class
        {
            TheContainer.Register(Component
                .For<Tinterface>()
                .Instance(instance)
                .Interceptors(interceptors));
        }

        static public T GetService<T>() where T : class
        {
            return GetService(typeof(T), null) as T;
        }
        static public T GetService<T>(object argsAnonymous) where T : class
        {
            return GetService(typeof(T), argsAnonymous) as T;
        }
        static public object GetService(Type type, object argsAnonymous)
        {
            try
            {
                if (null != argsAnonymous)
                    return TheContainer.Resolve(type, argsAnonymous);
                return TheContainer.Resolve(type);
            }
            catch (Castle.MicroKernel.ComponentNotFoundException)
            {
                if (typeof(IFakeValueGenerator).IsAssignableFrom(type))
                    return DefaultFakeValue();

                var all = TheContainer.ResolveAll<CSI.CastleWindsorHelper.Fake.FakeObjectInterceptor>();
                if (all.Length > 0)
                {
                    Type t = Fake.InterfaceWrapper.CreateType(type);
                    return CreateObjectWithIntercepter(t, all[0]);
                }
                throw;
            }
        }
        static public bool TryGetService<T>(out T svc) where T : class
        {
            try
            {
                svc = TheContainer.Resolve<T>();
                return true;
            }
            catch (Castle.MicroKernel.ComponentNotFoundException)
            {
                svc = null;
            }
            return false;
        }
        private static T CreateObjectWithIntercepter<T>(Type t, IInterceptor interceptor, params object[] args) where T : class
        {
            return CreateObjectWithIntercepter(t, interceptor, args) as T;
        }
        private static object CreateObjectWithIntercepter(Type t, IInterceptor interceptor, params object[] args)
        {
            ProxyGenerator pg = new ProxyGenerator();
            return pg.CreateClassProxy(t, args, interceptor);
        }
        private static IFakeValueGenerator DefaultFakeValue()
        {
            return new RandomValueGenerator();
        }
    }
}
