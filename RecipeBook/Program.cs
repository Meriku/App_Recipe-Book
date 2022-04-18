using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBookBL.Controller;

namespace RecipeBook
{
    internal class Program // View
    {

        static List<string> menuItems = new List<string>();
        static ProductController productController = new ProductController();
        static RecipeController recipeController = new RecipeController();


        static void Main(string[] args)
        {

            /* Программа - книга рецептов 
             * Возможности: 
             * Локализация (стоит ли?)
             * Подбор подходящего рецепта по ингредиентам   - реализовано (поиск по продуктам в наличии)
             * Возможность добавить свой рецепт             - реализовано (в том числе добавление продукта которого нет в базе)
             * Сериализация рецептов (сохранение в файл)    - реализовано
             */

            while (true)
            {
                MainMenu();
            }
        }




        private static void MainMenu()
        {
            Console.Clear();
            menuItems.Clear();
            menuItems.Add(Languages.Messages.MainMenuHeader);
            menuItems.Add("\n");
            menuItems.Add(Languages.Messages.FirstMainMenuItem);     // Найти рецепт
            menuItems.Add(Languages.Messages.SecondMainMenuItem);    // Добавить рецепт
            menuItems.Add("\n" + Languages.Messages.Exit);

            

            switch (UserChoice())
            {
                case 2:
                    MenuFindRecipe();
                    break;
                case 3:
                    MenuCreateRecipe();
                    break;
                case 4:
                    Console.WriteLine(Languages.Messages.EnterYorN);
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
            menuItems.Add(Languages.Messages.SecondMenuHeader);
            menuItems.Add("\n");
            menuItems.Add(Languages.Messages.FirstSecondaryMenuItem); // Найти по названию
            menuItems.Add(Languages.Messages.SecondSecondaryMenuItem);// Найти по составным продуктам
            menuItems.Add("\n" + Languages.Messages.GoBack);



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
                Console.WriteLine("Введите название (или часть названия) рецепта:");
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
                    Console.WriteLine($"Найдено рецептов: {result.Length}");
                    Console.WriteLine("Необходимо более точное название рецепта.");
                    Console.ReadKey();
                }

                
            }
      

        }

        private static void ChooseRecipe(int[] indexes)
        {
            Console.Clear();
            menuItems.Clear();
            menuItems.Add($"Найдено рецептов: {indexes.Length}");
            menuItems.Add("Выберите необходимый рецепт\n");

            foreach (var item in indexes)
            {
                menuItems.Add(recipeController.GetRecipeByIndex(item));
            }

            menuItems.Add("\n" + Languages.Messages.GoBack);


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
                Console.WriteLine("Введите название (или часть названия) продукта:");
                var name = ParseString();

                var result = productController.FindByName(name);

                if (result.Length <= 7 && result.Length > 0)
                {
                    // Выбираем из 7 вариантов нужный
                    var productIndex = ChooseProduct(result);

                    if (productIndexes.Count > 0 && productIndexes.Contains(productIndex))
                    {
                        Console.WriteLine($"Продукт {productController.GetProductName(productIndex)} уже был добавлен для поиска. \nПовторите попытку.");
                        Console.ReadKey();
                        continue; // Возврат в начало цикла
                    }

                    if (productIndex != 0)
                    {
                        productIndexes.Add(productIndex);
                        Console.WriteLine($"Продукт {productController.GetProductName(productIndex)} добавлен для поиска.");
                    }
                    else
                    {
                        break;
                    }


                    Console.WriteLine("Что бы добавить в поиск следующий продукт нажмите Y, что бы завершить добавление продуктов для поиска нажмите N");
                    if (!AskYesOrNo())
                    {   // Если true  - продолжаем работу, если false - переходим к поиску рецепта.

                        var productNames = productController.GetProductName(productIndexes.ToArray());
                        var recipeIndexes = recipeController.FindByProduct(productNames);

                        if (recipeIndexes.Length > 0)
                        {
                            var recipe = ChooseRecipeFromFounded(recipeIndexes);
                            Console.WriteLine("Приятного приготовления! \n");
                            Console.WriteLine(recipeController.GetRecipeByIndex(recipe, false));
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Не удалось найти рецепт с данной комбинацией продуктов: \n");

                            foreach (var names in productNames)
                            {
                                Console.WriteLine(names);
                            }

                            Console.WriteLine("\nПопробуйте еще раз.");

                            Console.ReadKey();
                        }
                        
                    

                        break;
                    }
                }
                else
                {
                    // Вводим более точное название если вариантов 0, или больше 7
                    Console.WriteLine($"Найдено продуктов: {result.Length}");
                    Console.WriteLine("Необходимо более точное название продукта.");
                    Console.ReadKey();
                }

            }
        }


        private static int ChooseRecipeFromFounded(int[] recipeIndexes)
        {
            Console.Clear();
            menuItems.Clear();
            menuItems.Add($"Найдено рецептов с данными продуктами: {recipeIndexes.Length}");
            menuItems.Add("Выберите необходимый рецепт\n");

            foreach (var recipeIndex in recipeIndexes)
            {
                menuItems.Add(recipeController.GetRecipeByIndex(recipeIndex));
            }

            menuItems.Add("\n" + Languages.Messages.GoBack);


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
            menuItems.Add($"Найдено продуктов: {indexes.Length}");
            menuItems.Add("Выберите необходимый продукт\n");

            foreach (var item in indexes)
            {
                menuItems.Add(productController.GetProductName(item));
            }

            menuItems.Add("\n" + Languages.Messages.GoBack);


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
            Console.WriteLine("Введите название нового рецепта");
            var recipename = ParseString();
            recipeController.AddToRecipe(name: recipename, null);

            while (true)
            {         
            Console.Clear();
            menuItems.Clear();
            menuItems.Add("Выберите продукт который хотите добавить в рецепт:");
            menuItems.Add("\n");
            menuItems.Add("Новый продукт");

            for (int i = 0; i < productController.ProdutsCount; i++)
            {
                menuItems.Add(productController.GetProductName(index: i));
            }

            var answer = UserChoice();

            switch (answer)
                {
                    case 2:
                        Console.Clear();
                        Console.WriteLine("Введите название нового продукта:");
                        var name = ParseString();
                        if (productController.IsExist(name))
                        {
                            Console.WriteLine("Данный продукт уже существует");
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            Console.Clear();
                            var units = AskUnitsForNewProduct();

                            Console.Clear();
                            Console.WriteLine($"Какое количество продукта необходимо в данном рецепте? Указывать в: { units } ");
                            double productvalue = ParseDouble();

                            productController.CreateProduct(name, units, productvalue);
                            recipeController.AddToRecipe(productController.CurrentProduct);

                            Console.WriteLine($"Вы успешно добавили продукт {name} в программу и в свой рецепт");

                        }
                        break;
                    default:

                        productController.SetProduct(index: answer - 3);

                        Console.Clear();
                        Console.WriteLine($"Какое количество продукта необходимо в данном рецепте? Указывать в: { productController.GetProductUnits(answer - 3) } ");
                        double newProductValue = ParseDouble();

                        productController.SetProductValue(newProductValue);


                        recipeController.AddToRecipe(productController.CurrentProduct);

                        Console.WriteLine($"Добавили продукт {productController.GetProductName(answer - 3)} в рецепт");
                        break;

                }

                Console.WriteLine("Что бы добавить следующий продукт нажмите Y, что бы завершить добавление продуктов нажмите N");
                if (!AskYesOrNo())
                {
                    break;
                }

            }


            Console.Clear();
            Console.WriteLine("Введите описание процесса приготовления для рецепта");

            var description = Console.ReadLine();
            recipeController.AddToRecipe(null, description);
            recipeController.SaveRecipe();

            Console.Clear();
            Console.WriteLine("Ваш рецепт создан и добавлен в базу программы.");

            Console.WriteLine(recipeController.CurrentRecipe);


            Console.ReadLine();

        }

        private static string AskUnitsForNewProduct()
        {
            Console.Clear();
            menuItems.Clear();
            menuItems.Add("Выберите меру измерения для данного продукта");
            menuItems.Add("\n");
            menuItems.Add(Languages.Messages.Item);
            menuItems.Add(Languages.Messages.Kilogram);
            menuItems.Add(Languages.Messages.Gram);
            menuItems.Add(Languages.Messages.Liter);
            menuItems.Add(Languages.Messages.Milliliter);
            menuItems.Add(Languages.Messages.Tablespoon);

            switch (UserChoice())
            {
                case 2:
                    return Languages.Messages.Item;

                case 3:
                    return Languages.Messages.Kilogram;
                  
                case 4:
                    return Languages.Messages.Gram;
                
                case 5:
                    return Languages.Messages.Liter;

                case 6:
                    return Languages.Messages.Milliliter;

                case 7:
                    return Languages.Messages.Tablespoon;

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
