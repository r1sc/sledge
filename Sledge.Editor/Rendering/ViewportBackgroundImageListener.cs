using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using Sledge.Common;
using Sledge.Common.Mediator;
using Sledge.Editor.Documents;
using Sledge.Editor.UI;
using Sledge.Graphics;
using Sledge.Graphics.Helpers;
using Sledge.Settings;
using Sledge.UI;
using View = Sledge.Settings.View;

namespace Sledge.Editor.Rendering
{
    // ReSharper disable CSharpWarnings::CS0612
    // OpenTK's TextPrinter is marked as obsolete but no suitable replacement exists
    public class ViewportBackgroundImageListener : IViewportEventListener, IDisposable
    {
        public ViewportBase Viewport { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }

        private GLTexture _texture;
        private bool _showing;
        private ContextMenu _menu;
        private RectangleF _rect;
        private TextPrinter _printer;

        public ViewportBackgroundImageListener(ViewportBase viewport)
        {
            Viewport = viewport;
            _menu = new ContextMenu(new[]
            {
                new MenuItem("Select background image...", (sender, args) =>
                {
                    using (var ofd = new OpenFileDialog())
                    {
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            //viewport.BackgroundImage = Image.FromFile(ofd.FileName);
                            var bmp = (Bitmap) Image.FromFile(ofd.FileName);
                            _texture = TextureHelper.Create("__backgroundimage", bmp, bmp.Width, bmp.Height,
                                TextureFlags.None);
                        }
                    }
                }),
                new MenuItem("Settings...", (sender, args) =>
                {
                    var frm = new BackgroundImageSettings(this);
                    frm.Show();
                }), 
            });
            _printer = new TextPrinter(TextQuality.Low);
        }

        public void Render2D()
        {
            if (_rect.IsEmpty)
            {
                var te = _printer.Measure("B", SystemFonts.MessageBoxFont, new RectangleF(0, 0, Viewport.Width, Viewport.Height));
                _rect = te.BoundingBox;
                _rect.X += 5;
                _rect.Y += 20;
                _rect.Width += 5;
                _rect.Height += 2;
            }

            GL.Disable(EnableCap.CullFace);

            _printer.Begin();
            if (_showing)
            {
                GL.Begin(BeginMode.Quads);
                GL.Color3(Viewport is Viewport3D ? View.ViewportBackground : Grid.Background);
                GL.Vertex2(0, 0);
                GL.Vertex2(_rect.Right, 0);
                GL.Vertex2(_rect.Right, _rect.Bottom);
                GL.Vertex2(0, _rect.Bottom);
                GL.End();
            }
            _printer.Print("B", SystemFonts.MessageBoxFont, _showing ? Color.White : Grid.GridLines, _rect);
            _printer.End();

            if (_texture != null)
            {
                GraphicsHelper.EnableDepthTesting();
                TextureHelper.EnableTexturing();
                _texture.Bind();
                var tw2 = _texture.Width/2;
                var th2 = _texture.Height/2;

                GL.PushMatrix();
                GL.Scale(ScaleX, ScaleY, 1);
                GL.Color4(1.0, 1.0, 1.0, 0.2);
                GL.Begin(BeginMode.Quads);
                GL.TexCoord2(0, 1);
                GL.Vertex3(-tw2, -th2, -10);
                GL.TexCoord2(1, 1);
                GL.Vertex3(tw2, -th2, -10);
                GL.TexCoord2(1, 0);
                GL.Vertex3(tw2, th2, -10);
                GL.TexCoord2(0, 0);
                GL.Vertex3(-tw2, th2, -10);
                GL.End();
                GL.PopMatrix();

                TextureHelper.Unbind();
                TextureHelper.DisableTexturing();
                GraphicsHelper.DisableDepthTesting();
            }

            GL.Enable(EnableCap.CullFace);
        }

        public void PostRender()
        {
            // 
        }

        public void Dispose()
        {

        }

        public void KeyUp(ViewportEvent e)
        {

        }

        public void KeyDown(ViewportEvent e)
        {

        }

        public void KeyPress(ViewportEvent e)
        {

        }

        public void MouseMove(ViewportEvent e)
        {
            if (_rect.IsEmpty) return;
            _showing = _rect.Contains(e.X, e.Y);
        }

        public void MouseWheel(ViewportEvent e)
        {

        }

        public void MouseUp(ViewportEvent e)
        {

        }

        public void MouseDown(ViewportEvent e)
        {
            if (_showing)
            {
                _menu.Show(Viewport, new Point(e.X, e.Y));
                e.Handled = true;
            }
        }

        public void MouseClick(ViewportEvent e)
        {

        }

        public void MouseDoubleClick(ViewportEvent e)
        {

        }

        public void MouseEnter(ViewportEvent e)
        {

        }

        public void MouseLeave(ViewportEvent e)
        {
            _showing = false;
        }

        public void UpdateFrame(FrameInfo frame)
        {

        }

        public void PreRender()
        {

        }

        public void Render3D()
        {

        }
    }
}
