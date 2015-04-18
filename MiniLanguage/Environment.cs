using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class Environment
    {
        public Environment Parent;
        Dictionary<String, int> Locations; // includes references as well.
        HashSet<String> References; 

        public Environment()
        {
            Locations = new Dictionary<string, int>();
            References = new HashSet<string>();
            Parent = null;
        }

        public bool IsGlobal()
        {
            return Parent == null;
        }
        public int GetLocation(String identifier)
        {
            int location;
            if (Locations.TryGetValue(identifier, out location))
                return location;
            else if(Parent != null)
                return Parent.GetLocation(identifier);
            else 
                throw new Exception("Identifier not found");
        }

        public bool SetLocation(String identifier, int location)
        {
            if (Locations.ContainsKey(identifier))
            {
                Locations[identifier] = location;
                return true;
            }
            else if (Parent != null)
            {
                if (!Parent.SetLocation(identifier, location))
                {
                    throw new Exception("Identifier not found");
                }
            }
            else
                throw new Exception("Identifier not found");

            return false;
        }

        public void AddReference(String reference)
        {
            References.Add(reference);
        }
        public bool ContainsReference(String reference) 
        { 
            return References.Contains(reference);
        }
        public void AddLocation(String identifier, int location)
        {
            Locations.Add(identifier, location);
        }

    }
}
