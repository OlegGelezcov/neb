using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client
{
    public class InvalidRaceException : NebulaException
    {
        public Race Race { get; private set; }
        public InvalidRaceException(Race race) : base() 
        {
            this.Race = race;
        }

        public InvalidRaceException(Race race, string message)
            : base(message)
        {
            this.Race = race;
        }

        public InvalidRaceException(Race race, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Race = race;
        }
    }
}
