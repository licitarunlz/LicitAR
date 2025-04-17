using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.FileStorage.DI
{
    public static class DIFileStorageRegistrations
    {

        /*Acá se ponen las registraciones de los managers de nuestro negocio*/
        public static IServiceCollection AddFileStorageRegistrations(this IServiceCollection services, IConfiguration config)
        {
/*
            services.AddSingleton(new BlobService(
                config["AzureBlobStorage:ConnectionString"],
                config["AzureBlobStorage:ContainerName"]
            ));
*/
            return services;

        }
    }
}