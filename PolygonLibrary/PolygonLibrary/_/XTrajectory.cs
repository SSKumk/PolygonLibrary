using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Robust
{
  /// <summary>
  /// CLass for XML-presentation of a trajectory data
  /// </summary>
  public class XTrajectory
  {
    #region Variable definition
    [XmlElement ("Signature")]
    public string Signature = SignClass.GetTrajectorySignature ();

    [XmlElement ("ExampleName")]
    public string ExampleName;

    /// <summary>
    /// Time instants
    /// </summary>
    public List<double> time;

    /// <summary>
    /// The first pursuer lateral coordinate
    /// </summary>
    public List<double> z1;

    /// <summary>
    /// The second pursuer lateral coordinate
    /// </summary>
    public List<double> z2;

    /// <summary>
    /// The evader lateral coordinate 
    /// </summary>
    public List<double> ze;

    /// <summary>
    /// The first pursuer lateral velocity
    /// </summary>
    public List<double> vp1;

    /// <summary>
    /// The second pursuer lateral velocity
    /// </summary>
    public List<double> vp2;

    /// <summary>
    /// Lateral velocity of the evader
    /// </summary>
    public List<double> ve;

    /// <summary>
    /// Lateral acceleration of the first pursuer
    /// </summary>
    public List<double> ap1;

    /// <summary>
    /// Lateral acceleration of the second pursuer
    /// </summary>
    public List<double> ap2;

    /// <summary>
    /// Lateral acceleration of the evader
    /// </summary>
    public List<double> ae;

    /// <summary>
    /// Realization of the first pursuer control
    /// </summary>
    public List<double> u1;

    /// <summary>
    /// Realization of the second pursuer control
    /// </summary>
    public List<double> u2;

    /// <summary>
    /// Realization of the evader control
    /// </summary>
    public List<double> v;

    /// <summary>
    /// The trajectory in two-dimensional equivalent coordinates
    /// </summary>
    public List<PointF> x;

    /// <summary>
    /// The trajectory of the first pursuer in two-dimensional original coordinates
    /// </summary>
    public List<PointF> z1full;

    /// <summary>
    /// The trajectory of the second pursuer in two-dimensional original coordinates
    /// </summary>
    public List<PointF> z2full;

    /// <summary>
    /// The trajectory of the evader in two-dimensional original coordinates
    /// </summary>
    public List<PointF> zefull;

    #endregion

    /// <summary>
    /// Empty constructor
    /// </summary>
    public XTrajectory ()
    {
      time = new List<double> ();
      z1 = new List<double> ();
      z2 = new List<double> ();
      ze = new List<double> ();
      vp1 = new List<double> ();
      vp2 = new List<double> ();
      ve = new List<double> ();
      ap1 = new List<double> ();
      ap2 = new List<double> ();
      ae = new List<double> ();
      u1 = new List<double> ();
      u2 = new List<double> ();
      v = new List<double> ();
      x = new List<PointF> ();
      z1full = new List<PointF> ();
      z2full = new List<PointF> ();
      zefull = new List<PointF> ();
    }

    /// <summary>
    /// Filling constructor
    /// </summary>
    /// <param name="exName">Name of the example</param>
    /// <param name="t">Time instants</param>
    /// <param name="zvec">Full phase vector of original one-dmensional games</param>
    /// <param name="zeq">The trajectory in the equivalent coordinates</param>
    /// <param name="uc1">The first pursuer control</param>
    /// <param name="uc2">The second pursuer control</param>
    /// <param name="vc">The evder control</param>
    /// <param name="xp1">The trajectory of the first pursuer in the original two-dimensional coordinates</param>
    /// <param name="xp2">The trajectory of the second pursuer in the original two-dimensional coordinates</param>
    /// <param name="xe">The trajectory of the evader in the original two-dimensional coordinates</param>
    public XTrajectory (string exName, Time t, double[][] zvec, PointF[] zeq, 
      float[] uc1, float[] uc2, float[] vc, PointF[] xp1, PointF[] xp2, PointF[] xe)
    {
      ExampleName = exName;
      time = new List<double> ();
      z1 = new List<double> ();
      z2 = new List<double> ();
      ze = new List<double> ();
      vp1 = new List<double> ();
      vp2 = new List<double> ();
      ve = new List<double> ();
      ap1 = new List<double> ();
      ap2 = new List<double> ();
      ae = new List<double> ();
      u1 = new List<double> ();
      u2 = new List<double> ();
      v = new List<double> ();
      for (int i = 0; i < t.Count; i++)
      {
        time.Add (t[i]);
        z1.Add (zvec[i][0]);
        z2.Add (zvec[i][1]);
        ze.Add (zvec[i][2]);
        vp1.Add (zvec[i][3]);
        vp2.Add (zvec[i][4]);
        ve.Add (zvec[i][5]);
        ap1.Add (zvec[i][6]);
        ap2.Add (zvec[i][7]);
        ae.Add (zvec[i][8]);
        u1.Add (uc1[i]);
        u2.Add (uc2[i]);
        v.Add (vc[i]);
      }

      x = new List<PointF> (zeq);
      z1full = new List<PointF> (xp1);
      z2full = new List<PointF> (xp2);
      zefull = new List<PointF> (xe);
    }

  }
}
