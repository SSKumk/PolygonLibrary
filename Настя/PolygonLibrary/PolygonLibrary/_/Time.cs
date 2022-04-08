using System;

namespace Robust
{
	public class Time
	{
		private readonly double t0, t1;
		private readonly double delta;
		private readonly int count;
		
		public Time(double T0, double T1, double Delta)
		{
			this.t0 = T0; this.t1 = T1; this.delta = Delta;
			this.count = (int)Math.Ceiling((T1 - T0) / Delta + 1);
		}

		public double T0 { get { return t0; } }
		public double T1 { get { return t1; } }
		public double Delta { get { return delta; } }
		public int Count { get { return count; } }

		public double this [int idx]
		{
			get
			{
				double t = delta * idx + t0;
				if (t < t0) t = t0; 
				if (t > t1) t = t1;
				return t;
			}
		}

		public int Index(double time)
		{
			if (time < t0) return 0;
			if (time > t1) return count - 1;
			return (int)Math.Round((time - t0) / delta);
		}

		public double[] Axis()
		{
			double[] axis = new double[count];
			for (int i = 0; i < count; i++)
				axis[i] = this[i];
			return axis;
		}
		
		public override bool Equals(object obj)
		{
			if (this.GetType() != obj.GetType()) return false;
			Time that = (Time)obj;
			if (this.T0 != that.T0 || this.T1 != that.T1 || 
				this.Delta != that.Delta) return false;
			return true;
		}

		public override int GetHashCode()
		{
			return T0.GetHashCode() + T1.GetHashCode() + Delta.GetHashCode();
		}

		public static bool operator == (Time a, Time b)
		{
			return a.Equals(b);
		}

		public static bool operator != (Time a, Time b)
		{
			return !a.Equals(b);
		}
	}
}
