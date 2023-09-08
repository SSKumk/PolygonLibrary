// using NUnit.Framework;
// using static CGLibrary.Geometry<ddouble, DConvertor>;
//
//
// namespace DoubleDoubleTests;
//
// [TestFixture]
// public class SubClassesTests {
//
//   [Test]
//   public void Test1() {
//     List<Point> Ps = new List<Point>()
//       {
//         new Point(new ddouble[] { 1, 2 })
//       , new Point(new ddouble[] { 3, 4 })
//       , new Point(new ddouble[] { 7, 4 })
//       , new Point(new ddouble[] { 2, 3 })
//       , new Point(new ddouble[] { 11.0/3, 10.0/3 })
//       };
//
//     IEnumerable<SubPoint> S                  = Ps.Select(s => new SubPoint(s, new SubPoint(s, null, s), s));
//     List<Point2D>         convexPolygon2D    = Convexification.ArcHull2D(S.Select(s => new SubPoint2D(s)));
//     IEnumerable<SubPoint> subConvexPolygon2D = convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint);
//   }
//
// }
