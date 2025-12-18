using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame
{
    public class Ink_SaveBitStorage
    {
        /// <summary>
        /// Stores IDs and values.
        /// </summary>
        public List<(string id, int value)> Values { get; set; } = [];
    }
}
