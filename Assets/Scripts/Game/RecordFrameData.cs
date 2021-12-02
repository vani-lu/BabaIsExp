using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Vani.Data
{
    public class RecordFrameData 
    {

        public static async Task AppendOneFrameAsync(string savePath, FrameData fData) 
        {
            using StreamWriter file = new StreamWriter(savePath, append: true);
            await file.WriteLineAsync(string.Format("{0},{1:d},{2:d},{3:g},{4:g},{5:d}", fData.frameTime, fData.chapter, fData.level, fData.gameControl, fData.operation, fData.numCommands));
        }

        public static async Task SetColNamesAsync(string savePath)
        {
            string[] lines =
            {
                "TimeFromStart,Chapter,Level,Control,Operation,NumOfCommands"
            };
            await Task.Run(() => File.WriteAllLines(savePath, lines));
        }

    }
}