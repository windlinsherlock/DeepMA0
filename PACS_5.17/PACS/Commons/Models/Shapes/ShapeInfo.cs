using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace PACS.Commons.Models.Shapes
{
    public enum ShapeType
    {
        Any,
        Line,
        Rectangle,
        Polygon,
        Square
    }

    
    public abstract class ShapeInfo
    {
        
        public System.Windows.Ink.Stroke Stroke { set; get; }

        protected System.Windows.Input.StylusPoint[] points;

        
        public System.Windows.Ink.DrawingAttributes drawingAttributes;


        public bool isFinished = false;

        protected int count = 0;
        protected int num = 2;

        

        public abstract void AddPoint(Point point);

        public abstract void Draw(Point point, System.Windows.Ink.DrawingAttributes drawingAttributes);

        public abstract System.Windows.Ink.Stroke Draw(string tag, System.Windows.Input.StylusPointCollection stylusPointCollection, Color color);

        public abstract string Serialize();

        public abstract string Deserialize();
    }
}
