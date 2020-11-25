using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rhino.Geometry;
using Rhino;

namespace Daisy2Pi
{
    public partial class MainUI : Form
    {
        internal double f0;
        internal double f1;
        internal double f2;
        internal double f3;
        internal double f4;
        internal double f5;
        internal double f6;
        internal double f7;
        internal Color crvclr;
        internal bool interpolated = true;
        internal D2Pi cmd = D2Pi.Instance; // refer to the rhino command static property
        internal bool preview = true;

        /// <summary>
        /// constructor
        /// </summary>
        public MainUI()
        {
            InitializeComponent();
            RefreshValues();

            FormClosing += OnClosing;
            Icon = Properties.Resources.daisy;

            Trig0.SelectedIndex = 0;
            Trig1.SelectedIndex = 1;
            Trig2.SelectedIndex = 1;
            Trig3.SelectedIndex = 0;
            Op0.SelectedIndex = 0;
            Op1.SelectedIndex = 1;
            U0LBL.Text = string.Format("{0:0.00}", U0SL.Value / 100.0 * 2 * Math.PI);
            U1LBL.Text = string.Format("{0:0.00}", U1SL.Value / 100.0 * 2 * Math.PI);

            var menustrip = new ContextMenu(); //empty context strip
            Trig0.KeyPress += BlockKeyPress;
            Trig0.SelectedIndexChanged += OnTrigChange;
            Trig0.ContextMenu = menustrip;
            Trig1.KeyPress += BlockKeyPress;
            Trig1.SelectedIndexChanged += OnTrigChange;
            Trig1.ContextMenu = menustrip;
            Trig2.KeyPress += BlockKeyPress;
            Trig2.SelectedIndexChanged += OnTrigChange;
            Trig2.ContextMenu = menustrip;
            Trig3.KeyPress += BlockKeyPress;
            Trig3.SelectedIndexChanged += OnTrigChange;
            Trig3.ContextMenu = menustrip;
            Op0.KeyPress += BlockKeyPress;
            Op0.SelectedIndexChanged += OnTrigChange;
            Op0.ContextMenu = menustrip;
            Op1.KeyPress += BlockKeyPress;
            Op1.SelectedIndexChanged += OnTrigChange;
            Op1.ContextMenu = menustrip;

            CoEf0.ValueChanged += OnNUD;
            CoEf0.ContextMenu = menustrip;
            CoEf1.ValueChanged += OnNUD;
            CoEf1.ContextMenu = menustrip;
            CoEf2.ValueChanged += OnNUD;
            CoEf3.ContextMenu = menustrip;
            CoEf3.ValueChanged += OnNUD;
            CoEf4.ValueChanged += OnNUD;
            CoEf4.ContextMenu = menustrip;
            CoEf5.ValueChanged += OnNUD;
            CoEf5.ContextMenu = menustrip;
            CoEf6.ValueChanged += OnNUD;
            CoEf6.ContextMenu = menustrip;
            CoEf7.ValueChanged += OnNUD;
            CoEf7.ContextMenu = menustrip;
        }

        #region event handlers

        private void OnNUD(object s, EventArgs e)
        {
            NumericUpDown nud = s as NumericUpDown;
            switch (nud.Name)
            {
                case "CoEf0":
                    f0 = (double)nud.Value;
                    break;
                case "CoEf1":
                    f1 = (double)nud.Value;
                    break;
                case "CoEf2":
                    f2 = (double)nud.Value;
                    break;
                case "CoEf3":
                    f3 = (double)nud.Value;
                    break;
                case "CoEf4":
                    f4 = (double)nud.Value;
                    break;
                case "CoEf5":
                    f5 = (double)nud.Value;
                    break;
                case "CoEf6":
                    f6 = (double)nud.Value;
                    break;
                case "CoEf7":
                    f7 = (double)nud.Value;
                    break;
                default: break;
            }
            cmd.RedrawViews();
        }

        private void OnTrigChange(object s, EventArgs e)
        {
            cmd.RedrawViews();
        }

        private void U0SL_Scroll(object sender, EventArgs e)
        {
            U0LBL.Text = string.Format("{0:0.00}", U0SL.Value / 100.0 * 2 * Math.PI);
            if (U0SL.Value == U0SL.Maximum) U0LBL.Text = @"2π";
            cmd.RedrawViews();
        }

        private void U1SL_Scroll(object sender, EventArgs e)
        {
            U1LBL.Text = string.Format("{0:0.00}", U1SL.Value / 100.0 * 2 * Math.PI);
            if (U1SL.Value == U1SL.Maximum) U1LBL.Text = @"2π";
            cmd.RedrawViews();
        }

        private void UCtSL_Scroll(object sender, EventArgs e)
        {
            UCtLBL.Text = UCtSL.Value.ToString();
            cmd.RedrawViews();
        }

        private void BlockKeyPress(object s, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void OnClosing(object s, FormClosingEventArgs e)
        {
            preview = false;
            cmd.RedrawViews();
            Hide(); // only allow this in the rhino context
            e.Cancel = true;
        }

        private void ClrBTN_Click(object sender, EventArgs e)
        {
            DialogResult r = ColorPicker0.ShowDialog(this);
            crvclr = ColorPicker0.Color;
            ClrBTN.BackColor = crvclr;
            cmd.RedrawViews();
        }

        private void PreviewBTN_Click(object sender, EventArgs e)
        {
            preview = !preview;
            if (preview) PreviewBTN.Text = "Hide Preview";
            else PreviewBTN.Text = "Show Preview";
            cmd.RedrawViews();
        }

        private void AddBTN_Click(object sender, EventArgs e)
        {
            foreach (Curve c in CrvGen())
                RhinoDoc.ActiveDoc.Objects.AddCurve(c);
            RhinoDoc.ActiveDoc.Views.Redraw();
        }

        private void CrvRBTN_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            interpolated = rb.Checked;
            cmd.RedrawViews();
        }

        private void ArrCB_CheckedChanged(object sender, EventArgs e)
        {
            cmd.RedrawViews();
            ArrSL.Enabled = !ArrSL.Enabled;
        }

        private void ArrSL_Scroll(object sender, EventArgs e)
        {
            CursorAid0.SetToolTip(ArrSL, ArrSL.Value.ToString());
            cmd.RedrawViews();
        }

        #endregion

        /// <summary>
        /// compute trignometry
        /// </summary>
        /// <param name="pos">the combobox position index</param>
        /// <param name="u">angle value in radian</param>
        /// <param name="f">the multiplier factor on u</param>
        /// <returns>evaluated result</returns>
        private double TrigCompute(int pos, double u, double f)
        {
            ComboBox Trig;
            switch (pos)
            {
                case 0:
                    Trig = Trig0;
                    break;
                case 1:
                    Trig = Trig1;
                    break;
                case 2:
                    Trig = Trig2;
                    break;
                case 3:
                    Trig = Trig3;
                    break;
                default:
                    Trig = null;
                    break;
            }
            double trig = double.NaN;
            if (Trig == null) return trig;
            switch (Trig.SelectedIndex)
            {
                case 0:
                    trig = Math.Sin(u*f);
                    break;
                case 1:
                    trig = Math.Cos(f*u);
                    break;
                case 2:
                    trig = Math.Tan(u*f);
                    break;
                default:
                    break;
            }
            return trig;
        }

        internal Curve[] CrvGen()
        {
            Point3d[] pts = new Point3d[UCtSL.Value+1]; // last item repeats first item
            if (U1SL.Value <= U0SL.Value) return null;
            for (int i = 0; i<UCtSL.Value; i++)
            {
                double incr = (U1SL.Value - U0SL.Value) / 100.0 * 2 * Math.PI / UCtSL.Value;
                double u = incr * i + U0SL.Value / 100.0 * 2 * Math.PI;
                double px;
                switch (Op0.SelectedIndex)
                {
                    case 0:
                        px = f0 * TrigCompute(0, u, f1) + f2 * TrigCompute(1, u, f3);
                        break;
                    case 1:
                        px = f0 * TrigCompute(0, u, f1) - f2 * TrigCompute(1, u, f3);
                        break;
                    case 2:
                        px = f0 * TrigCompute(0, u, f1) * f2 * TrigCompute(1, u, f3);
                        break;
                    case 3:
                        px = f0 * TrigCompute(0, u, f1) / f2 * TrigCompute(1, u, f3);
                        break;
                    default:
                        px = 0;
                        break;
                }
                double py;
                switch (Op1.SelectedIndex)
                {
                    case 0:
                        py = f4 * TrigCompute(2, u, f5) + f6 * TrigCompute(3, u, f7);
                        break;
                    case 1:
                        py = f4 * TrigCompute(2, u, f5) - f6 * TrigCompute(3, u, f7);
                        break;
                    case 2:
                        py = f4 * TrigCompute(2, u, f5) * f6 * TrigCompute(3, u, f7);
                        break;
                    case 3:
                        py = f4 * TrigCompute(2, u, f5) / f6 * TrigCompute(3, u, f7);
                        break;
                    default:
                        py = 0;
                        break;
                }
                pts.SetValue(new Point3d(px, py, 0), i);
            }
            pts.SetValue(pts[0], UCtSL.Value); // close the loop
            if (interpolated)
            {
                Curve og = Curve.CreateInterpolatedCurve(pts, 3);
                if (ArrCB.Checked)
                    return ArrPlr(og, ArrSL.Value);
                else
                    return new Curve[] { og };
            }
            else
            {
                Curve og = new PolylineCurve(pts);
                if (ArrCB.Checked)
                    return ArrPlr(og, ArrSL.Value);
                else
                    return new Curve[] { og };
            }
        }

        private Curve[] ArrPlr(Curve og, int ct)
        {
            Curve[] arr = new Curve[ct];
            for (int i = 0; i < ct; i++)
            {
                if (i == 0) arr.SetValue(og, 0);
                else
                {
                    double incr = Math.PI * 2 / ct;
                    Transform rot = Transform.Rotation(i * incr, Vector3d.ZAxis, Point3d.Origin);
                    Curve copy = og.DuplicateCurve();
                    copy.Transform(rot);
                    arr.SetValue(copy, i);
                }
            }
            return arr;
        }

        internal void RefreshValues()
        {
            double.TryParse(CoEf0.Text, out f0);
            double.TryParse(CoEf1.Text, out f1);
            double.TryParse(CoEf2.Text, out f2);
            double.TryParse(CoEf3.Text, out f3);
            double.TryParse(CoEf4.Text, out f4);
            double.TryParse(CoEf5.Text, out f5);
            double.TryParse(CoEf6.Text, out f6);
            double.TryParse(CoEf7.Text, out f7);

            crvclr = ColorPicker0.Color;
            CrvRBTN.Checked = interpolated;
            PLineRBTN.Checked = !interpolated;

            if (preview) PreviewBTN.Text = "Hide Preview";
            else PreviewBTN.Text = "Show Preview";
        }

        
    }
}
