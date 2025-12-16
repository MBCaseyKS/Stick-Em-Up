using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EmotionalBasketGame.Actors
{
    /// <summary>
    /// Uses a tilemap to draw the wall and board.
    /// </summary>
    public class Ink_OfficeBackground : Ink_Actor_Base
    {
        /// <summary>The map filename</summary>
        private string _mapFilename;

        /// <summary>The tileset texture</summary>
        private Texture2D _tilesetTexture;

        /// <summary>The scaling of each tile.</summary>
        private float _tileScale;

        /// <summary>The map and tile dimensions</summary>
        private int _tileWidth, _tileHeight, _mapWidth, _mapHeight;

        /// <summary>The tileset data</summary>
        private Rectangle[] _tiles;

        /// <summary>The map data</summary>
        private int[] _map;

        /// <summary>
        /// Inits a new tileset.
        /// </summary>
        /// <param name="fileName">The map filename</param>
        public Ink_OfficeBackground(string fileName)
        {
            _mapFilename = fileName;
        }

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            // Read in the map file
            string data = File.ReadAllText(Path.Join(content.RootDirectory, _mapFilename));
            var lines = data.Split('\n');

            // First line is tileset image file name 
            var tilesetFileName = lines[0].Trim();
            _tilesetTexture = content.Load<Texture2D>(tilesetFileName);

            //New line is the tile scaling.
            var scaleLine = lines[1].Trim();
            _tileScale = float.Parse(scaleLine);

            // Third line is tile size
            var secondLine = lines[2].Split(',');
            _tileWidth = int.Parse(secondLine[0]);
            _tileHeight = int.Parse(secondLine[1]);

            // Now that we know the tile size and tileset
            // image, we can determine tile bounds
            int tilesetColumns = _tilesetTexture.Width / _tileWidth;
            int tilesetRows = _tilesetTexture.Height / _tileWidth;
            _tiles = new Rectangle[tilesetColumns * tilesetRows];

            for (int y = 0; y < tilesetRows; y++)
            {
                for (int x = 0; x < tilesetColumns; x++)
                {
                    _tiles[y * tilesetColumns + x] = new Rectangle(
                        x * _tileWidth + (x >= 1 ? 1 : 0), // upper left-hand x cordinate
                        y * _tileHeight + (y >= 1 ? 1 : 0), // upper left-hand y coordinate
                        _tileWidth, // width 
                        _tileHeight // height
                        );
                }
            }

            // Third line is map size (in tiles)
            var thirdLine = lines[3].Split(',');
            _mapWidth = int.Parse(thirdLine[0]);
            _mapHeight = int.Parse(thirdLine[1]);

            // Fourth line is map data
            _map = new int[_mapWidth * _mapHeight];
            var fourthLine = lines[4].Replace(" ", "").Split(',');
            for (int i = 0; i < _mapWidth * _mapHeight; i++)
            {
                _map[i] = (i >= fourthLine.Length) ? -1 : int.Parse(fourthLine[i]);
            }
        }

        /// <summary>
        /// Updates the actor's visuals.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">The sprite batch provided.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            Vector2 position = GetScreenPosition();
            float tileScale = GetScreenScale() * GetDepthScale() * _tileScale;

            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    // Indexes start at 1, so shift for array coordinates
                    int index = _map[y * _mapWidth + x] - 1;
                    // Index of -1 (shifted from 0) should not be drawn
                    if (index <= -1) continue;

                    spriteBatch.Draw(
                        _tilesetTexture,
                        position + new Vector2(x * _tileWidth, y * _tileHeight) * tileScale,
                        _tiles[index],
                        Color.White,
                        0f,
                        Vector2.Zero,
                        tileScale + 0.01f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }
    }
}
