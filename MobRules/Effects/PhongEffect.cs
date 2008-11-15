using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MobRules.Helpers;

namespace MobRules.Effects
{
    class PhongEffect
    {
        private Effect phongShader;
        
        private EffectParameter wvp;
        private EffectParameter world;
        private EffectParameter lightPos;
        private EffectParameter intensity;
        private EffectParameter texture;
        private EffectParameter colour;

        public PhongEffect()
        {
        }

        public void Initialize()
        {
            phongShader = ServiceHelper.Content.Load<Effect>(@"Shaders\Phong");

            wvp       = phongShader.Parameters["wvp"];
            world     = phongShader.Parameters["world"];
            lightPos  = phongShader.Parameters["light"];
            intensity = phongShader.Parameters["intensity"];
            texture   = phongShader.Parameters["tex"];
            colour    = phongShader.Parameters["colour"];
            phongShader.CommitChanges();
        }

        public Effect Shader
        {
            get { return phongShader; }
        }

        #region Effect Parameters
        public Matrix WorldViewProjection
        {
            set
            {
                wvp.SetValue(value);
            }
        }

        public Matrix World
        {
            set
            {
                world.SetValue(value);
            }
        }

        public Vector4 LightPosition
        {
            set
            {
                lightPos.SetValue(value);
            }
        }

        public float LightIntensity
        {
            set
            {
                intensity.SetValue(value);
            }
        }

        public Texture2D Texture
        {
            set
            {
                texture.SetValue(value);
            }
        }

        public Vector4 LightColour
        {
            set
            {
                colour.SetValue(value);
            }
        }
        #endregion
    }
}
