using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;

namespace CarTrackAPI.Utilities
{
    public class HexEncoding
    {
        public HexEncoding()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static int GetByteCount(string hexString)
        {
            int numHexChars = 0;
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    numHexChars++;
            }
            // if odd number of characters, discard last character
            if (numHexChars % 2 != 0)
            {
                numHexChars--;
            }
            return numHexChars / 2; // 2 characters per byte
        }
        /// <summary>
        /// Creates a byte array from the hexadecimal string. Each two characters are combined
        /// to create one byte. First two hexadecimal characters become first byte in returned array.
        /// Non-hexadecimal characters are ignored. 
        /// </summary>
        /// <param name="hexString">string to convert to byte array</param>
        /// <param name="discarded">number of characters in string ignored</param>
        /// <returns>byte array, in the same left-to-right order as the hexString</returns>
        public static byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            string newString = "";
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    newString += c;
                else
                    discarded++;
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = newString.Substring(0, newString.Length - 1);
            }

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new Char[] { newString[j], newString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }
        public static string ToString(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i].ToString("X2");
            }
            return hexString;
        }
        /// <summary>
        /// Determines if given string is in proper hexadecimal string format
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static bool InHexFormat(string hexString)
        {
            bool hexFormat = true;

            foreach (char digit in hexString)
            {
                if (!IsHexDigit(digit))
                {
                    hexFormat = false;
                    break;
                }
            }
            return hexFormat;
        }

        /// <summary>
        /// Returns true is c is a hexadecimal digit (A-F, a-f, 0-9)
        /// </summary>
        /// <param name="c">Character to test</param>
        /// <returns>true if hex digit, false if not</returns>
        public static bool IsHexDigit(Char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = Char.ToUpper(c);
            numChar = Convert.ToInt32(c);
            if (numChar >= numA && numChar < (numA + 6))
                return true;
            if (numChar >= num1 && numChar < (num1 + 10))
                return true;
            return false;
        }
        /// <summary>
        /// Converts 1 or 2 character string into equivalant byte value
        /// </summary>
        /// <param name="hex">1 or 2 character string</param>
        /// <returns>byte</returns>
        private static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }
    }
    public static class CryptoUtility
    {
        public static string Encrypt(string key, string data, out string message)
        {
            TripleDES trippleDES = new TripleDESCryptoServiceProvider();

            string encryptedData = String.Empty;
            try
            {
                message = String.Empty;
                trippleDES.Key = StringToByte(key, 24); // convert to 24 characters - 192 bits

                trippleDES.IV = StringToByte("12345678");

                ICryptoTransform encryptor = trippleDES.CreateEncryptor(trippleDES.Key, trippleDES.IV);

                MemoryStream msEncrypt = new MemoryStream();
                CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

                // Write all data to the crypto stream and flush it.
                csEncrypt.Write(StringToByte(data), 0, StringToByte(data).Length);
                csEncrypt.FlushFinalBlock();

                // Get the encrypted array of bytes.
                byte[] encrypted = msEncrypt.ToArray();

                encryptedData = ByteToString(encrypted);

            }
            catch (Exception ex)
            {
                message = "System is unable to encrypt User Data.";
            }
            return encryptedData;
        }

        public static string Decrypt(string key, string data, out string messasge)
        {
            TripleDES trippleDES = new TripleDESCryptoServiceProvider();
            int discarded = 0;

            string decryptedData = String.Empty;
            try
            {
                messasge = String.Empty;
                trippleDES.Key = StringToByte(key, 24); // convert to 24 characters - 192 bits

                trippleDES.IV = StringToByte("12345678");

                //byte[] encrypted = StringToByte(data);
                byte[] encrypted = HexEncoding.GetBytes(data, out discarded);
                ICryptoTransform decryptor = trippleDES.CreateDecryptor(trippleDES.Key, trippleDES.IV);

                // Now decrypt the previously encrypted message using the decryptor
                MemoryStream msDecrypt = new MemoryStream(encrypted);
                CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

                decryptedData = ByteToString(csDecrypt);
            }
            catch (Exception ex)
            {
                messasge = "System is unable to decrypt User Data.";
            }
            return decryptedData;

        }

        public static string ByteToString(CryptoStream buff)
        {
            string sbinary = String.Empty;
            int b = 0;

            do
            {
                b = buff.ReadByte();
                if (b != -1) sbinary += ((char)b);

            } while (b != -1);

            return (sbinary);
        }

        public static string ByteToString(byte[] buff)
        {
            string sbinary = String.Empty;
            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }

            return (sbinary);
        }

        public static byte[] StringToByte(string stringToConvert, int length)
        {

            char[] CharArray = stringToConvert.ToCharArray();
            byte[] ByteArray = new byte[length];

            for (int i = 0; i < CharArray.Length; i++)
            {
                ByteArray[i] = Convert.ToByte(CharArray[i]);
            }

            return ByteArray;
        }

        public static byte[] StringToByte(string stringToConvert)
        {

            char[] CharArray = stringToConvert.ToCharArray();
            byte[] ByteArray = new byte[CharArray.Length];


            for (int i = 0; i < CharArray.Length; i++)
            {
                ByteArray[i] = Convert.ToByte(CharArray[i]);
            }


            return ByteArray;
        }
    }
}