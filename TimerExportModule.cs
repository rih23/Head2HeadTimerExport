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
            if(data.NewDef != null){
                if(data.NewDef.State == MatchState.Completed){
                    using (StreamWriter writer = new StreamWriter(Settings.FilePath, append: true)){
                        MatchDefinition match = data.NewDef;
                        if(match.Result != null){
                            DateTime currentDateTime = DateTime.Now;
                            writer.WriteLine("-----------"+currentDateTime+"----------");
                            writer.WriteLine(match.MatchDisplayName);
                            writer.WriteLine("-----------------------");
                            Dictionary<PlayerID, MatchResultPlayer> results = match.Result.players;
                            foreach(KeyValuePair<PlayerID, MatchResultPlayer> playerResult in results){
                                if(playerResult.Value.Result == ResultCategory.Completed){
                                    //Converting the time from ticks to ms ss mm 
                                    //Padding the time values with zeros
                                    long timeTicks = playerResult.Value.FileTimeTotal/10000;
                                    string ms = (timeTicks % 1000).ToString();
                                    while(ms.Length < 3){
                                        ms = '0' + ms;
                                    }
                                    timeTicks /= 1000;
                                    string ss = (timeTicks % 60).ToString();
                                    while(ss.Length < 2){
                                        ss = '0' + ss;
                                    }
                                    timeTicks /= 60;
                                    string mm = (timeTicks % 100).ToString();
                                    while(mm.Length < 2){
                                        mm = '0' + mm;
                                    }
                                    string timeString = mm+":"+ss+"."+ms;
                                    writer.WriteLine(playerResult.Key.Name+" - "+timeString);
                                }
                                else{
                                    writer.WriteLine(playerResult.Key.Name+" - "+playerResult.Value.Result);
                                }
                            }
                            writer.WriteLine();
                        }
                    }
                }
            }
        }
    }
}