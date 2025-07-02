using System.Globalization;
using System.IO;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Provides a set of helper methods for writing structured parameters to a file.
  /// This class extends <see cref="StreamWriter"/> to produce files with a specific 'fieldName = value;' CGLibrary syntax.
  /// </summary>
  /// <remarks>
  /// As this class inherits from <see cref="StreamWriter"/>, it manages an underlying file stream and must be
  /// properly disposed of to ensure the file handle is closed. The recommended way to use this class is
  /// with a <c>using</c> statement.
  /// </remarks>
  public class ParamWriter : StreamWriter {

    /// <summary>
    /// Initializes a new instance of the <see cref="ParamWriter"/> class for the specified file path.
    /// </summary>
    /// <param name="filePath">The complete file path to write to. If the file exists, it will be overwritten; otherwise, a new file will be created.</param>
    public ParamWriter(string filePath) : base(filePath) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParamWriter"/> class for the specified file,
    /// controlling whether to append or overwrite the data.
    /// </summary>
    /// <param name="filePath">The complete file path to write to.</param>
    /// <param name="append">
    /// <c>true</c> to append data to the file;  <c>false</c> to overwrite the file.
    /// If the specified file does not exist, this parameter has no effect, and the constructor creates a new file.
    /// </param>
    public ParamWriter(string filePath, bool append) : base(filePath, append) { }

    /// <summary>
    /// Writes a named numeric value to the file in the format <c>fieldName = number;</c>.
    /// The number is formatted using <see cref="CultureInfo.InvariantCulture"/> for consistency.
    /// </summary>
    /// <example>
    /// <code>
    /// writer.WriteNumber("ScaleFactor", 1.23); // Output: ScaleFactor = 1.23;
    /// </code>
    /// </example>
    /// <typeparam name="T">The type of the number, constrained to <see cref="INumber{T}"/>.</typeparam>
    /// <param name="fieldName">The name of the parameter to write.</param>
    /// <param name="number">The numeric value to write.</param>
    /// <param name="format">An optional standard or custom numeric format string.</param>
    public void WriteNumber<T>(string fieldName, T number, string? format = null) where T : INumber<T>
      => WriteLine($"{fieldName} = {number.ToString(format, CultureInfo.InvariantCulture)};");

    /// <summary>
    /// Writes a named string value to the file in the format <c>fieldName = "value";</c>.
    /// </summary>
    /// <example>
    /// <code>
    /// writer.WriteString("ObjectName", "MyCircle"); // Output: ObjectName = "MyCircle";
    /// </code>
    /// </example>
    /// <param name="fieldName">The name of the parameter.</param>
    /// <param name="mes">The string value to write. The value will be enclosed in double quotes in the output file.</param>
    public void WriteString(string fieldName, string mes) => WriteLine($"{fieldName} = \"{mes}\";");

    /// <summary>
    /// Writes a named one-dimensional array of numeric values.
    /// The output format is <c>fieldName = {val1,val2,...};</c>.
    /// </summary>
    /// <typeparam name="T">The numeric type of the elements in the array.</typeparam>
    /// <param name="fieldName">The name of the array parameter.</param>
    /// <param name="ar">The collection of numeric values to write.</param>
    public void Write1DArray<T>(string fieldName, IEnumerable<T> ar) where T : INumber<T>
      => WriteLine($"{fieldName} = {{{string.Join(',', ar)}}};");

    /// <summary>
    /// Writes a <see cref="Vector"/> object to the file as a one-dimensional array.
    /// This is a convenience method that calls <see cref="Write1DArray{T}"/> on the vector's components.
    /// </summary>
    /// <param name="fieldName">The name of the vector parameter.</param>
    /// <param name="v">The <see cref="Vector"/> object to write.</param>
    public void WriteVector(string fieldName, Vector v) => Write1DArray(fieldName, v.V);

    /// <summary>
    /// Writes a named two-dimensional array (collection of collections) of numeric values.
    /// The output format is <c>fieldName = {{val1,val2,...},{val3,val4,...}};</c>.
    /// </summary>
    /// <typeparam name="T">The numeric type of the elements in the array.</typeparam>
    /// <param name="fieldName">The name of the 2D array parameter.</param>
    /// <param name="ar2">The collection of collections of numeric values to write.</param>
    public void Write2DArray<T>(string fieldName, IEnumerable<IEnumerable<T>> ar2) where T : INumber<T> {
      var innerStrings = ar2.Select(ar => $"{{{string.Join(',', ar)}}}");
      var finalArrayString = string.Join(", ", innerStrings);

      WriteLine($"{fieldName} = {{{finalArrayString}}};");
    }

    /// <summary>
    /// Writes a collection of <see cref="Vector"/> objects to the file as a two-dimensional array.
    /// Each vector in the collection becomes a row in the 2D array. This method calls <see cref="Write2DArray{T}"/> internally.
    /// </summary>
    /// <param name="fieldName">The name of the parameter.</param>
    /// <param name="Vs">The collection of <see cref="Vector"/> objects to write.</param>
    public void WriteVectors(string fieldName, IEnumerable<Vector> Vs)
      => Write2DArray(fieldName, Vs.Select(v => v.GetCopyAsArray()));

    /// <summary>
    /// Writes a collection of <see cref="HyperPlane"/> objects to the file as a two-dimensional array.
    /// </summary>
    /// <remarks>
    /// Each hyperplane is converted into a 1D array containing its normal vector components followed by its constant term: <c>{n1, n2, ..., nd, c}</c>.
    /// The resulting 2D array will have a row for each hyperplane, where each row has <c>d+1</c> elements, with <c>d</c> being the dimension of the space.
    /// </remarks>
    /// <param name="fieldName">The name of the parameter.</param>
    /// <param name="HPs">The collection of <see cref="HyperPlane"/> objects to write.</param>
    public void WriteHyperPlanes(string fieldName, IEnumerable<HyperPlane> HPs)
      => Write2DArray
        (
         fieldName
       , HPs.Select
           (hp
              => {
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