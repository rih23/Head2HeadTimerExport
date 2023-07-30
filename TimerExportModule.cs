using System;
using System.IO;
using Celeste.Mod.TimerExport.HelperFunctions;
using Celeste.Mod.TimerExport.Structs;
using Celeste.Mod.TimerExport;
using Celeste.Mod.Head2Head.Data;
using Celeste.Mod.Head2Head.IO;
using Celeste.Mod.Head2Head.Shared;
using System.Collections.Generic;

namespace Celeste.Mod.TimerExport {
    public class TimerExportModule : EverestModule {
        public static TimerExportModule Instance { get; private set; }

        public override Type SettingsType => typeof(TimerExportModuleSettings);
        public static TimerExportModuleSettings Settings => (TimerExportModuleSettings) Instance._Settings;

        public override Type SessionType => typeof(TimerExportModuleSession);
        public static TimerExportModuleSession Session => (TimerExportModuleSession) Instance._Session;

        public TimerExportModule() {
            Instance = this;
#if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel(nameof(TimerExportModule), LogLevel.Verbose);
#else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel(nameof(TimerExportModule), LogLevel.Info);
#endif
        }

        public override void Load() {
            CNetComm.OnReceiveMatchUpdate += OnMatchUpdate; //Checking everytime there is a match update
        }

        public override void Unload() {
            CNetComm.OnReceiveMatchUpdate -= OnMatchUpdate;
        }

        private void OnMatchUpdate(DataH2HMatchUpdate data){
            //Doing nothing if the match update doesn't update to completed or if new match value is null
            if(data.NewDef != null){
                if(data.NewDef.State == MatchState.Completed){
                        MatchDefinition match = data.NewDef;
                        if(match.Result != null){
                            processMatchInfo(match);
                        }
                    }
                }
        }

        public static void processMatchInfo(MatchDefinition match){
            using (StreamWriter writer = new StreamWriter(Settings.FilePath, append: true)){
                //Header for match
                DateTime currentDateTime = DateTime.Now;
                writer.WriteLine("-----------"+currentDateTime+"----------");
                writer.WriteLine(match.MatchDisplayName);
                writer.WriteLine("-----------------------");

                //Get results
                Dictionary<PlayerID, MatchResultPlayer> results = match.Result.players;

                //Process each player in result
                foreach(KeyValuePair<PlayerID, MatchResultPlayer> playerResult in results){
                    if(playerResult.Value.Result == ResultCategory.Completed){
                        //Converting the time from ticks to ms ss mm 
                        //Padding the time values with zeros
                        timerStruct resultTime = Utils.ticksToTime(playerResult.Value.FileTimeTotal);
                        string timeString = resultTime.getTimerString();
                        //Write to file
                        writer.WriteLine(playerResult.Key.Name+" - "+timeString);
                    }
                    else{
                        writer.WriteLine(playerResult.Key.Name+" - "+playerResult.Value.Result);
                    }
                }
                //Empty line between matches
                writer.WriteLine();
            }
        }
    }
}