using System;
using System.Net.Sockets;

using NetBridge.Logging;
using NetBridge.Networking.Models;
using NetBridge.Networking.Models.Events;
using NetBridge.Networking.Models.Configuration;

namespace NetBridge.Networking.Core
{
    public class Server<PayloadType, ResultType>
    {
        public Logger Logger { get; set; }

        /// <summary>
        /// The underlying TCP listener.
        /// </summary>
        public TcpListener TcpListener { get; private set; }
        public ServerConfig Config { get; private set; }

        private readonly List<ClientStore> Clients = new();

        private readonly Queue<NetworkTask<PayloadType>> TaskQueue = new();

        private Dictionary<Guid, ResultType> TaskResults = new();


        #region Event Handlers
        public event EventHandler<TaskCompleteEvent> OnTaskComplete;
        #endregion


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

        private ResultType WaitForTaskCompletion(Guid guid)
        {
            // Create a TaskCompletionSource and store it locally.
            var tcs = new TaskCompletionSource<ResultType>();

            // Define the event handler.
            EventHandler<TaskCompleteEvent> myEventHandler = null;
            myEventHandler = (sender, args) =>
            {
                if (args.TaskId == guid)
                {
                    // Unsubscribe from the event.
                    OnTaskComplete -= myEventHandler;

                    // Set the result of the TaskCompletionSource to the result of the task.
                    tcs.TrySetResult((ResultType)args.Result);
                }
            };

            // Subscribe to the event.
            OnTaskComplete += myEventHandler;

            // Wait for the event to be fired.
            var result = tcs.Task.Result;

            // Return the result of the task.
            return result;
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
            while (true)
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
                    await ClientCompleteTask(clientStore, netTask);
                    clientStore.IsBusy = false;
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
        public async Task<ResultType> DoTask(NetworkTask<PayloadType> task)
        {
            // Hand out a new GUUID
            task.Guid = Guid.NewGuid();

            // Queue the task.
            TaskQueue.Enqueue(task);

            // Wait for the task to complete.
            ResultType result = WaitForTaskCompletion(task.Guid);

            return result;
        }

        /// <summary>
        /// Executes a task on the provided client and returns the result.
        /// </summary>
        /// <param name="clientStore"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        private async Task ClientCompleteTask(ClientStore clientStore, NetworkTask<PayloadType> task)
        {
            // Send the task to the client.
            NetworkStream networkStream = clientStore.TcpClient.GetStream();
            UtilityFunctions.SendObject(networkStream, task);
            Logger.Info("A task has been sent for execution. - " + task.Guid.ToString());

            // Wait for the result.
            ResultContainer<ResultType> resultContainer = UtilityFunctions.ReceiveObject<ResultContainer<ResultType>>(networkStream);

            // Return the result.
            RaiseOnTaskComplete(task.Guid, resultContainer.ResultPayload);

            Logger.Info("Received result from client.");
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

        protected virtual void RaiseOnTaskComplete(Guid taskId, object result)
        {
            OnTaskComplete?.Invoke(this, new TaskCompleteEvent { TaskId = taskId, Result = result });
        }
    }
}
