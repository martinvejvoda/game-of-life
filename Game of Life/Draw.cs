using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media;
using System.Text;
using System.Threading.Tasks;

namespace Game_of_Life
{
    class Draw
    {
        /// <summary>
        /// Vlastnost určijící, jak je posunuta vykreslovaná plocha. V základu [0, 0], ale posunutí uživatele to může změnit.
        /// </summary>
        public Point TopLeftCorner { get; set; }
        private Canvas canvas;
        /// <summary>
        /// Délka strany jedné buňnky.
        /// </summary>
        public int SideLength { get; private set; }

        /// <summary>
        /// Konstruktor, který nastaví počátek souřadnic (TopLesftCorner), délku strany a canvas, na který se bude vykreslovat.
        /// </summary>
        /// <param name="canvas">Canvas, na který se bude vykreslovat.</param>
        public Draw(Canvas canvas)
        {
            this.canvas = canvas;
            Point p = new Point(0, 0);
            TopLeftCorner = p;
            SideLength = 25;
        }


        /// <summary>
        /// Vykreslení aktuální generace na canvas.
        /// </summary>
        /// <param name="cells">Buňky, které jsou živé a tedy budou vykresleny.</param>
        public void Redraw(HashSet<Point> cells)
        {
            canvas.Children.Clear(); //vymazání aktuálního obsahu na canvasu
            DrawCells(cells); //vykreslení živých buněk
            DrawLines(); //vykreslení čar, které tak oddělí jednotlivé buňky od sebe
        }

        /// <summary>
        /// Vykreslení vodorovný a svyslých čar na canvas. Ty ve výsledku vytvoří mříž a tedy i jednotlivé buňky.
        /// </summary>
        public void DrawLines()
        {
            int horizontalOffset = SideLength - ((int)(TopLeftCorner.X % SideLength)); //posunutí první svislé čáry ve směru osy X
            int verticalOffset = SideLength - ((int)(TopLeftCorner.Y % SideLength)); //posunutí první vertikální čáry ve směru osy Y

            //vykreslení svislých čar
            for (int i = 0; i < (int)(canvas.ActualWidth / SideLength); i++)
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = 2;

                line.X1 = horizontalOffset + i * SideLength;
                line.Y1 = 0;
                line.X2 = horizontalOffset + i * SideLength;
                line.Y2 = canvas.ActualHeight;

                canvas.Children.Add(line);
            }

            //vykreslení vodorovných čar
            for (int i = 0; i < (int)(canvas.ActualHeight / SideLength); i++)
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = 2;

                line.X1 = 0;
                line.Y1 = verticalOffset + i * SideLength;
                line.X2 = canvas.ActualWidth;
                line.Y2 = verticalOffset + i * SideLength;

                canvas.Children.Add(line);
            }
        }

        /// <summary>
        /// Vykreslení živých buněk, které se nachází na canvasu.
        /// </summary>
        /// <param name="cells">Živé buňky, které budou vykresleny.</param>
        public void DrawCells(HashSet<Point> cells)
        {
            Point topLeftCell = new Point(); //buňka nacházející se v levém horním rohu
            topLeftCell.X = (int)(TopLeftCorner.X / SideLength);
            topLeftCell.Y = (int)(TopLeftCorner.Y / SideLength);

            foreach (Point cell in cells)
            {
                //Vyhodnocení, zda-li se buňka nachází na canvasu. Pokud ne, nebude vykreslena.
                if (IsCellInCanvas(cell))
                {
                    //nastavení parametrů nového čtverce
                    Rectangle r = new Rectangle();
                    r.Fill = Brushes.DarkGray;
                    r.Width = SideLength;
                    r.Height = SideLength;

                    Point topLeft = new Point(); //pozice levého horního rohu dané buňky
                    topLeft.X = SideLength * (cell.X - topLeftCell.X - 1) + (SideLength - (int)(TopLeftCorner.X % SideLength));
                    topLeft.Y = SideLength * (cell.Y - topLeftCell.Y - 1) + (SideLength - (int)(TopLeftCorner.Y % SideLength));

                    //přidání čtverce na canvas
                    Canvas.SetTop(r, topLeft.Y);
                    Canvas.SetLeft(r, topLeft.X);
                    canvas.Children.Add(r);
                }
            }
        }

        /// <summary>
        /// Určí, jestli se daná živá buňka nachází na canvasu (tedy jestli je viditelná).
        /// </summary>
        /// <param name="cell">Živá buňka.</param>
        /// <returns>Jetliže se buˇka nachází na canvasu, pak metoda vrací True, jinak False.</returns>
        public Boolean IsCellInCanvas(Point cell)
        {
            int leftBorder = (int)TopLeftCorner.X / SideLength; //X-ová souřadnice buněk na levé straně canvasu.
            int rightBorder = leftBorder + (int)(canvas.ActualWidth / SideLength); //X-ová souřadnice na pravé straně canvasu.
            int topBorder = (int)TopLeftCorner.Y / SideLength; //Y-ová souřadnice buněk na horním okraji canvasu.
            int bottomBorder = topBorder + (int)(canvas.ActualHeight / SideLength); //Y-ová souřadnice na dolním okraji canvasu.

            //vyhodnocení, zda-li se buňka nachází mezi příslušnými mezemi
            if (cell.X < leftBorder || cell.X > rightBorder || cell.Y < topBorder || cell.Y > bottomBorder)
                return false;

            return true;
        }
    }
}
