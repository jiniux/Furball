using System;
using System.Security.Cryptography;

namespace Furball.Engine.Engine.Helpers {
    public class CryptoHelper {
        private static MD5 _md5 = new MD5CryptoServiceProvider();
        private static SHA256 _sha256 = new SHA256CryptoServiceProvider();

        public static string GetMd5(byte[] bytes) {
            byte[] hash = _md5.ComputeHash(bytes);

            string hashString = string.Empty;
            for (int index = 0; index < hash.Length; index++) {
                byte x = hash[index];
                hashString += $"{x:x2}";
            }

            return hashString;
        }

        public static string GetSha256(byte[] bytes) {
            byte[] hash = _sha256.ComputeHash(bytes);

            string hashString = string.Empty;
            for (int index = 0; index < hash.Length; index++) {
                byte x = hash[index];
                hashString += $"{x:x2}";
            }

            return hashString;
        }

        public static byte Crc8(byte[] data, int dataLimit) {
            byte sum = 0;
            unchecked // Let overflow occur without exceptions
            {
                for (int index = 0; index < Math.Min(dataLimit, data.Length); index++) {
                    byte b = data[index];
                    sum += b;
                }
            }
            return sum;
        }

        public static short Crc16(byte[] data, int dataLimit) {
            short sum = 0;
            unchecked // Let overflow occur without exceptions
            {
                for (int index = 0; index < Math.Min(dataLimit, data.Length); index++) {
                    byte b = data[index];
                    sum += b;
                }
            }
            return sum;
        }

        public static int Crc32(byte[] data, int dataLimit) {
            int sum = 0;
            unchecked // Let overflow occur without exceptions
            {
                for (int index = 0; index < Math.Min(dataLimit, data.Length); index++) {
                    byte b = data[index];
                    sum += b;
                }
            }
            return sum;
        }

        public static long Crc64(byte[] data, int dataLimit) {
            long sum = 0;
            unchecked // Let overflow occur without exceptions
            {
                for (int index = 0; index < Math.Min(dataLimit, data.Length); index++) {
                    byte b = data[index];
                    sum += b;
                }
            }
            return sum;
        }
    }
}
