using System;
using System.Collections.Generic;
using System.Text;

namespace PACS.Commons.Models.Shapes
{
    class ShapeFactory
    {
        protected System.Windows.Ink.StrokeCollection Strokes;

        public static ShapeInfo GetShapeInfo(ShapeType shapetype)
        {
            if (shapetype == ShapeType.Line)
            {
                return new LineInfo();
            }
            else if (shapetype == ShapeType.Rectangle)
            {
                return new RectangleInfo();
            }
            else if (shapetype == ShapeType.Square)
            {
                return new SquareInfo();
            }
            else if (shapetype == ShapeType.Polygon)
            {
                return new PolyganInfo();
            }
            else
            {
                return null;
            }

            /*
            else if (shapetype == ShapeType.Triangle)
            {
                return new Triangle();
            }
            else
            {
                return new Square();
            }*/

        }

        public static ShapeInfo GetShapeInfo(string shapetype)
        {            
            switch (shapetype)
            {
                case "Line": return new LineInfo(); 
                case "Rectangle": return new RectangleInfo(); 
                case "Square": return new SquareInfo(); 
                case "Polygan": return new PolyganInfo(); 
                default: return null;
            }
            
        }
    }
}
