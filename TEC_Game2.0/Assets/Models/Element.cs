using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Models
{
    [Serializable]
    public class Elements
    {
        public List<Resistor> Resistors;
        public List<Conductor> Conductors;
        public List<Wire> Wires;
        public List<Norator> Norators;
        public List<Nullator> Nullators;
    }

}
