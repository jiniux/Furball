using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Furball.Engine.Engine.Input {
    public abstract class InputMethod {
        /// <summary>
        /// The position and states of all cursors
        /// </summary>
        public List<MouseState> CursorPositions = new();
        /// <summary>
        /// The keyboard keys that are currently held
        /// </summary>
        public List<Keys> HeldKeys = new();

        /// <summary>
        /// Used if the InputMethod needs to constantly poll a source
        /// </summary>
        public abstract void Update();
        /// <summary>
        /// Used if the InputMethod needs to destruct itself safely
        /// </summary>
        public abstract void Dispose();
        /// <summary>
        /// Used if the InputMethod needs to Initialize itself before being updated
        /// </summary>
        public abstract void Initialize();
    }
}
