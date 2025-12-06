using EmotionalBasketGame.Actors;
using EmotionalBasketGame.Actors.Targets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace EmotionalBasketGame.Screens
{
    public class Ink_Screen_Minigame : Ink_GameScreen_Base
    {
        public Ink_Screen_Minigame(EmotionalBasketGame game) : base(game)
        {
            BackgroundColor = Color.LightGray;
        }


        /// <summary>
        /// Loads game content for all current actors.
        /// </summary>
        /// <param name="graphics">The graphics device.</param>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            base.LoadContent(graphics, content);

            System.Random rand = new System.Random();
            AddActor(new Ink_TargetReticle(), Vector2.Zero);
            AddActor(new Ink_Target_Base(), new Vector2((float)rand.NextDouble() * 1240 - 620, (float)rand.NextDouble() * 680 - 340), true, 1);
            AddActor(new Ink_Target_Base(), new Vector2((float)rand.NextDouble() * 1240 - 620, (float)rand.NextDouble() * 680 - 340), true, 1);
            AddActor(new Ink_Target_Base(), new Vector2((float)rand.NextDouble() * 1240 - 620, (float)rand.NextDouble() * 680 - 340), true, 1);
        }
    }
}
