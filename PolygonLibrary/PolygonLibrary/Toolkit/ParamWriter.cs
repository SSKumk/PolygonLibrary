using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : class, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class ParamWriter : StreamWriter {

    public ParamWriter(string filePath) : base(filePath) { }

    public void WriteNumber<T>(string fieldName, T number, string? format = null) where T : INumber<T> {
      Write(fieldName + " = " + number.ToString(format, CultureInfo.InvariantCulture));
      WriteLine(';');
    }

    public void WriteString(string fieldName, string mes) {
      WriteLine($"{fieldName} = \"{mes}\";");
    }

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

  }

}
