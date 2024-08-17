using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Metalogix.UI.WinForms
{
	public class Coordinates
	{
		public int X
		{
			get;
			set;
		}

		public int Y
		{
			get;
			set;
		}

		public Coordinates(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public void Add(Coordinates coordinates)
		{
			Coordinates x = this;
			x.X = x.X + coordinates.X;
			Coordinates y = this;
			y.Y = y.Y + coordinates.Y;
		}

		public void Add(Point point)
		{
			Coordinates x = this;
			x.X = x.X + point.X;
			Coordinates y = this;
			y.Y = y.Y + point.Y;
		}

		public void Subtract(Coordinates coordinates)
		{
			Coordinates x = this;
			x.X = x.X - coordinates.X;
			Coordinates y = this;
			y.Y = y.Y - coordinates.Y;
		}

		public void Subtract(Point point)
		{
			Coordinates x = this;
			x.X = x.X - point.X;
			Coordinates y = this;
			y.Y = y.Y - point.Y;
		}

		public override string ToString()
		{
			return string.Format("X:{0} Y:{1}", this.X, this.Y);
		}
	}
}