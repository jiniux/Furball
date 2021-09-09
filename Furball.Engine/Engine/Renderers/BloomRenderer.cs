using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Renderers {
    /// <summary>
    ///
    /// Version 1.1, 16. Dez. 2016
    ///
    /// Bloom / Blur, 2016 TheKosmonaut
    ///
    /// High-Quality Bloom filter for high-performance applications
    ///
    /// Based largely on the implementations in Unreal Engine 4 and Call of Duty AW
    /// For more information look for
    /// "Next Generation Post Processing in Call of Duty Advanced Warfare" by Jorge Jimenez
    /// http://www.iryoku.com/downloads/Next-Generation-Post-Processing-in-Call-of-Duty-Advanced-Warfare-v18.pptx
    ///
    /// The idea is to have several rendertargets or one rendertarget with several mip maps
    /// so each mip has half resolution (1/2 width and 1/2 height) of the previous one.
    ///
    /// 32, 16, 8, 4, 2
    ///
    /// In the first step we extract the bright spots from the original image. If not specified otherwise thsi happens in full resolution.
    /// We can do that based on the average RGB value or Luminance and check whether this value is higher than our Threshold.
    ///     BloomUseLuminance = true / false (default is true)
    ///     BloomThreshold = 0.8f;
    ///
    /// Then we downscale this extraction layer to the next mip map.
    /// While doing that we sample several pixels around the origin.
    /// We continue to downsample a few more times, defined in
    ///     BloomDownsamplePasses = 5 ( default is 5)
    ///
    /// Afterwards we upsample again, but blur in this step, too.
    /// The final output should be a blur with a very large kernel and smooth gradient.
    ///
    /// The output in the draw is only the blurred extracted texture.
    /// It can be drawn on top of / merged with the original image with an additive operation for example.
    ///
    /// If you use ToneMapping you should apply Bloom before that step.
    /// </summary>
    public class BloomRenderer : IDisposable
    {
        #region fields & properties

        #region private fields

        //resolution
        private int _width;
        private int _height;

        //RenderTargets
        private RenderTarget2D _bloomRenderTarget2DMip0;
        private RenderTarget2D _bloomRenderTarget2DMip1;
        private RenderTarget2D _bloomRenderTarget2DMip2;
        private RenderTarget2D _bloomRenderTarget2DMip3;
        private RenderTarget2D _bloomRenderTarget2DMip4;
        private RenderTarget2D _bloomRenderTarget2DMip5;

        private SurfaceFormat _renderTargetFormat;

        //Objects
        private GraphicsDevice _graphicsDevice;
        private QuadRenderer _quadRenderer;

        //Shader + variables
        private Effect _bloomEffect;

        private EffectPass _bloomPassExtract;
        private EffectPass _bloomPassExtractLuminance;
        private EffectPass _bloomPassDownsample;
        private EffectPass _bloomPassUpsample;
        private EffectPass _bloomPassUpsampleLuminance;

        private EffectParameter _bloomParameterScreenTexture;
        private EffectParameter _bloomInverseResolutionParameter;
        private EffectParameter _bloomRadiusParameter;
        private EffectParameter _bloomStrengthParameter;
        private EffectParameter _bloomStreakLengthParameter;
        private EffectParameter _bloomThresholdParameter;

        //Preset variables for different mip levels
        private float _bloomRadius1 = 1.0f;
        private float _bloomRadius2 = 1.0f;
        private float _bloomRadius3 = 1.0f;
        private float _bloomRadius4 = 1.0f;
        private float _bloomRadius5 = 1.0f;

        private float _bloomStrength1 = 1.0f;
        private float _bloomStrength2 = 1.0f;
        private float _bloomStrength3 = 1.0f;
        private float _bloomStrength4 = 1.0f;
        private float _bloomStrength5 = 1.0f;

        public float BloomStrengthMultiplier = 1.0f;

        private float _radiusMultiplier = 1.0f;


        #endregion

        #region public fields + enums

        public bool BloomUseLuminance = true;
        public int BloomDownsamplePasses = 5;

        //enums
        public enum BloomPresets
        {
            Wide,
            Focussed,
            Small,
            SuperWide,
            Cheap,
            One
        };

        #endregion

        #region properties
        public BloomPresets BloomPreset
        {
            get { return this._bloomPreset; }
            set
            {
                if (this._bloomPreset == value) return;

                this._bloomPreset = value;
                this.SetBloomPreset(this._bloomPreset);
            }
        }
        private BloomPresets _bloomPreset;


        private Texture2D BloomScreenTexture { set {
            this._bloomParameterScreenTexture.SetValue(value); } }
        private Vector2 BloomInverseResolution
        {
            get { return this._bloomInverseResolutionField; }
            set
            {
                if (value != this._bloomInverseResolutionField)
                {
                    this._bloomInverseResolutionField = value;
                    this._bloomInverseResolutionParameter.SetValue(this._bloomInverseResolutionField);
                }
            }
        }
        private Vector2 _bloomInverseResolutionField;

        private float BloomRadius
        {
            get
            {
                return this._bloomRadius;
            }

            set
            {
                if (Math.Abs(this._bloomRadius - value) > 0.001f)
                {
                    this._bloomRadius = value;
                    this._bloomRadiusParameter.SetValue(this._bloomRadius * this._radiusMultiplier);
                }

            }
        }
        private float _bloomRadius;

        private float BloomStrength
        {
            get { return this._bloomStrength; }
            set
            {
                if (Math.Abs(this._bloomStrength - value) > 0.001f)
                {
                    this._bloomStrength = value;
                    this._bloomStrengthParameter.SetValue(this._bloomStrength * this.BloomStrengthMultiplier);
                }

            }
        }
        private float _bloomStrength;

        public float BloomStreakLength
        {
            get { return this._bloomStreakLength; }
            set
            {
                if (Math.Abs(this._bloomStreakLength - value) > 0.001f)
                {
                    this._bloomStreakLength = value;
                    this._bloomStreakLengthParameter.SetValue(this._bloomStreakLength);
                }
            }
        }
        private float _bloomStreakLength;

        public float BloomThreshold
        {
            get { return this._bloomThreshold; }
            set {
                if (Math.Abs(this._bloomThreshold - value) > 0.001f)
                {
                    this._bloomThreshold = value;
                    this._bloomThresholdParameter.SetValue(this._bloomThreshold);
                }
            }
        }
        private float _bloomThreshold;

        #endregion

        #endregion

        #region initialize

        /// <summary>
        /// Loads all needed components for the BloomEffect. This effect won't work without calling load
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="content"></param>
        /// <param name="width">initial value for creating the rendertargets</param>
        /// <param name="height">initial value for creating the rendertargets</param>
        /// <param name="renderTargetFormat">The intended format for the rendertargets. For normal, non-hdr, applications color or rgba1010102 are fine NOTE: For OpenGL, SurfaceFormat.Color is recommended for non-HDR applications.</param>
        /// <param name="quadRenderer">if you already have quadRenderer you may reuse it here</param>
        public void Load(GraphicsDevice graphicsDevice, ContentManager content, int width, int height, SurfaceFormat renderTargetFormat = SurfaceFormat.Color,  QuadRenderer quadRenderer = null)
        {
            this._graphicsDevice = graphicsDevice;
            this.UpdateResolution(width, height);

            //if quadRenderer == null -> new, otherwise not
            this._quadRenderer = quadRenderer ?? new QuadRenderer(graphicsDevice);

            this._renderTargetFormat = renderTargetFormat;

            //Load the shader parameters and passes for cheap and easy access
            this._bloomEffect                     = content.Load<Effect>("Shaders/Bloom");
            this._bloomInverseResolutionParameter = this._bloomEffect.Parameters["InverseResolution"];
            this._bloomRadiusParameter            = this._bloomEffect.Parameters["Radius"];
            this._bloomStrengthParameter          = this._bloomEffect.Parameters["Strength"];
            this._bloomStreakLengthParameter      = this._bloomEffect.Parameters["StreakLength"];
            this._bloomThresholdParameter         = this._bloomEffect.Parameters["Threshold"];

            //For DirectX / Windows
            this._bloomParameterScreenTexture = this._bloomEffect.Parameters["ScreenTexture"];

            //If we are on OpenGL it's different, load the other one then!
            if (this._bloomParameterScreenTexture == null)
            {
                //for OpenGL / CrossPlatform
                this._bloomParameterScreenTexture = this._bloomEffect.Parameters["LinearSampler+ScreenTexture"];
            }

            this._bloomPassExtract           = this._bloomEffect.Techniques["Extract"].Passes[0];
            this._bloomPassExtractLuminance  = this._bloomEffect.Techniques["ExtractLuminance"].Passes[0];
            this._bloomPassDownsample        = this._bloomEffect.Techniques["Downsample"].Passes[0];
            this._bloomPassUpsample          = this._bloomEffect.Techniques["Upsample"].Passes[0];
            this._bloomPassUpsampleLuminance = this._bloomEffect.Techniques["UpsampleLuminance"].Passes[0];

            //An interesting blendstate for merging the initial image with the bloom.
            //BlendStateBloom = new BlendState();
            //BlendStateBloom.ColorBlendFunction = BlendFunction.Add;
            //BlendStateBloom.ColorSourceBlend = Blend.BlendFactor;
            //BlendStateBloom.ColorDestinationBlend = Blend.BlendFactor;
            //BlendStateBloom.BlendFactor = new Color(0.5f, 0.5f, 0.5f);

            //Default threshold.
            this.BloomThreshold = 0.8f;
            //Setup the default preset values.
            //BloomPreset = BloomPresets.One;
            this.SetBloomPreset(this.BloomPreset);
        }

        /// <summary>
        /// A few presets with different values for the different mip levels of our bloom.
        /// </summary>
        /// <param name="preset">See BloomPresets enums. Example: BloomPresets.Wide</param>
        private void SetBloomPreset(BloomPresets preset)
        {
            switch(preset)
            {
                case BloomPresets.Wide:
                {
                    this._bloomStrength1    = 0.5f;
                    this._bloomStrength2    = 1;
                    this._bloomStrength3    = 2;
                    this._bloomStrength4    = 1;
                    this._bloomStrength5    = 2;
                    this._bloomRadius5      = 4.0f;
                    this._bloomRadius4      = 4.0f;
                    this._bloomRadius3      = 2.0f;
                    this._bloomRadius2      = 2.0f;
                    this._bloomRadius1      = 1.0f;
                    this.BloomStreakLength  = 1;
                    this.BloomDownsamplePasses = 5;
                        break;
                }
                case BloomPresets.SuperWide:
                    {
                        this._bloomStrength1    = 0.9f;
                        this._bloomStrength2    = 1;
                        this._bloomStrength3    = 1;
                        this._bloomStrength4    = 2;
                        this._bloomStrength5    = 6;
                        this._bloomRadius5      = 4.0f;
                        this._bloomRadius4      = 2.0f;
                        this._bloomRadius3      = 2.0f;
                        this._bloomRadius2      = 2.0f;
                        this._bloomRadius1      = 2.0f;
                        this.BloomStreakLength  = 1;
                        this.BloomDownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.Focussed:
                    {
                        this._bloomStrength1    = 0.8f;
                        this._bloomStrength2    = 1;
                        this._bloomStrength3    = 1;
                        this._bloomStrength4    = 1;
                        this._bloomStrength5    = 2;
                        this._bloomRadius5      = 4.0f;
                        this._bloomRadius4      = 2.0f;
                        this._bloomRadius3      = 2.0f;
                        this._bloomRadius2      = 2.0f;
                        this._bloomRadius1      = 2.0f;
                        this.BloomStreakLength  = 1;
                        this.BloomDownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.Small:
                    {
                        this._bloomStrength1    = 0.8f;
                        this._bloomStrength2    = 1;
                        this._bloomStrength3    = 1;
                        this._bloomStrength4    = 1;
                        this._bloomStrength5    = 1;
                        this._bloomRadius5      = 1;
                        this._bloomRadius4      = 1;
                        this._bloomRadius3      = 1;
                        this._bloomRadius2      = 1;
                        this._bloomRadius1      = 1;
                        this.BloomStreakLength  = 1;
                        this.BloomDownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.Cheap:
                    {
                        this._bloomStrength1    = 0.8f;
                        this._bloomStrength2    = 2;
                        this._bloomRadius2      = 2;
                        this._bloomRadius1      = 2;
                        this.BloomStreakLength  = 1;
                        this.BloomDownsamplePasses = 2;
                        break;
                    }
                case BloomPresets.One:
                    {
                        this._bloomStrength1    = 4f;
                        this._bloomStrength2    = 1;
                        this._bloomStrength3    = 1;
                        this._bloomStrength4    = 1;
                        this._bloomStrength5    = 2;
                        this._bloomRadius5      = 1.0f;
                        this._bloomRadius4      = 1.0f;
                        this._bloomRadius3      = 1.0f;
                        this._bloomRadius2      = 1.0f;
                        this._bloomRadius1      = 1.0f;
                        this.BloomStreakLength  = 1;
                        this.BloomDownsamplePasses = 5;
                        break;
                    }
            }
        }

        #endregion

        /// <summary>
        /// Main draw function
        /// </summary>
        /// <param name="inputTexture">the image from which we want to extract bright parts and blur these</param>
        /// <param name="width">width of our target. If different to the input.Texture width our final texture will be smaller/larger.
        /// For example we can use half resolution. Input: 1280px wide -> width = 640px
        /// The smaller this value the better performance and the worse our final image quality</param>
        /// <param name="height">see: width</param>
        /// <returns></returns>
        public Texture2D Draw(Texture2D inputTexture, int width, int height)
        {
            //Check if we are initialized
            if(this._graphicsDevice==null)
                throw new Exception("Module not yet Loaded / Initialized. Use Load() first");

            //Change renderTarget resolution if different from what we expected. If lower than the inputTexture we gain performance.
            if (width != this._width || height != this._height)
            {
                this.UpdateResolution(width, height);

                //Adjust the blur so it looks consistent across diferrent scalings
                this._radiusMultiplier = (float)width / inputTexture.Width;

                //Update our variables with the multiplier
                this.SetBloomPreset(this.BloomPreset);
            }

            this._graphicsDevice.RasterizerState = RasterizerState.CullNone;
            this._graphicsDevice.BlendState         = BlendState.Opaque;

            //EXTRACT  //Note: Is setRenderTargets(binding better?)
            //We extract the bright values which are above the Threshold and save them to Mip0
            this._graphicsDevice.SetRenderTarget(this._bloomRenderTarget2DMip0);

            this.BloomScreenTexture     = inputTexture;
            this.BloomInverseResolution = new Vector2(1.0f / this._width, 1.0f / this._height);

            if (this.BloomUseLuminance)
                this._bloomPassExtractLuminance.Apply();
            else
                this._bloomPassExtract.Apply();
            this._quadRenderer.RenderQuad(this._graphicsDevice, Vector2.One * -1, Vector2.One);

            //Now downsample to the next lower mip texture
            if (this.BloomDownsamplePasses > 0)
            {
                //DOWNSAMPLE TO MIP1
                this._graphicsDevice.SetRenderTarget(this._bloomRenderTarget2DMip1);

                this.BloomScreenTexture = this._bloomRenderTarget2DMip0;
                //Pass
                this._bloomPassDownsample.Apply();
                this._quadRenderer.RenderQuad(this._graphicsDevice, Vector2.One * -1, Vector2.One);

                if (this.BloomDownsamplePasses > 1)
                {
                    //Our input resolution is halfed, so our inverse 1/res. must be doubled
                    this.BloomInverseResolution *= 2;

                    //DOWNSAMPLE TO MIP2
                    this._graphicsDevice.SetRenderTarget(this._bloomRenderTarget2DMip2);

                    this.BloomScreenTexture = this._bloomRenderTarget2DMip1;
                    //Pass
                    this._bloomPassDownsample.Apply();
                    this._quadRenderer.RenderQuad(this._graphicsDevice, Vector2.One * -1, Vector2.One);

                    if (this.BloomDownsamplePasses > 2)
                    {
                        this.BloomInverseResolution *= 2;

                        //DOWNSAMPLE TO MIP3
                        this._graphicsDevice.SetRenderTarget(this._bloomRenderTarget2DMip3);

                        this.BloomScreenTexture = this._bloomRenderTarget2DMip2;
                        //Pass
                        this._bloomPassDownsample.Apply();
                        this._quadRenderer.RenderQuad(this._graphicsDevice, Vector2.One * -1, Vector2.One);

                        if (this.BloomDownsamplePasses > 3)
                        {
                            this.BloomInverseResolution *= 2;

                            //DOWNSAMPLE TO MIP4
                            this._graphicsDevice.SetRenderTarget(this._bloomRenderTarget2DMip4);

                            this.BloomScreenTexture = this._bloomRenderTarget2DMip3;
                            //Pass
                            this._bloomPassDownsample.Apply();
                            this._quadRenderer.RenderQuad(this._graphicsDevice, Vector2.One * -1, Vector2.One);

                            if (this.BloomDownsamplePasses > 4)
                            {
                                this.BloomInverseResolution *= 2;

                                //DOWNSAMPLE TO MIP5
                                this._graphicsDevice.SetRenderTarget(this._bloomRenderTarget2DMip5);

                                this.BloomScreenTexture = this._bloomRenderTarget2DMip4;
                                //Pass
                                this._bloomPassDownsample.Apply();
                                this._quadRenderer.RenderQuad(this._graphicsDevice, Vector2.One * -1, Vector2.One);

                                this.ChangeBlendState();

                                //UPSAMPLE TO MIP4
                                this._graphicsDevice.SetRenderTarget(this._bloomRenderTarget2DMip4);
                                this.BloomScreenTexture = this._bloomRenderTarget2DMip5;

                                this.BloomStrength = this._bloomStrength5;
                                this.BloomRadius   = this._bloomRadius5;
                                if (this.BloomUseLuminance)
                                    this._bloomPassUpsampleLuminance.Apply();
                                else
                                    this._bloomPassUpsample.Apply();
                                this._quadRenderer.RenderQuad(this._graphicsDevice, Vector2.One * -1, Vector2.One);

                                this.BloomInverseResolution /= 2;
                            }

                            this.ChangeBlendState();

                            //UPSAMPLE TO MIP3
                            this._graphicsDevice.SetRenderTarget(this._bloomRenderTarget2DMip3);
                            this.BloomScreenTexture = this._bloomRenderTarget2DMip4;

                            this.BloomStrength = this._bloomStrength4;
                            this.BloomRadius   = this._bloomRadius4;
                            if (this.BloomUseLuminance)
                                this._bloomPassUpsampleLuminance.Apply();
                            else
                                this._bloomPassUpsample.Apply();
                            this._quadRenderer.RenderQuad(this._graphicsDevice, Vector2.One * -1, Vector2.One);

                            this.BloomInverseResolution /= 2;

                        }

                        this.ChangeBlendState();

                        //UPSAMPLE TO MIP2
                        this._graphicsDevice.SetRenderTarget(this._bloomRenderTarget2DMip2);
                        this.BloomScreenTexture = this._bloomRenderTarget2DMip3;

                        this.BloomStrength = this._bloomStrength3;
                        this.BloomRadius   = this._bloomRadius3;
                        if (this.BloomUseLuminance)
                            this._bloomPassUpsampleLuminance.Apply();
                        else
                            this._bloomPassUpsample.Apply();
                        this._quadRenderer.RenderQuad(this._graphicsDevice, Vector2.One * -1, Vector2.One);

                        this.BloomInverseResolution /= 2;

                    }

                    this.ChangeBlendState();

                    //UPSAMPLE TO MIP1
                    this._graphicsDevice.SetRenderTarget(this._bloomRenderTarget2DMip1);
                    this.BloomScreenTexture = this._bloomRenderTarget2DMip2;

                    this.BloomStrength = this._bloomStrength2;
                    this.BloomRadius   = this._bloomRadius2;
                    if (this.BloomUseLuminance)
                        this._bloomPassUpsampleLuminance.Apply();
                    else
                        this._bloomPassUpsample.Apply();
                    this._quadRenderer.RenderQuad(this._graphicsDevice, Vector2.One * -1, Vector2.One);

                    this.BloomInverseResolution /= 2;
                }

                this.ChangeBlendState();

                //UPSAMPLE TO MIP0
                this._graphicsDevice.SetRenderTarget(this._bloomRenderTarget2DMip0);
                this.BloomScreenTexture = this._bloomRenderTarget2DMip1;

                this.BloomStrength = this._bloomStrength1;
                this.BloomRadius   = this._bloomRadius1;

                if (this.BloomUseLuminance)
                    this._bloomPassUpsampleLuminance.Apply();
                else
                    this._bloomPassUpsample.Apply();
                this._quadRenderer.RenderQuad(this._graphicsDevice, Vector2.One * -1, Vector2.One);
            }

            //Note the final step could be done as a blend to the final texture.

            return this._bloomRenderTarget2DMip0;
        }

        private void ChangeBlendState()
        {
            this._graphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        /// <summary>
        /// Update the InverseResolution of the used rendertargets. This should be the InverseResolution of the processed image
        /// We use SurfaceFormat.Color, but you can use higher precision buffers obviously.
        /// </summary>
        /// <param name="width">width of the image</param>
        /// <param name="height">height of the image</param>
        public void UpdateResolution(int width, int height)
        {
            this._width = width;
            this._height   = height;

            if (this._bloomRenderTarget2DMip0 != null)
            {
                this.Dispose();
            }

            this._bloomRenderTarget2DMip0 = new RenderTarget2D(this._graphicsDevice,
                                                               (int) (width),
                                                               (int) (height), false,
                                                               this._renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            this._bloomRenderTarget2DMip1 = new RenderTarget2D(this._graphicsDevice,
                                                               (int) (width/2),
                                                               (int) (height/2), false,
                                                               this._renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            this._bloomRenderTarget2DMip2 = new RenderTarget2D(this._graphicsDevice,
                                                               (int) (width/4),
                                                               (int) (height/4), false,
                                                               this._renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            this._bloomRenderTarget2DMip3 = new RenderTarget2D(this._graphicsDevice,
                                                               (int) (width/8),
                                                               (int) (height/8), false,
                                                               this._renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            this._bloomRenderTarget2DMip4 = new RenderTarget2D(this._graphicsDevice,
                                                               (int) (width/16),
                                                               (int) (height/16), false,
                                                               this._renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            this._bloomRenderTarget2DMip5 = new RenderTarget2D(this._graphicsDevice,
                                                               (int) (width/32),
                                                               (int) (height/32), false,
                                                               this._renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }

        /// <summary>
        ///Dispose our RenderTargets. This is not covered by the Garbage Collector so we have to do it manually
        /// </summary>
        public void Dispose()
        {
            this._bloomRenderTarget2DMip0.Dispose();
            this._bloomRenderTarget2DMip1.Dispose();
            this._bloomRenderTarget2DMip2.Dispose();
            this._bloomRenderTarget2DMip3.Dispose();
            this._bloomRenderTarget2DMip4.Dispose();
            this._bloomRenderTarget2DMip5.Dispose();
        }
    }
}
