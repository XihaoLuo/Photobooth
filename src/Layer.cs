using System;
using System.Collections.Generic;

namespace Photobooth
{
    // Implements a layer in a composite image
    class Layer
    {
        Gdk.Pixbuf _original = null; // unaltered layer image
        double _size = 1; // uniform scale
        double _x = 0; // horizontal position relative to the left of the image
        double _y = 0; // vertical position relative to the top of the image 
        string _name = ""; // (optional) name of the layer

        // requires: non-empty, valid pixels
        public Layer(Gdk.Pixbuf pixels, string name = "Layer")
        {
            _original = pixels;
            _name = name;
        }

        // requires: target contains non-empty image
        // effects: applies the operations to _original and composes it onto the given target
        //    IMPORTANT: does not modify _original 
        public void Apply(Gdk.Pixbuf target)
        {
            int layerWidth = (int) (Size*_original.Width);
            int layerHeight = (int) (Size*_original.Height);
            if (layerWidth > target.Width || layerHeight > target.Height)
            {
                Console.WriteLine("ERROR: Layer too large....aborting");
                return;
            }
            int startx = (int) (X - layerWidth/2);
            int starty = (int) (Y - layerHeight/2);
            int width = (int) (_original.Width*Size);
            int height = (int) (_original.Height*Size);

            startx = Math.Clamp(startx, 0, target.Width - layerWidth);
            starty = Math.Clamp(starty, 0, target.Height - layerHeight);
            _original.Composite(target, startx, starty, width, height, 
               startx, starty, Size, Size, Gdk.InterpType.Nearest, 255);
        }

        // requires: nothing
        // effects: returns true if the coordinate (x,y) is within this 
        //    layer's axis-aligned bounding box; false otherwise    
        public bool Hits(double x, double y)
        {
            double minX = X - Size*Width/2;
            double maxX = X + Size*Width/2;

            double minY = Y - Size*Height/2;
            double maxY = Y + Size*Height/2;

            return (minX <= x  && x <= maxX && minY <= y && y <= maxY);
        }

        // readonly name property
        public string Name 
        {
            get { return _name; }
            
        }

        // readonly: original height of layer
        public double Height
        {
            get { return _original.Height; }
        }

        // readonly: original width of layer
        public double Width
        {
            get { return _original.Width; }
        }

        // vertical position of layer, relative to the base image top
        public double Y
        {
            set { _y = value; }
            get { return _y; }
        }

        // horizontal position of layer, relative to the base image left
        public double X
        {
            set { _x = value; }
            get { return _x; }
        }

        // uniform scale for this layer
        public double Size
        {
            set { _size = value; }
            get { return _size; }
        }

        // readonly: pixels of the original image
        public Gdk.Pixbuf OriginalImage
        {
            get { return _original; }
        }
    }
}