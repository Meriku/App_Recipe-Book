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


        public bool FindByName(string name)
        {
            var recipe = Recipes.SingleOrDefault(x => x.Name == name);

            if (recipe != null)
            {
                CurrentRecipe = recipe;
                return true;
            }

            return false;


        }

        public void FindByProduct()
        {

        }




    }
}
