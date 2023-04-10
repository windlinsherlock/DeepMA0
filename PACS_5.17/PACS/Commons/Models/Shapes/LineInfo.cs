using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace PACS.Commons.Models.Shapes
{
    public class LineInfo : ShapeInfo
    {
        
        ShapeType EditMode = ShapeType.Line;

        System.Windows.Ink.StrokeCollection Strokes;


        public String Icon = "Minus";

        public String Name = "Line";

        public String ToolTip = "直线";

        public LineInfo()
        {
            points = new System.Windows.Input.StylusPoint[num];
        }

        public LineInfo(System.Windows.Ink.StrokeCollection Strokes)
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
            Stroke = new System.Windows.Ink.Stroke(new System.Windows.Input.StylusPointCollection(points), attributes);

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
            throw new NotImplementedException();
        }

        public override string Deserialize()
        {
            throw new NotImplementedException();
        }


    }
}
