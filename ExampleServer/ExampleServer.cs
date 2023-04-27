using System;

using NetBridge.Networking.Core;
using NetBridge.Networking.Models;
using NetBridge.Networking.Models.Configuration;
using NetBridge.Logging;

using ExampleServer.SharedModels;

namespace ExampleServer
{
    internal class ExampleServer
    {
        static void Main()
        {
            // Create a configuration for your server.
            ServerConfig serverConfig = new()
            {
                ServerIP = System.Net.IPAddress.Any,
                ServerPort = 1300,
                BufferSize = 1024,
                ConnectionTimeout = 10000,
                MaxConnections = 100,
                WriteLogsToConsole = true
            };

            // Create a logger for your server, if you want to.
            Logger logger = new(new LogConfig()
            {
                ConsoleLogLevel = LogLevel.Debug,
                FileLogLevel = LogLevel.Debug,
                LogFilePath = "server.log"
            });


            // Create a new server with the configuration.
            Server<CalculatorTask, int> server = new(serverConfig, logger);

            // Start the server.
            Task serverTask = Task.Run(server.Run);

            NetworkTask<CalculatorTask> netTask = new NetworkTask<CalculatorTask>(Guid.NewGuid(), new CalculatorTask(CalculatorOperation.Add, new int[] { 1, 2, 3 }));

            Object resultObj = server.DoTask(netTask);
            int result = (int)resultObj;

            Console.WriteLine("Result: {0}", result);

            serverTask.Wait();
        }
    }
}