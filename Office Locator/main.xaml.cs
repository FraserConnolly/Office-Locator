using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OffstageControls.OfficeLocator.Logitech;

namespace OffstageControls.OfficeLocator
{
    /// <summary>
    /// Interaction logic for main.xaml
    /// </summary>
    public partial class main : Window
    {
        private LogitechG19 keyboard;
        private Menu menu;
        POSDisplay display;

        public main()
        {
            InitializeComponent( );
        }

        public void Start( string comPort = "COM1" )
        {
            try
            {
                display = new POSDisplay( comPort );
            }
            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message );
            }
            keyboard = LogitechG19.GetG19Keyboard( );
            menu = new Menu( keyboard );
            menu.AddMenuItem( "Open", runCmd );
            menu.AddMenuItem( "Phone", runCmd );
            menu.AddMenuItem( "On Call", runCmd );
            menu.AddMenuItem( "Closed for lunch", runCmd );
            menu.AddMenuItem( "Gone home", runCmd );
            keyboard.OnButtonDown += Keyboard_OnButtonDown;
        }

        private void runCmd( string label )
        {
            if ( display == null )
                return;

            switch ( label )
            {
                case "Open":
                    display.ClearScreen( );
                    display.WriteAtCursor( "Fraser Connolly FHEA" );
                    display.WriteAtCursor( "         Open" );
                    break;
                case "Closed for lunch":
                    display.ClearScreen( );
                    display.WriteAtCursor( "Fraser Connolly FHEA" );
                    display.WriteAtPosition( "  Closed for lunch", 1, 2 );
                    break;
                case "Gone home":
                    display.ClearScreen( );
                    display.WriteAtCursor( "       Closed");
                    display.WriteAtPosition( "Back at 9am tomorrow", 1, 2 );
                    break;
                case "Phone":
                    display.ClearScreen( );
                    display.WriteAtCursor( "  I'm on the phone." );
                    display.WriteAtPosition( "  Please wait ...", 1, 2 );
                    break;
                case "On Call":
                    display.ClearScreen( );
                    display.WriteAtCursor( "Back in 15 minutes." );
                    break;
                default:
                    break;
            }
        }

        private void Keyboard_OnButtonDown( LogitechG19.Button btn )
        {
            if ( btn == LogitechG19.Button.Ok )
            {
                menu.Ok( );
            }
            else if ( btn == LogitechG19.Button.Down )
            {
                menu.Down( );
            }
            else if ( btn == LogitechG19.Button.Up)
            {
                menu.Up( );
            }
        }

        private void Window_Closed( object sender, EventArgs e )
        {
            display?.ClearScreen( );
            keyboard.Shutdown( );
            keyboard = null;
        }


    }
}
