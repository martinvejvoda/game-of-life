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
        /// Pořadové číslo dané generace buněk.
        /// </summary>
        public int GenerationNumber { get; set; }

        /// <summary>
        /// Bezparametrický konstruktor.
        /// </summary>
        public Generation()
        {
            Cells = new HashSet<Point>();
            GenerationNumber = new int();
        }

        /// <summary>
        /// Konstruktor, který přiřadí pravidla a číslo generace, ale žádnou živou buňku.
        /// </summary>
        /// <param name="generationNumber">pořadové číslo generace</param>
        public Generation(int generationNumber)
        {
            Cells = new HashSet<Point>();
            GenerationNumber = generationNumber;
        }

        /// <summary>
        /// Kontruktor, který přiřadí pravidla, číslo generace a i živé buňky.
        /// </summary>
        /// <param name="generationNumber">pořadové číslo generace</param>
        /// <param name="cells">množina všech živých buněk</param>
        public Generation(int generationNumber, HashSet<Point> cells)
        {
            Cells = cells;
            GenerationNumber = generationNumber;
        }

        /// <summary>
        /// Metoda, která vypočítá a vrátí další generace. Následující generace je vypočítána pomocí aktuální
        /// generace a pravidel, která byla nastavena.
        /// </summary>
        /// <returns>Množina, obsahující následující generaci živých buněk.</returns>
        public Generation NextGeneration(Rules rules)
        {
            Generation nextGeneration = new Generation(GenerationNumber + 1); //množina obsahující následující generaci

            //cyklus, který projde všechny živé buňky
            foreach (Point cell in Cells)
            {
                //do další generace jsou přidány ty buňky, které splňují daná pravidla
                //to obstará metoda EvaluationOfCell
                if (rules.EvaluationOfCell(cell, Cells))
                    nextGeneration.Cells.Add(cell);

                //cyklus, který pro danou buňku projde všechny okolní buňky, ale jen ty, které má mysl brát v úvahu
                foreach (Point direction in rules.Surroundings)
                {
                    //buňka, která sousedéís aktuální buňkou "cell"
                    //okolí buňky dle pravidel je v tomto případě bráno v úvahu obráceně
                    Point surroundingCell = new Point(cell.X - direction.X, cell.Y - direction.Y);
                    if ((!Cells.Contains(surroundingCell)) && rules.EvaluationOfCell(surroundingCell, Cells))
                        nextGeneration.Cells.Add(surroundingCell); //vyhodnocení buňky a možné přídání do další generace
                }
            }

            return nextGeneration;
        }

        
    }
}
