using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookBL.Model
{
    [Serializable]
    public class Recipe
    {
        public string Name { get; }

        public string Description { get; }

        private List<Product> _recipe = new List<Product>();



        public Recipe(string name, string description, List<Product> products)
        {
            Name = name;
            Description = description;
            _recipe = products;
        }


        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append($"Название рецепта: {Name}\nИнгредиенты:");
            foreach (var prod in _recipe)
            {
                result.Append("\n" + prod.ToStringWithValue());
            }
            result.Append("\n" + Description);

            return result.ToString();
        }






    }
}
