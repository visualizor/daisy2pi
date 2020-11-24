using System;
using System.Collections.Generic;
using System.Drawing;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using RhinoWindows;
using Rhino.Display;
using Rhino.Input;
using Rhino.Input.Custom;

namespace Daisy2Pi
{
    public class D2Pi : Command
    {
        private MainUI mainui;
        internal CurveConduit crvcond;

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
            {
                mainui.Show(RhinoWinApp.MainWindow);
                if (crvcond != null)
                {
                    mainui.RefreshValues();
                    RedrawViews();
                }
                else
                {
                    crvcond = new CurveConduit(mainui.CrvGen(), mainui.crvclr)
                    {
                        Enabled = mainui.preview,
                    };
                    RhinoDoc.ActiveDoc.Views.Redraw();
                }
            }
            else
            {
                RhinoApp.WriteLine(" window is already open");
                mainui.BringToFront();
                mainui.RefreshValues();
                RedrawViews();
            }
            
            return Result.Success;
        }

        internal void RedrawViews()
        {
            try
            {
                crvcond.crvs = mainui.CrvGen();
                crvcond.clr = mainui.crvclr;
                crvcond.Enabled = mainui.preview;
            }
            catch (ArgumentNullException) { RhinoApp.WriteLine(" nothing to redraw"); }
            catch (NullReferenceException) { RhinoApp.WriteLine(" nothing to redraw"); }
            catch (Exception e) { RhinoApp.WriteLine(e.Message); }
            finally { RhinoDoc.ActiveDoc.Views.Redraw(); }
        }

    }

    public class CurveConduit : DisplayConduit
    {
        internal Curve[] crvs;
        internal Color clr;
        public CurveConduit(Curve[] cs, Color t)
        {
            crvs = cs;
            clr = t;
        }
        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            if (crvs == null)
            {
                base.CalculateBoundingBox(e);
                return;
            }
            foreach (Curve c in crvs)
                e.IncludeBoundingBox(c.GetBoundingBox(false)); 
        }
        protected override void DrawOverlay(DrawEventArgs e)
        {
            if (crvs == null)
            {
                base.DrawOverlay(e);
                return;
            }
            foreach (Curve c in crvs)
                e.Display.DrawCurve(c, clr);
        }
    }
}
