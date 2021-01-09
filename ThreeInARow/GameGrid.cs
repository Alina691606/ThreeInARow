using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ThreeInARow
{
    class GameGrid : Grid
    {
        private static int dimension = 6;
        private Point previousCoordinates;
        private TileImage[,] tileImagesArr = new TileImage[dimension, dimension]; 
        List<string> imageList = new List<string> { "Blue", "Green", "Pink", "Red", "Yellow" };

        public delegate void PointAddition(int points);
        public event PointAddition AddPoints;
        // конструктор
        public GameGrid()
        {
            Width = Height = 600;
            for(int gridSize = 0; gridSize < dimension; gridSize++)
            {
                ColumnDefinitions.Add(new ColumnDefinition());
                RowDefinitions.Add(new RowDefinition());
            }
            
            Random rand = new Random();
            for(int columnCount = 0; columnCount < dimension; columnCount++)
                for(int rowCount = 0; rowCount < dimension; rowCount++)
                {
                    do
                    {
                        string color = imageList[rand.Next(0, 5)];
                        PlaceTImage(rowCount, columnCount, color);
                    } while (CheckThrees(rowCount, columnCount));
                }
        }

        // получение элемента по индексам
        public UIElement GetElement(int row, int column)
        {
            return Children.Cast<UIElement>().FirstOrDefault(e => GetRow(e) == row && GetColumn(e) == column);
        }

        // добавление тайла по индексам
        private TileImage PlaceTImage(int x, int y, string color)
        {
            RemoveTImage(x, y);
            var timage = new TileImage(color);
            SetRow(timage, x);
            SetColumn(timage, y);
            timage.MouseDown += OnMouseClick;
            timage.Drop += TargetDrop;
            Children.Add(timage);
            return timage;
        }

        // удаление тайла по индексам 
        private void RemoveTImage(int x, int y)
        {
            if (GetElement(x, y) != null)
                Children.Remove(GetElement(x, y));
        }
        // удаление по ссылке (полиморфизм)
        private void RemoveTImage(TileImage tile)
        {
            if (tile != null)
                Children.Remove(tile);
        }

        // проверка на 3 подряд влево и вверх
        private bool CheckThrees(int row, int column)
        {
            bool hasThrees = false;
            TileImage curImage = (TileImage)GetElement(row, column);
            if (row > 1)
            {
                TileImage imageMinusRow = (TileImage)GetElement(row-1, column);
                TileImage imageMinus2Row = (TileImage)GetElement(row - 2, column);
                if(curImage.ColorName == imageMinusRow.ColorName && curImage.ColorName == imageMinus2Row.ColorName)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                TileImage imageMinusColumn = (TileImage)GetElement(row, column-1);
                TileImage imageMinus2Column = (TileImage)GetElement(row, column-2);
                if (curImage.ColorName == imageMinusColumn.ColorName && curImage.ColorName == imageMinus2Column.ColorName)
                {
                    return true;
                }
            }
            return hasThrees;
        }

        // инициация перетаскивания
        public void OnMouseClick(object sender, RoutedEventArgs e)
        {
            var timage = (TileImage)sender;
            int x = GetRow(timage);
            int y = GetColumn(timage);
            previousCoordinates.X = x;
            previousCoordinates.Y = y;
            DragDrop.DoDragDrop(timage, timage.ColorName, DragDropEffects.Copy);
        }

        // действие при отпускании 
        public void TargetDrop(object sender, DragEventArgs e)
        {
            var timage = (TileImage)sender;
            int x = GetRow(timage);
            int y = GetColumn(timage);
            
            if (Math.Abs(x - (int)previousCoordinates.X) + Math.Abs(y-(int)previousCoordinates.Y) == 1)
            {
                string colorName = (string)e.Data.GetData(DataFormats.StringFormat);
                PlaceTImage(x, y, colorName);
                PlaceTImage((int)previousCoordinates.X, (int)previousCoordinates.Y, timage.ColorName);
                MatchAndClear();
            }
        }


        private void CopyBoard()
        {
            for(int x = 0; x< dimension; x++)
                for(int y = 0; y < dimension; y++)
                {
                    tileImagesArr[x, y] = (TileImage)GetElement(x, y);
                }
        }

        // нахождение рядов клеток
        private int tripleX = 0;
        private int tripleY = 0;
        TileImage currentTile;
        List<TileImage> collector = new List<TileImage>();
        public void MatchAndClear()
        {
            CopyBoard();
            currentTile = null;
            collector.Clear();
            for (int y = 0; y < dimension; y++)
                for (int x = 0; x < dimension; x++)
                {
                    TestTile(x, y);
                    if (collector.Count >= 3 && (tripleX>=3 || tripleY>=3))
                    {
                        foreach(TileImage tile in collector)
                        {
                            RemoveTImage(tile);
                            AddPoints(1);
                        }
                    }
                    currentTile = null;
                    collector.Clear();
                    tripleY = tripleX = 0;
                }
            FillEmptyTiles();
        }

        private void TestTile(int x, int y)
        {
            if (tileImagesArr[x, y] == null)
            {
                return;
            }
            if(currentTile == null)
            {
                currentTile = tileImagesArr[x, y];
                tileImagesArr[x, y] = null;
                collector.Add(currentTile);
            }
            else if (currentTile.ColorName != tileImagesArr[x, y].ColorName)
            {
                return;
            }
            else
            {
                collector.Add(tileImagesArr[x, y]);
                tileImagesArr[x, y] = null;
            }

            if (x > 0)
            {
                tripleX++;
                TestTile(x - 1, y);
            }
            if (y > 0)
            {
                tripleY++;
                TestTile(x, y - 1);
            }
            if (x < dimension - 1)
            {
                tripleX++;
                TestTile(x + 1, y);
            }
            if (y < dimension - 1)
            {
                tripleY++;
                TestTile(x, y + 1);
            }
        }

        // заполенеие освободившихся клеток
        private void FillEmptyTiles()
        {
            Random rand = new Random();
            bool hasNulls = false;
            for (int columnCount = 0; columnCount < dimension; columnCount++)
                for (int rowCount = 0; rowCount < dimension; rowCount++)
                {
                    if (GetElement(rowCount, columnCount) == null)
                    {
                        hasNulls = true;
                        string color = imageList[rand.Next(0, 5)];
                        PlaceTImage(rowCount, columnCount, color);
                    }
                }
            if(hasNulls == true)
            {
                MatchAndClear();
            }
        }
    }
}
