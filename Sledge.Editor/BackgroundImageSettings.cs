using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Editor.Rendering;

namespace Sledge.Editor
{
    public partial class BackgroundImageSettings : Form
    {
        private readonly ViewportBackgroundImageListener _backgroundImageListener;

        public BackgroundImageSettings()
        {
            InitializeComponent();
        }

        public BackgroundImageSettings(ViewportBackgroundImageListener backgroundImageListener) : this()
        {
            _backgroundImageListener = backgroundImageListener;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            _backgroundImageListener.ScaleX = trackBar1.Value / 10.0f;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            _backgroundImageListener.ScaleY = trackBar1.Value / 10.0f;
        }
    }
}
