﻿using System;
using System.Diagnostics;
using FontStashSharp;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Audio;
using Furball.Engine.Engine.Debug;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Input;
using Furball.Engine.Engine.Input.InputMethods;
using Furball.Engine.Engine.Platform;
using Furball.Engine.Engine.Timing;
using Furball.Engine.Engine.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Furball.Engine {
    public class FurballGame : Game {
        private GraphicsDeviceManager _graphics;
        private IGameComponent        _running;

        public static Random Random = new();

        public static FurballGame   Instance;
        public static DrawableBatch DrawableBatch;
        public static InputManager  InputManager;
        public static ITimeSource   GameTimeSource;

        public static DebugCounter DebugCounter;

        public static DrawableManager DrawableManager;
        public static DrawableManager DebugOverlayDrawableManager;
        public static bool            DrawDebugOverlay = true;

        public const int DEFAULT_WINDOW_WIDTH  = 1280;
        public const int DEFAULT_WINDOW_HEIGHT = 720;

        public static int WindowHeight => Instance.GraphicsDevice.Viewport.Height;
        public static int WindowWidth => Instance.GraphicsDevice.Viewport.Width;

        public static float HorizontalRatio => (float)WindowWidth / DEFAULT_WINDOW_WIDTH;
        public static float VerticalRatio => (float)WindowHeight / DEFAULT_WINDOW_HEIGHT;
        public static Rectangle DisplayRect => new(0, 0, (int)Math.Ceiling(Instance.GraphicsDevice.Viewport.Width / VerticalRatio), (int)Math.Ceiling(Instance.GraphicsDevice.Viewport.Height / VerticalRatio));
        public static Rectangle DisplayRectActual => new(0, 0, Instance.GraphicsDevice.Viewport.Width, Instance.GraphicsDevice.Viewport.Height);

        public static byte[] DEFAULT_FONT_DATA;
        public static readonly FontSystem DEFAULT_FONT = new(new FontSystemSettings {
            FontResolutionFactor = 2f,
            KernelWidth = 2,
            KernelHeight = 2,
            Effect = FontSystemEffect.None
        });

        public static Texture2D WhitePixel;

        public event EventHandler<Screen> BeforeScreenChange; 
        public event EventHandler<Screen> AfterScreenChange; 

        public FurballGame(Screen startScreen) {
            this._graphics             = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible        = true;

            GameTimeSource = new GameTimeSource();
            Instance       = this;
            
            Logger.AddLogger(new ConsoleLogger());

            this.ChangeScreen(startScreen);
        }

        protected override void Initialize() {
            Window.file
            Logger.Log(
                $@"Starting Furball {(Environment.Is64BitProcess ? "64-bit" : "32-bit")} on {Environment.OSVersion.VersionString} {(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")}",
                new LoggerLevelEngineInfo()
            );

            DEFAULT_FONT_DATA = ContentManager.LoadRawAsset("default-font.ttf");
            DEFAULT_FONT.AddFont(DEFAULT_FONT_DATA);

            _stopwatch.Start();

            InputManager = new();
            InputManager.RegisterInputMethod(new MonogameMouseInputMethod());
            InputManager.RegisterInputMethod(new MonogameKeyboardInputMethod());

            if (RuntimeInfo.IsDebug()) {
                InputManager.OnKeyDown += delegate(object _, Keys keys) {
                    if (keys == Keys.F11) DrawDebugOverlay = !DrawDebugOverlay;
                };
            }

            AudioEngine.Initialize(this.Window.Handle);

            DrawableManager             = new();
            DebugOverlayDrawableManager = new();

            WhitePixel = new Texture2D(this.GraphicsDevice, 1, 1);
            Color[] white = { Color.White };
            WhitePixel.SetData(white);
            
            base.Initialize();
        }

        public void ChangeScreen(Screen screen) {
            this.BeforeScreenChange?.Invoke(this, screen);
            
            if (this._running != null) {
                this.Components.Remove(this._running);

                ((GameComponent)this._running).Dispose();
                this._running = null;
            }

            this.Components.Add(screen);
            this._running = screen;
            
            this.AfterScreenChange?.Invoke(this, screen);
        }

        protected override void LoadContent() {
            DrawableBatch = new DrawableBatch(new SpriteBatch(this.GraphicsDevice));

            this.ChangeScreenSize(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);

            DebugCounter = new();

            DebugOverlayDrawableManager.Add(DebugCounter);

            ScreenManager.SetTransition(new FadeTransition());
        }

        public void ChangeScreenSize(int width, int height, bool fullscreen = false) {
            this._graphics.PreferredBackBufferWidth  = width;
            this._graphics.PreferredBackBufferHeight = height;

            this._graphics.IsFullScreen = fullscreen;

            this._graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep                          = false;
            this._graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime) {
            InputManager.Update();

            DrawableManager.Update(gameTime);

            if (RuntimeInfo.IsDebug())
                DebugOverlayDrawableManager.Update(gameTime);

            if (RuntimeInfo.LoggerEnabled())
                Logger.Update(gameTime);

            ScreenManager.UpdateTransition(gameTime);
                
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            this.GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);

            if(DrawableBatch.Begun)
                DrawableBatch.End();

            ScreenManager.DrawTransition(gameTime, DrawableBatch);
            
            DrawableManager.Draw(gameTime, DrawableBatch);
            if (RuntimeInfo.IsDebug() && DrawDebugOverlay)
                DebugOverlayDrawableManager.Draw(gameTime, DrawableBatch);
        }

        #region Timing

        private static Stopwatch _stopwatch = new();
        public static int Time => (int)_stopwatch.ElapsedMilliseconds;

        #endregion
    }
}
