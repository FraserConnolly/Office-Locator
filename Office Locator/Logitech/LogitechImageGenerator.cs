using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OffstageControls.OfficeLocator.Logitech
{
    class LogitechImageGenerator
    {
        private Bitmap bitmap;
        private Graphics g;
        private LogitechG19 keyboard;
        private SolidBrush sBrush;
        private Rectangle Menu = new Rectangle( 220, 0, 100, 40 );

        public LogitechImageGenerator()
        {
            sBrush = new SolidBrush( Color.Black );
            bitmap = new Bitmap( LogitechG19.LOGI_LCD_COLOR_WIDTH, LogitechG19.LOGI_LCD_COLOR_HEIGHT, System.Drawing.Imaging.PixelFormat.Format32bppPArgb );
            g = Graphics.FromImage( bitmap );
            keyboard = LogitechG19.GetG19Keyboard( );
        }

        public void SetBackgroundColour ( Color colour )
        {
            g.FillRectangle( new SolidBrush( colour ), new Rectangle( 0, 0, LogitechG19.LOGI_LCD_COLOR_WIDTH, LogitechG19.LOGI_LCD_COLOR_HEIGHT ) );
            g.Save( );
        }

        public void DrawMenuHeader ( string text, Color colour )
        {
            sBrush.Color = colour;
            g.FillRectangle( sBrush, Menu );
            g.DrawString( text, new Font( FontFamily.GenericSerif, 20 ), new SolidBrush( Color.Black ), Menu.Location );
            g.Save( );
        }

        public void UpdateKeyboard( )
        {
            keyboard.SetImage( bitmap );
        }

        //internal ImageSource GetImage()
        //{
        //    using ( MemoryStream memory = new MemoryStream( ) )
        //    {
        //        bitmap.Save( memory, System.Drawing.Imaging.ImageFormat.Bmp );
        //        memory.Position = 0;
        //        BitmapImage bitmapimage = new BitmapImage( );
        //        bitmapimage.BeginInit( );
        //        bitmapimage.StreamSource = memory;
        //        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapimage.EndInit( );

        //        return bitmapimage;
        //    }
        //}
    }

    public static class ImageExtensions
    {
        public static byte [ ] ToByteArray( this Bitmap image )
        {
            byte [ ] b = new byte [ image.Width * image.Height * 4 ];
            int i = 0;
            for ( int y = 0 ; y < image.Height ; y++ )
            {
                for ( int x = 0 ; x < image.Width ; x++ )
                {
                    var c = image.GetPixel( x, y );
                    b [ i++ ] = c.B;
                    b [ i++ ] = c.G;
                    b [ i++ ] = c.R;
                    b [ i++ ] = c.A;
                }
            }
            return b;
        }
    }
}
