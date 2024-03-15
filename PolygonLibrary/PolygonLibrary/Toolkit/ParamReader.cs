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
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Class whose exemplar takes a data from a file and disassembles it to objects on demand
  /// </summary>
  public class ParamReader {

    /// <summary>
    /// Enumerable type of the current state of the engine
    /// </summary>
    private enum State {

      /// <summary>
      /// Reading a token
      /// </summary>
      ReadingToken

     ,

      /// <summary>
      /// Waiting for terminal colon
      /// </summary>
      ReadingTerminator

     ,

      /// <summary>
      /// State when the reader is inside a string
      /// </summary>
      ReadingString

     ,

      /// <summary>
      /// State when the token is read
      /// </summary>
      TokenRead

    };

    /// <summary>
    /// The content of the given file
    /// </summary>
    private string data;

    /// <summary>
    /// Current position in the string
    /// </summary>
    private int ind = 0;

    /// <summary>
    /// Current state of the reader
    /// </summary>
    private State state;

    /// <summary>
    /// The only constructor connecting the object to the given file
    /// </summary>
    /// <param name="fileName">The name of the file to be read</param>
    public ParamReader(string fileName) {
      StreamReader sr = new StreamReader(fileName);
      data = sr.ReadToEnd();
      sr.Close();
    }

#region Reading methods
    /// <summary>
    /// Read the next object and treat it as an integer
    /// </summary>
    /// <param name="name">Name of the object</param>
    /// <param name="term">The termination symbol; default is colon,
    /// but for reading fields of a structure another symbols can be used</param>
    /// <returns>The read integer value</returns>
    public int ReadInt(string name, char term = ';') {
      ReadNameAndPassEquivalence(name);
      state = State.ReadingToken;
      string readData = ReadToken(name, term, State.TokenRead);

      if (!int.TryParse(readData, out int res))
        throw new Exception("Cannot convert data '" + data + "' read during parsing object '" + name + "' into double");

      return res;
    }

    /// <summary>
    /// Read the next object and treat it as a double.
    /// </summary>
    /// <param name="name">Name of the object.</param>
    /// <returns>The read double value.</returns>
    public double ReadDouble(string name, char term = ';') {
      ReadNameAndPassEquivalence(name);
      state = State.ReadingToken;
      string readData = ReadToken(name, term, State.TokenRead);

      if (!double.TryParse(readData, NumberStyles.Any, CultureInfo.InvariantCulture, out double res))
        throw new Exception("Cannot convert data '" + data + "' read during parsing object '" + name + "' into double");

      return res;
    }

    /// <summary>
    /// Read the next object and treat it as a double and convert it to TNum.
    /// </summary>
    /// <param name="name">Name of the object.</param>
    /// <returns>The read double value.</returns>
    public TNum ReadDoubleAndConvertToTNum(string name, char term = ';') => TConv.FromDouble(ReadDouble(name, term));

    /// <summary>
    /// Read the next object and treat it as a string
    /// </summary>
    /// <param name="name">Name of the object</param>
    /// <returns>The read string value</returns>
    public string ReadString(string name, char term = ';') {
      ReadNameAndPassEquivalence(name);

      state = State.ReadingString;
      string readData = ReadStringToken(name);
      ReadTerminator(name, term, State.TokenRead);

      return readData;
    }

    /// <summary>
    /// Read the next object and treat it as a boolean value
    /// </summary>
    /// <param name="name">The name of the object</param>
    /// <returns>The read boolean value</returns>
    public bool ReadBoolean(string name, char term = ';') {
      ReadNameAndPassEquivalence(name);
      state = State.ReadingToken;
      string readData = ReadToken(name, term, State.TokenRead);

      bool res;

      if (!bool.TryParse(readData, out res))
        throw new Exception("Cannot convert data '" + data + "' read during parsing object '" + name + "' into boolean");

      return res;
    }

    /// <summary>
    /// Method for reading one-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of array elements: bool, int, double, string</typeparam>
    /// <param name="name">The name of the object</param>
    /// <param name="elemQnt">The number of elements in the array</param>
    /// <returns>The read array of appropriate type and size</returns>
    public T[] Read1DArray<T>(string name, int elemQnt, char term = ';') {
      Type elemType = typeof(T);

      if (!elemType.IsPrimitive && elemType != typeof(String))
        throw new Exception("Read1DArray: wrong type of array elements in Read1DArray!");

      // Reading and checking the name
      ReadNameAndPassEquivalence(name);

      IEnumerable<T>       objs = ReadArrayRow<T>(name, elemQnt);
      using IEnumerator<T> en   = objs.GetEnumerator();
      T[]                  res  = new T[elemQnt];
      for (int i = 0; i < elemQnt; i++) {
        en.MoveNext();
        res[i] = en.Current;
      }

      state = State.ReadingTerminator;
      ReadTerminator(name, term, State.TokenRead);

      return res;
    }

    /// <summary>
    /// Method for reading one-dimensional array of doubles.
    /// </summary>
    /// <param name="name">The name of the object.</param>
    /// <param name="elemQnt">The number of elements in the array</param>
    /// <returns>The read array of appropriate type and size</returns>
    public TNum[] Read1DArray_double(string name, int elemQnt, char term = ';') {
      double[] r   = Read1DArray<double>(name, elemQnt, term);
      TNum[]   res = new TNum[elemQnt];
      for (int i = 0; i < elemQnt; i++) {
        res[i] = TConv.FromDouble(r[i]);
      }

      return res;
    }

    /// <summary>
    /// Method for reading two-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of array elements: bool, int, double, string</typeparam>
    /// <param name="name">The name of the object</param>
    /// <param name="rows">Number of rows in the array</param>
    /// <param name="cols">Number of columns in the array</param>
    /// <returns>The read array of appropriate type and size</returns>
    public T[,] Read2DArray<T>(string name, int rows, int cols, char term = ';') {
      Type elemType = typeof(T);

      if (!elemType.IsPrimitive && elemType != typeof(String))
        throw new Exception("Read2DArray: wrong type of array elements in Read1DArray!");

      // Reading and checking the name
      ReadNameAndPassEquivalence(name);

      // Passing the initial '{'
      state = State.ReadingTerminator;
      ReadTerminator(name, '{', State.TokenRead);

      T[,] res = new T[rows, cols];

      for (int j = 0; j < rows; j++) {
        // Reading the coming row of the array
        IEnumerable<T>       objs = ReadArrayRow<T>(name, cols);
        using IEnumerator<T> en   = objs.GetEnumerator();
        for (int i = 0; i < cols; i++) {
          en.MoveNext();
          res[j, i] = en.Current;
        }

        // Passing the following symbol: comma or closing '}'
        state = State.ReadingTerminator;
        ReadTerminator(name, j == rows - 1 ? '}' : ',', State.TokenRead);
      }

      // Passing the final colon
      state = State.ReadingTerminator;
      ReadTerminator(name, term, State.TokenRead);

      return res;
    }

    /// <summary>
    /// Method for reading two-dimensional array of doubles.
    /// </summary>
    /// <param name="name">The name of the object.</param>
    /// <param name="rows">Number of rows in the array.</param>
    /// <param name="cols">Number of columns in the array.</param>
    /// <returns>The read array of appropriate type and size.</returns>
    public TNum[,] Read2DArrayAndConvertToTNum(string name, int rows, int cols, char term = ';') {
      double[,] r   = Read2DArray<double>(name, rows, cols, term);
      TNum[,]   res = new TNum[rows, cols];
      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
          res[i, j] = TConv.FromDouble(r[i, j]);
        }
      }

      return res;
    }
#endregion

#region Internal methods
    /// <summary>
    /// Passing one row comment
    /// </summary>
    private void PassOneRowComment() {
      if (data[ind] == '/' && data[ind + 1] == '/') {
        ind += 2;
        while (data[ind] != '\n')
          ind++;
        ind++;
      } else
        throw new Exception("There is no one row comment!");
    }

    /// <summary>
    /// Passing multirow comment
    /// </summary>
    private void PassMultiRowComment() {
      if (data[ind] == '/' && data[ind + 1] == '*') {
        ind += 2;
        while (data[ind] != '*' || data[ind + 1] != '/')
          ind++;
        ind += 2;
      } else
        throw new Exception("There is no multirow comment!");
    }

    /// <summary>
    /// Method that reads the name of the next object and passes the sign =.
    /// Finally, the read name is compared with the given name
    /// </summary>
    /// <param name="ObjectName">The planned name</param>
    private void ReadNameAndPassEquivalence(string ObjectName) {
      state = State.ReadingToken;
      string readName = ReadToken(ObjectName, '=', State.TokenRead);

      if (!ObjectName.Equals(readName))
        throw new Exception("The read name '" + readName + "' doesn't coincide with the given name '" + ObjectName + "'");
    }

    /// <summary>
    /// Method that reads the termination colon
    /// </summary>
    /// <param name="name">The planned object name (for diagnostic messages)</param>
    /// <param name="term">The expected termination symbol</param>
    /// <param name="newState">The state to be set when the terminator is read</param>
    private void ReadTerminator(string name, char term, State newState) {
      if (state != State.ReadingTerminator)
        throw new Exception("Internal: ReadTerminator() method is called in a wrong state of the reader");

      bool flagContinue = true;
      while (flagContinue) {
        switch (data[ind]) {
          case ' ':
          case '\t':
          case '\n':
          case '\r':
            ind++;

            break;

          case '/':
            switch (data[ind + 1]) {
              case '/':
                PassOneRowComment();

                break;
              case '*':
                PassMultiRowComment();

                break;
              default:
                throw new Exception("Erroneous symbol '" + data[ind] + "' during reading the terminator of the object " + name);
            }

            break;

          default:
            if (data[ind] == term) {
              ind++;
              state        = newState;
              flagContinue = false;
            } else
              throw new Exception("Erroneous symbol '" + data[ind] + "' during reading the terminator of the object " + name);

            break;
        }
      }
    }

    /// <summary>
    /// Method that reads a token and the given terminator after it
    /// </summary>
    /// <param name="name">The planned object name (for diagnostic messages)</param>
    /// <param name="term">The expected termination symbol</param>
    /// <param name="newState">The state to be set when the terminator is read</param>
    private string ReadToken(string name, char term, State newState) {
      if (state != State.ReadingToken)
        throw new Exception("Internal: ReadToken() method is called in a wrong state of the reader");

      string readData     = "";
      bool   flagContinue = true;

      while (flagContinue) {
        switch (state) {
          case State.ReadingToken:
            switch (data[ind]) {
              case ' ':
              case '\t':
              case '\n':
              case '\r':
                ind++;
                if (readData != "") {
                  state        = State.ReadingTerminator;
                  flagContinue = false;
                }

                break;

              case '/':
                switch (data[ind + 1]) {
                  case '/': {
                    PassOneRowComment();
                    if (readData != "") {
                      state        = State.ReadingTerminator;
                      flagContinue = false;
                    }

                    break;
                  }
                  case '*': {
                    PassMultiRowComment();
                    if (readData != "") {
                      state        = State.ReadingTerminator;
                      flagContinue = false;
                    }

                    break;
                  }
                  default:
                    readData += data[ind];
                    ind++;

                    break;
                }

                break;

              default:
                if (data[ind] == term) {
                  ind++;
                  state        = State.TokenRead;
                  flagContinue = false;
                } else {
                  readData += data[ind];
                  ind++;
                }

                break;
            }

            break;
        }
      }

      if (state == State.ReadingTerminator)
        ReadTerminator(name, term, newState);
      state = newState;

      return readData;
    }

    /// <summary>
    /// Method that reads a string token according rules od string definition
    /// </summary>
    /// <param name="name">The name of the object</param>
    /// <returns>The read string</returns>
    private string ReadStringToken(string name) {
      if (state != State.ReadingString)
        throw new Exception("Internal: ReadStringToken() method is called in a wrong state of the reader");

      bool   flagContinue = true;
      string readData     = "";

      state = State.ReadingTerminator;
      ReadTerminator(name, '\"', State.ReadingString);

      while (flagContinue) {
        switch (data[ind]) {
          case '\"':
            ind++;
            state        = State.ReadingTerminator;
            flagContinue = false;

            break;

          case '\\':
            readData += data[ind + 1] switch
                          {
                            '\\' => '\\'
                          , '"'  => '"'
                          , 'n'  => '\n'
                          , 'r'  => '\r'
                          , 't'  => '\t'
                          , _ => throw new Exception
                                   ("Erroneous escape symbol '\\" + data[ind + 1] + "' when reading string object '" + name + "'")
                          };
            ind += 2;

            break;

          default:
            readData += data[ind];
            ind++;

            break;
        }
      }

      return readData;
    }

    /// <summary>
    /// Method that reads a collection of objects given in '{...}' and separated by comma
    /// </summary>
    /// <typeparam name="T">The type of array elements: bool, int, double, string</typeparam>
    /// <param name="name">The name of the object</param>
    /// <param name="elemQnt">The number of elements in the array</param>
    /// <returns>Lazy enumerable of the read objects</returns>
    private IEnumerable<T> ReadArrayRow<T>(string name, int elemQnt) {
      Type elemType = typeof(T);

      if (!elemType.IsPrimitive && elemType != typeof(String))
        throw new Exception("Internal - ReadArrayRow: wrong type of array elements in Read1DArray!");

      // Passing up to the opening '{'
      state = State.ReadingTerminator;
      ReadTerminator(name, '{', State.TokenRead);

      // Reading and parsing elements
      string readData;
      if (elemType == typeof(String)) {
        for (int i = 0; i < elemQnt; i++) {
          state    = State.ReadingString;
          readData = ReadStringToken(name);

          ReadTerminator(name, i == elemQnt - 1 ? '}' : ',', State.TokenRead);

          yield return (T)(object)readData;
        }
      } else {
        TypeConverter converter = TypeDescriptor.GetConverter(elemType);

        if (converter == null)
          throw new Exception
            ("Read1DArray: strange type '" + elemType.FullName + "' of array elements - no conversion from a string!");

        for (int i = 0; i < elemQnt; i++) {
          state    = State.ReadingToken;
          readData = ReadToken(name, i == elemQnt - 1 ? '}' : ',', State.TokenRead);

          object? result = converter.ConvertFrom(null, CultureInfo.InvariantCulture, readData);

          if (result is null)
            throw new Exception
              (
               "Read1DArray: cannot convert data '" + data + "' read during parsing object '" + name + "' into '" +
               elemType.FullName + "'"
              );

          yield return (T)result;
        }
      }
    }
#endregion

  }

}
