using System;
using System.Collections.Generic;
using System.Numerics;
using AVLUtils;

namespace CGLibrary;

public partial class Geometry<TNum, TConv> where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {


  /// <summary>
  /// Class that provides computation of the fundamental Cauchy matrix
  /// for the given constant square matrix.
  /// The integration is made by the 4th order Runge-Kutta method
  /// with the fixed given time step
  /// </summary>
  public class CauchyMatrix
  {
    /// <summary>
    /// Comparer based on epsilon comparison.
    /// Necessary for the internal storage
    /// </summary>
    internal class TimeComparer : IComparer<TNum>
    {
      public int Compare(TNum a, TNum b) => Tools.CMP(a, b);
    }

    #region Internal data
    /// <summary>
    /// The main storage: Cauchy matrix values (the value in the dictionary)
    /// for distinct instants (the keys in the dictionary)
    /// </summary>
    private readonly AVLDictionary<TNum, Matrix> _ms;

    /// <summary>
    /// The instant, to which the initial value is connected
    /// </summary>
    private readonly TNum T;

    /// <summary>
    /// The matrix, for which the Cauchy matrix is computed
    /// </summary>
    private readonly Matrix A;

    /// <summary>
    /// Maximal time step in the integration scheme
    /// </summary>
    private readonly TNum dt;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructor of the producing object
    /// </summary>
    /// <param name="nA">A square matrix</param>
    /// <param name="nT">The final instant</param>
    /// <param name="ndt">The time step</param>
    public CauchyMatrix(Matrix nA, TNum nT, TNum ndt)
    {
#if DEBUG
      if (nA.Rows != nA.Cols) {
        throw new ArgumentException("CauchyMatrix: The given matrix should be square!");
      }
#endif
      A = nA;
      T = nT;
      dt = ndt;
      _ms = new AVLDictionary<TNum, Matrix>(new TimeComparer());
      _ms[T] = Matrix.Eye(A.Rows);
    }
    #endregion

    #region The getting functions
    /// <summary>
    /// Indexer that gives the Cauchy matrix for given instant.
    /// It is based on the GetAt function
    /// </summary>
    /// <param name="t">The time instant, at which it is necessary to get the Cauchy matrix</param>
    /// <returns>The Cauchy matrix at the given instant</returns>
    public Matrix this[TNum t] => GetAt(t);

    /// <summary>
    /// Function that gives the Cauchy matrix for given instant.
    /// It is used in the indexer
    /// </summary>
    /// <param name="t">The time instant, at which it is necessary to get the Cauchy matrix</param>
    /// <returns>The Cauchy matrix at the given instant</returns>
    private Matrix GetAt(TNum t)
    {
      if (!_ms.ContainsKey(t)) {
        ComputeAt(t);
      }

      return _ms[t];
    }

    /// <summary>
    /// Function that computes the Cauchy matrix at the given instant and puts the computed value
    /// and values at all intermediate instant to the storage.
    /// This function is called only if there is no the matrix for the given instant!
    /// </summary>
    /// <param name="t">The instant, at which the Cauchy matrix should be computed</param>
    private void ComputeAt(TNum t)
    {
      // Find the instants neighbouring to t (if they are in the storage)
      KeyValuePair<TNum, Matrix>
        leftVal = new KeyValuePair<TNum, Matrix>(),
        rightVal = new KeyValuePair<TNum, Matrix>();

      // Flags showing whether the corresponding neighbor instants are in the storage
      bool leftExists = false, rightExists = false;

      // Current instant
      TNum tCur;

      // Current matrix value
      Matrix mCur;

      // The flag set if we apply forward integration and cleared if we apply backward integration

      try { leftVal = _ms.GetReverseEnumerator(t).Current; leftExists = true; }
      catch {
        // ignored
      }

      try { rightVal = _ms.GetEnumerator(t).Current; rightExists = true; }
      catch {
        // ignored
      }

      // Setting the flag of integration direction
      bool forward = leftExists && (!rightExists || Tools.LT(TNum.Abs(leftVal.Key - t), TNum.Abs(rightVal.Key - t)));

      // Setting the initial values for the integration
      if (forward)
      {
        tCur = leftVal.Key;
        mCur = leftVal.Value;
      }
      else
      {
        tCur = rightVal.Key;
        mCur = rightVal.Value;
      }

      // Flag defining the integration loop repetition;
      // do the integration if the period is not less than the time step
      bool flag = Tools.GE(TNum.Abs(t - tCur), dt);

      // The integration loop
      if (forward)
      {
        while (flag)
        {
          mCur      =  RungeKuttaStep(mCur, -dt);
          tCur      += dt;
          _ms[tCur] =  mCur;
          flag      =  Tools.GE(TNum.Abs(t - tCur), dt);
        }
      }
      else
      {
        while (flag)
        {
          mCur      =  RungeKuttaStep(mCur, dt);
          tCur      -= dt;
          _ms[tCur] =  mCur;
          flag      =  Tools.GE(TNum.Abs(t - tCur), dt);
        }
      }
      mCur = RungeKuttaStep(mCur, t - tCur);
      _ms[t] = mCur;
    }

    /// <summary>
    /// Procedure computing result of one step of 4th order Runge-Kutta method
    /// </summary>
    /// <param name="cur">Current value of the matrix</param>
    /// <param name="curTimeStep">Time step</param>
    /// <returns>The result of integration</returns>
    private Matrix RungeKuttaStep(Matrix cur, TNum curTimeStep)
    {
      Matrix
        K1 = A * cur,
        K2 = A * (cur + (curTimeStep / Tools.Two) * K1),
        K3 = A * (cur + (curTimeStep / Tools.Two) * K2),
        K4 = A * (cur + curTimeStep * K3);
      return cur + (curTimeStep / Tools.Six) * (K1 + Tools.Two * K2 + Tools.Two * K3 + K4);
    }
    #endregion
  }

}
