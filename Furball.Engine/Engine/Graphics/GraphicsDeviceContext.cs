using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics {
    public record Camera(Matrix WorldPosition, Matrix CameraState, Matrix Projection);

    public class GraphicsDeviceContext {
        private GraphicsDevice        _graphicsDevice;
        private RenderTargetBinding[] _oldTargets;
        private RenderTarget2D        _currentTarget;

        public GraphicsDeviceContext(GraphicsDevice device) {
            this._graphicsDevice = device;
        }

        public RenderTarget2D CreateRenderTarget(int width = -1, int height = -1) {
            return new RenderTarget2D(this._graphicsDevice, width == -1 ? 1280 : width, width == -1 ? 720 : height, false, SurfaceFormat.Color, DepthFormat.Depth24);
        }

        public void SetTarget(RenderTarget2D target) {
            this._oldTargets = this._graphicsDevice.GetRenderTargets();
            this._graphicsDevice.SetRenderTarget(target);

            this._currentTarget = target;
        }

        public RenderTarget2D PopTarget() {
            this._graphicsDevice.SetRenderTargets(this._oldTargets);

            return this._currentTarget;
        }

        public Camera GetBasicScreenCamera() {
            return new Camera(
                Matrix.CreateTranslation(0f, 0, 0),
                Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0)),
                Matrix.CreatePerspective(1f, 1f, 1f, 100f)
            );
        }

        public GraphicsDevice GetGraphicsDevice() => this._graphicsDevice;
    }
}
