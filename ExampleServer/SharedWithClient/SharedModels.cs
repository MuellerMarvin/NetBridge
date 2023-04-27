using System;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using NetBridge.Networking.Models;

namespace ExampleServer.SharedModels
{
    [Serializable]
    public enum CalculatorOperation
    {
        Add = 1,
        Subtract = 2,
        Multiply = 3,
        Divide = 4
    }

    [Serializable]
    public class CalculatorTask
    {
        public CalculatorOperation Operation { get; set; }
        public int[] Operands { get; set; } = Array.Empty<int>();


        [JsonConstructor]
        public CalculatorTask(CalculatorOperation operation, int[] operands)
        {
            Operation = operation;
            Operands = operands;
        }
    }
}