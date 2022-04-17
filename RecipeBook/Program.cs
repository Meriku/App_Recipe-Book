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
             * Подбор подходящего рецепта по ингредиентам 
             * Возможность добавить свой рецепт             - реализовано (в том числе добавление продукта которого нет в базе)
             * Сериализация рецептов (сохранение в файл)    - реализовано
             */






            MainMenu();
       
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

        private static void MenuFindRecipeByProduct()
        {
            throw new NotImplementedException();
        }

        private static void MenuFindRecipeByName()
        {
            throw new NotImplementedException();
        }

        private static void MenuCreateRecipe()
        {
            Console.Clear();
            Console.WriteLine("Введите название нового рецепта");
            var recipename = Console.ReadLine();
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
                        var name = Console.ReadLine();
                        if (productController.IsExist(name))
                        {
                            Console.WriteLine("Данный продукт уже существует");
                            Console.Read();
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
                var key = Console.ReadKey().Key;

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



    }
}
