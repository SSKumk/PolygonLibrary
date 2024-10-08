using System.Globalization;
using System.IO;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class ParamWriter : StreamWriter {

    public ParamWriter(string filePath) : base(filePath) { }

    public void WriteNumber<T>(string fieldName, T number, string? format = null) where T : INumber<T> {
      Write(fieldName + " = " + number.ToString(format, CultureInfo.InvariantCulture));
      WriteLine(';');
    }

    public void WriteString(string fieldName, string mes) { WriteLine($"{fieldName} = \"{mes}\";"); }

    public void Write1DArray<T>(string fieldName, IEnumerable<T> ar) where T : INumber<T> {
      Write(fieldName + " = {");
      Write(string.Join(',', ar));
      WriteLine("};");
    }

    public void WriteVector(string fieldName, Vector v) => Write1DArray(fieldName, v.GetAsArray());

    public void Write2DArray<T>(string fieldName, IEnumerable<IEnumerable<T>> ar2) where T : INumber<T> {
      Write(fieldName + " = {");

      int arCount = ar2.Count();
      int i       = 1;
      foreach (IEnumerable<T> ar in ar2) {
        Write("{" + string.Join(',', ar) + "}");
        if (i < arCount) {
          Write(", ");
        }
        i++;
      }

      WriteLine("};");
    }

    public void WriteVectors(string fieldName, IEnumerable<Vector> Vs) => Write2DArray(fieldName, Vs.Select(v => v.GetAsArray()));

    public void WriteHyperPlanes(string fieldName, IEnumerable<HyperPlane> HPs)
      => Write2DArray
        (
         fieldName
       , HPs.Select
           (
            hp => {
              int    dim = hp.Normal.SpaceDim;
              TNum[] ar  = new TNum[dim + 1];
              for (int i = 0; i < dim; i++) {
                ar[i] = hp.Normal[i];
              }
              ar[^1] = hp.ConstantTerm;

              return ar;
            }
           )
        );

  }

}
