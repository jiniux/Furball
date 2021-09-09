using System;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Drawables;
using Furball.Engine.Engine.Drawables.Tweens;
using Furball.Engine.Engine.Drawables.Tweens.TweenTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        public override void Initialize() {
            BloomedTexturedDrawable whiteTexture = new BloomedTexturedDrawable(FurballGame.Instance.Content.Load<Texture2D>("white"), new Vector2(240, 240));
           // whiteTexture.RefreshTexture();

            whiteTexture.Tweens.Add(new FloatTween(TweenType.Rotation, 0f, 1000000f,0, Int32.MaxValue, Easing.None));
            this.Manager.Add(whiteTexture);

            base.Initialize();
        }
    }
}
