using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace PACS.Commons.Models.Shapes
{
    class RectangleInfo : ShapeInfo
    {
        
        ShapeType EditMode = ShapeType.Rectangle;

        public String Icon = "RectangleOutline";

        public String Name = "Rectangle";

        public String ToolTip = "四边形";

        System.Windows.Ink.StrokeCollection Strokes;

        public RectangleInfo()
        {
            points = new System.Windows.Input.StylusPoint[num];
        }

        public RectangleInfo(System.Windows.Ink.StrokeCollection Strokes)
        {
            this.Strokes = Strokes;
            points = new System.Windows.Input.StylusPoint[num];
            
        }

        public override void AddPoint(Point point)
        {
            System.Windows.Input.StylusPoint stylusPoint = new System.Windows.Input.StylusPoint(point.X, point.Y);
            points[count] = stylusPoint;
            count = count + 1;
            if (count == num)
                isFinished = true;
        }

        public override void Draw(Point point, System.Windows.Ink.DrawingAttributes drawingAttributes)
        {
            System.Windows.Ink.DrawingAttributes attributes = drawingAttributes.Clone();
            System.Windows.Input.StylusPoint stylusPoint = new System.Windows.Input.StylusPoint(point.X, point.Y);
            points[count] = stylusPoint;

            System.Windows.Input.StylusPoint[] temp = new System.Windows.Input.StylusPoint[5];
            temp[0] = points[0];
            temp[1] = new System.Windows.Input.StylusPoint(points[1].X, points[0].Y);
            temp[2] = points[1];
            temp[3] = new System.Windows.Input.StylusPoint(points[0].X, points[1].Y);
            temp[4] = points[0];

            Stroke = new System.Windows.Ink.Stroke(new System.Windows.Input.StylusPointCollection(temp), attributes);
            
            
            //Strokes.Add(Stroke);
        }

        public override System.Windows.Ink.Stroke Draw(string tag, System.Windows.Input.StylusPointCollection stylusPointCollection, Color color)
        {

            System.Windows.Ink.DrawingAttributes attributes = new System.Windows.Ink.DrawingAttributes();
            attributes.Color = color;

            Stroke = new System.Windows.Ink.Stroke(stylusPointCollection, attributes);
            Stroke.AddPropertyData(new Guid("12345678-9012-3456-7890-123456789000"), Name);
            Stroke.AddPropertyData(new Guid("12345678-9012-3456-7890-123456789001"), tag);

            return Stroke;
        }

        public override string Serialize()
        {
            String str = "{\nRect,\n";
            foreach (System.Windows.Input.StylusPoint point in points)
            {
                str += point.X + "," + point.Y + "\n";
            }
            str += "}";
            return str;
        }

        public override string Deserialize()
        {
            throw new NotImplementedException();
        }
    }
}
