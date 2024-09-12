using PaymentProcessor.Services;

namespace PaymentProcessor.Infrastructure
{
    public static class PaymentProcessCollection
    {
        public static IServiceCollection AddPayments(this IServiceCollection services)
        {
            services.AddSingleton<IPaymentService, PaymentService>();
            return services;
        }

        public static IServiceCollection AddPayments(this IServiceCollection services, IConfiguration config)
        {
            return services;

            //var configSection = config.GetSection("StorageSettings");
            //var storageSettings = new StorageSettings();
            //configSection.Bind(storageSettings);
            //services.AddSingleton<StorageSettings>(storageSettings);
            //services.AddSingleton<TableDbContext>();

            //    IAsyncPolicy<HttpResponseMessage> wrapOfRetryAndFallback =
            //     Policy.WrapAsync(GetRetryPolicy, CircuitBreakerPolicy);
            //    services.AddHttpClient<PaymentService>()
            //        .AddPolicyHandler(wrapOfRetryAndFallback);
            //    return services;
            //}

            //public static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy
            //   = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            //     .OrResult(msg => (int)msg.StatusCode == 418)
            //     .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            //public static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy
            //    = Policy
            //    .HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.InternalServerError)
            //    .CircuitBreakerAsync(1, TimeSpan.FromSeconds(20), OnBreak, OnReset, OnHalfOpen);

            //static void OnHalfOpen()
            //{
            //    Console.WriteLine(">>>>>>>>>>>>> Circuit is half open, one request will be allowed.");
            //}

            //static void OnReset()
            //{
            //    Console.WriteLine(">>>>>>>>>>>>> Circuit closed, all is good!");
            //}

            //static void OnBreak(DelegateResult<HttpResponseMessage> result, TimeSpan ts)
            //{
            //    Console.WriteLine(">>>>>>>>>>>>> Circuit open, something is wrong!");
            //}
        }
    }
}


