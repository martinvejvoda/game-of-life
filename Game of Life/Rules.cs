using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;

namespace Game_of_Life
{
    public class Rules
    {
        /// <summary>
        /// Množina obsahující všechny směry, které jsou brány v úvahu. Směr má 2 složky (x a y),
        /// přičemž každá může nabývat hodnot -1, 0 a 1.
        /// </summary>
        public HashSet<Point> Surroundings = new HashSet<Point>();
        /// <summary>
        /// Množina obsahující počet počet živých sousedů, při kterých buňka obživne.
        /// </summary>
        public HashSet<int> Revive { get; set; }
        /// <summary>
        /// Množina obsahující počet živých sousedů, při kterých buňka přežije.
        /// </summary>
        public HashSet<int> Survive { get; set; }

        /// <summary>
        /// Konstruktor třídy, který všechny množiny nastaví na prázdné.
        /// </summary>
        public Rules()
        {
            Survive = new HashSet<int>();
            Revive = new HashSet<int>();
            Surroundings = new HashSet<Point>();
        }

        /// <summary>
        /// Kontruktor, který do příslušných vlastností dosadí daná pravidla.
        /// </summary>
        /// <param name="survive">množina obsahující počet živých sousedů, při kterých buňka přežije</param>
        /// <param name="revive">množina obsahující počet živých sousedů, při kterých buňka obživne</param>
        /// <param name="surroundings">množina obsahující okolí buňky, které je bráno v úvahu</param>
        public Rules(HashSet<int> survive, HashSet<int> revive, HashSet<Point> surroundings)
        {
            Survive = survive;
            Revive = revive;
            Surroundings = surroundings;
        }
    }
}
