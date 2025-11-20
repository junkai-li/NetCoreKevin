using Microsoft.Extensions.DependencyInjection;

namespace Kevin.Email
{
    public static class ServiceCollectionExtensions
    { 
        public static void AddEmailService(this IServiceCollection services, Action<EmailSetting> action)
        {
            services.Configure(action); 
            services.AddTransient<IEmailService, EmailService>(); 
        } 
    }
}
