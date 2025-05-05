using MediaTHOR.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediaTHOR.Implementations
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;
        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellation = default) where TNotification : INotification
        {
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
            var handlerInstances = _serviceProvider.GetServices(handlerType);

            if (handlerInstances == null)
                throw new InvalidOperationException($"Handler not found for {notification.GetType().Name}");

            foreach (var handlerInstance in handlerInstances)
            {
                await (Task)handlerType
                .GetMethod("Handle")!
                .Invoke(handlerInstance, new object[] { notification, cancellation });
            }
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellation = default)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            var handlerInstance = _serviceProvider.GetService(handlerType);

            if (handlerInstance == null)
                throw new InvalidOperationException($"Handler not found for {request.GetType().Name}");
            
            return await (Task<TResponse>)handlerType
                            .GetMethod("Handle")!
                            .Invoke(handlerInstance, new object[] { request, cancellation });
        }
    }
}
