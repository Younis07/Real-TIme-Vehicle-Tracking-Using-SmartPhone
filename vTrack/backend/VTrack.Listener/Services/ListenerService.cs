using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VTrack.Library.DI;
using VTrack.Listener.Server;

namespace VTrack.Listener.Services;

[Service(typeof(IListenerService))]
public class ListenerService : IListenerService
{
    private readonly IEnumerable<IProtocol> protocols;
    private readonly IProtocolHandler protocolHandler;

    public ListenerService(IEnumerable<IProtocol> protocols, IProtocolHandler protocolHandler)
    {
        this.protocols = protocols.OrderBy(x => x.Port);
        this.protocolHandler = protocolHandler;
    }

    [SuppressMessage("ReSharper", "AssignmentIsFullyDiscarded")]
    public async Task Execute(CancellationToken cancellationToken)
    {
        foreach (IProtocol protocol in protocols)
        {
            _ = protocolHandler.HandleProtocol(cancellationToken, protocol);
        }

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}