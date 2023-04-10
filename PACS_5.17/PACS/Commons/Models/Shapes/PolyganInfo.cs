using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;

namespace PACS.Commons.Models.Shapes
{
    class PolyganInfo : ShapeInfo
    {
        ShapeType EditMode = ShapeType.Polygon;

        public String Icon = "PentagonOutline";

        public String Name = "Polygan";

        public String ToolTip = "多边形";


        public PolyganInfo()
        {
            num = 99;
            points = new System.Windows.Input.StylusPoint[99];
        }

        public override void AddPoint(Point point)
        {
            System.Windows.Input.StylusPoint stylusPoint = new System.Windows.Input.StylusPoint(point.X, point.Y);



            double dx = points[0].X - points[count].X;
            double dy = points[0].Y - points[count].Y;



            if ((count > 1 && dx * dx + dy * dy <= 100) || count == num)
            {
                Stroke.StylusPoints.Add(points[0]);
                isFinished = true;
            }

            points[count] = stylusPoint;
            count = count + 1;
        }

        public override void Draw(Point point, DrawingAttributes drawingAttributes)
        {
            System.Windows.Ink.DrawingAttributes attributes = drawingAttributes.Clone();
            System.Windows.Input.StylusPoint stylusPoint = new System.Windows.Input.StylusPoint(point.X, point.Y);
            points[count] = stylusPoint;




            System.Windows.Input.StylusPoint[] temp = new System.Windows.Input.StylusPoint[count + 1];

            for (int i = 0; i <= count; i++)
            {
                temp[i] = points[i];
            }

            Stroke = new System.Windows.Ink.Stroke(new System.Windows.Input.StylusPointCollection(temp), attributes);
            
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
