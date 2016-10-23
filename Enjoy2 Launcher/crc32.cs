//Взято с сайта http://netobf.com/Examples_CRC32
using System;
using System.Text;


namespace Enjoy2_Launcher
{
    class CRCTools
    {
        /*
        static void Main(string[] args)
        {
            // Читаем файл test.txt
            System.IO.FileStream stream1 = System.IO.File.OpenRead("test.txt");
            Console.WriteLine(string.Format("{0:X}", CalculateCRC(stream1)));
            Console.ReadLine();
        }*/
        /// <summary>
        /// Вычисление CRC32
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static uint CalculateCRC(System.IO.Stream stream)
        {
            const int buffer_size = 1024;
            const uint POLYNOMIAL = 0xEDB88320;
            uint result = 0xFFFFFFFF;
            uint Crc32;
            byte[] buffer = new byte[buffer_size];
            uint[] table_CRC32 = new uint[256];
            unchecked
            {
                //
                // Инициалиазация таблицы
                //
                for (int i = 0; i < 256; i++)
                {
                    Crc32 = (uint)i;
                    for (int j = 8; j > 0; j--)
                    {
                        if ((Crc32 & 1) == 1)
                            Crc32 = (Crc32 >> 1) ^ POLYNOMIAL;
                        else
                            Crc32 >>= 1;
                    }
                    table_CRC32[i] = Crc32;
                }
                //
                // Чтение из буфера
                //
                int count = stream.Read(buffer, 0, buffer_size);
                //
                // Вычисление CRC
                //
                while (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        result = ((result) >> 8)
                              ^ table_CRC32[(buffer[i])
                              ^ ((result) & 0x000000FF)];
                    }
                    count = stream.Read(buffer, 0, buffer_size);
                }
            }
            return ~result;
        }
    }
}

