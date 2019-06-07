using Battlefield_BitStream_Common.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_2_BitStream.Processors
{
    public class ConFileProcessor : IConFileProcessor//this is a big fat headache
    {
        private Dictionary<string, Func<object, int>> registeredMethods { get; set; }
        public ConFileProcessor()
        {
            registeredMethods = new Dictionary<string, Func<object, int>>();
        }
        public void ExecuteConFile(string file)
        {
            var commands = File.ReadAllLines(file);
            foreach(var command in commands)
            {
                var data = command.Split(' ');
                if (!registeredMethods.ContainsKey(data[0]))
                {
                    Console.WriteLine("[CONPROCESSOR] Unknown con function: " + command);
                    continue;
                }
                var variable1 = data[1];
                if (variable1.StartsWith(Convert.ToString('"')))
                {
                    for(int i = 2; i < data.Length; i++)
                    {
                        variable1 += data[i];
                        if (data[i].EndsWith(Convert.ToString('"')))
                            break;
                    }
                }
                registeredMethods[data[0]](variable1);
            }
        }

        public void RegisterConMethod(string name, Func<object, int> method)
        {
            if (registeredMethods.ContainsKey(name))
                return;
            registeredMethods.Add(name, method);
        }
    }
}