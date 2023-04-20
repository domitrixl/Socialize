using System;
using System.Drawing;

namespace ClassLibrary1
{
	public class RegularClass
	{
		// Variable, mit der in der Klasse gearbeitet wird
		private int myVar;

		// öffentlich aufrufbar
		public int MyProperty
		{
			// gibt die Variable myVar zurück
			get { return myVar; }

			// Überprüfung des Wertes true -> myVar wird zugeteilt
			set
			{
				if (value > 0)
				{
					myVar = value;
				}
			}
		}

		#region stuff
		public int MyValue;
		private int myValue;

		public int Addition(int a, int b)
		{
			return a + b;
		}

		public void WriteText(string Text)
		{
			Console.WriteLine(Text);
		}
		#endregion
	}

	public static class StaticClass
	{
		public static int Addition(int a, int b)
		{
			return a + b;
		}

		public static void WriteText(string Text)
		{
			Console.WriteLine(Text);
		}
	}

	public class Car
	{
		private int wheels;
		private string plate;

		public Car(int wheels, string plate)
		{
			this.wheels = wheels;
			this.plate = plate;
		}
	}

	public class Person
	{
		private static string Name;

		// Konstruktoren werden beim Erstellen einer Klasse abgerufen - mit oder ohne Parameter
		public Person(string name)
		{
			Name = name;
		}

		// mit static als Modifikator kann diese auch nach der Erstellung noch ausgeführt werden
		static Person()
		{
			Name = "Max";
		}
	}

	public class Vehicle
	{
		protected int wheels;
		protected Color color;

		public Vehicle(int wheels, Color color)
		{
			this.wheels = wheels;
			this.color = color;
		}

		public virtual void Stop()
		{
			Console.WriteLine("Vehicle Stopped");
		}

		protected void WriteType()
		{
			Console.WriteLine("Vehicle");
		}
	}

	public class Bike : Vehicle
	{
		private int seats;

		// führt Vehicle mit dem Konstruktor base aus und fügt noch seats hinzu
		public Bike(int wheels, Color color, int seats) : base(wheels, color)
		{
			this.seats = seats;
		}

		// überschreibt die Methode Stop();
		public override void Stop()
		{
			Console.WriteLine("Bike Stopped");
		}

		// erzeugt eine neue Methode WriteType()
		new void WriteType()
		{
			Console.WriteLine("Bike");
		}
	}
}