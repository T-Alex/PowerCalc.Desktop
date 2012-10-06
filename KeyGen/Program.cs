//-------------------------------------------------
// Key generator for PowerCalc
//-------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Security.Cryptography;

namespace KeyGen
{
    /// <summary>
    /// This class implemens the key generator for PowerCalc.
    /// </summary>
    class Program
    {
        #region Fields

        private const int RegKeyLength = 150;

        /// <summary>
        /// Input/output file encoding.
        /// </summary>
        private static Encoding _fileEncoding = new UTF8Encoding();

        /// <summary>
        /// List of input values.
        /// </summary>
        private static SortedList _inputs = new SortedList();

        // generated key data

        /// <summary>
        /// The key for the user.
        /// </summary>
        private static string KeyResult1;

        /// <summary>
        /// The cckey for the publisher.
        /// </summary>
        private static string KeyResult2;

        private static Random _rnd = new Random();

        private static byte[] SK = new byte[]
        {
            18, 52, 75, 11, 76, 14, 195, 87
        };

        private static byte[] IV = new byte[]
        {
            246, 198, 57, 228, 138, 4, 126, 118
        };

        private static readonly string[] _secretKeys = new string[]
        {
            "gVmT19eF8TmAdg1IuJjN5AFrP0lpR9Xx2UH7boU8",
            "2PL5YLlABO09XcWryRy7iOls2mJc0yavmt30lfh3",
            "GxBF97x3zc6Xi8Up9D2c3HAiI1vC2M7IH3VD6PMR",
            "wIfv68Gd71e8aFi3Cg3D424866MVEqaNH49wqow8",
            "PlDI49u547UZI2S5kGTbWiqu2NScgusfoiH5d0Rx",
            "C54o507PIa7OwVcZO9T1900OT7E610o4b14DyQAi",
            "8uB8nUwflcFI0fWBrwIGC0u6Bvn85NiGmNuRTWla",
            "U4t2XR6Tg4VvHqThqI0ecrixAZQ7I77r090k0hCI",
            "TltPZIiQL0uN8VW7IfCjxw2k84sqi3Pf4j9QIGOr",
            "svEuV77z7j40A530f5e197yGI6bvD47k152xMM4C",
            "JItM8nVJ11v23H0DWIw2YA0CSsDmIo41wdk74lCS",
            "66THlxj9Dhj9FO3sn7D2J81kPTNqWKm088o53b2v",
            "QXECYxru7e5up3ngXC08klPGFQ6j3B2q0CmXFUJ1",
            "F2wc3X8R6vwz9m8Cum02paZ3A57ByU8ZY8m1ev5Q",
            "seRSWHfflJrmq8Ddos9JP7fk6Z4YAqsyHJ7r27xD",
            "4hE9N5A8J91qzx6f01qKS4S8bJ17mZw7Ls8zaP4D",
            "41CzYi1Nogu4zZ4vg7LF9fEbvVLsib8PUmZnUHWN",
            "2AoUh73v1pr3I9aKYaM0VzC83K280sLdN9883uh3",
            "11r3O7DEzjYgGD1nW2nUX09S7aH8WX4K3yf8d11d",
            "WMP290pIuwoXBR0ShlNY87Tf3UsEqeiEBL9v769N"
        };

        #endregion

        #region Methods

        /// <summary>
        /// Get input string values, return empty string if not defined.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            if (_inputs.ContainsKey(key))
                return _inputs[key].ToString();
            else
                return String.Empty;
        }

        /// <summary>
        /// Advanced algorithm for key generation.
        /// </summary>
        public static void GenerateKey()
        {
            SHA512 sha = new SHA512Managed();

            string regName = GetValue("REG_NAME");
            string regNameHash = SHA512Base64(regName);

            const int linHashStartIndex = 10;
            const int secretKeyStartIndex = 108;

            string regKey = GenerateRandomString(linHashStartIndex);
            regKey += regNameHash;
            regKey += GenerateRandomString(secretKeyStartIndex - regKey.Length);

            int secretKeyIndex = _rnd.Next(_secretKeys.Length);
            regKey += _secretKeys[secretKeyIndex];

            regKey += GenerateRandomString(RegKeyLength - regKey.Length);

            DESCryptoServiceProvider cipher = new DESCryptoServiceProvider();
            cipher.IV = IV;
            cipher.Key = SK;
            byte[] encRegKeyData = Encrypt(regKey, cipher);
            string encRegKey = Convert.ToBase64String(encRegKeyData);

            // result 1 - key for the customer
            KeyResult1 = encRegKey;

            // result 2 - cckey for the publisher
            KeyResult2 = regName + ": " + KeyResult1;
        }

        internal static string GenerateRandomString(int length)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int mode = _rnd.Next(0, 3);

                if (mode == 0)
                    sb.Append((Char)('A' + _rnd.Next(0, 26)));
                else if (mode == 1)
                    sb.Append((Char)('a' + _rnd.Next(0, 26)));
                else if (mode == 2)
                    sb.Append((Char)('0' + _rnd.Next(0, 10)));
            }

            return sb.ToString();
        }

        internal static string SHA512Base64(string source)
        {
            byte[] data = _fileEncoding.GetBytes(source);
            return Convert.ToBase64String(new SHA512Managed().ComputeHash(data));
        }

        internal static byte[] Encrypt(string plainText, SymmetricAlgorithm cipher)
        {
            MemoryStream ms = new MemoryStream();
            CryptoStream encStream = new CryptoStream(ms, cipher.CreateEncryptor(), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(encStream);
            sw.WriteLine(plainText);

            sw.Close();
            encStream.Close();

            byte[] buffer = ms.ToArray();

            ms.Close();
            return buffer;
        }

        /// <summary>
        /// Split a string at the first equals sign and add key/value to Inputs[].
        /// </summary>
        /// <param name="line"></param>
        public static void AddInputLine(string line)
        {
            int posEqual = line.IndexOf('=');

            if (posEqual > 0)
            {
                string akey = line.Remove(posEqual, line.Length - posEqual);
                string avalue = line.Substring(posEqual + 1);

                if (avalue.Length > 0)
                {
                    _inputs.Add(akey, avalue);
                }
            }
        }

        /// <summary>
        /// Read the input file and parse its lines into the Inputs[] list.
        /// </summary>
        /// <param name="pathname"></param>
        public static void ReadInput(string pathname)
        {
            _inputs.Clear();

            // attempt to open the input file for read-only access
            FileStream fsIn = new FileStream(pathname, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(fsIn, _fileEncoding, true);

            // process every line in the file
            for (String Line = sr.ReadLine(); Line != null; Line = sr.ReadLine())
            {
                AddInputLine(Line.Trim());
            }

            // explicitly close the StreamReader to properly flush all buffers
            sr.Close(); // this also closes the FileStream (fsIn)

            // check the input encoding
            string EncName = GetValue("ENCODING");

            if (EncName != String.Empty && EncName != "UTF8")
            {
                throw new KeyGenException("bad input encoding, expected UTF-8", KeyGenReturnCode.ERC_BAD_INPUT);
            }

            // check for valid input
            string regName = GetValue("REG_NAME");
            if (regName.Length < 8)
            {
                throw new KeyGenException("REG_NAME must have at least 8 characters", KeyGenReturnCode.ERC_BAD_INPUT);
            }
        }

        /// <summary>
        /// Write a string to an output file using the encoding specified in the input file.
        /// </summary>
        /// <param name="pathname"></param>
        /// <param name="data"></param>
        public static void WriteOutput(string pathname, string data)
        {
            // Create an instance of StreamWriter to write text to a file.
            // The using statement also closes the StreamWriter.
            FileStream fsOut = new FileStream(pathname, FileMode.Create);

            using (StreamWriter sw = new StreamWriter(fsOut, _fileEncoding))
            {
                sw.Write(data);
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("PowerCalc Key generator");

            try
            {
                if (args.Length == 3)
                {
                    Console.Write("> reading input file: ");
                    Console.WriteLine(args[0]);
                    ReadInput(args[0]);

                    Console.WriteLine("> processing ... ");
                    GenerateKey();

                    Console.WriteLine("> writing output files: ");

                    WriteOutput(args[1], KeyResult1);
                    WriteOutput(args[2], KeyResult2);
                    Environment.ExitCode = (int)KeyGenReturnCode.ERC_SUCCESS;
                }
                else
                {
                    Console.WriteLine("Usage: <input> <output1> <output2>");
                    Environment.ExitCode = (int)KeyGenReturnCode.ERC_BAD_ARGS;
                }
            }
            catch (KeyGenException e)
            {
                Console.WriteLine("* KeyGen Exception: " + e.Message);

                // set the exit code to the ERC of the exception object
                Environment.ExitCode = (int)e.ERC;

                // and write the error message to output file #1
                try
                {
                    WriteOutput(args[1], e.Message);
                }
                catch { };
            }
            catch (Exception e)
            {
                // for general exceptions return ERC_ERROR
                Environment.ExitCode = (int)KeyGenReturnCode.ERC_ERROR;
                Console.WriteLine("* CLR Exception: " + e.Message);

                // and write the error message to output file #1
                try
                {
                    WriteOutput(args[1], e.Message);
                }
                catch { };
            }

            Console.WriteLine("ExitCode: {0}", Environment.ExitCode);
        }

        #endregion
    }

    /// <summary>
    /// Possible key generator exit codes.
    /// </summary>
    public enum KeyGenReturnCode : int
    {
        // Success
        ERC_SUCCESS = 00,
        ERC_SUCCESS_BIN = 01,
        // Failure
        ERC_ERROR = 10,
        ERC_MEMORY = 11,
        ERC_FILE_IO = 12,
        ERC_BAD_ARGS = 13,
        ERC_BAD_INPUT = 14,
        ERC_EXPIRED = 15,
        ERC_INTERNAL = 16
    }

    /// <summary>
    /// Key generator exception class.
    /// </summary>
    public class KeyGenException : Exception
    {
        #region Fields

        public KeyGenReturnCode ERC;

        #endregion

        #region Constructors

        public KeyGenException(string message, KeyGenReturnCode e)
            : base(message)
        {
            ERC = e;
        }

        #endregion
    }
}