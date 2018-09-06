using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace Game_of_Life
{
    public class History
    {
        /// <summary>
        /// List obsahující historii - zaznamenávány jsou pouze generace, pří kterých došlo k zásahu uživatele
        /// </summary>
        public List<Record> Changelog { get; set; }
        /// <summary>
        /// Pořadové číslo aktuální vkreslené generace.
        /// </summary>
        public int CurrentGeneration { get; set; }

        public History()
        {
            Changelog = new List<Record>();
            CurrentGeneration = new int();
        }

        /// <summary>
        /// Konstruktor, který do Changelogu přidá první záznam - první generaci bez živých buněk.
        /// </summary>
        /// <param name="rules">pravidla hry</param>
        public History(Rules rules)
        {
            Changelog = new List<Record>();

            HashSet<int> survive = new HashSet<int>(rules.Survive);
            HashSet<int> revive = new HashSet<int>(rules.Revive);
            HashSet<Point> surroundings = new HashSet<Point>(rules.Surroundings);

            Rules r = new Rules(survive, revive, surroundings);
            Generation generation = new Generation(1);

            Record record = new Record(r, generation);
            CurrentGeneration = 1;
            Changelog.Add(record);
        }

        /// <summary>
        /// Přidá záznam do Changelogu. V případě, že se pořadové číslo přidávané generace shoduje s poslední zaznamenanou generací,
        /// je tato poslední generace přepsána.
        /// </summary>
        /// <param name="g">Generace, která se má uložit.</param>
        public void AddRecord(Rules r, Generation g)
        {
            HashSet<int> survive = new HashSet<int>(r.Survive);
            HashSet<int> revive = new HashSet<int>(r.Revive);
            HashSet<Point> surroundings = new HashSet<Point>(r.Surroundings);

            Rules rules = new Rules(survive, revive, surroundings);
            Generation generation = new Generation(g.GenerationNumber, g.Cells);

            Record record = new Record(rules, generation);

            //pokud předchozí záznam je ze stejné generace, je nutné ho nahradit
            if (Changelog[Changelog.Count - 1].Generation.GenerationNumber == generation.GenerationNumber)
                Changelog.RemoveAt(Changelog.Count - 1);

             Changelog.Add(record);

            CurrentGeneration = generation.GenerationNumber; //číslo aktuální vykreslené generace
        }

        /// <summary>
        /// Metoda vrátí generaci se zadaným pořadovým číslem, které obdrží. Zároveň vymaže všechny budoucí záznamy.
        /// </summary>
        /// <param name="number">pořadové číslo generace</param>
        /// <returns>Metoda vrací generaci s požadovaným pořadovým číslem.</returns>
        public Generation LoadRecord(int number)
        {
            Generation wantedGeneration = Changelog[0].Generation; //generace s požadovaným pořadovým číslem
            Generation loadedRecord = Changelog[0].Generation; //záznam načtený z Changelogu

            for (int i = 0; i < Changelog.Count; i++)
            {
                //získání generace, která předchází požadované generaci
                //generaace jsou řazeny chronologicky
                if (Changelog[i].Generation.GenerationNumber < number)
                    loadedRecord = Changelog[i].Generation;
            }

            int difference = number - loadedRecord.GenerationNumber; //počet generací mezi načteným záznamem a požadovanou generací
            wantedGeneration = loadedRecord;
            //dopočítání požadované generace
            for (int i = 0; i < difference; i++)
                wantedGeneration = new Generation(wantedGeneration.GenerationNumber + 1, wantedGeneration.NextGeneration());

            int serialNumber = Changelog.IndexOf(loadedRecord); //pořadové číslo záznamu v Changelogu, který předchází požadované generaci
            //vymazání všech generací, které následují
            Changelog.RemoveRange(serialNumber + 1, Changelog.Count - serialNumber - 1);

            CurrentGeneration = wantedGeneration.GenerationNumber; //aktuální číslo vykreslené generace

            return wantedGeneration;
        }
    }
}
