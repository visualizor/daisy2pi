using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using RhinoWindows;
using Rhino.Input;
using Rhino.Input.Custom;

namespace Daisy2Pi
{
    public class D2Pi : Command
    {
        private MainUI mainui;
        public D2Pi()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static D2Pi Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "daisy2pi"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (mainui == null) mainui = new MainUI();

            if (!mainui.Visible)
                mainui.Show(RhinoWinApp.MainWindow);
            else
            {
                RhinoApp.WriteLine(" window is already open");
                mainui.BringToFront();
            }
            

            return Result.Success;
        }
    }
}
