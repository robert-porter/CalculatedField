using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalculatedField
{
    class Symbols
    {
        public Symbols(List<Field> entityFields)
        {
            EntityFields = entityFields;
        }



        public Field GetField(string name)
        {
            return EntityFields.Find(field => field.Name == name);
        }

        public List<Field> EntityFields { get; protected set; }
    }
}
   
