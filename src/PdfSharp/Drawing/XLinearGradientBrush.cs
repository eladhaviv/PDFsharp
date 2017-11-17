#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.ComponentModel;
using PdfSharp.Internal;
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
using GdiLinearGradientBrush =System.Drawing.Drawing2D.LinearGradientBrush;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
using SysPoint = System.Windows.Point;
using SysSize = System.Windows.Size;
using SysRect = System.Windows.Rect;
using WpfBrush = System.Windows.Media.Brush;
#endif
#if UWP
using Windows.UI;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
#endif

// ReSharper disable RedundantNameQualifier because it is required for hybrid build

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Defines a Brush with a linear gradient.
    /// </summary>
    public sealed class XLinearGradientBrush : XBrush
    {
        //internal XLinearGradientBrush();

#if GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(System.Drawing.Point point1, System.Drawing.Point point2, XColor color1, XColor color2)
            : this(new XPoint(point1), new XPoint(point2), color1, color2)
        { }
#endif

#if WPF
        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(SysPoint point1, SysPoint point2, XColor color1, XColor color2)
            : this(new XPoint(point1), new XPoint(point2), color1, color2)
        { }
#endif

#if GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(PointF point1, PointF point2, XColor color1, XColor color2)
            : this(new XPoint(point1), new XPoint(point2), color1, color2)
        { }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(XPoint point1, XPoint point2, XColor color1, XColor color2)
            : this(point1, point2, new XColor[] { color1, color2 }, new float[] { 0.0f, 1.0f })
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(XPoint point1, XPoint point2, XColor[] colors, float[] offsets)
        {
            if (point1 == point2)
                throw new ArgumentException("Expected inequal bounds.");

            if (colors.Length < 2)
                throw new ArgumentException("Expected at least 2 colors.", "colors");

            if (colors.Length != offsets.Length)
                throw new ArgumentException("Expected equally sized arrays", "colors, offsets");
            
            _point1 = point1;
            _point2 = point2;
            _colors = colors.Clone() as XColor[];
            _offsets = offsets.Clone() as float[];
            Array.Sort(_offsets, _colors);
        }

#if GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(Rectangle rect, XColor color1, XColor color2, XLinearGradientMode linearGradientMode)
            : this(new XRect(rect), color1, color2, linearGradientMode)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(RectangleF rect, XColor color1, XColor color2, XLinearGradientMode linearGradientMode)
            : this(new XRect(rect), color1, color2, linearGradientMode)
        { }
#endif

#if WPF
        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(Rect rect, XColor color1, XColor color2, XLinearGradientMode linearGradientMode)
            : this(new XRect(rect), color1, color2, linearGradientMode)
        { }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(XRect rect, XColor color1, XColor color2, XLinearGradientMode linearGradientMode)
            : this(rect, new XColor[] { color1, color2 }, new float[] { 0.0f, 1.0f }, linearGradientMode)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(XRect rect, XColor[] colors, float[] offsets, XLinearGradientMode linearGradientMode)
            : this(
                  linearGradientMode == XLinearGradientMode.BackwardDiagonal ? rect.TopRight : rect.TopLeft,
                  linearGradientMode == XLinearGradientMode.Horizontal ? rect.TopRight : linearGradientMode == XLinearGradientMode.ForwardDiagonal ? rect.BottomRight : rect.BottomLeft,
                  colors,
                  offsets)
        {
            if (!Enum.IsDefined(typeof(XLinearGradientMode), linearGradientMode))
                throw new InvalidEnumArgumentException("linearGradientMode", (int)linearGradientMode, typeof(XLinearGradientMode));
        }

        // TODO: 
        //public XLinearGradientBrush(Rectangle rect, XColor color1, XColor color2, double angle);
        //public XLinearGradientBrush(RectangleF rect, XColor color1, XColor color2, double angle);
        //public XLinearGradientBrush(Rectangle rect, XColor color1, XColor color2, double angle, bool isAngleScaleable);
        //public XLinearGradientBrush(RectangleF rect, XColor color1, XColor color2, double angle, bool isAngleScaleable);
        //public XLinearGradientBrush(RectangleF rect, XColor color1, XColor color2, double angle, bool isAngleScaleable);

        //private Blend _GetBlend();
        //private ColorBlend _GetInterpolationColors();
        //private XColor[] _GetLinearColors();
        //private RectangleF _GetRectangle();
        //private Matrix _GetTransform();
        //private WrapMode _GetWrapMode();
        //private void _SetBlend(Blend blend);
        //private void _SetInterpolationColors(ColorBlend blend);
        //private void _SetLinearColors(XColor color1, XColor color2);
        //private void _SetTransform(Matrix matrix);
        //private void _SetWrapMode(WrapMode wrapMode);

        //public override object Clone();

        /// <summary>
        /// Gets or sets an XMatrix that defines a local geometric transform for this LinearGradientBrush.
        /// </summary>
        public XMatrix Transform
        {
            get { return _matrix; }
            set { _matrix = value; }
        }

        /// <summary>
        /// Translates the brush with the specified offset.
        /// </summary>
        public void TranslateTransform(double dx, double dy)
        {
            _matrix.TranslatePrepend(dx, dy);
        }

        /// <summary>
        /// Translates the brush with the specified offset.
        /// </summary>
        public void TranslateTransform(double dx, double dy, XMatrixOrder order)
        {
            _matrix.Translate(dx, dy, order);
        }

        /// <summary>
        /// Scales the brush with the specified scalars.
        /// </summary>
        public void ScaleTransform(double sx, double sy)
        {
            _matrix.ScalePrepend(sx, sy);
        }

        /// <summary>
        /// Scales the brush with the specified scalars.
        /// </summary>
        public void ScaleTransform(double sx, double sy, XMatrixOrder order)
        {
            _matrix.Scale(sx, sy, order);
        }

        /// <summary>
        /// Rotates the brush with the specified angle.
        /// </summary>
        public void RotateTransform(double angle)
        {
            _matrix.RotatePrepend(angle);
        }

        /// <summary>
        /// Rotates the brush with the specified angle.
        /// </summary>
        public void RotateTransform(double angle, XMatrixOrder order)
        {
            _matrix.Rotate(angle, order);
        }

        /// <summary>
        /// Multiply the brush transformation matrix with the specified matrix.
        /// </summary>
        public void MultiplyTransform(XMatrix matrix)
        {
            _matrix.Prepend(matrix);
        }

        /// <summary>
        /// Multiply the brush transformation matrix with the specified matrix.
        /// </summary>
        public void MultiplyTransform(XMatrix matrix, XMatrixOrder order)
        {
            _matrix.Multiply(matrix, order);
        }

        /// <summary>
        /// Resets the brush transformation matrix with identity matrix.
        /// </summary>
        public void ResetTransform()
        {
            _matrix = new XMatrix();
        }

        //public void SetBlendTriangularShape(double focus);
        //public void SetBlendTriangularShape(double focus, double scale);
        //public void SetSigmaBellShape(double focus);
        //public void SetSigmaBellShape(double focus, double scale);

#if GDI
        internal override System.Drawing.Brush RealizeGdiBrush()
        {
            //if (dirty)
            //{
            //  if (brush == null)
            //    brush = new SolidBrush(color.ToGdiColor());
            //  else
            //  {
            //    brush.Color = color.ToGdiColor();
            //  }
            //  dirty = false;
            //}

            // TODO: use dirty to optimize code
            GdiLinearGradientBrush brush;
            try
            {
                Lock.EnterGdiPlus();

                brush = new GdiLinearGradientBrush(
                    _point1.ToPointF(), _point2.ToPointF(),
                    _colors[0].ToGdiColor(), _colors[_colors.Length - 1].ToGdiColor());
                brush.InterpolationColors = new ColorBlend(_colors.Length);
                var colors = new Color[_colors.Length];
                for (int i = 0; i < _colors.Length; i++)
                    colors[i] = _colors[i].ToGdiColor();
                brush.InterpolationColors.Colors = colors;
                brush.InterpolationColors.Positions = _offsets;
                if (!_matrix.IsIdentity)
                    brush.Transform = _matrix.ToGdiMatrix();
                //brush.WrapMode = WrapMode.Clamp;
            }
            finally { Lock.ExitGdiPlus(); }
            return brush;
        }
#endif

#if WPF
        internal override WpfBrush RealizeWpfBrush()
        {
            //if (dirty)
            //{
            //  if (brush == null)
            //    brush = new SolidBrush(color.ToGdiColor());
            //  else
            //  {
            //    brush.Color = color.ToGdiColor();
            //  }
            //  dirty = false;
            //}

            System.Windows.Media.LinearGradientBrush brush;
            GradientStopCollection gsc = new GradientStopCollection();
            for (int i = 0; i < _colors.Length; i++)
                gsc.Add(new GradientStop() { Color = _colors[i].ToWpfColor(), Offset = _offsets[i] });
#if !SILVERLIGHT
            brush = new LinearGradientBrush(gsc, _point1, _point2);
#else
            brush = new LinearGradientBrush(gsc, 0);
            brush.StartPoint = _point1;
            brush.EndPoint = _point2;
#endif
            if (!_matrix.IsIdentity)
            {
#if !SILVERLIGHT
                brush.Transform = new MatrixTransform(_matrix.ToWpfMatrix());
#else
                MatrixTransform transform = new MatrixTransform();
                transform.Matrix = _matrix.ToWpfMatrix();
                brush.Transform = transform;
#endif
            }
            return brush;
        }
#endif

#if UWP
        internal override ICanvasBrush RealizeCanvasBrush()
        {
            ICanvasBrush brush;

            brush = new CanvasSolidColorBrush(CanvasDevice.GetSharedDevice(), Colors.RoyalBlue);

            return brush;
        }
#endif

        //public Blend Blend { get; set; }
        //public bool GammaCorrection { get; set; }
        //public ColorBlend InterpolationColors { get; set; }
        //public XColor[] LinearColors { get; set; }
        //public RectangleF Rectangle { get; }
        //public WrapMode WrapMode { get; set; }
        //private bool interpolationColorsWasSet;

        internal XPoint _point1, _point2;
        internal XColor[] _colors;
        internal float[] _offsets;

        internal XMatrix _matrix;
    }
}