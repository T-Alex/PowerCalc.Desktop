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
                string key = line.Remove(posEqual, line.Length - posEqual);
                string value = line.Substring(posEqual + 1);

                if (value.Length > 0)
                {
                    inputs.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Read the input file and parse its lines into the Inputs[] list.
        /// </summary>
        /// <param name="pathname"></param>
        public static IDictionary<string, string> ReadInputValues()
        {
            IDictionary<string, string> inputs = new Dictionary<string, string>();

            // process every line in the file
            for (String line = Console.ReadLine(); !String.IsNullOrEmpty(line); line = Console.ReadLine())
            {
                AddInputLine(inputs, line.Trim());
            }

            // check the input encoding
            string encName = GetValue(inputs, "ENCODING");

            if (encName != String.Empty && encName != "UTF8")
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
                Console.InputEncoding = _fileEncoding;
                IDictionary<string, string> inputs = ReadInputValues();

                Console.Write(KeyGenerator.Generate(inputs));
                Environment.ExitCode = (int)KeyGeneratorReturnCode.ERC_SUCCESS;
            }
            catch (KeyGeneratorException e)
            {
                // set the exit code to the ERC of the exception object
                Console.Error.WriteLine(e.Message);
                Environment.ExitCode = (int)e.ERC;
            }
            catch (Exception e)
            {
                // for general exceptions return ERC_ERROR
                Console.Error.WriteLine(e.Message);
                Environment.ExitCode = (int)KeyGeneratorReturnCode.ERC_ERROR;
            }
        }

        #endregion
    }
}