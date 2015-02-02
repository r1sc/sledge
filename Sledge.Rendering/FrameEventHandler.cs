﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Rendering
{
    public delegate void FrameEventHandler(IViewport viewport, Frame frame);
    public delegate void RenderExceptionEventHandler(object sender, Exception ex);
}
