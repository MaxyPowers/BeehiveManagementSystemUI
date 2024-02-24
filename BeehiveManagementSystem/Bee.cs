using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeesVault
{
    /// <summary>
    /// Описывает базовые характериситики и возможности пчелы
    /// </summary>
    class Bee
    {
        public virtual float CostPerShift { get; } //Виртуальное свойство возвращает количество потребляемого пчелой мёда

        public string Job { get; private set; } //Хранит строку с названием работы пчелы

        public Bee(string job) //Конструктор базового класса, устанвливает значение поля хранящего обязаности пчелы
        {
            Job = job;
        }
        /// <summary>
        /// Метод иммитирует отработанную смену пчелы. Проверяет и отбирает из хранилища необходимое для работы количестов мёда.
        /// Вызывает метоб "DoJob" для выполнения работы пчелы
        /// </summary>
        public void WorkTheNextShift()
        {
            if (HoneyVault.ConsumeHoney(CostPerShift))
                DoJob();
        }

        public virtual void DoJob() { /*Реализация в субкалассе*/ }
    }
    /// <summary>
    /// Класс "матка" является пчелой, управляет ульем и рабочими пчелами
    /// </summary>
    class Queen : Bee
    {
        private const float EGGS_PER_SHIFT = 0.45f;             //Хранит количество яиц производимых за одну смену
        private const float HONEY_PER_UNASSIGNED_WORKER = 0.5f; //Хранит количество потребляемого мёда незанятым рабочим за смену

        private Bee[] workers = new Bee[0];                     //Массив содержит обекты субклассов (рабочих пчёл)
        private float eggs = 0;                                 //Хранит количество произведённых яиц
        private float unassignedWorkers = 3;                    //Хранит количество незанятых рабочих

        public string StatusReport { get; private set; }        //Свойство хранит отчет матки о состоянии улья
        public override float CostPerShift { get { return 2.15f; } } //Количестов потребляемого маткой мёда за одну смену
        /// <summary>
        /// Конструктор передаёт базовуму классу строку "матка" и назначает обязаности трём незанятым рабочим 
        /// </summary>
        public Queen() : base("Queen")
        {
            AssignBee("Nectar Collector");      //Создать пчелу для сбора нектара
            AssignBee("Honey Manufacturer");    //Создать пчелу для производства мёда
            AssignBee("Egg Care");              //Создать пчелу для ухода за потомством
        }
        /// <summary>
        /// Добавляет новую рабочую пчелу с обязанностями в массив рабочих пчел, если в улье есть незанятые рабочие пчелы
        /// </summary>
        /// <param name="worker">Принимает объект рабочей пчелы</param>
        public virtual void AddWorker(Bee worker)
        {
            if (unassignedWorkers >= 1) //Если имеються незанятые рабочие пчелы
            {
                unassignedWorkers--;    //Уменьшить количество незанятых рабочих пчел
                Array.Resize(ref workers, workers.Length + 1); //Увеличить размер массива рабочих пчел
                workers[workers.Length - 1] = worker; //Присвоить объект новой рабочей пчелы в конец массива рабочих пчел
            }
        }
        /// <summary>
        /// Вызывает метод "AddWorker" для создания пчелы работника по указанному параметру
        /// </summary>
        /// <param name="job">Строковая переменнта указывает тип создаваемой пчелы</param>
        public void AssignBee(string job)
        {
            switch (job)
            {
                case "Nectar Collector":
                    AddWorker(new NectarCollector());   //Методу передаётся новый объект пчелы для сбора нектара
                    break;

                case "Honey Manufacturer":
                    AddWorker(new HoneyManufacturer()); //Методу передаётся новый объект пчелы для производства мёда
                    break;

                case "Egg Care":
                    AddWorker(new EggCare(this));       //Методу передаётся новый объект пчелы для ухода за потомством
                                                        //Конструктор принимает текущий объект класса "Queen"
                    break;
            }
            UpdateStatusReport();                       //Метод обновляет отчет о состоянии улья
        }
        /// <summary>
        /// Метод выполняет обязаности матки: Увеличивает количество яиц, вызывает метод "WorkTheNextShift" 
        /// для всех рабочих пчел из массива, распределяет мёд для не занятых пчел, создаёт новый отчет о состоянии улья
        /// </summary>
        public override void DoJob()
        {
            eggs += EGGS_PER_SHIFT;                                                   //Добавить не родившихся пчел
            foreach (Bee worker in workers)                                           //Пребрать массив рабочих пчел
                worker.WorkTheNextShift();                                            //Выполнить работы для всех пчел
            HoneyVault.ConsumeHoney(unassignedWorkers * HONEY_PER_UNASSIGNED_WORKER); //Распределить мёд для незанятых пчел
            UpdateStatusReport();                                                     //Создать отчет о состоянии улья
        }
        /// <summary>
        /// Метод преобразует отложенные яйца, в незанятых рабочих пчёл на указанное количество
        /// </summary>
        /// <param name="eggsToConvert">Количество преобразуемых пчел</param>
        public void CareForEggs(float eggsToConvert)
        {
            if (eggs >= eggsToConvert)
            {
                eggs -= eggsToConvert;
                unassignedWorkers += eggsToConvert;
            }
        }
        /// <summary>
        /// Создаёт отчет о состоянии хранилищ в улье, колчестве яиц, незатых рабочих и колчестве рабочих пчел каждого субкласса
        /// </summary>
        private void UpdateStatusReport()
        {
            StatusReport = $"{HoneyVault.StatusReport}\n" +
            $"\nEgg count: {eggs:0.0}\nUnassigned workers: {unassignedWorkers:0.0}\n" +
            $"{WorkerStatus("Nectar Collector")}\n{WorkerStatus("Honey Manufacturer")}" +
            $"\n{WorkerStatus("Egg Care")}\nTOTAL WORKERS: {workers.Length}";
        }
        /// <summary>
        /// Метод возвращает строку с количеством рабочих пчел указанной специальности
        /// </summary>
        /// <param name="job">Специальность пчел которых необходимо подсчитать</param>
        /// <returns></returns>
        private string WorkerStatus(string job)
        {
            int count = 0;                      //Хранит количество пчёл
            foreach (Bee worker in workers)
                if (worker.Job == job) count++; //Если специальность пчелы, совпадает с запрашиваемой специальность, увеличить счетчик
            string s = "s";                     //Добавить множественное число
            if (count == 1) { s = ""; }         //Если количество подсчитаных пчел больше одной, будет добавленно множественное число
            return $"{count} {job} bee{s}";     //Вернуть отчет о количестве подсчитанных пчел
        }
        /// <summary>
        /// Субклас хранит информацию и обязанности пчелы производящей мёд
        /// </summary>
        class HoneyManufacturer : Bee
        {
            private const float NECTAR_PROCESSED_PER_SHIFT = 33.15f;    //Количестов производимого мёда за одну смену
            public override float CostPerShift { get { return 1.7f; } } //Количество потребляемого мёда за одну смену
            /// <summary>
            /// Конструктор перелаёт базовому классу строковое значение должности пчелы
            /// </summary>
            public HoneyManufacturer() : base("Honey Manufacturer") { }
            /// <summary>
            /// Вызывает метод преобразубщий нектар в мёд. Передаёт в качестве аргумента количество производимого мёда за смену
            /// </summary>
            public override void DoJob()
            {
                HoneyVault.ConvertNectarToHoney(NECTAR_PROCESSED_PER_SHIFT);
            }
        }
        /// <summary>
        /// Субклас хранит информацию и обязанности пчелы добывающей нектар
        /// </summary>
        class NectarCollector : Bee
        {
            private const float NECTAR_COLLECTED_PER_SHIFT = 33.25f;     //Количество добываемого нектара за одну смену
            public override float CostPerShift { get { return 1.95f; } } //Количество потребляемого мёда за одну смену
            /// <summary>
            /// Конструктор перелаёт базовому классу строковое значение должности пчелы
            /// </summary>
            public NectarCollector() : base("Nectar Collector") { }
            /// <summary>
            /// Вызывает метод добавляющий нектар в хранилище. Передаёт в качестве аргумента колчество добываемого нектара за смену
            /// </summary>
            public override void DoJob()
            {
                HoneyVault.CollectNectar(NECTAR_COLLECTED_PER_SHIFT);
            }
        }
        /// <summary>
        /// Субклас хранит информацию и обязанности пчелы ухаживающей за потомством
        /// </summary>
        class EggCare : Bee
        {
            private const float CARE_PROGRESS_PER_SHIFT = 0.15f;         //Количество новых пчел преобразуемое за одну смену
            public override float CostPerShift { get { return 1.35f; } } //Количество потребляемого мёда за одну смену

            private Queen queen; //Пустая ссылка на обьект класса "Queen"
            /// <summary>
            /// Конструктор перелаёт базовому классу строковое значение должности пчелы. В качестве параметра принимает объект 
            /// класса "Queen"
            /// </summary>
            /// <param name="queen"></param>
            public EggCare(Queen queen) : base("Egg Care")
            {
                this.queen = queen; //Присвоить пустой ссылке, ссылку на текущий объект "Queen"
            }
            /// <summary>
            /// Вызывает метот у объекта "Queen" преобразубщий яйцо в незанятую рабочую пчелу. 
            /// В качестве аргумента, передаёт количество пчёд преобразуемое за смену
            /// </summary>
            public override void DoJob()
            {
                queen.CareForEggs(CARE_PROGRESS_PER_SHIFT);
            }
        }
    }
}
