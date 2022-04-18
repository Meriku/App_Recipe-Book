using RecipeBookBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RecipeBookBL.Controller
{
    public class ProductController : BaseController
    {

        public Product CurrentProduct { get; private set; }

        private List<Product> ExistingProducts = new List<Product>(); 

        public int ProdutsCount { get { return ExistingProducts.Count; } }


        public ProductController()
        {
            ExistingProducts = Load<Product>();
        }

        public bool IsExist(string name)
        {
            if (ExistingProducts.Count > 0)
            {
                var product = ExistingProducts.SingleOrDefault(x => x.Name == name);

                if (product != null)
                {
                    return true;
                }

            }
  
            return false;
            
        }

        public void CreateProduct(string name, string unitsofmeasure, double value)
        {
            CurrentProduct = new Product(name, new Value(unitsofmeasure, value));
            ExistingProducts.Add(CurrentProduct);
            Save<Product>(ExistingProducts);
        }

        public int[] FindByName(string name)
        {
            var indexes = new List<int>();

            for (int i = 0; i < ExistingProducts.Count; i++)
            {
                if (ExistingProducts[i].Name != null && ExistingProducts[i].Name.ToLower().Contains(name.ToLower()))
                {
                    indexes.Add(i);
                }
            }

            return indexes.ToArray();
        }


        public void SetProduct(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            CurrentProduct = ExistingProducts.SingleOrDefault(x => x.Name == name);

        }

        public void SetProduct(int index)
        {
            if (index < 0 || index > ExistingProducts.Count - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            CurrentProduct = ExistingProducts[index];
        }

        public void SetProductValue(double value)
        {

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            CurrentProduct.Value.Amount = value;

        }

        public string GetProductName(int index)
        {
            return ExistingProducts[index].ToString();
        }
        public string GetProductUnits(int index)
        {
            return ExistingProducts[index].Value.Name;
        }
        public string[] GetProductName(int[] indexes)
        {
            var names = new string[indexes.Length];   

            for (int i = 0; i < names.Length; i++)
            {
                names[i] = ExistingProducts[indexes[i]].ToString();
            }
 
            return names;
        }



    }






}
