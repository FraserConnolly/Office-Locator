using System.Drawing;

namespace OffstageControls.OfficeLocator
{
    public interface IDisplay
    {
        int MaxLines { get; }
        void WriteLine( int lineNumber, string text, Color colour );
        void WriteTitle( string title, Color colour );
    }
}