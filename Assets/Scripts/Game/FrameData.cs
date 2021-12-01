using Gfen.Game.Logic;
namespace Vani.Data
{
    // records data for a single frame update
    public class FrameData
    {
        public float frameTime;
        public int chapter;
        public int level;
        public GameControlType gameControl;
        public OperationType operation;
        public int numCommands;

        public FrameData(float t, int chp, int lvl, GameControlType gc, OperationType ot, int nc){
            // set values upon instantiation
            frameTime = t;
            chapter = chp;
            level = lvl;
            gameControl = gc;
            operation = ot;
            numCommands = nc;
        }
    }
}
