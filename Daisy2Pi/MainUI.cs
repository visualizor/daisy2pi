using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        internal enum Trigs { sin, cos, tan, };
        internal enum Ops { plus, minus, multi, div, };

        public MainUI()
        {
            InitializeComponent();
            Trig0.SelectedIndex = 0;
            Trig1.SelectedIndex = 1;
            Trig2.SelectedIndex = 1;
            Trig3.SelectedIndex = 0;
            Op0.SelectedIndex = 0;
            Op1.SelectedIndex = 1;
            U0LBL.Text = string.Format("{0:0.00}", U0SL.Value / 100.0 * 2 * Math.PI);
            U1LBL.Text = string.Format("{0:0.00}", U1SL.Value / 100.0 * 2 * Math.PI);

            FormClosing += OnClosing;
            CoEf0.KeyUp += OnCoEf0Enter;

            //empty context strip
            var menustrip = new ContextMenu();
            Trig0.KeyPress += BlockKeyPress;
            Trig0.ContextMenu = menustrip;
            Trig1.KeyPress += BlockKeyPress;
            Trig1.ContextMenu = menustrip;
            Trig2.KeyPress += BlockKeyPress;
            Trig2.ContextMenu = menustrip;
            Trig3.KeyPress += BlockKeyPress;
            Trig3.ContextMenu = menustrip;
        }

        private void CoEf0_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            string txt = tb.Text;
            if (!double.TryParse(txt, out f0)) return;
        }
        private void OnCoEf0Enter(object s, KeyEventArgs e)
        {
            TextBox tb = s as TextBox;
            string txt = tb.Text;
            if (e.KeyCode == Keys.Enter)
                if (!double.TryParse(txt, out f0))
                    tb.Text = txt;
        }

        private void U0SL_Scroll(object sender, EventArgs e)
        {
            U0LBL.Text = string.Format("{0:0.00}", U0SL.Value / 100.0 * 2 * Math.PI);
            if (U0SL.Value == U0SL.Maximum) U0LBL.Text = @"2π";
        }

        private void U1SL_Scroll(object sender, EventArgs e)
        {
            U1LBL.Text = string.Format("{0:0.00}", U1SL.Value / 100.0 * 2 * Math.PI);
            if (U1SL.Value == U1SL.Maximum) U1LBL.Text = @"2π";
        }

        private void BlockKeyPress(object s, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void OnClosing(object s, FormClosingEventArgs e)
        {
            Hide(); // only allow this in the rhino context
            e.Cancel = true;
        }
    }
}
