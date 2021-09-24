using System.Collections.Generic;
using Furball.Engine.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics.Drawables.Managers {
    public class EffectDrawableManager : UnmanagedDrawable {
        private DrawableManager      _drawableManager = new();
        private List<DrawableEffect> _effects         = new();
        private RenderTarget2D       _target2D;

        private GraphicsDevice _graphicsDevice;

        public EffectDrawableManager(GraphicsDevice graphicsDevice) {
            this._graphicsDevice = graphicsDevice;
        }

        public void AddEffect(DrawableEffect effect) => this._effects.Add(effect);

        public override void Draw(GameTime time, SpriteBatch batch, DrawableManagerArgs args = null) {
            Texture2D currentPass = this._drawableManager.DrawRenderTarget2D(time, batch, args);

            if(this._target2D?.Width != FurballGame.WindowWidth || this._target2D?.Height != FurballGame.WindowHeight)
                this._target2D = new RenderTarget2D(this._graphicsDevice, FurballGame.WindowWidth, FurballGame.WindowHeight);

            this._graphicsDevice.SetRenderTarget(this._target2D);
            this._graphicsDevice.Clear(Color.Transparent);

            for (int i = 0; i != this._effects.Count; i++) {
                DrawableEffect currentEffect = this._effects[i];

                currentPass = currentEffect.Draw(currentPass);

                batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                batch.Draw(currentPass, Vector2.Zero, Color.White);
                batch.End();
            }

            this._graphicsDevice.SetRenderTarget(null);

            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            batch.Draw(currentPass, Vector2.Zero, Color.White);
            batch.End();
        }

        public void Add(BaseDrawable drawable) => this._drawableManager.Add(drawable);
    }
}
