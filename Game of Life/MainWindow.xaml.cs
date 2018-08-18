using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Game_of_Life
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Rules rules;
        Draw draw;
        History history;
        Generation generation;
        System.Windows.Threading.DispatcherTimer timer;
        Generation[] last2Generations = new Generation[2]; //pole obsahující 2 předcházející generace
        Point currentPostition; //bod, na kterým se aktuálně nachází ukazatel myši
        Point startingPosition; //bod, ve kterém uživatel zmáčkne levé tlačítko myši

        public MainWindow()
        {
            InitializeComponent();

            //nastavení nového timeru
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick+= new System.EventHandler(Timer_Elapsed); //přidání události, která se vykoná při každém ticku
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000); //nastavení intervalu

            //nastavení základních pravidel hry pro přežívání buněk
            int[] surv = new int[] { 2, 3 };
            HashSet<int> survive = new HashSet<int>(surv);
            survive2CheckBox.IsChecked = true;
            survive3CheckBox.IsChecked = true;

            //nastavení základnách pravidel pro ožívání buněk
            int[] rev = new int[] { 3 };
            HashSet<int> revive = new HashSet<int>(rev);
            revive3CheckBox.IsChecked = true;

            //nastavení základního okolí buněk, které bude bráno v úvahu
            HashSet<Point> surroundings = new HashSet<Point>();
            Point p;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    p = new Point(i, j);
                    surroundings.Add(p);
                }
            }
            p = new Point(0, 0);
            surroundings.Remove(p);

            rules = new Rules(survive, revive, surroundings);
            DrawSurroundingCanvas(); //vykreslení canvasu zobrazujícího okolí

            history = new History(rules);

            draw = new Draw(gameCanvas);
            generation = new Generation(rules, 1);
            last2Generations[0] = generation;
            last2Generations[1] = generation; 
        }

        private void surviveCheckBox_Click(object sender, RoutedEventArgs e)
        {
            //úprava pravidel pro přežití buňky. 
            int number = int.Parse((sender as CheckBox).Content.ToString());
            if ((bool)(sender as CheckBox).IsChecked)
                rules.Survive.Add(number);
            else
                rules.Survive.Remove(number);

            //Přidání nového záznamu kvůli změně pravidel.
            history.AddRecord(generation);
        }

        private void reviveCheckBox_Click(object sender, RoutedEventArgs e)
        {
            //úprava pravidel pro oživení buňky
            int number = int.Parse((sender as CheckBox).Content.ToString());
            if ((bool)(sender as CheckBox).IsChecked)
                rules.Revive.Add(number);
            else
                rules.Revive.Remove(number);

            //Přidání nového záznamu do kvůli změně pravidel.
            history.AddRecord(generation);
        }

        /// <summary>
        /// Metoda obsluhující zadávání okolí buňky. Je zavolána při zvednutí levého tlačítka myši nad
        /// surrouningCanvas a přidá nebo odbere daný směr.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void surroundingsCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = Mouse.GetPosition(surroundingsCanvas); //bod, kde došlo k zvednutí tlačítka myši
            Point direction = new Point(); //výsledný směr, který je vypočítán z pozice myši

            //výpočet směru, canvas je rozdělen na třetiny
            direction.X = ((int) (mousePosition.X / (surroundingsCanvas.Width / 3))) - 1;
            direction.Y = ((int) ((mousePosition.Y) / (surroundingsCanvas.Height / 3))) - 1;

            //v případě, že se nejedná o samotnou buňku (tedy směr [0, 0]), je nutné upravit pravidla
            if (direction.X != 0 || direction.Y != 0)
            {
                if (rules.Surroundings.Contains(direction))
                    rules.Surroundings.Remove(direction);
                else
                    rules.Surroundings.Add(direction);

                DrawSurroundingCanvas(); //překreslení canvasu
                history.AddRecord(generation); //přidání nového záznamu do historie, protože došlo k úpravě pravidel
            }           
        }

        /// <summary>
        /// Metoda mající na starosti vykreslování canvasu, který zobrazuje okolí buňky.
        /// </summary>
        private void DrawSurroundingCanvas()
        {
            surroundingsCanvas.Children.Clear(); //vymazání canvasu
            int width = Convert.ToInt32(surroundingsCanvas.Width / 3); //nastavení rozměrů buněk
            int height = Convert.ToInt32(surroundingsCanvas.Height / 3);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Rectangle r = new Rectangle();
                    r.Width = width;
                    r.Height = height;
                    r.StrokeThickness = 1; //šířka okraje
                    r.Stroke = Brushes.Black; //barva okraje

                    //nastavení barvy: šedá - buňka, zelená - buňky v tomto směru jsou brány v úvahu, bílá - buňky v tomto směru jsou ignorovány
                    Point p = new Point(i - 1, j - 1);
                    if (i == 1 && j == 1)
                        r.Fill = Brushes.DarkGray;
                    else if (rules.Surroundings.Contains(p))
                        r.Fill = Brushes.LightGreen;
                    else
                        r.Fill = Brushes.White;

                    Canvas.SetTop(r, j * height); //nastavení pozice pro nový čtverec
                    Canvas.SetLeft(r, i * width);

                    surroundingsCanvas.Children.Add(r); //přidání čtverce na Canvas
                }
            }
        }

        /// <summary>
        /// Metoda aktualizující zaškrtnutí checkboxů při změně pravidel, například při načítání uloženého záznamu.
        /// </summary>
        private void UpdateCheckBoxes()
        {
            //ošetření checkboxů pro přežití buňky
            foreach (object element in stackPanelSurvive.Children)
            {
                if (element is CheckBox)
                {
                    int number = int.Parse((element as CheckBox).Content.ToString());
                    if (rules.Survive.Contains(number))
                        (element as CheckBox).IsChecked = true;
                    else
                        (element as CheckBox).IsChecked = false;
                }
            }

            //ošetření checkboxů pro oživení buňky
            foreach (object element in stackPanelRevive.Children)
            {
                if (element is CheckBox)
                {
                    int number = int.Parse((element as CheckBox).Content.ToString());
                    if (rules.Revive.Contains(number))
                        (element as CheckBox).IsChecked = true;
                    else
                        (element as CheckBox).IsChecked = false;
                }
            }
        }

        private void gameCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point difference = new Point(); //o kolik se posunula myš
                difference.X = e.GetPosition(gameCanvas).X - currentPostition.X;
                difference.Y = e.GetPosition(gameCanvas).Y - currentPostition.Y;

                //změna draw.TopLeftCorner o posunutí myši
                draw.TopLeftCorner = new Point(draw.TopLeftCorner.X - difference.X, draw.TopLeftCorner.Y - difference.Y);

                //nastavení nové aktuální pozice myši
                currentPostition = new Point(e.GetPosition(gameCanvas).X, e.GetPosition(gameCanvas).Y);

                draw.Redraw(generation.Cells); //překreslení canvasu
            }
        }

        private void gameCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //nastavení nové aktuální a výchozí pozice
            currentPostition = new Point(e.GetPosition(gameCanvas).X, e.GetPosition(gameCanvas).Y);
            startingPosition = new Point(currentPostition.X, currentPostition.Y);

        }

        /// <summary>
        /// Metoda obsluhující přidávání a odebírání živých buněk. Je zavolána při každém zvednutí 
        /// levého tlačítka myši nad gameCanvas a zabije nebo oživí příslušnou buňku.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Math.Abs(currentPostition.X - startingPosition.X) < 5 && Math.Abs(currentPostition.Y - currentPostition.Y) < 5)
            {
                //pokud se myš posunula o méně než 5 v horizontálním i vertikálním směru, je to vyhodnoceno jako kliknutí
                //a je tedy nutné změnit tav buňky
                Point mousePosition = Mouse.GetPosition(gameCanvas); //aktuální pozice myši na canvasu
                Point cell = new Point(); //buňka, která bude zabita nebo oživena (buňka, na kterou bylo kliknuto)

                //vypočítání souřadnic dané buňky
                cell.X = (int)((draw.TopLeftCorner.X + mousePosition.X) / draw.SideLength);
                if ((draw.TopLeftCorner.X + mousePosition.X) / draw.SideLength < 0)
                    cell.X--; //kvůli zaokrouhlování je nutné pro záporná čísla ještě odečíst 1 (aby -0.2 bylo -1 a ne 0)
                cell.Y = (int)((draw.TopLeftCorner.Y + mousePosition.Y) / draw.SideLength);
                if ((draw.TopLeftCorner.Y + mousePosition.Y) / draw.SideLength < 0)
                    cell.Y--; //kvůli zaokrouhlování je nutné pro záporná čísla ještě odečíst 1 (aby -0.2 bylo -1 a ne 0)

                //zabití nebo oživení dané buňky
                if (generation.Cells.Contains(cell))
                    generation.Cells.Remove(cell);
                else
                    generation.Cells.Add(cell);

                history.AddRecord(generation); //přidání nového záznamu do historie, protože došlo k úpravě buněk 
                draw.Redraw(generation.Cells);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            draw.Redraw(generation.Cells);
        }

        /// <summary>
        /// Zapnutí nebo vypnutí timeru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playPauseButtton_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
                timer.Stop();
            else
                timer.Start();
        }
        
        /// <summary>
        /// Metoda volána při každém ticku timeru. Vypočítá další generaci a překreslí canvas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, EventArgs e)
        {
            //vypočítání další generace
            Generation nextGeneration = new Generation(rules, generation.GenerationNumber + 1, generation.NextGeneration()); //další generace
            last2Generations[generation.GenerationNumber % 2] = generation; //přidání aktuální generace do záznamu 2 posledních generací
            generation = new Generation(nextGeneration.Rules, nextGeneration.GenerationNumber, nextGeneration.Cells);
            //v případě, že se nová generace rovná předchozá nebo generaci před ní, pak jsou buňky ustáleny nebo oscilují s periodou 2
            if (nextGeneration.Cells.SetEquals(last2Generations[0].Cells) || nextGeneration.Cells.SetEquals(last2Generations[1].Cells))
                StableOrTwoPeriodOscillation();
            history.CurrentGeneration++;

            generationTextBox.Text = generation.GenerationNumber.ToString();
            draw.Redraw(generation.Cells); //překreslení canvasu
        }

        /// <summary>
        /// Metoda, která zpomalí tickání timeru, nebo skočí na předchozí generaci.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void minusButton_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                //v případě, že timer běží, tak je délka jeho intervalu prodloužena o 40 ms
                TimeSpan interval = new TimeSpan(0, 0, 0, timer.Interval.Seconds, timer.Interval.Milliseconds + 40);
                timer.Interval = interval;
            }
            else if (generation.GenerationNumber > 1)
            {
                //jestliže timer neběží a nejedná se o první generaci, pak se zobrazí generace předchozí
                //získání předchozí generace
                generation = history.LoadRecord(generation.GenerationNumber - 1);
                history.CurrentGeneration--;

                HashSet<int> survive = new HashSet<int>(generation.Rules.Survive);
                HashSet<int> revive = new HashSet<int>(generation.Rules.Revive);
                HashSet<Point> surroundings = new HashSet<Point>(generation.Rules.Surroundings);
                rules = new Rules(survive, revive, surroundings);
                generation = new Generation(rules, generation.GenerationNumber, generation.Cells);

                //překreslení a aktualizování všech prvků ve formuláři
                UpdateCheckBoxes();
                generationTextBox.Text = generation.GenerationNumber.ToString();
                draw.Redraw(generation.Cells);
                DrawSurroundingCanvas();
            }
        }

        /// <summary>
        /// Metoda, která přeskočí na následující generaci a případně i zrychlí tickání timeru (pokud běží).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void plusButton_Click(object sender, RoutedEventArgs e)
        {
            //vypočítání nové generace a její vykreslení
            Generation nextGeneration = new Generation(rules, generation.GenerationNumber + 1, generation.NextGeneration()); //další generace
            last2Generations[generation.GenerationNumber % 2] = generation; //přidání aktuální generace do záznamu 2 posledních generací
            generation = new Generation(nextGeneration.Rules, nextGeneration.GenerationNumber, nextGeneration.Cells);
            //v případě, že se nová generace rovná předchozá nebo generaci před ní, pak jsou buňky ustáleny nebo oscilují s periodou 2
            if (nextGeneration.Cells.SetEquals(last2Generations[0].Cells) || nextGeneration.Cells.SetEquals(last2Generations[1].Cells))
                StableOrTwoPeriodOscillation();

            generationTextBox.Text = generation.GenerationNumber.ToString();
            draw.Redraw(generation.Cells);
            history.CurrentGeneration++;

            if (timer.IsEnabled)
            {
                //v případě, že timer běží, tak je jeho interval zkrácen o 40 ms
                if (timer.Interval.Milliseconds > 40 || timer.Interval.Seconds > 0 )
                {
                    TimeSpan interval = new TimeSpan(0, 0, 0, 0, timer.Interval.Seconds * 1000 + timer.Interval.Milliseconds - 40);
                    timer.Interval = interval;
                }
            }

        }

        /// <summary>
        /// Metoda skočí na generaci zadanou uživatelem v generationTextBox. Metoda je volána při
        /// stisku tlačítka generationButton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generationButton_Click(object sender, RoutedEventArgs e)
        {
            int genNumber; //číslo generace zadané uživatelem
            //ověření, že uživatel zadal celé číslo
            if (!int.TryParse(generationTextBox.Text, out genNumber))
                MessageBox.Show("Není zadáno číslo", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (genNumber > generation.GenerationNumber)
                {
                    //uživatel chce budoucí generaci
                    int difference = genNumber - generation.GenerationNumber; //počet generací mezi aktuální a chtěnou
                    Generation nextGeneration = generation;
                    //přeskočení na danou generaci pomocí počítání generací před ní
                    for (int i = 0; i < difference; i++)
                    {
                        nextGeneration = new Generation(rules, generation.GenerationNumber + 1, generation.NextGeneration()); //následující generace
                        last2Generations[generation.GenerationNumber % 2] = generation; //přidání aktuální generace do záznamu 2 posledních generací
                        generation = new Generation(nextGeneration.Rules, nextGeneration.GenerationNumber, nextGeneration.Cells);                        
                    }
                    //v případě, že se nová generace rovná předchozá nebo generaci před ní, pak jsou buňky ustáleny nebo oscilují s periodou 2
                    if (nextGeneration.Cells.SetEquals(last2Generations[0].Cells) || nextGeneration.Cells.SetEquals(last2Generations[1].Cells))
                        StableOrTwoPeriodOscillation();
                }
                else if (genNumber < generation.GenerationNumber && genNumber >= 1)
                {
                    //skok na minulou generaci
                    generation = history.LoadRecord(genNumber);
                    history.CurrentGeneration = generation.GenerationNumber;

                    HashSet<int> survive = new HashSet<int>(generation.Rules.Survive);
                    HashSet<int> revive = new HashSet<int>(generation.Rules.Revive);
                    HashSet<Point> surroundings = new HashSet<Point>(generation.Rules.Surroundings);
                    rules = new Rules(survive, revive, surroundings);
                    generation = new Generation(rules, generation.GenerationNumber, generation.Cells);           
                }

                //překreslení a aktualizování všech prvků ve formuláři
                UpdateCheckBoxes();
                generationTextBox.Text = generation.GenerationNumber.ToString();
                draw.Redraw(generation.Cells);
                DrawSurroundingCanvas();
                history.CurrentGeneration = generation.GenerationNumber;
            }
        }

        /// <summary>
        /// Metoda uvede program do výchozího stavu (až na pravidla), tedy vypne timer a zabije všechny živé buňky.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            generation = new Generation(rules, 1); //zabití všech buněk
            last2Generations[0] = generation;
            last2Generations[1] = generation;
            generationTextBox.Text = "1";
            history = new History(rules);
            draw.Redraw(generation.Cells);
            timer.Stop();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            //Vytvoření SaveFileDialogu pro uložení
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.FileName = "game of life"; //výchozí jméno
            sfd.Filter = "XML files(.xml)|*.xml";
            Nullable<bool> result = sfd.ShowDialog(); //Zobrazení dialogu a zjitění výsledku

            if (result == true)
            {
                SaveLoad sv = new SaveLoad();
                sv.Save(history, sfd.FileName + ".xml");
            }
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = ofd.ShowDialog(); //Zobrazení dialogu a zjitění výsledku

            if (result == true)
            {
                SaveLoad sv = new SaveLoad();
                history = sv.Load(ofd.FileName);

                generation = history.LoadRecord(history.CurrentGeneration);

                //překreslení a aktualizování všech ovládacích prvků
                UpdateCheckBoxes();
                draw.Redraw(generation.Cells);
                DrawSurroundingCanvas();
                generationTextBox.Text = generation.GenerationNumber.ToString();
            }

        }

        /// <summary>
        /// Metoda je volána v případě, že jsou generace ustáleny anebo oscilují s periodou délky 2.
        /// </summary>
        private void StableOrTwoPeriodOscillation()
        {
            //Aktuální generace buněk se musí rovnat alespoň jedné z posledních 2 generací. Otázka je které.
            //V případě, že se rovná předchozí, pak se buňky ustálily. Jinak buňky oscilují. 
            if (generation.Cells.SetEquals(last2Generations[(generation.GenerationNumber - 1) % 2].Cells))
                MessageBox.Show("Jednotlivé buňky se už nemění a jsou ustáleny.");
            else
                MessageBox.Show("Buňky oscilují s periodou 2.");

            timer.Stop();
        }
    }
}
