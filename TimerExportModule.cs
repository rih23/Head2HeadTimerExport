using System;
using System.IO;
using Celeste.Mod.Head2Head;
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
            //Doing nothing if the match update doesn't update to completed
            if(data.NewDef.State == MatchState.Completed){
                using (StreamWriter writer = new StreamWriter("./Head2Head-Times.txt", append: true)){
                    //All of the matches
                    Dictionary<string, MatchDefinition> matches = Head2HeadModule.knownMatches;
                    foreach(KeyValuePair<string, MatchDefinition> entry in matches){
                        if(entry.Value.Players.Count > 0 && entry.Value.State == MatchState.Completed){
                            DateTime currentDateTime = DateTime.Now;
                            writer.WriteLine("-----------"+currentDateTime+"----------");
                            writer.WriteLine(entry.Value.MatchDisplayName);
                            writer.WriteLine("-----------------------");
                            List<PlayerID> players = entry.Value.Players;
                            Dictionary<PlayerID, MatchResultPlayer> results = entry.Value.Result.players;
                            foreach(KeyValuePair<PlayerID, MatchResultPlayer> playerResult in results){
                                if(playerResult.Value.Result == ResultCategory.Completed){
                                    long timeTicks = playerResult.Value.FileTimeTotal/10000;
                                    string ms = (timeTicks % 1000).ToString();
                                    timeTicks /= 1000;
                                    string ss = (timeTicks % 60).ToString();
                                    timeTicks /= 60;
                                    string mm = (timeTicks % 100).ToString();
                                    string timeString = mm+":"+ss+"."+ms;
                                    writer.WriteLine(playerResult.Key.Name+" - "+timeString);
                                }
                                else{
                                    long timeStart = playerResult.Value.FileTimeStart;
                                    long timeEnd = playerResult.Value.FileTimeEnd;
                                    writer.WriteLine(playerResult.Key.Name+" - "+playerResult.Value.Result);
                                }
                                writer.WriteLine();
                            }
                        }
                    }
                }
            }
        }
    }
}