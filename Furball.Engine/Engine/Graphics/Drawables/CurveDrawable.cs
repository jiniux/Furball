using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Microsoft.Xna.Framework;
using Xssp.MonoGame.Primitives2D;

namespace Furball.Engine.Engine.Graphics.Drawables {
    public class CurveDrawable : ManagedDrawable {
        public CurveType Type;

        public Bindable<Vector2> P0;
        public Bindable<Vector2> P1;
        public Bindable<Vector2> P2;
        public Bindable<Vector2> P3;

        public int Quality = 20;

        public float Thickness = 2f;

        public override Vector2 Size => new(100);

        public CurveDrawable(Vector2 p0, Vector2 p1, Vector2 p2) {
            this.P0 = new Bindable<Vector2>(p0);
            this.P1 = new Bindable<Vector2>(p1);
            this.P2 = new Bindable<Vector2>(p2);

            this.Type = CurveType.Quadratic;
        }

        public CurveDrawable(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
            this.P0 = new Bindable<Vector2>(p0);
            this.P1 = new Bindable<Vector2>(p1);
            this.P2 = new Bindable<Vector2>(p2);
            this.P3 = new Bindable<Vector2>(p3);

            this.Type = CurveType.Cubic;
        }

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            for (int i = 0; i < this.Quality; i++) {
                float t     = (float)i       / this.Quality;
                float nextT = (float)(i + 1) / this.Quality;

                switch (this.Type) {
                    case CurveType.Cubic: {
                        (float x, float y)   = BezierHelper.CubicBezier(this.P0, this.P1, this.P2, this.P3, t);
                        (float x1, float y1) = BezierHelper.CubicBezier(this.P0, this.P1, this.P2, this.P3, nextT);

                        x  *= FurballGame.VerticalRatio;
                        y  *= FurballGame.VerticalRatio;
                        x1 *= FurballGame.VerticalRatio;
                        y1 *= FurballGame.VerticalRatio;

                        batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                        batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);

                        break;
                    }
                    case CurveType.Quadratic: {
                        (float x, float y)   = BezierHelper.QuadraticBezier(this.P0, this.P1, this.P2, t);
                        (float x1, float y1) = BezierHelper.QuadraticBezier(this.P0, this.P1, this.P2, nextT);

                        x  *= FurballGame.VerticalRatio;
                        y  *= FurballGame.VerticalRatio;
                        x1 *= FurballGame.VerticalRatio;
                        y1 *= FurballGame.VerticalRatio;

                        batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                        batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                        
                        break;
                    }
                    case CurveType.CatmullRom: {
                        (float x, float y)   = Vector2.CatmullRom(this.P0, this.P1, this.P2, this.P3, t);
                        (float x1, float y1) = Vector2.CatmullRom(this.P0, this.P1, this.P2, this.P3, nextT);

                        x  *= FurballGame.VerticalRatio;
                        y  *= FurballGame.VerticalRatio;
                        x1 *= FurballGame.VerticalRatio;
                        y1 *= FurballGame.VerticalRatio;

                        batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                        batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);

                        break;
                    }
                }
            }
        }
    }

    public enum CurveType {
        Quadratic,
        Cubic,
        CatmullRom
    }
}
