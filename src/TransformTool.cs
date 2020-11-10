using System;
using Gtk;

namespace Photobooth
{
    enum Mode
    {
        TRANSLATE,
        SCALE
    }
    class TransformTool
    {
        Mode _mode;
        bool _inOp = false;
        double _lastX = 0;
        double _lastY = 0;

        public TransformTool()
        {
            _mode = Mode.TRANSLATE;
            _inOp = false;
        }

        public Mode mode
        {
            set { _mode = value; }
            get { return _mode; }
        }

        public bool isActive
        {
            get { return _inOp; }
        }

        public void Activate(double mousex, double mousey)
        {
            _lastX = mousex;
            _lastY = mousey;
            _inOp = true;
        }

        public void Deactivate()
        {
            _inOp = false;
        }

        public void DoWork(double mousex, double mousey, int layer, CompositeModel model)
        {
            if (!isActive) return;

            if (_mode == Mode.TRANSLATE)
            {
                model.MoveLayer(layer, mousex, mousey);
            } 
            else if (_mode == Mode.SCALE) 
            {
                double dx = mousex - _lastX;
                double dy = mousey - _lastY;
                double delta = Math.Abs(dx) > Math.Abs(dy)? dx : dy;
                double scale = Math.Max(0, Math.Abs(delta)/50); // 50 pixels => scale by 2
                double size = delta < 0? 1/(1 + scale) : 1 + scale;
                model.ScaleLayer(layer, size);
            }
        }
    }
}