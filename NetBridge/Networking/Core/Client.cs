using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using NetBridge.Networking.Models;
using NetBridge.Networking.Models.Configuration;
using NetBridge.Networking.Serialization;
using NetBridge.Logging;
using System.Runtime.CompilerServices;

namespace NetBridge.Networking.Core
{
    public class Client
    {
        public Logger Logger { get; set; }

        /// <summary>
        /// The underlying TCP client.
        /// </summary>
        public TcpClient TcpClient { get; private set; }
        public ClientConfig Config { get; private set; }

        /// <summary>
        /// Gets executed every time  a task is received from the server, and returns the result to be sent back to the server.
        /// </summary>
        public Func<NetworkTask<T>, object> TaskHandler { get; set; }

        public Type TaskType { get; private set; }

        /// <summary>
        /// Create a new client instance with no logging.
        /// </summary>
        /// <param name="config"></param>
        public Client(ClientConfig config, Type taskType)
        {
            this.TcpClient = new TcpClient();
            this.Config = config;
            this.TaskType = taskType;

            this.Logger = new Logger(new LogConfig()
            {
                ConsoleLogLevel = LogLevel.None,
                FileLogLevel = LogLevel.None
            });
        }

        /// <summary>
        /// Create a new client instance with logging.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public Client(ClientConfig config, Type taskType, Logger logger)
        {
            this.TcpClient = new TcpClient();
            this.Config = config;
            this.Logger = logger;
            this.TaskType = taskType;
        }

        public async Task Run()
        {
            try
            {
                this.Logger.Log("Attempting to connect.", LogLevel.Info);
                this.TcpClient.Connect(Config.ServerIP, Config.ServerPort);
            }
            catch
            {
                    this.Logger.Warning("No connection could be made.");
            }

            if (this.TcpClient.Connected)
            {
                this.Logger.Log("Connected to server.", LogLevel.Info);
            }
            else { return; }

            // Complete tasks... forever.
            while (true)
            {
                // Receive task from server.
                NetworkTask task = ReceiveTask(); // Receive Task from server
                Logger.Log("Received task. - " + task.Guid, LogLevel.Info);

                // Do work.
                object result = TaskHandler(task);

                // Send result back to server.
                UtilityFunctions.SendObject(this.TcpClient.GetStream(), new ResultContainer(result));
                Logger.Log("Sent result.", LogLevel.Info);
            }
        }

        private NetworkTask ReceiveTask()
        {
            NetworkTask task = UtilityFunctions.ReceiveObject<NetworkTask>(TcpClient.GetStream());
            return task;
        }
    }
}
