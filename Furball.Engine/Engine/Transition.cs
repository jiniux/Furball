using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine {
    public abstract class Transition : UnmanagedDrawable {
        protected DrawableManager Manager = new();
        /// <summary>
        /// This gets called right before the Screen gets Changed,
        /// Imagine a Triangle Transition Effect, this could make Triangles start appearing
        /// <returns>How long the FadeIn Takes in milliseconds</returns>
        /// </summary>
        public abstract int TransitionBegin();
        /// <summary>
        /// This gets called right after the Screen got Changed,
        /// Imagine a Triangle Transition Effect, this could make the Triangles stop appearing
        /// This is made optional as some Transitions can be made with only the Begin call,
        /// For example the Fade Transition would be pretty inconvinient to make in 2 function calls
        /// <returns>How long the FadeOut Takes in milliseconds</returns>
        /// </summary>
        public virtual int TransitionEnd() { return 0; }
        /// <summary>
        /// Draws the DrawableManager of the Transition
        /// </summary>
        /// <param name="time"></param>
        /// <param name="drawableBatch"></param>
        /// <param name="args"></param>
        public override void Draw(GameTime time, DrawableBatch drawableBatch, DrawableManagerArgs args = null) {
            this.Manager.Draw(time, drawableBatch, args);
        }
        /// <summary>
        /// Updates the DrawableManager of the Transition
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time) {
            this.Manager.Update(time);
        }
    }
}
