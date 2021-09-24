using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.PostProcessEffects;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine {
    public class Screen : DrawableGameComponent {
        protected EffectDrawableManager Manager;
        public Screen() : base(FurballGame.Instance) {}

        public override void Draw(GameTime gameTime) {
            this.Manager.Draw(gameTime, FurballGame.SpriteBatch);

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime) {
            this.Manager.Update(gameTime);

            base.Update(gameTime);
        }
    }
}
