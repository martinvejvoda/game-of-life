using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;

namespace Game_of_Life
{
    public class Generation
    {
        /// <summary>
        /// Množina obsahující živé buňky, které jsou uloženy pomocí souřadnic.
        /// </summary>
        public HashSet<Point> Cells { get; private set; }

        /// <summary>
        /// Instance pravidel, podle kterých pak vzniká další generace.
        /// </summary>
        public Rules Rules { get; set; }

        /// <summary>
        /// Pořadové číslo dané generace buněk.
        /// </summary>
        public int GenerationNumber { get; set; }

        /// <summary>
        /// Bezparametrický konstruktor.
        /// </summary>
        public Generation()
        {
            Rules = new Rules();
            Cells = new HashSet<Point>();
            GenerationNumber = new int();
        }

        /// <summary>
        /// Konstruktor, který přiřadí pravidla a číslo generace, ale žádnou živou buňku.
        /// </summary>
        /// <param name="rules">pravidla hry</param>
        /// <param name="generationNumber">pořadové číslo generace</param>
        public Generation(Rules rules, int generationNumber)
        {
            Rules = rules;
            Cells = new HashSet<Point>();
            GenerationNumber = generationNumber;
        }

        /// <summary>
        /// Kontruktor, který přiřadí pravidla, číslo generace a i živé buňky.
        /// </summary>
        /// <param name="rules">pravidla hry</param>
        /// <param name="generationNumber">pořadové číslo generace</param>
        /// <param name="cells">množina všech živých buněk</param>
        public Generation(Rules rules, int generationNumber, HashSet<Point> cells)
        {
            Rules = rules;
            Cells = cells;
            GenerationNumber = generationNumber;
        }

        /// <summary>
        /// Metoda, která vypočítá a vrátí další generace. Následující generace je vypočítána pomocí aktuální
        /// generace a pravidel, která byla nastavena.
        /// </summary>
        /// <returns>Množina, obsahující následující generaci živých buněk.</returns>
        public HashSet<Point> NextGeneration()
        {
            HashSet<Point> nextGeneration = new HashSet<Point>(); //množina obsahující následující generaci

            //cyklus, který projde všechny živé buňky
            foreach (Point cell in Cells)
            {
                //do další generace jsou přidány ty buňky, které splňují daná pravidla
                //to obstará metoda EvaluationOfCell
                if (EvaluationOfCell(cell))
                    nextGeneration.Add(cell);

                //cyklus, který pro danou buňku projde všechny okolní buňky, ale jen ty, které má mysl brát v úvahu
                foreach (Point direction in Rules.Surroundings)
                {
                    //buňka, která sousedéís aktuální buňkou "cell"
                    //okolí buňky dle pravidel je v tomto případě bráno v úvahu obráceně
                    Point surroundingCell = new Point(cell.X - direction.X, cell.Y - direction.Y);
                    if ((!Cells.Contains(surroundingCell)) && EvaluationOfCell(surroundingCell))
                        nextGeneration.Add(surroundingCell); //vyhodnocení buňky a možné přídání do další generace
                }
            }

            return nextGeneration;
        }

        /// <summary>
        /// Metoda, která pro danou buňku rozhodne, zdali patří do příští generace.
        /// </summary>
        /// <param name="cell">buňka, která bude vyhodnocena</param>
        /// <returns>True, patří-li buňka do další generace, jinak false.</returns>
        private Boolean EvaluationOfCell(Point cell)
        {
            int numberOfNeighbours = 0; //počet živých sousedů buňky
            //cyklus, který spočítá počet živých sousedů dané buňky
            foreach (Point direction in Rules.Surroundings)
            {
                Point surroundingCell = new Point(cell.X + direction.X, cell.Y + direction.Y);
                if (Cells.Contains(surroundingCell))
                    numberOfNeighbours++;
            }

            if (Cells.Contains(cell))
            {
                //buňka žije a je tedy nutné rozhodnout, jestli přežije
                if (Rules.Survive.Contains(numberOfNeighbours))
                    return true;
            }
            else
            {
                //buňka je mrtvá a je tedy nutné rozhodnout, jestli obživne
                if (Rules.Revive.Contains(numberOfNeighbours))
                    return true;
            }

            return false;
        }
    }
}
