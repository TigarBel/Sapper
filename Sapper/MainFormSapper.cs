using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Sapper
{
    public partial class MainFormSapper : Form
    {
        /// <summary>
        /// Работа таймера
        /// </summary>
        bool timerOn;
        /// <summary>
        /// Объект игры, где проходит вся логига, собственно, игры
        /// </summary>
        Game game;
        /// <summary>
        /// Визуальное поле
        /// </summary>
        PictureBox[,] _pictureBoxsField;
        /// <summary>
        /// Для вызова ф-ции при клике правой и левой кнопки мыши
        /// </summary>
        private bool right = false;
        /// <summary>
        /// Для вызова ф-ции при клике правой и левой кнопки мыши
        /// </summary>
        private bool left = false;

        /// <summary>
        /// Начало работы программы
        /// </summary>
        public MainFormSapper()
        {
            InitializeComponent();
            DoubleBuffered = true; //!!!//
        }
        /// <summary>
        /// Новая игра лёгкой сложности
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void easyGame_Click(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(200, 253);
            this.MaximumSize = new Size(200, 253);
            game = new Game(GameDifficulty.Easy); // Создали новую игру с настройками по дефолту

            game.GameEnded += game_GameEnded;
            labelOfTimer.Text = "0";

            UpdatePictureField();
        }
        /// <summary>
        /// Игра средней сложности
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mediunGame_Click(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(200 + (20 * 7), 253 + (20 * 7));
            this.MaximumSize = new Size(200 + (20 * 7), 253 + (20 * 7));
            game = new Game(GameDifficulty.Medium); // Создали новую игру с настройками по дефолту

            game.GameEnded += game_GameEnded;
            labelOfTimer.Text = "0";

            UpdatePictureField();
        }
        /// <summary>
        /// Игра сложной сложности
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hardGame_Click(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(200 + (20 * 21), 253 + (20 * 7));
            this.MaximumSize = new Size(200 + (20 * 21), 253 + (20 * 7));
            game = new Game(GameDifficulty.Hard); // Создали новую игру с настройками по дефолту

            game.GameEnded += game_GameEnded;
            labelOfTimer.Text = "0";

            UpdatePictureField();
        }
        /// <summary>
        /// Событие при начале игры (запуск таймера)
        /// </summary>
        /// <param name="start">Старт</param>
        private void GameStarted(bool start)
        {
            if (start == true && timerOn == false)
            {
                timer1.Start();
                labelOfTimer.Text = "0";
                timerOn = true;
            }
            if (start == false)
            {
                timer1.Stop();
                timerOn = false;
            }
        }
        /// <summary>
        /// Событие при тиканье таймера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            labelOfTimer.Text = Convert.ToString(Convert.ToInt32(labelOfTimer.Text) + 1);
        }
        /// <summary>
        /// Событие при завершении игры
        /// </summary>
        /// <param name="points">Итог в очках, выигрыл или проигрыш</param>
        void game_GameEnded(int points)
        {
            GameStarted(game.TimerOn = false);
            if (points == 0) MessageBox.Show("Вы выиграли!"); // Выигрыш
            if (points == -1) GameOver(); // Проигрыш
            //GameStarted(game.TimerOn = false);// Иначе просто открываем данную ячейку (если работает данное условие)
        }
        /// <summary>
        /// Ф-ция под событие для окончания игры, как проигрыш
        /// </summary>
        private void GameOver()
        {
            for (int y = 0; y < game.HeightArray; y++)
            {
                for (int x = 0; x < game.WidthArray; x++)
                {
                    if (game.ShowClosedCell(x, y) == 3 && game.Field(x, y) == -1)
                    {   // Если обезвредили мину на данной ячейке
                        game.DefusedCell(x, y);
                    }
                    else if (game.ShowClosedCell(x, y) == 3 && game.Field(x, y) != -1)
                    {   // Если не обезвредили мину на данной ячейке
                        game.NonDefusedCell(x, y);
                    }   // Иначе просто открываем данную ячейку
                    //else if (game.ShowClosedCell(x, y) == 2) game.OpenCell(x, y);
                }
            }
        }
        /// <summary>
        /// Обновление визуального поля
        /// </summary>
        private void UpdatePictureField()
        {
            if (this._pictureBoxsField != null) // Чистим поле формы
            {
                for (int y = 0; y < _pictureBoxsField.GetLength(0); y++)
                {
                    for (int x = 0; x < _pictureBoxsField.GetLength(1); x++)
                    {
                        this.Controls.Remove(this._pictureBoxsField[y, x]);
                    }
                }
            }
            _pictureBoxsField = new PictureBox[game.HeightArray, game.WidthArray]; // Создаём ячейки поля формы
            for (int y = 0; y < game.HeightArray; y++)
            {
                for (int x = 0; x < game.WidthArray; x++)
                {
                    _pictureBoxsField[y, x] = new PictureBox()
                    {
                        Name = Convert.ToString(x) + '_' + Convert.ToString(y), // Ширина_Высота
                        Size = new Size(20, 20),
                        Location = new Point(20 * y + 2, 20 * x + 30),
                    };
                    _pictureBoxsField[y, x].MouseUp += new MouseEventHandler(pictureBoxsField_MouseUp);
                    _pictureBoxsField[y, x].MouseDown += new MouseEventHandler(pictureBoxsField_MouseDown);
                    this.Controls.Add(_pictureBoxsField[y, x]);
                }
            }
            for (int y = 0; y < game.HeightArray; y++)
            {
                for (int x = 0; x < game.WidthArray; x++)
                {
                    switch (game.ShowClosedCell(x, y))
                    {
                        case 1:
                            {   // Вырисовываем ячейку
                                Picture(x, y);
                                break;
                            }
                        case 2:
                            {   // Вырисовываем закрутую ячейку
                                _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.Enable_Field;
                                break;
                            }
                        case 3:
                            {   // Вырисовываем флаг
                                PictureFlag(x, y);
                                break;
                            }
                        case 4:
                            {   // Вырисовываем мину с другим фоном
                                _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.Defused_Mine_Field;
                                break;
                            }
                        case 5:
                            {   // Вырисовываем перечёркнутую мину
                                _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.NonDefused_Mine_Field;
                                break;
                            }
                    }
                }
            }
            GameStarted(game.TimerOn);
        }
        /// <summary>
        /// Скрипт при отпускании кнопки мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxsField_MouseUp(object sender, MouseEventArgs e)
        {
            var pictureBoxs = (PictureBox)sender;
            NameResolution nameResolution = new NameResolution();
            int x = nameResolution.X(pictureBoxs.Name); // Ширина
            int y = nameResolution.Y(pictureBoxs.Name); // Высота
            if (e.Button == MouseButtons.Left
            && e.Location.X >= 0 && e.Location.X <= 20
            && e.Location.Y >= 0 && e.Location.Y <= 20)
            {
                if (game.ShowClosedCell(x, y) == 2)
                {
                    game.OpenCell(x, y);
                }
                else if (game.ShowClosedCell(x, y) == 3)
                {
                    game.FieldClosedCell(x, y);
                    _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.Enable_Field;
                }
            }
            else if (e.Button == MouseButtons.Right
            && e.Location.X >= 0 && e.Location.X <= 20
            && e.Location.Y >= 0 && e.Location.Y <= 20)
            {
                if (game.ShowClosedCell(x, y) == 2)
                {
                    game.MarkCell(x, y);
                }
                else if (game.ShowClosedCell(x, y) == 3)
                {
                    game.FieldClosedCell(x, y);
                    _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.Enable_Field;
                }
            }
            left = right = false; // Чтобы не вызвать следом ф-цию по открыванию вокруг клеток
            UpdatePictureField(); // Обновляем визуальное поле
        }
        /// <summary>
        /// Если условие выполняется, открыть клетки вокруг заданной клетки, кроме флажков
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxsField_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Если нажата левая кнопка мыши
                right = true;
            if (e.Button == MouseButtons.Right) // Если нажата правая кнопка мыши
                left = true;
            if (right && left) // Если нажата обе кнопки мыши
            {
                var pictureBoxs = (PictureBox)sender;
                NameResolution nameResolution = new NameResolution();
                int x = nameResolution.X(pictureBoxs.Name); // Ширина
                int y = nameResolution.Y(pictureBoxs.Name); // Высота
                int numeric = game.Field(x, y);

                int count = 0;
                for (int y2 = y - 1; y2 <= y + 1; y2++)
                {
                    for (int x2 = x - 1; x2 <= x + 1; x2++)
                    {
                        if (x2 >= 0 && y2 >= 0
                            && x2 <= x + 1 && y2 <= y + 1
                            && x2 < game.WidthArray && y2 < game.HeightArray)
                         {
                            if (game.ShowClosedCell(x2, y2) == 3)
                            {
                                count++; // Считаем, сколько флагов вокрук ячейки
                            }
                        }
                    }
                }
                if (count == numeric)
                {
                    for (int y2 = y - 1; y2 <= y + 1; y2++)
                    {
                        for (int x2 = x - 1; x2 <= x + 1; x2++)
                        {
                            if (x2 >= 0 && y2 >= 0
                                && x2 <= x + 1 && y2 <= y + 1
                                && x2 < game.WidthArray && y2 < game.HeightArray)
                            {
                                if (game.ShowClosedCell(x2, y2) == 2)
                                {
                                    game.OpenCell(x2, y2); // Открываем во внутреннем массиве
                                }
                            }
                        }
                    }
                }
                right = left = false;
                UpdatePictureField(); // Обновляем визуальное поле
            }
        }
        /// <summary>
        /// Изменение картинки поля
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        private void Picture(int x, int y) 
        {
            int numeric = game.Field(x, y);
            
            switch (numeric)
            {
                case -1:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.Mine_Field;
                        break; // Проигрыш 
                    }
                case 0:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._0_Field;
                        break;
                    }
                case 1:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._1_Field;
                        break;
                    }
                case 2:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._2_Field;
                        break;
                    }
                case 3:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._3_Field;
                        break;
                    }
                case 4:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._4_Field;
                        break;
                    }
                case 5:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._5_Field;
                        break;
                    }
                case 6:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._6_Field;
                        break;
                    }
                case 7:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._7_Field;
                        break;
                    }
                case 8:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._8_Field;
                        break;
                    }
            }
        }
        /// <summary>
        /// Изменение картинки поля на флаг
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        private void PictureFlag(int x, int y)
        {
            _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.Flag;
        }
    }
}