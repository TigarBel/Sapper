using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sapper
{
    /// <summary>
    /// Делегат для вызова "Конец игры!" или "Вы выиграли!"
    /// </summary>
    public delegate void DelegateGameEnd();
    /// <summary>
    /// Класс логики игры сапёра
    /// </summary>
    class Game
    {
        /// <summary>
        /// Ширина массива (игрового поля)
        /// </summary>
        private int _widthArray = 0;
        /// <summary>
        /// Высота массива (игрового поля)
        /// </summary>
        private int _heightArray = 0;
        /// <summary>
        /// Количество мин на поле
        /// </summary>
        private int _mine = 0;
        /// <summary>
        /// Массив внутренний, с числами
        /// </summary>
        private int[,] _field;
        /// <summary>
        /// Массив равный размером с внутренним, для определения открытых, закрытых и помеченных ячеек поля (1-открыто; 2-закрыто; 3-флаг)
        /// </summary>
        private int[,] _fieldClosedCell;
        /// <summary>
        /// Подтверждение проигрыша
        /// </summary>
        private bool _gameOver;
        /// <summary>
        /// Количество вскрытых клеток и закрытых мин
        /// </summary>
        private bool _gameWin;
        /// <summary>
        /// Количество очков для выигрыша
        /// </summary>
        private int _pointsForWin;
        /// <summary>
        /// Разиер поля в высоту и ширину для лёгкой игры
        /// </summary>
        private const int easeGameLine = 9;
        /// <summary>
        /// Количество мин для лёгкой игры
        /// </summary>
        private const int easeGameMine = 10;
        /// <summary>
        /// Собитие по окончанию игры
        /// </summary>
        public event DelegateGameEnd GameEnded;
        /// <summary>
        /// Свойство по проверке проигрыша
        /// </summary>
        public bool GameOver
        {
            get { return _gameOver; }
            private set
            {
                if (value == true)
                {
                    _gameOver = value;
                    GameEnded += new DelegateGameEnd(() => BadEnd());// Подписываемся на событие "Вы проиграли!"
                    PointsForWin = -1;
                }
            }
        }
        /// <summary>
        /// Свойство по проверки выигрыша. Если полученное число равно 0, то открыты (проверенны) все ячейки массива
        /// </summary>
        public bool GameWin
        {
            get { return _gameWin; }
        }
        /// <summary>
        /// Количество очков для выигрыша
        /// </summary>
        private int PointsForWin
        {
            get { return _pointsForWin; }
            set
            {
                _pointsForWin = value;
                if (_pointsForWin == 0)
                {
                    _gameWin = true;
                    GameEnded += new DelegateGameEnd(() => GoodEnd());// Подписываемся на событие "Вы выиграли!"
                }
            }
        }
        /// <summary>
        /// Ширина массива (игрового поля)
        /// </summary>
        public int WidthArray
        {
            get
            {
                return _widthArray;
            }
            set
            {
                if (value >= 9 && value <= 30) _widthArray = value;
            }
        }
        /// <summary>
        /// Высота массиыва (игрового поля)
        /// </summary>
        public int HeightArray
        {
            get
            {
                return _heightArray;
            }
            set
            {
                if (value >= 9 && value <= 16) _heightArray = value;
            }
        }
        /// <summary>
        /// Количество мин. Для внесения количества мин, нужно ввести ширину и высоту поля
        /// </summary>
        public int Mine
        {
            get
            {
                return _mine;
            }
            set
            {
                if (HeightArray >= 9 && HeightArray <= 16 && // Нужная высота у поля
                    WidthArray >= 9 && WidthArray <= 30 &&  // Нужная ширина у поля
                    value >= 10 && value <= 100 &&  // Нужное количество мин
                    value <= (HeightArray * WidthArray) - 2) // Количество мин не превышает площадь поля, максимум 2 свободных поля
                { _mine = value; }
            }
        }

        /// <summary>
        /// Вывод числа обозначающего состояние ячейки массива (октрыка, закрыта или помечена)
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        /// <returns>(1-открыто; 2-закрыто; 3-флаг)</returns>
        public int ShowClosedCell(int x, int y)
        {
            if (y < 0 || y > HeightArray)
            { throw new ArgumentException(" Не корректная высота!"); }
            if (x < 0 || x > WidthArray)
            { throw new ArgumentException(" Не корректная ширина!"); }
            return _fieldClosedCell[y, x];
        }
        /// <summary>
        /// Обозначить ячейку поля открытой
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        private void FieldOpenCell(int x, int y)
        {
            if (y < 0 || y > HeightArray)
            { throw new ArgumentException(" Не корректная высота!"); }
            if (x < 0 || x > WidthArray)
            { throw new ArgumentException(" Не корректная ширина!"); }
            _fieldClosedCell[y, x] = 1;
        }
        /// <summary>
        /// Обозначить ячейку поля не открытой
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        public void FieldClosedCell(int x, int y)
        {
            if (y < 0 || y > HeightArray)
            { throw new ArgumentException(" Не корректная высота!"); }
            if (x < 0 || x > WidthArray)
            { throw new ArgumentException(" Не корректная ширина!"); }
            _fieldClosedCell[y, x] = 2;
            PointsForWin++; // Закрываем ячейку
        }
        /// <summary>
        /// Обозначить ячейку поля помеченной (поставить флажок)
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        private void FieldMarkCell(int x, int y)
        {
            if (y < 0 || y > HeightArray)
            { throw new ArgumentException(" Не корректная высота!"); }
            if (x < 0 || x > WidthArray)
            { throw new ArgumentException(" Не корректная ширина!"); }
            _fieldClosedCell[y, x] = 3;
        }
        /// <summary>
        /// Вывод числа обозначающего близость мин, из конкретной ячейки массива
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        /// <returns>(0-пусто;-1-мина;1,2,3...-близость мин)</returns>
        public int Field(int x, int y)
        {
            if (y < 0 || y > HeightArray)
            { throw new ArgumentException(" Не корректная высота!"); }
            if (x < 0 || x > WidthArray)
            { throw new ArgumentException(" Не корректная ширина!"); }
            return _field[y, x];
        }
        /// <summary>
        /// Включить настройки для лёгкой игры
        /// </summary>
        public void EaseGame()
        {
            HeightArray = easeGameLine;
            WidthArray = easeGameLine;
            Mine = easeGameMine;
        }
        /// <summary>
        /// Создание и размечивание игрового поля
        /// </summary>
        public void Begin()
        {
            GenerateField();
            MarkingField();
            GameEnded = null;
        }
        /// <summary>
        /// Генерация, создание поля с расставленными минами
        /// </summary>
        private void GenerateField()
        {
            _gameOver = false;
            _gameWin = false;
            int count = Mine;
            if (HeightArray == 0)
            {
                throw new ArgumentException(" При создании поля, были введены не коректные данные для высота поля!");
            }
            if (WidthArray == 0)
            {
                throw new ArgumentException(" При создании поля, были введены не коректные данные для ширины поля!");
            }
            if (Mine == 0)
            {
                throw new ArgumentException(" При создании поля, были введены не коректные данные для количества мин!");
            }
            _field = new int[HeightArray, WidthArray];
            _fieldClosedCell = new int[HeightArray, WidthArray];
            PointsForWin = HeightArray * WidthArray; // Количество открытых (проверенных) клеток для победы
            while (count > 0)
            {
                int y = new Random().Next(HeightArray);
                int x = new Random().Next(WidthArray);
                if (_field[y, x] != -1)
                {
                    _field[y, x] = -1;
                    count--;
                }
            }
        }
        /// <summary>
        /// Разметить поле(0-пусто;-1-мина;1,2,3...-близость мин)
        /// </summary>
        private void MarkingField()
        {
            if (_field == null) { throw new ArgumentException(" Внутреннее поле пустое. Убедитесь в создании поля!"); }
            for (int y = 0; y < HeightArray; y++)
            {
                for (int x = 0; x < WidthArray; x++)
                {
                    if (_field[y, x] != -1)
                    {
                        int count = 0;
                        for (int m = y - 1; m <= y + 1; m++)
                        {
                            for (int k = x - 1; k <= x + 1; k++)
                            {
                                if (m >= 0 && k >= 0
                                    && m <= y + 1 && k <= x + 1
                                    && m < HeightArray && k < WidthArray)
                                {
                                    if (_field[m, k] == -1)
                                    {
                                        count++;
                                    }
                                }
                            }
                        }
                        _field[y, x] = count;
                    }
                    _fieldClosedCell[y, x] = 2; // Закрываем все поля, для последующего открытия
                }
            }
        }
        /// <summary>
        /// Открыть ячейку поля
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        /// <returns>(0-пусто;-1-мина;1,2,3...-близость мин)</returns>
        public void OpenCell(int x, int y)
        {
            FieldOpenCell(x, y);
            PointsForWin--; // Ещё одна ячейка открыта
            if (Field(x, y) == -1) // Встал на мину. Вызов события "Конец игры!"  
            {
                GameOver = true;
            }
            if (Field(x, y) == 0) { RunOnZero(x, y); } // Вызов рекурсивной ф-ции по открытию массива
            if (GameEnded != null) // Вызов события
            {
                GameEnded();
            }
        }
        /// <summary>
        /// Пометить (поставить флажок) ячейку массива
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        public void MarkCell(int x, int y)
        {
            FieldMarkCell(x, y);
            PointsForWin--; // Ещё одна ячейка открыта
            if (GameWin == true) // Вызов события "Вы выиграли!"
            {
                GameEnded();
            }
        }
        /// <summary>
        /// Рекурсивный алгоритм по открыванию нулевых ячеек массива
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        private void RunOnZero(int x, int y)
        {
            if (x + 1 < WidthArray) // вправо
            {
                if (ShowClosedCell(x + 1, y) != 1)
                {
                    OpenCell(x + 1, y);
                }
            }
            if (x + 1 < WidthArray && y + 1 < HeightArray) // вправо и вниз
            {
                if (ShowClosedCell(x + 1, y + 1) != 1)
                {
                    OpenCell(x + 1, y + 1);
                }
            }
            if (y + 1 < HeightArray) // вниз
            {
                if (ShowClosedCell(x, y + 1) != 1)
                {
                    OpenCell(x, y + 1);
                }
            }
            if (x - 1 >= 0 && y + 1 < HeightArray) // влево и вниз
            {
                if (ShowClosedCell(x - 1, y + 1) != 1)
                {
                    OpenCell(x - 1, y + 1);
                }
            }
            if (x - 1 >= 0) // влево
            {
                if (ShowClosedCell(x - 1, y) != 1)
                {
                    OpenCell(x - 1, y);
                }
            }
            if (x - 1 >= 0 && y - 1 >= 0) // влево и вверх
            {
                if (ShowClosedCell(x - 1, y - 1) != 1)
                {
                    OpenCell(x - 1, y - 1);
                }
            }
            if (y - 1 >= 0) // вверх
            {
                if (ShowClosedCell(x, y - 1) != 1)
                {
                    OpenCell(x, y - 1);
                }
            }
            if (x + 1 < WidthArray && y - 1 >= 0) // вправо и вверх
            {
                if (ShowClosedCell(x + 1, y - 1) != 1)
                {
                    OpenCell(x + 1, y - 1);
                }
            }
        }
        /// <summary>
        /// Метод проигрыша для события по окончанию игры
        /// </summary>
        public void BadEnd()
        {
            for (int y = 0; y < HeightArray; y++)
            {
                for (int x = 0; x < WidthArray; x++)
                {
                    //if(game.ShowClosedCell(x,y) == 3 && game.Field(x,y)==-1)
                    //{ _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.Defused_Mine_Field; }
                    //else if (game.ShowClosedCell(x, y) == 3 && game.Field(x, y) != -1)
                    //{ _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.NonDefused_Mine_Field; }
                    FieldOpenCell(x, y);
                }
            }
        }
        /// <summary>
        /// Метод выигрыша для события по окончанию игры // тут будет смайлик в очках
        /// </summary>
        public void GoodEnd()
        {
            for (int y = 0; y < HeightArray; y++)
            {
                for (int x = 0; x < WidthArray; x++)
                {
                    MarkCell(x, y);
                }
            }
        }
    }
}
