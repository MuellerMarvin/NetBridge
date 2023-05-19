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
        static Server<CalculatorTask, int> Server;

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
            Server = new(serverConfig, logger);

            // Start the server.
            Task serverTask = Task.Run(Server.Run);

            


            int[] results = GetMultipleResults().Result;

            foreach (int result in results)
            {
                Console.WriteLine("Result: " + result);
            }

            serverTask.Wait();
        }

        static async Task<int[]> GetMultipleResults()
        {
            // Create a list of networked tasks to run.
            NetworkTask<CalculatorTask>[] netTasks = new NetworkTask<CalculatorTask>[] {
                new NetworkTask<CalculatorTask>
                {
                    Payload = new CalculatorTask(CalculatorOperation.Add, new int[] { 1, 2, 3 })
                },
                new NetworkTask<CalculatorTask>
                {
                    Payload = new CalculatorTask(CalculatorOperation.Multiply, new int[] { 1, 2, 3 })
                },
                new NetworkTask<CalculatorTask>
                {
                    Payload = new CalculatorTask(CalculatorOperation.Subtract, new int[] { 1, 2, 3 })
                }
            };

            // Run all tasks in parallel.
            List<Task<int>> tasks = new List<Task<int>>();

            // Add all tasks to the list.
            foreach (NetworkTask<CalculatorTask> netTask in netTasks)
            {
                tasks.Add(Server.DoTask(netTask));
            }

            // Wait for all tasks to finish.
            int[] results = await Task.WhenAll(tasks);

            return results;
        }
    }
}