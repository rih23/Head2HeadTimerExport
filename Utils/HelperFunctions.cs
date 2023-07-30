using Celeste.Mod.TimerExport.Structs;
using Celeste.Mod.Meta;
using MonoMod.Utils;
using System;

namespace Celeste.Mod.TimerExport.HelperFunctions{
    public static class Utils{
        public static timerStruct ticksToTime(long ticks){
            timerStruct timerStruct = new timerStruct();
            ticks /= 10000;
            timerStruct.ms = (ticks % 1000).ToString();
            timerStruct.ms = PadTo(timerStruct.ms, '0', 3);
            ticks /= 1000;
            timerStruct.seconds = (ticks % 60).ToString();
            timerStruct.seconds = PadTo(timerStruct.seconds, '0', 2);
            ticks /= 60;
            timerStruct.minutes = (ticks % 100).ToString();
            timerStruct.minutes = PadTo(timerStruct.minutes, '0', 2);

            return timerStruct;
        }

        public static string PadTo(string str, char paddingChar, int size){
            while(str.Length < size){
                str = str+paddingChar;
            }
            return str;
        }
    }
}