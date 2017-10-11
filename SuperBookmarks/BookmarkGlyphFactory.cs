using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace Konamiman.SuperBookmarks
{
    internal class BookmarkGlyphFactory : IGlyphFactory
    {
        const double m_glyphSize = 16.0;

        private static PointCollection points = PointCollection.Parse("0,1 12,1 12,14 6,10 0,14");

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            // Ensure we can draw a glyph for this marker.
            if (tag == null || !(tag is BookmarkTag))
            {
                return null;
            }

            var shape = new Polyline();
            shape.Points = points;
            shape.Fill = Brushes.LightBlue;
            shape.StrokeThickness = 0;
            shape.Stroke = Brushes.DarkBlue;
            return shape;
        }
    }
}
