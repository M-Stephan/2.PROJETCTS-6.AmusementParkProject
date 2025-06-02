using System;

namespace Park
{
    public class GridState
    {
        public string[,] Grid { get; private set; }

        public GridState()
        {
            Grid = new string[10, 10];
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    Grid[i, j] = "[green]🟩[/]";
        }

        public void PlaceAttraction(int x, int y, string attractionSymbol)
        {
            if (x < 0 || x >= 10 || y < 0 || y >= 10)
                throw new ArgumentOutOfRangeException("Coordonnées hors limites.");

            Grid[x, y] = attractionSymbol;
        }
    }
}
