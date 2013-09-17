
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ATLS_4519_Lab2
{
//PART 1
 /*   class Person
    {
        public static string City = "Boulder";
        public string HairColor;
        public int Age;
        public double Height;
        public double Weight;
    }
    // all of Class Program goes here - see below

    class Program
    {
        static void Main(string[] args)
        {
            Person Bob = new Person();
            Person Carol = new Person();
            // Initialize Bob
            Bob.Age = 21;
            Bob.HairColor = "Brown";
            Bob.Weight = 185.4;
            Bob.Height = 72.3;
            // Initialize Carol
            Carol.Age = 20;
            Carol.HairColor = "Red";
            Carol.Weight = 125.7;
            Carol.Height = 67.1;
            // print some values
            Console.WriteLine("Bob’s age = {0}; Bob's Height = {1}; Bob’s weight = {2}; Bob's Hair Color = {3} Bob’s city = {4}; ", Bob.Age,
                Bob.Height, Bob.Weight, Bob.HairColor, Person.City);
            Console.WriteLine("Carol’s age = {0}; Carol's Height = {1}; Carol’s weight = {2}; Carol's Hair Color = {3}; Carol’s city = {4}", Carol.Age,
                Carol.Height, Carol.Weight, Carol.HairColor, Person.City);
        }
    }
}*/

//PART 2
/*
    class Person
    {
        public static string City = "Boulder";
        //create private internal variables to hold the "real" data and give them a default value
        private string hcr = "Unknown";
        private int years = 0;
        private double wtt = 0.0;
        private double htt = 0.0;
        public string HairColor
        {
            get
            {
                return hcr;// "gets" the private properties of hcr 
            }
            set
            {
                hcr = value;// sets hcr to be the properties for the value of haircolor  
            }
        }
        public int Age
        {
            get
            {
                return years;
            }
            set
            {
                if (value <= 100 && value >= 0)// don't allow age to be something negative, or something over 100. 
                {
                    years = value;
                }
                else
                {
                    years = 0;
                }
            }
        }
        public double Height
        {
            get
            {
                return htt;
            }
            set
            {
                htt = value;
            }
        }
        public double Weight
        {
            get
            {
                return wtt;
            }
            set
            {
                wtt = value;
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Person Bob = new Person();
            Person Carol = new Person();
            // Initialize Bob
            Bob.Age = 21;
            Bob.HairColor = "Brown";
            Bob.Weight = 185.4;
            Bob.Height = 72.3;
            // Initialize Carol
            Carol.Age = 20;
            Carol.HairColor = "Red";
            Carol.Weight = 125.7;
            Carol.Height = 67.1;
            // print some values
            Console.WriteLine("Bob’s age = {0}; Bob’s weight = {1}; Bob’s city = {2}", Bob.Age,
            Bob.Weight, Person.City);
            Console.WriteLine("Carol’s age = {0}; Carol’s weight = {1}; Carol’s city = {2}", Carol.Age,
Carol.Weight, Person.City);
        }
    }
}
*/

//PART 3

 /*   class Person
    {
        public static string City = "Boulder";
        //create private internal variables to hold the "real" data and give them a default value
        private string hcr = "Unknown";
        private int years = 0;
        private double wtt = 0.0;
        private double htt = 0.0;
        public string HairColor
        {
            get
            {
                return hcr;
            }
            set
            {
                hcr = value;
            }
        }
        public int Age
        {
            get
            {
                return years;
            }
            set
            {
                if (value <= 100 && value >= 0)
                {
                    years = value;
                }
                else
                {
                    years = 0;
                }
            }
        }
        public double Height
        {
            get
            {
                return htt;
            }
            set
            {
                htt = value;
            }
        }
        public double Weight
        {
            get
            {
                return wtt;
            }
            set
            {
                wtt = value;
            }
        }
        //Create an explicit constructor for instances of Person
        public Person(string hc, int age, double ht, double wt)
        {
            HairColor = hc;
            Age = age;
            Height = ht;
            Weight = wt;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Person Bob = new Person("Brown", 21, 72.3, 185.4);
            Person Carol = new Person("Red", 20, 67.1, 125.7);
            // Note that we can still change Bob
            Bob.Age = 55;
            Bob.HairColor = "Blonde";
            // ... Carol - see if we can set a bogus value
            Carol.Age = -140;

            // print some values
            Console.WriteLine("Bob’s age = {0}; Bob’s weight = {1}; Bob’s city = {2}", Bob.Age,
            Bob.Weight, Person.City);
            Console.WriteLine("Carol’s age = {0}; Carol’s weight = {1}; Carol’s city = {2}", Carol.Age,
Carol.Weight, Person.City);
        }
    }
}
*/

//PART 4
  /*  public abstract class Starship
    {
        //Instance variables
        private double FuelCapacity;
        private double MaxSpeed;
        public int ShieldStrength;
        public bool ShieldsUp = false;
        //Class Methods
        public double GetFuelCapacity()
        {
            return FuelCapacity;
        }
        public double GetMaxSpeed()
        {
            return MaxSpeed;
        }
        public int GetShieldStrength()
        {
            return ShieldStrength;
        }
        public bool GetShieldStatus()
        {
            if (ShieldsUp)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
       public abstract void ReFuel(double amount);
        //public virtual void ReFuel(double amount);// I don't understand how this works.. 
    
        
        public abstract void ReArm();
        public abstract void RaiseShields();
    }
    public class FastScout : Starship
    {
        private const int FastScoutShieldStrength = 5;
        public override void ReFuel(double amount)
        {
            // Increase the fuel load by amount, up to FuelCapacity
        }
        public override void ReArm()
        {
            // Rearm according to what a FastScout can carry
        }
        public override void RaiseShields()
        {
            //Raise the ship's meager shields
            ShieldsUp = true;
            ShieldStrength = FastScoutShieldStrength;
        }
        public void ReportShipsInArea()
        {
            //Scan the area and report (in some way) if there are other ships in the vicinity
        }
    }
    public class CargoShip : Starship
    {
        private const int CargoShieldStrength = 3;
        public override void ReFuel(double amount)
        {
            // Increase the fuel load by amount, up to FuelCapacity
        }
        public override void ReArm()
        {            // Rearm according to what a Cargo Ship can carry
        }
        public override void RaiseShields()
        {
            //Raise the ship's meager shields
            ShieldsUp = true;
            ShieldStrength = CargoShieldStrength;
        }
        public void LoadCargo()
        {
            //Take on cargo
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Starship[] myFleetArray = new Starship[2];
            myFleetArray[0] = new FastScout();
            myFleetArray[1] = new CargoShip();
            foreach (Starship myShip in myFleetArray)
            {
                myShip.ReFuel(15);
                myShip.ReArm();
            }
        }
    }
}
*/

//PART 5
    //define an interface related to ship status
    public interface IShipStatus
    {
        float getFuel();
        int getTorpedoes();
    }
    //class SpaceShip announces that it will implement this interface
    public class SpaceShip : IShipStatus
    {
        private float FuelAmt;
        private int Torpedoes;
        //define a constructor for class SpaceShip
        public SpaceShip(float InitialFuel, int InitialTorpedoes)
        {
            FuelAmt = InitialFuel; Torpedoes = InitialTorpedoes;
        }
        // Explicitly implement the first interface member 
        public float getFuel()
        {
            return FuelAmt;
        }
        // Explicitly implement the second interface member
        public int getTorpedoes()
        {
            return Torpedoes;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // Declare an instance of a SpaceShip
            SpaceShip ship1 = new SpaceShip(1500.0f, 15);
            // Declare the interface instance shipstats:
            IShipStatus shipstats = (IShipStatus)ship1;
            // Print out the ship status by calling the methods 
            // from an instance of the interface:
            Console.WriteLine("Fuel: {0}", shipstats.getFuel());
            Console.WriteLine("Torpedoes: {0}", shipstats.getTorpedoes());
            // Print out the ship status by calling the methods 
            // directly from the class:
            Console.WriteLine("Fuel: {0}", ship1.getFuel());
            Console.WriteLine("Torpedoes: {0}", ship1.getTorpedoes());
        }
    }
}