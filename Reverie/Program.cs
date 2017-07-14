using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Reverie
{
    public class Function
    {
        private readonly dynamic ast_;
        private readonly Context mainContext_;

        public Function(dynamic ast)
        {
            ast_ = ast;
            mainContext_ = new Context();
        }

        public void Compile()
        {
            InitArguments();
            Console.WriteLine(ast_.Name);
        }

        private void InitArguments()
        {
            var astParameters = ast_["Parameters"] as IEnumerable<dynamic>;
            foreach (var parameter in astParameters)
            {
                var variable = new Variable2(parameter);
                mainContext_.AddVariable(variable);
            }
        }
    }

    public class Variable2
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public Variable2(dynamic ast)
        {
            Name = ast.Name;
            Type = ast.Type;
        }
    }

    public class Context
    {
        private Context parent_;

        public RecursiveDictionary<string, Variable2> Variables { get; set; }
        public Context Parent
        {
            get => parent_;
            set
            {
                parent_ = value;
                Variables.Parent = value.parent_.Variables;
            }
        }

        public void AddVariable(Variable2 variable2)
        {
            Variables.Add(variable2.Name, variable2);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var code = File.ReadAllText("ast.json");
            dynamic ast = JsonConvert.DeserializeObject(code);
            IEnumerable<dynamic> functionsAst = ast["Functions"];
            var functions = functionsAst.Select(x => new Function(x));
            Parallel.ForEach(functions, x => x.Compile());



            Console.WriteLine("Hello World!");
            Console.Read();
        }
    }
}