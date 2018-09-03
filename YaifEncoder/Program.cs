
using AForge.Imaging.ColorReduction;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace YaifEncoder
{
    class Program
    {
        
        static List<byte> output = new List<byte>();
        static String filename = "";
        static void Add16BitInt(int a) {
            output.Add((byte)(a & 0xFF));
            output.Add((byte)((a >> 8) & 0xFF));


        }
        static void Add32BitInt(int a) {

            output.Add((byte)(a & 0xFF));
            output.Add((byte)((a >> 8) & 0xFF));
            output.Add((byte)((a >> 16) & 0xFF));
            output.Add((byte)((a >> 24) & 0xFF));



        }
        static void Add8BitInt(int a) {
            output.Add((byte)((short)a));

        }
        static void AddNibbles(int a, int b) {
            output.Add((byte)(b + (a << 4)));

        }

        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("There was no file specified.");
                filename = "i_eat_ass.png";
            }
            else {
                filename = args[0];

            }
            
            Image imageFile = Image.FromFile(filename);
            Bitmap bmp = new Bitmap(imageFile);
            
            ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
            
            Color[] colorTable = ciq.CalculatePalette(bmp, 16);
            Dictionary<String, int> colorMap = new Dictionary<String, int>();
            for (int i = 0; i < colorTable.Length; i++) {
           
                try
                {
                    colorMap.Add(colorTable[i].ToString(), i);
                }
                catch(System.ArgumentException e) {



                }

              
              
            }


            Bitmap RImage = ciq.ReduceColors(bmp, colorTable);
            




            int[] flatImage = new int[RImage.Width * RImage.Height];
            for (int x = 0; x < RImage.Width; x++)
            {
                for (int y = 0; y < RImage.Height; y++)
                {
                    flatImage[x + RImage.Width * y] = colorMap[RImage.GetPixel(x, y).ToString()];
                }
            }
            output.AddRange(Encoding.UTF8.GetBytes("YAIF"));
            Add16BitInt(2);
            Add16BitInt(bmp.Width);
            Add16BitInt(bmp.Height);
            Add8BitInt(colorTable.Length * 3);
            Add32BitInt(flatImage.Length/2);
            output.AddRange(Encoding.UTF8.GetBytes("FUCK"));

            for (int i = 0; i < colorTable.Length; i++)
            {
                Add8BitInt(colorTable[i].R);
                Add8BitInt(colorTable[i].G);
                Add8BitInt(colorTable[i].B);
            }

            for (int i = 0; i < flatImage.Length; i += 2)
            {

                AddNibbles(flatImage[i], flatImage[i + 1]);
            }

            byte[] outputArray = output.ToArray();
            File.WriteAllBytes(args[0].Split('.')[0]+".yaif", outputArray);
            
        }
    }
}
