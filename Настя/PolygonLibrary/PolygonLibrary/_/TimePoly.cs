using System;

namespace Robust
{
	public class TimePoly : ITimePoly
	{
		private Poly[] poly;
		
		public TimePoly(int ntime)
		{
			poly = new Poly[ntime];
		}

		public Poly this[int time]
		{
			get { return poly[time]; }
			set { poly[time] = value; }
		}

		public int nTime
		{
			get { return poly.Length; }
		}

		public void ToCompactForm()
		{
			if (poly == null) return;
			foreach (Poly p in poly)
				p.ToCompactForm();
		}
	}
}