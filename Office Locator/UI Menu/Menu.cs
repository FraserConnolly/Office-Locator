using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OffstageControls.OfficeLocator.UI
{
    public class Menu
    {

        #region Static Methods

        public static Menu RootMenu { get; private set; }
        public static Menu CurrentMenu { get; private set; }

        internal static void Press_Menu()
        {
            CurrentMenu = RootMenu;
            CurrentMenu?.Update();
        }


        public static void Press_Back()
        {
            CurrentMenu = CurrentMenu?.ParentMenu;
        
            if ( CurrentMenu == null )
            {
                // use root menu if there was no parent menu
                CurrentMenu = RootMenu;
            }
            CurrentMenu?.Update();

        }

        public static void Press_Ok()
        {
            CurrentMenu?.Ok();
            CurrentMenu?.Update();
        }

        public static void Press_Up()
        {
            CurrentMenu?.Up();
            CurrentMenu?.Update();
        }

        public static void Press_Down()
        {
            CurrentMenu?.Down();
            CurrentMenu?.Update();
        }

        internal static void SetCurrentMenu ( Menu menu )
        {
            if (menu == null)
                throw new ArgumentNullException();

            CurrentMenu = menu;
            CurrentMenu.Update();
        }

        #endregion

        public IDisplay Display { get; private set; }
        public string MenuHeading { get; private set; }
        public Menu ParentMenu { get; private set; } = null;

        public Menu(string RootMenuHeading, IDisplay display)
        {
            Display = display;
            MenuHeading = RootMenuHeading;
            RootMenu = this;
            CurrentMenu = this;
        }

        public Menu (string MenuHeading, Menu Parent)
        {
            if ( Parent == null || Parent.Display == null )
            {
                throw new ArgumentException("Parent menu can not be null.");
            }
            if ( string.IsNullOrWhiteSpace(MenuHeading))
            {
                MenuHeading = string.Empty;
            }
            this.MenuHeading = MenuHeading;
            ParentMenu = Parent;
            Display = Parent.Display;
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

        private void Ok()
        {
            SelectedItem?.Select( );
        }

        private readonly Color SelectedColour = Color.Blue;
        private readonly Color ItemColour = Color.White;
        
        public void AddMenuItem ( MenuItem item )
        {
            items.Add(item);

            if (items.Count == 1)
            {
                SelectedItem = items.First();
            }

            Update();
        }


        public void Update ( )
        {
            if (this != CurrentMenu)
                return;

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

            Display.WriteLine(int.MaxValue, MenuHeading, Color.Aqua);

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

        List<MenuItem> items = new();
        private MenuItem SelectedItem { get; set; }

    }
}
