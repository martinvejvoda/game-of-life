using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace Game_of_Life
{
    public class Record
    {
        public Generation Generation { get; private set; }

        public Rules Rules { get; private set; }

        public Record()
        {
            Generation = new Generation();
            Rules = new Rules();
        }

        public Record(Rules r, Generation g)
        {
            HashSet<Point> cells = new HashSet<Point>(g.Cells);
            Generation = new Generation(g.GenerationNumber, g.Cells);

            HashSet<int> survive = new HashSet<int>(r.Survive);
            HashSet<int> revive = new HashSet<int>(r.Revive);
            HashSet<Point> surroundings = new HashSet<Point>(r.Surroundings);
            Rules = new Rules(r.Survive, r.Revive, r.Surroundings);
        }
    }
}
