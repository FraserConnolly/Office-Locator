using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;

namespace OffstageControls.OfficeLocator.Logitech
{
    public class LogitechG19 : IDisplay
    {
        private const int LOGI_LCD_COLOR_BUTTON_LEFT = ( 0x00000100 );
        private const int LOGI_LCD_COLOR_BUTTON_RIGHT = ( 0x00000200 );
        private const int LOGI_LCD_COLOR_BUTTON_OK = ( 0x00000400 );
        private const int LOGI_LCD_COLOR_BUTTON_CANCEL = ( 0x00000800 );
        private const int LOGI_LCD_COLOR_BUTTON_UP = ( 0x00001000 );
        private const int LOGI_LCD_COLOR_BUTTON_DOWN = ( 0x00002000 );
        private const int LOGI_LCD_COLOR_BUTTON_MENU = ( 0x00004000 );

        public enum Button : int
        {
            Left = LOGI_LCD_COLOR_BUTTON_LEFT,
            Right = LOGI_LCD_COLOR_BUTTON_RIGHT,
            Up = LOGI_LCD_COLOR_BUTTON_UP,
            Down = LOGI_LCD_COLOR_BUTTON_DOWN,
            Menu = LOGI_LCD_COLOR_BUTTON_MENU,
            Ok = LOGI_LCD_COLOR_BUTTON_OK,
            Cancel = LOGI_LCD_COLOR_BUTTON_CANCEL
        }

        Dictionary<int, bool> buttonState;

        public enum Line : int
        {
            Title = int.MaxValue,
            Line1 = 0,
            Line2,
            Line3,
            Line4,
            Line5,
            Line6,
            Line7,
            Line8
        }

        public static readonly int LOGI_LCD_COLOR_WIDTH = 320;
        public static readonly int LOGI_LCD_COLOR_HEIGHT = 240;
        private const int LOGI_LCD_TYPE_COLOR = ( 0x00000002 );

        #region C++ Wrapper

        [DllImport( "LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl )]
        static extern bool LogiLcdInit( String friendlyName, int lcdType );

        [DllImport( "LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl )]
        static extern bool LogiLcdIsConnected( int lcdType );

        [DllImport( "LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl )]
        static extern bool LogiLcdIsButtonPressed( int button );

        private static bool IsButtonPressed( Button btn )
            => LogiLcdIsButtonPressed( ( int ) btn );


        [DllImport( "LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl )]
        static extern void LogiLcdUpdate();

        [DllImport( "LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl )]
        static extern void LogiLcdShutdown();
        
        // Colour LCD functions
        [ DllImport( "LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl ) ]
        static extern bool LogiLcdColorSetBackground( byte [ ] colorBitmap );

        [DllImport( "LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl )]
        static extern bool LogiLcdColorSetTitle( String text, int red , int green , int blue );

        [DllImport( "LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl )]
        static extern bool LogiLcdColorSetText( int lineNumber, String text, int red, int green, int blue );

        #endregion

        #region Singleton

        private static LogitechG19 keyboard = null;

        public static LogitechG19 GetG19Keyboard ( )
        {
            if ( keyboard == null )
            {
                keyboard = new LogitechG19( );
            }

            return keyboard;
        }

        #endregion

        #region Constructor

        private LogitechG19( )
        {
            try
            {
                LogiLcdInit( "Office Locator", LOGI_LCD_TYPE_COLOR );
            }
            catch ( Exception ex )
            {

                throw;
            }

            buttonState = new Dictionary<int, bool>( );

            foreach ( var btn in Enum.GetValues( typeof( Button ) ) )
            {
                buttonState.Add( ( int ) btn, false );
            }

            updateTimer = new Timer( 30 ); // 30 millisecond update timer
            updateTimer.Elapsed += UpdateTimer_Elapsed;
            updateTimer.AutoReset = false;
            updateTimer.Start( );
        }

        #endregion

        private Timer updateTimer = new Timer( 30 );

        public bool IsConnected { get; private set; }


        #region Buttons

        public delegate void ButtonDown ( Button btn );
        public event ButtonDown OnButtonDown;

        private void UpdateTimer_Elapsed( object sender, ElapsedEventArgs e )
        {
            LogiLcdUpdate( );

            foreach ( Button btn in Enum.GetValues( typeof( Button ) ) )
            {
                var state = IsButtonPressed( btn );
                if ( state && !buttonState [ ( int ) btn ] )
                {
                    OnButtonDown.Invoke( btn );
                }

                buttonState [ ( int ) btn ] = state;
            }

            IsConnected = LogiLcdIsConnected( LOGI_LCD_TYPE_COLOR );
            updateTimer.Start( );
        }

        #endregion

        public void Shutdown ()
        {
            keyboard = null;
            updateTimer.Stop( );
            LogiLcdShutdown( );
        }

        public void Write ( string text, Line line )
        {
            Write( text, line, Color.White );
        }

        public void Write ( string text, Line line, Color colour )
        {
            if ( line == Line.Title )
            {
                LogiLcdColorSetTitle( text, colour.R, colour.G, colour.B );
            }
            else
            {
                LogiLcdColorSetText( ( int ) line, text, colour.R, colour.G, colour.B );
            }
        }

        int IDisplay.MaxLines => 8;

        void IDisplay.WriteLine( int lineNumber, string text, Color colour )
        {
            Write( text, ( Line ) lineNumber, colour );
        }

        void IDisplay.WriteTitle( string title, Color colour )
        {
            Write( title, Line.Title, colour );
        }

        public void SetImage ( Bitmap image )
        {
            if ( image == null )
            {
                throw new ArgumentNullException( "image can not be null" );
            }

            if ( image.Height == LOGI_LCD_COLOR_HEIGHT 
                 && image.Width == LOGI_LCD_COLOR_WIDTH )
            {
                ImageConverter converter = new ImageConverter( );
                byte [ ] b = image.ToByteArray( );
                LogiLcdColorSetBackground( b );
            }
            else
            {
                throw new ArgumentException( "Image is the wrong size" );
            }
        }
    }
}
