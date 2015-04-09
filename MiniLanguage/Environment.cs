using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class Environment
    {
        Environment Parent;
        Dictionary<String, Value> Identifiers;

        public Environment()
        {
            Identifiers = new Dictionary<string, Value>();
        }

        public Value GetVar(String identifier)
        {
            Value value;
            if (Identifiers.TryGetValue(identifier, out value))
                return value;
            else if(Parent != null)
                return Parent.GetVar(identifier);
            else 
                throw new Exception("Identifier not found");
        }

        public bool SetVar(String identifier, Value value)
        {
            if (Identifiers.ContainsKey(identifier))
            {
                Identifiers[identifier] = value;
                return true;
            }
            else if (Parent != null)
            {
                if (!Parent.SetVar(identifier, value))
                {
                    throw new Exception("Identifier not found");
                }
            }
            else
                throw new Exception("Identifier not found");

            return false;
        }

        public void AddVar(String identifier, Value initialValue = null)
        {
            Identifiers.Add(identifier, initialValue);
        }

    }
}
