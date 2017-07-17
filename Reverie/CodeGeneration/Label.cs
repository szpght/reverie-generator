﻿namespace Reverie.CodeGeneration
{
    public class Label
    {
        public string Name { get; }

        public Label(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}