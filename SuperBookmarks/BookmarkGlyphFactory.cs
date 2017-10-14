using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Color = System.Drawing.Color;
using MediaColor = System.Windows.Media.Color;

namespace Konamiman.SuperBookmarks
{
    internal class BookmarkGlyphFactory : IGlyphFactory
    {
        const double m_glyphSize = 16.0;

        private static PointCollection points = PointCollection.Parse("0,1 12,1 12,14 6,10 0,14");

        public static Color DefaultColor { get; } = Color.DodgerBlue;

        public static SolidColorBrush Brush;

        static BookmarkGlyphFactory()
        {
            SetGlyphColor(DefaultColor);
        }

        public static void SetGlyphColor(Color color)
        {
            Brush = new SolidColorBrush(MediaColor.FromRgb(color.R, color.G, color.B));
        }

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            // Ensure we can draw a glyph for this marker.
            if (tag == null || !(tag is BookmarkTag))
            {
                return null;
            }

            return new Polyline
            {
                Points = points,
                Fill = Brush,
                StrokeThickness = 0
            };
        }
    }
}
