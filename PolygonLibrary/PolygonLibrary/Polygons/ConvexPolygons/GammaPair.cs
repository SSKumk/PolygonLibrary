using System;

using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Polygons.ConvexPolygons; 

/// <summary>
/// Class for storing a pair (normal;value of function)
/// </summary>
public class GammaPair : IEquatable<GammaPair>, IComparable<GammaPair>
{
	/// <summary>
	/// The equality comparator
	/// </summary>
	/// <param name="other">The pair to be compared with</param>
	/// <returns>true, if equal; false, otherwise</returns>
	public bool Equals(GammaPair other)
	{
		bool res = Tools.EQ(this.Normal.PolarAngle, other.Normal.PolarAngle);
		if (res)
		{
			double
				l1 = this.Normal.Length,
				l2 = other.Normal.Length;

#if DEBUG
			if (Tools.EQ(l1)) {
				throw new ArgumentException("Equality of two gamma pairs: the first argument has zero normal");
			}

			if (Tools.EQ(l2)) {
				throw new ArgumentException("Equality of two gamma pairs: the second argument has zero normal");
			}
#endif

			res = Tools.EQ(this.Value / l1, other.Value / l2);
		}

		return res;
	}

	/// <summary>
	/// The less-greater comparator: 
	///   a) compares normals counterclockwise in the sense of the polar angle
	///   b) otherwise the values value/|normal| are compared; if at least, one normal is zero, 
	///      an exception is thrown
	/// Actually, comparison is performed in the sense of counterclockwise of the normals
	/// and, if they are equal, in the sense of inclusion of semi-planes
	/// </summary>
	/// <param name="other">The pair to be compared with</param>
	/// <returns>-1, this pair is less; +1, this pair is greater; 0, the pairs are equal</returns>
	public int CompareTo(GammaPair other)
	{
		int res = Tools.CMP(this.Normal.PolarAngle, other.Normal.PolarAngle);
		if (res != 0) {
			return res;
		}

		double
			l1 = this.Normal.Length,
			l2 = other.Normal.Length;

#if DEBUG
		if (Tools.EQ(l1)) {
			throw new ArgumentException("Comparison of two gamma pairs: the first argument has zero normal");
		}

		if (Tools.EQ(l2)) {
			throw new ArgumentException("Comparison of two gamma pairs: the second argument has zero normal");
		}
#endif

		return Tools.CMP(this.Value / l1, other.Value / l2);
	}

	/// <summary>
	/// The normal vector
	/// </summary>
	public Vector2D Normal { get; protected set; }

	/// <summary>
	/// The value of the support function on the vector
	/// </summary>
	public double Value { get; protected set; }

	/// <summary>
	/// Default constructor
	/// </summary>
	public GammaPair()
	{
		Normal = new Vector2D(1, 0);
		Value = 0;
	}

	/// <summary>
	/// Value construction of the pair.
	/// If the vector iz zero, an exception is thrown
	/// </summary>
	/// <param name="nv">The vector of the pair</param>
	/// <param name="ng">The value of the pair</param>
	/// <param name="ToNormalize">Flag showing necessity to normalize the vector before storing into the pair</param>
	public GammaPair(Vector2D nv, double ng, bool ToNormalize = false)
	{
#if DEBUG
		double ll = nv.Length;
		if (Tools.EQ(ll)) {
			throw new ArgumentException("Cannot construct a GammaPair with zero normal");
		}
#endif
		if (ToNormalize)
		{
			double l = nv.Length;
			Normal = nv / l;
			Value = ng / l;
		}
		else
		{
			Normal = nv;
			Value = ng;
		}
	}

	/// <summary>
	/// Copying constructor
	/// </summary>
	/// <param name="p">The pair to be copied</param>
	public GammaPair(GammaPair p)
	{
		Normal = p.Normal;
		Value = p.Value;
	}

	/// <summary>
	/// String representation of the pair
	/// </summary>
	/// <returns></returns>
	public override string ToString() => "[" + Normal + ";" + Value + "]";

	/// <summary>
	/// Computing the point, which is intersection of lines defined by two pairs.
	/// If the lines are parallel, an exception is thrown
	/// </summary>
	/// <param name="g1">The first pair</param>
	/// <param name="g2">The second pair</param>
	/// <returns>The corresponding point</returns>
	public static Point2D CrossPairs(GammaPair g1, GammaPair g2)
	{
#if DEBUG
		if (Vector2D.AreParallel(g1.Normal, g2.Normal)) {
			throw new ArgumentException("Cannot cross lines defined by pairs with parallel normals");
		}
#endif
		// g1.g = g1.v * r = g1.v.x * x + g1.v.y * y,  g2.g = g2.v * r = g2.v.x * x + g2.v.y * y
		double
			d = g1.Normal.x * g2.Normal.y - g2.Normal.x * g1.Normal.y,
			d1 = g1.Value * g2.Normal.y - g2.Value * g1.Normal.y,
			d2 = g1.Normal.x * g2.Value - g2.Normal.x * g1.Value;
		return new Point2D(d1 / d, d2 / d);
	}
}
