using System;
using System.Text;

namespace Reverie.CodeGeneration
{
    public class CString : Variable
    {
        public override bool Sign => false;
        public override VariableSize Size => VariableSize.Qword;
        public string Content { get; }
        public Label Label { get; }

        public CString(string content)
        {
            Content = content;
            Label = Label.New(LabelType.Global);
        }

        public override void Load(Register register, Assembly assembly)
        {
            assembly.Add($"mov {register}, {Label}");
        }

        public override void Store(Register register, Assembly assembly)
        {
            throw new Exception("CString cannot be stored");
        }

        public void GenerateRepresentation(Assembly assembly)
        {
            var raw = Encoding.UTF8.GetBytes(Content);
            assembly.Add(Label.Declaration);
            foreach (var character in raw)
            {
                assembly.Add($"db {character}");
            }
            assembly.Add("db 0");
        }

        public override string ToString()
        {
            return $"{Label.Name}: {Content}";
        }

        public override bool Equals(object obj)
        {
            return Content.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Content.GetHashCode();
        }
    }
}
