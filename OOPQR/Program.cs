using System;
using System.Collections.Generic;

// Общий класс для ингридиентов
public abstract class Ingridients
{
    private static int idCounter = 0;
    public int id { get; }
    public string name { get; set; }
    public DateTime expiryDate { get; set; } // срок годности

    public  Ingridients(string name, DateTime expiryDate)
    {
        idCounter++;
        id = idCounter;
        this.name = name;
        this.expiryDate = expiryDate;
    }
}

// Овощи и фрукты
public class VegetablesAndFruits : Ingridients
{
    public VegetablesAndFruits(string name, DateTime expiryDate) : base(name, expiryDate)
    {
    }
}

// Соусы
public class Sauce : Ingridients
{
    public Sauce(string name, DateTime expiryDate) : base (name, expiryDate) 
    {
       
    }
}

// Прочие продукты
public class OtherProducts : Ingridients
{
    public OtherProducts(string name, DateTime expiryDate) : base(name, expiryDate)
    {
       
    }
}

// Морепродукты
public class Seafood : Ingridients
{
    public Seafood(string name, DateTime expiryDate) : base(name, expiryDate)
    {
    }
}


// Общий класс для продукции
public class FoodAndDrinks
{
    private static int _idCounter = 0;
    public int id { get; }
    public string name { get; set; }
    public double price { get; set; } // в рублях
    public double weight { get; set; } // в килограммах
    public List<Ingridients> ingredients { get; set; }

    protected FoodAndDrinks(string name, double price, double weight, List<Ingridients> ingredients)
    {
        _idCounter++;
        id = _idCounter;
        this.name = name;
        this.price = price;
        this.weight = weight;
        this.ingredients = ingredients;
    }
}

// Напитки
public class Beverage : FoodAndDrinks
{
    public Beverage(string name, double price, double weight, List<Ingridients> ingredients) : base(name, price, weight, ingredients)
    {
      
    }
}

// Блюда
public class Dish : FoodAndDrinks
{
    public Dish(string name, double price, double weight, List<Ingridients> ingredients) : base(name, price, weight, ingredients)
    {
    }
}


// Заказ и возврат блюд
public class DishManagement
{
    public List<Dish> menu = new List<Dish>();
    public decimal income { get; set; } = 0;

    public void OrderDish(int dishId, IngredientsStorage manager)
    {
        if (!CheckIngredientsAvailability(dishId, manager))
        {
            Console.WriteLine("Нет ингридиентов");
            return;
        }

        TakeIngredientsFromStorage(dishId, manager);

        foreach (var dish in menu)
        {
            if (dish.id == dishId)
            {
                income += (decimal)dish.price;
                Console.WriteLine($"Заказано блюдо: {dish.name}, Сумма: {dish.price} руб., id заказа: {dish.id}");
                break;
            }
        }
    }

    public void ReturnDish(int dishId, IngredientsStorage manager)
    {
        if (!CheckIfOrderExists(dishId))
        {
            Console.WriteLine($"Блюдо с id {dishId} не было заказано.");
            return;
        }

        foreach (var dish in menu)
        {
            if (dish.id == dishId)
            {
                income -= (decimal)dish.price;
                Console.WriteLine($"Возвращено блюдо: {dish.name}, Сумма возврата: {dish.price} руб., id заказа: {dish.id}");
                break;
            }
        }
    }

    private void TakeIngredientsFromStorage(int dishId, IngredientsStorage ingredientsStorage)
    {
        foreach (Dish dish in menu)
        {
            if (dish.id == dishId)
            {
                foreach (var ingredient in dish.ingredients)
                {
                    foreach (var item in ingredientsStorage.ingredientsOnStorage)
                    {
                        if (item.ingridients.id == ingredient.id)
                        {
                            item.amount--;
                            Console.WriteLine($"Ингредиент \"{item.ingridients.name}\" списан со склада для блюда \"{dish.name}\"");
                            break;
                        }
                    }
                }
                break;
            }
        }
    }

    private bool CheckIngredientsAvailability(int dishId, IngredientsStorage ingredientsStorage)
    {
        foreach (var dish in menu)
        {
            if (dish.id == dishId)
            {
                foreach (var ingredient in dish.ingredients)
                {
                    foreach (var item in ingredientsStorage.ingredientsOnStorage)
                    {
                        if (item.ingridients.id == ingredient.id && item.amount > 0)
                        {
                            return true;
                        }
                    }
                }
                break;
            }
        }
        return false;
    }

    private bool CheckIfOrderExists(int dishId)
    {
        foreach (var orderDish in menu)
        {
            if (orderDish.id == dishId)
            {
                return true;
            }
        }
        return false;
    }
}

public class IngredientsOnStorage
{
    public Ingridients ingridients { get; set; }
    public int amount { get; set; }
    public IngredientsOnStorage(Ingridients ingridients, int amount)
    {
        this.ingridients = ingridients;
        this.amount = amount;
    }
}

// Учёт продуктов склада
public class IngredientsStorage
{
    public List<Ingridients> avaliableProductsFromOrder { get; set; } = new List<Ingridients>();
    public List<IngredientsOnStorage> ingredientsOnStorage { get; set; } = new List<IngredientsOnStorage>();

    // Доставка
    public void DeliveryIngredients(int idIngredient, int amount)
    {
        if (idIngredient < 0 )
        {
            Console.WriteLine($"Продукта с id: {idIngredient} не существует");
            return;
        }
        if (amount <= 0)
        {
            Console.WriteLine("Неверное количество");
            return;
        }
        Console.WriteLine("Проверка доступности продукта для заказа");
        foreach (Ingridients product in avaliableProductsFromOrder)
        {
            // Ищем совпадения по ид
            if (product.id == idIngredient)
            {
                Console.WriteLine("Продукт доступен");

                // Проверяем, есть ли такой продукт на складе
                bool found = false;
                foreach (IngredientsOnStorage ingredient in ingredientsOnStorage)
                {
                    if (ingredient.ingridients.id == idIngredient)
                    {
                        // Если продукт есть - увеличиваем его количество
                        ingredient.amount += amount;
                        found = true;
                        Console.WriteLine($"Количество продукта \"{ingredient.ingridients.name}\" " +
                            $"на складе увеличено на {amount}");
                        break;
                    }
                }
                if (!found)
                {
                    // Если продукта нет добавляем и увеличиваем
                    ingredientsOnStorage.Add(new IngredientsOnStorage(product, amount));
                    Console.WriteLine("Продукт заказан");
                }
                return;
            }
        }
        Console.WriteLine("Продукт не доступен");
    }

    // Списание
    public void DisposalIngredients(int idIngredient, int amount)
    {
        if (idIngredient < 0)
        {
            Console.WriteLine($"Продукта с id: {idIngredient} не существует");
            return;
        }
        if (amount <= 0)
        {
            Console.WriteLine("Ошибка, количество не может быть отрицательным");
            return;
        }
        Console.WriteLine("Проверка наличия продукта на складе");
        foreach (IngredientsOnStorage ingredient in ingredientsOnStorage)
        {
            if (ingredient.ingridients.id == idIngredient)
            {
                if (ingredient.amount >= amount)
                {
                    // Если на складе достаточно продукта, списываем его
                    ingredient.amount -= amount;
                    Console.WriteLine($"Списано {amount} единиц продукта \"{ingredient.ingridients.name}\" со склада");
                }
                else
                {
                    // Если нет - выводим ошибку
                    Console.WriteLine($"На складе недостаточно продукта \"{ingredient.ingridients.name}\"");
                }
                return;
            }
        }
        Console.WriteLine("Продукт не найден на складе");
    }

    // Продукты на складе
    public void ShowAvailableIngredients()
    {
        Console.WriteLine("Продукты на складе");
        foreach (IngredientsOnStorage ingredientOnStorage in ingredientsOnStorage)
        {
            Console.WriteLine($"Название: {ingredientOnStorage.ingridients.name}, количество {ingredientOnStorage.amount} ");
        }
    }
}


// Мэйн
internal class Program
{
    static void Main()
    {
        // Создаём овощи
        VegetablesAndFruits tomato = new VegetablesAndFruits("Помидоры", DateTime.Now.AddDays(7));
        VegetablesAndFruits cucumber = new VegetablesAndFruits("Огурцы", DateTime.Now.AddDays(5));
        VegetablesAndFruits cabbage = new VegetablesAndFruits("Капуста", DateTime.Now.AddDays(3));
        VegetablesAndFruits carrot = new VegetablesAndFruits("Морковь", DateTime.Now.AddDays(10));
        VegetablesAndFruits potato = new VegetablesAndFruits("Картофель", DateTime.Now.AddDays(14));

        // Создаём фрукты
        VegetablesAndFruits apple = new VegetablesAndFruits("Яблоки", DateTime.Now.AddDays(7));
        VegetablesAndFruits banana = new VegetablesAndFruits("Бананы", DateTime.Now.AddDays(5));
        VegetablesAndFruits orange = new VegetablesAndFruits("Апельсины", DateTime.Now.AddDays(4));
        VegetablesAndFruits pear = new VegetablesAndFruits("Груши", DateTime.Now.AddDays(6));
        VegetablesAndFruits pineapple = new VegetablesAndFruits("Ананасы", DateTime.Now.AddDays(7));

        // Создаём другие продукты
        OtherProducts sugar = new OtherProducts("Сахар", DateTime.Now.AddDays(90));
        OtherProducts salt = new OtherProducts("Соль", DateTime.Now.AddDays(180));
        OtherProducts flour = new OtherProducts("Мука", DateTime.Now.AddDays(120));
        OtherProducts oil = new OtherProducts("Масло", DateTime.Now.AddDays(150));
        OtherProducts cheese = new OtherProducts("Сыр", DateTime.Now.AddDays(60));
        OtherProducts egg = new OtherProducts("Яйца", DateTime.Now.AddDays(120));
        OtherProducts bread = new OtherProducts("Хлеб", DateTime.Now.AddDays(150));

        // Создаём блюда

        List<Ingridients> saladIngredients = new List<Ingridients> { tomato, cucumber, cabbage };
        List<Ingridients> fruitSaladIngredients = new List<Ingridients> { apple, banana, orange, pear, pineapple };
        List<Ingridients> cakeIngredients = new List<Ingridients> { flour, sugar, oil, apple };
        List<Ingridients> omeletteIngredients = new List<Ingridients> { egg, salt, cheese, tomato };
        List<Ingridients> sandwichIngredients = new List<Ingridients> { bread, cheese, tomato, cucumber };

        DishManagement dishManager = new DishManagement();

        dishManager.menu.Add(new Dish("Салат", 150, 0.1, saladIngredients));
        dishManager.menu.Add(new Dish("Фруктовый салат", 200, 0.1, fruitSaladIngredients));
        dishManager.menu.Add(new Dish("Яблочный пирог", 250, 0.1, cakeIngredients));
        dishManager.menu.Add(new Dish("Омлет", 180, 0.1, omeletteIngredients));
        dishManager.menu.Add(new Dish("Сэндвич", 120, 0.1, sandwichIngredients));

        IngredientsStorage ingredientsStorage = new IngredientsStorage();
        ingredientsStorage.avaliableProductsFromOrder.Add(tomato);
        ingredientsStorage.avaliableProductsFromOrder.Add(cucumber);
        ingredientsStorage.avaliableProductsFromOrder.Add(cabbage);
        ingredientsStorage.avaliableProductsFromOrder.Add(carrot);
        ingredientsStorage.avaliableProductsFromOrder.Add(potato);
        ingredientsStorage.avaliableProductsFromOrder.Add(apple);
        ingredientsStorage.avaliableProductsFromOrder.Add(banana);
        ingredientsStorage.avaliableProductsFromOrder.Add(orange);
        ingredientsStorage.avaliableProductsFromOrder.Add(pear);
        ingredientsStorage.avaliableProductsFromOrder.Add(pineapple);
        ingredientsStorage.avaliableProductsFromOrder.Add(sugar);
        ingredientsStorage.avaliableProductsFromOrder.Add(salt);
        ingredientsStorage.avaliableProductsFromOrder.Add(flour);
        ingredientsStorage.avaliableProductsFromOrder.Add(oil);
        ingredientsStorage.avaliableProductsFromOrder.Add(cheese);
        ingredientsStorage.avaliableProductsFromOrder.Add(egg);
        ingredientsStorage.avaliableProductsFromOrder.Add(bread);

        int amountOfAvailableDishes = 17;
        for (int i = 0; i < amountOfAvailableDishes; i++)
        {
            int amount = 5; 
            ingredientsStorage.DeliveryIngredients(i, amount);
        }
        Console.Clear();
        Console.WriteLine("Выполнена автоматическая доставка товара на склад");

        bool exit = false;

        while (!exit)
        {
            Console.WriteLine($"\nДоход дня: {dishManager.income}\n");
            Console.WriteLine("Меню выбора:");
            Console.WriteLine("1. Принять продукты на склад");
            Console.WriteLine("2. Списать продукты со склада");
            Console.WriteLine("3. Заказ блюда");
            Console.WriteLine("4. Возврат блюда");
            Console.WriteLine("5. Показать продукты на складе");
            Console.WriteLine("6. Выход");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введите id продукта: ");
                    int delivId = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Введите количество продуктов: ");
                    int delivWe = Convert.ToInt32(Console.ReadLine());
                    ingredientsStorage.DeliveryIngredients(delivId, delivWe);
                    break;
                case "2":
                    Console.Write("Введите id продукта: ");
                    int dispId = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Введите количество продуктов: ");
                    int dispWe = Convert.ToInt32(Console.ReadLine());
                    ingredientsStorage.DisposalIngredients(dispId, dispWe);
                    break;
                case "3":
                    Console.WriteLine("Выберите блюдо для заказа:");
                    for (int i = 0; i < dishManager.menu.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {dishManager.menu[i].name} - {dishManager.menu[i].price} руб.");
                    }
                    Console.Write("Введите номер блюда: ");
                    int dishIndex = Convert.ToInt32(Console.ReadLine());
                    if (dishIndex >= 1 && dishIndex <= dishManager.menu.Count)
                    {
                        dishManager.OrderDish(dishIndex, ingredientsStorage);
                    }
                    else
                    {
                        Console.WriteLine("Неверный номер блюда");
                    }
                    break;
                case "4":
                    Console.WriteLine("Выберите блюдо для возврата:");
                    for (int i = 0; i < dishManager.menu.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {dishManager.menu[i].name}");
                    }
                    Console.Write("Введите номер блюда: ");

                    int returnIndex = Convert.ToInt32(Console.ReadLine());
                    if (returnIndex >= 1 && returnIndex <= dishManager.menu.Count)
                    {
                        dishManager.ReturnDish(returnIndex, ingredientsStorage);
                    }
                    else
                    {
                        Console.WriteLine("Неверный номер блюда.");
                    }
                    break;
                case "5":
                    ingredientsStorage.ShowAvailableIngredients();
                    break;
                case "6":
                    Console.WriteLine("Закрытие приложения.");
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.");
                    break;
            }
        }

    }
}
