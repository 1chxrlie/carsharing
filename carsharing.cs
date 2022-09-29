using System;
using System.Linq;

namespace grisha
{
    public enum MoscowStantion
    {
        Voykovskaya,
        Sokol,
        Tverskaya,
    }

    public enum CarMark
    {
        Audi,
        BMW,
        Lada,
        Kia
    }

    public class Car
    {
        public readonly MoscowStantion stantion;
        public readonly CarMark mark;
        public readonly string address;

        public Car(MoscowStantion stantion, CarMark mark, string address)
        {
            this.stantion = stantion;
            this.mark = mark;
            this.address = address;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            CarBookingConsoleProcess cargrisha = new CarBookingConsoleProcess(new Car[] {
                new Car(MoscowStantion.Voykovskaya, CarMark.Audi, "Константина Цареева, 12"),
                new Car(MoscowStantion.Voykovskaya, CarMark.BMW, "Константина Цареева, 15"),
                new Car(MoscowStantion.Voykovskaya, CarMark.Kia, "Константина Цареева, 16"),
                new Car(MoscowStantion.Voykovskaya, CarMark.Lada, "Константина Цареева, 17"),
                new Car(MoscowStantion.Tverskaya, CarMark.Kia, "Тверская, 16"),
                new Car(MoscowStantion.Tverskaya, CarMark.Audi, "Тверская, 40"),
                new Car(MoscowStantion.Tverskaya, CarMark.BMW, "Тверская, 100"),
                new Car(MoscowStantion.Tverskaya, CarMark.Lada, "Тверская, 1"),
                new Car(MoscowStantion.Sokol, CarMark.Kia, "Космонавта Волкова, 16"),
                new Car(MoscowStantion.Sokol, CarMark.Audi, "Космонавта Волкова, 7"),
                new Car(MoscowStantion.Sokol, CarMark.BMW, "Космонавта Волкова, 10"),
                new Car(MoscowStantion.Sokol, CarMark.Lada, "Космонавта Волкова, 1"),
            });

            cargrisha.getCarBookingOrder();

            Console.ReadKey();
        }
    }

    public abstract class Process
    {
        protected abstract void sayMessage(string message);
        protected abstract void sayDictionaryValues<Enum>(Dictionary<Enum, string> dictionary);
        protected abstract string getString();
        protected abstract int getNumber();
        protected abstract Enum getEnum<Enum>(Dictionary<Enum, string> dictionary);
    }

    public class ConsoleProcess : Process
    {
        protected override void sayMessage(string message)
        {
            Console.WriteLine(message);
        }

        protected override void sayDictionaryValues<Enum>(Dictionary<Enum, string> dictionary)
        {
            foreach (KeyValuePair<Enum, string> dictionaryKeyValue in dictionary)
            {
                sayMessage(dictionaryKeyValue.Value);
            }
        }

        protected override string getString()
        {
            return Console.ReadLine();
        }

        protected override int getNumber()
        {
            try
            {
                return Int32.Parse(Console.ReadLine());
            }
            catch
            {
                sayMessage("Введите число");

                return getNumber();
            }
        }

        protected override Enum getEnum<Enum>(Dictionary<Enum, string> dictionary)
        {
            string inputValue = getString();

            foreach (KeyValuePair<Enum, string> dictionaryKeyValue in dictionary)
            {
                if (dictionaryKeyValue.Value == inputValue)
                {
                    return dictionaryKeyValue.Key;
                }
            }

            sayMessage("Введите значение из списка:");
            sayDictionaryValues(dictionary);

            return getEnum(dictionary);
        }
    }

    public class CarBookingConsoleProcess : ConsoleProcess
    {
        private CarsService carsService;

        public CarBookingConsoleProcess(Car[] cars)
        {
            this.carsService = new CarsService(cars);
        }

        public void getCarBookingOrder()
        {
            sayMessage("Здравствуйте! Вас приветствует каршеринг Cargrisha\n");

            MoscowStantion stantion = getInputStation();
            CarMark mark = getInputCarMark();
            string address = getInputAddress(mark, stantion);
            int hours = getInputHours();

            sayMessage("Ваш заказ:");
            sayMessage("Адресс машины: " + address);
            sayMessage("Марка машины: " + mark);
            sayMessage("Цена заказа: " + carsService.prices[mark] * hours);
        }

        private MoscowStantion getInputStation()
        {
            sayMessage("Введите станцию метро:");
            sayDictionaryValues(TextsState.stantions);

            return getEnum(TextsState.stantions);
        }

        private CarMark getInputCarMark()
        {
            sayMessage("Введите марку машины:");
            sayDictionaryValues(TextsState.marks);

            return getEnum(TextsState.marks);
        }

        private int getInputHours()
        {
            sayMessage("На сколько часов забронировать машину?");

            int hours = getNumber();

            if (hours > 48)
            {
                sayMessage("Введите количество часов меньше 48");
                return getInputHours();
            }
            if (hours <= 0)
            {
                sayMessage("Введите количество часов больше 0");
                return getInputHours();
            }

            return hours;
        }

        private string getInputAddress(CarMark mark, MoscowStantion stantion)
        {
            List<string> addreses = carsService.getAddresesByStationAndMark(stantion, mark);

            sayMessage("Выберите адресс (номер)");

            for (int index = 0; index < addreses.Count; index++)
            {
                sayMessage(index + ") " + addreses[index]);
            }

            int addressIndex = getNumber();

            if (addressIndex > addreses.Count || addreses.Count < 0)
            {
                sayMessage("Выберите адресс (номер)");

                for (int index = 0; index < addreses.Count; index++)
                {
                    sayMessage(index + ") " + addreses[index]);
                }

                return getInputAddress(mark, stantion);
            }

            return addreses[addressIndex];
        }

        public class CarsService
        {
            public readonly Dictionary<CarMark, int> prices = new Dictionary<CarMark, int>()
        {
            { CarMark.Audi, 100 },
            { CarMark.BMW, 200 },
            { CarMark.Lada, 300 },
            { CarMark.Kia, 400 }
        };

            private Car[] cars;

            public CarsService(Car[] cars)
            {
                this.cars = cars;
            }

            public List<string> getAddresesByStationAndMark(MoscowStantion station, CarMark mark)
            {
                List<string> adresess = new List<string>();

                foreach (Car car in cars)
                {
                    if (car.stantion == station && car.mark == mark)
                    {
                        adresess.Add(car.address);
                    }
                }

                return adresess;
            }
        }

        public static class TextsState
        {
            public static readonly Dictionary<MoscowStantion, string> stantions = new Dictionary<MoscowStantion, string>()
        {
            { MoscowStantion.Voykovskaya, "Войковская"},
            { MoscowStantion.Sokol, "Сокол"},
            { MoscowStantion.Tverskaya, "Тверская" }
        };
            public static readonly Dictionary<CarMark, string> marks = new Dictionary<CarMark, string>()
        {
            { CarMark.Audi, "Audi"},
            { CarMark.BMW, "BMW"},
            { CarMark.Lada, "Lada" },
            { CarMark.Kia, "Kia" }
        };
        }
    }
}