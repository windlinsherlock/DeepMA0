using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace PACS.Commons.Models.Shapes
{
    class SquareInfo : ShapeInfo
    {
        ShapeType EditMode = ShapeType.Square;

        public String Icon = "CodeBrackets";

        public String Name = "Squre";

        public String ToolTip = "关注区域";


        public SquareInfo()
        {
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
            temp[2] = new System.Windows.Input.StylusPoint(points[1].X, points[1].X - points[0].X + points[0].Y);
            temp[3] = new System.Windows.Input.StylusPoint(points[0].X, points[1].X - points[0].X + points[0].Y);
            temp[4] = points[0];          
            
            Stroke = new System.Windows.Ink.Stroke(new System.Windows.Input.StylusPointCollection(temp), attributes);
            
        }


        public override Stroke Draw(string tag,StylusPointCollection stylusPointCollection,Color color)
        {

            DrawingAttributes attributes = new DrawingAttributes();
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
