using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VTrack.Library.DI;
using VTrack.Listener.Models;
using VTrack.Listener.Services;

namespace VTrack.Listener.Server;

[Service(typeof(IClientHandler))]
public class ClientHandler : IClientHandler
{
    private readonly IConnectionService connectionService;
    private readonly IStreamHandler streamHandler;
    private readonly ILogger<ClientHandler> logger;

    public ClientHandler(IConnectionService connectionService, IStreamHandler streamHandler,
        ILogger<ClientHandler> logger)
    {
        this.connectionService = connectionService;
        this.streamHandler = streamHandler;
        this.logger = logger;
    }

    public async Task HandleClient(CancellationToken cancellationToken, Client client)
    {
        string endPoint = client.TcpClient.Client.RemoteEndPoint?.ToString();
        client.DeviceConnection = await connectionService.NewConnection(endPoint, client.Protocol.Port);

        try
        {
            await using INetworkStreamWrapper networkStream = new NetworkStreamWrapper(client.TcpClient.GetStream());

            await streamHandler.HandleStream(cancellationToken, client, networkStream);

            networkStream.Close();
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, $"{client.Protocol}");
        }
        finally
        {
            logger.LogDebug($"{client.Protocol}: disconnected {endPoint}");
                
            client.TcpClient.Close();

            await connectionService.MarkConnectionAsClosed(client.DeviceConnection);
        }
    }
}