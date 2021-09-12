using System.Drawing;

namespace OffstageControls
{
    public interface IDisplay
    {
        int MaxLines { get; }
        void WriteLine( int lineNumber, string text, Color colour );
        void WriteTitle( string title, Color colour );
    }
}