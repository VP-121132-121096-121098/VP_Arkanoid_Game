using ArkanoidGame.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArkanoidGameWindow
{
    public partial class SettingsForm : Form
    {
        public bool Fullscreen { get; private set; }

        public GraphicDetails GraphicDetails { get; private set; }

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rbLowDetails.Checked)
            {
                GraphicDetails = GraphicDetails.Low;
            }
            else if (rbHighDetails.Checked)
            {
                GraphicDetails = GraphicDetails.High;
            }
            else if (rbVeryHighDetails.Checked)
            {
                GraphicDetails = GraphicDetails.VeryHigh;
            }

            Fullscreen = cbFullscreen.Checked;
        }
    }
}
