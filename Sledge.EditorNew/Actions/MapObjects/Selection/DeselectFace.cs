using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;

namespace Sledge.EditorNew.Actions.MapObjects.Selection
{
    public class DeselectFace : ChangeFaceSelection
    {
        public DeselectFace(IEnumerable<Face> objects) : base(new Face[0], objects)
        {
        }

        public DeselectFace(params Face[] objects) : base(new Face[0], objects)
        {
        }
    }
}