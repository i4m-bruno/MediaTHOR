using System.Threading;
using System.Threading.Tasks;

namespace MediaTHOR.Interfaces
{
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellation = default);

        Task Publish<TNotification>(TNotification notification, CancellationToken cancellation = default) where TNotification : INotification;
    }
}
