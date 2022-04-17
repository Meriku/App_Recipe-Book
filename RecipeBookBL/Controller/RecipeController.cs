using RecipeBookBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookBL.Controller
{
    public class RecipeController : BaseController
    {
        public Recipe CurrentRecipe { get; private set; }

        private string Name;

        private string Description;

        private List<Product> RecipeProductList;

        private List<Recipe> Recipes; 

        public RecipeController()
        {

            Recipes = Load<Recipe>();
            RecipeProductList = new List<Product>();

        }


        public void AddToRecipe(string name, string description)
        {
            if (name != null)
            {
                Name = name;
            }
            if (description != null)
            {
                Description = description;
            }
        }

        public void AddToRecipe(Product product)
        {
            if (product != null)
            {
                RecipeProductList.Add(product);
            }
        }


        public void SaveRecipe()
        {
            if (Name != null && Description != null && RecipeProductList.Count > 0)
            {
                CurrentRecipe = new Recipe(Name, Description, RecipeProductList);
                Recipes.Add(CurrentRecipe);
                Save<Recipe>(Recipes);
            }
            else
            {
                throw new Exception("Can`t save recipe. Invalid arguments.", new SystemException(nameof(Recipe)));
            }


          
        }


        public int[] FindByName(string name)
        {
            var indexes = new List<int>();

            var count = 0;

            foreach (var item in Recipes)
            {
                if (item.Name != null && item.Name.Contains(name))
                {
                    count++;
                    indexes.Add(Recipes.FindIndex(x => x.Name == item.Name));

                }
            }
          
            return indexes.ToArray();
        }

        public void FindByProduct()
        {

        }

        public string GetRecipeByIndex(int index, bool shorted = true)
        {
            if (shorted)
            {
                return Recipes[index].Name;
            }
            else
            {
                return Recipes[index].ToString();
            }
            
        }




    }
}
