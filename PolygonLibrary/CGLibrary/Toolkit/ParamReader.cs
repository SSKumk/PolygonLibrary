﻿using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CGLibrary.Toolkit;

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
    /// The path to the file from which data is read.
    /// </summary>
    public readonly string filePath;

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

    }

    /// <summary>
    /// The content of the given file
    /// </summary>
    private readonly string data;

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
    /// <param name="filePath">The name of the file to be read</param>
    public ParamReader(string filePath) {
      this.filePath = filePath;
      StreamReader sr = new StreamReader(filePath);
      data = sr.ReadToEnd();
      sr.Close();
    }

    /// <summary>
    /// Calculates the MD5 hash of the file after cleaning it from comments and whitespace.
    /// </summary>
    /// <returns>The MD5 hash as a hexadecimal string.</returns>
    public string GetSanitizedData() {
      string cleaned = Regex.Replace(data, "//.*", "");
      cleaned = Regex.Replace(cleaned, @"/\*.*?\*/", "", RegexOptions.Singleline);
      cleaned = Regex.Replace(cleaned, "\\s+", "");

      return cleaned;
    }

#region Reading methods
    /// <summary>
    /// Read the next object and treat it as a bool.
    /// </summary>
    /// <param name="name">The name of the field to read.</param>
    /// <returns>The boolean value read from the file.</returns>
    /// <exception cref="ArgumentException">Thrown if the value read is not "true" or "false".</exception>
    public bool ReadBool(string name) {
      ReadNameAndPassEquivalence(name);
      state = State.ReadingToken;
      ReadToken(name, ";", out string readData);

      return readData switch
               {
                 "true"  => true
               , "false" => false
               , _ => throw new ArgumentException
                        (
                         $"ParamReader.ReadBool: Only 'true' and 'false' value of the {name} is possible. Found {readData}" +
                         $"\nin file {filePath}"
                        )
               };
    }

    /// <summary>
    /// Read the next object and treat it as a double.
    /// </summary>
    /// <param name="name">Name of the object.</param>
    /// <returns>The read double value.</returns>
    public T ReadNumber<T>(string name) where T : IParsable<T> {
      ReadNameAndPassEquivalence(name);
      state = State.ReadingToken;
      ReadToken(name, ";", out string readData);

      if (!T.TryParse(readData, CultureInfo.InvariantCulture, out T? res)) {
        throw new Exception
          ("Cannot convert data '" + readData + "' read during parsing object '" + name + "' into " + typeof(T).FullName + "!");
      }

      return res;
    }

    /// <summary>
    /// Read the next object and treat it as a string
    /// </summary>
    /// <param name="name">Name of the object</param>
    /// <returns>The read string value</returns>
    public string ReadString(string name) {
      ReadNameAndPassEquivalence(name);

      state = State.ReadingString;
      string readData = ReadStringToken(name);
      ReadTerminator(name, ";");

      return readData;
    }

    /// <summary>
    /// Read the next object and treat it as a string. But don't move to the next object.
    /// </summary>
    /// <param name="name">Name of the object.</param>
    /// <returns>The read string value.</returns>
    public string PeakString(string name) {
      State  saveState = state;
      int    saveInd   = ind;
      string readData  = ReadString(name);
      state = saveState;
      ind   = saveInd;

      return readData;
    }

    /// <summary>
    /// Method for reading one-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of array elements must be IParsable: bool, int, double, string, etc.</typeparam>
    /// <param name="name">The name of the object.</param>
    /// <param name="elemQnt">The number of elements in the array.</param>
    /// <returns>The read array of appropriate type and size.</returns>
    public T[] Read1DArray<T>(string name, int elemQnt) where T : IParsable<T> {
      // Reading and checking the name
      ReadNameAndPassEquivalence(name);

      IEnumerable<T> objs = ReadArrayRow<T>(name, elemQnt);
      IEnumerator<T> en   = objs.GetEnumerator();
      T[]            res  = new T[elemQnt];
      for (int i = 0; i < elemQnt; i++) {
        en.MoveNext();
        res[i] = en.Current;
      }
      en.Dispose();

      state = State.ReadingTerminator;
      ReadTerminator(name, ";");

      return res;
    }

    /// <summary>
    /// Reads the vector from file.
    /// </summary>
    /// <param name="name">The name of the vector.</param>
    /// <returns>The read vector.</returns>
    public Vector ReadVector(string name) => new(ReadList<TNum>(name).ToArray(), false);

    /// <summary>
    /// Method for reading a list.
    /// </summary>
    /// <typeparam name="T">The type of array elements must be IParsable: bool, int, double, string, etc.</typeparam>
    /// <param name="name">The name of the object</param>
    /// <returns>The read list of appropriate type and arbitrary size.</returns>
    public List<T> ReadList<T>(string name) where T : IParsable<T> {
      // Reading and checking the name
      ReadNameAndPassEquivalence(name);

      List<T> objs = ReadArrayRow<T>(name).ToList();

      state = State.ReadingTerminator;
      ReadTerminator(name, ";");

      return objs;
    }

    /// <summary>
    /// Method for reading two-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of array elements must be IParsable: bool, int, double, string, etc.</typeparam>
    /// <param name="name">The name of the object</param>
    /// <returns>The read array of appropriate type and size</returns>
    public List<List<T>> Read2DJaggedArray<T>(string name) where T : IParsable<T> {
      // Reading and checking the name
      ReadNameAndPassEquivalence(name);

      // Passing the initial "{"
      state = State.ReadingTerminator;
      ReadTerminator(name, "{");

      List<List<T>> res = new List<List<T>>();

      char readTerm;
      bool flagContinue = true;
      while (flagContinue) {
        // Reading the coming row of the array
        List<T> objs = ReadArrayRow<T>(name).ToList();
        res.Add(objs);

        // Passing the following symbol: comma or closing '}'
        state    = State.ReadingTerminator;
        readTerm = ReadTerminator(name, ",}");
        if (readTerm == '}') {
          flagContinue = false;
        }
      }

      // Passing the final colon
      state = State.ReadingTerminator;
      ReadTerminator(name, ";");

      return res;
    }

    /// <summary>
    /// Reads a collection of vectors from a specified source.
    /// Each vector may have a different length.
    /// </summary>
    /// <param name="name">The identifier for the collection of vectors to be read.</param>
    /// <returns>A list of <see cref="Vector"/> objects.</returns>
    public List<Vector> ReadVectors(string name)
      => Read2DJaggedArray<TNum>(name).Select(l => new Vector(l.ToArray(), false)).ToList();


    /// <summary>
    /// Reads a collection of hyper planes defined by normal and constant term from a specified source.
    /// Each plane may have a different length of its normal.
    /// </summary>
    /// <param name="name">The identifier for the collection of hyper planes to be read.</param>
    /// <returns>A list of <see cref="HyperPlane"/> objects.</returns>
    public List<HyperPlane> ReadHyperPlanes(string name) {
      List<HyperPlane> HPs     = new List<HyperPlane>();
      List<List<TNum>> hp_list = Read2DJaggedArray<TNum>(name);
      foreach (List<TNum> hp in hp_list) {
        TNum[] v = new TNum[hp.Count - 1];
        hp.CopyTo(0, v, 0, hp.Count - 1);
        HPs.Add(new HyperPlane(new Vector(v, false), hp[^1]));
      }

      return HPs;
    }

    /// <summary>
    /// Method for reading a two-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of array elements must be IParsable: bool, int, double, string, etc.</typeparam>
    /// <param name="name">The name of the object.</param>
    /// <param name="rows">Number of rows in the array.</param>
    /// <param name="cols">Number of columns in the array.</param>
    /// <returns>The read array of appropriate type and size.</returns>
    public T[,] Read2DArray<T>(string name, int rows, int cols) where T : IParsable<T> {
      // Reading and checking the name
      ReadNameAndPassEquivalence(name);

      // Passing the initial '{'
      state = State.ReadingTerminator;
      ReadTerminator(name, "{");

      T[,] res = new T[rows, cols];

      for (int j = 0; j < rows; j++) {
        // Reading the coming row of the array
        IEnumerable<T> objs = ReadArrayRow<T>(name, cols);
        IEnumerator<T> en   = objs.GetEnumerator();
        for (int i = 0; i < cols; i++) {
          en.MoveNext();
          res[j, i] = en.Current;
        }
        en.Dispose();

        // Passing the following symbol: comma or closing '}'
        state = State.ReadingTerminator;
        if (j == rows - 1) {
          ReadTerminator(name, "}");
        }
        else {
          ReadTerminator(name, ",");
        }
      }

      // Passing the final colon
      state = State.ReadingTerminator;
      ReadTerminator(name, ";");

      return res;
    }


    /// <summary>
    /// Reads a line of numbers from the input data.
    /// </summary>
    /// <param name="qnt">The expected number of numbers in the line.</param>
    /// <returns>An array containing the parsed numbers.</returns>
    /// <exception cref="ArgumentException">Thrown if the number of parsed numbers does not match the expected quantity.</exception>
    /// <exception cref="FormatException">Thrown if a number in the line has an invalid format.</exception>
    public TNum[] ReadNumberLine(int qnt) {
      while (ind < data.Length && "\n\r\t ".Contains(data[ind])) {
        ind++;
      }

      StringBuilder lineBuilder = new StringBuilder();
      while (ind < data.Length && !"\n\r".Contains(data[ind])) {
        lineBuilder.Append(data[ind]);
        ind++;
      }

      string line = lineBuilder.ToString();

      string[] splited = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

      if (splited.Length != qnt) {
        throw new ArgumentException
          ($"Expected {qnt} numbers, but found {splited.Length} in line: {line}" + $"\n in file {filePath}");
      }

      TNum[] result = new TNum[qnt];
      for (int i = 0; i < qnt; i++) {
        if (!TNum.TryParse(splited[i], CultureInfo.InvariantCulture, out result[i])) {
          throw new FormatException($"Invalid number format: {splited[i]} in line: {line}" + $"\n in file {filePath}");
        }
      }


      return result;
    }
#endregion

#region Internal methods
    /// <summary>
    /// Skips single-line comments (// ...).
    /// </summary>
    /// <exception cref="System.Exception">Thrown when a single-line comment is not found or doesn't end with a newline or end of file.</exception>
    private void PassOneRowComment() {
      Debug.Assert
        (
         data[ind] == '/' && data[ind + 1] == '/'
       , $"ParamReader.PassOneRowComment: // must be! Found {data[ind]}{data[ind + 1]}"
        );

      ind += 2;
      while (ind < data.Length && data[ind] != '\n') {
        ind++;
      }
    }

    /// <summary>
    /// Skips multi-line comments (/* ... */).
    /// </summary>
    /// <exception cref="System.Exception">Thrown when a multi-line comment is not found or is not closed properly.</exception>
    private void PassMultiRowComment() {
      if (data[ind] != '/' || data[ind + 1] != '*')
        throw new Exception("There is no multi-line comment." + $"\nin file {filePath}");

      ind += 2;
      while (ind < data.Length - 1) {
        if (data[ind] == '*' && data[ind + 1] == '/') {
          ind += 2;

          return;
        }
        ind++;
      }

      throw new Exception($"Multi-line comment is not closed properly" + $" in file {filePath}");
    }


    /// <summary>
    /// Method that reads the name of the next object and passes the sign =.
    /// Finally, the read name is compared with the given name
    /// </summary>
    /// <param name="objectName">The planned name</param>
    private void ReadNameAndPassEquivalence(string objectName) {
      state = State.ReadingToken;
      ReadToken(objectName, "=", out string readName);

      if (!objectName.Equals(readName))
        throw new Exception
          (
           "The read name '" + readName + "' doesn't coincide with the given name '" + objectName + "'." + $"\nin file {filePath}"
          );
    }

    /// <summary>
    /// Method that reads the termination colon
    /// </summary>
    /// <param name="name">The planned object name (for diagnostic messages)</param>
    /// <param name="term">The expected termination symbols</param>
    /// <returns>The actually read terminator.</returns>
    private char ReadTerminator(string name, string term) {
      if (state != State.ReadingTerminator)
        throw new Exception("Internal: ReadTerminator() method is called in a wrong state of the reader");

      char readTerm     = '\0';
      bool flagContinue = true;
      while (flagContinue) {
        if (ind >= data.Length) {
          throw new IndexOutOfRangeException
            (
             $"ReadTerminator: During reading the string {name}, the automaton has reached the end of the string! It can't find non of this '{term}' symbols." +
             $"\nin file {filePath}"
            );
        }
        switch (data[ind]) {
          case ' ':
          case '\t':
          case '\n':
          case '\r':
            ind++;

            break;

          case '/':
            if (ind + 1 >= data.Length) {
              throw new Exception
                (
                 $"Unexpected symbol '/' at the end of the file while reading terminator of the object {name} in file {filePath}."
                );
            }

            switch (data[ind + 1]) {
              case '/': PassOneRowComment(); break;
              case '*': PassMultiRowComment(); break;
              default:
                throw new Exception
                  (
                   $"Unexpected symbol '{data[ind]}{data[ind + 1]}' for a comment while reading terminator of the object {name}\"+$\"\nin file {filePath}. Expected '//' for single-line comment or '/*' for multi-line comment."
                  );
            }

            break;

          default:
            if (term.Contains(data[ind])) {
              readTerm = data[ind];
              ind++;
              state        = State.TokenRead;
              flagContinue = false;
            }
            else
              throw new Exception
                (
                 "Erroneous symbol '" + data[ind] + $"' during reading the terminator of the object '{name}'." +
                 $"The terminator '{term}' is expected before such a symbol." + $"\nin file {filePath}"
                );

            break;
        }
      }

      return readTerm;
    }

    /// <summary>
    /// Reads a token from the input data, up to a specified terminator character.
    /// </summary>
    /// <param name="name">The name of the parameter being read (used for error messages).</param>
    /// <param name="term">A string containing the characters that terminate the token.</param>
    /// <param name="readData">Outputs the token read from the input.</param>
    /// <returns>The terminator character that was encountered.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown if the end of the input data is reached before a terminator is found.</exception>
    /// <exception cref="Exception">Thrown if an unexpected character is encountered.</exception>
    private char ReadToken(string name, string term, out string readData) {
      if (state != State.ReadingToken)
        throw new Exception("Internal: ReadToken() method is called in a wrong state of the reader");

      StringBuilder readingData  = new StringBuilder();
      bool          flagContinue = true;
      char          readTerm     = '\0';

      while (flagContinue) {
        if (ind >= data.Length) {
          throw new IndexOutOfRangeException
            ("ReadToken: During reading the string, the automaton has reached the end of the string!");
        }
        switch (state) {
          case State.ReadingToken:
            switch (data[ind]) {
              case ' ':
              case '\t':
              case '\n':
              case '\r':
                ind++;
                if (readingData.Length > 0) {
                  state        = State.ReadingTerminator;
                  flagContinue = false;
                }

                break;

              case '/':
                if (ind + 1 >= data.Length) {
                  throw new Exception
                    (
                     $"Unexpected symbol '/' at the end of the file while reading token of the object {name}" +
                     $"\nin file {filePath}."
                    );
                }
                switch (data[ind + 1]) {
                  case '/': {
                    PassOneRowComment();
                    if (readingData.Length > 0) {
                      state        = State.ReadingTerminator;
                      flagContinue = false;
                    }

                    break;
                  }
                  case '*': {
                    PassMultiRowComment();
                    if (readingData.Length > 0) {
                      state        = State.ReadingTerminator;
                      flagContinue = false;
                    }

                    break;
                  }
                  default:
                    readingData.Append(data[ind]);
                    ind++;

                    break;
                }

                break;
              default:
                if (term.Contains(data[ind])) {
                  readTerm = data[ind];
                  ind++;
                  state        = State.TokenRead;
                  flagContinue = false;
                }
                else {
                  readingData.Append(data[ind]);

                  ind++;
                }

                break;
            }

            break;
        }
      }

      if (state == State.ReadingTerminator) {
        readTerm = ReadTerminator(name, term);
      }

      readData = readingData.ToString();

      return readTerm;
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
      ReadTerminator(name, "\"");

      while (flagContinue) {
        if (ind >= data.Length) {
          throw new Exception
            ($"The reader has reached the end of the file during the reading string '{name}'" + $"\n in file {filePath}.");
        }
        switch (data[ind]) {
          case '\"':
            ind++;
            state        = State.ReadingTerminator;
            flagContinue = false;

            break;

          case '\\':
            readData +=
              data[ind + 1] switch
                {
                  '\\' => '\\'
                , '"'  => '"'
                , 'n'  => '\n'
                , 'r'  => '\r'
                , 't'  => '\t'
                , _ => throw new Exception
                         (
                          "Erroneous escape symbol '\\" + data[ind + 1] + "' when reading string object '" + name + "'" +
                          $"\nin file {filePath}"
                         )
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
    /// <typeparam name="T">The type of array elements must be IParsable: int, double, string, etc.</typeparam>
    /// <param name="name">The name of the object</param>
    /// <param name="elemQnt"></param>
    /// <returns>Lazy enumerable of the read objects</returns>
    private IEnumerable<T> ReadArrayRow<T>(string name, int? elemQnt = null) where T : IParsable<T> {
      Type elemType = typeof(T);

      // Passing up to the opening "{"
      state = State.ReadingTerminator;
      ReadTerminator(name, "{");

      // Reading and parsing elements
      string readData;
      bool   flagContinue = true;
      char   readTerm;
      if (elemType == typeof(string)) {
        int i = 0;
        while (flagContinue) {
          state    = State.ReadingString;
          readData = ReadStringToken(name);
          readTerm = ReadTerminator(name, ",}");
          if (readTerm == '}') {
            if (elemQnt is not null) {
              if (i != elemQnt - 1) {
                throw new IndexOutOfRangeException
                  (
                   "ReadArrayRow: Tokens in the string before '}' must be " + elemQnt + ", but found only " + i + "!" +
                   $"\nin file {filePath}"
                  );
              }
            }
            flagContinue = false;
          }
          i++;

          yield return (T)(object)readData;
        }
      }
      else {
        int i = 0;
        while (flagContinue) {
          state    = State.ReadingToken;
          readTerm = ReadToken(name, ",}", out readData);
          if (readTerm == '}') {
            if (elemQnt is not null) {
              if (i != elemQnt - 1) {
                throw new IndexOutOfRangeException
                  (
                   "ReadArrayRow: Tokens in the string before '}' must be " + elemQnt + ", but found only " + i + "!" +
                   $"\nin file {filePath}"
                  );
              }
            }
            flagContinue = false;
          }
          if (!T.TryParse(readData, CultureInfo.InvariantCulture, out T? result)) {
            throw new Exception
              (
               "ReadArrayRow: Cannot convert data '" + readData + "' read during parsing object '" + name + "' into " +
               typeof(T).FullName + "!" + $"\nin file {filePath}"
              );
          }
          i++;

          yield return result;
        }
      }
    }
#endregion

  }

}
