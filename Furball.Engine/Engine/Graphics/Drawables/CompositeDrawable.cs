using System.Collections.Generic;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables {
    public class CompositeDrawable : ManagedDrawable {
        public List<ManagedDrawable> Drawables = new();

        public override Vector2 Size {
            get {
                Vector2 topLeft     = new(0, 0);
                Vector2 bottomRight = new(0, 0);

                foreach (ManagedDrawable managedDrawable in this.Drawables) {
                    if (managedDrawable.Rectangle.X < topLeft.X) topLeft.X = managedDrawable.Rectangle.X;
                    if (managedDrawable.Rectangle.Y < topLeft.Y) topLeft.Y = managedDrawable.Rectangle.Y;

                    if (managedDrawable.Rectangle.Right  > bottomRight.X) bottomRight.X = managedDrawable.Rectangle.Right;
                    if (managedDrawable.Rectangle.Bottom > bottomRight.Y) bottomRight.Y = managedDrawable.Rectangle.Bottom;
                }

                return bottomRight - topLeft;
            }
        }

        public CompositeDrawable() => this.OnClick += this.OnDrawableClick;

        private void OnDrawableClick(object sender, Point e) {
            Point adjustedPoint = e - this.Position.ToPoint() + this.LastCalculatedOrigin.ToPoint();

            for (int i = 0; i < this.Drawables.Count; i++) {
                ManagedDrawable drawable = this.Drawables[i];

                if (drawable.Contains(adjustedPoint)) {
                    drawable.Click(true,  adjustedPoint);
                    drawable.Click(false, adjustedPoint);
                }
            }
        }

        public override void Update(GameTime time) {
            foreach (ManagedDrawable drawable in this.Drawables) {
                drawable.Update(time);
                drawable.UpdateTweens();
            }
        }

        public override void Dispose(bool disposing) {
            this.OnClick -= this.OnDrawableClick;

            base.Dispose(disposing);
        }

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            foreach (ManagedDrawable drawable in this.Drawables) {
                if (!drawable.Visible) continue;
                
                DrawableManagerArgs drawableArgs = new() {
                    Color    = drawable.ColorOverride,
                    Effects  = args.Effects,
                    Position = args.Position + (drawable.Position * args.Scale),
                    Rotation = args.Rotation,
                    Scale    = args.Scale * drawable.Scale
                };
                
                drawable.Draw(time, batch, drawableArgs);
            }
        }
    }
}
