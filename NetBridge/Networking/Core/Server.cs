using System;
using System.Net.Sockets;

using NetBridge.Logging;
using NetBridge.Networking.Models;
using NetBridge.Networking.Models.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace NetBridge.Networking.Core
{
    internal class TaskCompleteEvent : EventArgs
    {
        public Guid TaskId { get; set; }
        [AllowNull]
        public object Result { get; set; }
    }

    public class Server<PayloadType>
    {
        public Logger Logger { get; set; }

        /// <summary>
        /// The underlying TCP listener.
        /// </summary>
        public TcpListener TcpListener { get; private set; }
        public ServerConfig Config { get; private set; }

        private readonly List<ClientStore> Clients = new();

        private readonly Queue<NetworkTask<PayloadType>> TaskQueue = new();



        /// <summary>
        /// Create a new server instance with no logging.
        /// </summary>
        /// <param name="config"></param>
        public Server(ServerConfig config)
        {
            TcpListener = new TcpListener(Config.ServerIP, Config.ServerPort);
            this.Config = config;

            this.Logger = new Logger(new LogConfig()
            {
                ConsoleLogLevel = LogLevel.None,
                FileLogLevel = LogLevel.None
            });
        }

        /// <summary>
        /// Create a new server instance with logging.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public Server(ServerConfig config, Logger logger)
        {
            this.Config = config;
            TcpListener = new TcpListener(Config.ServerIP, Config.ServerPort);

            this.Logger = logger;
        }

        public async Task Run()
        {
            // Clear the client list.
            Clients.Clear();

            // Start the TCP listener.
            this.TcpListener.Start();

            // Start the tasks.
            Task acceptClientsTask = Task.Run(AcceptNewClientsAsync);
            Task removeStaleConnectionsTask = Task.Run(RemoveStaleConnectionsAsync);

            Logger.Info("Server is running.");

            // Complete tasks... forever.
            while(true)
            {
                // If there are no tasks in the queue or no clients connected, wait.
                if (TaskQueue.Count == 0 || Clients.Count == 0)
                {
                    await Task.Delay(1000);
                    continue;
                }

                // Get the first task in the queue.
                NetworkTask<PayloadType> netTask = TaskQueue.Dequeue();

                ClientStore clientStore = FindAvailableClient(this.Clients);
                clientStore.IsBusy = true;

                Task task = Task.Run(async () =>
                {
                    object result = await ExecuteTask(clientStore, netTask);
                    clientStore.IsBusy = false;

                    return new object();
                });
            }
        }

        /// <summary>
        /// Queues, waits for completion, and returns the result of a task.
        /// This is the main method for interacting with the server.
        /// It makes executing tasks remotely in parallel very similar to executing them locally.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<object> DoTask(NetworkTask<PayloadType> task)
        {
            // Hand out a GUUID
            task.Guid = Guid.NewGuid();

            // Queue the task.
            TaskQueue.Enqueue(task);
            // Wait for the task to complete.
            while (true)
            {
                // TODO: Add a trigger to continue.
            }
        }

        /// <summary>
        /// Executes a task on the provided client and returns the result.
        /// </summary>
        /// <param name="clientStore"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        private async Task<object> ExecuteTask(ClientStore clientStore, NetworkTask<PayloadType> task)
        {
            // Set the client as busy.
            clientStore.IsBusy = true;

            // Send the task to the client.
            NetworkStream networkStream = clientStore.TcpClient.GetStream();
            UtilityFunctions.SendObject(networkStream, task);
            Logger.Info("A task has been sent for execution. - " + task.Guid.ToString());

            // Wait for the result.
            ResultContainer resultContainer = UtilityFunctions.ReceiveObject<ResultContainer>(networkStream);
            Logger.Info("Received result from client.");

            return resultContainer.Result;
        }

        private async Task RemoveStaleConnectionsAsync()
        {
            while (true)
            {
                // Remove stale connections.
                for (int i = 0; i < Clients.Count; i++)
                {
                    if (!Clients[i].TcpClient.Connected)
                    {
                        Logger.Log("A client has disconnected. - " + Clients[i].Identity.GUID.ToString(), LogLevel.Info);
                        Clients.RemoveAt(i);
                    }
                }
                await Task.Delay(10000); // Wait for 10 seconds before re-checking the condition
            }
        }

        /// <summary>
        /// Accepts new clients continiously.
        /// </summary>
        /// <returns></returns>
        private async Task AcceptNewClientsAsync()
        {
            while (true)
            {
                // If the maximum amount of connections has been reached, do not accept new connections.
                if (Clients.Count >= this.Config.MaxConnections)
                {
                    await Task.Delay(1000); // Wait for a second before re-checking the condition
                    continue;
                }

                // Accept the client.
                TcpClient client = await this.TcpListener.AcceptTcpClientAsync();
                ClientIdentity identity = new(Guid.NewGuid());

                // Add the client to the list of clients.
                Clients.Add(new ClientStore(identity, client, false));

                Logger.Log("A new client has connected. - " + identity.GUID.ToString(), LogLevel.Info);
            }
        }

        private static ClientStore? FindAvailableClient(List<ClientStore> clients)
        {
            foreach (ClientStore client in clients)
            {
                if (!client.IsBusy)
                {
                    return client;
                }
            }
            return null;
        }

    }
}
