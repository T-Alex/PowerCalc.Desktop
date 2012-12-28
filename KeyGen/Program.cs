using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace TAlex.PowerCalc.KeyGenerator
{
    /// <summary>
    /// This class implemens the key generator for PowerCalc.
    /// </summary>
    class Program
    {
        #region Fields

        /// <summary>
        /// Input file encoding.
        /// </summary>
        private static Encoding _fileEncoding = new UTF8Encoding();

        private static readonly IKeyGenerator KeyGenerator = new PowerCalcKeyGenerator();

        #endregion

        #region Methods

        /// <summary>
        /// Get input string values, return empty string if not defined.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(IDictionary<string, string> inputs, string key)
        {
            string value;

            if (inputs.TryGetValue(key, out value))
                return value;
            else
                return String.Empty;
        }

        /// <summary>
        /// Split a string at the first equals sign and add key/value to Inputs[].
        /// </summary>
        /// <param name="line"></param>
        public static void AddInputLine(IDictionary<string, string> inputs, string line)
        {
            int posEqual = line.IndexOf('=');

            if (posEqual > 0)
            {
                string akey = line.Remove(posEqual, line.Length - posEqual);
                string avalue = line.Substring(posEqual + 1);

                if (avalue.Length > 0)
                {
                    inputs.Add(akey, avalue);
                }
            }
        }

        /// <summary>
        /// Read the input file and parse its lines into the Inputs[] list.
        /// </summary>
        /// <param name="pathname"></param>
        public static IDictionary<string, string> ReadInput(string pathname)
        {
            IDictionary<string, string> inputs = new Dictionary<string, string>();

            // attempt to open the input file for read-only access
            FileStream fsIn = new FileStream(pathname, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(fsIn, _fileEncoding, true);

            // process every line in the file
            for (String Line = sr.ReadLine(); Line != null; Line = sr.ReadLine())
            {
                AddInputLine(inputs, Line.Trim());
            }

            // explicitly close the StreamReader to properly flush all buffers
            sr.Close(); // this also closes the FileStream (fsIn)

            // check the input encoding
            string EncName = GetValue(inputs, "ENCODING");

            if (EncName != String.Empty && EncName != "UTF8")
            {
                throw new KeyGeneratorException("bad input encoding, expected UTF-8", KeyGeneratorReturnCode.ERC_BAD_INPUT);
            }

            // check for valid input
            string regName = GetValue(inputs, "REG_NAME");
            if (regName.Length < 8)
            {
                throw new KeyGeneratorException("REG_NAME must have at least 8 characters", KeyGeneratorReturnCode.ERC_BAD_INPUT);
            }

            return inputs;
        }

        public static void Main(string[] args)
        {
            try
            {
                if (args.Length == 1)
                {
                    IDictionary<string, string> inputs = ReadInput(args[0]);

                    Console.Write(KeyGenerator.Generate(inputs));
                    Environment.ExitCode = (int)KeyGeneratorReturnCode.ERC_SUCCESS;
                }
                else
                {
                    Console.WriteLine("Usage: <input> <output1> <output2>");
                    Environment.ExitCode = (int)KeyGeneratorReturnCode.ERC_BAD_ARGS;
                }
            }
            catch (KeyGeneratorException e)
            {
                // set the exit code to the ERC of the exception object
                Environment.ExitCode = (int)e.ERC;
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                // for general exceptions return ERC_ERROR
                Environment.ExitCode = (int)KeyGeneratorReturnCode.ERC_ERROR;
                Console.WriteLine(e.Message);
            }
        }

        #endregion
    }
}