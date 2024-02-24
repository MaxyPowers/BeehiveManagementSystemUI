using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeesVault
{
    /// <summary>
    /// Класс описывает состояние пчелиного улья.
    /// </summary>
    static class HoneyVault
    {
        private const float NECTAR_CONVERSION_RATIO = .19f; //Константа хранит множитель, для превращения нектара в мёд
        private const float LOW_LEVEL_WARNING = 10f; //Константа хранит значения критически низкого уровня запасов

        private static float honey = 25f; //Поле для хранения количества мёда
        private static float nectar = 100f; //Поле для хранения количества нектара
        /// <summary>
        /// Свойство выводит состояние полей "Мёд" и "Нектар" + предупреждение о критически низком уровне запасов
        /// </summary>
        public static string StatusReport
        {
            get
            {
                if (nectar <= LOW_LEVEL_WARNING) //Добавить в отчет сообщение о низком уровне нектара
                {
                    return $"Vault report: \n{honey:0.0} units of honey \n{nectar:0.0} units of nectar " +
                        $"\n LOW NECTAR - ADD A NECTAR COLLECTOR";
                }
                else if(honey <= LOW_LEVEL_WARNING) //Добавить в отчет сообщение о низком уровне нектара
                {
                    return $"Vault report: \n{honey:0.0} units of honey \n{nectar:0.0} units of nectar " +
                        $"\n LOW HONEY - ADD A HONEY MANUFACTURER";
                }
                else //Вывести стандартный отчет
                {
                    return $"Vault report: \n{honey:0.0} units of honey \n{nectar:0.0} units of nectar ";
                }
                
            }
        }
        /// <summary>
        /// Добавляет указанное количество нектара в хранилище
        /// </summary>
        /// <param name="amount">Количество добаляемого нектара</param>
        public static void CollectNectar(float amount)
        {
            if(amount > 0) nectar += amount;
            else return;
        }
        /// <summary>
        /// Преобразует указанное количество нектара в мёд
        /// </summary>
        /// <param name="amount">Количество преобразуемного нектара</param>
        public static void ConvertNectarToHoney(float amount)
        {
            if (amount > nectar) amount = nectar; //Если указанное количество больше остатка нектара,
                                                  //преобразовать весь оставшийся нектар
            nectar -= amount;
            honey += (float)amount * NECTAR_CONVERSION_RATIO;
        }
        /// <summary>
        /// Возвращает истинну, если в хранилище достаточно мёда для потребителя и уменьшает количество мёда в хранилише на потребляемое количество
        /// </summary>
        /// <param name="amount">Потребляемое количество мёда</param>
        /// <returns></returns>
        public static bool ConsumeHoney(float amount)
        {
            if(amount > honey) return false; //Если мёда недостаточно, вернуть false
            else honey -= amount;            //Либо уменьшить количестов мёда и вернуть true
            return true;
        }
    }
}
