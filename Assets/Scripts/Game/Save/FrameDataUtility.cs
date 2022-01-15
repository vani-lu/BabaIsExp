using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Gfen.Game.Logic;

namespace Vani.Data
{
    public class FrameDataUtility 
    {

        public static async Task AppendOneFrameAsync(string savePath, FrameData fData) 
        {
            using StreamWriter file = new StreamWriter(savePath, append: true);
            await file.WriteLineAsync(string.Format("{0:d},{1},{2:d},{3:d},{4:g},{5:g},{6:d}", 
                                                    FrameData.count, fData.frameTime, 
                                                    fData.chapter, fData.level, 
                                                    fData.gameControl, fData.operation, 
                                                    fData.numCommands));
        }

        public static async Task InitFrameData(string savePath)
        {
            // Set headers for a new file
            if(!File.Exists(savePath)){
                string[] lines =
                {
                    "Count,TimeFromLaunch,Chapter,Level,Control,Operation,NumOfCommands"
                };
                await Task.Run(() => File.WriteAllLines(savePath, lines));
            }

        }


        public static async Task MarkLogin(string savePath) 
        {
            using StreamWriter file = new StreamWriter(savePath, append: true);
            FrameData fData = new FrameData(
                Time.unscaledTime,
                -1,
                -1,
                GameControlType.Login,
                OperationType.None,
                -1
            );
            await file.WriteLineAsync(string.Format("{0:d},{1},{2:d},{3:d},{4:g},{5:g},{6:d}", 
                                                    FrameData.count, fData.frameTime, 
                                                    fData.chapter, fData.level, 
                                                    fData.gameControl, fData.operation, 
                                                    fData.numCommands));
        }

        public static async Task MarkLogout(string savePath) 
        {
            using StreamWriter file = new StreamWriter(savePath, append: true);
            FrameData fData = new FrameData(
                Time.unscaledTime,
                -1,
                -1,
                GameControlType.Logout,
                OperationType.None,
                -1
            );
            await file.WriteLineAsync(string.Format("{0:d},{1},{2:d},{3:d},{4:g},{5:g},{6:d}", 
                                                    FrameData.count, fData.frameTime, 
                                                    fData.chapter, fData.level, 
                                                    fData.gameControl, fData.operation, 
                                                    fData.numCommands));
        }

        public static async Task MarkEnableHint(string savePath, int chapter, int level) 
        {
            using StreamWriter file = new StreamWriter(savePath, append: true);
            FrameData fData = new FrameData(
                Time.unscaledTime,
                chapter,
                level,
                GameControlType.EnableHint,
                OperationType.None,
                -1
            );
            await file.WriteLineAsync(string.Format("{0:d},{1},{2:d},{3:d},{4:g},{5:g},{6:d}", 
                                                    FrameData.count, fData.frameTime, 
                                                    fData.chapter, fData.level, 
                                                    fData.gameControl, fData.operation, 
                                                    fData.numCommands));
        }

        public static async Task MarkToggleHint(string savePath, int chapter, int level) 
        {
            using StreamWriter file = new StreamWriter(savePath, append: true);
            FrameData fData = new FrameData(
                Time.unscaledTime,
                chapter,
                level,
                GameControlType.ToggleHint,
                OperationType.None,
                -1
            );
            await file.WriteLineAsync(string.Format("{0:d},{1},{2:d},{3:d},{4:g},{5:g},{6:d}", 
                                                    FrameData.count, fData.frameTime, 
                                                    fData.chapter, fData.level, 
                                                    fData.gameControl, fData.operation, 
                                                    fData.numCommands));
        }
    }

}