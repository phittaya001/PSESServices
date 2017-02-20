using CSI.CastleWindsorHelper;
using CSI.CastleWindsorHelper.Fake;
using PesWeb.Interceptors;
using CSI.Common.Configuration;
using CSI.ModelHelper.Cache;
using CSI.Security.Authentication;
using CSI.Security.Authorization;
using PesWeb.Service.Security.Repositories;
using PesWeb.Service.Security;
using AutoMapper;
using PesWeb.Service.Common.Repositories;
using PesWeb.Service.Common;

namespace PesWeb.Injectors
{
    public class ServiceRegister
    {
        private static MapperConfiguration MapperConfig;
        public static void Register()
        {
            // interceptors
            ServiceContainer.RegisterServiceSingleton<FakeObjectInterceptor>();
            ServiceContainer.RegisterServiceSingleton<LoggingInterceptor>();
            ServiceContainer.RegisterServiceSingleton<CachedMethodInterceptor>();
            ServiceContainer.RegisterServiceSingleton<IFakeValueGenerator, RandomValueGenerator>();

            // mapper
            //MapperConfig = new MapperConfiguration(a => { a.CreateMissingTypeMaps = true; });
            //ServiceContainer.RegisterInstance<IMapper>(MapperConfig.CreateMapper());

            // security services
            //ServiceContainer.RegisterServiceSingleton<IAuthenticationRepository, AuthenticationRepo>
            //    (typeof(LoggingInterceptor), typeof(CachedMethodInterceptor));

            //ServiceContainer.RegisterServiceSingleton<IAuthorizationRepository, AuthorizationRepo>
            //    (typeof(LoggingInterceptor), typeof(CachedMethodInterceptor));

            ServiceContainer.RegisterServiceSingleton<IAuthentication, BypassAuthentication>
                ((k, d) => { d["repository"] = ServiceContainer.GetService<IAuthenticationRepository>(); });
            //ServiceContainer.RegisterServiceSingleton<IAuthentication, DefaultAuthentication>
            //    ((k, d) =>
            //    {
            //        d["repository"] = ServiceContainer.GetService<IAuthenticationRepository>();
            //        d["enableAutoAuthen"] = true;
            //    });
            //ServiceContainer.RegisterServiceSingleton<IAuthentication, LdapAuthentication>
            //    ((k, d) => { d["domainName"] = "csigroups"; d["ldapIp"] = "192.168.11.1"; d["ldapPort"] = 389; });

            ServiceContainer.RegisterServiceSingleton<IAuthorization, BypassAuthorization>();
            //ServiceContainer.RegisterServiceSingleton<IAuthorization, DefaultAuthorization>
            //    ((k, d) => { d["reposotiry"] = ServiceContainer.GetService<IAuthorizationRepository>(); });

            ServiceContainer.RegisterServiceTransient<UserMaintenanceSvc>
                (typeof(LoggingInterceptor), typeof(CachedMethodInterceptor));

            ServiceContainer.RegisterServiceTransient<GroupMaintenanceSvc>
                (typeof(LoggingInterceptor), typeof(CachedMethodInterceptor));

            ServiceContainer.RegisterServiceTransient<PermissionMaintenanceSvc>
                (typeof(LoggingInterceptor), typeof(CachedMethodInterceptor));

            ServiceContainer.RegisterServiceSingleton<DbMessageBoxRepository>
                (typeof(LoggingInterceptor), typeof(CachedMethodInterceptor));
            ServiceContainer.RegisterServiceSingleton<IMessageBoxSvc, DbMessageBoxSvc>
                (typeof(LoggingInterceptor), typeof(CachedMethodInterceptor));

            // Business services
            ServiceContainer.RegisterServiceSingleton<PesWeb.Service.Modules.UserLogSvr>
                   (typeof(LoggingInterceptor), typeof(CachedMethodInterceptor));
            ServiceContainer.RegisterServiceSingleton<PesWeb.Service.Modules.FormManage>
                   (typeof(LoggingInterceptor), typeof(CachedMethodInterceptor));
            ServiceContainer.RegisterServiceSingleton<PesWeb.Service.Modules.HeaderManage>
                   (typeof(LoggingInterceptor), typeof(CachedMethodInterceptor));
        }
    }
}
