using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OffstageControls.OfficeLocator.UI
{
    public abstract class MenuItem
    {
    
        protected Menu parent { get; }
        public virtual string DisplayString { get; private set; }

        protected MenuItem(Menu menu, string displayString)
        {
            parent = menu;
            DisplayString = displayString;
        }

        public abstract void Select();

        /// <summary>
        /// True if this menu item can be selected.
        /// </summary>
        public abstract bool IsEnabled { get; }
        
        /// <summary>
        /// True if this menu item is in use.
        /// </summary>
        public abstract bool IsActive { get; }

        /// <summary>
        /// True if this menu item can be displayed.
        /// </summary>
        public bool IsVisible { get; internal set; } = true;

    }


    public class ActionMenuItem : MenuItem
    {
        Action<string> run { get; }
        Func<string, bool> isActiveCheck;

        public override bool IsEnabled => true;
        public override bool IsActive => isActiveCheck?.Invoke( base.DisplayString ) ?? false;
        public override string DisplayString => IsActive ? base.DisplayString + "*" : base.DisplayString;

        public override void Select()
        {
            run.Invoke( DisplayString );
        }

        public ActionMenuItem( Menu menu, string displayString, Action<string> run ) :  base ( menu, displayString )
        {
            this.run = run;
        }

        public ActionMenuItem(Menu menu, string displayString, Action<string> run, Func<string, bool> isActiveCheck) : this (menu, displayString, run)
        {
            this.isActiveCheck = isActiveCheck;
        }

    }

    public class SubMenuItem : MenuItem
    {
        private Menu subMenu;
        
        public override bool IsEnabled => true;

        public override bool IsActive => false;

        public override void Select()
        {
            Menu.SetCurrentMenu ( subMenu );
        }

        public SubMenuItem(Menu menu, string displayString, Menu subMenu) : base ( menu, displayString )
        {
            this.subMenu = subMenu;
        }
    }

}
