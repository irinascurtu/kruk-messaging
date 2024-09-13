using Contracts.Infrastructure.Mappings;
using Contracts.Responses;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orders.Data;
using Orders.Domain;
using Orders.Service;
using OrdersApi.Consumers;
using OrdersApi.Service.Clients;
using OrdersApi.Services;
using Polly;
using Polly.Hedging;
using System.Net;
using System.Reflection;

namespace OrdersApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });
            builder.Services.AddAutoMapper(typeof(OrderProfileMapping));
            builder.Services.AddDbContext<OrderContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            //add MassTransit middleware

            //builder.Services.AddMassTransit(x =>
            //{
            //    x.UsingRabbitMq((context, cfg) =>
            //    {
            //        cfg.Host("rabbitmq://localhost", "/", h =>
            //        {
            //            h.Username("guest");
            //            h.Password("guest");
            //        });

            //        cfg.ConfigureEndpoints(context);
            //    });
            //});



            builder.Services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                // x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("hellos", true));//takes the namespace too
                // x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("hellos", false));//takes the namespace too

                x.AddConsumer<OrderCreatedConsumer>();
                x.AddConsumer<VerifyOrderConsumer>();

                x.AddRequestClient<VerifyOrder>();

                //let masstransit scan and find consumers
                //var entryAssembly = Assembly.GetEntryAssembly();
                //x.AddConsumers(entryAssembly);

                // Step 2: Select a Transport
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("order-created", e =>
                    {
                        //e.Batch<OrderCreated>(b =>
                        //    {
                        //        b.MessageLimit = 5;
                        //        b.TimeLimit = TimeSpan.FromSeconds(5);
                        //    });

                        //  e.UseRateLimit(100, TimeSpan.FromSeconds(1));
                        e.ConfigureConsumer<OrderCreatedConsumer>(context);
                      // e.UseMessageRetry(r => r.Interval(2, 1000));
                    });

                    // Step 4: Configure Endpoints
                    // All consumers registered in step 1, will get
                    // default endpoints created.
                    cfg.ConfigureEndpoints(context);
                });
            });


            //add grpc client
            builder.Services.AddGrpcClient<Stocks.Greeter.GreeterClient>(o =>
            {
                o.Address = new Uri("https://localhost:7277");
            });

            builder.Services.AddOpenTelemetry()
           .ConfigureResource(resource => resource.AddService("OrdersApi"))
           .WithTracing(tracing =>
           {
               tracing
                   .AddAspNetCoreInstrumentation()
                   .AddHttpClientInstrumentation()
                   .AddEntityFrameworkCoreInstrumentation(options =>
                   {
                       options.EnrichWithIDbCommand = (activity, command) =>
                       {
                           var stateDisplayName = $"{command.CommandType} main";
                           activity.DisplayName = stateDisplayName;
                           activity.SetTag("db.name", stateDisplayName);
                       };
                   });

               tracing.AddOtlpExporter();
           });

            //adding resilience

            //builder.Services.AddHttpClient<IProductStockServiceClient, ProductStockServiceClient>()
            //    .AddStandardHedgingHandler();


            builder.Services.AddHttpClient<IProductStockServiceClient, ProductStockServiceClient>()
                .AddResilienceHandler("my-policy", builder =>
                {
                    //builder.AddRetry(new HttpRetryStrategyOptions
                    //{
                    //    MaxRetryAttempts = 2,
                    //    Delay = TimeSpan.FromSeconds(1),
                    //    BackoffType = DelayBackoffType.Exponential
                    //});

                    builder.AddHedging(new HedgingStrategyOptions<HttpResponseMessage>()
                    {
                        MaxHedgedAttempts = 3,
                        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
    .Handle<ArgumentOutOfRangeException>()
    .HandleResult(response => response.StatusCode == HttpStatusCode.InternalServerError),
                        Delay = TimeSpan.FromSeconds(1),
                    });

                });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<OrderContext>().Database.EnsureCreated();
                }
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
