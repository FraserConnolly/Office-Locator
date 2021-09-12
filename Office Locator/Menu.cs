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

        public void AddSubMenu(string displayName, Menu subMenu)
        {
            items.Add(new SubMenuItem(this, displayName, subMenu));

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

        private class MenuItem
        {
            protected Menu menu { get; }
            public string DisplayString { get; private set; }
            public bool IsVisible { get; internal set; } = true;
            Action<string> run { get; }

            virtual public void Run()
            {
                run.Invoke( DisplayString );
            }

            protected MenuItem(Menu menu, string displayString)
            {
                this.menu = menu;
                DisplayString = displayString;
            }

            public MenuItem( Menu menu, string displayString, Action<string> run ) :  this ( menu, displayString )
            {
                this.run = run;
            }
        }

        private class SubMenuItem : MenuItem
        {
            private Menu subMenu;
            public override void Run()
            {
                CurrentMenu = subMenu;
            }

            public SubMenuItem(Menu menu, string displayString, Menu subMenu) : base ( menu, displayString )
            {
                this.subMenu = subMenu;
            }
        }

    }
}
