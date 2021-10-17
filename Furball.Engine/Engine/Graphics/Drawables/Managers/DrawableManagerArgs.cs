using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics.Drawables.Managers {
    public class DrawableManagerArgs {
        public Vector2       Position;
        public Vector2       ScaledPosition;
        public Color         Color;
        public float         Rotation;
        public Vector2       Scale;
        public Vector2       ScaledScale;
        public SpriteEffects Effects;
        public float         LayerDepth;
        public ViewRectangle ViewRectangle;
    }
}
