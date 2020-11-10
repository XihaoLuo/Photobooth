using System;
using System.Diagnostics;
using Gtk;

namespace Photobooth
{
    public abstract class Filter
    {
        // A struct representing a pixel
        // Pixels consist of red, green, blue color components
        protected struct RGB
        {
            public byte red; // 0-255
            public byte green; // 0-255
            public byte blue; // 0-255

            public RGB(byte r, byte g, byte b)
            {
                red = r;
                green = g;
                blue = b;
            }
        }

        // static creator method for creating filters
        // subclasses should define their own CreateFn which returns a 
        // new instance of the Filter
        public delegate Filter CreateFn();

        // requires: valid buffer
        // effects: creates a new Pixbuf modified according to the filter
        public abstract Gdk.Pixbuf Run(Gdk.Pixbuf buffer);

        // requires: 0 <= row < Width; 0 <= col < Height
        // effects: returns RGB with the color of the pixel
        protected static RGB GetPixel(Gdk.Pixbuf buffer, int row, int col)
        {
            Debug.Assert(0 <= row && row < buffer.Height, "Invalid row: "+row);
            Debug.Assert(0 <= col && col < buffer.Width, "Invalid col: "+col);

            bool hasAlpha = buffer.HasAlpha;
            int rowstride = buffer.Rowstride;
            int pixelstride = buffer.NChannels;

            int idx = row * rowstride + col * pixelstride;
            IntPtr data = buffer.Pixels + idx;

            RGB color = new RGB();
            unsafe
            {
                byte* raw = (byte*) data.ToPointer();
                color.red   = *(raw + 0);
                color.green = *(raw + 1);
                color.blue  = *(raw + 2);
            }
            return color;
        }

        // requires: 0 <= row < Width; 0 <= col < Height
        // effects: sets the RGB value of buffer to the color of the given pixel
        protected static void SetPixel(Gdk.Pixbuf buffer, int row, int col, RGB color)
        {
            Debug.Assert(0 <= row && row < buffer.Height, "Invalid row: "+row);
            Debug.Assert(0 <= col && col < buffer.Width, "Invalid col: "+col);

            bool hasAlpha = buffer.HasAlpha;
            int rowstride = buffer.Rowstride;
            int pixelstride = buffer.NChannels;

            int idx = row * rowstride + col * pixelstride;
            IntPtr data = buffer.Pixels + idx;

            unsafe
            {
                byte* raw = (byte*) data.ToPointer();
                *(raw + 0) = color.red; 
                *(raw + 1) = color.green;
                *(raw + 2) = color.blue;
            }
        }
    }

    public class NoneFilter : Filter
    {
        public static Filter Create()
        {
            return new NoneFilter();
        }

        public override Gdk.Pixbuf Run(Gdk.Pixbuf buffer)
        {
            return buffer;
        }
    }

    public class GrayscaleFilter : Filter
    {
        public static Filter Create()
        {
            return new GrayscaleFilter();
        }

        public override Gdk.Pixbuf Run(Gdk.Pixbuf buffer)
        {
            for(int i = 0; i < buffer.Height; i++)
            {
                for(int j = 0; j< buffer.Width; j++)
                {
                    RGB currentPixel = GetPixel(buffer, i, j);
                    int gray = (currentPixel.red + currentPixel.green + 
                            currentPixel.blue)/3;
                    Byte grayByte = Convert.ToByte(gray);
                    RGB newPixel = new RGB(grayByte, grayByte, grayByte);
                    SetPixel(buffer, i, j, newPixel);
                }
            } 
            return buffer;
        }
    }

    public class LightenFilter : Filter
    {
        public static Filter Create()
        {
            return new LightenFilter();
        }

        public override Gdk.Pixbuf Run(Gdk.Pixbuf buffer)
        {
            for(int i = 0; i < buffer.Height; i++)
            {
                for(int j = 0; j< buffer.Width; j++)
                {
                    RGB currentPixel = GetPixel(buffer, i, j);
                    int red = Math.Clamp(currentPixel.red + 100, 0, 255);
                    int green = Math.Clamp(currentPixel.green + 100, 0, 255);
                    int blue = Math.Clamp(currentPixel.blue + 100, 0, 255);
                    RGB newPixel = new RGB(Convert.ToByte(red), 
                                Convert.ToByte(green), Convert.ToByte(blue));
                    SetPixel(buffer, i, j, newPixel);
                }
            }
            return buffer;
        }
    }

    public class DarkenFilter : Filter
    {
        public static Filter Create()
        {
            return new DarkenFilter();
        }

        public override Gdk.Pixbuf Run(Gdk.Pixbuf buffer)
        {
            for(int i = 0; i < buffer.Height; i++)
            {
                for(int j = 0; j< buffer.Width; j++)
                {
                    RGB currentPixel = GetPixel(buffer, i, j);
                    int red = Math.Clamp(currentPixel.red - 100, 0, 255);
                    int green = Math.Clamp(currentPixel.green - 100, 0, 255);
                    int blue = Math.Clamp(currentPixel.blue - 100, 0, 255);
                    RGB newPixel = new RGB(Convert.ToByte(red), 
                                Convert.ToByte(green), Convert.ToByte(blue));
                    SetPixel(buffer, i, j, newPixel);
                }
            }
            return buffer;
        }
    }

    public class JitterFilter : Filter
    {
        public static Filter Create()
        {
            return new JitterFilter();
        }

        public override Gdk.Pixbuf Run(Gdk.Pixbuf buffer)
        {
            for(int i = 0; i < buffer.Height; i++)
            {
                for(int j = 0; j< buffer.Width; j++)
                {
                    RGB currentPixel = GetPixel(buffer, i, j);
                    Random rnd = new Random();
                    int rndNum = rnd.Next(-100, 100);
                    int red = Math.Clamp(currentPixel.red + rndNum, 0, 255);
                    int green = Math.Clamp(currentPixel.green + rndNum, 0, 255);
                    int blue = Math.Clamp(currentPixel.blue + rndNum, 0, 255);
                    RGB newPixel = new RGB(Convert.ToByte(red), 
                                Convert.ToByte(green), Convert.ToByte(blue));
                    SetPixel(buffer, i, j, newPixel);
                }
            }
            return buffer;
        }
    }
}
