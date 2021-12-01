using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Vani.Data
{
    public class RecordFrameData 
    {

        public static async Task AppendOneFrameAsync(FrameData fData) 
        {
            using StreamWriter file = new StreamWriter(Application.persistentDataPath + "/babaisyou/data.csv", append: true);
            await file.WriteLineAsync(String.Format("{0},{1:d},{2:d},{3:g},{4:g},{5:d}", fData.frameTime, fData.chapter, fData.level, fData.gameControl, fData.operation, fData.numCommands));
        }

    }
}