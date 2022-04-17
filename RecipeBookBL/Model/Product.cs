using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookBL.Model
{
    [Serializable]
    public class Product
    {

        public string Name { get; }

        public Value Value { get; }

        

        public Product(string name, Value value)
        {

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("ИМЯ: " + name);
                throw new ArgumentNullException("The name of Unit can`t be null or white space", nameof(name));
            }
            if (value == null)
            {
                throw new ArgumentNullException("The value can`t be null", nameof(value));
            }

            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
        public string ToStringWithValue()
        {
            return $"{Name} {Value}";
        }

    }
}
