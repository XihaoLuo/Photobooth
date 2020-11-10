using System.Collections.Generic;
using System.Threading;
using System;
using Gtk;


namespace Photobooth
{
    class CompositeModel
    {
        //Private data members
        List<Layer> _layerList;
        Gdk.Pixbuf _baseImage;  
        Gdk.Pixbuf _compImage; 

        FilterFactory _filters = new FilterFactory();

        //Callbacks when layers changes
        List<LayerChangedCallback> _layerChangeCbs = 
                                         new List<LayerChangedCallback>();
        
        //Callbacks when composite image changes
        List<CompositeChangedCallback> _compositeChangeCbs =
                                        new List<CompositeChangedCallback>();
        public delegate void LayerChangedCallback(); //function type
        public delegate void CompositeChangedCallback(); //function type

        //Constructor
        public CompositeModel()
        {
            _filters.RegisterFilter("None", NoneFilter.Create);
            _filters.RegisterFilter("GrayScale", GrayscaleFilter.Create);
            _filters.RegisterFilter("Lighten", LightenFilter.Create);
            _filters.RegisterFilter("Darken", DarkenFilter.Create);
            _filters.RegisterFilter("Jitter", JitterFilter.Create);
        }
         
        //Properties
        //returns composite image; not settable
        public Gdk.Pixbuf CompositeImage
        {
            get {return _compImage; }
        }

        // returns number of layers; not settable
        public int NumLayers
        {
            get {return _layerList.Count; }
        }
        
        // Return the names of the filters contained in the filter factory;
        //       not settable
        public List<string> FilterNames
        {
            get {return _filters.GetFilterNames();}
        }
        
        //Public Methods


        // requires: nothing
        // effects: Applies the filter corresponding to name to the composite image 
        //   and invokes callbacks to notify that the composite image has changed
        //   Does nothing if name does not correspond to a valid filter
        public void RunFilter(string name)
        {
            Filter newFilter = _filters.Create(name);
            if (newFilter != null)
            {
                newFilter.Run(_compImage);
                InvokeCompositeChangeCallbacks();
            }
        }

        // Wrapper function for FilterFactory.RegisterFilter
        public void RegisterFilter(string name, Filter.CreateFn fn)
        {
            _filters.RegisterFilter(name, fn);
        }

        // Wrapper function for FilterFactory.DeregisterFilter
        public void DeregisterFilter(string name)
        {
            _filters.DeregisterFilter(name);
        }
         
        // requires: non-null callback method or listener class
        // effects: removes the callback from the list of layer cbs; noop if not found
        public void RemoveLayerChangedCallback(LayerChangedCallback callback)
        {
            _layerChangeCbs.Remove(callback);
        }

        // requires: non-null callback method or listener class
        // effects: adds the callback to the list of layer cbs
        public void AddLayerChangedCallback(LayerChangedCallback callback)
        {
            _layerChangeCbs.Add(callback);
        }

        // requires: non-null callback method or listener class
        // effects: removes the callback from the list of composite image cbs; noop if not found
        public void RemoveCompositeChangedCallback(CompositeChangedCallback callback)
        {
            _compositeChangeCbs.Remove(callback);
        }

        // requires: non-null callback method or listener class
        // effects: adds the callback to the list of composite cbs
        public void AddCompositeChangedCallback(CompositeChangedCallback callback)
        {
            _compositeChangeCbs.Add(callback);
        }

        // requires: 0 <= id < NumLayers
        // effects: scales the layer with id uniformly by size; 
        //     updates the composite image; and invokes composite image cbs
        public void ScaleLayer(int id, double size)
        {
            Layer currentLayer = _layerList[id];
            currentLayer.Size = size;
            _compImage = _baseImage.Copy(); 
            foreach(var layer in _layerList)
            {
                layer.Apply(_compImage);
            }
            InvokeCompositeChangeCallbacks();
        }

        // requires: 0 <= id < NumLayers
        // effects: moves layer with id to position X (horizontal) and Y (vertical) 
        //     relative to the base image's top left corner; 
        //     updates the composite image; and invokes composite image cbs
        public void MoveLayer(int id, double x, double y)
        {   
            Layer currentLayer = _layerList[id];
            if (currentLayer.Hits(x,y) == true)
            {
                currentLayer.X = x;
                currentLayer.Y = y;
                _compImage = _baseImage.Copy(); 
                foreach(var layer in _layerList)
                {
                    layer.Apply(_compImage);
                }
                InvokeCompositeChangeCallbacks();
            }
        }

        // requires: 0 <= id < NumLayers
        // effects: Returns the name of layer with id (note: can be "")
        public string GetLayerName(int id)
        {
            return _layerList[id].Name;
        }

        // requires: non-empty pixels
        // effects: Adds a new layer having the given image and name; invokes layer and composite Image cbs
        public void AddLayer(Gdk.Pixbuf pixels, string name)
        {
            //resize pixels
            pixels = pixels.ScaleSimple(100, 100, Gdk.InterpType.Bilinear);

            Layer newLayer = new Layer(pixels, name);
            newLayer.Apply(_compImage);
            _layerList.Add(newLayer);
            InvokeCompositeChangeCallbacks();
            InvokeLayerChangeCallbacks();
        }

        // requires: 0 <= id < NumLayers
        // effects: removes layer with id from our list;  
        //     updates the composite image; and invokes composite and layer cbs
        public void DeleteLayer(int id)
        {
            _layerList.RemoveAt(id);
            _compImage = _baseImage.Copy(); 
            foreach(var layer in _layerList)
            {
                layer.Apply(_compImage);
            }
            InvokeCompositeChangeCallbacks();
            InvokeLayerChangeCallbacks();
        }

        // requires: nothing
        // effects: Saves the image with the given filename. 
        //     Returns true if successful; false otehrwise. 
        public bool SaveCompositeImage(string filename)
        {
            return _compImage.Save(filename, "png");
        }

        // requires: nothing
        // effects: Sets the base image with the given filename. 
        //     Initializes the compoiste image.
        //     Returns true if successful; false otehrwise. 
        public bool LoadBaseImage(string filename)
        {
            _layerList = new List<Layer>();
            try
            {
                Gdk.Pixbuf newImage = new Gdk.Pixbuf(filename);
                _baseImage = newImage.Copy();
                _compImage = newImage.Copy();
            }
            catch
            {
                return false;
            }
            return true;
        }

        //requires: nothing
        //effects: invokes layer change callbacks
        void InvokeLayerChangeCallbacks()
        {
            foreach (var item in _layerChangeCbs)
            {
                item();
            }
        }

        //requires: nothing
        //effects: invokes composite image callbacks
        void InvokeCompositeChangeCallbacks()
        {
            foreach (var item in _compositeChangeCbs)
            {
                item();
            }
        }
    }
}