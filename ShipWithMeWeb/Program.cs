using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ShipWithMe
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider(options =>
                {
                    options.ValidateScopes = true;
                })
                .ConfigureServices((context, services) =>
                {
                    CertConfig.CertificatePath = context.Configuration["CertificatePath"];
                    CertConfig.CertificatePassword = context.Configuration["CertificatePassword"];
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        if (!CertConfig.IsEmpty) {
                            options.ConfigureHttpsDefaults(options =>
                            {
                                options.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(CertConfig.CertificatePath, CertConfig.CertificatePassword);
                            });
                        }

                        options.Limits.MaxRequestBodySize = 1024 * 1024 * 1024;
                    });

                    var configuration = new ConfigurationBuilder()
                        .AddJsonFile("secrets.develop.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("secrets.production.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args)
                        .Build();

                    webBuilder.UseConfiguration(configuration);
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseSetting("https_port", "443");
                });
        }
    }

    public static class CertConfig
    {
        public static string CertificatePath { get; set; }

        public static string CertificatePassword { get; set; }

        public static bool IsEmpty => CertificatePath == null || CertificatePassword == null;
    }
}
