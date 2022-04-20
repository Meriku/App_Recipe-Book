using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using RecipeBookBL.Controller;

namespace RecipeBook
{
    internal class Program // View
    {

        static List<string> menuItems = new List<string>();
        static ProductController productController;
        static RecipeController recipeController;
        static CultureInfo culture;
        static ResourceManager resourceManager = new ResourceManager("RecipeBook.Languages.Messages", typeof(Program).Assembly);

        static void Main(string[] args)
        {

            /* Программа - книга рецептов 
             * Возможности: 
             * Локализация                                  - реализовано (в том числе загрузка различных сериализованных файлов в зависимости от языка)
             * Подбор подходящего рецепта по ингредиентам   - реализовано (поиск по продуктам в наличии)
             * Возможность добавить свой рецепт             - реализовано (в том числе добавление продукта которого нет в базе)
             * Сериализация рецептов (сохранение в файл)    - реализовано
             */

            ChooseLanguage();

            productController = new ProductController(culture);
            recipeController = new RecipeController(culture);

            while (true)
            {
                MainMenu();
            }


        }


        private static void ChooseLanguage()
        {
            Console.Clear();
            menuItems.Clear();
            menuItems.Add("Choose your language:");
            menuItems.Add("");
            menuItems.Add("English");
            menuItems.Add("Russian");
            menuItems.Add("\nExit");


            switch (UserChoice())
            {
                case 2:
                    culture = CultureInfo.CreateSpecificCulture("en-US");
                    break;
                case 3:
                    culture = CultureInfo.CreateSpecificCulture("ru-RU");
                    break;
                case 4:
                    Console.WriteLine(Languages.Messages.EnterYorN);
                    if (AskYesOrNo())
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        ChooseLanguage();
                    }
                    break;

            }

        }



        private static void MainMenu()
        {
            Console.Clear();
            menuItems.Clear();
            menuItems.Add(resourceManager.GetString("MainMenuHeader", culture));
            menuItems.Add("\n");
            menuItems.Add(resourceManager.GetString("FirstMainMenuItem", culture));     // Найти рецепт
            menuItems.Add(resourceManager.GetString("SecondMainMenuItem", culture));    // Добавить рецепт
            menuItems.Add("\n" + resourceManager.GetString("Exit", culture));


            switch (UserChoice())
            {
                case 2:
                    MenuFindRecipe();
                    break;
                case 3:
                    MenuCreateRecipe();
                    break;
                case 4:
                    Console.WriteLine(resourceManager.GetString("EnterYorN", culture));
                    if (AskYesOrNo())
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        MainMenu();
                    }
                    break;

            }

        }

   
        private static void MenuFindRecipe()
        {
            Console.Clear();
            menuItems.Clear();
            menuItems.Add(resourceManager.GetString("SecondMenuHeader", culture));
            menuItems.Add("\n");
            menuItems.Add(resourceManager.GetString("FirstSecondaryMenuItem", culture)); // Найти по названию
            menuItems.Add(resourceManager.GetString("SecondSecondaryMenuItem", culture));// Найти по составным продуктам
            menuItems.Add("\n" + resourceManager.GetString("GoBack", culture));



            switch (UserChoice())
            {
                case 2:
                    MenuFindRecipeByName();
                    break;
                case 3:
                    MenuFindRecipeByProduct();
                    break;
                case 4:
                    MainMenu();
                    break;

            }
        }
        private static void MenuFindRecipeByName()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(resourceManager.GetString("Enter recipe name", culture));
                var name = ParseString();

                var result = recipeController.FindByName(name);
          
                if (result.Length <= 7 && result.Length > 0)
                {
                    // Выбираем из 7 вариантов нужный
                    ChooseRecipe(result);
                    break;
                }
                else
                {
                    // Вводим более точное название если вариантов 0, или больше 7
                    Console.WriteLine(resourceManager.GetString("Found Recipes Count", culture) + result.Length);
                    Console.WriteLine(resourceManager.GetString("More Precise name is required", culture));
                    Console.ReadKey();
                }

                
            }
      

        }

        private static void ChooseRecipe(int[] indexes)
        {
            Console.Clear();
            menuItems.Clear();
            menuItems.Add(resourceManager.GetString("Found Recipes Count", culture) + indexes.Length);
            menuItems.Add(resourceManager.GetString("ChooseRecipe", culture) +  "\n");

            foreach (var item in indexes)
            {
                menuItems.Add(recipeController.GetRecipeByIndex(item));
            }

            menuItems.Add("\n" + resourceManager.GetString("GoBack", culture));


            var answer = UserChoice(3);
            if (answer == menuItems.Count-1) // Назад
            {
                MenuFindRecipeByName();
            }
            else
            {
                Console.Clear();
                Console.WriteLine(recipeController.GetRecipeByIndex(answer, shorted: false));
                Console.ReadKey();
            }
         
        }

        private static void MenuFindRecipeByProduct()
        {
            var productIndexes = new List<int>();

            while (true)
            {
                Console.Clear();
                Console.WriteLine(resourceManager.GetString("Enter product name", culture));
                var name = ParseString();

                var result = productController.FindByName(name);

                if (result.Length <= 7 && result.Length > 0)
                {
                    // Выбираем из 7 вариантов нужный
                    var productIndex = ChooseProduct(result);

                    if (productIndexes.Count > 0 && productIndexes.Contains(productIndex))
                    {
                        Console.WriteLine(productController.GetProductName(productIndex) + resourceManager.GetString("ProductAlreadyAddedToSearch", culture));
                        Console.ReadKey();
                        continue; // Возврат в начало цикла
                    }

                    if (productIndex != 0)
                    {
                        productIndexes.Add(productIndex);
                        Console.WriteLine(productController.GetProductName(productIndex) + " " + resourceManager.GetString("AddProductForSearch", culture));
                    }
                    else
                    {
                        break;
                    }


                    Console.WriteLine(resourceManager.GetString("AddNextProductOrExit", culture));
                    if (!AskYesOrNo())
                    {   // Если true  - продолжаем работу, если false - переходим к поиску рецепта.

                        var productNames = productController.GetProductName(productIndexes.ToArray());
                        var recipeIndexes = recipeController.FindByProduct(productNames);

                        if (recipeIndexes.Length > 0)
                        {
                            var recipe = ChooseRecipeFromFounded(recipeIndexes);
                            Console.WriteLine(resourceManager.GetString("Enjoy your meal", culture) + "\n");
                            Console.WriteLine(recipeController.GetRecipeByIndex(recipe, false));
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(resourceManager.GetString("SuchRecipeNotExist", culture) + "\n");

                            foreach (var names in productNames)
                            {
                                Console.WriteLine(names);
                            }

                            Console.WriteLine("\n" + resourceManager.GetString("TryAgain", culture));

                            Console.ReadKey();
                        }
                        
                    

                        break;
                    }
                }
                else
                {
                    // Вводим более точное название если вариантов 0, или больше 7
                    Console.WriteLine(resourceManager.GetString("Found Products Count", culture) + " " + result.Length);
                    Console.WriteLine(resourceManager.GetString("More Precise name is required", culture));
                    Console.ReadKey();
                }

            }
        }


        private static int ChooseRecipeFromFounded(int[] recipeIndexes)
        {
            Console.Clear();
            menuItems.Clear();
            menuItems.Add(resourceManager.GetString("FoundRecipeCount", culture) + " " + recipeIndexes.Length);
            menuItems.Add(resourceManager.GetString("ChooseRecipe", culture) + "\n");

            foreach (var recipeIndex in recipeIndexes)
            {
                menuItems.Add(recipeController.GetRecipeByIndex(recipeIndex));
            }

            menuItems.Add("\n" + resourceManager.GetString("GoBack", culture));


            var answer = UserChoice(3);
            if (answer == menuItems.Count - 1) // Назад
            {
                MenuFindRecipeByProduct();
            }
            else
            {
                Console.Clear();
                if (answer - 2 >= 0)
                {
                    return recipeIndexes[answer - 2];
                }

            }

            return 0;

        }

        private static int ChooseProduct(int[] indexes)
        {
            Console.Clear();
            menuItems.Clear();
            menuItems.Add(resourceManager.GetString("Found Products Count", culture) + " " + indexes.Length);
            menuItems.Add(resourceManager.GetString("ChooseProduct", culture) + "\n");

            foreach (var item in indexes)
            {
                menuItems.Add(productController.GetProductName(item));
            }

            menuItems.Add("\n" + resourceManager.GetString("GoBack", culture));


            var answer = UserChoice(3);
            if (answer == menuItems.Count - 1) // Назад
            {
                MenuFindRecipeByProduct();
            }
            else
            {
                Console.Clear();
                if (answer - 2 >= 0)
                {
                    return indexes[answer - 2];
                }
               
            }

            return 0;

        }

        private static void MenuCreateRecipe()
        {
            Console.Clear();
            Console.WriteLine(resourceManager.GetString("EnterNameofNewRecipe", culture));
            var recipename = ParseString();
            recipeController.AddToRecipe(name: recipename, null);

            while (true)
            {         
            Console.Clear();
            menuItems.Clear();
            menuItems.Add(resourceManager.GetString("ChooseProductForRecipe", culture));
            menuItems.Add("\n");
            menuItems.Add(resourceManager.GetString("NewProduct", culture));

                for (int i = 0; i < productController.ProdutsCount; i++)
                {
                menuItems.Add(productController.GetProductName(index: i));
                }

            var answer = UserChoice();

            switch (answer)
                {
                    case 2:
                        Console.Clear();
                        Console.WriteLine(resourceManager.GetString("EnterNameofNewProduct", culture));
                        var name = ParseString();
                        if (productController.IsExist(name))
                        {
                            Console.WriteLine(resourceManager.GetString("ProductAlreadyExist", culture));
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            Console.Clear();
                            var units = AskUnitsForNewProduct();

                            Console.Clear();
                            Console.WriteLine($"{resourceManager.GetString("CountOfProductNeaded", culture)} { units } ");
                            double productvalue = ParseDouble();

                            productController.CreateProduct(name, units, productvalue);
                            recipeController.AddToRecipe(productController.CurrentProduct);

                            Console.WriteLine($"{name} {resourceManager.GetString("AddNewProductToRecipe", culture)}");

                        }
                        break;
                    default:

                        productController.SetProduct(index: answer - 3);

                        Console.Clear();
                        Console.WriteLine($"{resourceManager.GetString("CountOfProductNeaded", culture)} { productController.GetProductUnits(answer - 3) } ");
                        double newProductValue = ParseDouble();

                        productController.SetProductValue(newProductValue);


                        recipeController.AddToRecipe(productController.CurrentProduct);

                        Console.WriteLine($"{productController.GetProductName(answer - 3)} {resourceManager.GetString("AddProductToRecipe", culture)}");
                        break;

                }

                Console.WriteLine(resourceManager.GetString("AddTheNextProductOrExit", culture));
                if (!AskYesOrNo())
                {
                    break;
                }

            }


            Console.Clear();
            Console.WriteLine(resourceManager.GetString("EnterRecipeDescription", culture));

            var description = Console.ReadLine();
            recipeController.AddToRecipe(null, description);
            recipeController.SaveRecipe();

            Console.Clear();
            Console.WriteLine(resourceManager.GetString("RecipeWasAddedtoDB", culture));

            Console.WriteLine(recipeController.CurrentRecipe);


            Console.ReadLine();

        }

        private static string AskUnitsForNewProduct()
        {
            Console.Clear();
            menuItems.Clear();
            menuItems.Add(resourceManager.GetString("ChooseUnitsForProduct", culture));
            menuItems.Add("\n");
            menuItems.Add(resourceManager.GetString("Item", culture));
            menuItems.Add(resourceManager.GetString("Kilogram", culture));
            menuItems.Add(resourceManager.GetString("Gram", culture));
            menuItems.Add(resourceManager.GetString("Liter", culture));
            menuItems.Add(resourceManager.GetString("Milliliter", culture));
            menuItems.Add(resourceManager.GetString("Tablespoon", culture));        

            switch (UserChoice())
            {
                case 2:
                    return resourceManager.GetString("Item", culture);

                case 3:
                    return resourceManager.GetString("Kilogram", culture);
                  
                case 4:
                    return resourceManager.GetString("Gram", culture);
                
                case 5:
                    return resourceManager.GetString("Liter", culture);

                case 6:
                    return resourceManager.GetString("Milliliter", culture);

                case 7:
                    return resourceManager.GetString("Tablespoon", culture);

            }

            return "";

        }

        private static int UserChoice(int index = 2)
        {
            while (true)
            {
                DrawMenu(index);
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.DownArrow:
                        if (index < menuItems.Count - 1)
                        {
                            index++;
                        }
                        break;

                    case ConsoleKey.UpArrow:
                        if (index > 2)
                        {
                            index--;
                        }
                        break;

                    case ConsoleKey.Enter:
                        return index;
                }
            }
        }

        private static void DrawMenu(int index = 2)
        {
            Console.Clear();
            int row = Console.CursorTop;
            int col = Console.CursorLeft;
            Console.SetCursorPosition(col, row);
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == index)
                {
                    Console.BackgroundColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.WriteLine(menuItems[i]);
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        public static bool AskYesOrNo()
        {
            while (true)
            {             
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Y)
                {
                    return true;
                }
                if (key == ConsoleKey.N)
                {
                    return false;
                }
         
            }
        }

        public static double ParseDouble(double min = 0, double max = 1000)
        {
            double result;

            while (!double.TryParse(Console.ReadLine(), out result) || result <= min || result > max)
            {
                Console.WriteLine($"Min {min} Max {max}");

            }

            return result;
        }

        public static string ParseString()
        {
            string result = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(result))
            {
                Console.WriteLine($"Answer can`t be empty");
                result = Console.ReadLine();

            }

            return result;
        }


    }
}
