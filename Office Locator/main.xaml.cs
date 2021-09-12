using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OffstageControls.HPPOSDriver;
using OffstageControls.LogitechG19SDriver;
using System.Drawing;
using System.Timers;
using System.Windows.Media.Imaging;
using OffstageControls.OfficeLocator.UI;
using Microsoft.Owin.Hosting;
using Microsoft.Owin;

namespace OffstageControls.OfficeLocator
{
    /// <summary>
    /// Interaction logic for main.xaml
    /// </summary>
    public partial class main : Window
    {
        private LogitechG19 keyboard;
        private Menu rootMenu;
        POSDisplay display;
        LogitechImageGenerator imgGen;

        public main()
        {
            InitializeComponent( );
        }

        public void Start( )
        {
            try
            {
                display = new POSDisplay( Properties.Settings.Default["COMPort"].ToString( ) );
            }
            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message );
            }

            keyboard = LogitechG19.GetG19Keyboard( );
            rootMenu = new Menu( "Office Locator", keyboard );

            Menu setDisplayMenu = new Menu("Set Display", rootMenu);
            
            setDisplayMenu.AddMenuItem( new ActionMenuItem(setDisplayMenu, "Open", runCmd, activeLabelCheck ) );
            setDisplayMenu.AddMenuItem( new ActionMenuItem(setDisplayMenu, "Phone", runCmd, activeLabelCheck) );
            setDisplayMenu.AddMenuItem( new ActionMenuItem(setDisplayMenu, "On Call", runCmd, activeLabelCheck) );
            setDisplayMenu.AddMenuItem( new ActionMenuItem(setDisplayMenu, "Closed for lunch", runCmd, activeLabelCheck) );
            setDisplayMenu.AddMenuItem( new ActionMenuItem(setDisplayMenu, "Gone home", runCmd, activeLabelCheck) );

            rootMenu.AddMenuItem(new SubMenuItem(rootMenu, "Set Display", setDisplayMenu) );

            keyboard.OnButtonDown += Keyboard_OnButtonDown;

            string url = "http://192.168.1.190:8085";
            WebApp.Start(url);
            Console.WriteLine("Server running on {0}", url);

        }

        string currentLabel = string.Empty;

        private void runCmd( string label )
        {
            if ( display == null )
                return;

            switch ( label )
            {
                case "Open":
                    display.ClearScreen( );
                    display.WriteAtCursor( "Fraser Connolly" );
                    display.WriteAtCursor( "         Open" );
                    break;
                case "Closed for lunch":
                    display.ClearScreen( );
                    display.WriteAtCursor( "Fraser Connolly" );
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
            imgGen.UpdateKeyboard( );

            currentLabel = label;
        }

        private bool activeLabelCheck ( string label )
        {
            return currentLabel == label;
        }

        private void Keyboard_OnButtonDown( LogitechG19.Button btn )
        {
            switch (btn)
            {
                case LogitechG19.Button.Cancel:
                case LogitechG19.Button.Left:
                    Menu.Press_Back( );
                    break;
                case LogitechG19.Button.Up:
                    Menu.Press_Up( );
                    break;
                case LogitechG19.Button.Down:
                    Menu.Press_Down( );
                    break;
                case LogitechG19.Button.Menu:
                    Menu.Press_Menu( );
                    break;
                case LogitechG19.Button.Right:
                case LogitechG19.Button.Ok:
                    Menu.Press_Ok( );
                    break;
                default:
                    break;
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
