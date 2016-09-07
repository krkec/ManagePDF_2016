using Rectangle2D = System.util.RectangleJ;
using ExtRenderListener = iTextSharp.text.pdf.parser.IExtRenderListener;
using ImageRenderInfo = iTextSharp.text.pdf.parser.ImageRenderInfo;
using LineSegment = iTextSharp.text.pdf.parser.LineSegment;
using Matrix = iTextSharp.text.pdf.parser.Matrix;
using Path = iTextSharp.text.pdf.parser.Path;
using PathConstructionRenderInfo = iTextSharp.text.pdf.parser.PathConstructionRenderInfo;
using PathPaintingRenderInfo = iTextSharp.text.pdf.parser.PathPaintingRenderInfo;
using RenderListener = iTextSharp.text.pdf.parser.IRenderListener;
using TextMarginFinder = iTextSharp.text.pdf.parser.TextMarginFinder;
using TextRenderInfo = iTextSharp.text.pdf.parser.TextRenderInfo;
using Vector = iTextSharp.text.pdf.parser.Vector;
using System.Collections;
using System.Collections.Generic;

namespace WpfApplication2
{
    public class MarginFinder : ExtRenderListener
    {
        private Rectangle2D textRectangle = null;
        private Rectangle2D currentPathRectangle = null;

        public virtual void RenderText(TextRenderInfo renderInfo)
        {
            if (textRectangle == null)
            {
                textRectangle = renderInfo.GetDescentLine().GetBoundingRectange();
            }
            else
            {
                textRectangle.Add(renderInfo.GetDescentLine().GetBoundingRectange());
            }
            textRectangle.Add(renderInfo.GetAscentLine().GetBoundingRectange());
        }
        public virtual float Llx
        {
            get
            {
                return textRectangle.X;
            }
        }
        public virtual float Lly
        {
            get
            {
                return textRectangle.Y;
            }
        }
        public virtual float Urx
        {
            get
            {
                return textRectangle.X + textRectangle.Width;
            }
        }
        public virtual float Ury
        {
            get
            {
                return textRectangle.Y + textRectangle.Height;
            }
        }
        public virtual float Width
        {
            get
            {
                return textRectangle.Width;
            }
        }
        public virtual float Height
        {
            get
            {
                return textRectangle.Height;
            }
        }
        public virtual void BeginTextBlock()
        {
        }
        public virtual void EndTextBlock()
        {
        }
        public virtual void RenderImage(ImageRenderInfo renderInfo)
        {
            Matrix imageCtm = renderInfo.GetImageCTM();
            Vector a = (new Vector(0, 0, 1)).Cross(imageCtm);
            Vector b = (new Vector(1, 0, 1)).Cross(imageCtm);
            Vector c = (new Vector(0, 1, 1)).Cross(imageCtm);
            Vector d = (new Vector(1, 1, 1)).Cross(imageCtm);
            LineSegment bottom = new LineSegment(a, b);
            LineSegment top = new LineSegment(c, d);
            if (textRectangle == null)
            {
                textRectangle = bottom.GetBoundingRectange();
            }
            else
            {
                textRectangle.Add(bottom.GetBoundingRectange());
            }
            textRectangle.Add(top.GetBoundingRectange());
        }
        public void ModifyPath(PathConstructionRenderInfo renderInfo)
        {
            IList<Vector> points = new List<Vector>();
            if (renderInfo.Operation == PathConstructionRenderInfo.RECT)
            {
                float x = renderInfo.SegmentData[0];
                float y = renderInfo.SegmentData[1];
                float w = renderInfo.SegmentData[2];
                float h = renderInfo.SegmentData[3];
                points.Add(new Vector(x, y, 1));
                points.Add(new Vector(x + w, y, 1));
                points.Add(new Vector(x, y + h, 1));
                points.Add(new Vector(x + w, y + h, 1));
            }
            else if (renderInfo.SegmentData != null)
            {
                for (int i = 0; i < renderInfo.SegmentData.Count - 1; i += 2)
                {
                    points.Add(new Vector(renderInfo.SegmentData[i], renderInfo.SegmentData[i + 1], 1));
                }
            }
            foreach (Vector point in points)
            {
                Vector point1 = point.Cross(renderInfo.Ctm);
                Rectangle2D pointRectangle = new Rectangle2D(point1[Vector.I1], point1[Vector.I2], 0, 0);
                if (currentPathRectangle == null)
                {
                    currentPathRectangle = pointRectangle;
                }
                else
                {
                    currentPathRectangle.Add(pointRectangle);
                }
            }
        }
        public Path RenderPath(PathPaintingRenderInfo renderInfo)
        {
            if (renderInfo.Operation != PathPaintingRenderInfo.NO_OP)
            {
                if (textRectangle == null)
                {
                    textRectangle = currentPathRectangle;
                }
                else
                {
                    textRectangle.Add(currentPathRectangle);
                }
            }
            currentPathRectangle = null;
            return null;
        }
        public void ClipPath(int rule)
        {

        }
    }
}
