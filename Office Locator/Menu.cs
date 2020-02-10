using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OffstageControls.OfficeLocator
{
    public class Menu
    {
        IDisplay Display;

        public Menu( IDisplay display )
        {
            Display = display;
            items = new List<MenuItem>( );
        }

        #region Movement

        public void Down()
        {
            if ( CountVisibleMenuItems == 0 )
            {
                return;
            }

            List<MenuItem> vis = VisibleMenuItems;

            if ( vis.Last( ) == SelectedItem )
            {
                SelectedItem = vis.First( );
                Update( );
                return;
            }

            SelectedItem = vis [ vis.IndexOf( SelectedItem ) + 1 ];

            Update( );
        }

        public void Up()
        {
            if ( items.Count == 0 )
            {
                Update( );
            }

            List<MenuItem> vis = VisibleMenuItems;

            if ( items.First( ) == SelectedItem )
            {
                SelectedItem = vis.Last( );
                Update( );
                return;
            }

            SelectedItem = vis [ vis.IndexOf( SelectedItem ) - 1 ];

            Update( );
        }

        #endregion

        public void Ok()
        {
            SelectedItem?.Run( );
        }

        private readonly Color SelectedColour = Color.Blue;
        private readonly Color ItemColour = Color.White;

        public void AddMenuItem ( string displayName, Action<string> action )
        {
            items.Add( new MenuItem( this, displayName, action ) );

            if ( items.Count == 1 )
            {
                SelectedItem = items.First( );
            }

            Update( );
        }

        public void Update ( )
        {
            if (SelectedItem == null )
            {
                SelectedItem = VisibleMenuItems.FirstOrDefault( );
            }

            int lineNumber = 0;

            bool ScrollNeeded = Display.MaxLines < CountVisibleMenuItems;

            if ( ScrollNeeded )
            {
                throw new NotImplementedException( "Too many visible menu items" );
            }

            foreach ( var item in VisibleMenuItems )
            {
                Display.WriteLine( 
                    lineNumber++, 
                    item.DisplayString, 
                    SelectedItem == item ? SelectedColour : ItemColour );
            }

            while ( lineNumber < Display.MaxLines )
            {
                // blank the unused lines.
                Display.WriteLine(lineNumber++,"", Color.White);
            }
        }

        int CountVisibleMenuItems => items.Where( i => i.IsVisible ).Count( );
        List<MenuItem> VisibleMenuItems => items.Where( i => i.IsVisible ).ToList( );

        List<MenuItem> items;
        private MenuItem SelectedItem { get; set; }

        private class MenuItem
        {
            private Menu menu;
            public string DisplayString { get; private set; }
            public bool IsVisible { get; internal set; } = true;
            Action<string> run { get; }

            public void Run()
            {
                run.Invoke( DisplayString );
            }

            public MenuItem( Menu menu, string displayString, Action<string> run )
            {
                this.menu = menu;
                DisplayString = displayString;
                this.run = run;
            }
        }
    }
}
