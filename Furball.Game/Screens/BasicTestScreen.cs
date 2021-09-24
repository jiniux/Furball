using System;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Drawables;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Engine.Engine.Graphics.PostProcessEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MathHelper=Furball.Engine.Engine.Helpers.MathHelper;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        public override void Initialize() {
            this.Manager = new EffectDrawableManager(FurballGame.Instance.GraphicsDevice);
            BloomedTexturedDrawable bloomRenderer = new BloomedTexturedDrawable(FurballGame.WindowWidth, FurballGame.WindowHeight);
            this.Manager.AddEffect(bloomRenderer);

            TexturedDrawable whiteTexture = new(ContentReader.LoadMonogameAsset<Texture2D>("white"), new Vector2(240, 240));

            whiteTexture.Tweens.Add(new VectorTween(TweenType.Movement, new Vector2(240, 240), new Vector2(540,   540),   1000, 10000, Easing.In));
            whiteTexture.Tweens.Add(new VectorTween(TweenType.Scale,    new Vector2(1,   1),   new Vector2(0.25f, 0.25f), 1000, 10000, Easing.In));
            whiteTexture.Tweens.Add(new FloatTween(TweenType.Rotation, 0f, (float)MathHelper.DegreesToRadians(1440), 1000, 10000));
            whiteTexture.Tweens.Add(new FloatTween(TweenType.Fade,     1f, 0.5f,                                     1000, 10000));
            whiteTexture.Tweens.Add(new ColorTween(TweenType.Color, Color.White, Color.Red, 1000, 10000));

            whiteTexture.OnClick += delegate {
                Console.WriteLine("clicked");
            };
            whiteTexture.OnClickUp += delegate {
                Console.WriteLine("unclicked");
            };

            this.Manager.Add(whiteTexture);

            UiButtonDrawable testButton = new("test button", ContentReader.LoadRawAsset("default-font.ttf"), 50, Color.Blue, Color.White, Color.Black) {
                Position = new Vector2(200, 200),
                Clickable = true
            };
            //testButton.OnHover += delegate {
            //    testButton.Tweens.Add(
            //        new VectorTween(
            //            TweenType.Movement,
            //            testButton.Position,
            //            new(testButton.Position.X + FurballGame.Random.NextSingle(-50, 50), testButton.Position.Y + FurballGame.Random.NextSingle(-50, 50)),
            //            FurballGame.Time,
            //            FurballGame.Time + 1000
            //        )
            //    );
            //};
            //testButton.OnHoverLost += delegate {
            //    testButton.Tweens.Add(
            //        new VectorTween(
            //            TweenType.Movement,
            //            testButton.Position,
            //            new(testButton.Position.X + FurballGame.Random.NextSingle(-50, 50), testButton.Position.Y + FurballGame.Random.NextSingle(-50, 50)),
            //            FurballGame.Time,
            //            FurballGame.Time + 1000
            //        )
            //    );
            //};

            testButton.OnDrag += delegate {
                testButton.Position = FurballGame.InputManager.CursorStates[0].State.Position.ToVector2() - new Vector2(testButton.Size.X / 2, testButton.Size.Y / 2);
            };

            testButton.OnDragEnd += delegate {
                Console.WriteLine("sdfdsf");
            };

            this.Manager.Add(testButton);

            //AudioStream stream = AudioEngine.LoadFile("testaudio.mp3");
            //stream.Play();
            //
            //whiteTexture.TimeSource = stream;

            base.Initialize();
        }
    }
}
