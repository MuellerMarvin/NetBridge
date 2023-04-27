using System;

using NetBridge.Networking.Core;
using NetBridge.Networking.Models;
using NetBridge.Networking.Models.Configuration;
using NetBridge.Logging;

using ExampleServer.SharedModels;

namespace ExampleClient
{
    internal class ExampleClient
    {
        static void Main()
        {
            // Create a cofniguration for your client.
            ClientConfig clientConfig = new()
            {
                ServerIP = System.Net.IPAddress.Parse("127.0.0.1"),
                ServerPort = 1300,
                WriteLogsToConsole = true
            };

            // Create a logger for your client, if you want to.
            Logger logger = new(new LogConfig()
            {
                ConsoleLogLevel = LogLevel.Debug,
                FileLogLevel = LogLevel.Debug,
                LogFilePath = "client.log"
            });

            // Restart the client if the server disconnects.
            while (true)
            {
                    // Create a new client with the configuration.
                    Client client = new(clientConfig, logger)
                    {
                        // Decide what to do when a task is received.
                        TaskHandler = DoTask<CalculatorTask>
                    };

                    // Start the client.
                    Task clientTask = Task.Run(client.Run);
                    clientTask.Wait();
            }
        }

        static object DoTask (CalculatorTask task)
        {
            CalculatorTask calcTask = task as CalculatorTask;
            int result = 0;
            switch (calcTask.Operation)
            {
                case CalculatorOperation.Add:
                    result = calcTask.Operands.Sum();
                    break;
                case CalculatorOperation.Subtract:
                    result = calcTask.Operands[0] - calcTask.Operands[1];
                    break;
                case CalculatorOperation.Multiply:
                    result = calcTask.Operands.Aggregate((a, b) => a * b);
                    break;
                case CalculatorOperation.Divide:
                    result = calcTask.Operands[0] / calcTask.Operands[1];
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}