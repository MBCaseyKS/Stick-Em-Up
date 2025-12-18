using EmotionalBasketGame.Actors;
using EmotionalBasketGame.Actors.Buttons;
using EmotionalBasketGame.Actors.HUDs;
using EmotionalBasketGame.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EmotionalBasketGame.Screens
{
    /// <summary>
    /// Displays the results screen OVER previous screens.
    /// </summary>
    public class Ink_Screen_Results : Ink_GameScreen_Base
    {
        /// <summary>
        /// The round handler.
        /// </summary>
        public Ink_TargetRound_Layout RoundHandler { get; set; }

        /// <summary>
        /// What screen we're overlaying.
        /// </summary>
        public Ink_GameScreen_Base OverlayScreen { get; set; }

        private Ink_HUD_Results resultsHud;
        private Ink_ButtonHandler buttonHandler;

        public Ink_Screen_Results(Ink_PinGameManager game, int sortPriority) : base(game, sortPriority)
        {
            BackgroundColor = new Color(0, 0, 0, 200);
        }

        /// <summary>
        /// Loads game content for all current actors.
        /// </summary>
        /// <param name="graphics">The graphics device.</param>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            base.LoadContent(graphics, content);
            resultsHud = (Ink_HUD_Results)OpenHUD(new Ink_HUD_Results(Game, this, 1) { RoundHandler = RoundHandler, MenuSelectDel = OnMenuSelect });

            buttonHandler = new Ink_ButtonHandler(Game, this, Actors);
            buttonHandler.MouseActive = true;
        }

        /// <summary>
        /// Handles a game Tick.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (buttonHandler != null)
                buttonHandler.Update(gameTime);
        }

        /// <summary>
        /// Handles when the options to return or restart are selected.
        /// </summary>
        /// <param name="option">The option selected.</param>
        public void OnMenuSelect(int option)
        {
            buttonHandler.CleanUp();
            buttonHandler = null;

            //Menu
            if (option == 1)
            {
                if (OverlayScreen != null)
                {
                    if (RoundHandler != null)
                        RoundHandler.CleanUp();

                    ScreenClosedDel = null;
                    MusicManager.StopTrack("Drive", 0.5);
                    Game.RunLoad(OverlayScreen, new Ink_Screen_Titlescreen(Game), new Ink_ScreenTransition_Shutters(Game, 0.5, 3, Color.Black), new Ink_ScreenTransition_Fade(Game, 0.1, Color.Black), 0.5);
                }
            }

            Game.RemoveScreen(this);
        }

        public override Ink_Actor_Base AddActor(Ink_Actor_Base actor, Vector2 actorPosition, bool doLoad, double depth = 0, int renderPriority = 0)
        {
            actor = base.AddActor(actor, actorPosition, doLoad, depth, renderPriority);

            if (actor is Ink_Button_Base button && buttonHandler != null)
            {
                buttonHandler = new Ink_ButtonHandler(Game, this, Actors);
                buttonHandler.MouseActive = true;
            }

            return actor;
        }
    }
}
