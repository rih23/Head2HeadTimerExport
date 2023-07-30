namespace Celeste.Mod.TimerExport.Structs{
    public class timerStruct{
        public string minutes;
        public string seconds;
        public string ms;

        public string getTimerString(){
            return minutes+":"+seconds+"."+ms;
        }
    }
}