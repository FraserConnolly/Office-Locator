using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OffstageControls.OfficeLocator
{
    public class POSDisplay
    {
        SerialPort port;
        Encoding ascii;

        public POSDisplay( string comPort )
        {
            ascii = Encoding.ASCII;
            port = new SerialPort( comPort, baudRate: 9600, parity: Parity.None, dataBits: 8, stopBits: StopBits.One );
            port.Open( );
            Initalise( );
        }

        public void Initalise()
        {
            Write( 0x1B, 0x40 );
        }

        public void WriteAtCursor( string text )
        {
            Write( ascii.GetBytes( text ) );
        }

        public void WriteAtPosition( string text, int r, int c )
        {
            MoveCursor( r, c );
            WriteAtCursor( text );
        }

        //Also moves cursor to top left.
        public void ClearScreen( )
        {
            Write( 0x0C );
        }

        public void MoveCursorHome ( )
        {
            MoveCursor( 1, 1 );
        }

        public void MoveCursorToSecondLine()
        {
            MoveCursor( 2, 1 );
        }

        public void MoveCursor ( int r, int c  )
        {
            if ( r > 20 || c > 2 )
                return;

            if ( r == 0 || c == 0 )
                return;

            Write( 0x1f, 0x24, ( byte ) r, ( byte ) c );
        }

        private void Write( params byte [ ] buffer )
        {
            if ( port != null && port.IsOpen )
            {
                port.Write( buffer, 0, buffer.Length );
            }
        }
    }
}
