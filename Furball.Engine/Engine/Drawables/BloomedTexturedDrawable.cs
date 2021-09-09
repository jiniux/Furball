using Furball.Engine.Engine.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Drawables {
    public class BloomedTexturedDrawable : UnmanagedDrawable {
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
        /// <summary>
        /// TexturedDrawable Constructor
        /// </summary>
        /// <param name="texture">Texture to Draw</param>
        /// <param name="position">Where to Draw</param>
        public BloomedTexturedDrawable(Texture2D texture, Vector2 position) {
            this.Position      = position;
            this.Rotation      = 0f;
            this.ColorOverride = Color.White;
            this.Scale         = new Vector2(1,             1);
            this.Size          = new Vector2(texture.Width, texture.Height);

            this._texture = texture;

            this._bloomRenderer = new BloomRenderer();
            this._bloomRenderer.Load(FurballGame.Instance.GraphicsDevice, FurballGame.Instance.Content, texture.Width, texture.Height);

            this._bloomRenderer.BloomPreset = BloomRenderer.BloomPresets.Wide;

        }
        public override void Draw(GameTime time, SpriteBatch batch) {
            this._bloomTexture = this._bloomRenderer.Draw(this._texture, this._texture.Width, this._texture.Height);
            FurballGame.Instance.GraphicsDevice.SetRenderTarget(null);


            batch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            batch.Draw(this._texture, this.Position, null, this.ColorOverride, this.Rotation, Vector2.Zero, this.Scale, this.SpriteEffect, 1f);
            batch.Draw(this._bloomTexture, this.Position, null, this.ColorOverride, this.Rotation, Vector2.Zero, this.Scale, this.SpriteEffect, 1f);

            batch.End();
        }
        //TODO(Eevee): offload to task and stuff
        public void RefreshTexture() {
            this._bloomTexture = this._bloomRenderer.Draw(this._texture, this._texture.Width, this._texture.Height);
            FurballGame.Instance.GraphicsDevice.SetRenderTarget(null);
        }
        /// <summary>
        /// Changes the Cropping of the Texture
        /// </summary>
        /// <param name="crop">New Cropping</param>
        public void ChangeCropping(Rectangle? crop) => this._cropped = crop;
    }
}
