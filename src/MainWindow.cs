using System;
using Gtk;

namespace Photobooth
{
   // Your MainWindow class here
   class MainWindow: Window
   {  
      
      //Private variables
      MenuBar _menuBar;
      Toolbar _toolBar;

      ListView _listView;

      Canvas _canvas;

      CompositeModel _model;

      //TransformTools
      TransformTool _transform = new TransformTool();

      Alignment Align(Widget widget, float xalign, float yalign, 
            float xscale, float yscale)
      {
         Alignment alignment = new Alignment(xalign, yalign, xscale, yscale);
         alignment.Add(widget);
         return alignment;
      }

      public MainWindow(string title) : base (title)
      {
         //Create model
         _model = new CompositeModel();

         _model.AddCompositeChangedCallback(OnCompositeImageChangeCallback);
         _model.AddLayerChangedCallback(OnLayerChangeCallback);

         //Below is all setting up initial look
         SetDefaultSize(800,600);
         SetPosition(WindowPosition.Center);

         VBox mainBox = new VBox(false, 5);

         // Create the Menubar, "File" and "Filter" menu
         _menuBar = new MenuBar();
         Menu file_menu1 = new Menu();
         MenuItem open_item = new MenuItem("Open");
         MenuItem save_item = new MenuItem("Save As");
         MenuItem exit_item = new MenuItem("Exit");
         file_menu1.Append(open_item);
         file_menu1.Append(save_item);
         file_menu1.Append(exit_item);

         MenuItem file_item1 = new MenuItem("File");
         file_item1.Submenu = file_menu1;
         _menuBar.Append(file_item1);

         Menu file_menu2 = new Menu();
         MenuItem[] filterItems = new MenuItem[_model.FilterNames.Count];
         for(int i = 0; i < _model.FilterNames.Count; i++)
         {
            filterItems[i] = new MenuItem(_model.FilterNames[i]);
            filterItems[i].Activated += OnFilterClickCallback;
            file_menu2.Append(filterItems[i]);
         }
         /* 
         MenuItem none_item = new MenuItem("None");
         MenuItem grayscale_item = new MenuItem("Grayscale");
         MenuItem lighten_item = new MenuItem("Lighten");
         MenuItem darken_item = new MenuItem("Darken");
         MenuItem jitter_item = new MenuItem("Jitter");

         file_menu2.Append(none_item);
         file_menu2.Append(grayscale_item);
         file_menu2.Append(lighten_item);
         file_menu2.Append(darken_item);
         file_menu2.Append(jitter_item);
         */

         MenuItem file_item2 = new MenuItem("Filter");
         file_item2.Submenu = file_menu2;
         _menuBar.Append(file_item2);

         // Creates the Toolbar and everything inside
         _toolBar = new Toolbar();
         
         // Creates a Pixbuf Array
         Gdk.Pixbuf[] pixbufArray = new Gdk.Pixbuf[]
         {  
            new Gdk.Pixbuf("ops/move.png"),
            new Gdk.Pixbuf("ops/scale.png"),
            new Gdk.Pixbuf("accessories/star.png"),
            new Gdk.Pixbuf("accessories/aviator.png"),            
            new Gdk.Pixbuf("accessories/heart.png"),
            new Gdk.Pixbuf("accessories/nerd.png"),
            new Gdk.Pixbuf("accessories/horns.png"),
            new Gdk.Pixbuf("accessories/halo.png"),
            new Gdk.Pixbuf("accessories/tiara.png"),
            new Gdk.Pixbuf("accessories/moustache.png"),
            new Gdk.Pixbuf("accessories/librarian.png")
         };

         // Creates a toolNames Array
         string[] toolNames = new string[]{"move", "scale", "star","aviator",
                                          "heart", "nerd", "horns", "halo",
                                          "tiara", "moustache", "librarian"};
         ToolButton[] buttons = new ToolButton[pixbufArray.Length];

         Widget widget;
         // Creates all the Toolbuttons
         for(int i = 0; i < pixbufArray.Length; i++)
         {
            widget = new Image(new IconSet(pixbufArray[i]), IconSize.Button);
            buttons[i] = new ToolButton(widget, toolNames[i]);
            _toolBar.Insert(buttons[i], -1);
         }
         _toolBar.ToolbarStyle = ToolbarStyle.Icons;
         
         //Create a HBox containing canvas and a VBox of _listView and delete  
         HBox viewCanvasBox = new HBox(false, 5);

         VBox viewAndDelete = new VBox(false, 5);

         _listView = new ListView("listview");
         Button delete = new Button();
         delete.Label = "Delete Layer";

         // Create Canvas and set a default image
         Gdk.Pixbuf image = new Gdk.Pixbuf("photos/kitty2.jpg"); //Default Image
         _canvas = new Canvas(image);
         _model.LoadBaseImage("photos/kitty2.jpg");
         
         // Create an EventBox containing canvas
         EventBox eventBox = new EventBox();   

         //Add every box in Window

         //The 4th level: add _listView and delete to viewAndDelete
         viewAndDelete.PackStart(Align(_listView, 0, 0, 1, 1), true, true, 0);
         viewAndDelete.PackStart(Align(delete, 1 , 0, 1, 0), false, false, 0);

         //The 4th level: add _canvas to EventBox
         eventBox.Add(_canvas);

         //The 3rd level: add viewAndDelete and eventBox to viewCanvasBox
         viewCanvasBox.PackStart(Align(viewAndDelete, 0, 0, 0.5f, 1), false, 
                                                                      false, 0);
         viewCanvasBox.Add(Align(eventBox, 0.5f, 0.5f, 0, 0));
         
         //The 2nd level: add _menuBar, _toolBar, and viewCanvasBox to mainBox
         mainBox.PackStart(Align(_menuBar, 0, 0, 0, 0 ), false ,false, 0);
         mainBox.PackStart(Align(_toolBar, 0, 0.5f, 1, 0 ), false, false, 0);
         mainBox.PackStart(Align(viewCanvasBox, 0, 0, 1, 1 ), true, true, 0);
         
         //The Surface level: add mainBox to MainWindow
         Add(mainBox);
         
         ShowAll();

         //Event-handling

         //MenuBar Event-handling
         exit_item.Activated += delegate { Application.Quit(); };
         open_item.Activated += delegate { OnOpenCallback(); };
         save_item.Activated += delegate { OnSaveCallback(); };

         //ToolBar Event-Handling
         buttons[0].Clicked += delegate { OnMoveLayerCallback();}; 
         buttons[1].Clicked += delegate { OnScaleLayerCallback();}; 
         buttons[2].Clicked += delegate { OnAccessoryCallback(pixbufArray[2],
                                          toolNames[2]);}; 
         buttons[3].Clicked += delegate { OnAccessoryCallback(pixbufArray[3],
                                          toolNames[3]);}; 
         buttons[4].Clicked += delegate { OnAccessoryCallback(pixbufArray[4],
                                          toolNames[4]);}; 
         buttons[5].Clicked += delegate { OnAccessoryCallback(pixbufArray[5],
                                          toolNames[5]);}; 
         buttons[6].Clicked += delegate { OnAccessoryCallback(pixbufArray[6],
                                          toolNames[6]);}; 
         buttons[7].Clicked += delegate { OnAccessoryCallback(pixbufArray[7],
                                          toolNames[7]);}; 
         buttons[8].Clicked += delegate { OnAccessoryCallback(pixbufArray[8],
                                          toolNames[8]);};     
         buttons[9].Clicked += delegate { OnAccessoryCallback(pixbufArray[9],
                                          toolNames[9]);};                                                            
         buttons[10].Clicked += delegate { OnAccessoryCallback(pixbufArray[10],
                                          toolNames[10]);}; 
         //Delete Event-handling
         delete.Clicked += delegate{ OnDeleteCallback(); };

         //EventBox Event-handling
         eventBox.ButtonPressEvent += delegate (object obj, 
                                                   ButtonPressEventArgs args) 
         { 
            OnButtonPressCallback(obj, args); 
         };
         eventBox.MotionNotifyEvent += delegate (object obj, 
                                                   MotionNotifyEventArgs args)  
         { 
            OnMotionNotifyCallback(obj, args); 
         };
         eventBox.ButtonReleaseEvent += delegate (object obj, 
                                                   ButtonReleaseEventArgs args) 
         { 
            OnButtonReleaseCallback(obj, args); 
         };
         
         //Closing Window
         DeleteEvent += delegate { Application.Quit(); };
      }
      
      //Callbacks

      //requires: nothing
      //effects:asks the User for a filename; 
      //        triggers LoadBasedImage of CompositeModel based on the filename; 
      //        updates the image displayed;
      //        if image fails to load, displays an error message.
      void OnOpenCallback()
      {
         FileChooserDialog file = new FileChooserDialog("Choose an image",
                     this, FileChooserAction.Open, 
                     "Cancel", Gtk.ResponseType.Cancel,
                     "Ok", Gtk.ResponseType.Ok);
            
         string filename;
         if (file.Run() != (int)Gtk.ResponseType.Ok)
         {
            file.Destroy();
            return;
         }
         else
         {
            filename = file.Filename;
            file.Destroy();
         }
         bool loadSuccess = _model.LoadBaseImage(filename); 
         if (loadSuccess == false)
         {
            MessageDialog message = new MessageDialog(this,
               DialogFlags.DestroyWithParent, MessageType.Info,
               ButtonsType.Close, "Error loading file");
               message.Run();
               message.Destroy();
         }
         _canvas.SetImage(_model.CompositeImage);
      }

      //requires: nothing
      //effects: opens a dialog that asks User for a filename;
      //         if User decides to save the image, triggers the
      //              SaveCompositeImage of the CompositeModel
      //         if save is not successful, displays an error message
      void OnSaveCallback()
      {
         FileChooserDialog save = new FileChooserDialog("Save As",
                  this, FileChooserAction.Save,
                  "Cancel", Gtk.ResponseType.Cancel,
                  "Save", Gtk.ResponseType.Ok);
         string filename;
         if (save.Run() != (int)Gtk.ResponseType.Ok)
         {
            save.Destroy();
            return;
         } 
         else
         {
            filename = save.Filename;
            save.Destroy();
         }  
         bool saveSuccess = _model.SaveCompositeImage(filename);
         if (saveSuccess == true)
         {
            Console.WriteLine("working");
            MessageDialog message = new MessageDialog(this,
                  DialogFlags.DestroyWithParent, MessageType.Info,
                  ButtonsType.Close, "Save As Successful!");
            message.Run();
            message.Destroy();
         }
         else
         {
            MessageDialog message = new MessageDialog(this,
                  DialogFlags.DestroyWithParent, MessageType.Info,
                  ButtonsType.Close, "Error saving file");
            message.Run();
            message.Destroy();
         }
      }

      //requires: nothing
      //effects: Applies the filter clicked to the composite image
      void OnFilterClickCallback(object obj, EventArgs args)
      {
         MenuItem menuItem = obj as MenuItem;
         if(menuItem != null)
         {
            string filterName = menuItem.Label;
            _model.RunFilter(filterName);
         }
      }

      //requires: nothing
      //effects: change the mode of the transform tool to Mode.TRANSLATE
      void OnMoveLayerCallback()
      {
         _transform.mode = Mode.TRANSLATE;
      }

      //requires: nothing
      //effects: change the mode of the transform tool to Mode.SCALE
      void OnScaleLayerCallback()
      {
         _transform.mode = Mode.SCALE;
      }
      
      //requires: the accessory image and the image name
      //effects: triggers AddLayer of the CompositeModel
      void OnAccessoryCallback(Gdk.Pixbuf accessoryImage, string accessoryName)
      {
         _model.AddLayer(accessoryImage, accessoryName);
      }

      //requires: nothing
      //effects: triggers DeleteLayer of the CompositeModel
      void OnDeleteCallback()
      {
         if (_listView.Selected >= 0)
         {
            _model.DeleteLayer(_listView.Selected);
         }
      }

      //requires: an object and ButtonPressEventargs
      //effects: gets (x,y) coordinate and activate tranformTool 
      //         and its DoWork method
      void OnButtonPressCallback(object obj, ButtonPressEventArgs args)
      {
         double x = args.Event.X;
         double y = args.Event.Y;
         if(_listView.Selected == -1)
         {            
            return;
         }
         _transform.Activate(x, y);
         _transform.DoWork(x, y, _listView.Selected, _model);
      }

      //requires: an object and MotionNotifyEventArgs
      //effects: gets the (x,y) coordinates and triggers transformTool's DoWork
      void OnMotionNotifyCallback(object obj, MotionNotifyEventArgs args)
      {
         double x = args.Event.X;
         double y = args.Event.Y;
         _transform.DoWork(x, y, _listView.Selected, _model);
      }

      //requires: an object and ButtonReleaseEventArgs
      //effects: deactivates transformTool
      void OnButtonReleaseCallback(object obj, ButtonReleaseEventArgs args)
      {
         _transform.Deactivate();
      }

      //requires: nothing
      //effects: updates the image in canvas
      void OnCompositeImageChangeCallback()
      {
         _canvas.SetImage(_model.CompositeImage);
      }

      //requires: nothing
      //effects: clears and updates _listView; selects the last item by default
      void OnLayerChangeCallback()
      {
         _listView.Clear();
         for(int i = 0; i < _model.NumLayers; i++) 
         {
            _listView.AddItem(_model.GetLayerName(i));
         }
         if (_model.NumLayers > 0)
         {
            _listView.Selected = _model.NumLayers-1;
         }
      }
   }
}