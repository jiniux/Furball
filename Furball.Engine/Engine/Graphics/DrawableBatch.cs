using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics {
    /// <summary>
    /// A Basic Abstraction for Sprite and Shape batch
    /// </summary>
    public class DrawableBatch {
        public SpriteBatch           SpriteBatch;
        public GraphicsDeviceContext DeviceContext;

        private bool _begun;
        public bool Begun => _begun;
        
        public DrawableBatch(SpriteBatch spriteBatch, GraphicsDeviceContext context) {
            this.SpriteBatch   = spriteBatch;
            this.DeviceContext = context;
        }

        public void Begin() {
            this.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            this._begun = true;
        }

        public void End() {
            this.SpriteBatch.End();
            this._begun = false;
        }
    }
}
