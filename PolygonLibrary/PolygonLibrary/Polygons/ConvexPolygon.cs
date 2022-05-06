using System;
using System.Collections.Generic;
using System.Linq;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Polygons
{
	/// <summary>
	/// Class for stroing a pair (normal;value of function)
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

			if (Vector2D.AreCounterdirected(this.Normal, other.Normal)) {
				return -1;
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
		public override string ToString() => "[" + Normal.ToString() + ";" + Value + "]";

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

	/// <summary>
	/// Class of a support function defined as an array of pairs (normal;value)
	/// ordered counterclockwise over the normals
	/// </summary>
	public class SupportFunction : List<GammaPair>
	{
		/// <summary>
		/// Constructor of the SupportFunction class, 
		/// which takes a list of pairs, filters non-zero vectors,
		/// normalizes vectors, sorts the pairs counterclockwise,
		/// and filters pairs with equal vectors
		/// </summary>
		/// <param name="gs">The list of pairs</param>
		public SupportFunction(IEnumerable<GammaPair> gs, bool ToSort = true)
		{
			List<GammaPair> gs1 = new List<GammaPair>();
			foreach (GammaPair pair in gs)
			{
				if (pair.Normal != Vector2D.Zero)
				{
					double l = pair.Normal.Length;
					gs1.Add(new GammaPair(pair.Normal / l, pair.Value / l));
				}
			}
			if (gs1.Count == 0) {
				throw new ArgumentException("Only pairs with zero normals in initialization of a support function");
			}

			if (ToSort) {
				gs1.Sort();
			}

			foreach (GammaPair pair in gs1)
			{
				if (this.Count == 0 || !this[this.Count - 1].Normal.Equals(pair.Normal)) {
					this.Add(pair);
				}
			}
		}

		/// <summary>
		/// Constructor of the SupposrtFunction class on the basis of vertices 
		/// of the polygon ordered counterclockwise (or the collection of points,
		/// which convex hull is the polygon). If there are too few points
		/// in the collection (after convexification, if necessary) - 1 or 0,
		/// then an exception is thrown
		/// </summary>
		/// <param name="ps">The collection of points</param>
		/// <param name="ToConvexify">Flag showing whether the collection should be convexified in the beginning</param>
		public SupportFunction(List<Point2D> ps, bool ToConvexify = false)
		{
			List<Point2D> ps1 = ToConvexify ? Convexification.ArcHull2D(ps) : ps;

			if (ps1.Count > 2)
			{
				int i, j;
				for (i = 0, j = 1; i < ps1.Count; i++, j = (j + 1) % ps1.Count)
				{
					Vector2D v = (ps1[j] - ps1[i]).TurnCW().Normalize();
					this.Add(new GammaPair(v, v * (Vector2D)ps1[i]));
				}
			}
			else if (ps1.Count == 2)
			{
				// Special generation of the support function of a segment
				Vector2D v = (ps1[1] - ps1[0]).Normalize();
				Vector2D v1 = v.TurnCW();
				this.Add(new GammaPair(v, v * (Vector2D)ps1[1]));
				this.Add(new GammaPair(-v, -v * (Vector2D)ps1[0]));
				this.Add(new GammaPair(v1, v1 * (Vector2D)ps1[0]));
				this.Add(new GammaPair(-v1, -v1 * (Vector2D)ps1[0]));
			}
			else
			{
				Vector2D p = (Vector2D)ps1[0];
				this.Add(new GammaPair(Vector2D.E1, Vector2D.E1 * p));
				this.Add(new GammaPair(Vector2D.E2, Vector2D.E2 * p));
				this.Add(new GammaPair(-Vector2D.E1, -Vector2D.E1 * p));
				this.Add(new GammaPair(-Vector2D.E2, -Vector2D.E2 * p));
			}

			this.Sort();
		}

		/// <summary>
		/// Procedure for finding cone that contains the given vector.
		/// If the vector coincides with a normal from the collection, 
		/// the cone will be returned, which has the vector as the counterclockwise boundary.
		/// The search is performed by dichotomy, therefore, it is of O(log n) complexity.
		/// If the collection is underdetermined (that is has one or zero pairs),
		/// an exception is thrown.
		/// </summary>
		/// <param name="v">The vector to be localized</param>
		/// <param name="i">Index of the clockwise boundary of the cone</param>
		/// <param name="j">Index of the counterclockwise boundary of the cone</param>
		public void FindCone(Vector2D v, out int i, out int j)
		{
#if DEBUG
			if (this.Count < 2) {
				throw new Exception("The function is defined by too few directions");
			}
#endif

			// If the vector coincides with the last vector in the collection,
			// the cone is between (Count-2)th and (Count-1)th normals
			if (Tools.EQ(v.PolarAngle, this[Count - 1].Normal.PolarAngle))
			{
				i = Count - 2;
				j = Count - 1;
			}
			// If the vector belongs to the cone between the last and the first normals,
			// process the case specially
			else if (v.IsBetween(this[Count - 1].Normal, this[0].Normal))
			{
				i = Count - 1;
				j = 0;
			}
			// Otherwise, do binary search
			else
			{
				j = this.BinarySearchByPredicate(
					swElem => Tools.LE(v.PolarAngle, swElem.Normal.PolarAngle), 0, Count - 1);
				i = this.NormalizeIndex(j - 1);
			}
		}

		/// <summary>
		/// Computing coordinates of the given vector in the basis of two given normals
		/// from the collection. If the indices of the basis vectors are wrong (less than 0, 
		/// greater than Coint-1, coincide), an exception is thrown
		/// </summary>
		/// <param name="v">The vector, whose coordinates should be computed</param>
		/// <param name="i">The index of the first basis vector</param>
		/// <param name="j">The index of the second basis vector</param>
		/// <param name="a">The first resultant coordinate</param>
		/// <param name="b">The second resultant coordinate</param>
		public void ConicCombination(Vector2D v, int i, int j, out double a, out double b)
		{
#if DEBUG
			if (i < 0 || i >= Count) {
				throw new IndexOutOfRangeException("ConicCombination: bad index i");
			}

			if (j < 0 || j >= Count) {
				throw new IndexOutOfRangeException("ConicCombination: bad index j");
			}

			if (i == j) {
				throw new ArgumentException("ConicCombination: indices i and j coincide");
			}
#endif

			// Computing coefficients of the conic combinations
			double
				// v = alpha * vi + beta * vj
				//  alpha * vi.x + beta * vj.x = v.x
				//  alpha * vi.y + beta * vj.y = v.y
				d = this[i].Normal.x * this[j].Normal.y - this[i].Normal.y * this[j].Normal.x,
				d1 = v.x * this[j].Normal.y - v.y * this[j].Normal.x,
				d2 = this[i].Normal.x * v.y - this[i].Normal.y * v.x;
			a = d1 / d;
			b = d2 / d;
		}

		/// <summary>
		/// Computation of the function value on a given vector
		/// </summary>
		/// <param name="v">The vector where the value should be computed</param>
		/// <param name="i">The supposed index of the clockwise boundary of the cone; 
		/// if -1, then the appropriate cone will be found</param>
		/// <param name="j">The supposed index of the couterclockwise boundary of the cone; 
		/// if -1, then the appropriate cone will be found</param>
		/// <returns>The support function</returns>
		public double FuncVal(Vector2D v, int i = -1, int j = -1)
		{
#if DEBUG
			if (this.Count <= 2) {
				throw new Exception("The function is defined by too few directions");
			}
#endif

			// Seeking the vector from the set, which is first equal or greater in the counterclockwise order
			if (i == -1 || j == -1) {
				FindCone(v, out i, out j);
			}

			// Computing coefficients of the conic combinations
			double alpha, beta;
			ConicCombination(v, i, j, out alpha, out beta);
			return alpha * this[i].Value + beta * this[j].Value;
		}

		/// <summary>
		/// Method that constructs linear combination of two support functions
		/// </summary>
		/// <param name="fa">The first function</param>
		/// <param name="fb">The second function</param>
		/// <param name="ca">The coefficient of the first function</param>
		/// <param name="cb">The coefficient of the second function</param>
		/// <param name="suspIndices">Array of indices in the resultant collection, 
		/// where vectors from the second function occurs; 
		/// if null, then this information does not accumulated</param>
		/// <param name="suspVectors">Array of vectorsof the second function; 
		/// if null, then this information does not accumulated</param>
		/// <returns>The desired linear combination of the original functions</returns>
		public static SupportFunction CombineFunctions(SupportFunction fa, SupportFunction fb,
			double ca, double cb, List<int> suspIndices = null, List<Vector2D> suspVectors = null)
		{
			List<GammaPair> res = new List<GammaPair>();

			int
				ia = fa.Count - 1, ja = 0, turna = 0,
				ib = fb.Count - 1, jb = 0, turnb = 0;
			double
				anglea = fa[0].Normal.PolarAngle + turna * 2 * Math.PI,
				angleb = fb[0].Normal.PolarAngle + turnb * 2 * Math.PI;

			if (suspIndices != null) {
				suspIndices.Clear();
			}

			if (suspVectors != null) {
				suspVectors.Clear();
			}

			while (turna == 0 || turnb == 0)
			{
				if (Tools.LT(anglea, angleb))
				{
					res.Add(new GammaPair(fa[ja].Normal, ca * fa[ja].Value + cb * fb.FuncVal(fa[ja].Normal, ib, jb)));

					turna += (ja + 1) / fa.Count;
					ia = ja;
					ja = (ja + 1) % fa.Count;
					anglea = fa[ja].Normal.PolarAngle + turna * 2 * Math.PI;
				}
				else if (Tools.GT(anglea, angleb))
				{
					if (suspIndices != null) {
						suspIndices.Add(res.Count);
					}

					if (suspVectors != null) {
						suspVectors.Add(fb[jb].Normal);
					}

					res.Add(new GammaPair(fb[jb].Normal, ca * fa.FuncVal(fb[jb].Normal, ia, ja) + cb * fb[jb].Value));

					turnb += (jb + 1) / fb.Count;
					ib = jb;
					jb = (jb + 1) % fb.Count;
					angleb = fb[jb].Normal.PolarAngle + turnb * 2 * Math.PI;
				}
				else
				{
					if (suspIndices != null) {
						suspIndices.Add(res.Count);
					}

					if (suspVectors != null) {
						suspVectors.Add(fb[jb].Normal);
					}

					res.Add(new GammaPair(fa[ja].Normal, ca * fa[ja].Value + cb * fb[jb].Value));

					turna += (ja + 1) / fa.Count;
					ia = ja;
					ja = (ja + 1) % fa.Count;
					anglea = fa[ja].Normal.PolarAngle + turna * 2 * Math.PI;

					turnb += (jb + 1) / fb.Count;
					ib = jb;
					jb = (jb + 1) % fb.Count;
					angleb = fb[jb].Normal.PolarAngle + turnb * 2 * Math.PI;
				}
			}

			return new SupportFunction(res, false);
		}

		/// <summary>
		/// Auxiliary type: element of double-linked list describing collection 
		/// of pairs passed the check procedure up to the corrent instant
		/// </summary>
		private struct PairInfo
		{
			/// <summary>
			/// Flag showing whether the element passed the check procedure
			/// </summary>
			public bool isValid;

			/// <summary>
			/// Index of the next valid element in the cyclic list 
			/// </summary>
			public int next;

			/// <summary>
			/// Index of the previous valid element in the cyclic list 
			/// </summary>
			public int prev;
		}

		/// <summary>
		/// Method supplement for the convexification procedure, which checks a triple of pairs
		/// for validity of the central element of the triple with respect to the extreme ones.
		/// It is assumed that normals of extreme elements of the triple are not parallel
		/// </summary>
		/// <param name="lm">Index of the clockwise neighbor</param>
		/// <param name="lc">Index of the pair to be checked</param>
		/// <param name="lp">Index of the counterclockwise neighbor</param>
		/// <returns>true, if the central pair is valid; false, otherwise</returns>
		protected bool CheckTriple(int lm, int lc, int lp) => SupportFunction.CheckTriple(this[lm], this[lc], this[lp]);

		/// <summary>
		/// Method supplement for the convexification procedure, which checks a triple of pairs
		/// for validity of the central element of the triple with respect to the extreme ones.
		/// </summary>
		/// <param name="pm">The clockwise neighbor</param>
		/// <param name="pc">The pair to be checked</param>
		/// <param name="pp">The counterclockwise neighbor</param>
		/// <returns>true, if the central pair is valid; false, otherwise</returns>
		/// <remarks>
		/// The algorithm is the following:
		///  - if the normals of the extreme elements of the triple are parallel,
		///    that is they cannot define any vertex, then the middle element of the triple
		///    is valid if the extreme elements are not compatible;
		///  - otherwise:
		///  - find the point of intersection of lines defined by the first and second
		///    elements of the triple (they are not parallel - by definition of the support function)
		///  - to allow to the middle element to be valid, the point should strictly belong
		///    to the semiplane defined by the third element of the triple
		///</remarks>
		protected static bool CheckTriple(GammaPair pm, GammaPair pc, GammaPair pp)
		{
			if (Tools.EQ(pm.Normal.x, -pp.Normal.x) && Tools.EQ(pm.Normal.y, -pp.Normal.y)) {
				return Tools.GE(pm.Value, -pp.Value);
			}

			Point2D p = GammaPair.CrossPairs(pm, pc);
			return Tools.LT(pp.Normal * (Vector2D)p, pp.Value);
		}

		/// <summary>
		/// Method that convexifies the current piecewise-linear positively homogeneous function
		/// taking into account information about places of possible violation of convexity
		/// (given as the subtrahend support function)
		/// </summary>
		/// <param name="f">Function to be convexified</param>
		/// <param name="suspIndices">Indices of the pair, around which normals the convexity can be violated</param>
		/// <returns>The resultant convex function; if the convex hull is improper function,
		/// the result equals null</returns>
		public SupportFunction ConvexifyFunctionWithInfo(List<int> suspIndices)
		{
			// If there is no suspicious elements, do nothing
			if (suspIndices.Count == 0) {
				return this;
			}

			int i;

			// Creating the double linked list
			PairInfo[] list = new PairInfo[Count];
			list[0].next = 1; list[0].prev = Count - 1; list[0].isValid = true;
			for (i = 1; i < Count - 1; i++)
			{
				list[i].next = i + 1; list[i].prev = i - 1; list[i].isValid = true;
			}
			list[Count - 1].next = 0; list[Count - 1].prev = Count - 2; list[Count - 1].isValid = true;

			// Stack of suspicious indices
			Stack<int> susp = new Stack<int>(suspIndices);
			double da;

			while (susp.Count > 0)
			{
				i = susp.Pop();
				if (list[i].isValid && !CheckTriple(list[i].prev, i, list[i].next))
				{
					// Mark the i-th element as invalid
					list[i].isValid = false;

					// Exclude the i-th element from the list (by rearranging links)
					list[list[i].next].prev = list[i].prev;
					list[list[i].prev].next = list[i].next;

					// Check the angle between the new neighbors
					if (Tools.LT(this[list[i].prev].Normal.PolarAngle, this[list[i].next].Normal.PolarAngle)) {
						da = this[list[i].next].Normal.PolarAngle - this[list[i].prev].Normal.PolarAngle;
					} else {
						da = 2 * Math.PI + this[list[i].next].Normal.PolarAngle - this[list[i].prev].Normal.PolarAngle;
					}

					if (Tools.GE(da, Math.PI)) {
						return null;
					}

					// If the angle is OK, add the neighbors to the suspicious collection
					susp.Push(list[i].prev);
					susp.Push(list[i].next);
				}
			}

			// If we reach this point, the convexification is successfully finished
			// The remained pairs are marked in the list
			// Put them to the resultant function
			List<GammaPair> res = new List<GammaPair>();
			for (i = 0; i < Count; i++) {
				if (list[i].isValid) {
					res.Add(this[i]);
				}
			}

			return new SupportFunction(res, false);
		}
	}

	/// <summary>
	/// Class of a convex polygon
	/// </summary>
	public class ConvexPolygon : BasicPolygon
	{
		#region Additional data structures and properties
		/// <summary>
		/// The storage for the dual description of the polygon
		/// </summary>
		private SupportFunction _sf;

		/// <summary>
		/// Property for getting the dual description of the polygon
		/// </summary>
		public SupportFunction SF
		{
			get
			{
				if (_sf == null) {
					ComputeSF();
				}

				return _sf;
			}
			protected set => _sf = value;
		}

		/// <summary>
		/// Property to get the only contour of the convex polygon
		/// </summary>
		public Polyline Contour
		{
			get
			{
				if (_contours == null) {
					ComputeContours();
				}

				return _contours[0];
			}
		}

		/// <summary>
		/// Storage for the square of the polygon
		/// </summary>
		protected double? _square = null;

		/// <summary>
		/// Property of the square of the polygon.
		/// Computes the square if necessary
		/// </summary>
		public double Square
		{
			get
			{
				if (!_square.HasValue) {
					GenerateTriangleWeights();
				}

				return _square.Value;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor on the basis of list of vertices represented as two-dimensional points
		/// </summary>
		public ConvexPolygon(IEnumerable<Point2D> vs, bool ToConvexify = false)
			: base(ToConvexify ? Convexification.ArcHull2D(vs.ToList()) : vs.ToList()) =>
			_sf = null;

		/// <summary>
		/// Constructor on the basis of list of vertices represented as multidimensional points
		/// </summary>
		public ConvexPolygon(IEnumerable<Point> vs, bool ToConvexify = false)
			: base(ToConvexify ?
								Convexification.ArcHull2D(vs.Select(p => (Point2D)p).ToList()) :
								vs.Select(p => (Point2D)p).ToList()) =>
			_sf = null;

		/// <summary>
		/// Constructor on the basis of the support function. 
		/// All other data will be computed in the lazy regime
		/// </summary>
		/// <param name="sf">The support function</param>
		public ConvexPolygon(SupportFunction sf)
			: base() =>
			_sf = sf;
		#endregion

		#region Internal methods
		/// <summary>
		/// On demand computation of the dual description of the polygon on the basis
		/// of the array of vertices (which, in its turn, can be computed on the basis 
		/// of contour of the polygon)
		/// </summary>
		private void ComputeSF()
		{
			if (_contours != null) {
				_sf = new SupportFunction(_contours[0].Vertices);
			} else if (_vertices != null)
			{
				ComputeContours();
				_sf = new SupportFunction(_contours[0].Vertices, false);
			}
			else {
				throw new Exception("Cannot construct dual description of a convex polygon: neither vertices, nor contours are initialized");
			}
		}

		/// <summary>
		/// Compute the contour on the basis of either list of vertices, or the dual description
		/// </summary>
		protected override void ComputeContours()
		{
			if (_sf != null)
			{
				int i, j;
				List<Point2D> ps = new List<Point2D>();
				for (i = 0, j = 1; i < _sf.Count; i++, j = (j + 1) % _sf.Count) {
					ps.Add(GammaPair.CrossPairs(_sf[i], _sf[j]));
				}

				List<Point2D> ps1 = Convexification.ArcHull2D(ps);

				_contours = new List<Polyline>();
				_contours.Add(new Polyline(ps1, PolylineOrientation.Counterclockwise, false, false));
				if (ps1.Count != ps.Count) {
					ComputeSF();
				}
			}
			else if (_vertices != null) {
				base.ComputeContours();
			}
		}
		#endregion

		#region Convex polygon utilities
		/// <summary>
		/// Method checking whether this polygon contains a given point
		/// </summary>
		/// <param name="p">The point to be checked</param>
		/// <returns>true, if the point is inside the polygon; false, otherwise</returns>
		public override bool Contains(Point2D p)
		{
			// Special case: the point coinsides with the initial vertex of the polygon
			if (p.Equals(Contour[0])) {
				return true;
			}

			Vector2D vp = p - Contour[0];

			// If the point is outside the cone (v0,p1);(v0,vn), then it is outside the polygon
			if (!vp.IsBetween(Contour[1] - Contour[0],
												Contour[Contour.Count - 1] - Contour[0])) {
				return false;
			}

			int ind = Contour.Vertices.BinarySearchByPredicate(
				vert => Tools.GE(vp ^ (vert - Contour[0])), 1, Contour.Count - 1);

			// The point is on the ray starting at v0 and passing through p1.
			// Check distance
			if (ind == 1) {
				return Tools.LE(vp.Length, (Contour[1] - Contour[0]).Length);
			}

			// The point is somewhere inside the polygon cone.
			// The final decision is made on the basis of support function calculation
			else
			{
				Vector2D norm = (Contour[ind] - Contour[ind - 1]).TurnCW();
				return Tools.LE(norm * (Vector2D)p, norm * (Vector2D)Contour[ind - 1]);
			}
		}

		/// <summary>
		/// Get elements of the polygon extremal on the given vector. 
		/// If the extremal elements consist of one point only, this point is returned in p1 
		/// and p2 is null. If the extremal elements give a segment, the enpoint of this segment
		/// are returned in p1 and p2 (in some order)
		/// </summary>
		/// <param name="direction">A vector defining the direction</param>
		/// <param name="p1">One extremal point</param>
		/// <param name="p2">Another extremal point</param>
		public void GetExtremeElements(Vector2D direction, out Point2D p1, out Point2D p2)
		{
			int i, j;
			SF.FindCone(direction, out i, out j);

			p1 = GammaPair.CrossPairs(SF[i], SF[j]);
			// A vector co-directed with the given one is found
			if (Tools.EQ(direction.PolarAngle, SF[j].Normal.PolarAngle)) {
				p2 = GammaPair.CrossPairs(SF[j], SF.GetAtCyclic(j + 1));
			}
			// The given vector is inside some cone
			else {
				p2 = null;
			}
		}

		/// <summary>
		/// Method auxiliary for the methods of computation the square of the polygon
		/// and generating a random point in the polygon
		/// </summary>
		protected void GenerateTriangleWeights()
		{
			int i;
			triangleWeights = new List<double>(Contour.Count - 1);
			for (i = Contour.Count - 1; i > 0; i--) {
				triangleWeights.Add(0);
			}

			for (i = 1; i < Contour.Count - 1; i++)
			{
				triangleWeights[i] = triangleWeights[i - 1] +
					(0.5 * (Contour[i] - Contour[0]) ^ (Contour[i + 1] - Contour[0]));
			}

			_square = triangleWeights[triangleWeights.Count - 1];
		}

		/// <summary>
		/// Generates data of a point uniformly distributed in the polygon:
		///  - choose a triangle of type (v_0,v_i,v_{i+1}) according its square;
		///  - choose a point in the triangle according to the algorithm from
		///    https://math.stackexchange.com/questions/18686/uniform-random-point-in-triangle
		///    P = (1-sqrt(r1))v_0 + (sqrt(r1)(1-r2))B + (r2*sqrt(r1))C, r1,r2 \in U[0,1]
		/// </summary>
		/// <param name="trInd">Index of the second vertex of the chosen triangle</param>
		/// <param name="a">The weight of the vertex v_0</param>
		/// <param name="b">The weight of the vertex v_i</param>
		/// <param name="c">The weight of the vertex v_{i+1}</param>
		/// <param name="rnd">Random generator to be used; if null, the internal generator of the polygon is used</param>
		public void GenerateDataForRandomPoint(
			out int trInd, out double a, out double b, out double c, Random rnd = null)
		{
			// Generate the list triangle weights, if necessary
			if (triangleWeights == null) {
				GenerateTriangleWeights();
			}

			if (rnd == null) {
				rnd = MyRnd;
			}

			double s = rnd.NextDouble() * _square.Value;
			trInd = triangleWeights.BinarySearch(s, new Tools.DoubleComparer(Tools.Eps));
			if (trInd < 0) {
				trInd = ~trInd;
			}

			double r1 = Math.Sqrt(rnd.NextDouble()), r2 = rnd.NextDouble();
			a = 1 - r1;
			b = r1 * (1 - r2);
			c = r2 * r1;
		}

		/// <summary>
		/// Auxiliary structure for the method of generating a random point in the polygon;
		/// it contains progressive sums of squares of triangle of type (v_0,v_i,v_{i+1}).
		/// It initializes at the first call to generation of a point and is used in further calls
		/// </summary>
		protected List<double> triangleWeights = null;

		/// <summary>
		/// Internal random generator
		/// </summary>
		protected Random _myRnd = null;

		/// <summary>
		/// Internal property for taking the internal random generator initializing it if necessary
		/// </summary>
		protected Random MyRnd
		{
			get
			{
				if (_myRnd == null) {
					_myRnd = new Random();
				}

				return _myRnd;
			}
		}

		/// <summary>
		/// Generates data of a point uniformly distributed in the polygon.
		/// Calls to <see cref="GenerateDataForRandomPoint"/> to generate data of the point,
		/// computes the point and returns it
		/// </summary>
		/// <param name="rnd">The random generator to be used; if null, the internal generator of the polygon is used</param>
		/// <returns>The generated point</returns>
		public Point2D GenerateRandomPoint(Random rnd = null)
		{
			int i;
			double a, b, c;
			GenerateDataForRandomPoint(out i, out a, out b, out c, rnd);
			return Point2D.LinearCombination(Vertices[0], a, Vertices[i], b, Vertices[i + 1], c);
		}

		/// <summary>
		/// Method computing signs of dot products of vectors from the given vertex
		/// to the neighbor ones and from the vertex to the given point
		/// </summary>
		/// <param name="x">The given point</param>
		/// <param name="curPointInd">The index of the vertex</param>
		/// <param name="sl">Sign of the dot product of vectors from the vertex to the previous vertex and to the given point</param>
		/// <param name="sr">Sign of the dot product of vectors from the vertex to the next vertex and to the given point</param>
		/// <returns>true, if the point is the nearest, that is, if both <see cref="sd"/> and <see cref="si"/> are non-positive</returns>
		protected bool ComputePointSigns(Point2D x, int curPointInd, out int sd, out int si)
		{
			Point2D
				prevVert = Contour.Vertices.GetAtCyclic(curPointInd - 1),
				curVert = Contour.Vertices.GetAtCyclic(curPointInd),
				nextVert = Contour.Vertices.GetAtCyclic(curPointInd + 1);
			Vector2D toPoint = x - curVert;
			sd = Tools.CMP((prevVert - curVert) * toPoint);
			si = Tools.CMP((nextVert - curVert) * toPoint);
			return sd <= 0 && si <= 0;
		}

		/// <summary>
		/// Method returning the point of the polygon nearest to the given point
		/// </summary>
		/// <param name="p">The given point</param>
		/// <returns>The nearest point of the polygon</returns>
		public Point2D NearestPoint(Point2D p) => throw new NotImplementedException();
		#endregion

		#region Operators
		/// <summary>
		/// Operator of algebraic (Minkowski) sum of two convex polygons
		/// </summary>
		/// <param name="cp1">The first polygon summand</param>
		/// <param name="cp2">The second polygon summand</param>
		/// <returns>The polygon sum</returns>
		public static ConvexPolygon operator +(ConvexPolygon cp1, ConvexPolygon cp2)
		{
			SupportFunction sf = SupportFunction.CombineFunctions(cp1.SF, cp2.SF, 1, 1);
			return new ConvexPolygon(sf);
		}

		/// <summary>
		/// Operator of geometric (Minkowski) difference of two convex polygons
		/// </summary>
		/// <param name="cp1">The polygon minuend</param>
		/// <param name="cp2">The polygon subtrahend</param>
		/// <returns>The polygon difference; if the difference is empty, null is returned</returns>
		public static ConvexPolygon operator -(ConvexPolygon cp1, ConvexPolygon cp2)
		{
			List<int> suspInds = new List<int>();
			SupportFunction sf =
				SupportFunction.CombineFunctions(cp1.SF, cp2.SF, 1, -1, suspInds)
					.ConvexifyFunctionWithInfo(suspInds);
			if (sf == null) {
				return null;
			} else {
				return new ConvexPolygon(sf);
			}
		}
		#endregion
	}
}
