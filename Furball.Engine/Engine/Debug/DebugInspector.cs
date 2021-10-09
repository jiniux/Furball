using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Debug {
    public class DebugInspector : UnmanagedDrawable {
        private DrawableManager _manager = new();

        public DebugInspector() {
            //TexturedDrawable background = new TexturedDrawable(FurballGame.WhitePixel, )
        }

        public override void Draw(GameTime time, DrawableBatch drawableBatch, DrawableManagerArgs args = null) {
            this._manager.Draw(time, drawableBatch, args);
        }
    }
}
