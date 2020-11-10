using System;
using System.IO;
using Gtk;

namespace Photobooth
{
    /// <summary>
    /// Canvas implements an image container capable of receiving events
    /// and changing image
    /// </summary>
    class Canvas : EventBox
    {
        Image _image;

        // requires: Non-empty, valid image
        public Canvas(Gdk.Pixbuf image)
        {
            _image = new Image(image);
            Add(_image);
        }

        /// <summary>
        /// Set the content of this view to the passed pixels and resizes this  
        /// container to fit it.
        /// </summary>
        /// <param name="pixels">a non-empty, valid image</param>
        public void SetImage(Gdk.Pixbuf pixels)
        {
            _image.Pixbuf = pixels;
            SetSizeRequest(_image.Pixbuf.Width, _image.Pixbuf.Height);
        }
    }
}