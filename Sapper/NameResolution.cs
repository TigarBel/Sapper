using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sapper
{
    /// <summary>
    /// Класс извлечения координат из имени
    /// </summary>
    public class NameResolution
    {
        /// <summary>
        /// Метод извелечения Х-ой координаты
        /// </summary>
        /// <param name="name">Имя подходящего объекта</param>
        /// <returns>Число координаты Х</returns>
        public int X(string name)
        {
            int i = 0;
            int x = 0;
            string strX = "";
            do
            {
                strX += name[i];
                i++;
            } while (name[i] != '_');
            x = Convert.ToInt32(strX);
            return x;
        }
        /// <summary>
        /// Метод извелечения Y-ой координаты
        /// </summary>
        /// <param name="name">Имя подходящего объекта</param>
        /// <returns>Число координаты Y</returns>
        public int Y(string name)
        {
            int i = 0;
            int y = 0;
            string strY = "";
            while (name[i] != '_')
            {
                i++;
            }
            do
            {
                i++;
                strY += name[i];
            } while (name.Length > i + 1);
            y = Convert.ToInt32(strY);
            return y;
        }
    }
}
