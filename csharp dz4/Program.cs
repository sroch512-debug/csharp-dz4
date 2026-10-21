
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetLessons
{
    public class InvalidAttributeException : Exception
    {
        public string AttributeName { get; }

        public InvalidAttributeException(string attributeName, string message)
            : base(message)
        {
            AttributeName = attributeName;
        }

        public InvalidAttributeException(string attributeName, string message, Exception innerException)
            : base(message, innerException)
        {
            AttributeName = attributeName;
        }
    }

    public class Attribute
    {
        public int BaseValue { get; set; }

        private List<(string Tag, int Value)> bonuses = new List<(string Tag, int Value)>();

        public Attribute(int baseValue)
        {
            BaseValue = baseValue;
        }

        public int Value
        {
            get
            {
                int summa = BaseValue;

                foreach (var bonus in bonuses)
                {
                    summa += bonus.Value;
                }

                return summa;
            }
        }

        public void AddBonus(string tag, int value)
        {
            bonuses.Add((tag, value));
        }

        public void AddBonuses(params (string Tag, int Value)[] newBonuses)
        {
            bonuses.AddRange(newBonuses);
        }

        public void AddToBaseValue(int value)
        {
            checked
            {
                BaseValue += value;
            }
        }

        public void RemoveBonus(string tag)
        {
            bonuses.RemoveAll(bonus => bonus.Tag == tag);
        }
    }

    public partial class Character
    {
        public Attribute Health { get; private set; }
        public Attribute Damage { get; private set; }

        public Character(int health, int damage)
        {
            Health = new Attribute(health);
            Damage = new Attribute(damage);
        }

        public override string ToString()
        {
            return "Health:" + Health.Value + "; Damage:" + Damage.Value;
        }

        public void TakeDamage(int value)
        {
            if (value < 0)
            {
                throw new InvalidAttributeException("Health", "Damage value cannot be negative.");
            }

            Health.BaseValue -= value;
        }

        public void Heal(int value)
        {
            if (value < 0)
            {
                throw new InvalidAttributeException("Health", "Heal value cannot be negative.");
            }

            Health.BaseValue += value;
        }

        public void Deconstruct(out int health, out int damage)
        {
            health = Health.Value;
            damage = Damage.Value;
        }
    }

    public partial class Character
    {
        public void LogStatus()
        {
            Console.WriteLine("[LOG] Current character status -> " + this);
        }
    }

    public class Program
    {
        public static void Main()
        {
            Character hero = new Character(100, 25);

            hero.Health.AddBonuses(("Ring", 15), ("Potion", 20));

            var (health, damage) = hero;
            Console.WriteLine("Deconstructed - Health: " + health + ", Damage: " + damage);

            hero.LogStatus();

            try
            {
                hero.Damage.AddToBaseValue(int.MaxValue);
            }
            catch (OverflowException)
            {
                Console.WriteLine("Overflow caught successfully in AddToBaseValue!");
            }

            try
            {
                hero.TakeDamage(-10);
            }
            catch (InvalidAttributeException ex) when (ex.AttributeName == "Health")
            {
                Console.WriteLine("Handled Health Exception: " + ex.Message);
            }
            catch (InvalidAttributeException ex) when (ex.AttributeName == "Damage")
            {
                Console.WriteLine("Handled Damage Exception: " + ex.Message);
            }
        }
    }
}
