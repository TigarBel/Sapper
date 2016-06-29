using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sapper
{
    /// <summary>
    /// Игровая сложность
    /// </summary>
    public enum GameDifficulty
    {
        Easy = 1,
        Medium,
        Hard
    };
    /// <summary>
    /// Делегат для вызова события "Завершить игру"
    /// </summary>
    public delegate void DelegateGameEnd(int points);
    /// <summary>
    /// Класс логики игры сапёра
    /// </summary>
    class Game
    {
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
                if (value == 0 || value == -1) // 0 - выиграл, -1 - проиграл
                {
                    _gameWin = true;
                    GameEnded(value); // Зажег событие
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
                return _field.GetLength(1);
            }
        }
        /// <summary>
        /// Высота массиыва (игрового поля)
        /// </summary>
        public int HeightArray
        {
            get
            {
                return _field.GetLength(0);
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
                if (value >= 10 &&  // Нужное количество мин
                    value <= (HeightArray * WidthArray) / 2) // Количество мин не превышает площадь поля, максимум 2 свободных поля
                { _mine = value; }
                else throw new ArgumentException("Количество мин не может быть больше половины количества ячеек поля, и меньше 10!");
            }
        }

        /// <summary>
        /// Дефолтный конструктор класса Game
        /// </summary>
        /// <param name="gameDifficulty">Игровая сложность</param>
        public Game(GameDifficulty gameDifficulty)
        {
            switch (gameDifficulty)
            {
                case GameDifficulty.Easy:
                    {
                        _field = new int[9, 9]; // Создаём пустое поле близости мин
                        _fieldClosedCell = new int[9, 9]; // Создаём пустое поле закрытых ячеек
                        Mine = 10;
                        GenerateField(); // Генерируем поле
                        break;
                    }
                case GameDifficulty.Medium:
                    {
                        _field = new int[16, 16]; // Создаём пустое поле близости мин
                        _fieldClosedCell = new int[16, 16]; // Создаём пустое поле закрытых ячеек
                        Mine = 50;
                        GenerateField(); // Генерируем поле
                        break;
                    }
                case GameDifficulty.Hard:
                    {
                        _field = new int[30, 16]; // Создаём пустое поле близости мин
                        _fieldClosedCell = new int[30, 16]; // Создаём пустое поле закрытых ячеек
                        Mine = 99;
                        GenerateField(); // Генерируем поле
                        break;
                    }
            }
        }
        /// <summary>
        /// Конструктор с водои параметр для класса Game
        /// </summary>
        /// <param name="x">Ширина поля</param>
        /// <param name="y">Высота поля</param>
        /// <param name="mine">Количество мин</param>
        public Game(int x, int y, int mine)
        {
            if (x < 9 || x > 30) throw new ArgumentException("Ширина поля не может быть меньше 9 или больше 30 ячеек!");
            if (y < 9 || y > 24) throw new ArgumentException("Высота поля не может быть меньше 9 или больше 24 ячеек!");
            _field = new int[y, x]; // Создаём пустое поле близости мин
            _fieldClosedCell = new int[y, x]; // Создаём пустое поле закрытых ячеек
            Mine = mine;
            GenerateField(); // Генерируем поле
        }
        /// <summary>
        /// Вывод числа обозначающего состояние ячейки массива (октрыка, закрыта или помечена)
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        /// <returns>(1-открыто; 2-закрыто; 3-флаг)</returns>
        public int ShowClosedCell(int x, int y)
        {
            return _fieldClosedCell[y, x];
        }
        /// <summary>
        /// Обозначить ячейку поля открытой
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        private void FieldOpenCell(int x, int y)
        {
            _fieldClosedCell[y, x] = 1;
        }
        /// <summary>
        /// Обозначить ячейку поля закрытой
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        public void FieldClosedCell(int x, int y)
        {
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
            _fieldClosedCell[y, x] = 3;
        }
        /// <summary>
        /// Обозначить ячейку поля обезвреженной (поставить мину с другим фоном)
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        public void DefusedCell(int x, int y)
        {
            _fieldClosedCell[y, x] = 4;
        }
        /// <summary>
        /// Обозначить ячейку поля ошибочным обезвреживанием (поставить перечёркнутую мину)
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        public void NonDefusedCell(int x, int y)
        {
            _fieldClosedCell[y, x] = 5;
        }
        /// <summary>
        /// Вывод числа обозначающего близость мин, из конкретной ячейки массива
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        /// <returns>(0-пусто;-1-мина;1,2,3...-близость мин)</returns>
        public int Field(int x, int y)
        {
            return _field[y, x];
        }
        private void GenerateField()
        {
            _gameOver = false;
            _gameWin = false;
            int count = Mine;
            PointsForWin = HeightArray * WidthArray; // Количество открытых (проверенных) клеток для победы
            var random = new Random();
            int y, x;
            while (count > 0)
            {
                y = random.Next(HeightArray);
                x = random.Next(WidthArray);
                if (_field[y, x] != -1)
                {
                    _field[y, x] = -1;
                    count--;
                    for (int m = y - 1; m <= y + 1; m++)
                    {
                        for (int k = x - 1; k <= x + 1; k++)
                        {
                            if (m >= 0 && k >= 0
                                && m <= y + 1 && k <= x + 1
                                && m < HeightArray && k < WidthArray)
                            {
                                if (_field[m, k] != -1)
                                {
                                    _field[m, k]++; // Увеличиваем близость мин в ячейках вокруг мины на 1
                                }
                            }
                        }
                    }
                }
            }
            for (y = 0; y < HeightArray; y++)
            {
                for (x = 0; x < WidthArray; x++)
                {
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
            if (Field(x, y) == -1) // Встал на мину. Вызов события "Конец игры!"  
            {
                GameOver = true;
            }
            else PointsForWin--; // Иначе ещё одна ячейка открыта
            if (Field(x, y) == 0) { RunOnZero(x, y); } // Вызов рекурсивной ф-ции по открытию массива
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
    }
}