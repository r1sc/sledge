using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes;
using Sledge.Providers.Texture;
using Sledge.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.OpenGL;
using Sledge.Rendering.Scenes.Lights;
using Sledge.Rendering.Scenes.Renderables;
using Face = Sledge.Rendering.Scenes.Renderables.Face;
using Vertex = Sledge.Rendering.Scenes.Renderables.Vertex;

namespace Sledge.Sandbox
{
    public class TestTexture : ITexture
    {
        private TextureItem _item;

        public TestTexture(TextureItem item)
        {
            _item = item;
        }

        public void Dispose()
        {
            
        }

        public TextureFlags Flags { get { return _item.Flags; } }
        public string Name { get { return _item.Name; } }
        public int Width { get { return _item.Width; } }
        public int Height { get { return _item.Height; } }
        public void Bind()
        {
            throw new NotImplementedException();
        }

        public void Unbind()
        {
            throw new NotImplementedException();
        }
    }
    public class MainForm2 : Form
    {
        public MainForm2()
        {
            var wp = new WadProvider();
            var packages = wp.CreatePackages(new[] { @"C:\Working\Wads" }, new string[0], new string[0], new[] { "halflife" }).ToList();
            var textures = packages.SelectMany(x => x.Items.Values).Skip(500).Take(100).ToList();

            ClientSize = new Size(600, 600);
            
            // Create engine
            var renderer = new OpenGLRenderer();
            var engine = new Engine(renderer);

            // Get render control/context
            var camera = new PerspectiveCamera { Position = new Coordinate(-10, -10, -10), LookAt = Coordinate.Zero };
            //var camera = new OrthographicCamera() { Zoom = 32 };
            var viewport = engine.CreateViewport(camera);

            viewport.Control.Dock = DockStyle.Fill;
            Controls.Add(viewport.Control);

            // Create scene
            var scene = renderer.Scene;
            //scene.StartUpdate();

            var light = new AmbientLight(Color.White, new Coordinate(1, 2, 3), 0.8f);
            scene.Add(light);

            scene.Add(new Sledge.Rendering.Scenes.Renderables.Line(Color.FromArgb(255, Color.Red), Coordinate.Zero, Coordinate.UnitX * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            scene.Add(new Sledge.Rendering.Scenes.Renderables.Line(Color.FromArgb(255, Color.Lime), Coordinate.Zero, Coordinate.UnitY * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            scene.Add(new Sledge.Rendering.Scenes.Renderables.Line(Color.FromArgb(255, Color.Blue), Coordinate.Zero, Coordinate.UnitZ * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });

            foreach (var ti in textures)
            {
                renderer.Textures.Create(ti.Name);
            }

            var animat = Material.Animated(7, Enumerable.Range(1, 7).Select(x => "+" + x + "~c2a4_cmp2").ToArray());
            renderer.Materials.Add(animat);

            var s1 = new Sledge.Rendering.Scenes.Renderables.Sprite(Coordinate.Zero, animat, 3, 3);
            scene.Add(s1);

            Task.Factory.StartNew(() =>
            {
                return;
                const int area = 20;
                var r = new Random();
                var b = new BlockBrush();
                for (var i = 0; i < area * 100; i++)
                {
                    Thread.Sleep(2);
                    lock (scene)
                    {
                        var coord = new Coordinate(r.Next(-area, area), r.Next(-area, area), r.Next(-area, area));
                        var brushes = b.Create(new IDGenerator(), new Box(coord, coord + Coordinate.One), new TestTexture(textures[i % textures.Count]), 2).ToList();

                        var r2 = new Random();
                        var material = i % 2 == 0
                            ? Material.Texture(textures[i % textures.Count].Name)
                            : Material.Flat(Color.FromArgb(r2.Next(128, 255), r2.Next(128, 255), r2.Next(128, 255)));
                        if (r2.Next(0, 100) > 50) material.Color = Color.FromArgb(128, material.Color);
                        renderer.Materials.Add(material);
                        foreach (var s in brushes.OfType<Solid>().SelectMany(x => x.Faces))
                        {
                            var randomflags = RenderFlags.None;
                            if (r2.Next(0, 100) > 50) randomflags |= RenderFlags.Polygon;
                            if (r2.Next(0, 100) > 50) randomflags |= RenderFlags.Wireframe;
                            if (r2.Next(0, 100) > 50) randomflags |= RenderFlags.Point;
                            s.FitTextureToPointCloud(new Cloud(s.Vertices.Select(v => v.Location)), 1, 1);
                            var face = new Face(material, s.Vertices.Select(x => new Vertex(x.Location, x.TextureU, x.TextureV)).ToList())
                                       {
                                           AccentColor = Color.FromArgb(r2.Next(128, 255), r2.Next(128, 255), r2.Next(128, 255)),
                                           TintColor = Color.FromArgb(r.Next(0, 128), Color.Red),
                                           //RenderFlags = RenderFlags.Polygon | RenderFlags.Wireframe | RenderFlags.Point
                                           RenderFlags = randomflags,
                                           // ForcedRenderFlags = RenderFlags.Wireframe
                                       };
                            scene.Add(face);
                        }
                    }
                }
            });

            Task.Factory.StartNew(() =>
            {
                for (var i = 0; ; i = (i + 1) % 4)
                {
                    Thread.Sleep(5);
                    var random = new Random();
                    lock (scene)
                    {
                        var objects = scene.Objects.ToList();
                        var index = random.Next(0, objects.Count);
                        var face = objects[index] as Face;
                        if (face != null)
                        {
                            if (false && i < 2)
                            {
                                var material = i % 2 == 0
                                    ? Material.Texture(textures[i % textures.Count].Name)
                                    : Material.Flat(Color.FromArgb(random.Next(128, 255), random.Next(128, 255), random.Next(128, 255)));
                                face.Material = material;
                            }
                            else
                            {
                                scene.Remove(face);
                                // face.Vertices = face.Vertices.Select(x => new Vertex(x.Position + Coordinate.One, x.TextureU, x.TextureV)).ToList();
                            }
                        }
                    }
                }
            });

            //scene.EndUpdate();

            // Add scene to renderer / add renderer to scene

            var c = this;

            Task.Factory.StartNew(() =>
            {
                using (var ss = wp.GetStreamSource(512, 512, packages))
                {
                    foreach (var ti in textures)
                    {
                        Thread.Sleep(100);
                        var ti1 = ti;
                        try
                        {
                            c.Invoke((Action) (() =>
                                               {
                                                   var bmp = ss.GetImage(ti1);
                                                   renderer.Textures.Create(ti1.Name, bmp, ti1.Width, ti1.Height, ti1.Flags);
                                                   bmp.Dispose();
                                               }));
                        }
                        catch
                        {
                        }
                    }
                }
            });
        }
    }
}