using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PrototypeCoopSim.Events;
using PrototypeCoopSim.Objects;
using PrototypeCoopSim.RenderLayer;
using PrototypeCoopSim.Managers;
using System.Collections.Generic;
using PrototypeCoopSim.Settings;


namespace PrototypeCoopSim.AIProfiles
{
    class Priority
    {
        public String priorityName;

        public Priority()
        {

        }
        
        public virtual bool Process(Game gameIn, mapManager mapIn, EventManager eventManagerIn, gameElement focusElementIn, GameTime gameTimeIn)
        {
            return false;
        }
    }
}
