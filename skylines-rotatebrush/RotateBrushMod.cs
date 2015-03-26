using System;
using ICities;
namespace skylinesrotatebrush
{
    public class RotateBrushMod : IUserMod
    {
        public string Name {
            get {
                return "RotateBrush";
            }
        }

        public string Description {
            get {
                return "Enables rotation of brushes in the map editor";
            }
        }
    }
}

