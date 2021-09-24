using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.PostProcessEffects;
using Furball.Engine.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Drawables {
    public class BloomedTexturedDrawable : DrawableEffect {
        private BloomRenderer _bloomRenderer;
        /// <summary>
        /// The Texture Being drawn
        /// </summary>
        private Texture2D _texture;
        /// <summary>
        /// Bloom Texture
        /// </summary>
        private Texture2D _bloomTexture;
        /// <summary>
        /// Crop Rectangle, this basically tells which part of the Texture to Render
        /// Leave null to draw the entire Texture
        /// </summary>
        private Rectangle? _cropped = null;

        private RenderTarget2D _target2D;

        public BloomedTexturedDrawable(int width, int height) {
            this._bloomRenderer = new BloomRenderer();
            this._bloomRenderer.Load(FurballGame.Instance.GraphicsDevice, FurballGame.Instance.Content, width, height);

            this._bloomRenderer.BloomPreset = BloomRenderer.BloomPresets.Focussed;

        }
        public override Texture2D Draw(Texture2D inputTexture, SpriteBatch batch) {
            if(this._target2D?.Width != FurballGame.WindowWidth || this._target2D?.Height != FurballGame.WindowHeight)
                this._target2D = new RenderTarget2D(FurballGame.Instance.GraphicsDevice, FurballGame.WindowWidth, FurballGame.WindowHeight, false, SurfaceFormat.Alpha8, DepthFormat.Depth24);

            FurballGame.Instance.GraphicsDevice.SetRenderTarget(this._target2D);

            this._bloomTexture = this._bloomRenderer.Draw(inputTexture);
            FurballGame.Instance.GraphicsDevice.SetRenderTarget(_target2D);

            batch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            batch.Draw(inputTexture,      Vector2.Zero, Color.White);
            batch.Draw(this._bloomTexture, Vector2.Zero, Color.White);

            batch.End();

            return this._target2D;
        }
        //TODO(Eevee): offload to task and stuff
        public void RefreshTexture() {
            this._bloomTexture = this._bloomRenderer.Draw(this._texture);
            FurballGame.Instance.GraphicsDevice.SetRenderTarget(null);
        }
        /// <summary>
        /// Changes the Cropping of the Texture
        /// </summary>
        /// <param name="crop">New Cropping</param>
        public void ChangeCropping(Rectangle? crop) => this._cropped = crop;
    }
}