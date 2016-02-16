/*
  Application Name:     Unitgate  
  Created by:           Allan Wamming Lie
  Created:              2014-2016
  Company:              Lie Development
  E-mail:               unitgate@allanlie.dk 
  Web:                  unitgate.eu
 */
using System;
using System.IO;

namespace UnitGate.Service
{
    public static class ErrorHandlingService
    {
    
        private static string _fileName = "errorLog.txt";
        private static string _folderName = "UnitGate";
        private static string _filePath =  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


        public static void PersistError(string from, Exception ex)
        {
            string folderPath = Path.Combine(_filePath, _folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, _fileName);

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + ex.Message + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                   "" + Environment.NewLine + "Date :" + DateTime.Now);
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }



    }
}
