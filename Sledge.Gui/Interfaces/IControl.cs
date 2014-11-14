using System;
using Sledge.Gui.Bindings;
using Sledge.Gui.Events;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Interfaces
{
    public interface IControl : IBindingTarget, IDisposable
    {
        IContainer Parent { get; }
        IControl Implementation { get; }
        bool Enabled { get; set; }
        bool Focused { get; }
        Size ActualSize { get; }
        Size PreferredSize { get; set; }
        object Tag { get; set; }
        event EventHandler ActualSizeChanged;
        event EventHandler PreferredSizeChanged;

        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseUp;
        event MouseEventHandler MouseWheel;
        event MouseEventHandler MouseMove;
        event MouseEventHandler MouseClick;
        event EventHandler MouseDoubleClick;
        event EventHandler MouseEnter;
        event EventHandler MouseLeave;
    }
}