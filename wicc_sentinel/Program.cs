using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace wincc_sentinel
{
    class Program
    {

        static long filePointer = 0;
        static bool firstTime = true;
        static string sentinelLogFile = "wincc_sentinel_log.txt";

        static void Main(string[] args)
        {
            try
            {
                int argsIndex = 0;
                string logFilePath = args[argsIndex++];
                string timesStr = args[argsIndex++];
                sentinelLogFile = args[argsIndex++];
                string stopProcess = args[argsIndex++];
                string startProcess = args[argsIndex++];
                 
                string startProcessArgs = "";
                for (; argsIndex < args.Length; argsIndex++)
                {
                    startProcessArgs += " "+args[argsIndex];
                }

                log("wincc_sentinel started");
                log("program arguments:");
                log("   - logFilePath: " + logFilePath);
                log("   - stopProcess: " + stopProcess);
                log("   - startProcess: " + startProcess);
                log("   - startProcessArgs: " + startProcessArgs);
                log("   - timesStr: " + timesStr);


                bool sentinelOn = true;
                bool activeAlarm = false;

                while (sentinelOn)
                {
                    Thread.Sleep(30000);
                    activeAlarm = activeAlarm || verifyAlarmActivation(logFilePath);
                    if (activeAlarm && canWinnccRestart(timesStr))
                    {
                        restartWinCC(stopProcess, startProcess, startProcessArgs);
                        Thread.Sleep(60000);
                        firstTime = true;
                        activeAlarm = false;
                    }
                }

                log("wincc_sentinel terminated");
            }
            catch (Exception ex) {
                log("wincc_sentinel fatal error: "+ex.ToString());
                log("wincc_sentinel terminated");
            }
            
        }

        
        private static bool verifyAlarmActivation(string logFilePath)
        {
            using (Stream stream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {

                // the first time thw program is executed we do not read the log file, we just infer where the pointer should start
                // if the file length is minor than the file pointer means the log had rotated and now we are looking a entirely new file
                long fileLength = stream.Length;
                filePointer = firstTime ? fileLength : (filePointer > fileLength) ? 0 : filePointer;
                firstTime = false;

                //set the file pointer to the last read location
                stream.Seek(filePointer, SeekOrigin.Begin);

                // set the pointer to the end of the file
                filePointer = fileLength;

                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        if (line.Contains("REQSTATE_CONNECTION_ERROR"))
                        {
                            log("WinCC fault detected: " + line);
                            return true;
                        }
                        line = reader.ReadLine();
                    }
                }
                return false;
            }
        }

        private static bool canWinnccRestart(string timesStr)
        {
            string time = DateTime.Now.ToString("HH:mm");
            if (timesStr.Contains(time))
            {
                log("WinCC restart authorized time detected: " + time);
                return true;
            }
            return false;
        }


        

        static void restartWinCC(string stopProcess, string startProcess,string startProcessArgs)
        {
            log("WinCC will be restarted");
            runVisualBasicScript(stopProcess);
            Thread.Sleep(30000);
            runProcess(startProcess, startProcessArgs);
        }


        static void runProcess(string process, string args)
        {
            Process.Start("\"" + process + "\"", args);
            log("WinCC start process launched");
        }

        static void runVisualBasicScript(string vbsFilePath)
        {
            String scriptDirectory = new FileInfo(vbsFilePath).Directory.FullName;

            Process scriptProc = new Process();
            scriptProc.StartInfo.FileName = @"wscript";
            scriptProc.StartInfo.WorkingDirectory = scriptDirectory; //<---very important 
            scriptProc.StartInfo.Arguments = "\"" + vbsFilePath + "\"";

            

            //scriptProc.StartInfo.Arguments = "" + vbsFilePath;
            scriptProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //prevent console window from popping up
            scriptProc.Start();
            log("WinCC Stopping process Running...");
            scriptProc.WaitForExit(); // <-- Optional if you want program running until your script exit
            scriptProc.Close();
            log("WinCC Stopping process exited");
        }

        static void log(string msg)
        {
            string log = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") + ": " + msg;

            Console.WriteLine(log);
            using (StreamWriter w = File.AppendText(sentinelLogFile))
            {
                w.Write(log+"\r\n");
            }
        }
    }
}
