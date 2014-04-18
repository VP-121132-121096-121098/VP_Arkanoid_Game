using System;
using System.Collections.Generic;
using System.IO;

namespace ArkanoidGame.Framework
{

    /// <summary>
    /// Класа за пишување на лог во датотеки. Имплементацијата
    /// е слична на Singleton шаблон (по еден објект за секоја датотека
    /// во која ќе се запишува).
    /// </summary>
    public class FileLogger
    {
        public enum LogVerbosity
        {
            normal = 1,
            debug = 2
        }

        private static readonly Object loggerLock = new Object();

        /// <summary>
        /// Својството Verbosity контролира дали ќе работи <c>LogDebug</c> методот.
        /// Ако вредноста на Verbosity е еднаква на LogVerbosity.debug тогаш 
        /// LogDebug работи слично како Log, единствена разлиак е 
        /// што се испишува "Debug: " пред секој стринг.
        /// Во спротивно секој повик кон LogDebug се игнорира. Предефинирана вредност
        /// е LogVerbosity.debug
        /// </summary>
        public static LogVerbosity Verbosity { get; set; }

        private TextWriter writer; /*за запишување во датотеката. Потребно е Flush 
                                    * да се повикува после секој повик кон Writee*/

        /// <summary>
        /// Приватен конструктор, дизајн сличен на Singleton шаблонот за дизајн.
        /// </summary>
        /// <param name="filePath">Патека до датотеката во која ќе се запишува</param>
        private FileLogger(String filePath)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(filePath, "stdout"))
                writer = TextWriter.Synchronized(Console.Out);
            else if (StringComparer.OrdinalIgnoreCase.Equals(filePath, "stderr"))
                writer = TextWriter.Synchronized(Console.Error);
            else
            {
                StreamWriter sw = new StreamWriter(filePath);
                writer = TextWriter.Synchronized(sw);
            }
        }

        /// <summary>
        /// Враќа инстанца од класата Logger со кој може да се запишува во 
        /// датотеката која се праќа како аргумент на овој метод.
        /// Притоа како аргумент се праќа апсолутна или релативна патека до датотеката.
        /// Повик на овој метод со ист аргумент повеќе пати враќа референци кон ист
        /// објект. За една датотека постои само еден Logger. Thread safe.
        /// Забелешка: Може аргументот да биде и "stdout" или "stderr", но ако се вклучени и 
        /// "stdout" и "stderr" НЕ СМЕАТ и двата да претставуваат иста датотека. Пример
        /// не смее "Izlez.txt" да биде и стандарден излез и стандарден излез за грешка.
        /// </summary>
        /// <param name="filePath">Патека до датотеката во која ќе се запишува</param>
        /// <returns>Logger кој запишува во датотеката чија патека е filePath</returns>
        public static FileLogger GetInstance(string filePath)
        {
            FileLogger loggerInstance = null;
            if (!FileLogger.instances.TryGetValue(filePath, out loggerInstance))
            {
                lock (loggerLock)
                {
                    if (!FileLogger.instances.TryGetValue(filePath, out loggerInstance))
                        FileLogger.instances.Add(filePath, loggerInstance = new FileLogger(filePath));
                }
            }

            return loggerInstance;
        }

        /// <summary>
        /// Враќа инстанца од класата Logger со кој може да се запишува во 
        /// датотеката Log.txt
        /// Повеќе повици на овој метод враќаат референци кон ист објект.
        /// Thread safe.
        /// </summary>
        /// <returns>Logger кој запишува во датотеката Log.txt</returns>
        public static FileLogger GetInstance()
        {
            return GetInstance("Log.txt");
        }

        /// <summary>
        /// Го запишува стрингот text пратен како аргумент во формат
        /// "ден.месец.година час:минути:секунди: text"
        /// </summary>
        /// <param name="text">Текстот кој се запишува</param>
        public void Log(String text)
        {
            writer.WriteLine(string.Format("{0}: ", DateTime.Now) + text);
            writer.Flush();
        }


        /// <summary>
        /// Запишува форматиран стринг при што додава датум пред стрингот во формат
        /// "ден.месец.година час:минути:секунди:"
        /// Параметрите format и args се однесуваат исто како
        /// <c>string.format(string format, params object[] args)</c>
        /// </summary>
        /// <param name="format">види документација за 
        /// <c>string.format(string format, params object[] args)</c></param>
        /// <param name="args">види документација за 
        /// <c>string.format(string format, params object[] args)</c></param>
        public void Log(String format, params object[] args)
        {
            writer.WriteLine(string.Format("{0}: ", DateTime.Now) + format, args);
            writer.Flush();
        }

        /// <summary>
        /// Го запишува стрингот text пратен како аргумент во формат
        /// "Debug: ден.месец.година час:минути:секунди: text"
        /// </summary>
        /// <param name="text">Текстот кој се запишува</param>
        public void LogDebug(String text)
        {
            if (Verbosity != LogVerbosity.debug)
                return;
            this.Log("Debug: " + text);
        }

        /// <summary>
        /// Запишува форматиран стринг при што се додава Debug и датум пред стрингот во формат
        /// "Debug: ден.месец.година час:минути:секунди:"
        /// Параметрите format и args се однесуваат исто како
        /// <c>string.format(string format, params object[] args)</c>
        /// </summary>
        /// <param name="format">види документација за 
        /// <c>string.format(string format, params object[] args)</c></param>
        /// <param name="args">види документација за 
        /// <c>string.format(string format, params object[] args)</c></param>
        public void LogDebug(String format, params object[] args)
        {
            if (Verbosity != LogVerbosity.debug)
                return;
            this.Log("Debug: " + format, args);
        }

        /// <summary>
        /// Го запишува стрингот text пратен како аргумент во формат
        /// "Error: ден.месец.година час:минути:секунди: text"
        /// </summary>
        /// <param name="text">Текстот кој се запишува</param>
        public void LogError(String error)
        {
            this.Log("Error: " + error);
        }

        /// <summary>
        /// Запишува форматиран стринг при што се додава Error и датум пред стрингот во формат
        /// "Error: ден.месец.година час:минути:секунди:"
        /// Параметрите format и args се однесуваат исто како
        /// <c>string.format(string format, params object[] args)</c>
        /// </summary>
        /// <param name="format">види документација за 
        /// <c>string.format(string format, params object[] args)</c></param>
        /// <param name="args">види документација за 
        /// <c>string.format(string format, params object[] args)</c></param>
        public void LogError(String format, params object[] args)
        {
            this.Log("Error: " + format, args);
        }


        /// <summary>
        /// Статички конструктор за иницијализација на Verbosity и instances.
        /// </summary>
        static FileLogger()
        {
            Verbosity = LogVerbosity.debug;
            instances = new Dictionary<string, FileLogger>(StringComparer.OrdinalIgnoreCase);
        }

        //мапа <string,Logger> кадешто како стринг се
        //чува името на датотеката, а Logger е соодветниот
        //Logger за таа датотека 
        private static Dictionary<string, FileLogger> instances;
    }
}
