using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookBL.Model
{
    [Serializable]
    public class Value
    {

        /// <summary>
        /// Unit - unit of measure
        /// </summary>
        
        public string Name { get; }

        public double Amount { get; set; }

        public Value(string name, double amount = 0)
        {
            // В виде имени мы передаем готовую строку с файла ресурсов 

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("The name of Unit can`t be null or white space", nameof(name));
            }
            if (amount < 0)
            {
                throw new ArgumentNullException("The amount can`t be less than 0", nameof(amount));
            }

            Name = name;
            Amount = amount;

        }

        public override string ToString()
        {
            return $"{Amount} {Name}.";
        }



    }
}
